using System.Text.RegularExpressions;

namespace Zaczytani.Domain.Helpers;

public class HtmlCleaner
{
    public static string? CleanHtmlDescription(string? html)
    {
        if (html == null)
        {
            return null;
        }

        string cleanHtml = Regex.Replace(html, @"<br\s*/?>", "\n");
        cleanHtml = Regex.Replace(cleanHtml, @"<\/p>", "\n");
        cleanHtml = Regex.Replace(cleanHtml, @"<\/p\s*\/?>", "\n");

        cleanHtml = Regex.Replace(cleanHtml, @"<.*?>", "");

        cleanHtml = Regex.Replace(cleanHtml, @"\s+", " ").Trim();

        return cleanHtml;
    }
}
