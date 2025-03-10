using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Text;

namespace Application.Services
{
    public class ExpertiseBranchService : BaseService, IExpertiseBranchService
    {
        private readonly IMapper mapper;
        private readonly IExpertiseBranchRepository expertiseBranchRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IRelatedExpertiseBranchRepository relatedExpertiseBranchRepository;
        private readonly ICKYSService cKYSService;
        private readonly IEmailSender emailSender;

        public ExpertiseBranchService(IMapper mapper, IUnitOfWork unitOfWork, IExpertiseBranchRepository expertiseBranchRepository, IRelatedExpertiseBranchRepository relatedExpertiseBranchRepository, IStudentRepository studentRepository, IEmailSender emailSender, ICKYSService cKYSService) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.expertiseBranchRepository = expertiseBranchRepository;
            this.relatedExpertiseBranchRepository = relatedExpertiseBranchRepository;
            this.studentRepository = studentRepository;
            this.emailSender = emailSender;
            this.cKYSService = cKYSService;
        }
        public async Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<ExpertiseBranch> query = unitOfWork.ExpertiseBranchRepository.IncludingQueryable(x => true, x => x.Profession, x => x.PrincipalBranches, x => x.SubBranches);

            List<ExpertiseBranch> expertiseBranches = await query.OrderBy(x => x.Name).ToListAsync();

            List<ExpertiseBranchResponseDTO> response = mapper.Map<List<ExpertiseBranchResponseDTO>>(expertiseBranches);

