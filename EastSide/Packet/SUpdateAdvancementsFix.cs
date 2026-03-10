
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;
using EastSide.Core.Utils;
using Serilog;

namespace EastSide.Packet;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 0x4D, EnumProtocolVersion.V1122, false)]
public class SUpdateAdvancementsFix : IPacket
{
    public bool Reset { get; set; }
    public EntityAdvancementEntry[] Advancements { get; set; } = Array.Empty<EntityAdvancementEntry>();
    public string[] RemovedAdvancements { get; set; } = Array.Empty<string>();
    public EntityAdvancementProgressEntry[] Progress { get; set; } = Array.Empty<EntityAdvancementProgressEntry>();

    public EnumProtocolVersion ClientProtocolVersion { get; set; }

    public void ReadFromBuffer(IByteBuffer buffer)
    {
        Reset = buffer.ReadBoolean();
        
        var advancementsCount = buffer.ReadVarIntFromBuffer();
        Advancements = new EntityAdvancementEntry[advancementsCount];
        for (int i = 0; i < advancementsCount; i++)
        {
            var advancementId = ReadIdentifier(buffer);
            advancementId = SanitizeNamespacedKey(advancementId);
            
            var advancement = ReadAdvancement(buffer);
            
            Advancements[i] = new EntityAdvancementEntry
            {
                Id = advancementId,
                Advancement = advancement
            };
        }
        
        var removedCount = buffer.ReadVarIntFromBuffer();
        RemovedAdvancements = new string[removedCount];
        for (int i = 0; i < removedCount; i++)
        {
            var removedId = ReadIdentifier(buffer);
            removedId = SanitizeNamespacedKey(removedId);
            RemovedAdvancements[i] = removedId;
        }
        
        var progressCount = buffer.ReadVarIntFromBuffer();
        Progress = new EntityAdvancementProgressEntry[progressCount];
        for (int i = 0; i < progressCount; i++)
        {
            var advancementId = ReadIdentifier(buffer);
            advancementId = SanitizeNamespacedKey(advancementId);
            
            var advancementProgress = ReadAdvancementProgress(buffer);
            
            Progress[i] = new EntityAdvancementProgressEntry
            {
                Id = advancementId,
                Progress = advancementProgress
            };
        }
    }

    public void WriteToBuffer(IByteBuffer buffer)
    {
        buffer.WriteBoolean(Reset);
        
        buffer.WriteVarInt(Advancements.Length);
        foreach (var entry in Advancements)
        {
            WriteIdentifier(buffer, entry.Id);
            WriteAdvancement(buffer, entry.Advancement);
        }
        
        buffer.WriteVarInt(RemovedAdvancements.Length);
        foreach (var removed in RemovedAdvancements)
        {
            WriteIdentifier(buffer, removed);
        }
        
        buffer.WriteVarInt(Progress.Length);
        foreach (var entry in Progress)
        {
            WriteIdentifier(buffer, entry.Id);
            WriteAdvancementProgress(buffer, entry.Progress);
        }
    }

    public bool HandlePacket(GameConnection connection)
    {
        Log.Debug("[Advancements] Received advancements update - Reset: {Reset}, Count: {Count}", Reset, Advancements.Length);
        return true;
    }

    private string ReadIdentifier(IByteBuffer buffer)
    {
        var length = buffer.ReadVarIntFromBuffer();
        
        if (length < 0 || length > buffer.ReadableBytes)
        {
            throw new IndexOutOfRangeException($"Invalid string length: {length}, readable bytes: {buffer.ReadableBytes}");
        }
        
        var bytes = new byte[length];
        buffer.ReadBytes(bytes);
        var str = System.Text.Encoding.UTF8.GetString(bytes);
        
        return SanitizeLocationString(str);
    }

