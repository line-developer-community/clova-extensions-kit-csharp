using System.IO;
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
        /// <returns>CEK Response</returns>
        Task<CEKRequest> GetRequest(string signatureCEK, Stream body, bool skipValidation = false);
    }
}