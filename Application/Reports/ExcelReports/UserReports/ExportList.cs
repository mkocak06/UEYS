using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels;

namespace Application.Reports.ExcelReports.UserReports
{
	public class ExportList
    {
        public static byte[] ExportListReport(List<UserPaginateResponseDTO> users)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("KULLANICI LİSTESİ");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 27;

                MainTitles(worksheet, 1, "Kullanıcı T.C.", 20);
                MainTitles(worksheet, 2, "Kullanıcı Adı-Soyadı", 25);
                MainTitles(worksheet, 3, "Kullanıcı E-Posta", 35);
                MainTitles(worksheet, 4, "Telefon Numarası", 20);
                MainTitles(worksheet, 5, "Roller", 40);
                MainTitles(worksheet, 6, "Kurumlar / İller / Programlar", 60);

                worksheet.SheetView.FreezeRows(1);
                worksheet.Range(1, 1, 1, 6).SetAutoFilter();

                int recordIndex = 2;
                foreach (var user in users)
                {
                    int j = 1;

                    List<string> concatZones = new();
                    if (user.HospitalZones != null)
                        concatZones.AddRange(user.HospitalZones);
                    if (user.FacultyZones != null)
                        concatZones.AddRange(user.FacultyZones);
                    if (user.UniversityZones != null)
                        concatZones.AddRange(user.UniversityZones);
                    if (user.ProgramZones != null)
                        concatZones.AddRange(user.ProgramZones);
                    if (user.ProvinceZones != null)
                        concatZones.AddRange(user.ProvinceZones);
                    if (user.EducatorZone != null && user.EducatorZone != " ")
                        concatZones.Add(user.EducatorZone);
                    if (user.StudentZone != null && user.StudentZone != " ")
                        concatZones.Add(user.StudentZone);

                    var zones = concatZones?.Count > 0 ? string.Join(", ", concatZones) : "";

                    string roles = "";
                    if (user?.Roles?.Count > 0)
                    {
                        roles = string.Join(", ", user.Roles);
                    }
                    CellStyles(worksheet, recordIndex, j, user.IdentityNo, ref j);
                    worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, user.Name, ref j);
                    worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, user.Email, ref j);
                    worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, user.Phone, ref j);
                    CellStyles(worksheet, recordIndex, j, roles, ref j);
                    worksheet.Cell(recordIndex, j - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    CellStyles(worksheet, recordIndex, j, zones, ref j);

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
        private static int CellStyles(IXLWorksheet worksheet, int column, int row, object? value, ref int i)
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
