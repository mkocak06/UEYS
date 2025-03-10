using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels;
using Shared.Types;

namespace Application.Reports.ExcelReports.SubQuotaRequestReports
{
	public class ExportList
    {
        public static byte[] ExportListReport(List<SubQuotaRequestPaginateResponseDTO> subQuotaRequests)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Talep Listesi");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 75.75;

                var types = GetTypes(subQuotaRequests);

                var titles = new List<(string title, double width)>()
                {
                    ("İl", 19),
                    ("Eğitimin Verildiği Kurum", 60),
                    ("Uzmanlık Alanı", 30),
                    ("Eğitim Görevlisi veya Profesör Sayısı", 13),
                    ("Doçent Sayısı", 13),
                    ("Başasistan Sayısı", 13),
                    ("Dr. Öğretim Üyesi Sayısı", 13),
                    ("Uzman Sayısı", 13),
                    ("Kapasite", 13),
                    ("Mevcut Öğrenci Sayısı", 13),
                    ("Öğrenim Süresinin Bitimine 6 Ay Kalan Öğrenci Sayısı", 13),
                };

                types.ForEach(x => titles.Add(new() { title = "Uzmanlık Öğrencisi Talebi-" + GetQuotaType(x), width = 18 }));

                worksheet.Range(1, 1, 1, titles.Count).SetAutoFilter();
                worksheet.SheetView.FreezeRows(1);

                int row = 1;
                int column = 1;
                for (int i = 0; i < titles.Count; i++)
                    MainTitles(worksheet, row, i + 1, titles[i].title, titles[i].width);

                foreach (var subQuotaRequest in subQuotaRequests)
                {
                    row++;
                    var values = new List<object>()
                    {
                        subQuotaRequest.ProvinceName,
                        subQuotaRequest.HospitalName,
                        subQuotaRequest.ExpertiseBranchName,
                        subQuotaRequest.ProfessorCount,
                        subQuotaRequest.AssociateProfessorCount,
                        subQuotaRequest.ChiefAssistantCount,
                        subQuotaRequest.DoctorLecturerCount,
                        subQuotaRequest.SpecialistDoctorCount,
                        subQuotaRequest.Capacity,
                        subQuotaRequest.CurrentStudentCount,
                        subQuotaRequest.StudentWhoLast6MonthToFinishCount,
                    };
                    foreach (var item in types)
                        values.Add(subQuotaRequest.StudentCounts.FirstOrDefault(x => x.QuotaType == item)?.RequestedCount ?? 0);
                    foreach (var item in values)
                        CellStyles(worksheet, row, ref column, item);
                    column = 1;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return content;
                }
            }
        }
        private static void MainTitles(IXLWorksheet worksheet, int row, int column, string titleName, double width)
        {
            worksheet.Cell(row, column).Value = titleName;
            worksheet.Cell(row, column).WorksheetColumn().Width = width;
            worksheet.Cell(row, column).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(row, column).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cell(row, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
            worksheet.Cell(row, column).Style.Font.FontSize = 11;
            worksheet.Cell(row, column).Style.Font.Bold = true;
            worksheet.Cell(row, column).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(row, column).Style.Alignment.WrapText = true;
        }
        private static int CellStyles(IXLWorksheet worksheet, int row, ref int column, object? value)
        {
            worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);
			worksheet.Cell(row, column).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(row, column).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cell(row, column).Style.Font.FontSize = 11;
            worksheet.Cell(row, column).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(row, column).Style.Alignment.WrapText = true;

            return column++;
        }

        private static List<QuotaType?> GetTypes(List<SubQuotaRequestPaginateResponseDTO> subQuotaRequests)
        {
            return subQuotaRequests.SelectMany(x => x.StudentCounts).Select(x => x.QuotaType).Distinct().ToList();
        }

        private static string GetQuotaType(QuotaType? quotaType)
        {
            return quotaType switch
            {
                QuotaType.YBU => "YBU(Yabancı Uyruklu)",
                QuotaType.ADL => "ADL(Adli Tıp Kurumu)",
                QuotaType.MAP => "MAP(Misafir Askeri Personel)",
                QuotaType.SB => "SB(Sağlık Bakanlığı)",
                QuotaType.Uni_State => "ÜNİ-D / Üniversite (Devlet)",
                QuotaType.Uni_Private => "ÜNİ-V / Üniversite (Vakıf)",
                QuotaType.SBA => "SBA(Sağlık Bakanlığı Adına)",
                QuotaType.KKTCFullTime => "KKTC-TS / (Tam Süreli)",
                QuotaType.KKTCHalfTime => "KKTC-YS / (Yarı Süreli-BNDH:Burhan Nalbantoğlu Devlet Hastanesi)",
                QuotaType.JGK => "JGK(Jandarma Genel Komutanlığı)",
                QuotaType.KKK => "MSB(Milli Savunma Bakanlığı)-KKK(Kara Kuvvetleri Komutanlığı)",
                QuotaType.HKK => "MSB(Milli Savunma Bakanlığı)-HKK(Hava Kuvvetleri Komutanlığı)",
                QuotaType.DKK => "MSB(Milli Savunma Bakanlığı)-DKK(Deniz Kuvvetleri Komutanlığı)",
                QuotaType.Vet => "Veteriner",
                QuotaType.Chemist => "Kimyager",
                QuotaType.Pharmacist => "Eczacı",
                _ => "",
            };
        }
    }
}
