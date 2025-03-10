using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.UnitOfWork;
using Infrastructure.Data;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Hospital;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class InstitutionService : BaseService, IInstitutionService
    {

        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;

        public InstitutionService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
        }
        public async Task<ResponseWrapper<List<InstitutionResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Institution> universities = await unitOfWork.InstitutionRepository.ListAsync(cancellationToken);

            List<InstitutionResponseDTO> response = mapper.Map<List<InstitutionResponseDTO>>(universities);

            return new ResponseWrapper<List<InstitutionResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<InstitutionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Institution institution = await unitOfWork.InstitutionRepository.GetByIdAsync(cancellationToken, id);

            InstitutionResponseDTO response = mapper.Map<InstitutionResponseDTO>(institution);

            return new ResponseWrapper<InstitutionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<InstitutionResponseDTO>> PostAsync(CancellationToken cancellationToken, InstitutionDTO institutionDTO)
        {
            Institution institution = mapper.Map<Institution>(institutionDTO);

            await unitOfWork.InstitutionRepository.AddAsync(cancellationToken, institution);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<InstitutionResponseDTO>(institution);

            return new ResponseWrapper<InstitutionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<InstitutionResponseDTO>> Put(CancellationToken cancellationToken, long id, InstitutionDTO institutionDTO)
        {
            Institution institution = await unitOfWork.InstitutionRepository.GetByIdAsync(cancellationToken, id);

            Institution updatedInstitution = mapper.Map(institutionDTO, institution);

            unitOfWork.InstitutionRepository.Update(updatedInstitution);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<InstitutionResponseDTO>(updatedInstitution);

            return new ResponseWrapper<InstitutionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<InstitutionResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Institution institution = await unitOfWork.InstitutionRepository.GetByIdAsync(cancellationToken, id);

            unitOfWork.InstitutionRepository.SoftDelete(institution);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<InstitutionResponseDTO> { Result = true };
        }
        public async Task<PaginationModel<InstitutionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Institution> ordersQuery = unitOfWork.InstitutionRepository.IncludingQueryable(x => true);
            FilterResponse<Institution> filterResponse = ordersQuery.ToFilterView(filter);

            var institutions = mapper.Map<List<InstitutionResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<InstitutionResponseDTO>
            {
                Items = institutions,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
        public async Task<ResponseWrapper<List<CountsByParentInstitutionModel>>> CountsByParentInstitution(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryableInstitutions = unitOfWork.InstitutionRepository.IncludingQueryable(x => true);

            //FilterResponse<Institution> filterResponse = queryableInstitutions.ToFilterView(filter);

            var response = await queryableInstitutions.GroupBy(x => x)
                                                     .Select(x => new CountsByParentInstitutionModel()
                                                     {
                                                         ParentInstitutionName = x.Key.Name,
                                                         StudentCount = unitOfWork.StudentRepository.StudentCountByInstitution(zone, x.Key.Id),
                                                         EducatorCount = unitOfWork.EducatorRepository.EducatorCountByInstitution(zone, x.Key.Id),
                                                         HospitalCount = unitOfWork.HospitalRepository.HospitalCountByInstitution(zone, x.Key.Id),
                                                         ProgramCount = unitOfWork.ProgramRepository.ProgramCountByInstitution(zone, x.Key.Id),
                                                         UniversityCount = unitOfWork.UniversityRepository.UniversityCountByInstitution(zone, x.Key.Id)
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<CountsByParentInstitutionModel>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<CountsByParentInstitutionModel>>> UniversityHospitalCountsByParentInstitution(CancellationToken cancellationToken)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryableInstitutions = unitOfWork.InstitutionRepository.IncludingQueryable(x => true);

            //FilterResponse<Institution> filterResponse = queryableInstitutions.ToFilterView(filter);

            var response = await queryableInstitutions.GroupBy(x => x)
                                                     .Select(x => new CountsByParentInstitutionModel()
                                                     {
                                                         ParentInstitutionName = x.Key.Name??"Bilinmiyor",
                                                         HospitalCount = unitOfWork.HospitalRepository.HospitalCountByInstitution(zone, x.Key.Id),
                                                         UniversityCount = unitOfWork.UniversityRepository.UniversityCountByInstitution(zone, x.Key.Id)
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<CountsByParentInstitutionModel>> { Item = response, Result = true };
        }
    }
}

