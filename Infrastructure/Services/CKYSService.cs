using AutoMapper;
using CKYS;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Core.Models.ConfigModels;
using Infrastructure.Data;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Models;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Extentsions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.ResponseModels;
using YOK;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public class CKYSService : ICKYSService
    {
        private readonly AppSettingsModel appSettingsModel;
        private Service1Soap ckysServicesProxy;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<CKYSService> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CKYSService(AppSettingsModel appSettingsModel, IMapper mapper, ApplicationDbContext dbContext,
            ILogger<CKYSService> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.appSettingsModel = appSettingsModel;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        private Service1Soap GetCKYSServicesProxy()
        {
            if (ckysServicesProxy != null) return ckysServicesProxy;
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
            {
                MaxReceivedMessageSize = int.MaxValue
            };

            ckysServicesProxy = new ChannelFactory<Service1Soap>(binding,
                    new EndpointAddress("https://ckysweb.saglik.gov.tr/UETSWs/Service1.asmx"))
                .CreateChannel();
            return ckysServicesProxy;
        }

        private DoctorExpertiseBranch GetExpertiseBranch(List<ExpertiseBranch> exBrs, string ckysBransAdi,
            string ckysTescilTarihi, string ckysTescilNo, string ckysGraduationSchool)
        {
            var doctorExBr = new DoctorExpertiseBranch();

            var uzmBransAdi = ckysBransAdi.Trim();
            if (uzmBransAdi.Contains(","))
            {
                if ((uzmBransAdi.StartsWith("AĞIZ") || uzmBransAdi.StartsWith("PLASTİK")) &&
                    uzmBransAdi.Split(',').Length != 2)
                    uzmBransAdi = uzmBransAdi.Split(',')[2].Trim();
                else
                    uzmBransAdi = uzmBransAdi.Split(',')[1].Trim();
            }

            var expertiseBranch = exBrs.FirstOrDefault(x => x.Name == uzmBransAdi);
            doctorExBr.ExpertiseBranchId = expertiseBranch?.Id;
            doctorExBr.ExpertiseBranchName = expertiseBranch?.Name;
            doctorExBr.RegistrationBranchName = uzmBransAdi;
            doctorExBr.RegistrationDate = ckysTescilTarihi;
            doctorExBr.RegistrationNo = ckysTescilNo;
            doctorExBr.RegistrationGraduationSchool = ckysGraduationSchool;
            doctorExBr.IsPrincipal = expertiseBranch?.IsPrincipal;
            doctorExBr.SubBrIds = expertiseBranch?.SubBranches?.Select(x => x.SubBranchId)?.ToList();

            return doctorExBr;
        }

        public async Task<ResponseWrapper<CKYSDoctor>> CKYSDoktorSorgula(string identityNo, bool isBackjob = false)
        {
            if (isBackjob == false)
            {
                var userId = httpContextAccessor.HttpContext.GetUserId();
                var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                var log = $"{userId} - {identityNo} CKYSDoktorSorgula.";
                logger.LogInformation(log);
                try
                {
                    await dbContext.Logs.AddAsync(new() { LogType = LogType.CKYS, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error CKYSDoktorSorgula SaveChanges");
                }
            }

            string userName = appSettingsModel.CKYS.UserName;
            string password = appSettingsModel.CKYS.Password;

            DoktorList doktorSorgula = await GetCKYSServicesProxy().UetsDrTcknAsync(userName, password, identityNo);

            var doktor = new CKYSDoctor() { DoctorExpertiseBranches = new() };

            if (doktorSorgula.Islem.sonuc == 1)
            {
                var cKYSResponse = doktorSorgula.DrList.FirstOrDefault();

                var exBrs = dbContext.ExpertiseBranches.Include(x => x.SubBranches).ToList();

                var doctorTitle = dbContext.Titles.Where(x => x.TitleType == TitleType.Staff).ToList()
                    .FirstOrDefault(x => x.Name.Contains(cKYSResponse.v_aktif_unvan_ad.Trim()));

                doktor.WorkPlaceName = cKYSResponse.v_aktif_birim_ad;
                doktor.WorkSituation = cKYSResponse.v_calisma_durumu;
                doktor.StaffTitleId = doctorTitle?.Id;
                doktor.StaffTitleName = doctorTitle?.Name;
                doktor.Email = cKYSResponse.email;
                doktor.Phone = cKYSResponse.cep_no;


                if (cKYSResponse.uzm1_brans_adi != "")
                {
                    doktor.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs, cKYSResponse.uzm1_brans_adi,
                        cKYSResponse.uzm1_tescil_trh, cKYSResponse.uzm1_tescil_no, cKYSResponse.uzm1_mezun_okul));

                    if (cKYSResponse.uzm2_brans_adi != "")
                    {
                        doktor.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs, cKYSResponse.uzm2_brans_adi,
                            cKYSResponse.uzm2_tescil_trh, cKYSResponse.uzm2_tescil_no, cKYSResponse.uzm2_mezun_okul));

                        if (cKYSResponse.uzm3_brans_adi != "")
                        {
                            doktor.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs, cKYSResponse.uzm3_brans_adi,
                                cKYSResponse.uzm3_tescil_trh, cKYSResponse.uzm3_tescil_no,
                                cKYSResponse.uzm3_mezun_okul));

                            if (cKYSResponse.uzm4_brans_adi != "")
                            {
                                doktor.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs,
                                    cKYSResponse.uzm4_brans_adi, cKYSResponse.uzm4_tescil_trh,
                                    cKYSResponse.uzm4_tescil_no, cKYSResponse.uzm4_mezun_okul));

                                if (cKYSResponse.uzm5_brans_adi != "")
                                    doktor.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs,
                                        cKYSResponse.uzm5_brans_adi, cKYSResponse.uzm5_tescil_trh,
                                        cKYSResponse.uzm5_tescil_no, cKYSResponse.uzm5_mezun_okul));
                            }
                        }
                    }
                }

                return new() { Item = doktor, Result = true };
            }
            else
                return new() { Result = false, Message = doktorSorgula.Islem.bilgi };
        }

        public async Task<ResponseWrapper<GraduationDetailResponseDTO>> GetMedicineInfo(string identityNo)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - {identityNo} GetMedicineInfo.";
            logger.LogInformation(log);
            try
            {
                await dbContext.Logs.AddAsync(new() { LogType = LogType.CKYS, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetMedicineInfo SaveChanges");
            }

            string userName = appSettingsModel.CKYS.UserName;
            string password = appSettingsModel.CKYS.Password;

            DoktorList doktorSorgula = await GetCKYSServicesProxy().UetsDrTcknAsync(userName, password, identityNo);

            var doktor = new GraduationDetailResponseDTO();

            if (doktorSorgula.Islem.sonuc == 1)
            {
                var cKYSResponse = doktorSorgula.DrList.FirstOrDefault();

                doktor.GraduationDate = cKYSResponse.tip_mez_trh;
                doktor.GraduationFaculty = cKYSResponse.tip_mezun_fakulte;
                doktor.GraduationField = "TIP";
                doktor.HigherEducationDetail = "Lisans";
                doktor.GraduationUniversity = cKYSResponse.tip_mezun_okul;
                doktor.IsPhd = false;
                doktor.DiplomaNumber = cKYSResponse.tip_tescil_no;

                return new() { Item = doktor, Result = true };
            }
            else
                return new() { Result = false, Message = doktorSorgula.Islem.bilgi };
        }

        public async Task<CKYSStudent> CKYSOgrenciSorgulaAsync(string identityNo)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - {identityNo} CKYSOgrenciSorgulaAsync.";
            logger.LogInformation(log);
            try
            {
                await dbContext.Logs.AddAsync(new() { LogType = LogType.CKYS, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error CKYSOgrenciSorgulaAsync SaveChanges");
            }
            string userName = appSettingsModel.CKYS.UserName;
            string password = appSettingsModel.CKYS.Password;

            DoktorList ogrenciSorgula = await GetCKYSServicesProxy().UetsDrTcknAsync(userName, password, identityNo);

            var ogrenci = new CKYSStudent() { DoctorExpertiseBranches = new() };

            if (ogrenciSorgula.Islem.sonuc == 1)
            {
                var cKYSResponse = ogrenciSorgula.DrList.FirstOrDefault();

                var exBrs = dbContext.ExpertiseBranches.Include(x => x.SubBranches).ToList();

                var studentTitle = dbContext.Titles.Where(x => x.TitleType == TitleType.Staff).ToList()
                    .FirstOrDefault(x => x.Name.Contains(cKYSResponse.v_aktif_unvan_ad.Trim()));

                ogrenci.GraduatedSchool = string.IsNullOrEmpty(cKYSResponse.tip_mezun_okul) == true ? cKYSResponse.dhk_mezun_okul + " " + cKYSResponse.dhk_mezun_fakulte : cKYSResponse.tip_mezun_okul + " " + cKYSResponse.tip_mezun_fakulte;
                ogrenci.GraduatedDate = string.IsNullOrEmpty(cKYSResponse.tip_mezun_okul) == true ? cKYSResponse.dhk_mez_trh : cKYSResponse.tip_mez_trh;
                ogrenci.MedicineRegistrationDate = string.IsNullOrEmpty(cKYSResponse.tip_mezun_okul) == true ? cKYSResponse.dhk_tescil_trh : cKYSResponse.tip_tescil_trh;
                ogrenci.MedicineRegistrationNo = string.IsNullOrEmpty(cKYSResponse.tip_mezun_okul) == true ? cKYSResponse.dhk_tescil_no : cKYSResponse.tip_tescil_no;
                ogrenci.StaffTitleId = studentTitle?.Id;
                ogrenci.StaffTitleName = studentTitle?.Name;
                ogrenci.Phone = cKYSResponse.cep_no;
                ogrenci.Email = cKYSResponse.email;

                if (cKYSResponse.uzm1_brans_adi != "")
                {
                    ogrenci.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs, cKYSResponse.uzm1_brans_adi,
                        cKYSResponse.uzm1_tescil_trh, cKYSResponse.uzm1_tescil_no, cKYSResponse.uzm1_mezun_okul));

                    if (cKYSResponse.uzm2_brans_adi != "")
                    {
                        ogrenci.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs, cKYSResponse.uzm2_brans_adi,
                            cKYSResponse.uzm2_tescil_trh, cKYSResponse.uzm2_tescil_no, cKYSResponse.uzm2_mezun_okul));

                        if (cKYSResponse.uzm3_brans_adi != "")
                        {
                            ogrenci.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs, cKYSResponse.uzm3_brans_adi,
                                cKYSResponse.uzm3_tescil_trh, cKYSResponse.uzm3_tescil_no,
                                cKYSResponse.uzm3_mezun_okul));

                            if (cKYSResponse.uzm4_brans_adi != "")
                            {
                                ogrenci.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs,
                                    cKYSResponse.uzm4_brans_adi, cKYSResponse.uzm4_tescil_trh,
                                    cKYSResponse.uzm4_tescil_no, cKYSResponse.uzm4_mezun_okul));

                                if (cKYSResponse.uzm5_brans_adi != "")
                                    ogrenci.DoctorExpertiseBranches.Add(GetExpertiseBranch(exBrs,
                                        cKYSResponse.uzm5_brans_adi, cKYSResponse.uzm5_tescil_trh,
                                        cKYSResponse.uzm5_tescil_no, cKYSResponse.uzm5_mezun_okul));
                            }
                        }
                    }
                }
            }

            return ogrenci;
        }

        public async Task<dynamic> CKYSTest(string identityNo)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - {identityNo} CKYSTest.";
            logger.LogInformation(log);
            try
            {
                await dbContext.Logs.AddAsync(new() { LogType = LogType.CKYS, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error CKYSTest SaveChanges");
            }
            var userName = appSettingsModel.CKYS.UserName;
            var password = appSettingsModel.CKYS.Password;

            var doktorSorgula = await GetCKYSServicesProxy().UetsDrTcknAsync(userName, password, identityNo);

            return doktorSorgula;
        }
    }
}