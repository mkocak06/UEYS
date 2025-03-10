using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.FilterModels.Base;
using Shared.FilterModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class FacultyService : BaseService, IFacultyService
    {
        private readonly IMapper mapper;
        private readonly IFacultyRepository facultyRepository;

        public FacultyService(IMapper mapper, IUnitOfWork unitOfWork, IFacultyRepository facultyRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.facultyRepository = facultyRepository;
        }

        public async Task<PaginationModel<FacultyResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Faculty> ordersQuery = unitOfWork.FacultyRepository.IncludingQueryable(x => true);
            FilterResponse<Faculty> filterResponse = ordersQuery.ToFilterView(filter);

            var authorizationCategories = mapper.Map<List<FacultyResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<FacultyResponseDTO>
            {
                Items = authorizationCategories,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<List<FacultyResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Faculty> faculties = await facultyRepository.ListAsync(cancellationToken);

            List<FacultyResponseDTO> response = mapper.Map<List<FacultyResponseDTO>>(faculties);

            return new ResponseWrapper<List<FacultyResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<FacultyResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId)
        {
            List<Faculty> faculties = await facultyRepository.GetListByUniversityId(cancellationToken, uniId);

            List<FacultyResponseDTO> response = mapper.Map<List<FacultyResponseDTO>>(faculties);

            return new ResponseWrapper<List<FacultyResponseDTO>> { Result = true, Item = response };
        }
    }
}
