using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Serilog;

namespace EastSide.UI.Bridge;

public static class ResourceExtractor
{
    private const string Prefix = "EastSide.UI.wwwroot.";

    private static readonly HashSet<string> KnownDirs = new(StringComparer.OrdinalIgnoreCase)
    {
        "css", "js", "assets"
    };
    
    private static string GetSafeBaseDir()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        return Path.Combine(appData, "EastSide");
    }

    public static string Extract()
    {
        var resourceDir = Path.Combine(GetSafeBaseDir(), "wwwroot");

        var asm = Assembly.GetExecutingAssembly();
        var names = asm.GetManifestResourceNames()
            .Where(n => n.StartsWith(Prefix, StringComparison.Ordinal))
            .ToList();

        if (names.Count == 0)
        {
            Log.Warning("未找到嵌入的 wwwroot 资源");
            return resourceDir;
        }

        var extracted = 0;
        foreach (var name in names)
        {
            var remainder = name[Prefix.Length..];
            var relativePath = ResolveRelativePath(remainder);
            var destPath = Path.Combine(resourceDir, relativePath);

            using var stream = asm.GetManifestResourceStream(name);
            if (stream == null) continue;

            var newBytes = new byte[stream.Length];
            stream.ReadExactly(newBytes);

            if (File.Exists(destPath))
            {
                var existing = File.ReadAllBytes(destPath);
                if (existing.AsSpan().SequenceEqual(newBytes)) continue;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
            File.WriteAllBytes(destPath, newBytes);
            extracted++;
        }

        Log.Information("wwwroot 资源已释放到: {Path} (共 {Total} 个, 本次释放 {New} 个)",
            resourceDir, names.Count, extracted);
        return resourceDir;
    }

    private static string ResolveRelativePath(string remainder)
    {
        var firstDot = remainder.IndexOf('.');
        if (firstDot < 0) return remainder;

        var firstPart = remainder[..firstDot];

        if (KnownDirs.Contains(firstPart))
        {
            var fileName = remainder[(firstDot + 1)..];
            return Path.Combine(firstPart, fileName);
        }

        return remainder;
    }
}
