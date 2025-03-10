using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Core.Models.LogInformation;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Shared.ResponseModels.Program;

namespace Application.Reports.ExcelReports.LogReports.DailyLog
{
    public class Detail
    {
        static string GetExcelStrings(int value)
        {
            string[] alphabet = { string.Empty, "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            var excelStrings = (from c1 in alphabet
                                from c2 in alphabet
                                from c3 in alphabet.Skip(1)                    // c3 is never empty
                                where c1 == string.Empty || c2 != string.Empty // only allow c2 to be empty if c1 is also empty
                                select c1 + c2 + c3).ToList();

            return excelStrings[value];
        }

        public static void DailyLogReport(IXLWorksheet worksheet, List<DetailedLogInformation> logInformations)
        {
            worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Style.Alignment.WrapText = true;
            worksheet.Columns().AdjustToContents();

            worksheet.Row(1).Height = 34.50;
            worksheet.Row(2).Height = 50;
            worksheet.SheetView.FreezeRows(2);

            var row = 1;
            MergedTitle(worksheet, row, 8, 12, "BİR ÖNCEKİ GÜN GİRİŞ YAPAN TOPLAM KULLANICI SAYISI");
            MergedTitle(worksheet, row, 14, 18, "BİR ÖNCEKİ GÜN EKLENEN KULLANICI SAYISI");

            row++;
            var titles = new List<(string name, double width)>()
                {
                    ("EĞİTİM\nVERİLEN İL", 18),
                    ("ÜST KURUM", 18.50),
                    ("BAKANLIK / ÜNİVERSİTE", 50),
                    ("EĞİTİM HASTANESİ / FAKÜLTE", 50),
                    ("PROGRAMIN EĞİTİM VERDİĞİ YER", 50),
                    ("TOPLAM EĞİTİCİ SAYISI", 14),
                    ("TOPLAM ÖĞRENCİ SAYISI", 14),

                    ("UZMANLIK ÖĞRENCİSİ", 12),
                    ("EĞİTİCİ", 12),
                    ("KURUM UEYS SORUMLUSU", 14),
                    ("KURUM UEYS TEMSİLCİSİ", 14),
                    ("TOPLAM", 12),
                    ("8 EKİM'den BİR ÖNCEKİ GÜNE KADAR GİRİŞ YAPAN TOPLAM KULLANICI SAYISI", 29),

                    ("UZMANLIK ÖĞRENCİSİ", 12),
                    ("EĞİTİCİ", 12),
                    ("KURUM UEYS SORUMLUSU", 14),
                    ("KURUM UEYS TEMSİLCİSİ", 14),
                    ("TOPLAM", 12),
                    ("8 EKİM'den BİR ÖNCEKİ GÜNE KADAR EKLENEN TOPLAM KULLANICI SAYISI", 29),
                };

            for (int i = 0; i < titles.Count; i++)
            {
                MainTitles(worksheet, row, i + 1, titles[i].name, titles[i].width);
            }

            worksheet.Range(row, 1, row, titles.Count).SetAutoFilter();

            row++;
            int column = 1;
            foreach (var log in logInformations)
            {
                var values = new List<object>()
                {
                    log.ProvinceName,
                    log.ParentInstitutionName,
                    log.UniversityName,
                    log.FacultyName,
                    log.HospitalName,
                    log.TotalEducatorCount,
                    log.TotalStudentCount,

                    log.TodaysLoggedInStudentCount == 0 ? "" : log.TodaysLoggedInStudentCount,
                    log.TodaysLoggedInEducatorCount == 0 ? "" : log.TodaysLoggedInEducatorCount,
                    log.TodaysLoggedInHeadCount == 0 ? "" : log.TodaysLoggedInHeadCount,
                    log.TodaysLoggedInAgentCount == 0 ? "" : log.TodaysLoggedInAgentCount,
                    ((log.TodaysLoggedInStudentCount ?? 0) + (log.TodaysLoggedInEducatorCount ?? 0) + (log.TodaysLoggedInHeadCount ?? 0) + (log.TodaysLoggedInAgentCount ?? 0)) == 0 ? "" :
                                ((log.TodaysLoggedInStudentCount ?? 0) + (log.TodaysLoggedInEducatorCount ?? 0) + (log.TodaysLoggedInHeadCount ?? 0) + (log.TodaysLoggedInAgentCount ?? 0)),
                    ((log.TotalLoggedInStudentCount ?? 0) + (log.TotalLoggedInEducatorCount ?? 0) + (log.TotalLoggedInHeadCount ?? 0) + (log.TotalLoggedInAgentCount ?? 0)) == 0 ? "" :
                                ((log.TotalLoggedInStudentCount ?? 0) + (log.TotalLoggedInEducatorCount ?? 0) + (log.TotalLoggedInHeadCount ?? 0) + (log.TotalLoggedInAgentCount ?? 0)),

                    log.TodaysCreatedStudentCount,
                    log.TodaysCreatedInEducatorCount,
                    log.TodaysCreatedHeadCount,
                    log.TodaysCreatedAgentCount,
                    ((log.TodaysCreatedStudentCount ?? 0) + (log.TodaysCreatedInEducatorCount ?? 0) + (log.TodaysCreatedHeadCount ?? 0) + (log.TodaysCreatedAgentCount ?? 0)) == 0 ? "" :                   ((log.TodaysCreatedStudentCount ?? 0) + (log.TodaysCreatedInEducatorCount ?? 0) + (log.TodaysCreatedHeadCount ?? 0) + (log.TodaysCreatedAgentCount ?? 0)),
                    ((log.TotalCreatedStudentCount ?? 0) + (log.TotalCreatedEducatorCount ?? 0) + (log.TotalCreatedHeadCount ?? 0) + (log.TotalCreatedAgentCount ?? 0)) == 0 ? "" :        ((log.TotalCreatedStudentCount ?? 0) + (log.TotalCreatedEducatorCount ?? 0) + (log.TotalCreatedHeadCount ?? 0) + (log.TotalCreatedAgentCount ?? 0)),
                };

                foreach (var value in values)
                {
                    CellStyles(worksheet, row, ref column, value);
                }

                column = 1;
                row++;
            }

            IXLRange title = worksheet.Range(worksheet.Cell(row, 1), worksheet.Cell(row, 5)).Merge();
            title.Value = "TOPLAM";
            title.Style.Font.Bold = true;
            title.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));

            for (int i = 5; i < titles.Count; i++)
            {
                worksheet.Cell(row, i + 1).FormulaA1 = $"SUBTOTAL(9, {GetExcelStrings(i)}3:{GetExcelStrings(i)}{row - 1})";
            }

            worksheet.Range(1, 1, row, titles.Count).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border
                       .SetInsideBorder(XLBorderStyleValues.Thin);
        }

        private static void MainTitles(IXLWorksheet worksheet, int row, int column, string titleName, double width)
        {
            worksheet.Cell(row, column).Value = titleName;
            worksheet.Cell(row, column).WorksheetColumn().Width = width;
            worksheet.Cell(row, column).Style.Font.Bold = true;
            worksheet.Cell(row, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
        }

        private static void CellStyles(IXLWorksheet worksheet, int row, ref int column, object value)
        {
            worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);

            column++;
        }

        private static void MergedTitle(IXLWorksheet worksheet, int row, int firstColumn, int lastColumn, string value)
        {
            IXLRange title = worksheet.Range(worksheet.Cell(row, firstColumn), worksheet.Cell(row, lastColumn)).Merge();
            title.Value = SanitizeForExcel.SanitizeInput(value);
            title.Style.Font.Bold = true;
            title.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
        }
    }
}
