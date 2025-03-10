using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels;

namespace Application.Reports.ExcelReports.HospitalReports.HospitalDetail;

public static class Educators
{
    public static void EducatorsSheet(IXLWorksheet worksheet, List<EducatorPaginateResponseDTO> educators)
    {
        worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        worksheet.Style.Alignment.WrapText = true;
        worksheet.Columns().AdjustToContents();

        var titles = new List<(string title, double width)>()
        {
            ("Sıra No", 5),
            ("T.C. Kimlik Numarası", 16),
            ("Eğitici Unvanı", 26),
            ("Adı Soyadı", 30),
            ("Kadrosunun Bulunduğu Üst Kurum", 52),
            ("Kadrosunun Bulunduğu Kurum", 52),
            ("Eğitimin Kurumu", 52),
            ("Ana Dal", 21),
            ("Ana Dal Asli-Geçici", 15),
            ("Yan Dal", 21),
            ("Yan Dal Asli-Geçici", 15),
            ("Roller", 15),
            ("İdari Görevler", 23),
            ("TUEY Geçici Birinci Madde Kapsamında Eğitici Mi?", 21),
        };

        worksheet.Range(1, 1, 1, titles.Count).SetAutoFilter();
        worksheet.SheetView.FreezeRows(1);

        int row = 1;
        for (int i = 0; i < titles.Count; i++)
            MainTitles(worksheet, row, i + 1, titles[i].title, titles[i].width);

        int column = 1;
        foreach (var educator in educators.OrderBy(x => x.PrincipalBranchDutyPlace  ?? x.SubBranchDutyPlace).ThenBy(x => x.PrincipalBranchName))
        {
            row++;
            column = 1;
            
            string roles = "";
            if (educator?.Roles?.Count > 0)
                roles = string.Join(", ", educator.Roles);
            string administrativeTitles = "";
            if (educator?.EducatorAdministrativeTitles?.Count > 0)
                administrativeTitles = string.Join(", ", educator.EducatorAdministrativeTitles);

            var values = new List<object>()
            {
                row -1,
                educator.IdentityNo,
                educator.AcademicTitle,
                educator.Name,
                educator.StaffParentInstitution,
                educator.StaffInstitution,
                educator.PrincipalBranchDutyPlace ?? educator.SubBranchDutyPlace,
                educator.PrincipalBranchName,
                educator.PrincipalBranchDutyType == 0 ? "Asli Görev Yeri" : educator.PrincipalBranchDutyType == 1 ? "Geçici Görev Yeri" : "",
                educator.SubBranchName,
                educator.SubBranchDutyType == 0 ? "Asli Görev Yeri" : educator.SubBranchDutyType == 1 ? "Geçici Görev Yeri" : "",
                roles,
                administrativeTitles,
                educator.IsConditionalEducator == true ? "Evet" : "Hayır",
            };
            
            foreach (var item in values)
            {
                if (column is 3 or 4 or 5)                                      
                    worksheet.Cell(row, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                CellStyles(worksheet, row, ref column, item);
            }
        }

        worksheet.Range(1, 1, row, titles.Count).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border
            .SetInsideBorder(XLBorderStyleValues.Thin);
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