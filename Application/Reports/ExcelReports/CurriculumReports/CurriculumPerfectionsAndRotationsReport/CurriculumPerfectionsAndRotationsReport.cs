using System;
using ClosedXML.Excel;
using Shared.ResponseModels.Curriculum;

namespace Application.Reports.ExcelReports.CurriculumReports.CurriculumPerfectionsAndRotationsReport
{
    public static class CurriculumPerfectionsAndRotationsReport
    {
        public static byte[] Report(List<CurriculumExportModel> curriculumExports)
        {
            using (var workbook = new XLWorkbook())
            {
                var perfectionsWS = workbook.Worksheets.Add("Müfredat Yetkinlikleri");
                IXLSheetView view = perfectionsWS.SheetView;
                view.ZoomScale = 90;
                CurriculumPerfections.Report(perfectionsWS, curriculumExports);

                var rotationWS = workbook.Worksheets.Add($"Müfredat Rotasyonları");
                IXLSheetView view1 = rotationWS.SheetView;
                view1.ZoomScale = 75;
                CurriculumRotations.Report(rotationWS, curriculumExports);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return content;
                }
            }
        }
    }
}