    private EntityAdvancement ReadAdvancement(IByteBuffer buffer)
    {
        var adv = new EntityAdvancement();

        var hasParent = buffer.ReadBoolean();
        if (hasParent)
        {
            var parentId = ReadIdentifier(buffer);
            parentId = SanitizeNamespacedKey(parentId);
            adv.ParentId = parentId;
        }

        var hasDisplay = buffer.ReadBoolean();
        if (hasDisplay)
        {
            adv.DisplayData = ReadAdvancementDisplay(buffer);
        }

        var criteriaCount = buffer.ReadVarIntFromBuffer();
        adv.Criteria = new string[criteriaCount];
        for (int i = 0; i < criteriaCount; i++)
        {
            var criterionId = ReadIdentifier(buffer);
            criterionId = SanitizeNamespacedKey(criterionId);
            adv.Criteria[i] = criterionId;
        }

        var requirementsArrayLength = buffer.ReadVarIntFromBuffer();
        adv.Requirements = new string[requirementsArrayLength][];
        for (int i = 0; i < requirementsArrayLength; i++)
        {
            var requirementLength = buffer.ReadVarIntFromBuffer();
            var requirementArray = new string[requirementLength];
            for (int j = 0; j < requirementLength; j++)
            {
                var requirement = ReadIdentifier(buffer);
                requirement = SanitizeNamespacedKey(requirement);
                requirementArray[j] = requirement;
            }
            adv.Requirements[i] = requirementArray;
        }

        return adv;
    }

    private EntityAdvancementDisplay ReadAdvancementDisplay(IByteBuffer buffer)
    {
        var display = new EntityAdvancementDisplay();

        display.Title = TextComponentSerializer.Deserialize(buffer);

        display.Description = TextComponentSerializer.Deserialize(buffer);

        display.Icon = ReadSlot(buffer);

        display.FrameType = (EnumAdvancementFrameType)buffer.ReadVarIntFromBuffer();

        var flags = buffer.ReadInt();
        
        if ((flags & 0x1) != 0)
        {
            var backgroundTexture = ReadIdentifier(buffer);
            backgroundTexture = SanitizeNamespacedKey(backgroundTexture);
            display.BackgroundTexture = backgroundTexture;
        }

        display.XCoord = buffer.ReadFloat();

        display.YCoord = buffer.ReadFloat();

        display.ShowToast = (flags & 0x2) != 0;
        display.Hidden = (flags & 0x4) != 0;

        return display;
    }

    private EntityItemStack ReadSlot(IByteBuffer buffer)
    {
        var present = buffer.ReadBoolean();
        if (!present)
            return new EntityItemStack { Present = false };

        var itemId = buffer.ReadVarIntFromBuffer();
        var itemCount = buffer.ReadByte();
        
        if (!buffer.IsReadable())
        {
            return new EntityItemStack
            {
                Present = true,
                ItemId = itemId,
                ItemCount = itemCount,
                Nbt = null
            };
        }
        
        var nbt = ReadNbt(buffer);

        return new EntityItemStack
        {
            Present = true,
            ItemId = itemId,
            ItemCount = itemCount,
            Nbt = nbt
        };
    }

    private object? ReadNbt(IByteBuffer buffer)
    {
        var marker = buffer.MarkReaderIndex();
        
        try
        {
            var firstByte = buffer.ReadByte();
            
            if (firstByte == 0)
            {
                return new NbtCompound();
            }
            
            buffer.ResetReaderIndex();
            
            if (buffer.IsReadable())
            {
                return ReadNbtTag(buffer);
            }
            
            return null;
        }
        catch
        {
            buffer.ResetReaderIndex();
            return null;
        }
    }
    
    private object ReadNbtTag(IByteBuffer buffer)
    {
        var tagType = buffer.ReadByte();
        
