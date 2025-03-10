using Core.Entities;
using Core.Interfaces;
using Core.Models.ConfigModels;
using Core.Models.YOKModels;
using Infrastructure.Data;
using MezunSorgulamav2;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Extentsions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Types;
using TariheGoreSaglikMezunlariniSorgulaV1;
using TcKimlikNoileAkademikIdariPersonelveGorevlendirmeSorgulaV1;
using TcKimlikNoileAkademisyenGorevlendirmeSorgulaV1;
using TcKimlikNoileAkademisyenUzmanlikSorgulaV1;
using TcKimlikNoileIdariPersonelSorgulaV1;
using YOK;

namespace Infrastructure.Services
{
    public class YOKService : IYOKService
    {
        private readonly AppSettingsModel appSettingsModel;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<YOKService> logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICKYSService cKYSService;

        public YOKService(AppSettingsModel appSettingsModel, ApplicationDbContext dbContext, ILogger<YOKService> logger,
            IHttpContextAccessor httpContextAccessor, ICKYSService cKYSService)
        {
            this.appSettingsModel = appSettingsModel;
            this.dbContext = dbContext;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
            this.cKYSService = cKYSService;
        }

        #region MezunSorgula

        private MezunOgrenciv2_PortTypeClient GetClientMezun()
        {
            var url = appSettingsModel.YOK.MezunUrl;

            var usr = appSettingsModel.YOK.UserName;
            var pass = appSettingsModel.YOK.Password;

            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
            {
                MaxReceivedMessageSize = int.MaxValue
            };

            EndpointAddress address = new EndpointAddress(url);

            MezunOgrenciv2_PortTypeClient client = new MezunOgrenciv2_PortTypeClient(binding, address);
            client.ClientCredentials.UserName.UserName = usr;
            client.ClientCredentials.UserName.Password = pass;

            return client;
        }

