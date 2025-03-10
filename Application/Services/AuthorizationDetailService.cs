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
    public class AuthorizationDetailService : BaseService, IAuthorizationDetailService
    {
        private readonly IMapper mapper;
        private readonly IAuthorizationDetailRepository authorizationDetailRepository;

        public AuthorizationDetailService(IMapper mapper, IUnitOfWork unitOfWork, IAuthorizationDetailRepository authorizationDetailRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.authorizationDetailRepository = authorizationDetailRepository;
        }
        public async Task<ResponseWrapper<List<AuthorizationDetailResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId)
        {
            List<AuthorizationDetail> authorizationDetails = await authorizationDetailRepository.GetIncludingList(cancellationToken, x => x.ProgramId == programId, x => x.AuthorizationCategory);

            List<AuthorizationDetailResponseDTO> response = mapper.Map<List<AuthorizationDetailResponseDTO>>(authorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate));

            return new ResponseWrapper<List<AuthorizationDetailResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AuthorizationDetailResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            AuthorizationDetail authorizationDetail = await authorizationDetailRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.AuthorizationCategory);

            AuthorizationDetailResponseDTO response = mapper.Map<AuthorizationDetailResponseDTO>(authorizationDetail);

            return new ResponseWrapper<AuthorizationDetailResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AuthorizationDetailResponseDTO>> PostAsync(CancellationToken cancellationToken, AuthorizationDetailDTO authorizationDetailDTO)
        {
            AuthorizationDetail authorizationDetail = mapper.Map<AuthorizationDetail>(authorizationDetailDTO);

            await authorizationDetailRepository.AddAsync(cancellationToken, authorizationDetail);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<AuthorizationDetailResponseDTO>(await authorizationDetailRepository.GetIncluding(cancellationToken, x => x.Id == authorizationDetail.Id, x => x.AuthorizationCategory));

            return new ResponseWrapper<AuthorizationDetailResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AuthorizationDetailResponseDTO>> Put(CancellationToken cancellationToken, long id, AuthorizationDetailDTO authorizationDetailDTO)
        {
            AuthorizationDetail authorizationDetail = await authorizationDetailRepository.GetByIdAsync(cancellationToken, id);

            AuthorizationDetail updatedAuthorizationDetail = mapper.Map(authorizationDetailDTO, authorizationDetail);

            authorizationDetailRepository.Update(updatedAuthorizationDetail);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<AuthorizationDetailResponseDTO>(updatedAuthorizationDetail);

            return new ResponseWrapper<AuthorizationDetailResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AuthorizationDetailResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            AuthorizationDetail authorizationDetail = await authorizationDetailRepository.GetByIdAsync(cancellationToken, id);

            authorizationDetailRepository.SoftDelete(authorizationDetail);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<AuthorizationDetailResponseDTO> { Result = true };
        }
    }
}
