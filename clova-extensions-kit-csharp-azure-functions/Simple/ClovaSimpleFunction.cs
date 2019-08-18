using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    public class ClovaSimpleFunction
    {
        private ILoggableClova Clova { get; }

        public ClovaSimpleFunction(ILoggableClova clova)
        {
            Clova = clova;
        }

        [FunctionName(nameof(SimpleHttpStart))]
        public async Task<IActionResult> SimpleHttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // set logger
            Clova.Logger = log;

            var response = await Clova.RespondAsync(req.Headers["SignatureCEK"], req.Body);
            return new OkObjectResult(response);
        }
    }
}
