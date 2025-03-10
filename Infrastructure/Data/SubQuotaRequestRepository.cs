using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using Shared.ResponseModels;
using System.Collections.Generic;

namespace Infrastructure.Data
{
    public class SubQuotaRequestRepository : EfRepository<SubQuotaRequest>, ISubQuotaRequestRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SubQuotaRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<SubQuotaRequest> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.SubQuotaRequests.AsSplitQuery()
                .Include(x => x.Program).ThenInclude(x=>x.ExpertiseBranch)
                .Include(x => x.QuotaRequest)
                .Include(x => x.SubQuotaRequestPortfolios).ThenInclude(x => x.Portfolio)
                .Include(x => x.StudentCounts)
                .FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id, cancellationToken);
        }
        

        public async Task<SubQuotaRequest> GetByProgramIdWithSubRecords(CancellationToken cancellationToken,
            long programId)
        {
            return await dbContext.SubQuotaRequests.AsSplitQuery()
                .Include(x => x.SubQuotaRequestPortfolios.Where(y=>y.Portfolio.IsDeleted==false)).ThenInclude(x => x.Portfolio)
                .Include(x => x.StudentCounts)
                .FirstOrDefaultAsync(x => x.IsDeleted == false && x.ProgramId == programId, cancellationToken);
        }

        public IQueryable<SubQuotaRequestPaginateResponseDTO> PaginateQuery()
        {
            return dbContext.SubQuotaRequests.Select(x => new SubQuotaRequestPaginateResponseDTO()
            {
                Id = x.Id,
                ExpertiseBranchName = x.Program.ExpertiseBranch.Name,
                //IsDeleted = x.IsDeleted,
                HospitalName = x.Program.Hospital.Name,
                ProvinceName = x.Program.Hospital.Province.Name,
                AssociateProfessorCount = x.AssociateProfessorCount,
                ChiefAssistantCount = x.ChiefAssistantCount,
                DoctorLecturerCount = x.DoctorLecturerCount,
                ProfessorCount = x.ProfessorCount,
                SpecialistDoctorCount = x.SpecialistDoctorCount,
                TotalEducatorCount = x.TotalEducatorCount,
                CurrentStudentCount = x.CurrentStudentCount,
                StudentWhoLast6MonthToFinishCount = x.StudentWhoLast6MonthToFinishCount,
                Capacity = x.Capacity,
                CapacityIndex = x.CapacityIndex,
                EducatorIndex = x.EducatorIndex,
                PortfolioIndex = x.PortfolioIndex,
                ExpertiseBranchId = x.Program.ExpertiseBranchId,
                ProgramId = x.ProgramId,
                ProgramName = x.Program.Hospital.Name + " " + x.Program.ExpertiseBranch.Name,
                QuotaRequestId = x.QuotaRequestId,
                Portfolios = x.SubQuotaRequestPortfolios != null ? x.SubQuotaRequestPortfolios.Select(y=> new SubQuotaRequestPortfolioResponseDTO_2()
                {
                    Id = y.Id,
                    PortfolioId = y.PortfolioId,
                    PortfolioName = y.Portfolio.Name,
                    Ratio = y.Portfolio.Ratio,
                    Answer = y.Answer,
                    SubQuotaRequestId = x.Id,
                    
                }).ToList() : null,
                StudentCounts = x.StudentCounts != null ? x.StudentCounts.Select(y=> new StudentCountResponseDTO()
                {
                    Id=y.Id,
                    BoardAllocatedCount = y.BoardAllocatedCount,
                    RequestedCount = y.RequestedCount,
                    SecretaryAllocatedCount = y.SecretaryAllocatedCount,
                    QuotaType = y.QuotaType,
                    SubQuotaRequestId = x.Id
                }).ToList() : null
            });
        }
    }
}