using ClosedXML.Excel;
using Core.Entities;
using Core.Models.DetailedReportModels;
using DocumentFormat.OpenXml.Spreadsheet;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;

namespace Application.Reports.ExcelReports.HospitalReports.HospitalDetail;

public class HospitalDetail
{
    public static byte[] HospitalDetailReport(List<CountByExpertiseBranch> staffCountbyExpBranch, List<ProgramsStaffCount> programs, List<EducatorPaginateResponseDTO> educators, List<StudentExcelExportModel> students)
    {
        using (var workbook = new XLWorkbook())
        {
            var expBranchSheet = workbook.Worksheets.Add("İcmal Tablosu");
            StaffCountByExpertiseBranch.Report(expBranchSheet, staffCountbyExpBranch);
            
            var summaryWorksheet = workbook.Worksheets.Add("Detaylı İcmal Tablosu");
            IXLSheetView view = summaryWorksheet.SheetView;
            view.ZoomScale = 70;
            Summary.Report(summaryWorksheet, programs);

            var educatorsWorksheet = workbook.Worksheets.Add("Eğitici Listesi");
            IXLSheetView view1 = educatorsWorksheet.SheetView;
            view1.ZoomScale = 70;
            Educators.EducatorsSheet(educatorsWorksheet, educators);

            var studentsWorksheet = workbook.Worksheets.Add("Öğrenci Listesi");
            IXLSheetView view2 = studentsWorksheet.SheetView;
            view2.ZoomScale = 85;
            Students.StudentsSheet(studentsWorksheet, students);

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return content;
            }
        }
    }
}