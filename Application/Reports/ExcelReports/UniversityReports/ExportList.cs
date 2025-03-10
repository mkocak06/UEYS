using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels;

namespace Application.Reports.ExcelReports.UniversityReports
{
	public  class ExportList
    {
        public static byte[] ExportListReport(List<UniversityResponseDTO> universities)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Üniversite Listesi");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 75.75;

                MainTitles(worksheet, 1, "Üniversite İsmi", 60);
                MainTitles(worksheet, 2, "Telefon Numarası", 20);
                MainTitles(worksheet, 3, "Web Adresi", 20);
                MainTitles(worksheet, 4, "Adres", 66.43);
                MainTitles(worksheet, 5, "Vakıf Üniversitesi Mi?", 30);

                worksheet.SheetView.FreezeRows(1);
                worksheet.Range(1, 1, 1, 5).SetAutoFilter();

                int recordIndex = 2;
                foreach (var university in universities)
                {
                    int j = 1;
                    CellStyles(worksheet, recordIndex, j, university.Name, ref j);
                    worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, university.Phone, ref j);
                    worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, university.WebAddress, ref j);
                    worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, university.Address, ref j);
                    worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, university.IsPrivate == true ? "Evet" : "Hayır", ref j);

                    recordIndex++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return content;
                }
            }
        }
        private static void MainTitles(IXLWorksheet worksheet, int row, string titleName, double width)
        {
            worksheet.Cell(1, row).Value = titleName;
            worksheet.Cell(1, row).WorksheetColumn().Width = width;
            worksheet.Cell(1, row).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(1, row).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cell(1, row).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
            worksheet.Cell(1, row).Style.Font.FontSize = 11;
            worksheet.Cell(1, row).Style.Font.Bold = true;
            worksheet.Cell(1, row).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(1, row).Style.Alignment.WrapText = true;
        }
        private static int CellStyles(IXLWorksheet worksheet, int column, int row, object value, ref int i)
        {
            worksheet.Cell(column, row).Value = SanitizeForExcel.SanitizeInput(value);
			worksheet.Cell(column, row).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(column, row).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cell(column, row).Style.Font.FontSize = 11;
            worksheet.Cell(column, row).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(column, row).Style.Alignment.WrapText = true;

            return i++;
        }
    }
}
