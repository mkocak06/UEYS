using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Core.Models.DetailedReportModels;

namespace Application.Reports.ExcelReports.HospitalReports.HospitalDetail;

public class StaffCountByExpertiseBranch
{
    private static void TitleStyle(IXLWorksheet worksheet, int row, ref int column, string value,
        double width)
    {
        worksheet.Cell(row, column).Value = value;
		worksheet.Cell(row, column).WorksheetColumn().Width = width;
        worksheet.Cell(row, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9E1F2"));
        worksheet.Cell(row, column).Style.Font.FontSize = 12;
        worksheet.Cell(row, column).Style.Font.Bold = true;

        column++;
    }

    private static void CellStyle(IXLWorksheet worksheet, int row, ref int column, object value)
    {
        if (value is decimal || value is double || value is int)
        {
            worksheet.Cell(row, column).Style.NumberFormat.Format = "#,##0.00";
        }

        worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);

		column++;
    }

    public static void Report(IXLWorksheet worksheet, List<CountByExpertiseBranch> staffCountbyExpBranch)
    {
        worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Style.Alignment.WrapText = true;
        worksheet.Columns().AdjustToContents();
        worksheet.Row(1).Height = 31.50;

        var titles = new List<(string title, double width)>()
        {
            ("Uzmanlık Alanı", 12),
            ("Ana Dal / Yan Dal", 12),
            ("Uzmanlık Dalı", 22),
            ("Uzmanlık Eğitimi Program Sayısı", 17),
            ("Eğitici Sayısı", 16),
            ("Öğrenci Sayısı", 16),
            ("Eğitici Başına Düşen Öğrenci Sayısı", 16),
            ("Program Başına Düşen Öğrenci Sayısı", 16),
        };

        int column = 1;
        foreach (var (title, width) in titles)
        {
            TitleStyle(worksheet, 1, ref column, title, width);
        }

        int row = 2;
        foreach (var expertiseBranch in staffCountbyExpBranch.Select(x => x.ExpertiseBranchName).OrderBy(x => x))
        {
            var data = staffCountbyExpBranch.FirstOrDefault(x => x.ExpertiseBranchName == expertiseBranch);

            column = 1;
            var values = new List<object>()
            {
                data.ProfessionName,
                data.IsPrincipal ? "Ana Dal" : "Yan Dal",
                data.ExpertiseBranchName,
                data.ProgramCount,
                data.EducatorCount,
                data.StudentCount,
                data.StudentCount / ((data.EducatorCount == 0 || data.EducatorCount == null) ? 1 : data.EducatorCount),
                data.StudentCount / ((data.ProgramCount == 0 || data.ProgramCount == null) ? 1 : data.ProgramCount),
            };

            foreach (var item in values)
            {
                CellStyle(worksheet, row, ref column, item);
            }

            row++;
        }

        worksheet.Range(1, 1, row - 1, titles.Count).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border
            .SetInsideBorder(XLBorderStyleValues.Thin);
    }
}