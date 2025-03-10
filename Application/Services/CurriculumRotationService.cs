using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Shared.FilterModels.Base;
using Shared.FilterModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CurriculumRotationService : BaseService, ICurriculumRotationService
    {
        private readonly IMapper mapper;
        private readonly ICurriculumRotationRepository curriculumRotationRepository;

        public CurriculumRotationService(IMapper mapper, IUnitOfWork unitOfWork, ICurriculumRotationRepository curriculumRotationRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.curriculumRotationRepository = curriculumRotationRepository;
        }

        public async Task<ResponseWrapper<CurriculumRotationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            CurriculumRotation curriculumRotation = await curriculumRotationRepository.GetIncluding(cancellationToken, x => x.Id == id && x.IsDeleted == false, x => x.Curriculum, x => x.Rotation);
            CurriculumRotationResponseDTO response = mapper.Map<CurriculumRotationResponseDTO>(curriculumRotation);

            return new ResponseWrapper<CurriculumRotationResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<bool>> UnDeleteCurriculumRotation(CancellationToken cancellationToken, long id)
        {
            var curriculumRotation = await curriculumRotationRepository.GetByIdAsync(cancellationToken, id);

            if (curriculumRotation != null && curriculumRotation.IsDeleted == true)
            {
                curriculumRotation.IsDeleted = false;
                curriculumRotation.DeleteDate = null;

                curriculumRotationRepository.Update(curriculumRotation);
                await unitOfWork.CommitAsync(cancellationToken);
                return new ResponseWrapper<bool>() { Result = true };
            }
            else
            {
                return new ResponseWrapper<bool>() { Result = false, Message = "Kayıt bulunamadı!" };
            }
        }
        public async Task<PaginationModel<CurriculumRotationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<CurriculumRotation> ordersQuery = curriculumRotationRepository.GetWithSubRecords();
            FilterResponse<CurriculumRotation> filterResponse = ordersQuery.ToFilterView(filter);

            var curriculumRotations = mapper.Map<List<CurriculumRotationResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<CurriculumRotationResponseDTO>
            {
                Items = curriculumRotations,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<CurriculumRotationResponseDTO>> PostAsync(CancellationToken cancellationToken, CurriculumRotationDTO curriculumRotationDTO)
        {
            CurriculumRotation curriculumRotation = mapper.Map<CurriculumRotation>(curriculumRotationDTO);

            await curriculumRotationRepository.AddAsync(cancellationToken, curriculumRotation);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<CurriculumRotationResponseDTO>(curriculumRotation);

            return new ResponseWrapper<CurriculumRotationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CurriculumRotationResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            CurriculumRotation curriculumRotation = await curriculumRotationRepository.GetByAsync(cancellationToken, x => x.Id == id && x.IsDeleted == false);

            curriculumRotationRepository.SoftDelete(curriculumRotation);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<CurriculumRotationResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<CurriculumRotationResponseDTO>> UnDelete(CancellationToken cancellationToken, long id)
        {
            var curriculumRotation = await curriculumRotationRepository.GetByIdAsync(cancellationToken, id);

            curriculumRotation.IsDeleted = false;
            curriculumRotation.DeleteDate = null;

            curriculumRotationRepository.Update(curriculumRotation);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<CurriculumRotationResponseDTO>() { Result = true };
        }
    }
}
