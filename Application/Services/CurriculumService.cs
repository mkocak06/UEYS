using Application.Interfaces;
using Application.Reports.ExcelReports.CurriculumReports.CurriculumPerfectionsAndRotationsReport;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using DocumentFormat.OpenXml.Presentation;
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
    public class CurriculumService : BaseService, ICurriculumService
    {
        private readonly IMapper mapper;
        private readonly ICurriculumRepository curriculumRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IEducationTrackingRepository educationTrackingRepository;

        public CurriculumService(IMapper mapper, IUnitOfWork unitOfWork, ICurriculumRepository curriculumRepository, IStudentRepository studentRepository, IEducationTrackingRepository educationTrackingRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.curriculumRepository = curriculumRepository;
            this.studentRepository = studentRepository;
            this.educationTrackingRepository = educationTrackingRepository;
        }
        public async Task<ResponseWrapper<List<CurriculumResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Curriculum> curriculums = await curriculumRepository.GetListWithSubRecords(cancellationToken);

            List<CurriculumResponseDTO> response = mapper.Map<List<CurriculumResponseDTO>>(curriculums);

            return new ResponseWrapper<List<CurriculumResponseDTO>> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<bool>> UnDeleteCurriculum(CancellationToken cancellationToken, long id)
        {
            var curriculum = await curriculumRepository.GetByIdAsync(cancellationToken, id);

            if (curriculum != null && curriculum.IsDeleted == true)
            {
                curriculum.IsDeleted = false;
                curriculum.DeleteDate = null;

                curriculumRepository.Update(curriculum);
                await unitOfWork.CommitAsync(cancellationToken);
                return new ResponseWrapper<bool>() { Result = true };
            }
            else
            {
                return new ResponseWrapper<bool>() { Result = false, Message = "Kayıt bulunamadı!" };
            }
        }
        public async Task<PaginationModel<CurriculumResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Curriculum> ordersQuery = curriculumRepository.QueryableCurriculums(x => true);
            FilterResponse<Curriculum> filterResponse = ordersQuery.ToFilterView(filter);

            filterResponse.Query = filterResponse.Query.OrderBy(x => x.ExpertiseBranch.Name)
                                            .ThenBy(x => x.Version);


            var curriculums = mapper.Map<List<CurriculumResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<CurriculumResponseDTO>
            {
                Items = curriculums,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<CurriculumResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Curriculum curriculum = await curriculumRepository.GetByIdWithSubRecords(cancellationToken, id);
            CurriculumResponseDTO response = mapper.Map<CurriculumResponseDTO>(curriculum);

            return new ResponseWrapper<CurriculumResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<CurriculumResponseDTO>>> GetByExpertiseBranchIdAsync(CancellationToken cancellationToken, long expertiseBranchId)
        {
            List<Curriculum> curriculums = await curriculumRepository.GetIncludingList(cancellationToken, x => x.ExpertiseBranchId == expertiseBranchId && x.IsDeleted == false, x => x.ExpertiseBranch);
            List<CurriculumResponseDTO> response = mapper.Map<List<CurriculumResponseDTO>>(curriculums);

            return new ResponseWrapper<List<CurriculumResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CurriculumResponseDTO>> GetLatestByBeginningDateAndExpertiseBranchIdAsync(CancellationToken cancellationToken, long expertiseBranchId, DateTime beginningDate)
        {
            var curriculums = await curriculumRepository.QueryableCurriculums(x => x.ExpertiseBranchId == expertiseBranchId && x.IsDeleted == false).ToListAsync();

            var curriculum = curriculums.OrderByDescending(x => x.EffectiveDate).FirstOrDefault(x => x.EffectiveDate?.Date <= beginningDate.Date);

            CurriculumResponseDTO response = mapper.Map<CurriculumResponseDTO>(curriculum);

            //effectiveDate boş gelme ihtimaline karşı
            response ??= mapper.Map<CurriculumResponseDTO>(curriculums.FirstOrDefault());

            return new ResponseWrapper<CurriculumResponseDTO> { Result = true, Item = response };
        }


        public async Task<ResponseWrapper<CurriculumResponseDTO>> PostAsync(CancellationToken cancellationToken, CurriculumDTO curriculumDTO)
        {
            Curriculum curriculum = mapper.Map<Curriculum>(curriculumDTO);

            await curriculumRepository.AddAsync(cancellationToken, curriculum);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<CurriculumResponseDTO>(curriculum);

            return new ResponseWrapper<CurriculumResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CurriculumResponseDTO>> Put(CancellationToken cancellationToken, long id, CurriculumDTO curriculumDTO)
        {
            Curriculum curriculum = await curriculumRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.ExpertiseBranch);

            if (curriculum.Duration != curriculumDTO.Duration)
            {
                var students = await studentRepository.IncludingQueryable(x => x.IsDeleted == false && x.IsHardDeleted == false && x.CurriculumId == id, x => x.EducationTrackings.Where(x => x.ReasonType == ReasonType.EstimatedFinish)).ToListAsync(cancellationToken);

                foreach (var item in students)
                {
                    var estimatedFinish = item.EducationTrackings.FirstOrDefault();
                    if (estimatedFinish != null)
                    {
                        estimatedFinish.ProcessDate = estimatedFinish?.ProcessDate?.AddYears(curriculumDTO.Duration - curriculum.Duration ?? 0);
                        educationTrackingRepository.Update(estimatedFinish);
                    }
                }
            }
            Curriculum updatedCurriculum = mapper.Map(curriculumDTO, curriculum);

            curriculumRepository.Update(updatedCurriculum);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<CurriculumResponseDTO>(updatedCurriculum);

            return new ResponseWrapper<CurriculumResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CurriculumResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Curriculum curriculum = await curriculumRepository.GetByIdAsync(cancellationToken, id);

            curriculumRepository.SoftDelete(curriculum);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<CurriculumResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<CurriculumResponseDTO>> UnDelete(CancellationToken cancellationToken, long id)
        {
            var curriculum = await curriculumRepository.GetByIdAsync(cancellationToken, id);

            curriculum.IsDeleted = false;
            curriculum.DeleteDate = null;

            curriculumRepository.Update(curriculum);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<CurriculumResponseDTO>() { Result = true };
        }

        public async Task<ResponseWrapper<CurriculumResponseDTO>> CreateCopy(CancellationToken cancellationToken, long id, CurriculumDTO curriculumDTO)
        {
            Curriculum curriculum = await curriculumRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.CurriculumPerfections, x => x.CurriculumRotations, x => x.Standards);

            Curriculum newCurriculum = mapper.Map<Curriculum>(curriculumDTO);
            newCurriculum.CurriculumRotations = new List<CurriculumRotation>();
            newCurriculum.CurriculumPerfections = new List<CurriculumPerfection>();
            newCurriculum.Standards = new List<Standard>();

            newCurriculum.CurriculumRotations = curriculum.CurriculumRotations.Select(x => new CurriculumRotation() { RotationId = x.RotationId }).ToList();
            newCurriculum.CurriculumPerfections = curriculum.CurriculumPerfections.Select(x => new CurriculumPerfection() { PerfectionId = x.PerfectionId }).ToList();
            newCurriculum.Standards = curriculum.Standards.ToList();

            await curriculumRepository.AddAsync(cancellationToken, newCurriculum);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<CurriculumResponseDTO> { Result = true, Item = new() { Id = newCurriculum.Id } };
        }

        public async Task<ResponseWrapper<CurriculumResponseDTO>> UploadExcel(CancellationToken cancellationToken, IFormFile file)
        {
            var expertiseBranches = await unitOfWork.ExpertiseBranchRepository.GetAsync(cancellationToken);
            var curricula = await unitOfWork.CurriculumRepository.GetAsync(cancellationToken);

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

                        var curriculum = new Curriculum()
                        {
                            ExpertiseBranchId = expBranch.Id,
                            Version = reader.GetValue(1)?.ToString(),
                            Duration = reader.GetValue(2) != null ? Convert.ToInt32(reader.GetValue(2)?.ToString()) : 0,
                        };

                        await curriculumRepository.AddAsync(cancellationToken, curriculum);
                    }
                }
            }

            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<byte[]>> CurriculumDetailsExport(CancellationToken cancellationToken)
        {
            var curriculums = await curriculumRepository.CurriculumExportModel(cancellationToken);

            var byteArray = CurriculumPerfectionsAndRotationsReport.Report(curriculums);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }
    }
}
