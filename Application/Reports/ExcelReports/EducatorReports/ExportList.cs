using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels;

namespace Application.Reports.ExcelReports.EducatorReports
{
    public class ExportList
    {
        public static byte[] ExportListReport(List<EducatorPaginateResponseDTO> educators)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Eğitici Listesi");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 75.75;

                MainTitles(worksheet, 1, "T.C. Kimlik Numarası", 12);
                MainTitles(worksheet, 2, "Eğitici Unvanı", 20);
                MainTitles(worksheet, 3, "Adı Soyadı", 30);
                MainTitles(worksheet, 4, "Eğitimin Kurumu", 30);
                MainTitles(worksheet, 5, "Ana Dal", 30);
                MainTitles(worksheet, 6, "Ana Dal Asli-Geçici", 15);
                MainTitles(worksheet, 7, "Yan Dal", 30);
                MainTitles(worksheet, 8, "Yan Dal Asli-Geçici", 15);
                MainTitles(worksheet, 9, "Roller", 45);
                MainTitles(worksheet, 10, "İdari Görevler", 30);
                MainTitles(worksheet, 11, "TUEY Geçici Birinci Madde Kapsamında Eğitici Mi?", 15);
                MainTitles(worksheet, 12, "Telefon", 12);
                MainTitles(worksheet, 13, "E-posta", 30);
                worksheet.SheetView.FreezeRows(1);
                worksheet.Range(1, 1, 1, 11).SetAutoFilter();

                int recordIndex = 2;
                foreach (var educator in educators)
                {
                    string roles = "";
                    if (educator?.Roles?.Count > 0)
                        roles = string.Join(", ", educator.Roles);
                    string administrativeTitles = "";
                    if (educator?.EducatorAdministrativeTitles?.Count > 0)
                        administrativeTitles = string.Join(", ", educator.EducatorAdministrativeTitles);

                    int j = 1;
                    CellStyles(worksheet, recordIndex, j, educator.IdentityNo, ref j);
                    CellStyles(worksheet, recordIndex, j, educator.AcademicTitle, ref j);
                    CellStyles(worksheet, recordIndex, j, educator.Name, ref j);
                    CellStyles(worksheet, recordIndex, j, educator.PrincipalBranchDutyPlace ?? educator.SubBranchDutyPlace, ref j);
                    CellStyles(worksheet, recordIndex, j, educator.PrincipalBranchName, ref j);
                    CellStyles(worksheet, recordIndex, j, educator.PrincipalBranchDutyType == 0 ? "Asli Görev Yeri" : educator.PrincipalBranchDutyType == 1 ? "Geçici Görev Yeri" : "", ref j);
                    CellStyles(worksheet, recordIndex, j, educator.SubBranchName, ref j);
                    CellStyles(worksheet, recordIndex, j, educator.SubBranchDutyType == 0 ? "Asli Görev Yeri" : educator.SubBranchDutyType == 1 ? "Geçici Görev Yeri" : "", ref j);
                    CellStyles(worksheet, recordIndex, j, roles, ref j);
                    CellStyles(worksheet, recordIndex, j, administrativeTitles, ref j);
                    CellStyles(worksheet, recordIndex, j, educator.IsConditionalEducator == true ? "Evet" : "Hayır", ref j);
                    CellStyles(worksheet, recordIndex, j, educator.Phone, ref j);
                    CellStyles(worksheet, recordIndex, j, educator.Email, ref j);
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
