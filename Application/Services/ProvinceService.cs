using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using Microsoft.EntityFrameworkCore;
using Core.Extentsions;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.ResponseModels.StatisticModels;

namespace Application.Services
{
    public class ProvinceService : BaseService, IProvinceService
    {
        private readonly IMapper mapper;
        private readonly IProvinceRepository provinceRepository;
        public ProvinceService(IMapper mapper, IUnitOfWork unitOfWork, IProvinceRepository provinceRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.provinceRepository = provinceRepository;
        }
        public async Task<PaginationModel<ProvinceResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Province> ordersQuery = unitOfWork.ProvinceRepository.IncludingQueryable(x => true);
            FilterResponse<Province> filterResponse = ordersQuery.ToFilterView(filter);

            var provinces = mapper.Map<List<ProvinceResponseDTO>>(await filterResponse.Query.ToListAsync());

            var response = new PaginationModel<ProvinceResponseDTO>
            {
                Items = provinces,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<List<ProvinceResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<Province> query = unitOfWork.ProvinceRepository.Queryable();

            List<Province> provinces = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<ProvinceResponseDTO> response = mapper.Map<List<ProvinceResponseDTO>>(provinces);

            return new ResponseWrapper<List<ProvinceResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<CityDetailsForMapModel>>> ProvinceDetailsForMap(CancellationToken cancellationToken)
        {
            var response = await unitOfWork.ProvinceRepository.DetailsForMap(cancellationToken);

            return new ResponseWrapper<List<CityDetailsForMapModel>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<ProvinceResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Province province = await provinceRepository.GetByIdAsync(cancellationToken, id);

            ProvinceResponseDTO response = mapper.Map<ProvinceResponseDTO>(province);

            return new ResponseWrapper<ProvinceResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProvinceResponseDTO>> PostAsync(CancellationToken cancellationToken, ProvinceDTO provinceDTO)
        {
            Province province = mapper.Map<Province>(provinceDTO);

            await provinceRepository.AddAsync(cancellationToken, province);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<ProvinceResponseDTO>(province);

            return new ResponseWrapper<ProvinceResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProvinceResponseDTO>> Put(CancellationToken cancellationToken, long id, ProvinceDTO provinceDTO)
        {
            Province province = await provinceRepository.GetByIdAsync(cancellationToken, id);

            Province updatedProvince = mapper.Map(provinceDTO, province);

            provinceRepository.Update(updatedProvince);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<ProvinceResponseDTO>(updatedProvince);

            return new ResponseWrapper<ProvinceResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProvinceResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Province province = await provinceRepository.GetByIdAsync(cancellationToken, id);

            provinceRepository.SoftDelete(province);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<ProvinceResponseDTO> { Result = true };
        }
    }
}

