using Application.Reports.ExcelReports.Helpers;
using ChustaSoft.Common.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels;
using Shared.Types;

namespace Application.Reports.ExcelReports.StudentReports
{
    public class ExportList
    {
        public static byte[] ExportListReport(List<StudentExcelExportModel> students)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Öğrenci Listesi");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 35;

                var row = 1;
                var titles = new List<(string title, double width)>()
                {
                    ("Kişisel Bilgiler", 15),
                    ("Eğitim Bilgileri", 15),
                    ("İşlem Bilgileri", 30),
                };

                var personalTitles = new List<(string title, double width)>()
                {
                    ("TCKN", 15),
                    ("İl", 15),
                    ("Adı Soyadı", 30),
                    ("Doğum Tarihi", 10),
                    ("Cinsiyet", 10)
                };
                var educationTitles = new List<(string title, double width)>()
                {
                    ("Eğitim Alanı", 12),
                    ("Eğitimin Verildiği Kurum", 50),
                    ("Uzmanlık Eğitim Programı", 26),
                    ("Ana Dal / Yan Dal", 10),
                    ("Uzmanlık Eğitim Programı Kategorisi", 14),
                    ("Kontenjan", 23),
                    ("Yerleştirildiği Sınav", 12),
                    ("Sınav Yılı", 8),
                    ("Başladığı Dönem", 15),
                    ("Eğitime Başlama Tarihi", 13),
                    ("Eğitimin Tahmini Bitiş Tarihi", 13),
                    ("Kalan Gün", 17),
                    ("Tez Başarı Durumu", 15),
                    ("Tez Konusu Belirlendi Mi?", 15),
                    ("Bitirme Sınavı Başarı Durumu", 15),
                    ("Müfredat", 10),
                    ("Durum", 25),
                };

                var informationTitles = new List<(string title, double width)>()
                {
                    ("Muayene Sayısı", 9),
                    ("Reçete Sayısı", 9),
                    ("İşlem Sayısı", 9),
                    ("Laboratuvar Tetkik İstemi Sayısı", 12),
                    ("A Grubu Ameliyat Sayısı", 9),
                    ("B Grubu Ameliyat Sayısı", 9),
                    ("C Grubu Ameliyat Sayısı", 9),
                    ("D Grubu Ameliyat Sayısı", 9),
                    ("E Grubu Ameliyat Sayısı", 9),
                };

                MergedTitle(worksheet, row, 1, personalTitles.Count, titles[0].title);
                MergedTitle(worksheet, row, personalTitles.Count + 1, personalTitles.Count + educationTitles.Count, titles[1].title);
                MergedTitle(worksheet, row, personalTitles.Count + educationTitles.Count + 1, personalTitles.Count + educationTitles.Count + informationTitles.Count, titles[2].title);

                var subTitles = new List<(string title, double width)>();
                subTitles.AddRange(personalTitles);
                subTitles.AddRange(educationTitles);
                subTitles.AddRange(informationTitles);

                row++;
                worksheet.Range(row, 1, row, subTitles.Count).SetAutoFilter();
                worksheet.SheetView.FreezeRows(row);

                int column = 1;
                for (int i = 0; i < subTitles.Count; i++)
                    MainTitles(worksheet, row, i + 1, subTitles[i].title, subTitles[i].width);

                foreach (var student in students)
                {
                    row++;

                    var values = new List<object>()
                    {
                        student.IdentityNo,
                        student.ProvinceName,
                        student.Name,
                        student.BirthDate,
                        GetGender(student.Gender),
                        student.ProfessionName,
                        student.OriginalHospitalName,
                        student.OriginalExpertiseBranchName,
                        student.ExpertiseBranchIsPrincipal == true ? "Ana Dal" : "Yan Dal",
                        student.ProgramCategory,
                        GetQuotaType(student.QuatoType),
                        student.BeginningExam?.GetDescription(),
                        student.BeginningYear,
                        student.BeginningPeriod?.GetDescription(),
                        student.BeginningDate,
                        student.EstimatedFinish,
                    };

                    string formattedRemainingTime = "";

                    if (student?.RemainingDays != null)
                    {
                        int? years = student?.RemainingDays / 365;
                        int? months = (student?.RemainingDays % 365) / 30;
                        int? days = (student?.RemainingDays % 365) % 30;
                        formattedRemainingTime = $"{years} Yıl {months} Ay {days} Gün";
                    }

                    values.Add(formattedRemainingTime);
                    values.Add(student.ThesisResult);
                    values.Add(student.IsThesisSubjectDetermined == true ? "Evet" : "Hayır");
                    values.Add(student.ExitExamResult);
                    values.Add("(" + student.CurriculumVersion + ")");
                    values.Add(GetStatus(student.Status));

                    #region enabiz
                    var enabizVerileri = new List<object>()
                    {
                        student.MuayeneSayisi,
                        student.ReceteSayisi,
                        student.IslemSayisi,
                        student.LaboratuvarTetkikİstemiSayisi,
                        student.ATypeSurgeryCount,
                        student.BTypeSurgeryCount,
                        student.CTypeSurgeryCount,
                        student.DTypeSurgeryCount,
                        student.ETypeSurgeryCount,
                    };

                    values.AddRange(enabizVerileri);
                    #endregion

                    foreach (var item in values)
                    {
                        //if (column == 1 || column == 5 || column == 6)
                        //    worksheet.Cell(row, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        CellStyles(worksheet, row, ref column, item);
                    }

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
            worksheet.Cell(row, column).Style.Font.Bold = true;
        }

