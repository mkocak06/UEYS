using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels.Program;

namespace Application.Reports.ExcelReports.ProgramReports;

public static class ProgramsPeople
{
    public static byte[] ExportProgramsStaffReport(List<ProgramStaffModel> programs)
        {
            using (var workbook = new XLWorkbook())
            {
                workbook.Protect("Tuk123++");
                var worksheet = workbook.Worksheets.Add("YUEP LİSTESİ (Tıp)");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Font.FontSize = 11;
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 75.75;
                worksheet.SheetView.FreezeRows(1);

                var titles = new List<(string name, double width, bool isGray)>()
                {
                    ("EĞİTİM\r\nVERİLEN İL", 10.43, false),
                    ("UZMANLIK\r\nALANI", 15.86, false),
                    ("ÜST KURUM", 15.86, false),
                    ("BAKANLIK / ÜNİVERSİTE", 15.86, false),
                    ("EĞİTİM KURUMU / FAKÜLTE", 15.86, false),
                    ("EĞİTİMİN VERİLDİĞİ KURUM", 15.86, false),
                    ("UZMANLIK EĞİTİMİ PROGRAMI", 15.86, false),
                    ("KURUM\nTEMSİLCİ SAYISI", 20.86, false),
                    ("KURUM\nEĞİTİCİ SAYISI", 29, false),
                    ("KURUM\nÖĞRENCİ SAYISI", 29, false),
                };

                for (int i = 0; i < titles.Count; i++)
                {
                    MainTitles(worksheet, i + 1, titles[i].name, titles[i].width, titles[i].isGray);
                }

                worksheet.Range(1, 1, 1, titles.Count).SetAutoFilter();

                var row = 2;
                int column = 1;
                foreach (var program in programs)
                {
                    var programValues = new List<object>()
                    {
                        program.ProvinceName,
                        program.ProfessionName,
                        program.ParentInstitutionName,
                        program.UniversityName,
                        program.FacultyName,
                        program.HospitalName,
                        program.ExpertiseBranchName,
                        program.EmployeeCount,
                        program.EducatorCount,
                        program.StudentCount,
                    };

                    foreach (var value in programValues)
                    {
                        CellStyles(worksheet, row, ref column, value);
                    }

                    column = 1;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return content;
                }
            }
        }
        private static void MainTitles(IXLWorksheet worksheet, int column, string titleName, double width, bool isGray)
        {
            string backgroundColor = isGray ? "#D0CECE" : "#F2F2F2";
            worksheet.Cell(1, column).Value = titleName;
		    worksheet.Cell(1, column).WorksheetColumn().Width = width;
            worksheet.Cell(1, column).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(1, column).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cell(1, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml(backgroundColor));
            worksheet.Cell(1, column).Style.Font.Bold = true;
        }
        private static void CellStyles(IXLWorksheet worksheet, int row, ref int column, object value)
        {
            worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);
		    worksheet.Cell(row, column).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(row, column).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            column++;
        }
}