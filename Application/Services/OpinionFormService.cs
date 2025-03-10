using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    public class OpinionFormService : BaseService, IOpinionFormService
    {
        private readonly IMapper mapper;
        private readonly IOpinionFormRepository opinionFormRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IEducationTrackingRepository educationTrackingRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;
        private readonly IProgramRepository programRepository;

        public OpinionFormService(IOpinionFormRepository opinionFormRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IStudentRepository studentRepository, IDocumentRepository documentRepository, IKoruRepository koruRepository, IProgramRepository programRepository, IEducationTrackingRepository educationTrackingRepository) : base(unitOfWork)
        {
            this.opinionFormRepository = opinionFormRepository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.studentRepository = studentRepository;
            this.documentRepository = documentRepository;
            this.koruRepository = koruRepository;
            this.programRepository = programRepository;
            this.educationTrackingRepository = educationTrackingRepository;
        }

        public async Task<PaginationModel<OpinionFormResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<OpinionForm> ordersQuery = opinionFormRepository.IncludingQueryable(x => true, x => x.Secretary, x => x.Student.User, x => x.Educator.User, x => x.Educator.StaffTitle, x => x.ProgramManager.User, x => x.ProgramManager.StaffTitle, x => x.Program.Hospital, x => x.Program.Hospital.Province, x => x.Program.ExpertiseBranch);

            FilterResponse<OpinionForm> filterResponse = ordersQuery.ToFilterView(filter);

            var opinionForms = mapper.Map<List<OpinionFormResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            for (int i = 0; i < opinionForms.Count; i++)
            {
                var documents = await documentRepository.GetByEntityId(cancellationToken, (long)opinionForms[i].Id, DocumentTypes.OpinionForm);
                var communiqueDocuments = await documentRepository.GetByEntityId(cancellationToken, (long)opinionForms[i].Id, DocumentTypes.Communique);
                opinionForms[i].Documents = mapper.Map<List<DocumentResponseDTO>>(documents);
                for (int j = 0; j < communiqueDocuments?.Count; j++)
                    opinionForms[i].Documents.Add(mapper.Map<DocumentResponseDTO>(communiqueDocuments[j]));
                if (opinionForms[i].EducatorId == null)
                    opinionForms[i].Program = null;
            }

            var response = new PaginationModel<OpinionFormResponseDTO>
            {
                Items = opinionForms,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
            return response;
        }

        public async Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetCanceledListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            return new() { Result = true, Item = mapper.Map<List<OpinionFormResponseDTO>>(await opinionFormRepository.GetIncludingList(cancellationToken, x => x.FormStatusType == FormStatusType.Canceled && !x.IsDeleted && x.StudentId == studentId, x => x.Secretary, x => x.Student.User, x => x.Educator.User, x => x.Educator.StaffTitle, x => x.ProgramManager.User, x => x.ProgramManager.StaffTitle, x => x.Program.Hospital, x => x.Program.Hospital.Province, x => x.Program.ExpertiseBranch)) };
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            OpinionForm opinionForm = await opinionFormRepository.GetByOpinionId(cancellationToken, id);

            var response = mapper.Map<OpinionFormResponseDTO>(opinionForm);

            return new ResponseWrapper<OpinionFormResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> GetStartAndEndDates(CancellationToken cancellationToken, long studentId)
        {
            OpinionFormResponseDTO response = new();
            var checkDatelessRecord = await educationTrackingRepository.AnyAsync(cancellationToken, x => x.IsDeleted == false && x.StudentId == studentId && x.ProcessDate == null);
            if (checkDatelessRecord)
                return new() { Result = false, Message = "Eğitim süre takibinde işlem tarihi girilmemiş kayıt bulunmaktadır. Kanaat formu ekleyebilmek için o kayda tarih girmelisiniz. Tarihi girilmemiş kayıtlar en üstte görünür." };
            var estimatedFinish = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.IsDeleted == false && x.StudentId == studentId && x.ProcessType == ProcessType.EstimatedFinish);
            var lastOpinionForm = await opinionFormRepository.Queryable().OrderByDescending(x => x.StartDate).FirstOrDefaultAsync(x => x.IsDeleted == false && x.CancellationDate == null && x.StudentId == studentId, cancellationToken);
            var educationStart = await educationTrackingRepository.Queryable().OrderBy(x => x.ProcessDate).FirstOrDefaultAsync(x => x.StudentId == studentId && x.IsDeleted == false && x.ProcessType == ProcessType.Start, cancellationToken);

            if (lastOpinionForm != null)
            {
                response.StartDate = lastOpinionForm.EndDate?.AddDays(1) ?? educationStart?.ProcessDate;
                response.Period = lastOpinionForm.Period + 1;
            }
            else
            {
                response.StartDate = educationStart?.ProcessDate;
                response.Period = PeriodType.Period_1;
            }
            response.EndDate = response.StartDate?.AddMonths(6).AddDays(-1);

            var timeIncreasings = await educationTrackingRepository.Queryable().OrderBy(x => x.ProcessDate).Where(x => x.IsDeleted == false && x.StudentId == studentId && x.ProcessType == ProcessType.TimeIncreasing && x.ProcessDate >= response.StartDate && x.ReasonType != ReasonType.ExtensionByApplicationForSubjection && x.ReasonType != ReasonType.TimeExtensionDueToFailureInRotation && x.ReasonType != ReasonType.ForceMajeure && x.ReasonType != ReasonType.DueToUnusualCircumstances).ToListAsync(cancellationToken);

            var timeWithoutEducation = await educationTrackingRepository.Queryable().OrderBy(x => x.ProcessDate).Where(x => !x.IsDeleted && x.StudentId == studentId && x.ProcessDate >= response.StartDate && x.ReasonType != ReasonType.BranchChange && (x.ProcessType == ProcessType.Transfer || x.ProcessType == ProcessType.Assignment || x.ProcessType == ProcessType.Start) && (x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment || x.ReasonType == ReasonType.BeginningToAssignedInstitution || x.ReasonType == ReasonType.CompletionOfAssignment || x.ReasonType == ReasonType.BeginningToOwnInstitutionAfterAssignment || x.ReasonType == ReasonType.Other || x.ReasonType == ReasonType.ExcusedTransfer || x.ReasonType == ReasonType.UnexcusedTransfer)).ToListAsync(cancellationToken);

            for (int i = 1; i < timeWithoutEducation.Count; i++)
                if (i % 2 == 1)
                    timeIncreasings.Add(new() { ProcessDate = timeWithoutEducation[i].ProcessDate, AdditionalDays = (timeWithoutEducation[i].ProcessDate?.Date - timeWithoutEducation[i - 1].ProcessDate?.Date)?.Days ?? 0 });

            if (timeIncreasings != null)
                foreach (var item in timeIncreasings)
                    if (item.ProcessDate <= response.EndDate)
                        response.EndDate = response.EndDate?.AddDays(item.AdditionalDays ?? 0);

            if (response?.EndDate?.Date > estimatedFinish?.ProcessDate?.Date)
                response.EndDate = estimatedFinish.ProcessDate;

            return new ResponseWrapper<OpinionFormResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            List<OpinionForm> opinionForms = await opinionFormRepository.GetListByStudentId(cancellationToken, studentId);

            var response = mapper.Map<List<OpinionFormResponseDTO>>(opinionForms);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> PostAsync(CancellationToken cancellationToken, OpinionFormDTO opinionFormDTO)
        {
            //var result= await opinionFormRepository.GetEducatorByStudentId(cancellationToken, (long)opinionFormDTO.StudentId, httpContextAccessor.HttpContext.GetUserId());
            //if (result != null)
            //{
            OpinionForm opinionForm = mapper.Map<OpinionForm>(opinionFormDTO);
            var student = await studentRepository.GetByIdAsync(cancellationToken, (long)opinionFormDTO.StudentId);

            var educatorProgram = await opinionFormRepository.GetProgramManagerByStudentId(cancellationToken, (long)opinionFormDTO.StudentId);
            if (educatorProgram != null && opinionForm.Result != null)
                opinionForm.ProgramManagerId = educatorProgram.EducatorId;
            await opinionFormRepository.AddAsync(cancellationToken, opinionForm);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<OpinionFormResponseDTO>(opinionForm);

            var result = await CheckNegativeOpinions(cancellationToken, student.Id);

            var studentStatusChanged = false;
            if (result.Item.IsEndOfEducationNecessary == true)
            {
                student.Status = StudentStatus.EndOfEducationDueToNegativeOpinion;
                studentStatusChanged = true;
            }
            else if (result.Item.IsTransferNecessary == true)
            {
                student.Status = StudentStatus.TransferDueToNegativeOpinion;
                studentStatusChanged = true;
            }

            if (studentStatusChanged)
            {
                studentRepository.Update(student);
                await unitOfWork.CommitAsync(cancellationToken);
                response.Navigate = true;
            }
            return new() { Result = true, Item = response };
            //}
            //else
            //    return new() { Result = false, Message = "Öğrencinin programında kayıtlı eğitici olmadığınız için bu öğrenciyi değerlendiremezsiniz!" };
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> Put(CancellationToken cancellationToken, long id, OpinionFormDTO opinionFormDTO)
        {
            OpinionForm opinionForm = await opinionFormRepository.GetByIdAsync(cancellationToken, id);
            var student = await studentRepository.GetByIdAsync(cancellationToken, (long)opinionForm.StudentId);

            OpinionForm updatedOpinionForm = mapper.Map(opinionFormDTO, opinionForm);

            opinionFormRepository.Update(updatedOpinionForm);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<OpinionFormResponseDTO>(updatedOpinionForm);

            var result = await CheckNegativeOpinions(cancellationToken, student.Id);

            var studentStatusChanged = false;
            if (result.Item.IsEndOfEducationNecessary == true)
            {
                student.Status = StudentStatus.EndOfEducationDueToNegativeOpinion;
                studentStatusChanged = true;
            }
            else if (result.Item.IsTransferNecessary == true)
            {
                student.Status = StudentStatus.TransferDueToNegativeOpinion;
                studentStatusChanged = true;
            }
            else if (student.Status != StudentStatus.EducationContinues && (result.Item.IsEndOfEducationNecessary == false || result.Item.IsTransferNecessary == false))
            {
                student.Status = StudentStatus.EducationContinues;
                studentStatusChanged = true;
            }

            if (studentStatusChanged)
            {
                studentRepository.Update(student);
                await unitOfWork.CommitAsync(cancellationToken);
                response.Navigate = true;
            }

            return new ResponseWrapper<OpinionFormResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            OpinionForm opinionForm = await opinionFormRepository.GetByIdAsync(cancellationToken, id);

            opinionFormRepository.SoftDelete(opinionForm);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<OpinionFormResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> Cancellation(CancellationToken cancellationToken, long id)
        {
            OpinionForm opinionForm = await opinionFormRepository.GetByIdAsync(cancellationToken, id);
            var program = await programRepository.GetByIdAsync(cancellationToken, (long)opinionForm.ProgramId);

            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            if ((zone.Hospitals != null && zone.Hospitals.Any()) || zone.RoleCategory == RoleCategoryType.Admin || zone.RoleCategory == RoleCategoryType.Ministry)
            {
                var hospitalIds = zone.Hospitals?.Select(x => x.Id)?.ToList();
                if (hospitalIds?.Any(x => x == program.HospitalId) == true || zone.RoleCategory == RoleCategoryType.Admin || zone.RoleCategory == RoleCategoryType.Ministry)
                {
                    opinionForm.FormStatusType = FormStatusType.Canceled;
                    opinionForm.CancellationDate = DateTime.UtcNow;

                    opinionFormRepository.Update(opinionForm);
                    await unitOfWork.CommitAsync(cancellationToken);

                    return new ResponseWrapper<OpinionFormResponseDTO> { Result = true };
                }
            }
            return new() { Result = false, Message = "You are not authorized for this operation!" };
        }

        public async Task<ResponseWrapper<StudentStatusCheckDTO>> CheckNegativeOpinions(CancellationToken cancellationToken, long studentId)
        {
            var student = await studentRepository.GetByIdAsync(cancellationToken, studentId);

            var opinionForms = await opinionFormRepository.GetListByStudentAndProgramId(cancellationToken, student.Id, (long)student.ProgramId);

            var list = opinionForms.OrderBy(x => x.StartDate).Select(x => (x.Result, x.IsNotified)).ToList();
            var response = new StudentStatusCheckDTO() ;

            var result = list.Where((x, i) => i < list.Count - 1 ? list[i].Result == RatingResultType.Negative && list[i].IsNotified == true && list[i + 1].Result == RatingResultType.Negative && list[i+1].IsNotified == true : false).Count() > 0;

            if (result)
            {
                if (student.TransferredDueToOpinion == true)
                    response.IsEndOfEducationNecessary = true;
                else
                    response.IsTransferNecessary = true;
            }
            else
            {
                if (student.TransferredDueToOpinion == true && student.Status == StudentStatus.EndOfEducationDueToNegativeOpinion)
                    response.IsEndOfEducationNecessary = false;
                else if ((student.TransferredDueToOpinion == false || student.TransferredDueToOpinion == null) && student.Status == StudentStatus.TransferDueToNegativeOpinion)
                    response.IsTransferNecessary = false;
            }
            return new() { Item= response};
        }
    }
}