        public async Task<ResponseWrapper<dynamic>> GetMezunAsync(string identityNo)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - {identityNo} GetMezunAsync.";
            logger.LogInformation(log);
            try
            {
                await dbContext.Logs.AddAsync(new() { LogType = LogType.YOK, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetMezunAsync SaveChanges");
            }
            MezunOgrenciv2_PortTypeClient client = GetClientMezun();

            var request = new TcKimlikNoilMezunOgrenciSorgulav2Request(Convert.ToInt64(identityNo));

            var response = await client.TcKimlikNoilMezunOgrenciSorgulav2Async(request);

            var listResult = response.MEZUN_OGRENCI_KAYITLARI.ToList();
            var sonuc = response.SONUC;

            GraduatedDTO entItem = new GraduatedDTO();

            if (listResult.Count > 0)
            {
                for (int i = 0; i < listResult.Count; i++)
                {
                    var resultItem = listResult[i];

                    entItem.TckNo = resultItem.TCKNO;
                    entItem.Ad = resultItem.ADI;
                    entItem.Soyad = resultItem.SOYADI;
                    entItem.AnneAd = resultItem.ANNE_ADI;
                    entItem.BabaAd = resultItem.BABA_ADI;
                    entItem.DiplomaNot = resultItem.DIPLOMA_NOTU ?? 0;
                    entItem.DiplomaNotSistemi = resultItem.DIPLOMA_NOT_SISTEMI;
                    entItem.DiplomaNo = resultItem.DIPLOMA_NO;
                    entItem.UniversiteAd = resultItem.UNIVERSITE_ADI;
                    entItem.FakMyoYoEns = resultItem.FAK_MYO_YO_ENS;
                    entItem.ProgramAd = resultItem.PROGRAM_ADI;
                    entItem.BirimId = resultItem.BIRIM_ID;
                    entItem.UniversiteId = resultItem.UNIV_ID ?? 0;
                    entItem.Durum = resultItem.DURUM;

                    if (resultItem.DOGUM_TARIHI != null)
                        entItem.DogumTarih = new DateTime(resultItem.DOGUM_TARIHI.YIL.Value,
                            resultItem.DOGUM_TARIHI.AY.Value, resultItem.DOGUM_TARIHI.GUN.Value);

                    if (resultItem.MEZUNIYET_TARIHI != null)
                        entItem.MezuniyetTarih = new DateTime(resultItem.MEZUNIYET_TARIHI.YIL.Value,
                            resultItem.MEZUNIYET_TARIHI.AY.Value, resultItem.MEZUNIYET_TARIHI.GUN.Value);

                    return new() { Result = true, Item = entItem };
                }
            }

            return new() { Result = false, Item = new GraduatedDTO() };
        }

        #endregion

        #region TariheGoreSaglikMezunuSorgula

        private SaglikMezunlariPortClient GetClientSaglikMezun()
        {
            var url = appSettingsModel.YOK.SaglikMezunUrl;

            var usr = appSettingsModel.YOK.UserName;
            var pass = appSettingsModel.YOK.Password;

            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 524288;

            EndpointAddress address = new EndpointAddress(url);

            SaglikMezunlariPortClient client = new SaglikMezunlariPortClient(binding, address);
            client.ClientCredentials.UserName.UserName = usr;
            client.ClientCredentials.UserName.Password = pass;

            return client;
        }

        public async Task<ResponseWrapper<dynamic>> GetSaglikMezunAsync(int gun, int ay, int yil)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - gun:{gun} - ay:{ay} - yil:{yil} GetSaglikMezunAsync.";
            logger.LogInformation(log);
            try
            {
                await dbContext.Logs.AddAsync(new() { LogType = LogType.YOK, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetSaglikMezunAsync SaveChanges");
            }
            SaglikMezunlariPortClient client = GetClientSaglikMezun();

            var request = new SaglikMezunlariRequest();
            request.SaglikMezunlariniGetir.MezuniyetGun = gun;
            request.SaglikMezunlariniGetir.MezuniyetAy = ay;
            request.SaglikMezunlariniGetir.MezuniyetYil = yil;

            var response = await client.SaglikMezunlariAsync(request);

            var resultList = response.SaglikMezunlariResponse.SaglikMezunlari.ToList();
            var sonuc = response.SaglikMezunlariResponse.Sonuc;

            HealthGraduatedDTO entItem = new HealthGraduatedDTO();

            if (resultList.Count > 0)
            {
                for (int i = 0; i < resultList.Count; i++)
                {
                    var resultItem = resultList[i];

                    entItem.TckNo = resultItem.TcKimlikNo;
                    entItem.BirimId = resultItem.BirimID;
                }

                return new() { Result = true, Item = entItem };
            }

            return new() { Result = false, Item = entItem };
        }

        #endregion

        #region AkademikIdariPersonelSorgula

        private AkademikveIdariPersonelPortClient GetClientAkademikVeIdariPersonel()
        {
            var url = appSettingsModel.YOK.AkademikIdariUrl;

            var usr = appSettingsModel.YOK.UserName;
            var pass = appSettingsModel.YOK.Password;

            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 524288;

            EndpointAddress address = new EndpointAddress(url);

            AkademikveIdariPersonelPortClient client = new AkademikveIdariPersonelPortClient(binding, address);
            client.ClientCredentials.UserName.UserName = usr;
            client.ClientCredentials.UserName.Password = pass;

            return client;
        }

        public AcademicAdminStaffResponseDTO GetAkademikIdariPersonelGorevAsync(long? tckNo)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            var log = $"{userId} - {tckNo} GetAkademikIdariPersonelGorevAsync.";
            logger.LogInformation(log);
            try
            {
                dbContext.Logs.Add(new() { LogType = LogType.YOK, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetAkademikIdariPersonelGorevAsync SaveChanges");
            }

            AkademikveIdariPersonelPortClient client = GetClientAkademikVeIdariPersonel();

            var request = new AkademikveIdariPersonelRequest
            {
                AkademikveIdariPersonelBilgiGetir = new AkademikveIdariPersonelRequestType
                {
                    TcKimlikNo = tckNo ?? 0
                }
            };

            var yokResponse = client.AkademikveIdariPersonelAsync(request).Result.AkademikveIdariPersonelResponse;

            AcademicAdminStaffResponseDTO response = new AcademicAdminStaffResponseDTO();

            if (yokResponse.AkademikveIdariPersonel?.AkademikPersonel != null)
            {
                var titles = dbContext.Titles.Where(x => x.TitleType == TitleType.Academic).ToList();
                var doctorTitle = titles.FirstOrDefault(x =>
                    x.Name.Contains(yokResponse.AkademikveIdariPersonel.AkademikPersonel.KadroUnvan.Trim()));
                response.AcademicTitleName = doctorTitle?.Name;
                response.AcademicTitleId = doctorTitle?.Id;
            }

            return response;
        }

        #endregion

        #region AkademisyenGorevlendirmeSorgula

        private AkademisyenGorevlendirmePortClient GetClientAkademisyenGorevlendirme()
        {
            var url = appSettingsModel.YOK.AkademisyenGorevlendirmeUrl;

            var usr = appSettingsModel.YOK.UserName;
            var pass = appSettingsModel.YOK.Password;

            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 524288;

            EndpointAddress address = new EndpointAddress(url);

            AkademisyenGorevlendirmePortClient client = new AkademisyenGorevlendirmePortClient(binding, address);
            client.ClientCredentials.UserName.UserName = usr;
            client.ClientCredentials.UserName.Password = pass;

            return client;
        }

        public async Task<ResponseWrapper<dynamic>> GetAkademisyenGorevlendirmeAsync(long? tckNo)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - {tckNo} GetAkademisyenGorevlendirmeAsync.";
            logger.LogInformation(log);
            try
            {
                await dbContext.Logs.AddAsync(new() { LogType = LogType.YOK, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetAkademisyenGorevlendirmeAsync SaveChanges");
            }

            AkademisyenGorevlendirmePortClient client = GetClientAkademisyenGorevlendirme();

            var request = new AkademisyenGorevlendirmeRequest
            {
                AkademisyenGorevlendirmeBilgiGetir = new AkademisyenGorevlendirmeRequestType()
                {
                    TcKimlikNo = tckNo ?? 0
                }
            };

            var response = await client.AkademisyenGorevlendirmeAsync(request);

            var resultItem = response.AkademisyenGorevlendirmeResponse.AkademisyenGorevlendirme;
            var sonuc = response.AkademisyenGorevlendirmeResponse.Sonuc;

            AcademicianAssignmentDTO entItem = new AcademicianAssignmentDTO();

            if (resultItem != null)
            {
                entItem.TckNo = resultItem.TcKimlikNo;
                entItem.GorevKurumId = resultItem.KirkB_BirimId;
                entItem.GorevBaslangicTarihi = resultItem.KirkB_BaslangicTarihi;
                entItem.GorevBitisTarihi = resultItem.KirkB_BitisTarihi;

                return new() { Item = entItem, Result = true };
            }

            return new() { Item = entItem, Result = false };
        }

        #endregion

        #region AkademisyenUzmanlik

        private AkademisyenUzmanlikPortClient GetAkamisyenUzmanlikClient()
        {
            string urlStr = appSettingsModel.YOK.AkademisyenUzmanlikUrl;

            var usr = appSettingsModel.YOK.UserName;
            var pass = appSettingsModel.YOK.Password;

            //Binding
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 524288;

            //Endpoint Address
            EndpointAddress address = new EndpointAddress(urlStr);

            //Client
            AkademisyenUzmanlikPortClient client = new AkademisyenUzmanlikPortClient(binding, address);
            client.ClientCredentials.UserName.UserName = usr;
            client.ClientCredentials.UserName.Password = pass;

            return client;
        }

        public async Task<ResponseWrapper<dynamic>> GetAkademisyenUzmanlikAsync(long? tckNo)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - {tckNo} GetAkademisyenUzmanlikAsync.";
            logger.LogInformation(log);
            try
            {
                await dbContext.Logs.AddAsync(new() { LogType = LogType.YOK, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetAkademisyenUzmanlikAsync SaveChanges");
            }

            AkademisyenUzmanlikPortClient client = GetAkamisyenUzmanlikClient();

            var request = new AkademisyenUzmanlikRequest
            {
                AkademisyenUzmanlikBilgiGetir = new AkademisyenUzmanlikRequestType()
                {
                    TcKimlikNo = tckNo ?? 0,
                }
            };

            var response = await client.AkademisyenUzmanlikAsync(request);

            var resultItem = response.AkademisyenUzmanlikResponse.AkademisyenUzmanlik;
            var sonuc = response.AkademisyenUzmanlikResponse.Sonuc;

            AcademicianExpertiseDTO entItem = new AcademicianExpertiseDTO();

            if (resultItem != null)
            {
                entItem.TckNo = resultItem.TcKimlikNo;
                entItem.Dus = resultItem.DUS;
                entItem.Eus = resultItem.EUS;
                entItem.Tus = resultItem.TUS;
                entItem.Yandal1 = resultItem.YDUS1;
                entItem.Yandal2 = resultItem.YDUS2;

                return new() { Item = entItem, Result = true };
            }

            ;
            return new() { Item = entItem, Result = false };
        }

        #endregion

        #region IdariPersonel

        private IdariPersonelPortClient GetIdariPersonelClient()
        {
            string urlStr = appSettingsModel.YOK.IdariPersonelUrl;

            var usr = appSettingsModel.YOK.UserName;
            var pass = appSettingsModel.YOK.Password;

            //Binding
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 524288;

            //Endpoint Address
            EndpointAddress address = new EndpointAddress(urlStr);

            //Client
            IdariPersonelPortClient client = new IdariPersonelPortClient(binding, address);
            client.ClientCredentials.UserName.UserName = usr;
            client.ClientCredentials.UserName.Password = pass;

            return client;
        }

        public async Task<ResponseWrapper<dynamic>> GetIdariPersonelAsync(long? tckNo)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - {tckNo} GetIdariPersonelAsync.";
            logger.LogInformation(log);
            try
            {
                await dbContext.Logs.AddAsync(new() { LogType = LogType.YOK, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetIdariPersonelAsync SaveChanges");
            }

            IdariPersonelPortClient client = GetIdariPersonelClient();

            var request = new IdariPersonelRequest
            {
                IdariPersonelBilgiGetir = new IdariPersonelRequestType
                {
                    TcKimlikNo = tckNo ?? 0
                }
            };

            var response = await client.IdariPersonelAsync(request);

            var resultItem = response.IdariPersonelResponse.IdariPersonel;
            var sonuc = response.IdariPersonelResponse.Sonuc;

            AdminStaffDTO entItem = new AdminStaffDTO();

            if (resultItem == null)
            {
                int returnCode = response.IdariPersonelResponse.Sonuc.ReturnCode;
                string returnMessage = response.IdariPersonelResponse.Sonuc.ReturnMessage;

                return new()
                {
                    Result = false,
                    Message =
                        returnCode.ToString() + " Message : " + response.IdariPersonelResponse.Sonuc.ReturnMessage,
                    Item = entItem
                };
            }
            else
            {
                entItem.TckNo = resultItem.TcKimlikNo;
                entItem.Ad = resultItem.Ad;
                entItem.BirimAd = resultItem.BirimAd;
                entItem.BirimId = resultItem.BirimId;
                entItem.GorevBirimAd = resultItem.GorevBirimAd;
                entItem.GorevBirimId = resultItem.GorevBirimId;
                entItem.GorevBirimTur = resultItem.GorevBirimTur;
                entItem.KadroAd = resultItem.KadroAd;
                entItem.KadroBirimAd = resultItem.KadroBirimAd;
                entItem.KadroBirimId = resultItem.KadroBirimId;
                entItem.KadroBirimTur = resultItem.KadroBirimTur;
                entItem.KadroId = resultItem.KadroId;
                entItem.Soyad = resultItem.Soyad;

                return new() { Result = true, Item = entItem };
            }
        }

        #endregion

        #region EgitimBilgisiSorgula

        private YuksekOgretimEgitimBilgisiPortClient GetClientEgitimBilgisi()
        {
            var url = appSettingsModel.YOK.EgitimBilgisiUrl;

            var usr = appSettingsModel.YOK.UserName;
            var pass = appSettingsModel.YOK.Password;

            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 524288;

            EndpointAddress address = new EndpointAddress(url);

            YuksekOgretimEgitimBilgisiPortClient client = new YuksekOgretimEgitimBilgisiPortClient(binding, address);
            client.ClientCredentials.UserName.UserName = usr;
            client.ClientCredentials.UserName.Password = pass;

            return client;
        }

        public async Task<List<GraduationDetailResponseDTO>> GetEgitimBilgisiAsync(long tc)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - {tc} GetEgitimBilgisiAsync.";
            logger.LogInformation(log);
            try
            {
                await dbContext.Logs.AddAsync(new() { LogType = LogType.YOK, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetEgitimBilgisiAsync SaveChanges");
            }

            YuksekOgretimEgitimBilgisiPortClient client = GetClientEgitimBilgisi();

            var request = new EgitimBilgisiMezunRequestType();
            request.TcKimlikNo = tc;

            var response = await client.EgitimBilgisiMezunAsync(request);

            var resultList = response.EgitimBilgisiMezunResponse;
            var sonuc = response.EgitimBilgisiMezunResponse.Sonuc;

            var graduationDetails = new List<GraduationDetailResponseDTO>();

            if (resultList.EgitimBilgisiMezunKayit?.Length > 0)
                foreach (var egitimBilgisi in resultList.EgitimBilgisiMezunKayit)
                    if (egitimBilgisi.MezunBilgi.AyrilmaNedeni.Kod == 9 && egitimBilgisi.BirimBilgi?.BirimGrubu?.Kod != 5370) // mezuniyet tıp ise aşağıda ckys'den çekilecek
                        graduationDetails.Add(new GraduationDetailResponseDTO()
                        {
                            GraduationDate = egitimBilgisi.MezunBilgi?.AyrilmaTarihi,
                            GraduationFaculty = egitimBilgisi.BirimBilgi?.FakulteYoMyoEnstitu?.Ad,
                            GraduationField = egitimBilgisi.BirimBilgi?.BirimAdi,
                            HigherEducationDetail = egitimBilgisi.BirimBilgi?.BirimTuru?.Ad,
                            GraduationUniversity = egitimBilgisi.BirimBilgi?.Universite?.Ad,
                            IsPhd = egitimBilgisi.BirimBilgi?.BirimTuru?.Kod == 17,
                        });

            var ckysResponse = await cKYSService.GetMedicineInfo(tc.ToString());
            if (ckysResponse.Result)
                graduationDetails.Add(ckysResponse.Item);

            return graduationDetails;
        }

        #endregion
    }
}