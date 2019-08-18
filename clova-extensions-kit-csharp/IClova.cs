using LineDC.CEK.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    /// <summary>
    /// Clova Interface
    /// </summary>
    public interface IClova
    {
        /// <summary>
        ///  Handle CEK Request with your concrete class extended RequestHandler
        /// </summary>
        /// <param name="signatureCEK"></param>
        /// <param name="body"></param>
        /// <param name="skipValidation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CEKResponse> RespondAsync(string signatureCEK, Stream body, bool skipValidation = false, CancellationToken cancellationToken = default);
    }
}