            return new ResponseWrapper<List<ExpertiseBranchResponseDTO>> { Result = true, Item = response };
        }
        public async Task<PaginationModel<ExpertiseBranchResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<ExpertiseBranch> ordersQuery = unitOfWork.ExpertiseBranchRepository.Queryable();
            FilterResponse<ExpertiseBranch> filterResponse = ordersQuery.ToFilterView(filter);

            var branches = filterResponse.Query.Select(x => new ExpertiseBranchResponseDTO()
            {
                Name = x.Name,
                Id = x.Id,
                Details = x.Details,
                Profession = mapper.Map<ProfessionResponseDTO>(x.Profession),
                ProfessionId = x.ProfessionId,
                IsPrincipal = x.IsPrincipal,
                ProtocolProgramCount = x.ProtocolProgramCount,
                PrincipalBranches = x.PrincipalBranches.Select(a => new RelatedExpertiseBranchResponseDTO() { SubBranchId = x.Id, PrincipalBranchId = a.PrincipalBranchId, PrincipalBranch = new ExpertiseBranchResponseDTO { Id = a.PrincipalBranchId, Name = a.PrincipalBranch.Name } }).ToList(),
                SubBranches = x.SubBranches.Select(a => new RelatedExpertiseBranchResponseDTO() { PrincipalBranchId = x.Id, SubBranchId = a.SubBranchId, SubBranch = new ExpertiseBranchResponseDTO { Id = a.SubBranchId, Name = a.SubBranch.Name } }).ToList(),
                EducatorIndexRateToCapacityIndex = x.EducatorIndexRateToCapacityIndex,
                PortfolioIndexRateToCapacityIndex = x.PortfolioIndexRateToCapacityIndex,
            }).ToList();

            var response = new PaginationModel<ExpertiseBranchResponseDTO>
            {
                Items = branches,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<ExpertiseBranchResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            ExpertiseBranch expertiseBranch = await unitOfWork.ExpertiseBranchRepository.GetById(cancellationToken, id);

            var response = new ExpertiseBranchResponseDTO()
            {
                Name = expertiseBranch.Name,
                Id = expertiseBranch.Id,
                Details = expertiseBranch.Details,
                IsPrincipal = expertiseBranch.IsPrincipal,
                Profession = mapper.Map<ProfessionResponseDTO>(expertiseBranch.Profession),
                ProfessionId = expertiseBranch.ProfessionId,
                PrincipalBranches = expertiseBranch.PrincipalBranches.Select(x => new RelatedExpertiseBranchResponseDTO() { PrincipalBranch = new ExpertiseBranchResponseDTO { Id = x.PrincipalBranchId, Name = x.PrincipalBranch.Name }, PrincipalBranchId = x.PrincipalBranchId, SubBranchId = expertiseBranch.Id }).ToList(),
                SubBranches = expertiseBranch.SubBranches.Select(x => new RelatedExpertiseBranchResponseDTO() { SubBranch = new ExpertiseBranchResponseDTO { Id = x.SubBranchId, Name = x.SubBranch.Name }, PrincipalBranchId = expertiseBranch.Id, SubBranchId = x.SubBranchId }).ToList(),
                ProtocolProgramCount = expertiseBranch.ProtocolProgramCount,
                Code = expertiseBranch.Code,
                IsIntensiveCare = expertiseBranch.IsIntensiveCare,
                EducatorIndexRateToCapacityIndex = expertiseBranch.EducatorIndexRateToCapacityIndex,
                PortfolioIndexRateToCapacityIndex = expertiseBranch.PortfolioIndexRateToCapacityIndex,
                CKYSCode = expertiseBranch.CKYSCode,
                SKRSCode = expertiseBranch.SKRSCode,
            };

            return new ResponseWrapper<ExpertiseBranchResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListRelatedWithProgramsByProfessionIdAsync(CancellationToken cancellationToken, long id)
        {
            List<ExpertiseBranch> programs = await unitOfWork.ExpertiseBranchRepository.GetListRelatedWithProgramsByProfessionId(cancellationToken, id);

            List<ExpertiseBranchResponseDTO> response = mapper.Map<List<ExpertiseBranchResponseDTO>>(programs);

            return new ResponseWrapper<List<ExpertiseBranchResponseDTO>> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListByProfessionIdAsync(CancellationToken cancellationToken, long id)
        {
            IQueryable<ExpertiseBranch> query = unitOfWork.ExpertiseBranchRepository.IncludingQueryable(x => x.ProfessionId == id, x => x.Profession);
            List<ExpertiseBranch> expertiseBranches = await query.OrderBy(x => x.Name).ToListAsync();

            List<ExpertiseBranchResponseDTO> response = mapper.Map<List<ExpertiseBranchResponseDTO>>(expertiseBranches);

            return new ResponseWrapper<List<ExpertiseBranchResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListForProtocolProgramByHospitalId(CancellationToken cancellationToken, long hospitalId)
        {
            List<ExpertiseBranch> expertiseBranches = await expertiseBranchRepository.GetListForProtocolProgramByHospitalId(cancellationToken, hospitalId);

            List<ExpertiseBranchResponseDTO> response = mapper.Map<List<ExpertiseBranchResponseDTO>>(expertiseBranches);

            return new ResponseWrapper<List<ExpertiseBranchResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ExpertiseBranchResponseDTO>> PostAsync(CancellationToken cancellationToken, ExpertiseBranchDTO expertiseBranchDTO)
        {
            await unitOfWork.BeginTransactionAsync();
            ExpertiseBranch expertiseBranch = mapper.Map<ExpertiseBranch>(expertiseBranchDTO);
            expertiseBranch.SubBranches = null;
            await unitOfWork.ExpertiseBranchRepository.AddAsync(cancellationToken, expertiseBranch);
            await unitOfWork.CommitAsync(cancellationToken);
            if (expertiseBranchDTO.SubBranches != null && expertiseBranchDTO.SubBranches.Any())
            {
                foreach (var item in expertiseBranchDTO.SubBranches)
                {
                    var subBranch = await unitOfWork.ExpertiseBranchRepository.GetByIdAsync(cancellationToken, (long)item.SubBranchId);
                    if (subBranch != null)
                    {
                        //subBranch.PrincipalBranchId = expertiseBranch.Id; // TODO Expertise Branch
                        unitOfWork.ExpertiseBranchRepository.Update(subBranch);
                    }
                }
                await unitOfWork.CommitAsync(cancellationToken);
            }
            await unitOfWork.EndTransactionAsync(cancellationToken);
            ExpertiseBranchResponseDTO response = mapper.Map<ExpertiseBranchResponseDTO>(expertiseBranch);

            return new ResponseWrapper<ExpertiseBranchResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ExpertiseBranchResponseDTO>> Put(CancellationToken cancellationToken, long id, ExpertiseBranchDTO expertiseBranchDTO)
        {
            ExpertiseBranch expertiseBranch = await expertiseBranchRepository.GetByIdAsync(cancellationToken, id);
            var updatedExpertiseBranch = mapper.Map(expertiseBranchDTO, expertiseBranch);

            await expertiseBranchRepository.UpdateAsync(updatedExpertiseBranch);

            var response = mapper.Map<ExpertiseBranchResponseDTO>(expertiseBranch);

            return new ResponseWrapper<ExpertiseBranchResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ExpertiseBranchResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            ExpertiseBranch expertiseBranch = await unitOfWork.ExpertiseBranchRepository.GetByIdAsync(cancellationToken, id);

            unitOfWork.ExpertiseBranchRepository.SoftDelete(expertiseBranch);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<ExpertiseBranchResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<bool>> ImportFromExcel(CancellationToken cancellationToken, IFormFile formFile) // TODO silinecek
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                formFile.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var i = 1;
                    while (reader.Read()) //Each row of the file
                    {
                        if (i == 1)
                        {
                            i++;
                            continue;
                        }

                        if (reader.GetValue(1) != null)
                        {
                            var subBranchName = reader.GetValue(1).ToString().Trim();
                            var mainBranchName = reader.GetValue(2).ToString().Trim();

                            var subBranch = await unitOfWork.ExpertiseBranchRepository.GetByAsync(cancellationToken, x => x.Name == subBranchName);
                            var mainBranch = await unitOfWork.ExpertiseBranchRepository.GetByAsync(cancellationToken, x => x.Name == mainBranchName);

                            if (subBranch == null || mainBranch == null)
                            {

                            }

                            var relatedExpBranch = new RelatedExpertiseBranch()
                            {
                                SubBranchId = subBranch.Id,
                                PrincipalBranchId = mainBranch.Id,
                            };

                            await relatedExpertiseBranchRepository.AddAsync(cancellationToken, relatedExpBranch);
                        }
                    }
                }
            }
            await unitOfWork.CommitAsync(cancellationToken);
            return new();
        }
        
    }
}
