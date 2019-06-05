using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    public static class ClovaSimpleFunction
    {
        [FunctionName(nameof(SimpleHttpStart))]
        public static async Task<IActionResult> SimpleHttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var client = new CEKClient(new SampleRequestHandler(log));
            var response = await client.HandleRequestAsync(req.Headers["SignatureCEK"], req.Body, true);
            return new OkObjectResult(response);
        }
    }
}