        switch (tagType)
        {
            case 0:
                return new NbtEnd();
            case 1:
                return new NbtByte(buffer.ReadByte());
            case 2:
                return new NbtShort(buffer.ReadShort());
            case 3:
                return new NbtInt(buffer.ReadInt());
            case 4:
                return new NbtLong(buffer.ReadLong());
            case 5:
                return new NbtFloat(buffer.ReadFloat());
            case 6:
                return new NbtDouble(buffer.ReadDouble());
            case 7:
                var byteArrayLength = buffer.ReadInt();
                
                if (byteArrayLength < 0 || byteArrayLength > buffer.ReadableBytes)
                {
                    throw new IndexOutOfRangeException($"Invalid byte array length in NBT: {byteArrayLength}, readable bytes: {buffer.ReadableBytes}");
                }
                
                var byteArray = new byte[byteArrayLength];
                buffer.ReadBytes(byteArray);
                return new NbtByteArray(byteArray);
            case 8:
                var stringLength = buffer.ReadUnsignedShort();
                
                if (stringLength < 0 || stringLength > buffer.ReadableBytes)
                {
                    throw new IndexOutOfRangeException($"Invalid string length in NBT: {stringLength}, readable bytes: {buffer.ReadableBytes}");
                }
                
                var stringBytes = new byte[stringLength];
                buffer.ReadBytes(stringBytes);
                var stringValue = System.Text.Encoding.UTF8.GetString(stringBytes);
                return new NbtString(stringValue);
            case 9:
                var listTagType = buffer.ReadByte();
                var listLength = buffer.ReadInt();
                
                if (listLength < 0 || listLength > 10000) 
                {
                    throw new IndexOutOfRangeException($"Invalid list length in NBT: {listLength}");
                }
                
                var listElements = new object[listLength];
                for (int i = 0; i < listLength; i++)
                {
                    listElements[i] = ReadNbtTagByType(buffer, listTagType);
                }
                return new NbtList(listTagType, listElements);
            case 10:
                var compound = new NbtCompound();
                
                int tagCount = 0;
                const int maxTags = 10000;
                
                while (true)
                {
                    if (tagCount >= maxTags)
                    {
                        throw new IndexOutOfRangeException($"Too many tags in NBT compound: exceeded {maxTags}");
                    }
                    
                    var childTagType = buffer.ReadByte();
                    if (childTagType == 0)
                        break;
                        
                    var tagNameLength = buffer.ReadUnsignedShort();
                    
                    if (tagNameLength < 0 || tagNameLength > buffer.ReadableBytes)
                    {
                        throw new IndexOutOfRangeException($"Invalid tag name length in NBT: {tagNameLength}, readable bytes: {buffer.ReadableBytes}");
                    }
                    
                    var tagNameBytes = new byte[tagNameLength];
                    buffer.ReadBytes(tagNameBytes);
                    var tagName = System.Text.Encoding.UTF8.GetString(tagNameBytes);
                    
                    var tagValue = ReadNbtTagByType(buffer, childTagType);
                    compound.Tags.Add(new KeyValuePair<string, object>(tagName, tagValue));
                    
                    tagCount++;
                }
                return compound;
            case 11:
                var intArrayLength = buffer.ReadInt();
                
                if (intArrayLength < 0 || intArrayLength * sizeof(int) > buffer.ReadableBytes)
                {
                    throw new IndexOutOfRangeException($"Invalid int array length in NBT: {intArrayLength}, readable bytes: {buffer.ReadableBytes}");
                }
                
                var intArray = new int[intArrayLength];
                for (int i = 0; i < intArrayLength; i++)
                {
                    intArray[i] = buffer.ReadInt();
                }
                return new NbtIntArray(intArray);
            default:
                throw new ArgumentException($"Unknown NBT tag type: {tagType}");
        }
    }
    
    private object ReadNbtTagByType(IByteBuffer buffer, byte tagType)
    {
        var tempBuffer = buffer.Duplicate();
        tempBuffer.WriteByte(tagType);
        return ReadNbtTag(tempBuffer);
    }

    private EntityAdvancementProgress ReadAdvancementProgress(IByteBuffer buffer)
    {
        var progress = new EntityAdvancementProgress();
        
        var size = buffer.ReadVarIntFromBuffer();
        progress.Criteria = new EntityCriterionProgress[size];
        
        for (int i = 0; i < size; i++)
        {
            var criterionId = ReadIdentifier(buffer);
            criterionId = SanitizeNamespacedKey(criterionId);
            
            var criterionProgress = ReadCriterionProgress(buffer);
            
            progress.Criteria[i] = new EntityCriterionProgress
            {
                CriterionId = criterionId,
                Progress = criterionProgress
            };
        }
        
        return progress;
    }

