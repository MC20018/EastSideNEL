using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DotNetty.Buffers;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Entities;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using EastSide.Manager;
using EastSide.Type;
using EastSide.Core.Utils;
using EastSide.Utils;
using Serilog;

namespace EastSide.Packet;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 0x40, EnumProtocolVersion.V1206, false)]
public class SSynchronizePlayerPosition : IPacket
{
    private static readonly ConcurrentDictionary<Guid, bool> _detected = new();

    public EnumProtocolVersion ClientProtocolVersion { get; set; }

    private double X { get; set; }
    private double Y { get; set; }
    private double Z { get; set; }
    private float Yaw { get; set; }
    private float Pitch { get; set; }
    private byte Flags { get; set; }
    private int TeleportId { get; set; }

    private byte[]? _raw;

    public void ReadFromBuffer(IByteBuffer buffer)
    {
        _raw = new byte[buffer.ReadableBytes];
        buffer.GetBytes(buffer.ReaderIndex, _raw);

        X = buffer.ReadDouble();
        Y = buffer.ReadDouble();
        Z = buffer.ReadDouble();
        Yaw = buffer.ReadFloat();
        Pitch = buffer.ReadFloat();
        Flags = buffer.ReadByte();
        TeleportId = buffer.ReadVarIntFromBuffer();
    }

    public void WriteToBuffer(IByteBuffer buffer)
    {
        if (_raw != null) buffer.WriteBytes(_raw);
    }

    public bool HandlePacket(GameConnection connection)
    {
        if (!IsBlackRoom()) return false;

        var id = connection.InterceptorId;
        if (!_detected.TryAdd(id, true)) return false;

        Log.Warning("[小黑屋] 检测到封禁特征: X={X} Y={Y} Z={Z}", X,Y,Z);
        HandleBan(connection);
        return false;
    }

    private bool IsBlackRoom()
    {
        if ((Flags & 0x07) != 0) return false;
        
        return X >= 12 && X <= 13 &&
               Y >= -60 && Y <= -58 &&
               Z >= 10 && Z <= 11;
    }

    private void HandleBan(GameConnection connection)
    {
        var autoAction = SettingManager.Instance.Get().AutoDisconnectOnBan;

        BanRecordManager.Instance.AddBan(new BanEntry
        {
            UserId = connection.Session.UserId,
            ServerId = connection.GameId,
            RoleName = connection.NickName,
            Reason = "小黑屋",
            BanTime = DateTime.Now,
            UnbanTime = null,
            IsPermanent = true
        });

        if (autoAction == "none") return;

        SendDisconnect(connection, "此账号是封号的账号");

        if (autoAction == "close")
        {
            var interceptorId = connection.InterceptorId;
            _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                Log.Warning("[小黑屋] 正在关闭通道...");
                GameManager.Instance.ShutdownInterceptor(interceptorId);
                _detected.TryRemove(interceptorId, out _);
                NotificationHelper.ShowSuccess("检测到小黑屋封禁,已成功关闭通道");
            });
        }
        else if (autoAction == "switch")
        {
            var interceptorId = connection.InterceptorId;
            var userId = connection.Session.UserId;
            var userToken = connection.Session.UserToken;
            var serverId = connection.GameId;
            var currentRole = connection.NickName;

            var interceptor = GameManager.Instance.GetInterceptor(interceptorId);
            var serverName = interceptor?.ServerName ?? string.Empty;

            var settings = SettingManager.Instance.Get();
            var socks5 = settings.Socks5Enabled ? new EntitySocks5
            {
                Enabled = true,
                Address = settings.Socks5Address,
                Port = settings.Socks5Port,
                Username = settings.Socks5Username,
                Password = settings.Socks5Password
            } : new EntitySocks5();

            _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                Log.Warning("[小黑屋] 检测到封禁，正在关闭通道并切换角色...");
                GameManager.Instance.ShutdownInterceptor(interceptorId);
                _detected.TryRemove(interceptorId, out _);

                await BannedRoleTracker.TrySwitchToAnotherRole(
                    userId,
                    userToken,
                    serverId,
                    serverName,
                    currentRole,
                    socks5);
            });
        }
    }

    private static void SendDisconnect(GameConnection connection, string reason)
    {
        var disconnect = new SPlayDisconnect
        {
            Reason = new TextComponent { Text = reason, Color = "red" },
            ClientProtocolVersion = connection.ProtocolVersion
        };
        connection.ClientChannel.WriteAndFlushAsync(disconnect);
    }

    public static void ClearDetection(Guid interceptorId)
    {
        _detected.TryRemove(interceptorId, out _);
    }
}
