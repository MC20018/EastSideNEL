/*
<OxygenNEL>
Copyright (C) <2025>  <OxygenNEL>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
*/

using System;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;
using Serilog;

namespace EastSide.IRC.Packet;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 0x3D, EnumProtocolVersion.V1206, false)]
public class SPlayerInfoRemove : IPacket
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
        if (_rawBytes == null || _rawBytes.Length < 1) return false;

        var client = IrcManager.Get(connection);
        if (client == null) return false;

        var buf = Unpooled.WrappedBuffer(_rawBytes);
        try
        {
            int count = buf.ReadVarIntFromBuffer();
            for (int i = 0; i < count; i++)
            {
                var uuid = ReadUuid(buf);
                client.TabList.OnPlayerRemoved(uuid);
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "[IRC-TAB] 解析 PlayerInfoRemove 失败");
        }
        finally
        {
            buf.Release();
        }

        return false;
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
}