        private static void CellStyles(IXLWorksheet worksheet, int row, ref int column, object value)
        {
            worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);
            worksheet.Cell(row, column).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(row, column).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            column++;
        }

        private static string GetGender(GenderType? genderType)
        {
            switch (genderType)
            {
                case GenderType.Male:
                    return "Erkek";
                case GenderType.Female:
                    return "Kadın";
                default:
                    return "";
            }
        }

        private static string GetStatus(StudentStatus? statusType)
        {
            switch (statusType)
            {
                case StudentStatus.EducationContinues:
                    return "Eğitime Devam Ediyor";
                case StudentStatus.Gratuated:
                    return "Mezun";
                case StudentStatus.Abroad:
                    return "Yurt Dışında";
                case StudentStatus.Assigned:
                    return "Görevlendirme/Eğitim";
                case StudentStatus.TransferDueToNegativeOpinion:
                    return "Olumsuz Kanaat Nedeniyle Nakil Olması";
                case StudentStatus.EndOfEducationDueToNegativeOpinion:
                    return "Olumsuz Kanaat Nedeniyle Eğitimin Sonlandırılması";
                case StudentStatus.Rotation:
                    return "Rotasyon";
                case StudentStatus.Other:
                    return "Diğer";
                default:
                    return "";
            }
        }

        private static string GetQuota(QuotaType_1? quotationType)
        {
            switch (quotationType)
            {
                case QuotaType_1.MinistryOfHealth:
                    return "Sağlık Bakanlığı";
                case QuotaType_1.Uni:
                    return "Üniversite";
                case QuotaType_1.KKTC:
                    return "KKTC";
                case QuotaType_1.MSB:
                    return "Milli Savunma Bakanlığı";
                case QuotaType_1.MinistryOfInterior:
                    return "İçişleri Bakanlığı";
                case QuotaType_1.TDMM:
                    return "Tıp Dışı Meslek Mensupları";
                case QuotaType_1.YBU:
                    return "Yabancı Uyruklu";
                case QuotaType_1.ADL:
                    return "Adli Tıp Kurumu";
                case QuotaType_1.GuestMilitaryPersonnel:
                    return "Misafir Askeri Personel";
                default:
                    return "";
            }
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

        private static string GetQuota2(QuotaType_2? quotationType2)
        {
            switch (quotationType2)
            {
                case QuotaType_2.EAH:
                    return "Eğitim ve Araştırma Hastanesi";
                case QuotaType_2.SBA:
                    return "Sağlık Bakanlığı Adına";
                case QuotaType_2.University_State:
                    return "Üniversite (Devlet)";
                case QuotaType_2.University_Private:
                    return "Üniversite (Vakıf)";
                case QuotaType_2.KKTCFullTime:
                    return "KKTC-TS / (Tam Süreli)";
                case QuotaType_2.KKTCHalfTime:
                    return "KKTC-YS / (Yarı Süreli-BNDH:Burhan Nalbantoğlu Devlet Hastanesi)";
                case QuotaType_2.KKK:
                    return "Kara Kuvvetleri Komutanlığı";
                case QuotaType_2.HKK:
                    return "Hava Kuvvetleri Komutanlığı";
                case QuotaType_2.DKK:
                    return "Deniz Kuvvetleri Komutanlığı";
                case QuotaType_2.Vet:
                    return "Veteriner";
                case QuotaType_2.Chemist:
                    return "Kimyager";
                case QuotaType_2.Pharmacist:
                    return "Eczacı";
                default:
                    return "";
            }
        }
        private static void MergedTitle(IXLWorksheet worksheet, int row, int firstColumn, int lastColumn, string value)
        {
            IXLRange title = worksheet.Range(worksheet.Cell(row, firstColumn), worksheet.Cell(row, lastColumn)).Merge();
            title.Value = SanitizeForExcel.SanitizeInput(value);
            title.Style.Font.Bold = true;
            title.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
            title.Style.Border.SetOutsideBorderColor(XLColor.Black);
            title.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
        }
    }
}