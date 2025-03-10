using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Entities.Koru;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Koru.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
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
	public class EducationOfficerService : BaseService, IEducationOfficerService
	{
		private readonly IEducationOfficerRepository educationOfficerRepository;
		private readonly IEducatorRepository educatorRepository;
		private readonly IKoruRepository koruRepository;
		private readonly IDocumentRepository documentRepository;
		private readonly IMapper mapper;
		public EducationOfficerService(IUnitOfWork unitOfWork, IEducationOfficerRepository educationOfficerRepository, IKoruRepository koruRepository, IMapper mapper, IEducatorRepository educatorRepository, IDocumentRepository documentRepository) : base(unitOfWork)
		{
			this.educationOfficerRepository = educationOfficerRepository;
			this.koruRepository = koruRepository;
			this.mapper = mapper;
			this.educatorRepository = educatorRepository;
			this.documentRepository = documentRepository;
		}

		public async Task<ResponseWrapper<EducationOfficerResponseDTO>> ChangeProgramManager(CancellationToken cancellationToken, EducationOfficerDTO educationOfficerDTO)
		{
			unitOfWork.BeginTransaction();
			var role = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);

			var oldEducationOfficer = await educationOfficerRepository.GetIncluding(cancellationToken, x => x.ProgramId == educationOfficerDTO.ProgramId && x.EndDate == null && x.Educator.IsDeleted == false && x.Educator.User.IsDeleted == false, x => x.Educator);

			if (oldEducationOfficer != null)
			{
				oldEducationOfficer.EndDate = DateTime.UtcNow;
				await koruRepository.RemoveRoleFromUser(cancellationToken, (long)oldEducationOfficer.Educator.UserId, role.Id);
			}

			var educator = await educatorRepository.GetByIdAsync(cancellationToken, (long)educationOfficerDTO.EducatorId);
			koruRepository.AddRoleToUser(role, (long)educator.UserId);

			var educationOfficer = mapper.Map<EducationOfficer>(educationOfficerDTO);
			await educationOfficerRepository.AddAsync(cancellationToken, educationOfficer);
			await unitOfWork.CommitAsync(cancellationToken);

			var document = await documentRepository.GetByAsync(cancellationToken, x => x.BucketKey == educationOfficerDTO.DocumentOrder);
			document.EntityId = educationOfficer.Id;
			documentRepository.Update(document);

			await unitOfWork.EndTransactionAsync(cancellationToken);


			return new ResponseWrapper<EducationOfficerResponseDTO> { Result = true, Item = mapper.Map<EducationOfficerResponseDTO>(educationOfficer) };
		}

		public async Task<PaginationModel<EducationOfficerResponseDTO>> GetPaginateListForProgramDetail(CancellationToken cancellationToken, FilterDTO filter)
		{
			var programId = (long)filter.Filter.Filters.FirstOrDefault(x => x.Field == "ProgramId").Value;
			IQueryable<EducationOfficer> ordersQuery = educationOfficerRepository.GetPaginateListForProgramDetailQuery(programId);
			FilterResponse<EducationOfficer> filterResponse = ordersQuery.ToFilterView(filter);

			var educationOfficers = mapper.Map<List<EducationOfficerResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

			return new PaginationModel<EducationOfficerResponseDTO>
			{
				Items = educationOfficers,
				TotalPages = filterResponse.PageNumber,
				Page = filter.page,
				PageSize = filter.pageSize,
				TotalItemCount = filterResponse.Count
			};
		}

		public async Task<ResponseWrapper<EducationOfficerResponseDTO>> PostAsync(CancellationToken cancellationToken, EducationOfficerDTO educationOfficerDTO)
		{
			unitOfWork.BeginTransaction();
			var role = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);

			var educationOfficer = mapper.Map<EducationOfficer>(educationOfficerDTO);

			await educationOfficerRepository.AddAsync(cancellationToken, educationOfficer);

			koruRepository.AddRoleToUser(role, (long)educationOfficer.Educator.UserId);

			var response = mapper.Map<EducationOfficerResponseDTO>(educationOfficer);

			await unitOfWork.EndTransactionAsync(cancellationToken);

			return new ResponseWrapper<EducationOfficerResponseDTO> { Result = true, Item = response };
		}

		//public async Task<ResponseWrapper<EducationOfficerResponseDTO>> FinishDuty(CancellationToken cancellationToken, long programId, long educatorId)
		//{
		//	unitOfWork.BeginTransaction();

		//	var role = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);

		//	await educationOfficerRepository.FinishEducationOfficersDuty(cancellationToken, programId, educatorId);
		//	var educationOfficer = await educationOfficerRepository.GetIncluding(cancellationToken, x => x.ProgramId == programId && x.EducatorId == educatorId && x.EndDate == null, x => x.Educator);

		//	await koruRepository.RemoveRoleFromUser(cancellationToken, (long)educationOfficer.Educator.UserId, role.Id);

		//	var response = mapper.Map<EducationOfficerResponseDTO>(educationOfficer);

		//	await unitOfWork.EndTransactionAsync(cancellationToken);

		//	return new ResponseWrapper<EducationOfficerResponseDTO> { Result = true, Item = response };

		//}

		public async Task<ResponseWrapper<List<EducationOfficerResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId)
		{
			var educationOfficers = await educationOfficerRepository.GetIncludingList(cancellationToken, x => x.ProgramId == programId && x.EndDate == null, x => x.Educator.User);
			return new() { Result = true, Item = mapper.Map<List<EducationOfficerResponseDTO>>(educationOfficers) };
		}
	}
}