    private EntityCriterionProgressData ReadCriterionProgress(IByteBuffer buffer)
    {
        var progressData = new EntityCriterionProgressData();
        
        progressData.Achieved = buffer.ReadBoolean();
        
        if (progressData.Achieved)
        {
            progressData.DateOfAchieving = buffer.ReadLong();
        }
        
        return progressData;
    }

    private void WriteIdentifier(IByteBuffer buffer, string identifier)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(identifier);
        buffer.WriteVarInt(bytes.Length);
        buffer.WriteBytes(bytes);
    }

    private void WriteAdvancement(IByteBuffer buffer, EntityAdvancement advancement)
    {
        buffer.WriteBoolean(!string.IsNullOrEmpty(advancement.ParentId));
        if (!string.IsNullOrEmpty(advancement.ParentId))
        {
            WriteIdentifier(buffer, advancement.ParentId);
        }

        buffer.WriteBoolean(advancement.DisplayData != null);
        if (advancement.DisplayData != null)
        {
            WriteAdvancementDisplay(buffer, advancement.DisplayData);
        }

        buffer.WriteVarInt(advancement.Criteria?.Length ?? 0);
        if (advancement.Criteria != null)
        {
            foreach (var criterion in advancement.Criteria)
            {
                WriteIdentifier(buffer, criterion);
            }
        }

        var requirementsLength = advancement.Requirements?.Length ?? 0;
        buffer.WriteVarInt(requirementsLength);
        if (advancement.Requirements != null)
        {
            foreach (var requirementArray in advancement.Requirements)
            {
                buffer.WriteVarInt(requirementArray?.Length ?? 0);
                if (requirementArray != null)
                {
                    foreach (var requirement in requirementArray)
                    {
                        WriteIdentifier(buffer, requirement);
                    }
                }
            }
        }
    }

    private void WriteAdvancementDisplay(IByteBuffer buffer, EntityAdvancementDisplay display)
    {
        var serializedTitle = TextComponentSerializer.Serialize(display.Title);
        buffer.WriteBytes(serializedTitle);
        
        var serializedDescription = TextComponentSerializer.Serialize(display.Description);
        buffer.WriteBytes(serializedDescription);
        
        WriteSlot(buffer, display.Icon);
        
        buffer.WriteVarInt((int)display.FrameType);
        
        var flags = 0;
        if (!string.IsNullOrEmpty(display.BackgroundTexture)) flags |= 0x1;
        if (display.ShowToast) flags |= 0x2;
        if (display.Hidden) flags |= 0x4;
        
        buffer.WriteInt(flags);
        
        if (!string.IsNullOrEmpty(display.BackgroundTexture))
        {
            WriteIdentifier(buffer, display.BackgroundTexture);
        }
        
        buffer.WriteFloat(display.XCoord);
        
        buffer.WriteFloat(display.YCoord);
    }

    private void WriteSlot(IByteBuffer buffer, EntityItemStack itemStack)
    {
        buffer.WriteBoolean(itemStack.Present);
        if (!itemStack.Present) return;

        buffer.WriteVarInt(itemStack.ItemId);
        buffer.WriteByte(itemStack.ItemCount);
        
        if (itemStack.Nbt != null)
        {
            WriteNbtTag(buffer, itemStack.Nbt);
        }
        else
        {
            buffer.WriteByte(0);
        }
    }
    
    private void WriteNbtTag(IByteBuffer buffer, object nbtTag)
    {
        if (nbtTag is NbtEnd)
        {
            buffer.WriteByte(0);
        }
        else if (nbtTag is NbtByte nbtByte)
        {
            buffer.WriteByte(1);
            buffer.WriteByte(nbtByte.Value);
        }
        else if (nbtTag is NbtShort nbtShort)
        {
            buffer.WriteByte(2);
            buffer.WriteShort(nbtShort.Value);
        }
        else if (nbtTag is NbtInt nbtInt)
        {
            buffer.WriteByte(3);
            buffer.WriteInt(nbtInt.Value);
        }
        else if (nbtTag is NbtLong nbtLong)
        {
            buffer.WriteByte(4);
            buffer.WriteLong(nbtLong.Value);
        }
        else if (nbtTag is NbtFloat nbtFloat)
        {
            buffer.WriteByte(5);
            buffer.WriteFloat(nbtFloat.Value);
        }
        else if (nbtTag is NbtDouble nbtDouble)
        {
            buffer.WriteByte(6);
            buffer.WriteDouble(nbtDouble.Value);
        }
        else if (nbtTag is NbtByteArray nbtByteArray)
        {
            buffer.WriteByte(7);
            buffer.WriteInt(nbtByteArray.Value.Length);
            buffer.WriteBytes(nbtByteArray.Value);
        }
        else if (nbtTag is NbtString nbtString)
        {
            buffer.WriteByte(8);
            var stringBytes = System.Text.Encoding.UTF8.GetBytes(nbtString.Value);
            buffer.WriteShort(stringBytes.Length);
            buffer.WriteBytes(stringBytes);
        }
        else if (nbtTag is NbtList nbtList)
        {
            buffer.WriteByte(9);
            buffer.WriteByte(nbtList.TagType);
            buffer.WriteInt(nbtList.Elements.Length);
            foreach (var element in nbtList.Elements)
            {
                WriteNbtTag(buffer, element);
            }
        }
        else if (nbtTag is NbtCompound nbtCompound)
        {
            buffer.WriteByte(10);
            foreach (var tag in nbtCompound.Tags)
            {
                var tagValue = tag.Value;
                byte tagType = GetNbtTagType(tagValue);
                buffer.WriteByte(tagType);
                
                var nameBytes = System.Text.Encoding.UTF8.GetBytes(tag.Key);
                buffer.WriteShort(nameBytes.Length);
                buffer.WriteBytes(nameBytes);
                
                WriteNbtTag(buffer, tagValue);
            }
            buffer.WriteByte(0);
        }
        else if (nbtTag is NbtIntArray nbtIntArray)
        {
            buffer.WriteByte(11);
            buffer.WriteInt(nbtIntArray.Value.Length);
            foreach (var value in nbtIntArray.Value)
            {
                buffer.WriteInt(value);
            }
        }
        else
        {
            buffer.WriteByte(0);
        }
    }
    
    private byte GetNbtTagType(object tagValue)
    {
        return tagValue switch
        {
            NbtByte => 1,
            NbtShort => 2,
            NbtInt => 3,
            NbtLong => 4,
            NbtFloat => 5,
            NbtDouble => 6,
            NbtByteArray => 7,
            NbtString => 8,
            NbtList => 9,
            NbtCompound => 10,
            NbtIntArray => 11,
            _ => 0
        };
    }

    private void WriteAdvancementProgress(IByteBuffer buffer, EntityAdvancementProgress progress)
    {
        buffer.WriteVarInt(progress.Criteria?.Length ?? 0);
        
        if (progress.Criteria != null)
        {
            foreach (var criterionProgress in progress.Criteria)
            {
                WriteIdentifier(buffer, criterionProgress.CriterionId);
                
                WriteCriterionProgress(buffer, criterionProgress.Progress);
            }
        }
    }

    private void WriteCriterionProgress(IByteBuffer buffer, EntityCriterionProgressData progressData)
    {
        buffer.WriteBoolean(progressData.Achieved);
        
        if (progressData.Achieved)
        {
            buffer.WriteLong(progressData.DateOfAchieving);
        }
    }

    private static string SanitizeLocationString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        var locationPattern = @"[^\s/:\\]+:[^\s/:\\]+";
        var matches = Regex.Matches(input, locationPattern);

        foreach (Match match in matches)
        {
            var locationStr = match.Value;
            var parts = locationStr.Split(':');
            
            if (parts.Length == 2)
            {
                var namespacePart = parts[0];
                var keyPart = parts[1];

                if (ContainsNonAsciiChars(namespacePart) || ContainsNonAsciiChars(keyPart))
                {
                    var cleanNamespace = CleanNamespace(namespacePart);
                    var cleanKey = CleanKey(keyPart);
                    
                    var cleanLocation = $"{cleanNamespace}:{cleanKey}";
                    input = input.Replace(locationStr, cleanLocation);
                }
            }
        }

        return input;
    }

    private static string SanitizeNamespacedKey(string namespacedKey)
    {
        if (string.IsNullOrEmpty(namespacedKey))
            return namespacedKey;

        var parts = namespacedKey.Split(':');
        if (parts.Length != 2)
            return namespacedKey; 

        var namespacePart = CleanNamespace(parts[0]);
        var keyPart = CleanKey(parts[1]);

        return $"{namespacePart}:{keyPart}";
    }

    private static bool ContainsNonAsciiChars(string str)
    {
        foreach (char c in str)
        {
            if (c > 127)
                return true;
        }
        return false;
    }

    private static string CleanNamespace(string ns)
    {
        if (string.IsNullOrEmpty(ns))
            return ns;
            
        var cleaned = Regex.Replace(ns, @"[^a-zA-Z0-9_.-]", "_");
        return cleaned.ToLowerInvariant();
    }

    private static string CleanKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            return key;
            
        var cleaned = Regex.Replace(key, @"[^a-zA-Z0-9_.-]", "_");
        return cleaned.ToLowerInvariant();
    }
}

