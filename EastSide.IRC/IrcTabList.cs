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
using System.Linq;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using EastSide.IRC.Packet;
using Serilog;

namespace EastSide.IRC;

public class IrcTabList
{
    readonly Dictionary<string, string> _ircPlayers = new();
    readonly Dictionary<string, Guid> _knownUuids = new();
    readonly HashSet<Guid> _marked = new();

    public void OnPlayerAdded(string name, Guid uuid)
    {
        _knownUuids[name] = uuid;
    }

    public void OnPlayerRemoved(Guid uuid)
    {
        var name = _knownUuids.FirstOrDefault(kv => kv.Value == uuid).Key;
        if (name != null) _knownUuids.Remove(name);
        _marked.Remove(uuid);
    }

    public void UpdateIrcList(List<(string RoleId, string Username)> entries)
    {
        _ircPlayers.Clear();
        foreach (var (roleId, username) in entries)
            _ircPlayers[roleId] = username;
    }

    public void Tick(GameConnection conn)
    {
        if (conn.ProtocolVersion != EnumProtocolVersion.V1206) return;
        if (conn.State != EnumConnectionState.Play) return;

        var toMark = new List<(Guid Uuid, string Username)>();
        foreach (var (roleId, username) in _ircPlayers)
        {
            if (_knownUuids.TryGetValue(roleId, out var uuid) && !_marked.Contains(uuid))
            {
                _marked.Add(uuid);
                toMark.Add((uuid, username));
            }
        }

        var toUnmark = new List<Guid>();
        foreach (var uuid in _marked)
        {
            var name = _knownUuids.FirstOrDefault(kv => kv.Value == uuid).Key;
            if (name == null || !_ircPlayers.ContainsKey(name))
                toUnmark.Add(uuid);
        }
        foreach (var uuid in toUnmark)
            _marked.Remove(uuid);

        if (toMark.Count > 0)
            SPlayerInfoUpdate.SendDisplayNameUpdate(conn, toMark);
        if (toUnmark.Count > 0)
            SPlayerInfoUpdate.ClearDisplayName(conn, toUnmark);
    }

    public void Clear(GameConnection conn)
    {
        if (_marked.Count > 0 && conn.ProtocolVersion == EnumProtocolVersion.V1206
            && conn.State == EnumConnectionState.Play)
        {
            SPlayerInfoUpdate.ClearDisplayName(conn, _marked.ToList());
        }
        _ircPlayers.Clear();
        _knownUuids.Clear();
        _marked.Clear();
    }
}
