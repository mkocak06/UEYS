using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class EducatorExpertiseBranchService : BaseService, IEducatorExpertiseBranchService
    {

        private readonly IMapper mapper;
        private readonly IEducatorExpertiseBranchRepository educatorExpertiseBranchRepository;

        public EducatorExpertiseBranchService(IMapper mapper, IUnitOfWork unitOfWork, IEducatorExpertiseBranchRepository educatorExpertiseBranchRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.educatorExpertiseBranchRepository = educatorExpertiseBranchRepository;
        }

        public async Task<ResponseWrapper<List<EducatorExpertiseBranchResponseDTO>>> GetListByEducatorIdAsync(CancellationToken cancellationToken, long educatorId)
        {
            List<EducatorExpertiseBranch> educatorExpertiseBranch = await educatorExpertiseBranchRepository.GetIncludingList(cancellationToken, x => x.EducatorId == educatorId, x => x.ExpertiseBranch);
            List<EducatorExpertiseBranchResponseDTO> response = mapper.Map<List<EducatorExpertiseBranchResponseDTO>>(educatorExpertiseBranch);

            return new ResponseWrapper<List<EducatorExpertiseBranchResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducatorExpertiseBranchResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorExpertiseBranchDTO educatorExpertiseBranchDTO)
        {
            EducatorExpertiseBranch educatorExpertiseBranch = mapper.Map<EducatorExpertiseBranch>(educatorExpertiseBranchDTO);

            await educatorExpertiseBranchRepository.AddAsync(cancellationToken, educatorExpertiseBranch);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<EducatorExpertiseBranchResponseDTO>(educatorExpertiseBranch);

            return new ResponseWrapper<EducatorExpertiseBranchResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducatorExpertiseBranchResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            EducatorExpertiseBranch educatorExpertiseBranch = await educatorExpertiseBranchRepository.GetByAsync(cancellationToken, x => x.Id == id);

            educatorExpertiseBranchRepository.HardDelete(educatorExpertiseBranch);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<EducatorExpertiseBranchResponseDTO> { Result = true };
        }
    }
}
