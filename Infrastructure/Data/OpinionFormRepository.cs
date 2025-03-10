using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class OpinionFormRepository : EfRepository<OpinionForm>, IOpinionFormRepository
    {
        private readonly ApplicationDbContext dbContext;
        public OpinionFormRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<OpinionForm>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            return await dbContext.OpinionForms.AsSplitQuery()
                                               .Where(x => x.StudentId == studentId && x.IsDeleted == false)
                                               .ToListAsync(cancellationToken);
        }

        public async Task<List<OpinionForm>> GetListByStudentAndProgramId(CancellationToken cancellationToken, long studentId, long programId)
        {
            return await dbContext.OpinionForms.AsSplitQuery()
                                               .Where(x => x.StudentId == studentId && x.ProgramId == programId && x.IsDeleted == false)
                                               .ToListAsync(cancellationToken);
        }

        public async Task<OpinionForm> GetByOpinionId(CancellationToken cancellationToken, long id)
        {
            return await dbContext.OpinionForms.AsSplitQuery().AsNoTracking()
                                               .Include(x => x.Student).ThenInclude(x => x.User)
                                               .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
        }

        public async Task<Educator> GetEducatorByStudentId(CancellationToken cancellationToken, long studentId, long userId)
        {
            var query = from s in dbContext.Students
                        join p in dbContext.Programs on s.ProgramId equals p.Id
                        join ep in dbContext.EducatorPrograms on p.Id equals ep.ProgramId
                        where s.Id == studentId && ep.Educator.User.Id == userId
                        select ep.Educator;
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<EducationOfficer> GetProgramManagerByStudentId(CancellationToken cancellationToken, long studentId)
        {
            var query = from s in dbContext.Students
                        join p in dbContext.Programs on s.ProgramId equals p.Id
                        join eo in dbContext.EducationOfficers on p.Id equals eo.ProgramId
                        where s.Id == studentId && eo.EndDate == null
                        select eo;
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        //Süre uzatma geldiyse kanaat formlarının başlangıç ve bitiş tarihlerini artır
        public async Task ChangeDateByTimeIncreasing(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO, int? dayDiff = null)
        {
            var opinionForms = await GetListByStudentId(cancellationToken, (long)educationTrackingDTO.StudentId);

            var opinionForm = opinionForms.FirstOrDefault(x => x.StartDate <= educationTrackingDTO.ProcessDate && x.EndDate >= educationTrackingDTO.ProcessDate);

            if (opinionForm != null)
            {
                var selectedOpinionForms = opinionForms.Where(x => x.StartDate >= opinionForm.StartDate).ToList();
                for (int i = 0; i < selectedOpinionForms.Count; i++)
                {
                    if (i != 0 || educationTrackingDTO.ProcessType != ProcessType.TimeIncreasing)
                        selectedOpinionForms[i].StartDate = selectedOpinionForms[i].StartDate?.Date.AddDays(dayDiff != null ? (int)dayDiff : (int)educationTrackingDTO.AdditionalDays);
                    selectedOpinionForms[i].EndDate = selectedOpinionForms[i].EndDate?.Date.AddDays(dayDiff != null ? (int)dayDiff : (int)educationTrackingDTO.AdditionalDays);
                    dbContext.Entry(selectedOpinionForms[i]).State = EntityState.Modified;
                }
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<ResponseWrapper<EducationTrackingDTO>> CheckTransferPossible(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO)
        {
            var checkDatelessRecord = await dbContext.EducationTrackings.AnyAsync(x => x.IsDeleted == false && x.StudentId == educationTrackingDTO.StudentId && x.ProcessDate == null, cancellationToken);
            if (checkDatelessRecord)
                return new() { Result = false, Message = "Eğitim süre takibinde işlem tarihi girilmemiş kayıt bulunmaktadır. Kanaat formu ekleyebilmek için o kayda tarih girmelisiniz. Tarihi girilmemiş kayıtlar en üstte görünür." };

            var educationStart = await dbContext.EducationTrackings.OrderBy(x => x.ProcessDate).FirstOrDefaultAsync(x => x.StudentId == educationTrackingDTO.StudentId && x.IsDeleted == false && x.ProcessType == ProcessType.Start, cancellationToken);

            var student = await dbContext.Students.Include(x => x.Curriculum).FirstOrDefaultAsync(x => x.Id == educationTrackingDTO.StudentId, cancellationToken);

            if (student.Curriculum == null || student.Curriculum.Duration == null)
                return new() { Result = false, Message = "The student is not subject to any curriculum or the duration of the curriculum he is subject to is empty. Select the curriculum from the student details or inform the system administrator that the duration of the curriculum is empty." };

            var periodCount = student.Curriculum?.Duration * 2;

            var opinionForms = new List<OpinionForm>() { new() { Period = PeriodType.Period_1, StartDate = educationStart?.ProcessDate, EndDate = educationStart?.ProcessDate?.AddMonths(6).AddDays(-1) } };

            var timeIncreasings = await dbContext.EducationTrackings.OrderBy(x => x.ProcessDate).Where(x => x.IsDeleted == false && x.StudentId == educationTrackingDTO.StudentId && x.ProcessType == ProcessType.TimeIncreasing && x.ProcessDate >= opinionForms.FirstOrDefault().StartDate && x.ReasonType != ReasonType.ExtensionByApplicationForSubjection && x.ReasonType != ReasonType.TimeExtensionDueToFailureInRotation && x.ReasonType != ReasonType.ForceMajeure && x.ReasonType != ReasonType.DueToUnusualCircumstances).ToListAsync(cancellationToken);
            var timeWithoutEducation = await dbContext.EducationTrackings.OrderBy(x => x.ProcessDate).Where(x => !x.IsDeleted && x.StudentId == educationTrackingDTO.StudentId && x.ReasonType != ReasonType.BranchChange && (x.ProcessType == ProcessType.Transfer || x.ProcessType == ProcessType.Assignment || x.ProcessType == ProcessType.Start) && (x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment || x.ReasonType == ReasonType.BeginningToAssignedInstitution || x.ReasonType == ReasonType.CompletionOfAssignment || x.ReasonType == ReasonType.BeginningToOwnInstitutionAfterAssignment || x.ReasonType == ReasonType.Other || x.ReasonType == ReasonType.ExcusedTransfer || x.ReasonType == ReasonType.UnexcusedTransfer)).ToListAsync(cancellationToken);

            for (int i = 1; i < timeWithoutEducation.Count; i++)
                if (i % 2 == 1)
                    timeIncreasings.Add(new() { ProcessDate = timeWithoutEducation[i].ProcessDate, AdditionalDays = (timeWithoutEducation[i].ProcessDate?.Date - timeWithoutEducation[i - 1].ProcessDate?.Date)?.Days ?? 0 });

            if (timeIncreasings != null)
                foreach (var item in timeIncreasings)
                    if (item.ProcessDate <= opinionForms.FirstOrDefault().EndDate)
                        opinionForms.FirstOrDefault().EndDate = opinionForms.FirstOrDefault().EndDate?.AddDays(item.AdditionalDays ?? 0);

            for (int i = 1; i < periodCount; i++)
            {
                //eklenecek formun dönemi = i+1
                opinionForms.Add(new() { Period = (PeriodType)i+1, StartDate = opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i)?.EndDate?.AddDays(1).Date, EndDate = opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i)?.EndDate?.AddMonths(6).Date });

                var timeIncreasingsInPeriod = timeIncreasings.Where(x => x.ProcessDate >= opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i+1)?.StartDate);

                if (timeIncreasingsInPeriod != null)
                    foreach (var item in timeIncreasingsInPeriod)
                        if (item.ProcessDate < opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i + 1).EndDate)
                            opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i + 1).EndDate = opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i + 1).EndDate?.AddDays(item.AdditionalDays ?? 0);
            }

            var currentOpinionForm = opinionForms.FirstOrDefault(x => x.StartDate <= educationTrackingDTO.ProcessDate && x.EndDate >= educationTrackingDTO.ProcessDate);

            var checkOpForm_1 = await dbContext.OpinionForms.AnyAsync(x => x.StudentId == educationTrackingDTO.StudentId && !x.IsDeleted && x.CancellationDate == null && x.StartDate <= educationTrackingDTO.ProcessDate && x.EndDate >= educationTrackingDTO.ProcessDate, cancellationToken);
            //dönemin içinde eğitim alınmayan zaman
            var nonEducationalTime = timeIncreasings.Where(x => x.ProcessDate >= currentOpinionForm?.StartDate && x.ProcessDate <= educationTrackingDTO.ProcessDate).Sum(x => x.AdditionalDays);
            //dönem başlangıcından nakil istenen süreye kadar eğitim alınan süre
            var daysFromOpFormStart = (educationTrackingDTO.ProcessDate?.Date - currentOpinionForm?.StartDate?.Date)?.Days - nonEducationalTime;

            if (checkOpForm_1 || daysFromOpFormStart < 91 || student.Status == StudentStatus.TransferDueToNegativeOpinion)
                return new() { Result = true };
            else
                return new() { Result = false, Message = "You cannot transfer the student without saving the student's opinion information!" };
        }
    }
}
