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
using Shared.Types;

namespace Application.Services
{
    public class RotationService : BaseService, IRotationService
    {
        private readonly IMapper mapper;
        private readonly IRotationRepository rotationRepository;

        public RotationService(IMapper mapper, IUnitOfWork unitOfWork, IRotationRepository rotationRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.rotationRepository = rotationRepository;
        }

        public async Task<ResponseWrapper<List<RotationResponseDTO>>> GetListByCurriculumId(CancellationToken cancellationToken, long curriculumId)
        {
            var rotations = await rotationRepository.GetListByCurriculumId(cancellationToken, curriculumId);
            var response = mapper.Map<List<RotationResponseDTO>>(rotations);

            return new ResponseWrapper<List<RotationResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<CurriculumRotationResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            var curriculumRotations = await rotationRepository.GetListByStudentId(cancellationToken, studentId);
            List<PerfectionResponseDTO> perfections = new();

            curriculumRotations.ForEach(x => x.Perfections.ToList().ForEach(a => perfections.Add(new PerfectionResponseDTO()
            {
                Id = a.Id,
                PerfectionType = a.PerfectionType,
                CurriculumId = a.CurriculumPerfections != null ? a.CurriculumPerfections.First().CurriculumId : null,
                CurriculumRotationId = a.CurriculumRotationId,
                PName = mapper.Map<PropertyResponseDTO>(a.PerfectionProperties?.Select(z => z.Property).FirstOrDefault(z => z.PropertyType == PropertyType.PerfectionName)),
                Group = mapper.Map<PropertyResponseDTO>(a.PerfectionProperties?.Select(z => z.Property).FirstOrDefault(z => z.PropertyType == PropertyType.PerfectionGroup)),
                Seniorty = mapper.Map<PropertyResponseDTO>(a.PerfectionProperties?.Select(z => z.Property).FirstOrDefault(z => z.PropertyType == PropertyType.PerfectionSeniorty)),
                LevelList = mapper.Map<List<PropertyResponseDTO>>(a.PerfectionProperties?.Select(z => z.Property).Where(z => z.PropertyType == PropertyType.PerfectionLevel).ToList()),
                MethodList = mapper.Map<List<PropertyResponseDTO>>(a.PerfectionProperties?.Select(z => z.Property).Where(z => z.PropertyType == PropertyType.PerfectionMethod).ToList()),
                IsDeleted = a.IsDeleted,
                StudentPerfections = studentId != 0 ? mapper.Map<List<StudentPerfectionResponseDTO>>(a.StudentPerfections?.Where(z => z.IsDeleted == false && z.StudentId == studentId)) : new()

            })));

            var response = mapper.Map<List<CurriculumRotationResponseDTO>>(curriculumRotations);
            // TODO Burası ne anlamadım, değişecek
            response.ForEach(x =>
            {
                var matchingPerfections = perfections.Where(a => a.CurriculumRotationId == x.Id).ToList();
                x.Perfections = matchingPerfections;
            });
            return new ResponseWrapper<List<CurriculumRotationResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<RotationResponseDTO>>> GetFormerStudentListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            var rotations = await rotationRepository.GetFormerStudentListByStudentId(cancellationToken, studentId);

            var response = mapper.Map<List<RotationResponseDTO>>(rotations);

            return new ResponseWrapper<List<RotationResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<RotationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Rotation> ordersQuery = rotationRepository.IncludingQueryable(x => x.IsDeleted == false, x => x.ExpertiseBranch);
            FilterResponse<Rotation> filterResponse = ordersQuery.ToFilterView(filter);

            var rotations = mapper.Map<List<RotationResponseDTO>>(await filterResponse.Query.OrderBy(x => x.ExpertiseBranch.Name).ToListAsync(cancellationToken));

            var response = new PaginationModel<RotationResponseDTO>
            {
                Items = rotations,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<RotationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Rotation rotation = await rotationRepository.GetByIdWithSubRecords(cancellationToken, id);
            RotationResponseDTO response = mapper.Map<RotationResponseDTO>(rotation);

            return new ResponseWrapper<RotationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<RotationResponseDTO>> PostAsync(CancellationToken cancellationToken, RotationDTO rotationDTO)
        {
            Rotation rotation = mapper.Map<Rotation>(rotationDTO);

            await rotationRepository.AddAsync(cancellationToken, rotation);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<RotationResponseDTO>(rotation);

            return new ResponseWrapper<RotationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<RotationResponseDTO>> Put(CancellationToken cancellationToken, long id, RotationDTO rotationDTO)
        {
            Rotation rotation = await rotationRepository.GetByIdAsync(cancellationToken, id);

            Rotation updatedRotation = mapper.Map(rotationDTO, rotation);

            rotationRepository.Update(updatedRotation);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<RotationResponseDTO>(updatedRotation);

            return new ResponseWrapper<RotationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<RotationResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Rotation rotation = await rotationRepository.GetByIdAsync(cancellationToken, id);

            rotationRepository.SoftDelete(rotation);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<RotationResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<RotationResponseDTO>> UploadExcel(CancellationToken cancellationToken, IFormFile file)
        {
            var expertiseBranches = await unitOfWork.ExpertiseBranchRepository.GetAsync(cancellationToken);

            System.Globalization.CultureInfo turkey = new System.Globalization.CultureInfo("tr-TR");
            System.Threading.Thread.CurrentThread.CurrentCulture = turkey;

            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    int i = 1;

                    while (reader.Read()) //Each row of the file
                    {
                        if (i == 1)
                        {
                            i++;
                            continue;
                        }

                        var expBranchName = reader.GetValue(0)?.ToString()?.Trim();
                        var expBranch = expertiseBranches.FirstOrDefault(x => x.Name == expBranchName);

                        var rotation = new Rotation()
                        {
                            ExpertiseBranchId = expBranch.Id,
                            Duration = reader.GetValue(1)?.ToString(),
                            IsRequired = reader.GetValue(3)?.ToString() == "False",
                        };

                        await rotationRepository.AddAsync(cancellationToken, rotation);
                    }
                }
            }

            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}
