using System.Text;
using System.Text.RegularExpressions;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Service;

internal static class CodeGen
{
    // Create a compact code from a name, e.g. "Thyroid Stimulating Hormone"
    // -> "TSH". Falls back to sanitized first token, caps at 12 chars.
    public static string FromName(string name)
    {
        name ??= "";
        var words = Regex.Split(name.Trim(), @"\s+")
                         .Where(w => !string.IsNullOrWhiteSpace(w))
                         .ToArray();

        if (words.Length == 0) return "T";

        // Try an acronym first: first letters of each word (<= 6)
        var acronym = new string(words.Take(6).Select(w => char.ToUpperInvariant(w[0])).ToArray());
        if (acronym.Length >= 2) return acronym;

        // Fallback: strip non-alphanum, take up to 12 chars, uppercase
        var cleaned = Regex.Replace(name, "[^A-Za-z0-9]", "");
        if (string.IsNullOrWhiteSpace(cleaned)) return "T";
        return cleaned[..Math.Min(cleaned.Length, 12)].ToUpperInvariant();
    }

    // Ensure uniqueness in LabTests (adds numeric suffix if needed)
    public static async Task<string> EnsureUniqueLabTestCodeAsync(LabDbContext db, string baseCode, CancellationToken ct)
    {
        baseCode = (baseCode ?? "T").Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(baseCode)) baseCode = "T";

        var code = baseCode;
        int i = 2;
        while (await db.LabTests.AnyAsync(t => t.Code == code, ct))
        {
            var suffix = "_" + i.ToString();
            var maxLen = 40; // matches your EF config
            var head = Math.Min(baseCode.Length, maxLen - suffix.Length);
            code = baseCode[..head] + suffix;
            i++;
        }
        return code;
    }

    // Panels too
    public static async Task<string> EnsureUniquePanelCodeAsync(LabDbContext db, string baseCode, CancellationToken ct)
    {
        baseCode = (baseCode ?? "P").Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(baseCode)) baseCode = "P";

        var code = baseCode;
        int i = 2;
        while (await db.LabPanels.AnyAsync(p => p.Code == code, ct))
        {
            var suffix = "_" + i.ToString();
            var maxLen = 40;
            var head = Math.Min(baseCode.Length, maxLen - suffix.Length);
            code = baseCode[..head] + suffix;
            i++;
        }
        return code;
    }
}
