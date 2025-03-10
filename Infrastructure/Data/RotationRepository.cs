using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Koru;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Types;

namespace Infrastructure.Data
{
    public class RotationRepository : EfRepository<Rotation>, IRotationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public RotationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Rotation>> GetListByCurriculumId(CancellationToken cancellationToken, long curriculumId)
        {
            return await dbContext.CurriculumRotations.AsSplitQuery()
                                                      .Include(x => x.Rotation).ThenInclude(x => x.ExpertiseBranch)
                                                      .Where(x => x.CurriculumId == curriculumId && x.Rotation.IsDeleted == false && x.Curriculum.IsDeleted == false)
                                                      .Select(x => x.Rotation)
                                                      .ToListAsync();
        }
        public async Task<Rotation> GetByCurriculumIdAndExpBranchName(CancellationToken cancellationToken, long curriculumId, string rotationName)
        {
            return await dbContext.CurriculumRotations.Where(x => x.CurriculumId == curriculumId && x.Rotation.IsDeleted == false && x.Rotation.ExpertiseBranch.Name == rotationName && x.Curriculum.IsDeleted == false)
                                                      .Select(x => x.Rotation)
                                                      .FirstOrDefaultAsync();
        }

        public async Task<List<CurriculumRotation>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            var student = dbContext.Students.FirstOrDefault(x => x.Id == studentId);

            return await dbContext.CurriculumRotations.Include(x => x.Perfections.Where(a => a.IsDeleted == false)).ThenInclude(x => x.PerfectionProperties).ThenInclude(x => x.Property)
                                  .Include(x => x.Rotation).ThenInclude(x => x.ExpertiseBranch)
                                  .Include(x => x.Rotation).ThenInclude(x => x.Perfections.Where(a => a.IsDeleted == false)).ThenInclude(x => x.PerfectionProperties).ThenInclude(x => x.Property)
                                  .Include(x => x.Rotation).ThenInclude(x => x.StudentRotations.Where(x => x.IsDeleted == false && x.StudentId == studentId))
                                  .Where(x => x.CurriculumId == student.CurriculumId && x.IsDeleted == false).ToListAsync(cancellationToken);
        }

        public async Task<List<Rotation>> GetFormerStudentListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            var student = dbContext.Students.FirstOrDefault(x => x.Id == studentId);

            var formerStudent = await dbContext.Students.FirstOrDefaultAsync(x => x.UserId == student.UserId && x.EducationTrackings.Any(x => x.ReasonType == ReasonType.BranchChange_End), cancellationToken);

            if (formerStudent != null)
            {
                return await dbContext.CurriculumRotations
                    .Include(x => x.Rotation).ThenInclude(x => x.ExpertiseBranch)
                    .Include(x => x.Rotation).ThenInclude(x => x.StudentRotations.Where(x => x.IsDeleted == false))
                    .Where(x => x.CurriculumId == formerStudent.CurriculumId && x.IsDeleted == false).Select(x => x.Rotation).ToListAsync(cancellationToken);
            }
            return new();
        }

        public async Task<Rotation> GetByIdWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Rotations
                .Include(x => x.ExpertiseBranch).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task SetZoneOfEducators(CancellationToken cancellationToken)
        {
            var educatorPrograms = await dbContext.EducatorPrograms.Where(x => x.Id < 500 && (x.DutyEndDate == null || x.DutyEndDate < DateTime.UtcNow) && x.Educator.UserId != null).Include(x => x.Educator.User.UserRoles).AsNoTracking().ToListAsync(cancellationToken);

            foreach (var item in educatorPrograms)
            {
                var userRoleProgram = new UserRoleProgram() { CreatedDate = (DateTime)item.Educator.CreateDate, ProgramId = item.ProgramId, UserRoleId = item.Educator.User.UserRoles.FirstOrDefault(x => x.RoleId == 66)?.Id };
                dbContext.UserRolePrograms.Add(userRoleProgram);
            }
        }

        public async Task SetZoneOfStudents(CancellationToken cancellationToken)
        {
            var students = await dbContext.Students.AsSplitQuery().Where(x => x.UserId != null && x.Id >= 41000).Include(x => x.User.UserRoles).ThenInclude(x => x.UserRoleStudents).Select(x => new UserRoleStudent() { StudentId = x.Id, CreatedDate = (DateTime)x.CreateDate, UserRoleId = x.User.UserRoles.FirstOrDefault(y => y.RoleId == 65).Id }).ToListAsync(cancellationToken);

            foreach (var item in students)
            {
                dbContext.UserRoleStudents.Add(item);
            }
            await dbContext.SaveChangesAsync();
        }

        //belirli müfredata ait öðrencilerin eðitim sürelerini düzenler
        public async Task ReArrangeEducationTime(CancellationToken cancellationToken)
        {
            var students = await dbContext.Students.Include(x => x.EducationTrackings.Where(x => x.IsDeleted == false)).Where(x => x.CurriculumId == 42 && x.IsDeleted == false && x.IsHardDeleted == false && x.EducationTrackings.Any(x => x.ReasonType == ReasonType.ExtensionByApplicationForSubjection && x.IsDeleted == false) == false).ToListAsync(cancellationToken);

            foreach (var item in students)
            {
                var estFinish = item.EducationTrackings.First(x => x.ReasonType == ReasonType.EstimatedFinish);
                var start = item.EducationTrackings.First(x => x.ReasonType == ReasonType.BeginningSpecializationEducation);
                var sumOfTimeIncreasings = item.EducationTrackings.Where(x => x.ProcessType == ProcessType.TimeIncreasing).Sum(x => x.AdditionalDays);

                var asdasdads = estFinish?.ProcessDate?.Date.AddDays(-(int)sumOfTimeIncreasings) - start?.ProcessDate?.Date;

                if (asdasdads?.Days < 1200)
                {
                    estFinish.ProcessDate = estFinish.ProcessDate?.AddYears(1);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
