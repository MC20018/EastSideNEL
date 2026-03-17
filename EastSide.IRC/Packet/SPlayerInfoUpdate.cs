/*
<OxygenNEL>
Copyright (C) <2025>  <OxygenNEL>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;
using EastSide.Core.Utils;
using Serilog;

namespace EastSide.IRC.Packet;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 0x3E, EnumProtocolVersion.V1206, false)]
public class SPlayerInfoUpdate : IPacket
{
    public EnumProtocolVersion ClientProtocolVersion { get; set; }

    private byte[]? _rawBytes;
    private byte[]? _modifiedBytes;

    static readonly ConcurrentDictionary<string, bool> _skinRequested = new();
    static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(120) };
    public static string SkinServerUrl { get; set; } = "https://api.fandmc.cn";
    public static string GameId { get; set; } = "4661334467366178884";

    public void ReadFromBuffer(IByteBuffer buffer)
    {
        _rawBytes = new byte[buffer.ReadableBytes];
        buffer.GetBytes(buffer.ReaderIndex, _rawBytes);
        buffer.SkipBytes(buffer.ReadableBytes);
    }

    public void WriteToBuffer(IByteBuffer buffer)
    {
        if (_modifiedBytes != null)
            buffer.WriteBytes(_modifiedBytes);
        else if (_rawBytes != null)
            buffer.WriteBytes(_rawBytes);
    }

    public bool HandlePacket(GameConnection connection)
    {
        if (_rawBytes == null || _rawBytes.Length < 2) return false;
        var client = IrcManager.Get(connection);
        if (client == null) return false;

        var tabList = client.TabList;
        var src = Unpooled.WrappedBuffer(_rawBytes);
        var dst = Unpooled.Buffer();
        bool modified = false;
        var uncached = new List<(string Name, Guid Uuid)>();

        try
        {
            byte actions = src.ReadByte();
            Log.Information("[Skin] PlayerInfoUpdate actions=0x{Actions:X2} rawLen={Len}", actions, _rawBytes.Length);
            if ((actions & 0x01) == 0) { src.Release(); dst.Release(); return false; }

            dst.WriteByte(actions);
            int count = src.ReadVarIntFromBuffer();
            dst.WriteVarInt(count);
            Log.Information("[Skin] Rebuilding packet: {Count} players, actions=0x{Actions:X2}", count, actions);

            for (int i = 0; i < count; i++)
            {
                int playerStart = dst.WriterIndex;

                // UUID
                var uuid = ReadUuid(src);
                WriteUuid(dst, uuid);

                // Add Player: Name
                string name = src.ReadStringFromBuffer(16);
                dst.WriteStringToBuffer(name);

                // Properties — 检查缓存，注入 textures
                int propCount = src.ReadVarIntFromBuffer();
                (string Value, string Signature) skin = default;
                bool hasSkin = !name.StartsWith("CIT-", StringComparison.OrdinalIgnoreCase)
                    && _skinCache.TryGetValue(name, out skin);

                if (hasSkin)
                {
                    dst.WriteVarInt(propCount + 1);
                    dst.WriteStringToBuffer("textures");
                    dst.WriteStringToBuffer(skin.Value);
                    dst.WriteBoolean(true);
                    dst.WriteStringToBuffer(skin.Signature);
                    modified = true;
                }
                else
                {
                    dst.WriteVarInt(propCount);
                    if (!name.StartsWith("CIT-", StringComparison.OrdinalIgnoreCase))
                        uncached.Add((name, uuid));
                }

                // 复制原始 properties
                for (int p = 0; p < propCount; p++)
                {
                    var pn = src.ReadStringFromBuffer(32767); dst.WriteStringToBuffer(pn);
                    var pv = src.ReadStringFromBuffer(32767); dst.WriteStringToBuffer(pv);
                    bool signed = src.ReadBoolean(); dst.WriteBoolean(signed);
                    if (signed) { var ps = src.ReadStringFromBuffer(32767); dst.WriteStringToBuffer(ps); }
                }

                // 其他 actions 原样复制
                if ((actions & 0x02) != 0)
                {
                    bool hasSig = src.ReadBoolean(); dst.WriteBoolean(hasSig);
                    if (hasSig)
                    {
                        CopyBytes(src, dst, 16 + 8);
                        int ks = src.ReadVarIntFromBuffer(); dst.WriteVarInt(ks); CopyBytes(src, dst, ks);
                        int ss = src.ReadVarIntFromBuffer(); dst.WriteVarInt(ss); CopyBytes(src, dst, ss);
                    }
                }
                if ((actions & 0x04) != 0) { dst.WriteVarInt(src.ReadVarIntFromBuffer()); }
                if ((actions & 0x08) != 0) { dst.WriteBoolean(src.ReadBoolean()); }
                if ((actions & 0x10) != 0) { dst.WriteVarInt(src.ReadVarIntFromBuffer()); }
                if ((actions & 0x20) != 0)
                {
                    bool hasDisp = src.ReadBoolean(); dst.WriteBoolean(hasDisp);
                    if (hasDisp)
                    {
                        Log.Information("[Skin] Player {Name} has DisplayName, copying NBT", name);
                        CopyNbt(src, dst);
                    }
                    else
                    {
                        Log.Information("[Skin] Player {Name} has NO DisplayName", name);
                    }
                }

                tabList.OnPlayerAdded(name, uuid);

                int playerEnd = dst.WriterIndex;
                var playerBytes = new byte[playerEnd - playerStart];
                dst.GetBytes(playerStart, playerBytes);
                _playerRawData[name] = playerBytes;
            }

            if (modified)
            {
                if (src.ReadableBytes > 0)
                    CopyBytes(src, dst, src.ReadableBytes);
                _modifiedBytes = new byte[dst.ReadableBytes];
                dst.GetBytes(dst.ReaderIndex, _modifiedBytes);
                Log.Information("[Skin] Rebuilt packet: original={OrigLen} modified={ModLen} remaining={Rem}",
                    _rawBytes.Length, _modifiedBytes.Length, 0);
            }
            else
            {
                var rem = src.ReadableBytes;
                if (rem > 0)
                    Log.Warning("[Skin] Unread bytes in original packet: {Rem}", rem);
            }

            if (uncached.Count > 0)
            {
                var self = connection.NickName;
                var sorted = uncached.OrderByDescending(p => p.Name == self).ToList();
                var conn = connection;
                _ = Task.Run(() => Parallel.ForEach(sorted, p => RequestSkinAsync(conn, p.Name, p.Uuid)));
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "[IRC-TAB] 解析/重建 PlayerInfoUpdate 失败，使用原始包");
            _modifiedBytes = null;
        }
        finally
        {
            src.Release();
            dst.Release();
        }

        return false;
    }

    static void CopyBytes(IByteBuffer src, IByteBuffer dst, int len)
    {
        var tmp = new byte[len];
        src.ReadBytes(tmp);
        dst.WriteBytes(tmp);
    }

    static void CopyNbt(IByteBuffer src, IByteBuffer dst)
    {
        byte t = src.ReadByte(); dst.WriteByte(t);
        CopyNbtPayload(src, dst, t);
    }

    static void CopyNbtPayload(IByteBuffer src, IByteBuffer dst, byte t)
    {
        switch (t)
        {
            case 0: break;
            case 1: CopyBytes(src, dst, 1); break;
            case 2: CopyBytes(src, dst, 2); break;
            case 3: CopyBytes(src, dst, 4); break;
            case 4: CopyBytes(src, dst, 8); break;
            case 5: CopyBytes(src, dst, 4); break;
            case 6: CopyBytes(src, dst, 8); break;
            case 7: { int l = src.ReadInt(); dst.WriteInt(l); CopyBytes(src, dst, l); break; }
            case 8: { int l = src.ReadUnsignedShort(); dst.WriteShort(l); CopyBytes(src, dst, l); break; }
            case 9:
                byte lt = src.ReadByte(); dst.WriteByte(lt);
                int ll = src.ReadInt(); dst.WriteInt(ll);
                for (int i = 0; i < ll; i++) CopyNbtPayload(src, dst, lt);
                break;
            case 10:
                while (true)
                {
                    byte ct = src.ReadByte(); dst.WriteByte(ct);
                    if (ct == 0) break;
                    int nl = src.ReadUnsignedShort(); dst.WriteShort(nl); CopyBytes(src, dst, nl);
                    CopyNbtPayload(src, dst, ct);
                }
                break;
            case 11: { int l = src.ReadInt(); dst.WriteInt(l); CopyBytes(src, dst, l * 4); break; }
            case 12: { int l = src.ReadInt(); dst.WriteInt(l); CopyBytes(src, dst, l * 8); break; }
        }
    }
    public static void SendDisplayNameUpdate(GameConnection conn, List<(Guid Uuid, string Username)> players)
    {
        try
        {
            var buffer = Unpooled.Buffer();
            buffer.WriteVarInt(0x3E);
            buffer.WriteByte(0x20);
            buffer.WriteVarInt(players.Count);
            foreach (var (uuid, username) in players)
            {
                WriteUuid(buffer, uuid);
                buffer.WriteBoolean(true);
                var nbt = TextComponentSerializer.Serialize(
                    new TextComponent { Text = $"§7[§bES§7] {username}" });
                buffer.WriteBytes(nbt);
                nbt.Release();
            }
            conn.ClientChannel?.WriteAndFlushAsync(buffer);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[IRC-TAB] SendDisplayNameUpdate 失败");
        }
    }

    public static void ClearDisplayName(GameConnection conn, List<Guid> uuids)
    {
        try
        {
            var buffer = Unpooled.Buffer();
            buffer.WriteVarInt(0x3E);
            buffer.WriteByte(0x20);
            buffer.WriteVarInt(uuids.Count);
            foreach (var uuid in uuids)
            {
                WriteUuid(buffer, uuid);
                buffer.WriteBoolean(false);
            }
            conn.ClientChannel?.WriteAndFlushAsync(buffer);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[IRC-TAB] ClearDisplayName 失败");
        }
    }

    static Guid ReadUuid(IByteBuffer buffer)
    {
        long most = buffer.ReadLong();
        long least = buffer.ReadLong();
        var b = new byte[16];
        b[3] = (byte)(most >> 56); b[2] = (byte)(most >> 48);
        b[1] = (byte)(most >> 40); b[0] = (byte)(most >> 32);
        b[5] = (byte)(most >> 24); b[4] = (byte)(most >> 16);
        b[7] = (byte)(most >> 8);  b[6] = (byte)most;
        b[8] = (byte)(least >> 56); b[9] = (byte)(least >> 48);
        b[10] = (byte)(least >> 40); b[11] = (byte)(least >> 32);
        b[12] = (byte)(least >> 24); b[13] = (byte)(least >> 16);
        b[14] = (byte)(least >> 8);  b[15] = (byte)least;
        return new Guid(b);
    }

    static void WriteUuid(IByteBuffer buffer, Guid uuid)
    {
        var b = uuid.ToByteArray();
        long most = ((long)b[3] << 56) | ((long)b[2] << 48) |
                    ((long)b[1] << 40) | ((long)b[0] << 32) |
                    ((long)b[5] << 24) | ((long)b[4] << 16) |
                    ((long)b[7] << 8)  | b[6];
        long least = ((long)b[8] << 56) | ((long)b[9] << 48) |
                     ((long)b[10] << 40) | ((long)b[11] << 32) |
                     ((long)b[12] << 24) | ((long)b[13] << 16) |
                     ((long)b[14] << 8)  | b[15];
        buffer.WriteLong(most);
        buffer.WriteLong(least);
    }

    static void SkipNbt(IByteBuffer buf)
    {
        SkipNbtPayload(buf, buf.ReadByte());
    }

    static void SkipNbtPayload(IByteBuffer buf, byte t)
    {
        switch (t)
        {
            case 0: break;
            case 1: buf.SkipBytes(1); break;
            case 2: buf.SkipBytes(2); break;
            case 3: buf.SkipBytes(4); break;
            case 4: buf.SkipBytes(8); break;
            case 5: buf.SkipBytes(4); break;
            case 6: buf.SkipBytes(8); break;
            case 7: buf.SkipBytes(buf.ReadInt()); break;
            case 8: buf.SkipBytes(buf.ReadUnsignedShort()); break;
            case 9:
                byte lt = buf.ReadByte(); int ll = buf.ReadInt();
                for (int i = 0; i < ll; i++) SkipNbtPayload(buf, lt);
                break;
            case 10:
                while (true) { byte ct = buf.ReadByte(); if (ct == 0) break;
                    buf.SkipBytes(buf.ReadUnsignedShort()); SkipNbtPayload(buf, ct); }
                break;
            case 11: buf.SkipBytes(buf.ReadInt() * 4); break;
            case 12: buf.SkipBytes(buf.ReadInt() * 8); break;
        }
    }

    static readonly ConcurrentDictionary<string, (string Value, string Signature)> _skinCache = new();
    static readonly ConcurrentDictionary<string, byte[]> _playerRawData = new();

    static void RequestSkinAsync(GameConnection conn, string name, Guid uuid)
    {
        if (name.StartsWith("CIT-", StringComparison.OrdinalIgnoreCase)) return;
        if (_skinCache.ContainsKey(name)) return;

        var key = $"{name}:{uuid}";
        if (!_skinRequested.TryAdd(key, true)) return;

        _ = Task.Run(async () =>
        {
            try
            {
                var lookup = IrcManager.SkinLookupProvider;
                if (lookup == null) return;

                var skinInfo = await lookup(name, conn.GameId);
                if (skinInfo == null) return;

                var (skinId, skinUrl, skinMode) = skinInfo.Value;
                var url = $"{SkinServerUrl}/skin?skinId={Uri.EscapeDataString(skinId)}&skinMode={skinMode}&skinUrl={Uri.EscapeDataString(skinUrl)}";
                var resp = await _http.GetAsync(url);
                if (!resp.IsSuccessStatusCode) return;

                var json = await resp.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var value = doc.RootElement.GetProperty("value").GetString();
                var signature = doc.RootElement.GetProperty("signature").GetString();
                if (value == null || signature == null) return;

                _skinCache[name] = (value, signature);
                Log.Information("[Skin] Cached skin for {Name}", name);
            }
            catch (Exception ex)
            {
                Log.Information("[Skin] Failed for {Name}: {Error}", name, ex.Message);
                _skinRequested.TryRemove(key, out _);
            }
        });
    }

    static void SendSkinWithFullData(GameConnection conn, string name, byte[] playerRaw, string value, string signature, byte actions)
    {
        // playerRaw 包含: UUID + Name + Properties(无textures) + 其他actions数据
        // 需要解析 playerRaw，在 properties 里注入 textures

        var src = Unpooled.WrappedBuffer(playerRaw);
        var buf = Unpooled.Buffer();
        try
        {
            buf.WriteVarInt(0x3E);
            buf.WriteByte(actions);
            buf.WriteVarInt(1);

            // UUID
            CopyBytes(src, buf, 16);

            // Name
            var n = src.ReadStringFromBuffer(16);
            buf.WriteStringToBuffer(n);

            // Properties: 读原始 count，写 count+1，注入 textures
            int propCount = src.ReadVarIntFromBuffer();
            buf.WriteVarInt(propCount + 1);

            buf.WriteStringToBuffer("textures");
            buf.WriteStringToBuffer(value);
            buf.WriteBoolean(true);
            buf.WriteStringToBuffer(signature);

            for (int p = 0; p < propCount; p++)
            {
                var pn = src.ReadStringFromBuffer(32767); buf.WriteStringToBuffer(pn);
                var pv = src.ReadStringFromBuffer(32767); buf.WriteStringToBuffer(pv);
                bool signed = src.ReadBoolean(); buf.WriteBoolean(signed);
                if (signed) { var ps = src.ReadStringFromBuffer(32767); buf.WriteStringToBuffer(ps); }
            }

            // 剩余的 action 数据原样复制
            if (src.ReadableBytes > 0)
                CopyBytes(src, buf, src.ReadableBytes);

            conn.ClientChannel?.WriteAndFlushAsync(buf);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "[Skin] SendSkinWithFullData failed for {Name}", name);
            buf.Release();
        }
        finally
        {
            src.Release();
        }
    }

    static void SendSkinProperty(GameConnection conn, Guid uuid, string name, string value, string signature)
    {
        Log.Information("[Skin] SendSkinProperty Remove+Add for {Name}", name);
        var removeBuf = Unpooled.Buffer();
        removeBuf.WriteVarInt(0x3D);
        removeBuf.WriteVarInt(1);
        WriteUuid(removeBuf, uuid);
        conn.ClientChannel?.WriteAndFlushAsync(removeBuf);

        var addBuf = Unpooled.Buffer();
        addBuf.WriteVarInt(0x3E);
        addBuf.WriteByte(0x01 | 0x08);
        addBuf.WriteVarInt(1);

        WriteUuid(addBuf, uuid);

        var n = name.Length > 16 ? name[..16] : name;
        addBuf.WriteStringToBuffer(n);
        addBuf.WriteVarInt(1);

        addBuf.WriteStringToBuffer("textures");
        addBuf.WriteStringToBuffer(value);
        addBuf.WriteBoolean(true);
        addBuf.WriteStringToBuffer(signature);

        addBuf.WriteBoolean(true);

        conn.ClientChannel?.WriteAndFlushAsync(addBuf);
    }
}
