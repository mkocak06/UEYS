using Core.Extentsions;
using Core.Interfaces;
using Core.Models;
using Core.Models.ConfigModels;
using Infrastructure.Services.Kps;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Xml;
using Infrastructure.Data;
using Shared.Types;

namespace Infrastructure.Services
{
    public class KPSService : IKPSService
    {
        private readonly KPSRequestHelper kpsHelper;
        private readonly ILogger<KPSService> logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ApplicationDbContext dbContext;

        public KPSService(AppSettingsModel appSettingsModel, ILogger<KPSService> logger, IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            kpsHelper = new KPSRequestHelper(
                appSettingsModel.KPS.KPSEndPointAdress,
                appSettingsModel.KPS.KPSSTSEndpoint,
                appSettingsModel.KPS.KpsUserName,
                appSettingsModel.KPS.KpsPassword);

            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public KPSResult GetKPSResult(long tckn)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            var log = $"{userId} - {tckn} GetKPSResult.";
            logger.LogInformation(log);
            try
            {
                dbContext.Logs.Add(new() { LogType = LogType.KPS, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetKPSResult SaveChanges");
            }
            try
            {
                var bilesikKisiSorgula = kpsHelper.BilesikKisiSorgula(tckn);

                var citizen = new KPSResult();

                XmlNode bilesikKisiSorgulaResult = bilesikKisiSorgula.GetElementsByTagName("BilesikKisiSorgulaResult").Item(0);

                XmlNode kisiBilgisi;

                if (bilesikKisiSorgulaResult["Sonuc"]["MaviKartliKisiKutukleri"].InnerXml != String.Empty)
                {
                    kisiBilgisi = bilesikKisiSorgulaResult["Sonuc"]["MaviKartliKisiKutukleri"]["KisiBilgisi"];
                    citizen.TCKN = Convert.ToInt64(kisiBilgisi["KimlikNo"].InnerText);
                    citizen.Name = kisiBilgisi["TemelBilgisi"]["Ad"].InnerText;
                    citizen.Surname = kisiBilgisi["TemelBilgisi"]["Soyad"].InnerText;
                    citizen.Gender = Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["Cinsiyet"]["Kod"].InnerText);
                    citizen.BirthPlace = kisiBilgisi["TemelBilgisi"]["DogumYer"].InnerText;

                    citizen.BirthDate = new DateTime(Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Yil"].InnerText),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) : 01),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) : 01));

                    citizen.DeathDay = !String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"].InnerText) ?
                        new DateTime(Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"].InnerText),
                            (!String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"].InnerText) ?
                                Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"].InnerText) : 01),
                            (!String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"].InnerText) ?
                                Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"].InnerText) : 01)) : (DateTime?)null;
                }
                else if (bilesikKisiSorgulaResult["Sonuc"]["TCVatandasiKisiKutukleri"].InnerXml != String.Empty)
                {
                    kisiBilgisi = bilesikKisiSorgulaResult["Sonuc"]["TCVatandasiKisiKutukleri"]["KisiBilgisi"];
                    citizen.TCKN = Convert.ToInt64(kisiBilgisi["TCKimlikNo"].InnerText);
                    citizen.Name = kisiBilgisi["TemelBilgisi"]["Ad"].InnerText;
                    citizen.Surname = kisiBilgisi["TemelBilgisi"]["Soyad"].InnerText;
                    citizen.Gender = Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["Cinsiyet"]["Kod"].InnerText);
                    citizen.BirthPlace = kisiBilgisi["TemelBilgisi"]["DogumYer"].InnerText;

                    citizen.BirthDate = new DateTime(Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Yil"].InnerText),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) : 01),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) : 01));

                    citizen.DeathDay = !String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"].InnerText) ?
                        new DateTime(Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"].InnerText),
                            (!String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"].InnerText) ?
                                Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"].InnerText) : 01),
                            (!String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"].InnerText) ?
                                Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"].InnerText) : 01)) : (DateTime?)null;
                }
                else if (bilesikKisiSorgulaResult["Sonuc"]["YabanciKisiKutukleri"].InnerXml != String.Empty)
                {
                    kisiBilgisi = bilesikKisiSorgulaResult["Sonuc"]["YabanciKisiKutukleri"]["KisiBilgisi"];
                    citizen.TCKN = Convert.ToInt64(kisiBilgisi["KimlikNo"].InnerText);
                    citizen.Name = kisiBilgisi["TemelBilgisi"]["Ad"].InnerText;
                    citizen.Surname = kisiBilgisi["TemelBilgisi"]["Soyad"].InnerText;
                    citizen.Gender = Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["Cinsiyet"]["Kod"].InnerText);
                    citizen.BirthPlace = kisiBilgisi["TemelBilgisi"]["DogumYer"].InnerText;

                    citizen.BirthDate = new DateTime(Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Yil"].InnerText),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) : 01),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) : 01));

                    citizen.DeathDay = kisiBilgisi["DurumBilgisi"]["OlumTarih"] != null ?
                        new DateTime(Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"].InnerText),
                            Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"].InnerText),
                            Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"].InnerText)) : (DateTime?)null;
                }
                else
                {
                    return null;
                }

                return citizen;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error GetKPSResult.");
                throw;
            }

        }
        public KPSResult GetKPSResultWithAddress(long tckn)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var userIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = $"{userId} - {tckn} GetKPSResultWithAddress.";
            logger.LogInformation(log);
            try
            {
                dbContext.Logs.Add(new() { LogType = LogType.KPS, Message = log, UserId = userId, UserIPAddress = userIPAddress });
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error GetKPSResultWithAddress SaveChanges");
            }
            try
            {
                var bilesikKisiveAdresSorgula = kpsHelper.BilesikKisiveAdresSorgula(tckn);

                var citizen = new KPSResult();

                XmlNode bilesikKisiveAdresSorgulaResult = bilesikKisiveAdresSorgula.GetElementsByTagName("BilesikKisiveAdresSorgulaResult").Item(0);

                XmlNode kisiBilgisi;
                XmlNode maviKartBilgisi;

                if (bilesikKisiveAdresSorgulaResult["Sonuc"]["MaviKartliKisiKutukleri"]?.InnerXml != String.Empty)
                {
                    kisiBilgisi = bilesikKisiveAdresSorgulaResult["Sonuc"]?["MaviKartliKisiKutukleri"]?["KisiBilgisi"];
                    if (kisiBilgisi == null)
                        return null;
                    citizen.TCKN = Convert.ToInt64(kisiBilgisi["KimlikNo"]?.InnerText);
                    citizen.Name = kisiBilgisi["TemelBilgisi"]["Ad"]?.InnerText;
                    citizen.Surname = kisiBilgisi["TemelBilgisi"]["Soyad"]?.InnerText;
                    citizen.Gender = Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["Cinsiyet"]["Kod"]?.InnerText);
                    citizen.BirthPlace = kisiBilgisi["TemelBilgisi"]["DogumYer"]?.InnerText;
                    citizen.MotherName = kisiBilgisi["TemelBilgisi"]["AnneAd"]?.InnerText;
                    citizen.FatherName = kisiBilgisi["TemelBilgisi"]["BabaAd"]?.InnerText;


                    maviKartBilgisi = bilesikKisiveAdresSorgulaResult["Sonuc"]["MaviKartliKisiKutukleri"]["MaviKartBilgisi"];
                    //citizen.Nationality = kisiBilgisi["Uyruk"]["Aciklama"].InnerText;

                    citizen.BirthDate = new DateTime(Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Yil"]?.InnerText),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"]?.InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"]?.InnerText) : 01),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"]?.InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"]?.InnerText) : 01));

                    citizen.DeathDay = !String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"]?.InnerText) ?
                        new DateTime(Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"]?.InnerText),
                            (!String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"]?.InnerText) ?
                                Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"]?.InnerText) : 01),
                            (!String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"]?.InnerText) ?
                                Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"]?.InnerText) : 01)) : (DateTime?)null;
                }
                else if (bilesikKisiveAdresSorgulaResult["Sonuc"]["TCVatandasiKisiKutukleri"].InnerXml != String.Empty)
                {
                    kisiBilgisi = bilesikKisiveAdresSorgulaResult["Sonuc"]["TCVatandasiKisiKutukleri"]["KisiBilgisi"];
                    citizen.TCKN = Convert.ToInt64(kisiBilgisi["TCKimlikNo"].InnerText);
                    citizen.Name = kisiBilgisi["TemelBilgisi"]["Ad"].InnerText;
                    citizen.Surname = kisiBilgisi["TemelBilgisi"]["Soyad"].InnerText;
                    citizen.Gender = Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["Cinsiyet"]["Kod"].InnerText);
                    citizen.BirthPlace = kisiBilgisi["TemelBilgisi"]["DogumYer"].InnerText;
                    citizen.MotherName = kisiBilgisi["TemelBilgisi"]["AnneAd"].InnerText;
                    citizen.FatherName = kisiBilgisi["TemelBilgisi"]["BabaAd"].InnerText;

                    citizen.BirthDate = new DateTime(Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Yil"].InnerText),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) : 01),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) : 01));

                    citizen.DeathDay = !String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"].InnerText) ?
                        new DateTime(Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"].InnerText),
                            (!String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"].InnerText) ?
                                Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"].InnerText) : 01),
                            (!String.IsNullOrWhiteSpace(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"].InnerText) ?
                                Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"].InnerText) : 01)) : (DateTime?)null;
                }
                else if (bilesikKisiveAdresSorgulaResult["Sonuc"]["YabanciKisiKutukleri"].InnerXml != String.Empty)
                {
                    kisiBilgisi = bilesikKisiveAdresSorgulaResult["Sonuc"]["YabanciKisiKutukleri"]["KisiBilgisi"];
                    citizen.TCKN = Convert.ToInt64(kisiBilgisi["KimlikNo"].InnerText);
                    citizen.Name = kisiBilgisi["TemelBilgisi"]["Ad"].InnerText;
                    citizen.Surname = kisiBilgisi["TemelBilgisi"]["Soyad"].InnerText;
                    citizen.Gender = Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["Cinsiyet"]["Kod"].InnerText);
                    citizen.BirthPlace = kisiBilgisi["TemelBilgisi"]["DogumYer"].InnerText;
                    citizen.MotherName = kisiBilgisi["TemelBilgisi"]["AnneAd"].InnerText;
                    citizen.FatherName = kisiBilgisi["TemelBilgisi"]["BabaAd"].InnerText;
                    //citizen.Nationality = kisiBilgisi["Uyruk"]["Aciklama"].InnerText;

                    citizen.BirthDate = new DateTime(Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Yil"].InnerText),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Ay"].InnerText) : 01),
                        (!String.IsNullOrWhiteSpace(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) ?
                            Convert.ToInt32(kisiBilgisi["TemelBilgisi"]["DogumTarih"]["Gun"].InnerText) : 01));

                    citizen.DeathDay = kisiBilgisi["DurumBilgisi"]["OlumTarih"] != null ?
                        new DateTime(Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Yil"].InnerText),
                            Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Ay"].InnerText),
                            Convert.ToInt32(kisiBilgisi["DurumBilgisi"]["OlumTarih"]["Gun"].InnerText)) : (DateTime?)null;
                }
                else
                {
                    return null;
                }
                if (bilesikKisiveAdresSorgulaResult["Sonuc"]["AdresBilgisi"].InnerXml != String.Empty)
                {
                    XmlNode adresBilgisi = bilesikKisiveAdresSorgulaResult["Sonuc"]["AdresBilgisi"];
                    citizen.AddressInfo = new();
                    citizen.AddressInfo.Address = adresBilgisi["AcikAdres"].InnerText;
                    if (adresBilgisi["BeldeAdresi"].InnerXml != string.Empty)
                    {
                        if (!String.IsNullOrWhiteSpace(adresBilgisi["BeldeAdresi"]["IlKodu"].InnerText))
                            citizen.AddressInfo.ProvinceCode = Convert.ToInt32(adresBilgisi["BeldeAdresi"]["IlKodu"].InnerText);
                        if (!String.IsNullOrWhiteSpace(adresBilgisi["BeldeAdresi"]["IlceKodu"].InnerText))
                            citizen.AddressInfo.ProvinceCode = Convert.ToInt32(adresBilgisi["BeldeAdresi"]["IlceKodu"].InnerText);

                        citizen.AddressInfo.ProvinceName = adresBilgisi["BeldeAdresi"]["Il"].InnerText;
                        citizen.AddressInfo.DistrictName = adresBilgisi["BeldeAdresi"]["Ilce"].InnerText;

                    }
                    else if (adresBilgisi["IlIlceMerkezAdresi"].InnerXml != string.Empty)
                    {
                        if (!String.IsNullOrWhiteSpace(adresBilgisi["IlIlceMerkezAdresi"]["IlKodu"].InnerText))
                            citizen.AddressInfo.ProvinceCode = Convert.ToInt32(adresBilgisi["IlIlceMerkezAdresi"]["IlKodu"].InnerText);
                        if (!String.IsNullOrWhiteSpace(adresBilgisi["IlIlceMerkezAdresi"]["IlceKodu"].InnerText))
                            citizen.AddressInfo.DistrictCode = Convert.ToInt32(adresBilgisi["IlIlceMerkezAdresi"]["IlceKodu"].InnerText);

                        citizen.AddressInfo.ProvinceName = adresBilgisi["IlIlceMerkezAdresi"]["Il"].InnerText;
                        citizen.AddressInfo.DistrictName = adresBilgisi["IlIlceMerkezAdresi"]["Ilce"].InnerText;
                    }
                    else if (adresBilgisi["KoyAdresi"].InnerXml != string.Empty)
                    {
                        if (!String.IsNullOrWhiteSpace(adresBilgisi["KoyAdresi"]["IlKodu"].InnerText))
                            citizen.AddressInfo.ProvinceCode = Convert.ToInt32(adresBilgisi["KoyAdresi"]["IlKodu"].InnerText);
                        if (!String.IsNullOrWhiteSpace(adresBilgisi["KoyAdresi"]["IlceKodu"].InnerText))
                            citizen.AddressInfo.DistrictCode = Convert.ToInt32(adresBilgisi["KoyAdresi"]["IlceKodu"].InnerText);

                        citizen.AddressInfo.ProvinceName = adresBilgisi["KoyAdresi"]["Il"].InnerText;
                        citizen.AddressInfo.DistrictName = adresBilgisi["KoyAdresi"]["Ilce"].InnerText;
                    }
                    else if (adresBilgisi["YurtDisiAdresi"].InnerXml != string.Empty)
                    {
                        citizen.AddressInfo.Address = adresBilgisi["YurtDisiAdresi"]["YabanciAdres"].InnerText;
                        citizen.AddressInfo.ProvinceName = adresBilgisi["YurtDisiAdresi"]["YabanciSehir"].InnerText;
                    }
                    else
                    {
                        citizen.AddressInfo = null;
                    }
                }
                return citizen;
            }
            catch (Exception ex)
            {
                dbContext.Logs.Add(new Core.Entities.Log() { Message = ex.ToString() });
                dbContext.SaveChanges();
                logger.LogError(ex, "Error GetKPSResultWithAddress.");
                throw ex;
            }

        }
    }
}
