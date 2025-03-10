using Application.Reports.ExcelReports.Helpers;
using ChustaSoft.Common.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels;
using Shared.Types;

namespace Application.Reports.ExcelReports.HospitalReports.HospitalDetail;

public static class Students
{
    public static void StudentsSheet(IXLWorksheet worksheet, List<StudentExcelExportModel> students)
    {
        worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        worksheet.Style.Alignment.WrapText = true;
        worksheet.Columns().AdjustToContents();

        var titles = new List<(string title, double width)>()
        {
            ("Sıra No", 5),
            ("T.C. Kimlik Numarası", 16),
            ("Adı Soyadı", 30),
            ("Eğitim Alanı", 11),
            ("Eğitimin Verildiği Kurum", 52),
            ("Uzmanlık Eğitim Programı", 21),
            ("Ana Dal / Yan Dal", 11),
            ("Uzmanlık Eğitim Programı Kategorisi", 13),
            ("Kontenjan", 15),
            ("Yerleştirildiği Sınav", 11),
            ("Sınav Yılı", 8),
            ("Başladığı Dönem", 11),
            ("Eğitime Başlama Tarihi", 15),
            ("Eğitimin Tahmini Bitiş Tarihi", 15),
            ("Kalan Gün", 15),
            ("Tez Başarı Durumu", 14),
            ("Bitirme Sınavı Başarı Durumu", 14),
            ("Müfredat", 10),
            ("Durum", 15),
        };

        worksheet.Range(1, 1, 1, titles.Count).SetAutoFilter();
        worksheet.SheetView.FreezeRows(1);

        int row = 1;
        int column = 1;
        for (int i = 0; i < titles.Count; i++)
            MainTitles(worksheet, row, i + 1, titles[i].title, titles[i].width);

        foreach (var student in students.OrderBy(x => x.OriginalHospitalName).ThenBy(x => x.OriginalExpertiseBranchName))
        {
            row++;
            column = 1;

            string formattedRemainingTime = "";

            if (student?.RemainingDays != null)
            {
                int? years = student?.RemainingDays / 365;
                int? months = (student?.RemainingDays % 365) / 30;
                int? days = (student?.RemainingDays % 365) % 30;
                formattedRemainingTime = $"{years} Yıl {months} Ay {days} Gün";
            }
            
            var values = new List<object>()
            {
                row -1,
                student.IdentityNo,
                student.Name,
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
                formattedRemainingTime,
                student.ThesisResult,
                student.ExitExamResult,
                "(" + student.CurriculumVersion + ")",
                GetStatus(student.Status)
            };

            foreach (var item in values)
            {
                if (column is 1 or 3 or 5)
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
            case StudentStatus.EstimatedFinishDatePast:
                return "Eğitimin Tahmini Bitiş Tarihi Geçmiş";
            default:
                return "";
        }
    }
}