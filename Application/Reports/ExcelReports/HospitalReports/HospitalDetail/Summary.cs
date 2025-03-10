using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Core.Entities;
using Core.Models.DetailedReportModels;

namespace Application.Reports.ExcelReports.HospitalReports.HospitalDetail;

public static class Summary
{
    public static void Report(IXLWorksheet worksheet, List<ProgramsStaffCount> programs)
    {
        worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        worksheet.Style.Alignment.WrapText = true;
        worksheet.Columns().AdjustToContents();

        worksheet.Row(1).Height = 30;
        worksheet.Row(2).Height = 34.50;
        worksheet.SheetView.FreezeRows(2);

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


        var educatorTitleList = programs.SelectMany(x => x.EducatorList).Distinct().Select(x  => (x, (double)21)).ToList();
        educatorTitleList.Add(("TOPLAM", 12));

        var studentTitleList = new List<(string name, double width)>()
        {
            ("UZMANLIK ÖĞRENCİSİ SAYISI", 22),
            ("6 AY İÇERİSİNDE EĞİTİMİ BİTECEK OLAN UZMANLIK ÖĞRENCİSİ SAYISI", 47),
        };

        var row = 1;
        MergedTitle(worksheet, row, titles.Count + 1, titles.Count + educatorTitleList.Count, "EĞİTİCİ LİSTESİ");
        MergedTitle(worksheet, row, titles.Count + educatorTitleList.Count() + 1,
            titles.Count + educatorTitleList.Count + studentTitleList.Count, "ÖĞRENCİ LİSTESİ");

        titles.AddRange(educatorTitleList);
        titles.AddRange(studentTitleList);

        int column = 1;
        row++;
        foreach (var (title, width) in titles)
        {
            TitleStyle(worksheet, row, ref column, title, width);
        }

        worksheet.Range(row, 1, row, titles.Count).SetAutoFilter();

        foreach (var program in programs)
        {
            row++;
            column = 1;

            var programValues = new List<(object value, string color)>()
            {
                (program.ProfessionName, null),
                (program.ProvinceName, null),
                (program.ParentInstitutionName == "Yükseköğretim Kurumu (YÖK)"
                    ? (program.IsPrivate == true ? "YÖK-Üni/Vakıf" : "YÖK-Üni/Devlet")
                    : program.ParentInstitutionName, null),
                (program.UniversityName, null),
                (program.FacultyName, null),
                (program.HospitalName, null),
                (program.ExpertiseBranchName, null),
                (program.AffiliatedUniversityName, null),
                (program.AffiliatedHospitalName, null),
                (program.AuthorizationCategory, program.AuthorizationCategoryColorCode),
            };

            int totalEducatorCount = 0;
            foreach (var educatorTitle in educatorTitleList)
            {
                if (educatorTitle.x == "TOPLAM") 
                { 
                    continue; 
                }

                var educatorCount = program.EducatorList?.Count(x => x == educatorTitle.x) ?? 0;
                programValues.Add((educatorCount, null));

                totalEducatorCount += educatorCount;
            }

            programValues.Add((totalEducatorCount, null));

            programValues.Add((program.StudentList.Count(), null));
            programValues.Add((program.StudentList.Count(x => x <= DateTime.UtcNow.AddMonths(6)), null));

            foreach (var value in programValues)
            {
                CellStyles(worksheet, row, ref column, value.Item1, value.Item2);
            }
        }
        
        worksheet.Range(1, 1, row, titles.Count).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border
            .SetInsideBorder(XLBorderStyleValues.Thin);
    }

    private static void TitleStyle(IXLWorksheet worksheet, int row, ref int column, string value,
        double width)
    {
        worksheet.Cell(row, column).Value = value;
        worksheet.Cell(row, column).WorksheetColumn().Width = width;
        worksheet.Cell(row, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
        worksheet.Cell(row, column).Style.Font.FontSize = 12;
        worksheet.Cell(row, column).Style.Font.Bold = true;

        column++;
    }

    private static void CellStyles(IXLWorksheet worksheet, int row, ref int column, object value,
        string backgroundColor)
    {
        if (backgroundColor != null)
        {
            if (!(backgroundColor?.ToLower().Contains("rgb") == true))
            {
                worksheet.Cell(row, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml(backgroundColor));
            }
            else
            {
                worksheet.Cell(row, column).Style.Fill.SetBackgroundColor(ConvertToColorArray(backgroundColor));
            }
        }

        worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);

        column++;
    }

    private static XLColor ConvertToColorArray(string rgbColor)
    {
        rgbColor = StringFilter(rgbColor);

        var splitString = rgbColor.Split(',');
        var splitInts = splitString.Select(item => int.Parse(item)).ToArray();

        return XLColor.FromArgb(splitInts[0], splitInts[1], splitInts[2]);
    }

    private static string StringFilter(this string str)
    {
        List<string> strList = new() { "rgb", "(", ")" };

        foreach (string s in strList)
        {
            str = str.Replace(s, String.Empty);
        }

        return str;
    }

    private static void MergedTitle(IXLWorksheet worksheet, int row, int firstColumn, int lastColumn, string value)
    {
        IXLRange title = worksheet.Range(worksheet.Cell(row, firstColumn), worksheet.Cell(row, lastColumn)).Merge();
        title.Value = SanitizeForExcel.SanitizeInput(value);
		title.Style.Font.Bold = true;
        title.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
    }
}