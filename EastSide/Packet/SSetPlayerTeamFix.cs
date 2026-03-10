
using System;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;
using Serilog;

namespace EastSide.Packet;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 0x60, EnumProtocolVersion.V1206, false)]
public class SSetPlayerTeamFix : IPacket
{
    public string TeamName { get; set; } = string.Empty;
    public byte Mode { get; set; }
    public TeamInfo? Info { get; set; }
    public string[] Entities { get; set; } = Array.Empty<string>();

    public EnumProtocolVersion ClientProtocolVersion { get; set; }

    public void ReadFromBuffer(IByteBuffer buffer)
    {
        try
        {
            TeamName = buffer.ReadStringFromBuffer(32767);
            Mode = buffer.ReadByte();
            if (Mode == 0 || Mode == 2)
            {
                Info = ReadTeamInfo(buffer);
            }

            if (Mode == 0 || Mode == 3 || Mode == 4)
            {
                var count = buffer.ReadVarIntFromBuffer();
                Entities = new string[count];
                for (int i = 0; i < count; i++)
                {
                    Entities[i] = buffer.ReadStringFromBuffer(32767);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "[SSetPlayerTeamFix] Error reading team packet for team '{TeamName}', Mode={Mode}", TeamName, Mode);
        }
    }

    public void WriteToBuffer(IByteBuffer buffer)
    {
        buffer.WriteStringToBuffer(TeamName);
        buffer.WriteByte(Mode);

        if (Mode == 0 || Mode == 2)
        {
            WriteTeamInfo(buffer, Info!);
        }

        if (Mode == 0 || Mode == 3 || Mode == 4)
        {
            buffer.WriteVarInt(Entities.Length);
            foreach (var entity in Entities)
            {
                buffer.WriteStringToBuffer(entity);
            }
        }
    }

    public bool HandlePacket(GameConnection connection)
    {
        if (Mode == 4 && Entities.Length > 0)
        {
            Log.Debug("[SSetPlayerTeamFix] Fixed remove entities from team '{TeamName}', cleared {Count} entities", 
                TeamName, Entities.Length);
            
            Entities = Array.Empty<string>();
        }

        return true;
    }

    private TeamInfo ReadTeamInfo(IByteBuffer buffer)
    {
        var info = new TeamInfo
        {
            DisplayName = buffer.ReadStringFromBuffer(short.MaxValue),
            Flags = buffer.ReadByte(),
            NameTagVisibility = buffer.ReadStringFromBuffer(40),
            CollisionRule = buffer.ReadStringFromBuffer(40),
            Color = buffer.ReadVarIntFromBuffer(),
            Prefix = buffer.ReadStringFromBuffer(short.MaxValue),
            Suffix = buffer.ReadStringFromBuffer(short.MaxValue)
        };
        return info;
    }

    private void WriteTeamInfo(IByteBuffer buffer, TeamInfo info)
    {
        buffer.WriteStringToBuffer(info.DisplayName);
        buffer.WriteByte(info.Flags);
        buffer.WriteStringToBuffer(info.NameTagVisibility);
        buffer.WriteStringToBuffer(info.CollisionRule);
        buffer.WriteVarInt(info.Color);
        buffer.WriteStringToBuffer(info.Prefix);
        buffer.WriteStringToBuffer(info.Suffix);
    }

    public class TeamInfo
    {
        public string DisplayName { get; set; } = string.Empty;
        public byte Flags { get; set; }
        public string NameTagVisibility { get; set; } = string.Empty;
        public string CollisionRule { get; set; } = string.Empty;
        public int Color { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
    }
}
