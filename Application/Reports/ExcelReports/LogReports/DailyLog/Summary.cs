using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Core.Models.DetailedReportModels;
using Core.Models.LogInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Reports.ExcelReports.LogReports.DailyLog
{
    public static class Summary
    {
        public static void Report(IXLWorksheet worksheet, List<DetailedLogInformation> detailedLogs)
        {
            worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Style.Alignment.WrapText = true;
            worksheet.Columns().AdjustToContents();

            worksheet.Row(1).Height = 30;
            worksheet.SheetView.FreezeRows(1);

            var titles = new List<(string name, double width)>()
            {
                ("UZMANLIK\nALANI", 14.33),
                ("EĞİTİM\nVERİLEN İL", 19),
                ("ÜST KURUM", 18.50),
                ("BAKANLIK / ÜNİVERSİTE", 39.50),
                ("EĞİTİM HASTANESİ / FAKÜLTE", 84.50),
                ("PROGRAMIN EĞİTİM VERDİĞİ YER", 88.50),
                ("UZMANLIK EĞİTİMİ PROGRAMI", 48.17),
                ("BİRLİKTE KULLANIM PROTOKOL YAPILAN ÜNİVERSİTE", 50),
                ("BİRLİKTE KULLANIM PROTOKOL YAPILAN FAKÜLTE", 50),
                ("GÜNCEL YETKİ KATEGORİSİ", 21),
            };

            var row = 1;
            int column = 1;
            foreach (var (title, width) in titles)
            {
                TitleStyle(worksheet, row, ref column, title, width);
            }

            worksheet.Range(row, 1, row, titles.Count).SetAutoFilter();

            row++;
            foreach (var program in detailedLogs)
            {
                var values = new List<object>()
                {
                };

                foreach (var value in values)
                {
                    CellStyles(worksheet, row, ref column, value);
                }

                row++;
                column = 1;
            }

            worksheet.Range(1, 1, row, titles.Count).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border
                .SetInsideBorder(XLBorderStyleValues.Thin);
        }

        private static void TitleStyle(IXLWorksheet worksheet, int row, ref int column, string value,
            double width)
        {
            worksheet.Cell(row, column).Value = value;
            worksheet.Cell(row, column).WorksheetColumn().Width = width;
            worksheet.Cell(row, column).Style.Font.FontSize = 12;
            worksheet.Cell(row, column).Style.Font.Bold = true;

            column++;
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
