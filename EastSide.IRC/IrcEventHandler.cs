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
using DotNetty.Buffers;
using EastSide.IRC.Packet;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Event;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Manager;
using Codexus.Development.SDK.Utils;
using Serilog;

namespace EastSide.IRC;

public static class IrcEventHandler
{
    static readonly ConcurrentDictionary<GameConnection, bool> _processed = new();
    static readonly ConcurrentDictionary<string, bool> _southsideChannels = new();

    public static void MarkSouthside(string localAddress)
    {
        _southsideChannels[localAddress] = true;
        Log.Information("[IRC] 标记 SOUTHSIDE 通道: {Address}", localAddress);
    }

    public static bool IsSouthside(string localAddress)
    {
        return _southsideChannels.ContainsKey(localAddress);
    }

    public static void Register(Func<string> tokenProvider)
    {
        IrcManager.TokenProvider = tokenProvider;
        IrcManager.OnClientRemoved = conn => _processed.TryRemove(conn, out _);

        foreach (var channel in MessageChannels.AllVersions)
        {
            EventManager.Instance.RegisterHandler<EventLoginSuccess>(channel, OnLoginSuccess);
        }

        EventManager.Instance.RegisterHandler<EventConnectionClosed>("channel_connection", OnConnectionClosed);
    }

    public static Func<Guid, string?>? LocalAddressLookup { get; set; }

    static void OnLoginSuccess(EventLoginSuccess args)
    {
        var nickName = args.Connection.NickName;
        if (string.IsNullOrEmpty(nickName)) return;

        if (!_processed.TryAdd(args.Connection, true)) return;

        var client = IrcManager.GetOrCreate(args.Connection);
        client.ChatReceived += OnChatReceived;

        var tag = "";
        var localAddr = LocalAddressLookup?.Invoke(args.Connection.InterceptorId);
        if (!string.IsNullOrEmpty(localAddr) && IsSouthside(localAddr))
            tag = "SOUTHSIDE";

        client.Start(nickName, tag);
    }

    static void OnConnectionClosed(EventConnectionClosed args)
    {
        IrcManager.Remove(args.Connection);
    }

    static void OnChatReceived(object? sender, IrcChatEventArgs e)
    {
        if (sender is not IrcClient client) return;
        CChatCommandIrc.SendLocalMessage(client.Connection, e.Message);
    }
}
