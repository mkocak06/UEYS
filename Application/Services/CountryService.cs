using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.UnitOfWork;
using Infrastructure.Data;
using Shared.FilterModels.Base;
using Shared.FilterModels;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CountryService : BaseService, ICountryService
    {
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepository;

        public CountryService(IUnitOfWork unitOfWork, IMapper mapper, ICountryRepository countryRepository) : base(unitOfWork)
        {
            _mapper = mapper;
            _countryRepository = countryRepository;
        }
        public async Task<PaginationModel<CountryResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Country> ordersQuery = _countryRepository.Get(x => !x.IsDeleted);
            FilterResponse<Country> filterResponse = ordersQuery.ToFilterView(filter);

            var countrys = _mapper.Map<List<CountryResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<CountryResponseDTO>
            {
                Items = countrys,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
        public async Task<ResponseWrapper<List<CountryResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Country> countrys = await _countryRepository.GetAsync(cancellationToken, x => !x.IsDeleted);

            List<CountryResponseDTO> response = _mapper.Map<List<CountryResponseDTO>>(countrys);
            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CountryResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Country country = await _countryRepository.FirstOrDefaultAsync(cancellationToken, x => x.Id == id);

            CountryResponseDTO response = _mapper.Map<CountryResponseDTO>(country);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CountryResponseDTO>> PostAsync(CancellationToken cancellationToken, CountryDTO countryDTO)
        {
            Country country = _mapper.Map<Country>(countryDTO);

            await unitOfWork.CountryRepository.AddAsync(cancellationToken, country);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = _mapper.Map<CountryResponseDTO>(country);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CountryResponseDTO>> Put(CancellationToken cancellationToken, long id, CountryDTO countryDTO)
        {
            Country country = await _countryRepository.GetByIdAsync(cancellationToken, id);

            Country updatedCountry = _mapper.Map(countryDTO, country);

            unitOfWork.CountryRepository.Update(updatedCountry);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = _mapper.Map<CountryResponseDTO>(updatedCountry);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CountryResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Country country = await _countryRepository.GetByIdAsync(cancellationToken, id);

            unitOfWork.CountryRepository.SoftDelete(country);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}