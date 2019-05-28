using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LineDC.CEK.Models;

namespace LineDC.CEK
{
    /// <summary>
    /// CEKClient Interface
    /// </summary>
    public interface ICEKClient
    {
        /// <summary>
        /// Get CEK Request from HttpRequest Body (Stream)
        /// </summary>
        /// <param name="signatureCEK">Signature Header value</param>
        /// <param name="body">Content Body (Stream)</param>
        /// <returns>CEK Request</returns>
        Task<CEKRequest> GetRequest(string signatureCEK, Stream body, bool skipValidation = false);

        /// <summary>
        /// Handle CEK Request with your concrete class extended RequestHandler
        /// </summary>
        /// <param name="signatureCEK"></param>
        /// <param name="body"></param>
        /// <param name="skipValidation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CEKResponse> HandleRequestAsync(string signatureCEK, Stream body, bool skipValidation = false, CancellationToken cancellationToken = default(CancellationToken))
    }
}