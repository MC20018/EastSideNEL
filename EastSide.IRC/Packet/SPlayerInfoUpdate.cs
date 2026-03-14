/*
<OxygenNEL>
Copyright (C) <2025>  <OxygenNEL>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
*/

using System;
using System.Collections.Generic;
using System.Text;
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

    public void ReadFromBuffer(IByteBuffer buffer)
    {
        _rawBytes = new byte[buffer.ReadableBytes];
        buffer.GetBytes(buffer.ReaderIndex, _rawBytes);
        buffer.SkipBytes(buffer.ReadableBytes);
    }

    public void WriteToBuffer(IByteBuffer buffer)
    {
        if (_rawBytes != null)
            buffer.WriteBytes(_rawBytes);
    }

    public bool HandlePacket(GameConnection connection)
    {
        if (_rawBytes == null || _rawBytes.Length < 2) return false;
        var client = IrcManager.Get(connection);
        if (client == null) return false;

        var tabList = client.TabList;
        var buf = Unpooled.WrappedBuffer(_rawBytes);
        try
        {
            byte actions = buf.ReadByte();
            if ((actions & 0x01) == 0) return false;

            int count = buf.ReadVarIntFromBuffer();
            for (int i = 0; i < count; i++)
            {
                var uuid = ReadUuid(buf);

                string name = buf.ReadStringFromBuffer(16);
                int propCount = buf.ReadVarIntFromBuffer();
                for (int p = 0; p < propCount; p++)
                {
                    buf.ReadStringFromBuffer(32767);
                    buf.ReadStringFromBuffer(32767);
                    if (buf.ReadBoolean()) buf.ReadStringFromBuffer(32767);
                }

                if ((actions & 0x02) != 0)
                {
                    if (buf.ReadBoolean())
                    {
                        buf.SkipBytes(16 + 8);
                        buf.SkipBytes(buf.ReadVarIntFromBuffer());
                        buf.SkipBytes(buf.ReadVarIntFromBuffer());
                    }
                }

                if ((actions & 0x04) != 0) buf.ReadVarIntFromBuffer();
                if ((actions & 0x08) != 0) buf.ReadBoolean();
                if ((actions & 0x10) != 0) buf.ReadVarIntFromBuffer();
                if ((actions & 0x20) != 0 && buf.ReadBoolean()) SkipNbt(buf);

                tabList.OnPlayerAdded(name, uuid);
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "[IRC-TAB] 解析 PlayerInfoUpdate 失败");
        }
        finally
        {
            buf.Release();
        }

        return false;
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
}
