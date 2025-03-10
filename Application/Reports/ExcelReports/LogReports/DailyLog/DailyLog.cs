using ClosedXML.Excel;
using Core.Models.DetailedReportModels;
using Core.Models.LogInformation;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Reports.ExcelReports.LogReports.DailyLog
{
    public class DailyLog
    {
        public static byte[] DetailLogReport(List<DetailedLogInformation> detailedLogs)
        {
            using (var workbook = new XLWorkbook())
            {
                //var summaryWS = workbook.Worksheets.Add("İcmal Tablosu");
                //IXLSheetView summaryView = summaryWS.SheetView;
                //summaryView.ZoomScale = 70;
                //Summary.Report(summaryWS, );

                var logWS = workbook.Worksheets.Add("Detay Tablosu");
                IXLSheetView logView = logWS.SheetView;
                logView.ZoomScale = 70;
                Detail.DailyLogReport(logWS, detailedLogs);

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
