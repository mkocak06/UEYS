using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels.ENabiz;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ExpertiseBranchRepository : EfRepository<ExpertiseBranch>, IExpertiseBranchRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ExpertiseBranchRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<ExpertiseBranch>> GetListRelatedWithProgramsByProfessionId(CancellationToken cancellationToken, long id)
        {
            return await (from e in dbContext.ExpertiseBranches
                          join p in dbContext.Programs on e.Id equals p.ExpertiseBranchId
                          where p.IsDeleted == false && e.ProfessionId == id
                          select e).Distinct().OrderBy(x => x.Name).ToListAsync(cancellationToken);
        }

        public async Task<List<ExpertiseBranch>> GetListForProtocolProgramByHospitalId(CancellationToken cancellationToken, long hospitalId)
        {
            return await (from ep in dbContext.EducatorPrograms
                          join eeb in dbContext.EducatorExpertiseBranches on ep.EducatorId equals eeb.EducatorId
                          where eeb.ExpertiseBranchId == 49 && ep.Program.HospitalId == hospitalId && ep.Program.ExpertiseBranch.Id != 49
                          select ep.Program.ExpertiseBranch).Distinct().OrderBy(x => x.Name).ToListAsync(cancellationToken);
        }
        public IQueryable<ExpertiseBranch> QueryableList()
        {
            return dbContext.ExpertiseBranches.Include(x => x.Profession)
                                              .Include(x => x.SubBranches).ThenInclude(x => x.SubBranch)
                                              .Include(x => x.PrincipalBranches).ThenInclude(x => x.PrincipalBranch);
        }

        public async Task<ExpertiseBranch> GetById(CancellationToken cancellationToken, long id)
        {
            return await dbContext.ExpertiseBranches.Include(x => x.Profession)
                                              .Include(x => x.SubBranches).ThenInclude(x => x.SubBranch)
                                              .Include(x => x.PrincipalBranches).ThenInclude(x => x.PrincipalBranch)
                                              .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(ExpertiseBranch expertiseBranch)
        {
            var existExpertsiteBranch = await dbContext.ExpertiseBranches.AsNoTracking()
                                                                         .Include(x => x.SubBranches)
                                                                         .Include(x => x.PrincipalBranches)
                                                                         .FirstOrDefaultAsync(x => x.Id == expertiseBranch.Id);

            dbContext.Entry(expertiseBranch).State = EntityState.Modified;

            if (expertiseBranch.IsPrincipal == true)
            {
                if (existExpertsiteBranch?.SubBranches?.Count > 0)
                    foreach (var item in existExpertsiteBranch.SubBranches)
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                if (expertiseBranch?.SubBranches?.Count > 0)
                    foreach (var item in expertiseBranch.SubBranches)
                    {
                        dbContext.Entry(item).State = EntityState.Added;
                    }
            }
            else
            {
                if (existExpertsiteBranch?.PrincipalBranches?.Count > 0)
                    foreach (var item in existExpertsiteBranch.PrincipalBranches)
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                if (expertiseBranch?.PrincipalBranches?.Count > 0)
                    foreach (var item in expertiseBranch.PrincipalBranches)
                    {
                        dbContext.Entry(item).State = EntityState.Added;
                    }
            }


            //if (expertiseBranch.SubBranches != null && expertiseBranch.SubBranches.Count != 0)
            //{
            //    foreach (var branch in expertiseBranch.SubBranches)
            //    {
            //        dbContext.Entry(branch).State = EntityState.Modified;
            //    }

            //    //if (existExpertsiteBranch.SubBranches != null && existExpertsiteBranch.SubBranches.Count != 0) // TODO Expertise Branch
            //    //{
            //    //    var subBranchIds = expertiseBranch.SubBranches.Select(x => x.Id).ToList();

            //    //    foreach (var removedExpertiseBranch in existExpertsiteBranch.SubBranches.Where(x => !subBranchIds.Contains(x.Id)))
            //    //    {
            //    //        removedExpertiseBranch.PrincipalBranchId = null;
            //    //        dbContext.Entry(removedExpertiseBranch).State = EntityState.Modified;
            //    //    }
            //    //}
            //}
            //else
            //{
            //    if (existExpertsiteBranch.SubBranches != null && existExpertsiteBranch.SubBranches.Count != 0)
            //    {
            //        foreach (var removedExpertiseBranch in existExpertsiteBranch.SubBranches)
            //        {
            //            //removedExpertiseBranch.PrincipalBranchId = null; // TODO Expertise Branch a
            //            dbContext.Entry(removedExpertiseBranch).State = EntityState.Modified;
            //        }
            //    }
            //}

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<long>> GetSubBrachIds(CancellationToken cancellationToken, long id)
        {
            return await dbContext.RelatedExpertiseBranches.Where(x => x.PrincipalBranchId == id).Select(x => x.SubBranchId).ToListAsync(cancellationToken);
        }

        public long? GetLastCurriculum(long expBranchId)
        {
            return dbContext.Curricula.Where(x => x.ExpertiseBranchId == expBranchId).MaxBy(x => x.CreateDate)?.ExpertiseBranchId;
        }

        public async Task<List<ExpertiseBranchResponseDTO>> ExpertiseBranchListEnabiz(CancellationToken cancellationToken)
        {
            return await dbContext.ExpertiseBranches.Select(x => new ExpertiseBranchResponseDTO
            {
                Code = x.Code,
                Name = x.Name,
                Faculty = x.Profession.Name
            }).ToListAsync(cancellationToken);
        }
    }
}