public class EntityAdvancementEntry
{
    public string Id { get; set; } = string.Empty;
    public EntityAdvancement Advancement { get; set; } = new();
}

public class EntityAdvancementProgressEntry
{
    public string Id { get; set; } = string.Empty;
    public EntityAdvancementProgress Progress { get; set; } = new();
}

public class EntityAdvancement
{
    public string? ParentId { get; set; }
    public EntityAdvancementDisplay? DisplayData { get; set; }
    public string[] Criteria { get; set; } = Array.Empty<string>();
    public string[][] Requirements { get; set; } = Array.Empty<string[]>();
}

public class EntityAdvancementDisplay
{
    public TextComponent Title { get; set; } = new();
    public TextComponent Description { get; set; } = new();
    public EntityItemStack Icon { get; set; } = new();
    public EnumAdvancementFrameType FrameType { get; set; }
    public string? BackgroundTexture { get; set; }
    public float XCoord { get; set; }
    public float YCoord { get; set; }
    public bool ShowToast { get; set; }
    public bool Hidden { get; set; }
}

public class EntityAdvancementProgress
{
    public EntityCriterionProgress[] Criteria { get; set; } = Array.Empty<EntityCriterionProgress>();
}

public class EntityCriterionProgress
{
    public string CriterionId { get; set; } = string.Empty;
    public EntityCriterionProgressData Progress { get; set; } = new();
}

