using System;
using System.Linq;
using Humanizer;
using Newtonsoft.Json;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace UI.Helper;

public static class Tools
{
    public static string GetFileName(DocumentResponseDTO file, bool willTruncate = true)
    {
        if (!String.IsNullOrEmpty(file.Name))
        {
            if (willTruncate)
                return file.Name.Truncate(30);
            else
                return file.Name;
        }
        if (willTruncate)
            return file.FileUrl.Split('\\').Last().Truncate(30);
        else
            return file.FileUrl.Split('\\').Last();
    }
    public static string GetUntilOrEmpty(this string text, string stopAt = "-")
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        var charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);
        return charLocation > 0 ? text[..charLocation] : string.Empty;
    }
    public static string FormatBytes(long bytes)
    {
        string[] suffix = { "B", "KB", "MB", "GB", "TB" };
        int i;
        double dblSByte = bytes;
        for (i = 0; i < suffix.Length && bytes >= 1024; i++, bytes /= 1024)
        {
            dblSByte = bytes / 1024.0;
        }

        return String.Format("{0:0.##} {1}", dblSByte, suffix[i]);
    }
    public static T Clone<T>(this T source)
    {
        // Don't serialize a null object, simply return the default for that object
        if (object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
        var serializeSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
    }
    public static string GetFileExtensionImage(string fileName)
    {
        var ext = fileName.Split('.').Last();
        return ext switch
        {
            "css" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/css.svg\">",
            "csv" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/csv.svg\">",
            "doc" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/doc.svg\">",
            "docx" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/doc.svg\">",
            "html" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/html.svg\">",
            "js" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/javascript.svg\">",
            "jpg" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/jpg.svg\">",
            "jpeg" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/jpg.svg\">",
            "mp4" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/mp4.svg\">",
            "pdf" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/pdf.svg\">",
            "xml" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/xml.svg\">",
            "zip" => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/files/zip.svg\">",
            _ => "<img class=\"d-inline\" alt=\"Pic\" src=\"/assets/media/svg/icons/Files/File.svg\">",
        };
    }
}