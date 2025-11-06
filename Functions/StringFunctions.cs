using System;
using System.IO;
using System.Linq;

namespace LastDayToPlant.Functions;

internal class StringFunctions
{
    public static string ExtractModNameFromPath(string path)
    {
        var parts = Path.GetFullPath(path)
        .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        .ToList();

        int modsIndex = parts.FindIndex(p => string.Equals(p, "Mods", StringComparison.OrdinalIgnoreCase));

        if (modsIndex >= 0 && modsIndex + 1 < parts.Count)
        {
            return parts[modsIndex + 1];
        }

        return string.Empty;
    }
}
