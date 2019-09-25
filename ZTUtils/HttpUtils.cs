using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ZTUtils
{
    public class HttpUtils
    {
        public static string HttpGet(string url)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 100000;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                {
                    var resposeResult = streamReader.ReadToEnd();
                    return resposeResult;
                }
            }
            catch (Exception ex)
            {
                //LogHelper.WriteTextLog(ex);
                return "";
            }
        }
        public static string HttpPost(string url, string param)
        {
            X509Certificate2 cerCaiShang = new X509Certificate2();
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => { return true; /*总是接受*/});

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            //httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 100000;
            var encoding = new UTF8Encoding();
            var b = encoding.GetBytes(param);
            using (var reqStream = httpWebRequest.GetRequestStream())
            {
                reqStream.Write(b, 0, b.Length);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
