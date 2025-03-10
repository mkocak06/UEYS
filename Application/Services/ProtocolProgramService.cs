using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Globalization;

namespace Application.Services
{
    public class ProtocolProgramService : BaseService, IProtocolProgramService
    {
        private readonly IMapper mapper;
        private readonly IProtocolProgramRepository protocolProgramRepository;
        private readonly IExpertiseBranchRepository expertiseBranchRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly ILogger<ProtocolProgramService> logger;
        private readonly IKoruRepository koruRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProtocolProgramService(IMapper mapper, IUnitOfWork unitOfWork, IDocumentRepository documentRepository, IProtocolProgramRepository protocolProgramRepository, ILogger<ProtocolProgramService> logger, IExpertiseBranchRepository expertiseBranchRepository, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.protocolProgramRepository = protocolProgramRepository;
            this.logger = logger;
            this.documentRepository = documentRepository;
            this.expertiseBranchRepository = expertiseBranchRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
        }

        public async Task<ResponseWrapper<List<ProtocolProgramResponseDTO>>> GetListAsync(CancellationToken cancellationToken, ProgramType progType)
        {
            List<ProtocolProgram> protocolPrograms = await protocolProgramRepository.GetAsync(cancellationToken, x => x.IsDeleted == false && x.Type == progType);

            List<ProtocolProgramResponseDTO> response = mapper.Map<List<ProtocolProgramResponseDTO>>(protocolPrograms);

            return new ResponseWrapper<List<ProtocolProgramResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<bool>> UnDeleteProtocolProgram(CancellationToken cancellationToken, long id)
        {
            var protocolProgram = await protocolProgramRepository.GetByIdAsync(cancellationToken, id);

            if (protocolProgram != null && protocolProgram.IsDeleted == true)
            {
                protocolProgram.IsDeleted = false;
                protocolProgram.DeleteDate = null;

                protocolProgramRepository.Update(protocolProgram);
                await unitOfWork.CommitAsync(cancellationToken);
                return new ResponseWrapper<bool>() { Result = true };
            }
            else
            {
                return new ResponseWrapper<bool>() { Result = false, Message = "Kayıt bulunamadı!" };
            }
        }

        public async Task<PaginationModel<ProtocolProgramPaginatedResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);
            IQueryable<ProtocolProgram> ordersQuery = protocolProgramRepository.PaginateQuery(zone);
            FilterResponse<ProtocolProgram> filterResponse = ordersQuery.ToFilterView(filter);

            var programs = await filterResponse.Query.Select(x => new ProtocolProgramPaginatedResponseDTO()
            {
                Id = x.Id,
                ProtocolNo = x.ProtocolNo,
                ExpertiseBranch = x.ParentProgram.ExpertiseBranch.Name,
                Faculty = x.ParentProgram.Faculty.Name,
                Hospital = x.ParentProgram.Hospital.Name,
                Province = x.ParentProgram.Hospital.Province != null ? x.ParentProgram.Hospital.Province.Name : "",
                RelatedProgramsCount = x.RelatedDependentPrograms.FirstOrDefault(x => x.IsActive == true) != null ? x.RelatedDependentPrograms.FirstOrDefault(x => x.IsActive == true).ChildPrograms.Count() : 0,
                RelatedRevision = x.RelatedDependentPrograms.FirstOrDefault(x => x.IsActive == true) != null ? x.RelatedDependentPrograms.FirstOrDefault(x => x.IsActive == true).Revision : 0,
                CancelingProtocolNo = x.CancelingProtocolNo
            }).ToListAsync(cancellationToken);

            var response = new PaginationModel<ProtocolProgramPaginatedResponseDTO>
            {
                Items = programs,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id, DocumentTypes docType, ProgramType progType)
        {
            ProtocolProgram? protocolProgram = await protocolProgramRepository.GetByIdWithSubRecords(cancellationToken, id, progType);

            ProtocolProgramResponseDTO response = mapper.Map<ProtocolProgramResponseDTO>(protocolProgram);

            if (progType == ProgramType.Complement)
                foreach (var rdp in response.RelatedDependentPrograms)
                    foreach (var dp in rdp.ChildPrograms)
                    {
                        dp.EducatorDependentPrograms = await protocolProgramRepository.GetEducatorListForComplementProgram(cancellationToken, (long)dp.ProgramId);
                        foreach (var edp in dp.EducatorDependentPrograms)
                            edp.DependentProgramId = dp.Id;
                    }

            var docs = await documentRepository.GetByEntityId(cancellationToken, id, docType);

            var docsResponse = mapper.Map<List<DocumentResponseDTO>>(docs);
            if (protocolProgram != null)
            {
                response.Documents = docsResponse;
                if (response.RelatedDependentPrograms != null)
                    foreach (var rdp in response.RelatedDependentPrograms)
                        rdp.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, (long)rdp.Id, DocumentTypes.RelatedDependentProgram));
            }

