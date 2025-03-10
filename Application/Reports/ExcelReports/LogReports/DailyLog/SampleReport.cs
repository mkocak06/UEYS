using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels;

namespace Application.Reports.ExcelReports.EducatorReports
{
    public class SampleReport
    {
        public static byte[] ExportListReport(List<StudentResponseDTO> students)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Öğrenci Listesi");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 40;

                MainTitles(worksheet, 1, "Adı Soyadı", 45);
                MainTitles(worksheet, 2, "Eğitimin Kurumu", 65);
                MainTitles(worksheet, 3, "Uzmanlık Dalı", 25);

                int recordIndex = 2;
                foreach (var student in students)
                {
                    int j = 1;
                    CellStyles(worksheet, recordIndex, j, student.User.Name, ref j);
                    CellStyles(worksheet, recordIndex, j, student.OriginalProgram.Hospital.Name, ref j);
                    CellStyles(worksheet, recordIndex, j, student.OriginalProgram.ExpertiseBranch.Name, ref j);

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
            worksheet.Cell(column, row).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            return i++;
        }
    }
}
