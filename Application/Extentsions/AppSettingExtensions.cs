using Application.Models;
using Core.Models.ConfigModels;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extentsions
{
    public static class AppSettingExtensions
    {
        public static AppSettingsModel GetSettingModel(this AppSettings setting)
        {
            EmailConfiguration EmailConfigurationModel = new()
            {
                From = setting.EmailConfiguration["From"],
                Password = setting.EmailConfiguration["Password"],
                Port = Convert.ToInt32(setting.EmailConfiguration["Port"]),
                SmtpServer = setting.EmailConfiguration["SmtpServer"],
                UserName = setting.EmailConfiguration["Username"]
            };
            TokenOptions TokenOptionsModel = new()
            {
                AccessExpiration = Convert.ToInt32(setting.TokenOptions["accessExpiration"]),
                Audience = setting.TokenOptions["audience"],
                Issuer = setting.TokenOptions["issuer"],
                RefreshExpiration = Convert.ToInt32(setting.TokenOptions["refreshExpiration"]),
                Secret = setting.TokenOptions["secret"]
            };

            SSOSettings SSOModel = new()
            {
                ClientId = setting.SSO["ClientId"],
                RedirectUri = setting.SSO["RedirectUri"],
                LogoutUri = setting.SSO["LogoutUri"],
                Scope = setting.SSO["Scope"],
                ClientSecret = setting.SSO["ClientSecret"],
                OpenIdServer = setting.SSO["OpenIdServer"]
            };
            CKYSSettings CKYSModel = new()
            {
                UserName = setting.CKYS["UserName"],
                Password = setting.CKYS["Password"]
            };
            KPSSettings KPSModel = new()
            {
                KPSEndPointAdress = setting.KPS["KPSEndPointAdress"],
                KPSSTSEndpoint = setting.KPS["KPSSTSEndpoint"],
                KpsUserName = setting.KPS["KpsUserName"],
                KpsPassword = setting.KPS["KpsPassword"]
            };
            YOKSettings YOKModel = new()
            {
                UserName = setting.YOK["UserName"],
                Password = setting.YOK["Password"],
                EgitimBilgisiUrl = setting.YOK["EgitimBilgisiUrl"],
                MezunUrl = setting.YOK["MezunUrl"],
                SaglikMezunUrl = setting.YOK["SaglikMezunUrl"],
                AkademikIdariUrl = setting.YOK["AkademikIdariUrl"],
                AkademisyenGorevlendirmeUrl = setting.YOK["AkademisyenGorevlendirmeUrl"],
                AkademisyenUzmanlikUrl = setting.YOK["AkademisyenUzmanlikUrl"],
                IdariPersonelUrl = setting.YOK["IdariPersonelUrl"]
            };
            S3Settings S3Model = new()
            {
                BucketName = setting.S3["BucketName"],
                ServiceURL = setting.S3["ServiceURL"],
                AccessKey = setting.S3["AccessKey"],
                SecretKey = setting.S3["SecretKey"],
                SSL = setting.S3["SSL"] == "true",
                SignatureVersion = setting.S3["SignatureVersion"],
                ForcePathStyle = setting.S3["ForcePathStyle"] == "true"
            };
            SMSSettings SMSModel = new()
            {
                Username = setting.SMS["Username"],
                Password = setting.SMS["Password"],
                TokenUrl = setting.SMS["TokenUrl"],
                SMSUrl = setting.SMS["SMSUrl"],
                BulkSMSUrl = setting.SMS["BulkSMSUrl"],
            };
            EnvironmentSettings EnvironmentModel = new()
            {
                Domain = setting.ENVIRONMENT["Domain"],
                URL = setting.ENVIRONMENT["URL"],
            };
            return new()
            {
                CKYS = CKYSModel,
                KPS = KPSModel,
                S3 = S3Model,
                SSO = SSOModel,
                YOK = YOKModel,
                EmailConfiguration = EmailConfigurationModel,
                TokenOptions = TokenOptionsModel,
                SMS = SMSModel,
                Environment = EnvironmentModel
            };
        }
    }
}
