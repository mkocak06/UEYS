using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    public class AdvisorThesisService : BaseService, IAdvisorThesisService
    {
        private readonly IAdvisorThesisRepository advisorThesisRepository;
        private readonly IUserRepository userRepository;
        private readonly IKoruRepository koruRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IThesisRepository thesisRepository;
        private readonly IMapper mapper;
        public AdvisorThesisService(IAdvisorThesisRepository advisorThesisRepository, IKoruRepository koruRepository, IMapper mapper, IUnitOfWork unitOfWork, IUserRepository userRepository, IStudentRepository studentRepository, IThesisRepository thesisRepository) : base(unitOfWork)
        {
            this.advisorThesisRepository = advisorThesisRepository;
            this.koruRepository = koruRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.studentRepository = studentRepository;
            this.thesisRepository = thesisRepository;
        }
        public async Task<ResponseWrapper<List<AdvisorThesisResponseDTO>>> GetListByThesisIdAsync(CancellationToken cancellationToken, long thesisId)
        {
            List<AdvisorThesis> advisorTheses = await advisorThesisRepository.GetIncludingList(cancellationToken, x => x.ThesisId == thesisId && x.Thesis.IsDeleted == false && x.Educator.IsDeleted == false, x => x.Thesis, x => x.Educator);

            List<AdvisorThesisResponseDTO> response = mapper.Map<List<AdvisorThesisResponseDTO>>(advisorTheses);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            var advisorThesis = await advisorThesisRepository.GetById(cancellationToken, id);

            AdvisorThesisResponseDTO response = mapper.Map<AdvisorThesisResponseDTO>(advisorThesis);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> PostAsync(CancellationToken cancellationToken, AdvisorThesisDTO advisorThesisDTO)
        {
            var existAdvisor = await advisorThesisRepository.GetAsync(cancellationToken, x => x.ThesisId == advisorThesisDTO.ThesisId && !x.IsDeleted && x.ExpertiseBranchId == advisorThesisDTO.ExpertiseBranchId);

            if (existAdvisor?.Count > 0)
                return new() { Result = false, Message = "Seçtiğiniz uzmanlık dalına sahip tez danışmanı mevcuttur. Başka uzmanlık dalı seçiniz." };

            advisorThesisDTO.Type = AdvisorType.Educator;
            await koruRepository.AddStudentZoneToUserRole(cancellationToken, advisorThesisDTO.UserId ?? 0, advisorThesisDTO.StudentId ?? 0, RoleCodeConstants.TEZ_DANISMANI);

            AdvisorThesis advisorThesis = mapper.Map<AdvisorThesis>(advisorThesisDTO);
            var addedAdvisor = await advisorThesisRepository.AddAsync(cancellationToken, advisorThesis);
            await unitOfWork.CommitAsync(cancellationToken);

            var respons = await advisorThesisRepository.GetIncluding(cancellationToken, x=>x.Id == addedAdvisor.Id, x=>x.Educator.User);

            AdvisorThesisResponseDTO response = mapper.Map<AdvisorThesisResponseDTO>(advisorThesis);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> AddNotEducatorAdvisor(CancellationToken cancellationToken, AdvisorThesisDTO advisorThesisDTO)
        {
            advisorThesisDTO.Type = AdvisorType.NotEducator;
            if (advisorThesisDTO.User.Id == 0)
            {
                var addedUser = await userRepository.AddAsync(cancellationToken, mapper.Map<User>(advisorThesisDTO.User));
                await unitOfWork.CommitAsync(cancellationToken);
                advisorThesisDTO.UserId = addedUser.Id;
            }
            else
            {
                var user = mapper.Map<User>(advisorThesisDTO.User);
                user.IsDeleted = false;
                userRepository.Update(user);
                await unitOfWork.CommitAsync(cancellationToken);
            }
            advisorThesisDTO.User = null;

            await koruRepository.AddStudentZoneToUserRole(cancellationToken, advisorThesisDTO.UserId ?? 0, advisorThesisDTO.StudentId ?? 0, RoleCodeConstants.TEZ_DANISMANI);

            AdvisorThesis advisorThesis = mapper.Map<AdvisorThesis>(advisorThesisDTO);
            await advisorThesisRepository.AddAsync(cancellationToken, advisorThesis);
            await unitOfWork.CommitAsync(cancellationToken);

            AdvisorThesisResponseDTO response = mapper.Map<AdvisorThesisResponseDTO>(advisorThesis);

            return new() { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> Delete(CancellationToken cancellationToken, long id, ChangeCoordinatorAdvisorThesisDTO? advisorThesisToDelete = null)
        {
            var advisorThesis = await advisorThesisRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.Educator, x => x.Thesis);
            advisorThesis.IsDeleted = true;
            advisorThesis.DeleteDate = DateTime.UtcNow;
            if (advisorThesisToDelete != null)
            {
                advisorThesis.DeleteReason = advisorThesisToDelete.DeleteReason;
                advisorThesis.DeleteExplanation = advisorThesisToDelete.DeleteExplanation;
            }
            advisorThesisRepository.Update(advisorThesis);

            var anyOtherAdvisor = await advisorThesisRepository.FirstOrDefaultAsync(cancellationToken, x => advisorThesis.Type == AdvisorType.NotEducator ? x.UserId == advisorThesis.UserId : x.EducatorId == advisorThesis.EducatorId && !x.IsDeleted && x.Id != id);

            var advisorRole = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.TEZ_DANISMANI);
            if (anyOtherAdvisor == null)
                await koruRepository.RemoveRoleFromUser(cancellationToken, advisorThesis.Educator.UserId ?? advisorThesis.UserId ?? 0, advisorRole.Id);
            else
                await koruRepository.RemoveUserRoleStudent(advisorThesis.Educator.UserId ?? advisorThesis.UserId ?? 0, advisorRole.Id, advisorThesis?.Thesis?.StudentId ?? 0);

            await unitOfWork.CommitAsync(cancellationToken);
            return new() { Result = true };
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> Put(CancellationToken cancellationToken, long id, AdvisorThesisDTO advisorThesisDTO)
        {
            var existAdvisor = await advisorThesisRepository.GetAsync(cancellationToken, x => x.Id != advisorThesisDTO.Id && x.ThesisId == advisorThesisDTO.ThesisId && !x.IsDeleted && x.ExpertiseBranchId == advisorThesisDTO.ExpertiseBranchId);

            if (existAdvisor.Count > 0)
                return new() { Result = false, Message = "Seçtiğiniz uzmanlık dalına sahip tez danışmanı mevcuttur. Başka uzmanlık dalı seçiniz." };

            if (advisorThesisDTO.MakeCoordinator == true)
            {
                var existCoordinator = await advisorThesisRepository.AnyAsync(cancellationToken, x => x.ThesisId == advisorThesisDTO.ThesisId && x.IsCoordinator == true && x.IsDeleted == false);
                if (existCoordinator == true)
                    return new() { Result = false, Message = "Bu danışmanı koordinatör yapamazsınız. Tezde koordinatör danışman mevcuttur." };
                var thesis = await thesisRepository.GetByIdAsync(cancellationToken, advisorThesisDTO.ThesisId ?? 0);
                var student = await studentRepository.GetIncluding(cancellationToken, x => x.Id == thesis.StudentId, x=>x.OriginalProgram);
                if (student.OriginalProgram.ExpertiseBranchId != advisorThesisDTO.ExpertiseBranchId)
                    return new() { Result = false, Message = "Öğrencinin eğitim aldığı uzmanlık dalı ile koordinatör danışmanın seçilen uzmanlık dalı aynı olmadılır." };
                advisorThesisDTO.IsCoordinator = true;
            }

            var advisorThesis = await advisorThesisRepository.GetByIdAsync(cancellationToken, id);

            var updatedAdvisorThesis = mapper.Map(advisorThesisDTO, advisorThesis);

            advisorThesisRepository.Update(updatedAdvisorThesis);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<AdvisorThesisResponseDTO>(await advisorThesisRepository.GetById(cancellationToken, id));

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> ChangeCoordinatorAdvisor(CancellationToken cancellationToken, long id, ChangeCoordinatorAdvisorThesisDTO advisorThesisDTO)
        {
            var hasSameBranch = await advisorThesisRepository.AnyAsync(cancellationToken, x => !x.IsDeleted && x.ExpertiseBranchId == advisorThesisDTO.ExpertiseBranchId && x.ThesisId == advisorThesisDTO.ThesisId && x.Id != advisorThesisDTO.OldAdvisorId);

            if (hasSameBranch)
                return new() { Result = false, Message = "Seçtiğiniz uzmanlık dalına sahip tez danışmanı mevcuttur. Başka uzmanlık dalı seçiniz." };

            await Delete(cancellationToken, advisorThesisDTO.OldAdvisorId, advisorThesisDTO);
            advisorThesisDTO.DeleteExplanation = null;
            advisorThesisDTO.DeleteReason = null;

            AdvisorThesis advisorThesis = mapper.Map<AdvisorThesis>(advisorThesisDTO);
            var addedAdvisorThesis = await advisorThesisRepository.AddAsync(cancellationToken, advisorThesis);

            await unitOfWork.CommitAsync(cancellationToken);

            var response = await GetByIdAsync(cancellationToken, addedAdvisorThesis.Id);

            return new() { Result = true, Item = response.Item };
        }
    }
}
