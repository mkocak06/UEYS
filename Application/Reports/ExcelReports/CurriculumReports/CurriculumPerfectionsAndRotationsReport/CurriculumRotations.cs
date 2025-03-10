using Application.Reports.ExcelReports.Helpers;
using ClosedXML.Excel;
using Shared.ResponseModels.Curriculum;

namespace Application.Reports.ExcelReports.CurriculumReports.CurriculumPerfectionsAndRotationsReport
{
	public static class CurriculumRotations
    {
        private static void TitleStyle(IXLWorksheet worksheet, int row, int column, string value, double width)
        {
            worksheet.Cell(row, column).Value = value;
			worksheet.Cell(row, column).WorksheetColumn().Width = width;
            worksheet.Cell(row, column).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(row, column).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cell(row, column).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9E1F2"));
            worksheet.Cell(row, column).Style.Font.FontSize = 14;
            worksheet.Cell(row, column).Style.Font.Bold = true;
        }
        private static void DataStyle(IXLWorksheet worksheet, int row, ref int column, object value)
        {
            if (value is decimal || value is double || value is int)
            {
                worksheet.Cell(row, column).Style.NumberFormat.Format = "#,##0";
            }

            worksheet.Cell(row, column).Value = SanitizeForExcel.SanitizeInput(value);
			worksheet.Cell(row, column).Style.Border.SetOutsideBorderColor(XLColor.Black);
            worksheet.Cell(row, column).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cell(row, column).Style.Font.FontColor = XLColor.Black;

            column++;
        }
        public static void Report(IXLWorksheet worksheet, List<CurriculumExportModel> curriculumExports)
        {
            worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Style.Alignment.WrapText = true;
            worksheet.Columns().AdjustToContents();
            worksheet.SheetView.FreezeRows(1);

            var titles = new List<(string title, double width)>()
            {
                ("S.No", 6),
                ("Uzmanlık Alanı İsmi", 30),
                ("Yürürlük Tarihi", 15),
                ("Eğitim Alanı", 15),
                ("Eğitim Süresi (Yıl)", 15),
                ("Versiyon", 15),
                ("Durum", 15),
                ("Rotasyon Dalı", 30),
                ("Rotasyon Süresi", 15),
                ("Rotasyon Zorunlu mu?", 15),
                ("Yetkinlik Türü", 15),
                ("Grup", 30),
                ("İsim", 30),
                ("Eğitim Yılı-Kıdem", 15),
                ("Düzey", 15),
                ("Yöntem", 15),
            };

            int row = 1;
            for (int i = 0; i < titles.Count; i++)
            {
                TitleStyle(worksheet, row, i + 1, titles[i].title, titles[i].width);
            }

            worksheet.Range(1, 1, 1, titles.Count).SetAutoFilter();

            int column = 1;
            foreach (var curriculum in curriculumExports)
            {
                if (curriculum.Rotations?.Count > 0)
                {
                    foreach (var rotation in curriculum.Rotations)
                    {
                        if (rotation.Perfections?.Count > 0)
                        {
                            foreach (var perfection in rotation.Perfections)
                            {
                                row++;
                                var perfectionValues = new List<object>
                                {
                                    row - 1,
                                    curriculum.CurriculumName,
                                    curriculum.EffectiveDate,
                                    curriculum.ProfessionName,
                                    curriculum.CurriculumDuration,
                                    curriculum.CurriculumVersion,
                                    curriculum.IsActive,
                                    rotation.RotationName,
                                    rotation.RotationDuration,
                                    rotation.IsRequired,
                                    perfection.PerfectionType,
                                    perfection.GroupName,
                                    perfection.PerfectionName,
                                    perfection.SeniorityName,
                                    perfection.Levels,
                                    perfection.Methods
                                };

                                foreach (var value in perfectionValues)
                                {
                                    DataStyle(worksheet, row, ref column, value);
                                }
                                column = 1;
                            }
                        }
                        else
                        {
                            row++;
                            var rotationValues = new List<object>
                            {
                                row - 1,
                                curriculum.CurriculumName,
                                curriculum.EffectiveDate,
                                curriculum.ProfessionName,
                                curriculum.CurriculumDuration,
                                curriculum.CurriculumVersion,
                                curriculum.IsActive,
                                rotation.RotationName,
                                rotation.RotationDuration
                            };
                            foreach (var value in rotationValues)
                            {
                                DataStyle(worksheet, row, ref column, value);
                            }
                            column = 1;
                        }
                    }
                }
                else
                {
                    row++;
                    var curriculumValues = new List<object>
                    {
                        row - 1,
                        curriculum.CurriculumName,
                        curriculum.EffectiveDate,
                        curriculum.ProfessionName,
                        curriculum.CurriculumDuration,
                        curriculum.CurriculumVersion,
                        curriculum.IsActive
                    };

                    foreach (var value in curriculumValues)
                    {
                        DataStyle(worksheet, row, ref column, value);
                    }
                    column = 1;
                }
            }
        }
    }
}

