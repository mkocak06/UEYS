using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.UnitOfWork;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Services
{
    public class ProfessionService : BaseService, IProfessionService
    {
        private readonly IMapper mapper;

        public ProfessionService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.mapper = mapper;
        }
        public async Task<ResponseWrapper<List<ProfessionResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Profession> professions = await unitOfWork.ProfessionRepository.ListAsync(cancellationToken);

            List<ProfessionResponseDTO> response = mapper.Map<List<ProfessionResponseDTO>>(professions);

            return new ResponseWrapper<List<ProfessionResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProfessionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Profession profession = await unitOfWork.ProfessionRepository.GetWithSubRecords(cancellationToken, id);

            ProfessionResponseDTO response = mapper.Map<ProfessionResponseDTO>(profession);

            return new ResponseWrapper<ProfessionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ProfessionResponseDTO>>> GetByUniversityIdAsync(CancellationToken cancellationToken, long uniId)
        {
            List<Profession> professions = await unitOfWork.ProfessionRepository.GetByUniversityId(cancellationToken, uniId);

            List<ProfessionResponseDTO> response = mapper.Map<List<ProfessionResponseDTO>>(professions);

            return new ResponseWrapper<List<ProfessionResponseDTO>> { Result = true, Item = response };
        }
        public async Task<PaginationModel<ProfessionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Profession> ordersQuery = unitOfWork.ProfessionRepository.IncludingQueryable(x => true);
            FilterResponse<Profession> filterResponse = ordersQuery.ToFilterView(filter);

            var professions = mapper.Map<List<ProfessionResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<ProfessionResponseDTO>
            {
                Items = professions,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<ProfessionResponseDTO>> PostAsync(CancellationToken cancellationToken, ProfessionDTO professionDTO)
        {
            Profession profession = mapper.Map<Profession>(professionDTO);

            await unitOfWork.ProfessionRepository.AddAsync(cancellationToken, profession);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<ProfessionResponseDTO>(profession);

            return new ResponseWrapper<ProfessionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProfessionResponseDTO>> Put(CancellationToken cancellationToken, long id, ProfessionDTO professionDTO)
        {
            Profession profession = await unitOfWork.ProfessionRepository.GetByIdAsync(cancellationToken, id);

            Profession updatedProfession = mapper.Map(professionDTO, profession);

            unitOfWork.ProfessionRepository.Update(updatedProfession);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<ProfessionResponseDTO>(updatedProfession);

            return new ResponseWrapper<ProfessionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProfessionResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Profession profession = await unitOfWork.ProfessionRepository.GetByIdAsync(cancellationToken, id);

            unitOfWork.ProfessionRepository.SoftDelete(profession);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<ProfessionResponseDTO> { Result = true };
        }
    }
}
