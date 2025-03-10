using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class LogRepository : EfRepository<Log>, ILogRepository
    {
        private readonly ApplicationDbContext dbContext;
        public LogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<string>> SystemLogInformation()
        {
            var yesterday = DateTime.UtcNow.AddDays(-1).Date;

            var logMessages = await dbContext.Logs.AsNoTracking()
                .Where(x => x.CreateDate.Date == yesterday && (x.Message.Contains("/api/Student/Get , QueryString") || x.Message.Contains("/api/Educator/Get/")))
                .Select(x => x.Message)
                .ToListAsync();

            return logMessages;
        }
        //public async Task<List<LogInformation>> SystemLogInformation()
        //{
        //    var yesterday = DateTime.UtcNow.AddDays(-1).Date;

        //    var logMessages = await dbContext.Logs.AsNoTracking()
        //        .Where(x => x.CreateDate.Date == yesterday)
        //        .Select(x => x.Message).Take(2000)
        //        .ToListAsync();

        //    //var pathRegex = new Regex(@"(/api/\w+/[\w-]+)");
        //    var pathRegex = new Regex(@"(/api/\w+/[\w/]+)");
        //    var logSummary = logMessages
        //        .Select(log =>
        //        {
        //            var method = ExtractValue(log, "Request.Method");
        //            var statusCode = ExtractValue(log, "Response.StatusCode");

        //            var pathMatch = pathRegex.Match(ExtractValue(log, "Request.Path"));
        //            var path = pathMatch.Success ? pathMatch.Value : "Unknown";

        //            return new { method, path, statusCode };
        //        })
        //        .Select(group => new LogInformation()
        //        {
        //            Path = group.path,
        //        })
        //        .OrderByDescending(x => x.Count)
        //        .ToList();

        //    return logSummary;

        //    //var yesterdayStart = DateTime.UtcNow.AddDays(-1);
        //    //var yesterdayEnd = DateTime.UtcNow;

        //    //var logMessages = await dbContext.Logs.AsNoTracking()
        //    //    .Where(x => x.CreateDate >= yesterdayStart && x.CreateDate < yesterdayEnd)
        //    //    .Select(x => new { x.Message, x.UserId }).Take(2000)
        //    //    .ToListAsync();

        //    //var pathRegex = new Regex(@"(/api/\w+/[\w-]+)");

        //    //var logSummary = logMessages
        //    //    .Select(log =>
        //    //    {
        //    //        var method = ExtractValue(log.Message, "Request.Method");
        //    //        var statusCode = ExtractValue(log.Message, "Response.StatusCode");

        //    //        var pathMatch = pathRegex.Match(ExtractValue(log.Message, "Request.Path"));
        //    //        var path = pathMatch.Success ? pathMatch.Value : "Unknown";

        //    //        return new { method, path, statusCode };
        //    //    })
        //    //    .GroupBy(log => log.path)
        //    //    .Select(group => new LogInformation()
        //    //    {
        //    //        Path = group.Key,
        //    //        Count = group.Count()
        //    //    })
        //    //    .OrderByDescending(x => x.Count)
        //    //    .ToList();

        //    //return logSummary;
        //}
        private static string ExtractValue(string message, string fieldName)
        {
            var regex = new Regex($@"{fieldName} : (.*?)( ,|$)");
            var match = regex.Match(message);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
