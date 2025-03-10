using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Core.Entities;
using Shared.ResponseModels;

namespace Application.Reports.ExcelReports.HospitalReports
{
	public class ExportList
    {
        public static byte[] ExportListReport(List<HospitalResponseDTO> hospitals)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("HASTANE LİSTESİ");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 30;

				var titles = new List<(string title, double width)>()
		        {
			        ("Sıra No", 6),
					("İl", 17),
					("Bağlı Olduğu\nKurum", 20),
					("Hastane İsmi", 40),
			        ("Telefon Numarası", 15),
			        ("E-Posta", 26),
			        ("Web Adresi", 26),
			        ("Adres", 66.43),
		        };

				worksheet.Range(1, 1, 1, titles.Count).SetAutoFilter();
				worksheet.SheetView.FreezeRows(1);

				int row = 1;
				int column = 1;
				for (int i = 0; i < titles.Count; i++)
                {
					MainTitles(worksheet, row, i + 1, titles[i].title, titles[i].width);
				}

                foreach (var hospital in hospitals)
                {
					row++;
					column = 1;

					var values = new List<object>()
					{
						row -1,
						hospital?.Province?.Name,
						hospital?.Institution?.Name,
						hospital.Name,
						hospital.Phone,
						hospital.Email,
						hospital.WebAddress,
						hospital.Address,
					};

					foreach (var item in values)
					{
						if (column is 4 or 8)
							worksheet.Cell(row, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

						CellStyles(worksheet, row, ref column, item);
					}
				}

				worksheet.Range(1, 1, row, titles.Count).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border
			                .SetInsideBorder(XLBorderStyleValues.Thin);

				using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return content;
                }
            }
        }
		private static void MainTitles(IXLWorksheet worksheet, int row, int column, string titleName, double width)
		{
			worksheet.Cell(row, column).Value = titleName;
			worksheet.Cell(row, column).WorksheetColumn().Width = width;
			worksheet.Cell(row, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
			worksheet.Cell(row, column).Style.Font.Bold = true;
		}
		private static void CellStyles(IXLWorksheet worksheet, int row, ref int column, object? value)
		{
			worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);

			column++;
		}
	}
}