public class EntityCriterionProgressData
{
    public bool Achieved { get; set; }
    public long DateOfAchieving { get; set; }
}

public enum EnumAdvancementFrameType
{
    Task = 0,
    Challenge = 1,
    Goal = 2
}

public class EntityItemStack
{
    public bool Present { get; set; } = true;
    public int ItemId { get; set; }
    public int ItemCount { get; set; } = 1;
    public object? Nbt { get; set; }
}

public class NbtEnd {}

public class NbtByte
{
    public byte Value { get; set; }
    public NbtByte(byte value) => Value = value;
}

public class NbtShort
{
    public short Value { get; set; }
    public NbtShort(short value) => Value = value;
}

public class NbtInt
{
    public int Value { get; set; }
    public NbtInt(int value) => Value = value;
}

public class NbtLong
{
    public long Value { get; set; }
    public NbtLong(long value) => Value = value;
}

public class NbtFloat
{
    public float Value { get; set; }
    public NbtFloat(float value) => Value = value;
}

public class NbtDouble
{
    public double Value { get; set; }
    public NbtDouble(double value) => Value = value;
}

public class NbtByteArray
{
    public byte[] Value { get; set; }
    public NbtByteArray(byte[] value) => Value = value;
}

public class NbtString
{
    public string Value { get; set; }
    public NbtString(string value) => Value = value;
}

public class NbtList
{
    public byte TagType { get; set; }
    public object[] Elements { get; set; }
    public NbtList(byte tagType, object[] elements)
    {
        TagType = tagType;
        Elements = elements;
    }
}

public class NbtCompound
{
    public List<KeyValuePair<string, object>> Tags { get; set; } = new List<KeyValuePair<string, object>>();
}

public class NbtIntArray
{
    public int[] Value { get; set; }
    public NbtIntArray(int[] value) => Value = value;
}
