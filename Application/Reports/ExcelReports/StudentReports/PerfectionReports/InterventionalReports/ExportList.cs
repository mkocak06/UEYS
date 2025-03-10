using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels;

namespace Application.Reports.ExcelReports.StudentReports.PerfectionReports.InterventionalReports
{
	public class ExportList
    {
        public static byte[] ExportListReport(List<StudentPerfectionResponseDTO> studentPerfections)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"{studentPerfections?.FirstOrDefault()?.Student?.User?.Name} Klinik Yetkinlik Listesi");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 75.75;

                MainTitles(worksheet, 1, "YETKİNLİK", 40);
                MainTitles(worksheet, 2, "İŞLEM TARİHİ", 15);
                MainTitles(worksheet, 3, "PORTFÖY", 10);
                MainTitles(worksheet, 4, "UZMANLIK EĞİTİM\nPROGRAMI", 65);
                MainTitles(worksheet, 5, "EĞİTİCİ", 28);
                MainTitles(worksheet, 6, "BAŞARILI/BAŞARISIZ", 20);

                worksheet.SheetView.FreezeRows(1);
                worksheet.Range(1, 1, 1, 6).SetAutoFilter();

                int recordIndex = 2;
                foreach (var studentPerfection in studentPerfections)
                {
                    int j = 1;
                    CellStyles(worksheet, recordIndex, j, studentPerfection.Perfection?.PName?.Name, ref j);
                    //worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, studentPerfection.ProcessDate, ref j);
                    CellStyles(worksheet, recordIndex, j, studentPerfection.Experience, ref j);
                    //worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, studentPerfection.Program?.Name, ref j);
                    //worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, studentPerfection.Educator?.User?.Name, ref j);
                    CellStyles(worksheet, recordIndex, j, studentPerfection.IsSuccessful == null ? "TAMAMLANMADI": studentPerfection.IsSuccessful == true ? "BAŞARILI" : "BAŞARISIZ", ref j);

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
        private static void MainTitles(IXLWorksheet worksheet, int column, string titleName, double width)
        {
            worksheet.Cell(1, column).Value = titleName;
            worksheet.Cell(1, column).WorksheetColumn().Width = width;
            worksheet.Cell(1, column).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(1, column).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cell(1, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
            worksheet.Cell(1, column).Style.Font.FontSize = 11;
            worksheet.Cell(1, column).Style.Font.Bold = true;
            worksheet.Cell(1, column).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(1, column).Style.Alignment.WrapText = true;
        }
        private static int CellStyles(IXLWorksheet worksheet, int row, int column, object value, ref int i)
        {
            worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);
			worksheet.Cell(row, column).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(row, column).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cell(row, column).Style.Font.FontSize = 11;
            worksheet.Cell(row, column).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(row, column).Style.Alignment.WrapText = true;

            return i++;
        }
    }
}
