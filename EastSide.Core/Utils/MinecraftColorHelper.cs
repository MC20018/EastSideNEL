using System.Text;
using System.Text.RegularExpressions;

namespace EastSide.Core.Utils;

public static class MinecraftColorHelper
{
    private static readonly Dictionary<char, string> ColorCodes = new()
    {
        { '0', "#000000" },
        { '1', "#0000AA" },
        { '2', "#00AA00" },
        { '3', "#00AAAA" },
        { '4', "#AA0000" },
        { '5', "#AA00AA" },
        { '6', "#FFAA00" },
        { '7', "#AAAAAA" },
        { '8', "#555555" },
        { '9', "#5555FF" },
        { 'a', "#55FF55" },
        { 'b', "#55FFFF" },
        { 'c', "#FF5555" },
        { 'd', "#FF55FF" },
        { 'e', "#FFFF55" },
        { 'f', "#FFFFFF" },
    };

    public static List<TextSegment> ParseToSegments(string? text)
    {
        var segments = new List<TextSegment>();
        
        if (string.IsNullOrEmpty(text))
            return segments;

        if (!text.Contains('§'))
        {
            segments.Add(new TextSegment(text, null, false, false, false, false));
            return segments;
        }

        string? currentColor = null;
        var bold = false;
        var italic = false;
        var underline = false;
        var strikethrough = false;
        var currentText = new StringBuilder();
        var i = 0;

        while (i < text.Length)
        {
            if (i + 1 < text.Length && text[i] == '§')
            {
                var code = char.ToLower(text[i + 1]);

                if (currentText.Length > 0)
                {
                    segments.Add(new TextSegment(currentText.ToString(), currentColor, bold, italic, underline, strikethrough));
                    currentText.Clear();
                }

                if (ColorCodes.TryGetValue(code, out var color))
                {
                    currentColor = color;
                    bold = false;
                    italic = false;
                    underline = false;
                    strikethrough = false;
                    i += 2;
                    continue;
                }

                switch (code)
                {
                    case 'l': bold = true; break;
                    case 'o': italic = true; break;
                    case 'n': underline = true; break;
                    case 'm': strikethrough = true; break;
                    case 'k': break;
                    case 'r':
                        currentColor = null;
                        bold = false;
                        italic = false;
                        underline = false;
                        strikethrough = false;
                        break;
                }
                i += 2;
                continue;
            }

            currentText.Append(text[i]);
            i++;
        }

        if (currentText.Length > 0)
        {
            segments.Add(new TextSegment(currentText.ToString(), currentColor, bold, italic, underline, strikethrough));
        }

        return segments;
    }

    public static string StripColors(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return Regex.Replace(text, "§[0-9a-fk-orA-FK-OR]", "");
    }

    public static string? GetFirstColor(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        for (var i = 0; i < text.Length - 1; i++)
        {
            if (text[i] == '§')
            {
                var code = char.ToLower(text[i + 1]);
                if (ColorCodes.TryGetValue(code, out var color))
                    return color;
            }
        }

        return null;
    }

    public static bool HasColorCodes(string? text)
    {
        return !string.IsNullOrEmpty(text) && text.Contains('§');
    }
}

public record TextSegment(
    string Text,
    string? Color,
    bool Bold,
    bool Italic,
    bool Underline,
    bool Strikethrough
);
