using LineDC.CEK.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    /// <summary>
    /// Client to handle followings
    /// - verify incoming request
    /// - convert request from Clova and return CEK Response
    /// </summary>
    public class CEKClient : ICEKClient
    {
        static private HttpClient _httpClient;
        static private RequestHandler _requestHandler;
        static private string cert;
        private HttpClient httpClient
        {
            get
            {
                if (_httpClient == null)
                    _httpClient = new HttpClient();

                return _httpClient;
            }
        }
        public CEKClient()
        {
        }

        public CEKClient(RequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        /// <summary>
        /// Verify the signature
        /// https://clova-developers.line.me/guide/#/CEK/References/CEK_API.md#RequestMessageValidation
        /// </summary>
        /// <param name="signatureCEK">Signature Header value</param>
        /// <param name="body">Content Body (byte array)</param>
        /// <returns></returns>
        private async Task<bool> VerifySignature(string signatureCEK, byte[] body)
        {
            if (string.IsNullOrEmpty(signatureCEK))
                throw new Exception("Signature missing");

            if (string.IsNullOrEmpty(cert))
                cert = await httpClient.GetStringAsync("https://clova-cek-requests.line.me/.well-known/signature-public-key.pem");

            RSACryptoServiceProvider provider = PemKeyUtils.GetRSAProviderFromPemString(cert.Trim());
            if (!provider.VerifyData(body, Convert.FromBase64String(signatureCEK), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                throw new Exception("Invalid Signature");
            else
                return true;
        }

        private byte[] ConvertStreamToByteArray(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[16 * 1024];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Get CEK Request from HttpRequest Body (Stream)
        /// </summary>
        /// <param name="signatureCEK">Signature Header value</param>
        /// <param name="body">Content Body (Stream)</param>
        /// <returns>CEK Response</returns>
        public async Task<CEKRequest> GetRequest(string signatureCEK, Stream body, bool skipValidation = false)
        {
            byte[] bodyContent = ConvertStreamToByteArray(body);
            if (!skipValidation)
                await VerifySignature(signatureCEK, bodyContent);
            return JsonConvert.DeserializeObject<CEKRequest>(Encoding.UTF8.GetString(bodyContent));
        }

        /// <summary>
        /// Handle CEK Request with your concrete class extended RequestHandler
        /// </summary>
        /// <param name="signatureCEK"></param>
        /// <param name="body"></param>
        /// <param name="skipValidation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CEKResponse> HandleRequestAsync(string signatureCEK, Stream body, bool skipValidation = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_requestHandler == null)
                throw new Exception("RequestHandler is not set");

            var response = await this.GetRequest(signatureCEK, body, skipValidation);
            return await _requestHandler.HandleRequestAsync(response, cancellationToken);
        }
    }
}
