using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Core.Helper;
using Shared.ResponseModels.Program;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Application.Reports.ExcelReports.ProgramReports
{
    public static class ExportList
    {
        private static int centeredTitleCount = 0;
        public static byte[] ExportListReport(List<ProgramExportResponseDTO> programs)
        {
            var authDetailCount = programs.MaxBy(x => x.AuthorizationDetails.Count)?.AuthorizationDetails?.Count ?? 1;

            using (var workbook = new XLWorkbook())
            {
                workbook.Protect("Tuk123++");
                var worksheet = workbook.Worksheets.Add("YUEP LİSTESİ (Tıp)");
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Style.Font.FontSize = 11;
                worksheet.Style.Alignment.WrapText = true;
                worksheet.Columns().AdjustToContents();
                worksheet.Protect("Tuk123++").AllowElement(XLSheetProtectionElements.AutoFilter);

                IXLSheetView view = worksheet.SheetView;
                view.ZoomScale = 70;

                worksheet.Row(1).Height = 45;
                worksheet.SheetView.FreezeRows(1);

                var titles = new List<(string name, double width, bool isGray)>()
                {
                    ("UZMANLIK\nALANI", 14.33, false),
                    ("EĞİTİM\nVERİLEN İL", 19, false),
                    ("ÜST KURUM", 18.50, false),
                    ("BAKANLIK / ÜNİVERSİTE", 39.50, false),
                    ("EĞİTİM HASTANESİ / FAKÜLTE", 84.50, false),
                    ("PROGRAMIN EĞİTİM VERDİĞİ YER", 88.50, false),
                    ("UZMANLIK EĞİTİMİ PROGRAMI", 48.17, false),
                    //("ÖĞRENCİ SAYISI",13, false),
                    ("BİRLİKTE KULLANIM PROTOKOL YAPILAN ÜNİVERSİTE", 35.17, false),
                    ("BİRLİKTE KULLANIM PROTOKOL YAPILAN FAKÜLTE", 46.83, false),
                    ("Ana Dal / Yan Dal", 16.50, false),
                    ("YETKİLENDİRİLME BİTİŞ TARİHİ", 20.33, false),
                };

                var subTitles = new List<string>()
                {
                    "YETKİ KATEGORİSİ",
                    "ZİYARET TARİHİ",
                    "YETKİLENDİRME KARAR TARİHİ",
                    "YETKİLENDİRME KARAR NO",
                };

                for (int i = 0; i < authDetailCount; i++)
                {
                    var period = NumberToWordHelper.ConvertToWord(i);

                    foreach (var sub in subTitles)
                    {
                        titles.Add(new()
                        {
                            name = $"{(i == 0 ? "GÜNCEL" : period + " ÖNCEKİ")} {sub}",
                            width = 15.17,
                            isGray = i % 2 == 0
                        });
                    }
                }

                for (int i = 0; i < titles.Count; i++)
                {
                    MainTitles(worksheet, i + 1, titles[i].name, titles[i].width, titles[i].isGray);
                }

                worksheet.Range(1, 1, 1, titles.Count).SetAutoFilter();

                var row = 2;
                int column = 1;
                foreach (var program in programs)
                {
                    var programValues = new List<(object value, string color)>()
                    {
                        (program.ProfessionName, null),
                        (program.ProvinceName, null),
                        ((program.ParentInstitutionName == "Yükseköğretim Kurumu (YÖK)" ? (program.IsPrivate == true ? "YÖK-Üni/Vakıf" : "YÖK-Üni/Devlet") : program.ParentInstitutionName), null),
                        (program.UniversityName, null),
                        (program.FacultyName, null),
                        (program.HospitalName, null),
                        (program.ExpertiseBranchName, null),
                        //(program.StudentCount, null),
                        (program.AffiliatedUniversityName, null),
                        (program.AffiliatedFacultyName, null),
                        (program.IsPrincipal == true ? "Ana Dal" : "Yan Dal", null),
                    };

                    centeredTitleCount = programValues.Count;

                    if (program.AuthorizationDetails != null)
                    {
                        var currentAuthDetail = true;
                        foreach (var authorizationDetail in program.AuthorizationDetails.OrderByDescending(x => x.AuthorizationDecisionDate))
                        {
                            if (currentAuthDetail)
                            {
                                programValues.Add(new()
                                {
                                    value = authorizationDetail.AuthorizationEndDate?.ToString("d"),
                                    color = null
                                });
                                currentAuthDetail = false;
                            }
                            programValues.AddRange(new List<(object value, string color)>
                            {
                                new()
                                {
                                    value = authorizationDetail.AuthorizationCategory,
                                    color = authorizationDetail.ColorCode
                                },
                                new()
                                {
                                    value = authorizationDetail.VisitDate?.ToString("d")
                                },
                                new()
                                {
                                    value = authorizationDetail.AuthorizationDecisionDate?.ToString("d")
                                },
                                new()
                                {
                                    value = authorizationDetail.AuthorizationDecisionNo
                                }
                            });
                        }
                    }

                    foreach (var value in programValues)
                    {
                        CellStyles(worksheet, row, ref column, value.Item1, value.Item2);
                    }

                    column = 1;
                    row++;
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
        private static void MainTitles(IXLWorksheet worksheet, int column, string titleName, double width, bool isGray)
        {
            string backgroundColor = isGray ? "#D0CECE" : "#F2F2F2";
            worksheet.Cell(1, column).Value = titleName;
            worksheet.Cell(1, column).WorksheetColumn().Width = width;
            worksheet.Cell(1, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml(backgroundColor));
            worksheet.Cell(1, column).Style.Font.Bold = true;
        }
        private static void CellStyles(IXLWorksheet worksheet, int row, ref int column, object value, string backgroundColor)
        {
            worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);

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

            if (centeredTitleCount >= column)
            {
                worksheet.Cell(row, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            }

            column++;
        }
        private static XLColor ConvertToColorArray(string rgbColor)
        {
            rgbColor = rgbColor.StringFilter();

            var splitString = rgbColor.Split(',');
            var splitInts = splitString.Select(item => int.Parse(item)).ToArray();

            return XLColor.FromArgb(splitInts[0], splitInts[1], splitInts[2]);
        }
        private static string StringFilter(this string str)
        {
            List<string> strList = new() { "rgb", "(", ")" };

            foreach (string s in strList)
            {
                str = str.Replace(s, string.Empty);
            }

            return str;
        }
    }
}