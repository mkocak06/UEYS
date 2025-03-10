using Core.Entities;
using Core.Interfaces;
using Koru;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class EducationOfficerRepository : EfRepository<EducationOfficer>, IEducationOfficerRepository
    {
        private readonly ApplicationDbContext dbContext;
        public EducationOfficerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        //Gönderilen id'ye göre o eğitim sorumlusunun görevi sonlandırılır
        public async Task<ResponseWrapper<EducationOfficerResponseDTO>> FinishEducationOfficersDuty(CancellationToken cancellationToken, long id, DateTime? dateTime)
        {
            var officer = await dbContext.EducationOfficers.Include(x => x.Educator.User).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (officer != null)
            {
                var besRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Code == RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);
                var userRole = await dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == officer.Educator.UserId && x.RoleId == besRole.Id, cancellationToken);
                if (userRole != null)
                    dbContext.Entry(userRole).State = EntityState.Deleted;
                officer.EndDate = dateTime == null ? DateTime.UtcNow : dateTime;
                dbContext.Entry(officer).State = EntityState.Modified;
                await dbContext.SaveChangesAsync(cancellationToken);
                return new() { Result = true, Item = new() { EducatorId = officer.EducatorId, ProgramId = officer.ProgramId, StartDate = officer.StartDate, EndDate = officer.EndDate } };
            }
            return new() { Result = false, Message = "Birim Eğitim Sorumlusu Bulunamadı!" };
        }

        //Gönderilen programId'ye göre o programdaki eğitim sorumlusunun görevi sonlandırılır
        public async Task<ResponseWrapper<EducationOfficerResponseDTO>> FinishCurrentDuty(CancellationToken cancellationToken, long programId, DateTime? dateTime)
        {
            var officer = await dbContext.EducationOfficers.Include(x => x.Educator).FirstOrDefaultAsync(x => x.ProgramId == programId && x.EndDate == null, cancellationToken);
            if (officer != null)
            {
                var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Code == RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE, cancellationToken);
                var userRole = await dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == officer.Educator.UserId && x.RoleId == role.Id, cancellationToken);
                if (userRole != null) dbContext.UserRoles.Remove(userRole);
                officer.EndDate = dateTime == null ? DateTime.UtcNow : dateTime;
                dbContext.Entry(officer).State = EntityState.Modified;
                await dbContext.SaveChangesAsync(cancellationToken);
                return new() { Result = true, Item = new() { EducatorId = officer.EducatorId, ProgramId = officer.ProgramId, StartDate = officer.StartDate, EndDate = officer.EndDate } };
            }
            else
                return new() { Result = true, Item = new() };
        }

        public IQueryable<EducationOfficer> GetPaginateListForProgramDetailQuery(long programId)
        {
            return dbContext.EducationOfficers.AsSplitQuery()
                .Include(x => x.Educator).ThenInclude(x => x.User)
                .Include(x => x.Educator).ThenInclude(x => x.AcademicTitle)
                .Include(x => x.Educator).ThenInclude(x => x.StaffTitle)
                .Where(x => x.ProgramId == programId && x.EndDate != null && x.EducatorId != null && x.Educator.UserId != null);
        }
    }
}
