using System.Reflection;
using System.Text;

namespace LifeSyncApp.Client.Services.Http;

internal static class QueryStringHelper
{
    public static string ToQueryString(object? filter)
    {
        if (filter == null) return string.Empty;

        var props = filter.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var sb = new StringBuilder();
        bool first = true;

        foreach (var p in props)
        {
            var value = p.GetValue(filter);
            if (value == null) continue;

            string stringValue = value switch
            {
                DateOnly d => d.ToString("yyyy-MM-dd"),
                DateTime dt => dt.ToString("yyyy-MM-ddTHH:mm:ss"),
                bool b => b ? "true" : "false",
                Enum e => e.ToString(),
                _ => value.ToString() ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(stringValue)) continue;

            sb.Append(first ? '?' : '&');
            first = false;
            sb.Append(Uri.EscapeDataString(p.Name));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(stringValue));
        }

        return sb.ToString();
    }
}
