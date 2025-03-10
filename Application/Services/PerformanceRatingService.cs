using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using MailKit.Net.Imap;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels.Base;
using Shared.FilterModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.Types;

namespace Application.Services
{
    public class PerformanceRatingService : BaseService, IPerformanceRatingService
    {
        private readonly IMapper mapper;
        private readonly IPerformanceRatingRepository performanceRatingRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PerformanceRatingService(IPerformanceRatingRepository performanceRatingRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IDocumentRepository documentRepository) : base(unitOfWork)
        {
            this.performanceRatingRepository = performanceRatingRepository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.documentRepository = documentRepository;
        }

        public async Task<PaginationModel<PerformanceRatingResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<PerformanceRating> ordersQuery = performanceRatingRepository.IncludingQueryable(x => true, x => x.Student.User, x => x.Educator.User);

            FilterResponse<PerformanceRating> filterResponse = ordersQuery.ToFilterView(filter);

            var aaa = await filterResponse.Query.ToListAsync(cancellationToken);

            var performanceRatings = mapper.Map<List<PerformanceRatingResponseDTO>>(aaa);

            var response = new PaginationModel<PerformanceRatingResponseDTO>
            {
                Items = performanceRatings,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
        public async Task<ResponseWrapper<PerformanceRatingResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            PerformanceRating performancerating = await performanceRatingRepository.GetByRatingId(cancellationToken, id);

            var response = mapper.Map<PerformanceRatingResponseDTO>(performancerating);

            return new ResponseWrapper<PerformanceRatingResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<List<PerformanceRatingResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            List<PerformanceRating> performanceRatings = await performanceRatingRepository.GetListByStudentId(cancellationToken, studentId);

            var response = mapper.Map<List<PerformanceRatingResponseDTO>>(performanceRatings);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PerformanceRatingResponseDTO>> GetByStudentId(CancellationToken cancellationToken, long studentId)
        {
            var performanceRating = await performanceRatingRepository.GetByAsync(cancellationToken, x => x.StudentId == studentId && x.IsDeleted == false);

            var response = mapper.Map<PerformanceRatingResponseDTO>(performanceRating);
            if (response != null)
            {
                response.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, performanceRating.Id, DocumentTypes.PerformanceRating));
            }

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PerformanceRatingResponseDTO>> PostAsync(CancellationToken cancellationToken, PerformanceRatingDTO performanceRatingDTO)
        {
            var educatorPr = performanceRatingRepository.GetEducatorByStudentId(cancellationToken, performanceRatingDTO.StudentId, httpContextAccessor.HttpContext.GetUserId());
            //if (educatorPr.Result != null)
            //{

            PerformanceRating performanceRating = mapper.Map<PerformanceRating>(performanceRatingDTO);
            performanceRating.EducatorId = educatorPr?.Result?.EducatorId;

            await performanceRatingRepository.AddAsync(cancellationToken, performanceRating);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<PerformanceRatingResponseDTO>(performanceRating);

            return new() { Result = true, Item = response };
            //}
            //else
            //    return new() { Result = false, Message = "Öğrencinin programında kayıtlı eğitici olmadığınız için bu öğrenciyi değerlendiremezsiniz!" };            
        }

        public async Task<ResponseWrapper<PerformanceRatingResponseDTO>> Put(CancellationToken cancellationToken, long id, PerformanceRatingDTO performanceRatingDTO)
        {
            PerformanceRating performanceRating = await performanceRatingRepository.GetByIdAsync(cancellationToken, id);

            PerformanceRating updatedPerformanceRating = mapper.Map(performanceRatingDTO, performanceRating);

            performanceRatingRepository.Update(updatedPerformanceRating);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<PerformanceRatingResponseDTO>(updatedPerformanceRating);

            return new ResponseWrapper<PerformanceRatingResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PerformanceRatingResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            PerformanceRating performanceRating = await performanceRatingRepository.GetByIdAsync(cancellationToken, id);

            performanceRatingRepository.SoftDelete(performanceRating);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<PerformanceRatingResponseDTO> { Result = true };
        }
    }
}
