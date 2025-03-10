using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Core.KDSModels;
using Shared.ResponseModels;
using System.Globalization;

namespace Application.Reports.ExcelReports.AsistanHekimENabiz
{
    public class ExportListByProgramId
    {
        public static byte[] Report(List<ENabizPortfolio> studentENabizOperations)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("İŞLEMLER LİSTESİ");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();

                worksheet.Row(1).Height = 30;

                var titles = new List<(string title, double width)>()
                {
                    ("Sıra No", 6),
                    ("Tarih", 12),
                    ("İl", 18),
                    ("Kurum Adı", 40),
                    ("Klinik Adı", 26),
                    ("Muayene Sayısı", 10),
                    ("Reçete Sayısı", 10),
                    ("İlaç Sayısı", 10),
                    ("Ameliyat ve Girişimler Sayısı", 20),
                    ("Diğer İşlemler Sayısı", 13),
                    ("Diş İşlemleri Sayısı", 13),
                    ("Doğum İşlemleri Sayısı", 14),
                    ("Kan İşlemleri Sayısı", 13),
                    ("Malzeme Sayısı", 11),
                    ("Tahlil Tetkik ve Radyoloji İşlemleri Sayısı", 22),
                    ("Yatak İşlemleri Sayısı", 14),
                    ("A Grubu Ameliyat Sayısı", 14),
                    ("B Grubu Ameliyat Sayısı", 14),
                    ("C Grubu Ameliyat Sayısı", 14),
                    ("D Grubu Ameliyat Sayısı", 14),
                    ("E Grubu Ameliyat Sayısı", 14),
                };

                worksheet.Range(1, 1, 1, titles.Count).SetAutoFilter();
                worksheet.SheetView.FreezeRows(1);

                int row = 1;
                int column = 1;
                for (int i = 0; i < titles.Count; i++)
                {
                    MainTitles(worksheet, row, i + 1, titles[i].title, titles[i].width);
                }

                foreach (var operation in studentENabizOperations)
                {
                    row++;
                    column = 1;

                    var values = new List<object>()
                    {
                        row -1,
                        DateTime.ParseExact(operation.yilay, "yyyyMM", null).ToString("MMMM yyyy", new CultureInfo("tr-TR")),
                        operation.il_adi,
                        operation.kurum_adi,
                        operation.klinik_adi,
                        operation.muayene_sayisi,
                        operation.recete_sayisi,
                        operation.ilac_sayisi,
                        operation.ameliyat_ve_girisimler_sayisi,
                        operation.diger_islemler_sayisi,
                        operation.dis_islemleri_sayisi,
                        operation.dogum_islemleri_sayisi,
                        operation.kan_islemleri_sayisi,
                        operation.malzeme_sayisi,
                        operation.tahlil_tetkik_ve_radyoloji_islemleri_sayisi,
                        operation.yatak_islemleri_sayisi,
                        operation.a_grubu_ameliyat_sayisi,
                        operation.b_grubu_ameliyat_sayisi,
                        operation.c_grubu_ameliyat_sayisi,
                        operation.d_grubu_ameliyat_sayisi,
                        operation.e_grubu_ameliyat_sayisi,
                    };

                    foreach (var item in values)
                    {
                        if (column is 4)
                            worksheet.Cell(row, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        CellStyles(worksheet, row, ref column, item);
                    }
                }

                worksheet.Range(1, 1, row, titles.Count).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border
                            .SetInsideBorder(XLBorderStyleValues.Thin);

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
            worksheet.Cell(row, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
            worksheet.Cell(row, column).Style.Font.Bold = true;
        }
        private static void CellStyles(IXLWorksheet worksheet, int row, ref int column, object? value)
        {
            worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);

            column++;
        }
    }
}
