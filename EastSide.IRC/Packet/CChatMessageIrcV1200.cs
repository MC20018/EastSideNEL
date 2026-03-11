/*
<OxygenNEL>
Copyright (C) <2025>  <OxygenNEL>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
*/

using System;
using DotNetty.Buffers;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using Serilog;

namespace EastSide.IRC.Packet;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ServerBound, 5, EnumProtocolVersion.V1200, false)]
public class CChatMessageIrcV1200 : IPacket
{
    public EnumProtocolVersion ClientProtocolVersion { get; set; }

    private byte[]? _rawBytes;
    private string _message = string.Empty;
    private bool _isIrcCommand;

    public void ReadFromBuffer(IByteBuffer buffer)
    {
        _rawBytes = new byte[buffer.ReadableBytes];
        buffer.GetBytes(buffer.ReaderIndex, _rawBytes);

        _message = buffer.ReadStringFromBuffer(256);
        buffer.SkipBytes(buffer.ReadableBytes);

        _isIrcCommand = _message.StartsWith("/irc ", StringComparison.OrdinalIgnoreCase)
                     || _message.Equals("/irc", StringComparison.OrdinalIgnoreCase);
    }

    public void WriteToBuffer(IByteBuffer buffer)
    {
        if (_isIrcCommand) return;

        if (_rawBytes != null)
            buffer.WriteBytes(_rawBytes);
    }

    public bool HandlePacket(GameConnection connection)
    {
        if (!_isIrcCommand) return false;
        
        var content = _message.Length > 5 ? _message.Substring(5).Trim() : string.Empty;

        if (string.IsNullOrWhiteSpace(content))
        {
            CChatCommandIrcV1200.SendLocalMessage(connection, "§e[IRC] 用法: /irc <消息>");
            return true;
        }

        var playerName = connection.NickName;
        if (string.IsNullOrEmpty(playerName))
        {
            CChatCommandIrcV1200.SendLocalMessage(connection, "§c[IRC] 未登录");
            return true;
        }

        var ircClient = IrcManager.Get(connection);
        if (ircClient == null)
        {
            CChatCommandIrcV1200.SendLocalMessage(connection, "§c[IRC] IRC 未连接");
            return true;
        }
        ircClient.SendChat(playerName, content);
        return true;
    }
}
