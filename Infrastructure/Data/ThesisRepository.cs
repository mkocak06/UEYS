using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ThesisRepository : EfRepository<Thesis>, IThesisRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ThesisRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Thesis> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Theses.AsSplitQuery().AsNoTracking()
                                               .Include(x => x.AdvisorTheses).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                               .Include(x => x.AdvisorTheses).ThenInclude(x => x.Educator).ThenInclude(x => x.EducatorExpertiseBranches)
                                               .Include(x => x.AdvisorTheses).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.AdvisorTheses).ThenInclude(x => x.User)
                                               .Include(x => x.ThesisDefences.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Hospital)
                                               //.Include(x => x.ThesisDefences.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Juries).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                               //.Include(x => x.ThesisDefences.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Juries).ThenInclude(x => x.Educator).ThenInclude(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.EthicCommitteeDecisions.Where(x => x.IsDeleted == false))
                                               .Include(x => x.OfficialLetters.Where(x => x.IsDeleted == false))
                                               .Include(x => x.ProgressReports.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                               .Include(x => x.Student)
                                               .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Thesis>> GetListWithSubRecords(CancellationToken cancellationToken, long studentId)
        {
            return await dbContext.Theses.AsSplitQuery().AsNoTracking()
                                               .Include(x => x.AdvisorTheses).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                               .Include(x => x.ThesisDefences.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Hospital)
                                               .Include(x => x.ThesisDefences.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Juries).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                               .Include(x => x.ThesisDefences.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Juries).ThenInclude(x => x.Educator).ThenInclude(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.EthicCommitteeDecisions.Where(x => x.IsDeleted == false))
                                               .Include(x => x.OfficialLetters.Where(x => x.IsDeleted == false))
                                               .Include(x => x.ProgressReports.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                               .Include(x => x.Student)
                                               .Where(x => x.StudentId == studentId && x.IsDeleted == false)
                                               .ToListAsync(cancellationToken);
        }

        public IQueryable<ThesisResponseDTO> QueryableThesis()
        {
            return dbContext.Theses.AsNoTracking()
                .Select(x => new ThesisResponseDTO
                {
                    Id = x.Id,
                    Status = x.Status,
                    StudentId = x.StudentId,
                    Subject = x.Subject,
                    IsDeleted = x.IsDeleted,
                    ThesisTitle = x.ThesisTitle,
                    DeleteReasonExplanation = x.DeleteReasonExplanation,
                    AdvisorTheses = x.AdvisorTheses.Where(x => x.IsDeleted == false)
                    .Select(y => new AdvisorThesisResponseDTO()
                    {
                        IsCoordinator = y.IsCoordinator,
                        IsDeleted = y.IsDeleted,
                        Educator = new EducatorResponseDTO()
                        {
                            User = new()
                            {
                                Name = y.Educator.User.Name
                            }
                        },
                        User = new()
                        {
                            Name = y.User.Name
                        }
                    }).ToList(),
                    EthicCommitteeDecisions = x.EthicCommitteeDecisions.Where(x => x.IsDeleted == false)
                    .Select(x => new EthicCommitteeDecisionResponseDTO
                    {
                        Date = x.Date,
                        Number = x.Number,
                        Description = x.Description,
                    }).ToList()
                });
        }
        public async Task<Thesis> UpdateWithSubRecords(CancellationToken cancellationToken, long id, Thesis thesis)
        {
            Thesis existThesis = await GetWithSubRecords(cancellationToken, id);
            dbContext.Entry(thesis).State = EntityState.Modified;

            if (thesis.AdvisorTheses != null)
            {

                foreach (var item in thesis.AdvisorTheses)
                {
                    var existAdvisorThesis = existThesis.AdvisorTheses.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existAdvisorThesis == null ? EntityState.Added : EntityState.Modified;

                }
                var AdvisorThesesIds = thesis.AdvisorTheses.Select(r => r.Id).ToList();
                if (existThesis?.AdvisorTheses != null)
                {
                    foreach (var item in existThesis.AdvisorTheses.Where(x => !AdvisorThesesIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }

            if (thesis.EthicCommitteeDecisions != null)
            {

                foreach (var item in thesis.EthicCommitteeDecisions)
                {
                    var existEthicCommitteeDecision = existThesis.EthicCommitteeDecisions.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existEthicCommitteeDecision == null ? EntityState.Added : EntityState.Modified;

                }
                var EthicCommitteeDecisionsIds = thesis.EthicCommitteeDecisions.Select(r => r.Id).ToList();
                if (existThesis?.EthicCommitteeDecisions != null)
                {
                    foreach (var item in existThesis.EthicCommitteeDecisions.Where(x => !EthicCommitteeDecisionsIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }

            if (thesis.ProgressReports != null)
            {

                foreach (var item in thesis.ProgressReports)
                {
                    var existProgressReport = existThesis.ProgressReports.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existProgressReport == null ? EntityState.Added : EntityState.Modified;

                }
                var ProgressReportsIds = thesis.ProgressReports.Select(r => r.Id).ToList();
                if (existThesis?.ProgressReports != null)
                {
                    foreach (var item in existThesis.ProgressReports.Where(x => !ProgressReportsIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }
            if (thesis.ThesisDefences != null)
            {

                foreach (var item in thesis.ThesisDefences)
                {
                    var existThesisDefence = existThesis.ThesisDefences.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existThesisDefence == null ? EntityState.Added : EntityState.Modified;

                }
                var ThesisDefencesIds = thesis.ThesisDefences.Select(r => r.Id).ToList();
                if (existThesis?.ThesisDefences != null)
                {
                    foreach (var item in existThesis.ThesisDefences.Where(x => !ThesisDefencesIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }
            if (thesis.OfficialLetters != null)
            {

                foreach (var item in thesis.OfficialLetters)
                {
                    var existOfficialLetter = existThesis.OfficialLetters.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existOfficialLetter == null ? EntityState.Added : EntityState.Modified;

                }
                var OfficialLettersIds = thesis.OfficialLetters.Select(r => r.Id).ToList();
                if (existThesis?.OfficialLetters != null)
                {
                    foreach (var item in existThesis.OfficialLetters.Where(x => !OfficialLettersIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }

            await dbContext.SaveChangesAsync();
            return thesis;


        }

    }
}
