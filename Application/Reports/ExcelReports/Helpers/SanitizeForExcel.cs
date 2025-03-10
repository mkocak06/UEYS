using ClosedXML.Excel;
using Sentry.Protocol;
using System.Web;

namespace Application.Reports.ExcelReports.Helpers
{
	public static class SanitizeForExcel
	{
		public static XLCellValue SanitizeInput(object? input)
		{
			if (input != null)
			{
                if (input is DateTime date)
                {
                    if (date < new DateTime(1900, 1, 1))
                    {
                        XLCellValue.FromObject("Geçersiz Tarih");
                    }

                    return XLCellValue.FromObject(date.ToString("dd.MM.yyyy"));
                }

				if (input is string)
				{
                    var data = input as string;

                    if (data.StartsWith("=") || data.StartsWith("+") || data.StartsWith("-") || data.StartsWith("@"))
                    {
                        data = "'" + data; // Başına tek tırnak ekleyerek Excel'in formül olarak işlemesini engelle
                    }

                    string encoded = HttpUtility.HtmlEncode(input);

                    // Encode edilen Türkçe karakterleri orijinal hallerine geri döndür
                    encoded = encoded.Replace("&#252;", "ü")
                                     .Replace("&#220;", "Ü")
                                     .Replace("&#246;", "ö")
                                     .Replace("&#214;", "Ö")
                                     .Replace("&#231;", "ç")
                                     .Replace("&#199;", "Ç")
                                     .Replace("&#351;", "ş")
                                     .Replace("&#350;", "Ş")
                                     .Replace("&#287;", "ğ")
                                     .Replace("&#286;", "Ğ")
                                     .Replace("&#305;", "ı")
                                     .Replace("&#304;", "İ")
                                     .Replace("&#226;", "â")
                                     .Replace("&#194;", "Â");

                    return XLCellValue.FromObject(encoded); // Veya uygun bir encoding yöntemi
                }
			}

			return XLCellValue.FromObject(input);
		}
	}
}
