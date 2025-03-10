using Application.Interfaces;
using Application.Reports.ExcelReports.StudentReports.PerfectionReports.ClinicalReports;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Globalization;
using System.Text;

namespace Application.Services
{
    public class PerfectionService : BaseService, IPerfectionService
    {
        private readonly IMapper mapper;
        private readonly IPerfectionRepository perfectionRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly IStudentPerfectionRepository studentPerfectionRepository;

        public PerfectionService(IMapper mapper, IUnitOfWork unitOfWork, IPerfectionRepository perfectionRepository, IPropertyRepository propertyRepository, IStudentPerfectionRepository studentPerfectionRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.perfectionRepository = perfectionRepository;
            this.propertyRepository = propertyRepository;
            this.studentPerfectionRepository = studentPerfectionRepository;
        }
        public async Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<Perfection> query = perfectionRepository.IncludingQueryable(x => x.IsDeleted == false, x => x.CurriculumPerfections);

            List<Perfection> perfections = await query.ToListAsync(cancellationToken);

            List<PerfectionResponseDTO> response = mapper.Map<List<PerfectionResponseDTO>>(perfections);

            return new() { Result = true, Item = response };
        }


        public async Task<PaginationModel<PerfectionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Perfection> ordersQuery = perfectionRepository.IncludingQueryable(x => true, x => x.PerfectionProperties, x => x.CurriculumPerfections);
            long studentId = 0;
            var studentIdFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "StudentId");
            if (studentIdFilter != null)
            {
                studentId = (long)studentIdFilter.Value;
                filter.Filter.Filters.Remove(studentIdFilter);
            }

            if (filter.Filter.Filters.Any(x => x.Field == "PropertyType"))
            {
                var filteredTypeList = filter.Filter.Filters.Where(x => x.Field == "PropertyType").ToList();
                foreach (var item in filteredTypeList)
                {
                    ordersQuery = ordersQuery.Where(x => x.PerfectionProperties.Any(x => x.Property.PropertyType == EnumExtension.ParseEnum<PropertyType>(item.Operator) &&
                                                                                                             (item.Value == null || x.Property.Name.ToLower().Contains(item.Value.ToString().ToLower(new CultureInfo("tr-TR"))))));
                    filter.Filter.Filters.Remove(item);
                }
            }

            var rotationFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "RotationId");
            if (rotationFilter != null)
            {
                ordersQuery = ordersQuery.Where(x => x.CurriculumRotation.RotationId == (long)rotationFilter.Value);
                filter.Filter.Filters.Remove(rotationFilter);
            }
            var crotationFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "CurriculumId");
            if (crotationFilter != null && rotationFilter != null)
            {
                ordersQuery = ordersQuery.Where(x => x.CurriculumRotation.CurriculumId == (long)crotationFilter.Value);
                filter.Filter.Filters.Remove(crotationFilter);
            }
            FilterResponse<PerfectionResponseDTO> filterResponse = ordersQuery.Select(x => new PerfectionResponseDTO()
            {
                Id = x.Id,
                PerfectionType = x.PerfectionType,
                CurriculumId = x.CurriculumPerfections != null ? x.CurriculumPerfections.First(x=>x.CurriculumId == (long)crotationFilter.Value).CurriculumId : null,
                CurriculumRotationId = x.CurriculumRotationId,
                PName = mapper.Map<PropertyResponseDTO>(x.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionName)),
                Group = mapper.Map<PropertyResponseDTO>(x.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionGroup)),
                Seniorty = mapper.Map<PropertyResponseDTO>(x.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionSeniorty)),
                LevelList = mapper.Map<List<PropertyResponseDTO>>(x.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionLevel).ToList()),
                MethodList = mapper.Map<List<PropertyResponseDTO>>(x.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionMethod).ToList()),
                IsDeleted = x.IsDeleted,
                StudentPerfections = studentId != 0 ? mapper.Map<List<StudentPerfectionResponseDTO>>(x.StudentPerfections.Where(x => x.IsDeleted == false && x.StudentId == studentId)) : new()
            }).ToFilterView(filter);

            var perfectionList = await filterResponse.Query.ToListAsync(cancellationToken);

            var response = new PaginationModel<PerfectionResponseDTO>
            {
                Items = perfectionList,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType)
        {
            var perfections = perfectionRepository.GetByStudentIdQuery(studentId);
            var response = await perfections.Where(x => x.PerfectionType == perfectionType).Select(x => new PerfectionResponseDTO()
            {
                Id = x.Id,
                CurriculumPerfections = mapper.Map<List<CurriculumPerfectionResponseDTO>>(x.CurriculumPerfections),
                PerfectionType = x.PerfectionType,
                StudentPerfections = mapper.Map<List<StudentPerfectionResponseDTO>>(x.StudentPerfections.Where(x => x.IsDeleted == false && x.StudentId == studentId)),
                PName = mapper.Map<PropertyResponseDTO>(x.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionName)),
                Group = mapper.Map<PropertyResponseDTO>(x.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionGroup)),
                Seniorty = mapper.Map<PropertyResponseDTO>(x.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionSeniorty)),
                LevelList = mapper.Map<List<PropertyResponseDTO>>(x.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionLevel).ToList()),
                MethodList = mapper.Map<List<PropertyResponseDTO>>(x.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionMethod).ToList()),
                IsDeleted = x.IsDeleted
            }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<PerfectionResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PerfectionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            var perfection = perfectionRepository.IncludingQueryable(x => x.Id == id, x => x.CurriculumPerfections, x => x.PerfectionProperties);
            PerfectionResponseDTO response = perfection.Select(perfection => new PerfectionResponseDTO()
            {
                Id = perfection.Id,
                CurriculumRotationId = perfection.CurriculumRotationId,
                CurriculumId = perfection.CurriculumPerfections != null ? perfection.CurriculumPerfections.First().CurriculumId : null,
                CurriculumPerfections = mapper.Map<List<CurriculumPerfectionResponseDTO>>(perfection.CurriculumPerfections),
                PerfectionType = perfection.PerfectionType,
                PName = mapper.Map<PropertyResponseDTO>(perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionName)),
                Group = mapper.Map<PropertyResponseDTO>(perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionGroup)),
                Seniorty = mapper.Map<PropertyResponseDTO>(perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionSeniorty)),
                LevelList = mapper.Map<List<PropertyResponseDTO>>(perfection.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionLevel).ToList()),
                MethodList = mapper.Map<List<PropertyResponseDTO>>(perfection.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionMethod).ToList()),
                IsDeleted = perfection.IsDeleted
            }).First();
            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PerfectionResponseDTO>> PostAsync(CancellationToken cancellationToken, PerfectionDTO perfectionDTO)
        {
            Perfection perfection = mapper.Map<Perfection>(perfectionDTO);

            AddPerfectionProperty(ref perfection, perfectionDTO.PName);
            AddPerfectionProperty(ref perfection, perfectionDTO.Group);
            AddPerfectionProperty(ref perfection, perfectionDTO.Seniorty);

            foreach (var item in perfectionDTO.LevelList)
            {
                AddPerfectionProperty(ref perfection, item);
            }
            foreach (var item in perfectionDTO.MethodList)
            {
                AddPerfectionProperty(ref perfection, item);
            }

            await perfectionRepository.AddAsync(cancellationToken, perfection);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<PerfectionResponseDTO>(perfection);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PerfectionResponseDTO>> PostCurriculumRotationPerfection(CancellationToken cancellationToken, long curriculumId, long rotationId, PerfectionDTO perfectionDTO)
        {
            CurriculumRotation curriculumRotation = await unitOfWork.CurriculumRotationRepository.GetByAsync(cancellationToken, x => x.CurriculumId == curriculumId && x.RotationId == rotationId);

            Perfection perfection = mapper.Map<Perfection>(perfectionDTO);
            perfection.CurriculumRotationId = curriculumRotation?.Id;

            AddPerfectionProperty(ref perfection, perfectionDTO.PName);
            AddPerfectionProperty(ref perfection, perfectionDTO.Group);
            AddPerfectionProperty(ref perfection, perfectionDTO.Seniorty);

            foreach (var item in perfectionDTO.LevelList)
            {
                AddPerfectionProperty(ref perfection, item);
            }
            foreach (var item in perfectionDTO.MethodList)
            {
                AddPerfectionProperty(ref perfection, item);
            }

            await perfectionRepository.AddAsync(cancellationToken, perfection);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<PerfectionResponseDTO>(perfection);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PerfectionResponseDTO>> Put(CancellationToken cancellationToken, long id, PerfectionDTO perfectionDTO)
        {
            Perfection perfection = await perfectionRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.PerfectionProperties);

            Perfection updatedPerfection = mapper.Map(perfectionDTO, perfection);

            perfectionRepository.Update(updatedPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<PerfectionResponseDTO>(updatedPerfection);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PerfectionResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Perfection perfection = await perfectionRepository.GetByIdAsync(cancellationToken, id);

            perfectionRepository.SoftDelete(perfection);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<RotationResponseDTO>> UploadExcel(CancellationToken cancellationToken, IFormFile file)
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

                        var curriculumName = reader.GetValue(2)?.ToString()?.Trim();
                        var curriculum = curricula.FirstOrDefault(x => x.ExpertiseBranch.Name == curriculumName);

                        var expBranchName = reader.GetValue(0)?.ToString()?.Trim();
                        var expBranch = expertiseBranches.FirstOrDefault(x => x.Name == expBranchName);

                        if (curriculum != null)
                        {
                            var perfection = new Perfection();


                            if (!string.IsNullOrWhiteSpace(reader.GetValue(4)?.ToString()?.Trim()))
                            {
                                var group = await propertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(4).ToString().Trim() && x.PropertyType == PropertyType.PerfectionGroup);

                                if (group != null)
                                {
                                    perfection.PerfectionProperties.Add(new PerfectionProperty()
                                    {
                                        PropertyId = group.Id
                                    });
                                }
                                else
                                {

                                }
                            }
                            if (!string.IsNullOrWhiteSpace(reader.GetValue(7)?.ToString()?.Trim()))
                            {
                                var seniority = await propertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(7).ToString().Trim() && x.PropertyType == PropertyType.PerfectionSeniorty);

                                if (seniority != null)
                                {
                                    perfection.PerfectionProperties.Add(new PerfectionProperty()
                                    {
                                        PropertyId = seniority.Id
                                    });
                                }
                                else
                                {

                                }
                            }
                            if (!string.IsNullOrWhiteSpace(reader.GetValue(5)?.ToString()?.Trim()))
                            {
                                var perfectionLevels = reader.GetValue(5)?.ToString()?.Trim().Split(",");

                                foreach (var perfectionLevel in perfectionLevels)
                                {
                                    var level = await propertyRepository.GetByAsync(cancellationToken, x => x.Name == perfectionLevel && x.PropertyType == PropertyType.PerfectionLevel);

                                    if (level != null)
                                    {
                                        perfection.PerfectionProperties.Add(new PerfectionProperty()
                                        {
                                            PropertyId = level.Id,
                                        });
                                    }
                                    else
                                    {

                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(reader.GetValue(6)?.ToString()?.Trim()))
                            {
                                var perfectionMethods = reader.GetValue(6)?.ToString()?.Trim().Split(",");

                                foreach (var perfectionMethod in perfectionMethods)
                                {
                                    var method = await propertyRepository.GetByAsync(cancellationToken, x => x.Name == perfectionMethod && x.PropertyType == PropertyType.PerfectionMethod);

                                    if (method != null)
                                    {
                                        perfection.PerfectionProperties.Add(new PerfectionProperty()
                                        {
                                            PropertyId = method.Id,
                                        });
                                    }
                                    else
                                    {

                                    }
                                }
                            }

                            await perfectionRepository.AddAsync(cancellationToken, perfection);
                        }
                        else
                        {

                        }

                    }
                }
            }

            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<bool>> ImportPerfectionList(CancellationToken cancellationToken, IFormFile formFile) // TODO silinecek 
        {
            var perfectionWithoutCurriculum = new List<Perfection>();
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

                        var curriculum = await unitOfWork.CurriculumRepository.GetByAsync(cancellationToken, x => x.ExpertiseBranch.Name == reader.GetValue(1).ToString().Trim());

                        var perfection = new Perfection();
                        perfection.PerfectionProperties = new List<PerfectionProperty>();

                        //perfection.CurriculumId = curriculum.Id;
                        perfection.PerfectionType = PerfectionType.Interventional;

                        if (!string.IsNullOrEmpty(reader.GetValue(2)?.ToString()?.Trim()))
                        {
                            var group = await unitOfWork.PropertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(2).ToString().Trim() && x.PropertyType == PropertyType.PerfectionGroup && x.PerfectionType == PerfectionType.Interventional);

                            perfection.PerfectionProperties.Add(new PerfectionProperty()
                            {
                                PropertyId = group.Id
                            });
                        }

                        if (!string.IsNullOrEmpty(reader.GetValue(3)?.ToString()?.Trim()))
                        {
                            var name = await unitOfWork.PropertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(3).ToString().Trim() && x.PropertyType == PropertyType.PerfectionName && x.PerfectionType == PerfectionType.Interventional);

                            perfection.PerfectionProperties.Add(new PerfectionProperty()
                            {
                                PropertyId = name.Id
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(reader.GetValue(4)?.ToString()?.Trim()))
                        {
                            var perfectionLevels = reader.GetValue(4)?.ToString()?.Trim().Split(",");

                            foreach (var perfectionLevel in perfectionLevels)
                            {
                                if (!string.IsNullOrWhiteSpace(perfectionLevel))
                                {
                                    var level = await propertyRepository.GetByAsync(cancellationToken, x => x.Name == perfectionLevel.Trim() && x.PropertyType == PropertyType.PerfectionLevel);

                                    perfection.PerfectionProperties.Add(new PerfectionProperty()
                                    {
                                        PropertyId = level.Id,
                                    });
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(reader.GetValue(5)?.ToString()?.Trim()))
                        {
                            var seniority = await unitOfWork.PropertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(5).ToString().Trim() && x.PropertyType == PropertyType.PerfectionSeniorty);

                            perfection.PerfectionProperties.Add(new PerfectionProperty()
                            {
                                PropertyId = seniority.Id
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(reader.GetValue(6)?.ToString()?.Trim()))
                        {
                            var perfectionMethods = reader.GetValue(6)?.ToString()?.Trim().Split(",");

                            foreach (var perfectionMethod in perfectionMethods)
                            {
                                if (!string.IsNullOrWhiteSpace(perfectionMethod))
                                {
                                    var method = await propertyRepository.GetByAsync(cancellationToken, x => x.Name == perfectionMethod.Trim() && x.PropertyType == PropertyType.PerfectionMethod);

                                    perfection.PerfectionProperties.Add(new PerfectionProperty()
                                    {
                                        PropertyId = method.Id,
                                    });
                                }
                            }
                        }

                        try
                        {
                            await unitOfWork.PerfectionRepository.AddAsync(cancellationToken, perfection);
                        }
                        catch (Exception e)
                        {
                        }
                        await unitOfWork.CommitAsync(cancellationToken);
                    }
                }
            }
            //await unitOfWork.CommitAsync(cancellationToken);
            return new();
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExportClinical(CancellationToken cancellationToken, long studentId)
        {
            var response_1 = await GetListByStudentId(cancellationToken, studentId, PerfectionType.Clinical);

            var response_2 = await studentPerfectionRepository.GetListByStudentId(cancellationToken, studentId, PerfectionType.Clinical);

            foreach (var item in response_1.Item)
            {
                if (!response_2.Any(x => x.PerfectionId == item.Id))
                {
                    response_2.Add(new StudentPerfectionResponseDTO { Perfection = item });
                }
            }

            var byteArray = ExportList.ExportListReport(response_2);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExportInterventional(CancellationToken cancellationToken, long studentId)
        {
            var response_1 = await GetListByStudentId(cancellationToken, studentId, PerfectionType.Interventional);

            var response_2 = await studentPerfectionRepository.GetListByStudentId(cancellationToken, studentId, PerfectionType.Interventional);

            foreach (var item in response_1.Item)
            {
                if (!response_2.Any(x => x.PerfectionId == item.Id))
                {
                    response_2.Add(new StudentPerfectionResponseDTO { Perfection = item });
                }
            }

            var byteArray = ExportList.ExportListReport(response_2);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }

        private void AddPerfectionProperty(ref Perfection perfection, PropertyDTO property)
        {
            if (property?.Id != null)
            {
                perfection.PerfectionProperties.Add(new PerfectionProperty()
                {
                    PropertyId = property.Id
                });
            }
        }
        public async Task<ResponseWrapper<bool>> ImportRotationPerfectionList(CancellationToken cancellationToken, IFormFile formFile)
        {
            var expBranchList = await unitOfWork.ExpertiseBranchRepository.GetAsync(cancellationToken);
            var curriculumList = await unitOfWork.CurriculumRepository.GetAsync(cancellationToken);
            var propertyList = await unitOfWork.PropertyRepository.GetAsync(cancellationToken);

            var perfectionWithoutCurriculum = new List<Perfection>();
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

                        var version = reader.GetValue(2).ToString().Trim().TrimEnd('.');

                        var expBranch = expBranchList.FirstOrDefault(x => x.Name == reader.GetValue(1).ToString().Trim());
                        if (expBranch == null)
                        {

                        }

                        var curriculum = curriculumList.FirstOrDefault(x => x.ExpertiseBranch.Name == expBranch.Name && x.Version == version);
                        if (curriculum == null)
                        {
                            curriculum = new Curriculum()
                            {
                                Duration = reader.GetValue(7) != null ? Convert.ToInt32(reader.GetValue(7).ToString().Trim()) : null,
                                ExpertiseBranchId = expBranch.Id,
                                Version = version,
                                IsActive = true,
                            };

                            await unitOfWork.CurriculumRepository.AddAsync(cancellationToken, curriculum);
                            await unitOfWork.CommitAsync(cancellationToken);
                            curriculumList.Add(curriculum);
                        };

                        var rotationName = reader.GetValue(3).ToString().Trim();
                        var rotation = await unitOfWork.RotationRepository.GetByCurriculumIdAndExpBranchName(cancellationToken, curriculum.Id, rotationName);

                        if (rotation == null)
                        {
                            rotation = new Rotation()
                            {
                                Duration = reader.GetValue(7)?.ToString().Trim(),
                                ExpertiseBranchId = expBranchList.FirstOrDefault(x => x.Name == rotationName)?.Id,
                                CurriculumRotations = new List<CurriculumRotation>() { new CurriculumRotation()
                                    {
                                        CurriculumId = curriculum.Id,
                                    }
                                }
                            };

                            if (rotation.ExpertiseBranchId == null)
                            {

                            }
                            await unitOfWork.RotationRepository.AddAsync(cancellationToken, rotation);
                            await unitOfWork.CommitAsync(cancellationToken);
                        }

                        var perfection = new Perfection();
                        var perfectionType = reader.GetValue(4)?.ToString()?.Trim() == "KLİNİK YETKİNLİK" ? PerfectionType.Clinical : PerfectionType.Interventional;
                        perfection.PerfectionType = perfectionType;

                        var isRequired = reader.GetValue(8)?.ToString()?.Trim() == "ZORUNLU";
                        perfection.IsRequired = isRequired;
                        perfection.CurriculumRotationId = (await unitOfWork.CurriculumRotationRepository.GetByAsync(cancellationToken, x => x.CurriculumId == curriculum.Id && x.RotationId == rotation.Id))?.Id;

                        if (perfection.CurriculumRotationId == null)
                        {

                        }

                        perfection.SpecialProvision = reader.GetValue(9)?.ToString()?.Trim();

                        if (!string.IsNullOrEmpty(reader.GetValue(5)?.ToString()?.Trim()))
                        {
                            var name = propertyList.FirstOrDefault(x => x.Name == reader.GetValue(5).ToString().Trim().ToUpper(new CultureInfo("tr-TR")) && x.PropertyType == PropertyType.PerfectionName && x.PerfectionType == perfectionType);
                            if (name == null)
                            {
                                name = new Property()
                                {
                                    Name = reader.GetValue(5)?.ToString().Trim().ToUpper(new CultureInfo("tr-TR")),
                                    PerfectionType = perfectionType,
                                    PropertyType = PropertyType.PerfectionName,
                                };

                                await unitOfWork.PropertyRepository.AddAsync(cancellationToken, name);
                                await unitOfWork.CommitAsync(cancellationToken);
                                propertyList.Add(name);
                            }

                            perfection.PerfectionProperties.Add(new PerfectionProperty()
                            {
                                PropertyId = name.Id
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(reader.GetValue(6)?.ToString()?.Trim()))
                        {
                            var perfectionLevels = reader.GetValue(6)?.ToString()?.Trim().Split(",");

                            foreach (var perfectionLevel in perfectionLevels)
                            {
                                if (!string.IsNullOrWhiteSpace(perfectionLevel))
                                {
                                    var level = propertyList.FirstOrDefault(x => x.Name == perfectionLevel.Trim() && x.PropertyType == PropertyType.PerfectionLevel);

                                    if (level == null)
                                    {
                                        level = new Property()
                                        {
                                            Name = perfectionLevel,
                                            PropertyType = PropertyType.PerfectionLevel,
                                        };

                                        await unitOfWork.PropertyRepository.AddAsync(cancellationToken, level);
                                        await unitOfWork.CommitAsync(cancellationToken);
                                        propertyList.Add(level);
                                    }
                                    perfection.PerfectionProperties.Add(new PerfectionProperty()
                                    {
                                        PropertyId = level.Id,
                                    });
                                }
                            }
                        }

                        try
                        {
                            await unitOfWork.PerfectionRepository.AddAsync(cancellationToken, perfection);
                            await unitOfWork.CommitAsync(cancellationToken);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
            //await unitOfWork.CommitAsync(cancellationToken);
            return new ResponseWrapper<bool>()
            {
                Result = true
            };
        }
        public async Task<ResponseWrapper<bool>> CurriculumPerfectionsImport(CancellationToken cancellationToken, IFormFile formFile) // TODO silinecek 
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

                        var perfectionType = PerfectionType.Interventional;
                        var perfection = new Perfection();
                        perfection.PerfectionProperties = new List<PerfectionProperty>();

                        if (reader.GetValue(0)?.ToString()?.Trim() == "KLİNİK YETKİNLİK")
                        {
                            perfectionType = PerfectionType.Clinical;
                        }
                        
                        if (!string.IsNullOrEmpty(reader.GetValue(1)?.ToString()?.Trim()))
                        {
                            var group = await unitOfWork.PropertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(1).ToString().Trim() && x.PropertyType == PropertyType.PerfectionGroup && x.PerfectionType == perfectionType);

                            if (group == null)
                            {
                               group = new Property()
                               {
                                   PropertyType = PropertyType.PerfectionGroup,
                                   PerfectionType = perfectionType,
                                   Name = reader.GetValue(1)?.ToString()?.Trim(),
                               };
                               
                               await unitOfWork.PropertyRepository.AddAsync(cancellationToken, group);
                               await unitOfWork.CommitAsync(cancellationToken);
                            }

                            perfection.PerfectionProperties.Add(new PerfectionProperty()
                            {
                                PropertyId = group.Id
                            });
                        }

                        if (!string.IsNullOrEmpty(reader.GetValue(2)?.ToString()?.Trim()))
                        {
                            var name = await unitOfWork.PropertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(2).ToString().Trim() && x.PropertyType == PropertyType.PerfectionName && x.PerfectionType == perfectionType);

                            if (name == null)
                            {
                                name = new Property()
                                {
                                    PropertyType = PropertyType.PerfectionName,
                                    PerfectionType = perfectionType,
                                    Name = reader.GetValue(2)?.ToString()?.Trim(),
                                };
                               
                                await unitOfWork.PropertyRepository.AddAsync(cancellationToken, name);
                                await unitOfWork.CommitAsync(cancellationToken);
                            }
                            
                            perfection.PerfectionProperties.Add(new PerfectionProperty()
                            {
                                PropertyId = name.Id
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(reader.GetValue(3)?.ToString()?.Trim()))
                        {
                            var perfectionLevels = reader.GetValue(3)?.ToString()?.Trim().Split(",");

                            foreach (var perfectionLevel in perfectionLevels)
                            {
                                if (!string.IsNullOrWhiteSpace(perfectionLevel))
                                {
                                    var level = await propertyRepository.GetByAsync(cancellationToken, x => x.Name == perfectionLevel.Trim() && x.PropertyType == PropertyType.PerfectionLevel);

                                    if (level == null)
                                    {
                                        level = new Property()
                                        {
                                            PropertyType = PropertyType.PerfectionLevel,
                                            Name = perfectionLevel,
                                        };
                               
                                        await unitOfWork.PropertyRepository.AddAsync(cancellationToken, level);
                                        await unitOfWork.CommitAsync(cancellationToken);
                                    }
                                    
                                    perfection.PerfectionProperties.Add(new PerfectionProperty()
                                    {
                                        PropertyId = level.Id,
                                    });
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(reader.GetValue(4)?.ToString()?.Trim()))
                        {
                            var seniority = await unitOfWork.PropertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(4).ToString().Trim() && x.PropertyType == PropertyType.PerfectionSeniorty);

                            if (seniority == null)
                            {
                                seniority = new Property()
                                {
                                    PropertyType = PropertyType.PerfectionSeniorty,
                                    Name = reader.GetValue(4)?.ToString()?.Trim(),
                                };
                               
                                await unitOfWork.PropertyRepository.AddAsync(cancellationToken, seniority);
                                await unitOfWork.CommitAsync(cancellationToken);
                            }
                            
                            perfection.PerfectionProperties.Add(new PerfectionProperty()
                            {
                                PropertyId = seniority.Id
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(reader.GetValue(5)?.ToString()?.Trim()))
                        {
                            var perfectionMethods = reader.GetValue(5)?.ToString()?.Trim().Split(",");

                            foreach (var perfectionMethod in perfectionMethods)
                            {
                                if (!string.IsNullOrWhiteSpace(perfectionMethod))
                                {
                                    var method = await propertyRepository.GetByAsync(cancellationToken, x => x.Name == perfectionMethod.Trim() && x.PropertyType == PropertyType.PerfectionMethod);

                                    if (method == null)
                                    {
                                        method = new Property()
                                        {
                                            PropertyType = PropertyType.PerfectionMethod,
                                            Name = perfectionMethod,
                                        };
                               
                                        await unitOfWork.PropertyRepository.AddAsync(cancellationToken, method);
                                        await unitOfWork.CommitAsync(cancellationToken);
                                    }
                                    
                                    perfection.PerfectionProperties.Add(new PerfectionProperty()
                                    {
                                        PropertyId = method.Id,
                                    });
                                }
                            }
                        }

                        try
                        {
                            await unitOfWork.PerfectionRepository.AddAsync(cancellationToken, perfection);
                            await unitOfWork.CommitAsync(cancellationToken);
                        }
                        catch (Exception e)
                        {
                        }
                        
                        var curriculumPerfection = new CurriculumPerfection()
                        {
                            CurriculumId = reader.GetValue(6)?.ToString()?.Trim() == "V 2.3" ? 223 : 30,
                            PerfectionId = perfection?.Id
                        };
                        
                        try
                        {
                            await unitOfWork.CurriculumPerfectionRepository.AddAsync(cancellationToken, curriculumPerfection);
                            await unitOfWork.CommitAsync(cancellationToken);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
            //await unitOfWork.CommitAsync(cancellationToken);
            return new();
        }
        public async Task<ResponseWrapper<bool>> CurriculumRotationPerfectionsImport(CancellationToken cancellationToken, IFormFile formFile) // TODO silinecek 
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

                        var perfectionType = PerfectionType.Interventional;
                        var perfection = new Perfection();
                        perfection.PerfectionProperties = new List<PerfectionProperty>();

                        if (reader.GetValue(4)?.ToString()?.Trim() == "KLİNİK YETKİNLİK")
                        {
                            perfectionType = PerfectionType.Clinical;
                        }
                        
                        if (!string.IsNullOrEmpty(reader.GetValue(5)?.ToString()?.Trim()))
                        {
                            var name = await unitOfWork.PropertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(5).ToString().Trim() && x.PropertyType == PropertyType.PerfectionName && x.PerfectionType == perfectionType);

                            if (name == null)
                            {
                                name = new Property()
                                {
                                    PropertyType = PropertyType.PerfectionName,
                                    PerfectionType = perfectionType,
                                    Name = reader.GetValue(5)?.ToString()?.Trim(),
                                };
                               
                                await unitOfWork.PropertyRepository.AddAsync(cancellationToken, name);
                                await unitOfWork.CommitAsync(cancellationToken);
                            }
                            
                            perfection.PerfectionProperties.Add(new PerfectionProperty()
                            {
                                PropertyId = name.Id
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(reader.GetValue(6)?.ToString()?.Trim()))
                        {
                            var perfectionLevels = reader.GetValue(6)?.ToString()?.Trim().Split(",");

                            foreach (var perfectionLevel in perfectionLevels)
                            {
                                if (!string.IsNullOrWhiteSpace(perfectionLevel))
                                {
                                    var level = await propertyRepository.GetByAsync(cancellationToken, x => x.Name == perfectionLevel.Trim() && x.PropertyType == PropertyType.PerfectionLevel);

                                    if (level == null)
                                    {
                                        level = new Property()
                                        {
                                            PropertyType = PropertyType.PerfectionLevel,
                                            Name = perfectionLevel,
                                        };
                               
                                        await unitOfWork.PropertyRepository.AddAsync(cancellationToken, level);
                                        await unitOfWork.CommitAsync(cancellationToken);
                                    }
                                    
                                    perfection.PerfectionProperties.Add(new PerfectionProperty()
                                    {
                                        PropertyId = level.Id,
                                    });
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(reader.GetValue(7)?.ToString()?.Trim()))
                        {
                            var seniority = await unitOfWork.PropertyRepository.GetByAsync(cancellationToken, x => x.Name == reader.GetValue(7).ToString().Trim() && x.PropertyType == PropertyType.PerfectionSeniorty);

                            if (seniority == null)
                            {
                                seniority = new Property()
                                {
                                    PropertyType = PropertyType.PerfectionSeniorty,
                                    Name = reader.GetValue(7)?.ToString()?.Trim(),
                                };
                               
                                await unitOfWork.PropertyRepository.AddAsync(cancellationToken, seniority);
                                await unitOfWork.CommitAsync(cancellationToken);
                            }
                            
                            perfection.PerfectionProperties.Add(new PerfectionProperty()
                            {
                                PropertyId = seniority.Id
                            });
                        }

                        perfection.IsRequired = reader.GetValue(8)?.ToString()?.Trim() == "ZORUNLU";
                        perfection.PerfectionType = perfectionType;

                        var curriculumId = reader.GetValue(2)?.ToString()?.Trim() == "2.3." ? 223 : 30;

                        var curriculumRotation = await unitOfWork.CurriculumRotationRepository.GetByAsync(cancellationToken, x => x.CurriculumId == curriculumId && x.Rotation.ExpertiseBranch.Name == reader.GetValue(3).ToString().Trim() && x.IsDeleted == false);

                        if (curriculumRotation == null)
                        {
                            
                        }
                        perfection.CurriculumRotationId = curriculumRotation.Id;

                        try
                        {
                            await unitOfWork.PerfectionRepository.AddAsync(cancellationToken, perfection);
                            await unitOfWork.CommitAsync(cancellationToken);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
            //await unitOfWork.CommitAsync(cancellationToken);
            return new();
        }
    }
}
