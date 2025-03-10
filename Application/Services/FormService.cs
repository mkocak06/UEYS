using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Koru;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FormService : BaseService, IFormService
    {
        private readonly IMapper mapper;
        private readonly IFormRepository formRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;

        public FormService(IMapper mapper, IUnitOfWork unitOfWork, IFormRepository formRepository, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.formRepository = formRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
        }

        public async Task<ResponseWrapper<FormResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Form form = await formRepository.GetByIdAsync(cancellationToken, id);

            formRepository.SoftDelete(form);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<FormResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            FormResponseDTO response = await formRepository.GetWithSubRecords(cancellationToken, id);

            var committeeDocs = await unitOfWork.DocumentRepository.GetByEntityId(cancellationToken, id, Shared.Types.DocumentTypes.CommitteeForm);
            var committeeDocsResponse = mapper.Map<List<DocumentResponseDTO>>(committeeDocs);

            var tukDocs = await unitOfWork.DocumentRepository.GetByEntityId(cancellationToken, id, Shared.Types.DocumentTypes.TUKForm);
            var tukDocssResponse = mapper.Map<List<DocumentResponseDTO>>(tukDocs);

            response.CommitteeDocuments = committeeDocsResponse;
            response.TUKDocuments = tukDocssResponse;

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<FormResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<Form> query = formRepository.IncludingQueryable(x => x.IsDeleted == false);

            List<Form> Form = await query.ToListAsync(cancellationToken);

            List<FormResponseDTO> response = mapper.Map<List<FormResponseDTO>>(Form);

            return new ResponseWrapper<List<FormResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<FormResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<Form> ordersQuery = formRepository.GetByZoneQueryable(zone);
            FilterResponse<Form> filterResponse = ordersQuery.ToFilterView(filter);

            var form = mapper.Map<List<FormResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<FormResponseDTO>
            {
                Items = form,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<FormResponseDTO>> PostAsync(CancellationToken cancellationToken, FormDTO FormDTO)
        {
            Form Form = mapper.Map<Form>(FormDTO);

            await formRepository.AddAsync(cancellationToken, Form);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<FormResponseDTO>(Form);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<FormResponseDTO>> Put(CancellationToken cancellationToken, long id, FormDTO formDTO)
        {
            var existform = await formRepository.Queryable()
                .Include(x => x.OnSiteVisitCommittees).ThenInclude(x => x.User)
                .Include(x => x.FormStandards).ThenInclude(x => x.Standard)
                .Include(x => x.AuthorizationDetail).ThenInclude(x => x.AuthorizationCategory)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);


            var updatedForm = mapper.Map(formDTO, existform);

            formRepository.Update(updatedForm);
            await unitOfWork.CommitAsync(cancellationToken);

            var formUpdated = await formRepository.Queryable()
                .Include(x => x.OnSiteVisitCommittees).ThenInclude(x => x.User)
                .Include(x => x.FormStandards).ThenInclude(x => x.Standard)
                .Include(x => x.AuthorizationDetail).ThenInclude(x => x.AuthorizationCategory)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            var response = mapper.Map<FormResponseDTO>(formUpdated);

            return new() { Result = true, Item = response };
        }
    }
}
