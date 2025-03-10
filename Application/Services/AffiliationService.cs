using Application.Interfaces;
using Application.Reports.ExcelReports.AffiliationReports;
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
    public class AffiliationService : BaseService, IAffiliationService
    {
        private readonly IMapper mapper;
        private readonly IDocumentRepository documentRepository;
        private readonly IKoruRepository koruRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAffiliationRepository affiliationRepository;

        public AffiliationService(IMapper mapper, IUnitOfWork unitOfWork, IAffiliationRepository affiliationRepository, IDocumentRepository documentRepository, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.affiliationRepository = affiliationRepository;
            this.documentRepository = documentRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
        }
        public async Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<Affiliation> query = affiliationRepository.IncludingQueryable(x => x.IsDeleted == false, x => x.Hospital, x => x.Faculty.University);

            List<Affiliation> affiliations = await query.OrderBy(x => x.Hospital.Name).ToListAsync(cancellationToken);

            List<AffiliationResponseDTO> response = mapper.Map<List<AffiliationResponseDTO>>(affiliations);

            return new ResponseWrapper<List<AffiliationResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<AffiliationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);
            IQueryable<Affiliation> ordersQuery = affiliationRepository.PaginateQuery(zone);
            FilterResponse<Affiliation> filterResponse = ordersQuery.ToFilterView(filter);

            var affiliations = mapper.Map<List<AffiliationResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<AffiliationResponseDTO>
            {
                Items = affiliations,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId)
        {
            List<Affiliation> affiliations = await affiliationRepository.GetListByUniversityId(cancellationToken, uniId);

            List<AffiliationResponseDTO> response = mapper.Map<List<AffiliationResponseDTO>>(affiliations);

            return new ResponseWrapper<List<AffiliationResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId)
        {
            List<Affiliation> affiliations = await affiliationRepository.GetListByHospitalId(cancellationToken, hospitalId);

            List<AffiliationResponseDTO> response = mapper.Map<List<AffiliationResponseDTO>>(affiliations);

            return new ResponseWrapper<List<AffiliationResponseDTO>> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<AffiliationResponseDTO>> GetByAffiliation(CancellationToken cancellationToken, long facultyid, long hospitalid)
        {
            Affiliation affiliation = await affiliationRepository.GetWithFacultyHospitalId(cancellationToken, facultyid, hospitalid);

            AffiliationResponseDTO response = mapper.Map<AffiliationResponseDTO>(affiliation);
            return new ResponseWrapper<AffiliationResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<AffiliationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id, DocumentTypes docType)
        {
            Affiliation affiliation = await affiliationRepository.GetWithSubRecords(cancellationToken, id);

            AffiliationResponseDTO response = mapper.Map<AffiliationResponseDTO>(affiliation);

            var docs = await documentRepository.GetByEntityId(cancellationToken, id, docType);

            var docsResponse = mapper.Map<List<DocumentResponseDTO>>(docs);
            if (affiliation != null)
                response.Documents = docsResponse;

            return new ResponseWrapper<AffiliationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AffiliationResponseDTO>> PostAsync(CancellationToken cancellationToken, AffiliationDTO AffiliationDTO)
        {
            Affiliation affiliation = mapper.Map<Affiliation>(AffiliationDTO);

            await unitOfWork.AffiliationRepository.AddAsync(cancellationToken, affiliation);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<AffiliationResponseDTO>(affiliation);

            return new ResponseWrapper<AffiliationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AffiliationResponseDTO>> Put(CancellationToken cancellationToken, long id, AffiliationDTO AffiliationDTO)
        {
            Affiliation affiliation = await affiliationRepository.GetByIdAsync(cancellationToken, id);

            Affiliation updatedAffiliation = mapper.Map(AffiliationDTO, affiliation);

            unitOfWork.AffiliationRepository.Update(updatedAffiliation);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<AffiliationResponseDTO>(updatedAffiliation);

            return new ResponseWrapper<AffiliationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AffiliationResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Affiliation Affiliation = await affiliationRepository.GetByIdAsync(cancellationToken, id);

            unitOfWork.AffiliationRepository.SoftDelete(Affiliation);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<AffiliationResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter)
        {
            filter.pageSize = int.MaxValue;
            filter.page = 1;
            //filter.Filter = null;

            //var response = await GetPaginateList(cancellationToken, filter);
            List<AffiliationExcelExport> affiliations = await affiliationRepository.ExcelExport(cancellationToken);

            var byteArray = ExportList.ExportListReport(affiliations);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }
    }
}
