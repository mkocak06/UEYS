using Application.Interfaces;
using Application.Reports.ExcelReports.HospitalReports;
using Application.Reports.ExcelReports.HospitalReports.HospitalDetail;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using ExcelDataReader;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Hospital;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Application.Services
{
    public class HospitalService : BaseService, IHospitalService
    {
        private readonly IMapper mapper;
        private readonly IHospitalRepository hospitalRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;

        public HospitalService(IMapper mapper, IUnitOfWork unitOfWork, IHospitalRepository hospitalRepository, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.hospitalRepository = hospitalRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
        }

        public async Task<ResponseWrapper<List<HospitalResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<Hospital> query = hospitalRepository.IncludingQueryable(x => x.IsDeleted == false, x => x.Institution, x => x.Province);

            List<Hospital> hospitals = await query.OrderBy(x=>x.Name).ToListAsync();

            List<HospitalResponseDTO> response = mapper.Map<List<HospitalResponseDTO>>(hospitals);

            return new ResponseWrapper<List<HospitalResponseDTO>> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<bool>> UnDeleteHospital(CancellationToken cancellationToken, long id)
        {
            var hospital = await hospitalRepository.GetByIdAsync(cancellationToken, id);

            if (hospital != null && hospital.IsDeleted == true)
            {
                hospital.IsDeleted = false;
                hospital.DeleteDate = null;

                hospitalRepository.Update(hospital);
                await unitOfWork.CommitAsync(cancellationToken);
                return new ResponseWrapper<bool>() { Result = true };
            }
            else
            {
                return new ResponseWrapper<bool>() { Result = false, Message = "Kayıt bulunamadı!" };
            }
        }
        public async Task<PaginationModel<HospitalResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Hospital> ordersQuery = hospitalRepository.IncludingQueryable(x =>true, x => x.Institution, x => x.Province, x=>x.Faculty.University);
            FilterResponse<Hospital> filterResponse = ordersQuery.ToFilterView(filter);

            var hospitals = mapper.Map<List<HospitalResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<HospitalResponseDTO>
            {
                Items = hospitals,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<PaginationModel<HospitalResponseDTO>> GetDeletedPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Hospital> ordersQuery = hospitalRepository.QueryableHospitals();

            FilterResponse<Hospital> filterResponse = ordersQuery.ToFilterView(filter);

            var hospitals = mapper.Map<List<HospitalResponseDTO>>(await filterResponse.Query.ToListAsync());

            var response = new PaginationModel<HospitalResponseDTO>
            {
                Items = hospitals,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<List<HospitalResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId)
        {
            List<Hospital> hospitals = await hospitalRepository.GetListByUniversityId(uniId);

            List<HospitalResponseDTO> response = mapper.Map<List<HospitalResponseDTO>>(hospitals);

            return new ResponseWrapper<List<HospitalResponseDTO>> { Result = true, Item = response };
        }
        
        public async Task<ResponseWrapper<List<HospitalBreadcrumbDTO>>> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expId)
        {
            List<HospitalBreadcrumbDTO> hospitals = await hospitalRepository.GetListByExpertiseBranchId(cancellationToken, expId);

            return new ResponseWrapper<List<HospitalBreadcrumbDTO>> { Result = true, Item = hospitals };
        }

        public async Task<ResponseWrapper<HospitalResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Hospital hospital = await hospitalRepository.GetWithSubRecords(cancellationToken,id);

            HospitalResponseDTO response = mapper.Map<HospitalResponseDTO>(hospital);

            return new ResponseWrapper<HospitalResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<HospitalResponseDTO>> PostAsync(CancellationToken cancellationToken, HospitalDTO HospitalDTO)
        {
            Hospital hospital = mapper.Map<Hospital>(HospitalDTO);

            await hospitalRepository.AddAsync(cancellationToken, hospital);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<HospitalResponseDTO>(hospital);

            return new ResponseWrapper<HospitalResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<HospitalResponseDTO>> Put(CancellationToken cancellationToken, long id, HospitalDTO HospitalDTO)
        {
            Hospital hospital = await hospitalRepository.GetByIdAsync(cancellationToken, id);

            Hospital updatedHospital = mapper.Map(HospitalDTO, hospital);

            hospitalRepository.Update(updatedHospital);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<HospitalResponseDTO>(updatedHospital);

            return new ResponseWrapper<HospitalResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<HospitalResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Hospital Hospital = await hospitalRepository.GetByIdAsync(cancellationToken, id);

            hospitalRepository.SoftDelete(Hospital);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<HospitalResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<List<MapDTO>>> GetListForMap(CancellationToken cancellationToken, long? universityId)
        {
            return new() { Result = true, Item = await hospitalRepository.GetListForMap(cancellationToken, universityId) };
        }

        public async Task<ResponseWrapper<UserHospitalDetailDTO>> GetUserHospitalDetail(CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            UserHospitalDetailDTO hospital = await hospitalRepository.GetUserHospitalDetail(cancellationToken, userId);
            return new ResponseWrapper<UserHospitalDetailDTO> { Result = true, Item = hospital };
        }

        public async Task<ResponseWrapper<bool>> GetLatLongDetailsFromGoogle(CancellationToken cancellationToken)
        {
            var googleAPIKey = "AIzaSyAtlvLPSqxfqOy1aMbuUxmEtPe4c5KuFv8";
            var hospitals = await hospitalRepository.GetAsync(cancellationToken, x => true);
            var universities = await unitOfWork.UniversityRepository.GetAsync(cancellationToken, x => true);
            var provinces = await unitOfWork.ProvinceRepository.GetAsync(cancellationToken);

            foreach (var hospital in hospitals)
            {
                string address;

                if (hospital.Name.Contains("SUAM"))
                {
                    address = hospital.Name.Replace("SUAM", "TIP FAKÜLTESİ HASTANESİ");
                }
                else
                {
                    address = hospital.Name;
                }

                HttpClient client = new() { BaseAddress = new Uri("https://maps.googleapis.com") };
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/maps/api/geocode/json?address={address}&key={googleAPIKey}");

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

                GoogleAPIModel? googleResult = await response.Content.ReadFromJsonAsync<GoogleAPIModel>(cancellationToken: cancellationToken);

                if (googleResult?.Status == "OK")
                {
                    hospital.Latitude = googleResult.Results?.FirstOrDefault()?.Geometry.Location.Lat;
                    hospital.Longitude = googleResult.Results?.FirstOrDefault()?.Geometry.Location.Lng;

                    hospitalRepository.Update(hospital);
                }
            }

            foreach (var university in universities)
            {
                string address;

                if (university.Name.Contains("SUAM"))
                {
                    address = university.Name.Replace("SUAM", "TIP FAKÜLTESİ HASTANESİ");
                }
                else
                {
                    address = university.Name;
                }

                HttpClient client = new() { BaseAddress = new Uri("https://maps.googleapis.com") };
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/maps/api/geocode/json?address={address}&key={googleAPIKey}");

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

                GoogleAPIModel? googleResult = await response.Content.ReadFromJsonAsync<GoogleAPIModel>(cancellationToken: cancellationToken);

                if (googleResult?.Status == "OK")
                {
                    university.Latitude = googleResult.Results?.FirstOrDefault()?.Geometry.Location.Lat;
                    university.Longitude = googleResult.Results?.FirstOrDefault()?.Geometry.Location.Lng;

                    unitOfWork.UniversityRepository.Update(university);
                }
            }

            foreach (var province in provinces)
            {
                HttpClient client = new() { BaseAddress = new Uri("https://maps.googleapis.com") };
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/maps/api/geocode/json?address={province.Name}&key={googleAPIKey}");

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

                GoogleAPIModel? googleResult = await response.Content.ReadFromJsonAsync<GoogleAPIModel>(cancellationToken: cancellationToken);

                if (googleResult?.Status == "OK")
                {
                    province.Latitude = googleResult.Results?.FirstOrDefault()?.Geometry.Location.Lat;
                    province.Longitude = googleResult.Results?.FirstOrDefault()?.Geometry.Location.Lng;

                    unitOfWork.ProvinceRepository.Update(province);
                }
            }

            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true, Item = true };
        }

		public async Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter)
		{
			filter.pageSize = int.MaxValue;
			filter.page = 1;

			var response = await GetPaginateList(cancellationToken, filter);

			var byteArray = ExportList.ExportListReport(response.Items);

			return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
		}
        
        public async Task<ResponseWrapper<byte[]>> DetailedExcelExport(CancellationToken cancellationToken, ProgramFilter excelFilter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);
            
            var staffCountbyExpBranches = await unitOfWork.ProgramRepository.CountByExpertiseBranch(cancellationToken, zone, excelFilter);
            var educators = await unitOfWork.EducatorRepository.ListForExcelQuery(zone, excelFilter);
            var students = await unitOfWork.StudentRepository.ExportExcelDetailedQuery(zone, excelFilter);
            var programs = await unitOfWork.ProgramRepository.GetProgramsForExcelExport(zone, excelFilter);

            var byteArray = HospitalDetail.HospitalDetailReport(staffCountbyExpBranches, programs, educators, students);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }

        public async Task<ResponseWrapper<HospitalResponseDTO>> UnDelete(CancellationToken cancellationToken, long id)
        {
            var hospital = await hospitalRepository.GetByIdAsync(cancellationToken, id);

            hospital.IsDeleted = false;
            hospital.DeleteDate = null;

            hospitalRepository.Update(hospital);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<HospitalResponseDTO>() { Result = true };
        }
        public async Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetHospitalCountByParentInstitution(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryableHospitals = hospitalRepository.QueryableHospitalsForCharts(zone);

            FilterResponse<HospitalChartModel> filterResponse = queryableHospitals.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false).GroupBy(x => x.ParentInstitutionName)
                                                     .Select(x => new ActivePassiveResponseModel()
                                                     {
                                                         SeriesName = !string.IsNullOrEmpty(x.Key) ? x.Key : "Belirtilmemiş",
                                                         ActiveRecordsCount = x.Count()
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<ActivePassiveResponseModel>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<bool>> ImportFromExcel(CancellationToken cancellationToken, IFormFile formFile) // TODO YUEP güncellemeleri bittikten sonra silinecek
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

                        if (reader.GetValue(0) != null)
                        {
                            var existProffesionName = reader.GetValue(0)?.ToString()?.Trim();
                            var existProffesion = await unitOfWork.ProfessionRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == existProffesionName);

                            var existProvinceName = reader.GetValue(1)?.ToString()?.Trim();
                            var existProvince = await unitOfWork.ProvinceRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == existProvinceName);

                            var existInstitutionName = reader.GetValue(2)?.ToString()?.Trim();
                            var existInstitution = await unitOfWork.InstitutionRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == existInstitutionName);

                            var existUniversityName = reader.GetValue(3)?.ToString()?.Trim();
                            var existUniversity = await unitOfWork.UniversityRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == existUniversityName);

                            var existFacultyName = reader.GetValue(4)?.ToString()?.Trim();
                            var existFaculty = await unitOfWork.FacultyRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == existFacultyName);

                            var existHospitalName = reader.GetValue(5)?.ToString()?.Trim();
                            var existHospital = await unitOfWork.HospitalRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == existHospitalName);

                            var institutionName = reader.GetValue(6)?.ToString()?.Trim() == "YÖK-Üni/Devlet" ? "Yükseköğretim Kurumu (YÖK)" : reader.GetValue(6)?.ToString()?.Trim();
                            var institution = await unitOfWork.InstitutionRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == institutionName);

                            var universityName = reader.GetValue(7)?.ToString()?.Trim();
                            var university = await unitOfWork.UniversityRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == universityName);

                            var facultyName = reader.GetValue(8)?.ToString()?.Trim();
                            var faculty = await unitOfWork.FacultyRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == facultyName);

                            var hospitalName = reader.GetValue(9)?.ToString()?.Trim();
                            var hospital = await unitOfWork.HospitalRepository.FirstOrDefaultAsync(cancellationToken, x => x.Name == hospitalName);

                            if (existFacultyName != facultyName)
                            {
                                if (faculty == null)
                                {
                                    if (university == null)
                                    {
                                    }

                                    faculty = new()
                                    {
                                        Name = facultyName,
                                        ProfessionId = existProffesion.Id,
                                        UniversityId = university.Id,
                                        Address = existProvinceName
                                    };

                                    await unitOfWork.FacultyRepository.AddAsync(cancellationToken, faculty);
                                    await unitOfWork.CommitAsync(cancellationToken);
                                }
                                else
                                {

                                }
                            }
                            if (existHospitalName != hospitalName)
                            {
                                if (existHospitalName == null)
                                {
                                    if (hospital == null)
                                    {
                                        hospital = new ()
                                        {
                                            Name = hospitalName,
                                            ProvinceId = existProvince.Id,
                                            Address = existProffesionName,
                                            InstitutionId = institution.Id,
                                            FacultyId = faculty.Id,
                                        };

                                        await unitOfWork.HospitalRepository.AddAsync(cancellationToken, hospital);
                                        await unitOfWork.CommitAsync(cancellationToken);
                                    }
                                }
                                else
                                {
                                    if (existHospital == null)
                                    {

                                    }
                                    else
                                    {
                                        existHospital.Name = hospitalName;

                                        unitOfWork.HospitalRepository.Update(existHospital);
                                        await unitOfWork.CommitAsync(cancellationToken);
                                    }
                                }
                            }
                            if (existUniversityName != universityName)
                            {

                            }
                            if (university == null)
                            {

                            }
                        }
                    }
                }
            }
            return new();
        }
    }
}