            return new ResponseWrapper<ProtocolProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>> GetEducatorListForComplementProgram(CancellationToken cancellationToken, long programId)
        {
            List<EducatorDependentProgramResponseDTO> response = await protocolProgramRepository.GetEducatorListForComplementProgram(cancellationToken, programId);

            return new ResponseWrapper<List<EducatorDependentProgramResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ProtocolProgramByUniversityIdResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId, ProgramType progType)
        {
            List<ProtocolProgramByUniversityIdResponseDTO> response = await protocolProgramRepository.GetListByUniversityId(cancellationToken, uniId, progType);

            return new ResponseWrapper<List<ProtocolProgramByUniversityIdResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> GetByProgramId(CancellationToken cancellationToken, long programId, ProgramType progType)
        {
            ProtocolProgram protocolProgram = await protocolProgramRepository.GetIncluding(cancellationToken, x => x.ParentProgramId == programId && x.Type == progType, x => x.RelatedDependentPrograms);

            ProtocolProgramResponseDTO response = mapper.Map<ProtocolProgramResponseDTO>(protocolProgram);

            return new ResponseWrapper<ProtocolProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, ProtocolProgramDTO protocolProgramDTO)
        {
            ProtocolProgram protocolProgram = mapper.Map<ProtocolProgram>(protocolProgramDTO);

            await protocolProgramRepository.AddAsync(cancellationToken, protocolProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            ProtocolProgramResponseDTO response = mapper.Map<ProtocolProgramResponseDTO>(protocolProgram);

            return new ResponseWrapper<ProtocolProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, ProtocolProgramResponseDTO protocolProgramResponseDTO)
        {
            if (protocolProgramResponseDTO.ParentProgram.ExpertiseBranch.IsIntensiveCare == true)
            {
                List<ExpertiseBranchResponseDTO> expertiseBranches = new();

                foreach (var item in protocolProgramResponseDTO.RelatedDependentPrograms)
                {
                    var hospitalIdList = item.ChildPrograms.Select(x => x.Program.HospitalId).Distinct().ToList();

                    if (!hospitalIdList.Contains(protocolProgramResponseDTO.ParentProgram.HospitalId))
                        hospitalIdList.Add(protocolProgramResponseDTO.ParentProgram.HospitalId);

                    foreach (var hospitalId in hospitalIdList)
                    {
                        var result = await expertiseBranchRepository.GetListForProtocolProgramByHospitalId(cancellationToken, (long)hospitalId);
                        foreach (var item_1 in result)
                        {
                            if (!expertiseBranches.Select(x => x.Id).Contains(item_1.Id))
                                expertiseBranches.Add(mapper.Map<ExpertiseBranchResponseDTO>(item_1));
                        }
                    }
                    //var missingExpertiseBranches = expertiseBranches.Where(x => !item.ChildPrograms.Select(y => y.Program.ExpertiseBranchId).ToList().Contains(x.Id)).ToList();

                    //if (missingExpertiseBranches?.Count > 0)
                    //    return new() { Result = false, Item = new() { MissingExpertiseBranches = missingExpertiseBranches } };
                }
            }
            ProtocolProgram protocolProgram = await protocolProgramRepository.GetByIdWithSubRecords(cancellationToken, id, protocolProgramResponseDTO.Type);

            ProtocolProgramDTO protocolProgramDTO = mapper.Map<ProtocolProgramDTO>(protocolProgramResponseDTO);

            ProtocolProgram updatedProtocolProgram = mapper.Map<ProtocolProgram>(protocolProgramDTO);

            var addedProtocolProgram = await protocolProgramRepository.UpdateWithSubRecords(cancellationToken, id, updatedProtocolProgram);

            if (addedProtocolProgram.RelatedDependentPrograms != null)
                foreach (var rdp in addedProtocolProgram.RelatedDependentPrograms)
                    if (rdp.DocumentId != null)
                    {
                        var docs = await documentRepository.GetAsync(cancellationToken, x => x.Id == rdp.DocumentId && x.EntityId == 0);
                        foreach (var doc in docs)
                            doc.EntityId = rdp.Id;
                    }

            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<ProtocolProgramResponseDTO>(updatedProtocolProgram);

            return new ResponseWrapper<ProtocolProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            ProtocolProgram protocolProgram = await protocolProgramRepository.GetByIdAsync(cancellationToken, id);

            protocolProgramRepository.SoftDelete(protocolProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<ProtocolProgramResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> UnDelete(CancellationToken cancellationToken, long id)
        {
            var protocolProgram = await protocolProgramRepository.GetByIdAsync(cancellationToken, id);

            protocolProgram.IsDeleted = false;
            protocolProgram.DeleteDate = null;

            protocolProgramRepository.Update(protocolProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<ProtocolProgramResponseDTO>() { Result = true };
        }

        private static bool Validation(object value)
        {
            return (value?.ToString() != null && value?.ToString() != "N/A");
        }
        private static dynamic DateTimeParser(object value)
        {
            if (value != null)
            {
                if (Validation(value))
                {
                    return DateTimeOffset.Parse(value.ToString()).DateTime.AddHours(3).ToUniversalTime();
                }
            }
            return null;
        }
        private static dynamic GetAuthorizationCategoryId(object value)
        {
            if (value?.ToString() == "0")
            {
                return 1;
            }
            else if (value?.ToString() == "1")
            {
                return 2;
            }
            else if (value?.ToString() == "2")
            {
                return 3;
            }
            else if (value?.ToString() == "3")
            {
                return 4;
            }
            else if (value?.ToString() == "9")
            {
                return 5;
            }
            else
            {
                return null;
            }
        }
        private async Task<dynamic> GetProvinceId(CancellationToken cancellationToken, object value)
        {
            if (value != null)
            {
                var cities = await unitOfWork.ProvinceRepository.GetAsync(cancellationToken, x => x.Name.ToLower() == value.ToString().Trim().ToLower(new CultureInfo("tr-TR")));
                return cities.FirstOrDefault()?.Id;
            }

            return null;
        }
        private async Task<dynamic> GetExpertiseBranchId(CancellationToken cancellationToken, object value)
        {
            if (value != null)
            {
                var expertiseBranches = await unitOfWork.ExpertiseBranchRepository.GetByAsync(cancellationToken, x => x.Name == value.ToString().Trim());
                return expertiseBranches?.Id;
            }

            return null;
        }
    }
}