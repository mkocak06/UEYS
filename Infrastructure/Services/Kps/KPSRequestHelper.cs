using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Infrastructure.Services.Kps
{
    public class KPSRequestHelper
    {
        private DateTime tokenExpires = DateTime.Now.AddMinutes(-1);

        private readonly string _kpsUrl;
        private readonly string _stsUrl;
        private readonly string _kpsUsername;
        private readonly string _kpsPassword;
        private string _token = string.Empty;

        public KPSRequestHelper(string kpsUrl, string stsUrl, string username, string password)
        {
            _kpsUrl = kpsUrl;
            _stsUrl = stsUrl;
            _kpsUsername = username;
            _kpsPassword = password;
        }

        public XmlDocument BilesikKisiSorgula(long tckn)
        {
            _token = GetSTSToken(tckn);
            var now = DateTime.Now;
            var created = now.AddMinutes(-1).ToUniversalTime().ToString("o");
            var expires = now.AddMinutes(5).ToUniversalTime().ToString("o");

            var xml = new StringBuilder();
            xml.Append("<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">");

            xml.Append("<s:Header>");
            xml.Append("<a:Action s:mustUnderstand=\"1\">https://www.saglik.gov.tr/KPS/01/01/2017/IKpsServices/BilesikKisiSorgula</a:Action>");
            xml.Append("<a:MessageID>urn:uuid:" + Guid.NewGuid() + "</a:MessageID>");
            xml.Append("<a:ReplyTo>");
            xml.Append("<a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>");
            xml.Append("</a:ReplyTo>");

            xml.Append("<a:To s:mustUnderstand=\"1\">" + _kpsUrl + "</a:To>");
            xml.Append("<o:Security s:mustUnderstand=\"1\" xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
            xml.Append("<u:Timestamp u:Id=\"_0\">");
            xml.Append("<u:Created>" + created + "</u:Created>");
            xml.Append("<u:Expires>" + expires + "</u:Expires>");
            xml.Append("</u:Timestamp>");

            // token
            xml.Append(_token);

            xml.Append("</o:Security>");
            xml.Append("</s:Header>");

            xml.Append("<s:Body>");
            xml.Append("<BilesikKisiSorgula xmlns=\"https://www.saglik.gov.tr/KPS/01/01/2017\">");
            xml.Append("<kimlikNo>" + tckn + "</kimlikNo>");
            xml.Append("</BilesikKisiSorgula>");
            xml.Append("</s:Body>");
            xml.Append("</s:Envelope>");

            return SendSoapRequest(xml.ToString(), _kpsUrl);
        }
        public XmlDocument BilesikKisiveAdresSorgula(long tckn)
        {
            _token = GetSTSToken(tckn);
            var now = DateTime.Now;
            var created = now.AddMinutes(-1).ToUniversalTime().ToString("o");
            var expires = now.AddMinutes(5).ToUniversalTime().ToString("o");

            var xml = new StringBuilder();
            xml.Append("<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">");

            xml.Append("<s:Header>");
            xml.Append("<a:Action s:mustUnderstand=\"1\">https://www.saglik.gov.tr/KPS/01/01/2017/IKpsServices/BilesikKisiveAdresSorgula</a:Action>");
            xml.Append("<a:MessageID>urn:uuid:" + Guid.NewGuid() + "</a:MessageID>");
            xml.Append("<a:ReplyTo>");
            xml.Append("<a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>");
            xml.Append("</a:ReplyTo>");

            xml.Append("<a:To s:mustUnderstand=\"1\">" + _kpsUrl + "</a:To>");
            xml.Append("<o:Security s:mustUnderstand=\"1\" xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
            xml.Append("<u:Timestamp u:Id=\"_0\">");
            xml.Append("<u:Created>" + created + "</u:Created>");
            xml.Append("<u:Expires>" + expires + "</u:Expires>");
            xml.Append("</u:Timestamp>");

            // token
            xml.Append(_token);

            xml.Append("</o:Security>");
            xml.Append("</s:Header>");

            xml.Append("<s:Body>");
            xml.Append("<BilesikKisiveAdresSorgula xmlns=\"https://www.saglik.gov.tr/KPS/01/01/2017\">");
            xml.Append("<kimlikNo>" + tckn + "</kimlikNo>");
            xml.Append("</BilesikKisiveAdresSorgula>");
            xml.Append("</s:Body>");
            xml.Append("</s:Envelope>");

            return SendSoapRequest(xml.ToString(), _kpsUrl);
        }

        private string GetSTSToken(long sorgulayanKimlikNo)
        {
            //if old token is not expired use old token
            if (DateTime.Now < tokenExpires)
            {
                return _token;
            }

            _token = string.Empty;
            var now = DateTime.Now;
            var created = now.AddMinutes(-1).ToUniversalTime().ToString("o");
            var expires = now.AddMinutes(5).ToUniversalTime().ToString("o");

            var envelope = new StringBuilder();
            envelope.Append("<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">");
            envelope.Append("<s:Header>");
            envelope.Append("<a:Action s:mustUnderstand=\"1\">http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue</a:Action>");
            envelope.Append("<a:MessageID>urn:uuid:" + Guid.NewGuid() + "</a:MessageID>");
            envelope.Append("<a:ReplyTo>");
            envelope.Append("<a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>");
            envelope.Append("</a:ReplyTo>");
            envelope.Append("<a:To s:mustUnderstand=\"1\">" + _stsUrl + "</a:To>");

            envelope.Append("<SorgulayanKimlikNo a:IsReferenceParameter=\"true\" xmlns=\"\">" + sorgulayanKimlikNo + "</SorgulayanKimlikNo>");

            envelope.Append("<o:Security s:mustUnderstand=\"1\" xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
            envelope.Append("<u:Timestamp u:Id=\"_0\">");
            envelope.Append("<u:Created>" + created + "</u:Created>");
            envelope.Append("<u:Expires>" + expires + "</u:Expires>");
            envelope.Append("</u:Timestamp>");
            envelope.Append("<o:UsernameToken u:Id=\"uuid-" + Guid.NewGuid() + "-1\">");

            //username,password
            envelope.Append("<o:Username>" + _kpsUsername + "</o:Username>");
            envelope.Append("<o:Password>" + _kpsPassword + "</o:Password>");

            envelope.Append("</o:UsernameToken>");
            envelope.Append("</o:Security>");
            envelope.Append("</s:Header>");
            envelope.Append("<s:Body>");

            envelope.Append("<trust:RequestSecurityToken xmlns:trust=\"http://docs.oasis-open.org/ws-sx/ws-trust/200512\">");
            envelope.Append("<wsp:AppliesTo xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\">");
            envelope.Append("<a:EndpointReference>");
            envelope.Append("<a:Address>" + _kpsUrl + "</a:Address>");
            envelope.Append("</a:EndpointReference>");
            envelope.Append("</wsp:AppliesTo>");
            envelope.Append("<trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType>");
            envelope.Append("<trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType>");
            envelope.Append("</trust:RequestSecurityToken>");
            envelope.Append("</s:Body>");
            envelope.Append("</s:Envelope>");

            var envopleString = envelope.ToString();
            var x = SendSoapRequest(envopleString, _stsUrl);
            tokenExpires = Convert.ToDateTime(x.GetElementsByTagName("saml:Conditions")[0].Attributes["NotOnOrAfter"].Value);
            var securityToken = x.GetElementsByTagName("trust:RequestedSecurityToken")[0];

            return securityToken.InnerXml.ToString();
        }

        private XmlDocument SendSoapRequest(string envelopeString, string url)
        {
            var result = string.Empty;
            var xDoc = new XmlDocument();
            var request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                var encoding = new ASCIIEncoding();
                var bytesToWrite = encoding.GetBytes(envelopeString);
                request.Method = "POST";
                request.ContentLength = bytesToWrite.Length;
                request.ContentType = "application/soap+xml; charset=UTF-8";

                using (var newStream = request.GetRequestStream())
                {
                    newStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var dataStream = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(dataStream))
                            {
                                xDoc.Load(reader);
                            }
                        }
                    }
                }

                return xDoc;
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    if (ex.Response.ContentLength != 0)
                    {
                        using (var stream = ex.Response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                result = reader.ReadToEnd();
                                throw new Exception(result);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public XmlDocument AileBireyleriSorgula(long tckn)
        {
            var token = GetSTSToken(tckn);
            var now = DateTime.Now;
            var created = now.AddMinutes(-1).ToUniversalTime().ToString("o");
            var expires = now.AddMinutes(5).ToUniversalTime().ToString("o");

            var xml = new StringBuilder();
            xml.Append("<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">");

            xml.Append("<s:Header>");
            xml.Append("<a:Action s:mustUnderstand=\"1\">https://www.saglik.gov.tr/KPS/01/01/2017/IKpsServices/AileBireyleriSorgula</a:Action>");
            xml.Append("<a:MessageID>urn:uuid:" + Guid.NewGuid() + "</a:MessageID>");
            xml.Append("<a:ReplyTo>");
            xml.Append("<a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>");
            xml.Append("</a:ReplyTo>");

            xml.Append("<a:To s:mustUnderstand=\"1\">" + _kpsUrl + "</a:To>");
            xml.Append("<o:Security s:mustUnderstand=\"1\" xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
            xml.Append("<u:Timestamp u:Id=\"_0\">");
            xml.Append("<u:Created>" + created + "</u:Created>");
            xml.Append("<u:Expires>" + expires + "</u:Expires>");
            xml.Append("</u:Timestamp>");

            // token
            xml.Append(token);

            xml.Append("</o:Security>");
            xml.Append("</s:Header>");

            xml.Append("<s:Body>");

            xml.Append("<AileBireyleriSorgula xmlns=\"https://www.saglik.gov.tr/KPS/01/01/2017\">");
            xml.Append("<kriter>");
            xml.Append("<TCKimlikNo>" + tckn + "</TCKimlikNo>");
            xml.Append("<Tip>TumAile</Tip>");
            xml.Append("</kriter>");
            xml.Append("</AileBireyleriSorgula>");

            xml.Append("</s:Body>");
            xml.Append("</s:Envelope>");

            return SendSoapRequest(xml.ToString(), _kpsUrl);
        }
    }

}
