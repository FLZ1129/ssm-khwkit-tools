using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CrazySharp.Std
{
    public class BaseHttpResponse
    {
        [JsonIgnore]
        public int HttpStatusCode { get; set; } = 200;
        [JsonIgnore]
        public string HttpResponse { get; set; }
        [JsonIgnore]
        protected bool IsSuccessStatusCode => HttpStatusCode >= 200 && HttpStatusCode <= 209;
    }
    /// <summary>
    /// </summary>
    public static class HttpUtils
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static readonly WebRequestHandler GWebHandler = new WebRequestHandler()
        {
            //设置不使用代理
            UseProxy = false,
            AllowAutoRedirect = true,
            ReadWriteTimeout = 30 * 1000,
            ClientCertificateOptions = ClientCertificateOption.Manual,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        private static readonly HttpClient GlobalClient = new HttpClient(GWebHandler)
        {
            Timeout = TimeSpan.FromSeconds(30),
        };

        static HttpUtils()
        {
            InitSsl();
        }

        public static bool TrySetUpClientCert(X509Certificate2 certificate, out string message)
        {
            message = "";
            try
            {
                if (certificate.NotBefore > DateTime.Now || certificate.NotAfter < DateTime.Now)
                {
                    message = "授权文件已失效";
                    return false;
                }

                return SetUpClientCert(certificate);
            }
            catch (Exception ex)
            {
                logger.Warn($"SetUpClientCert Error : {ex.Message}");
                message = ex.Message;
                return false;
            }
        }

        public static bool SetUpClientCert(string fileName)
        {
            return SetUpClientCert(new X509Certificate2(fileName));
        }

        public static bool SetUpClientCert(string fileName, SecureString password)
        {
            return SetUpClientCert(new X509Certificate2(fileName, password));
        }

        public static bool SetUpClientCert(byte[] rawData)
        {
            return SetUpClientCert(new X509Certificate2(rawData));
        }

        public static bool SetUpClientCert(byte[] rawData, SecureString password)
        {
            return SetUpClientCert(new X509Certificate2(rawData, password));
        }

        public static bool SetUpClientCert(X509Certificate2 certificate)
        {
            GWebHandler.ClientCertificates.Clear();
            GWebHandler.ClientCertificates.Add(certificate);
            return true;
        }

        public static bool LicenseValidate()
        {
            if (GWebHandler.ClientCertificates.Count == 0)
            {
                return false;
            }

            var cert = (X509Certificate2) GWebHandler.ClientCertificates[0];
            return cert.NotAfter >= DateTime.Now && cert.NotBefore <= DateTime.Now;
        }

        private static void InitSsl()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, errors) =>
                {
                    //因主机自签证书存在主机名不匹配的问题，这个地方只能无脑信任
                    //foreach (X509ChainStatus status in chain.ChainStatus)
                    //{
                    //    if (status.Status != X509ChainStatusFlags.NoError)
                    //    {
                    //        logger.Error($"X509链状态:{status.StatusInformation}");
                    //        return false;
                    //    }
                    //}
                    //if (errors != SslPolicyErrors.None)
                    //{
                    //    logger.Error($"SSL策略问题 : {errors.ToString()}");
                    //    return false;
                    //}
                    return true;
                };
        }
        public static HttpContent ToHttpStringContent(this string bodyData,string contentType)
        {
            var stringContent = new StringContent(bodyData, Encoding.UTF8);
            stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            return stringContent;
        }
        public static HttpContent ToHttpJsonStringContent(this string bodyData)
        {
            return ToHttpStringContent(bodyData, "application/json");
        }

        private static async Task<string> AsyncGetHttpResponseString(this Task<HttpResponseMessage> task)
        {
            using (var rsp = await task.ConfigureAwait(false))
            {
                string resString = await rsp.Content.ReadAsStringAsync();
                if (rsp.StatusCode<=0)
                {
                    var statusCode = rsp.StatusCode;
                    resString = new JObject()
                    {
                        {"HttpStatusCode", (int) statusCode},
                        {"HttpResponse", resString}
                    }.ToString(Formatting.None);
                }
                logger.Debug($"Http Response >>\r\n{resString}");
                return resString;
            }
        }

        private static async Task<string> SafeAsyncCall(this Task<string> t)
        {
            string ret = "";
            try
            {
                ret = await t;
            }
            catch (Exception e)
            {
                logger.Warn(e);
            }
            finally
            {
                if (string.IsNullOrEmpty(ret))
                {
                    ret = new JObject()
                    {
                        {"HttpStatusCode", -1},
                        {"HttpResponse", "请求异常,无法连接到远程服务器"}
                    }.ToString(Formatting.None);
                }
            }

            return ret;
        }


        // ReSharper disable once UnusedMember.Local
        public static async Task<string> SendAsync(this HttpRequestMessage request)
        {
            try
            {
                if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Delete)
                {
                    logger.Debug($"Http {request.Method} >>{request.RequestUri} ");
                }
                else
                {
                    var body = await request.Content.ReadAsStringAsync();
                    logger.Debug($"Http {request.Method} >>{request.RequestUri} \r\n{body}");
                }
            }
            catch (Exception e)
            {
                logger.Warn($"http log error:{e.Message}");
            }
            return await GlobalClient.SendAsync(request).AsyncGetHttpResponseString().SafeAsyncCall();
            //return Task.Run(async () =>
            //{
            //    //异步发送并等待结果
            //    string retString = String.Empty;
            //    string errMessage = string.Empty;
            //    HttpResponseMessage rsp = null;
            //    HttpStatusCode httpStatusCode = HttpStatusCode.GatewayTimeout;
            //    try
            //    {
                    
            //        httpStatusCode = rsp.StatusCode;
            //        var resStr = await rsp.Content.ReadAsStringAsync();
            //        if (rsp.IsSuccessStatusCode)
            //        {
            //            retString = resStr;
            //        }
            //        else
            //        {
            //            errMessage = retString;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.Debug($"HttpUtils SendAsync Exception :{ex.Message} {ex.InnerException?.Message}");
            //        errMessage = ex.Message;
            //    }
            //    finally
            //    {
            //        if (string.IsNullOrWhiteSpace(retString))
            //        {
            //            retString = new JObject()
            //            {
            //                {"HttpStatusCode", (int) httpStatusCode},
            //                {"HttpResponse", errMessage}
            //            }.ToString(Formatting.None);
            //        }
            //        rsp?.Dispose();
            //    }

            //    logger.Debug($"Http Response >> {retString}");
            //    return retString;
            //});
        }

        public static async Task<string> Get(string url)
        {
            logger.Debug($"Http Get >> {url}");
            return await GlobalClient.GetAsync(url).AsyncGetHttpResponseString().SafeAsyncCall();
            //return await new HttpRequestMessage(HttpMethod.Get, url).SendAsync();
        }

        public static async Task<string> Delete(string url)
        {
            logger.Debug($"Http Delete >> {url}");
            return await GlobalClient.DeleteAsync(url).AsyncGetHttpResponseString().SafeAsyncCall();
            //return await new HttpRequestMessage(HttpMethod.ApiDeleteProject, url).SendAsync();
        }

        public static async Task<string> Post(string url, string bodyData)
        {
            logger.Debug($"Http Post >> {url} {bodyData}");
            return await GlobalClient.PostAsync(url, bodyData.ToHttpJsonStringContent()).AsyncGetHttpResponseString()
                .SafeAsyncCall();
            //return await new HttpRequestMessage(HttpMethod.Post, url) { Content = bodyData.ToHttpStringContent() }.SendAsync();
        }

        public static async Task<string> Post(string url, HttpContent bodyData)
        {
            logger.Debug($"Http Post HttpContent >> {url} ");
            return await GlobalClient.PostAsync(url, bodyData).AsyncGetHttpResponseString().SafeAsyncCall();
            //return await new HttpRequestMessage(HttpMethod.Post, url) { Content = bodyData,Version = HttpVersion.Version10}.SendAsync();
        }

        public static async Task<string> Put(string url, string bodyData)
        {
            logger.Debug($"Http Put >> {url} {bodyData}");
            return await GlobalClient.PutAsync(url, bodyData.ToHttpJsonStringContent()).AsyncGetHttpResponseString()
                .SafeAsyncCall();
            //  return await new HttpRequestMessage(HttpMethod.Put, url) { Content = bodyData.ToHttpStringContent() }.SendAsync();
        }

        public static async Task<TRet> SendHttpGet<TRet>(this string url)
        {
            return JsonConvert.DeserializeObject<TRet>(await Get(url));
        }

        public static async Task<TRet> SendHttpDelete<TRet>(this string url)
        {
            return JsonConvert.DeserializeObject<TRet>(await Delete(url));
        }

        public static async Task<TRet> SendHttpPost<TRet>(this string url, string reqJsonStr)
        {
            return JsonConvert.DeserializeObject<TRet>(await Post(url, reqJsonStr));
        }

        public static async Task<TRet> SendHttpPost<TRet>(this string url, HttpContent formData)
        {
            return JsonConvert.DeserializeObject<TRet>(await Post(url, formData));
        }

        public static async Task<TRet> SendHttpPost<TRet>(this string url, object bodyData)
        {
            return await url.SendHttpPost<TRet>(bodyData.ToJsonString());
        }

        public static async Task<TRet> SendHttpPut<TRet>(this string url, string reqJsonStr)
        {
            return JsonConvert.DeserializeObject<TRet>(await Put(url, reqJsonStr));
        }

        public static async Task<TRet> SendHttpPut<TRet>(this string url, object bodyData)
        {
            return await url.SendHttpPut<TRet>(bodyData.ToJsonString());
        }

        public static async Task<bool> DownloadFile(this string url, string file)
        {
            logger.Debug($"download : {url} to {file}");
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }
            try
            {
                using (Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var contentStream = await GlobalClient.GetStreamAsync(url))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"DownloadFile from : {url} error :{ex.Message}");
                return false;
            }
        }
    }
}