using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    public class ClovaDurableFunction
    {
        private IDurableClova Clova { get; }

        public ClovaDurableFunction(IDurableClova clova)
        {
            Clova = clova;
        }

        [FunctionName("ClovaDurableFunction_HttpStart")]
        public async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequest req,
            [OrchestrationClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            // set orchestration client and logger
            Clova.DurableOrchestrationClient = starter;
            Clova.Logger = log;

            var response = await Clova.RespondAsync(req.Headers["SignatureCEK"], req.Body);
            return new OkObjectResult(response);
        }


        [FunctionName("ClovaDurableFunction_RunOrchestrator")]

        public async Task<string> RunOrchestrator(
            [OrchestrationTrigger]IDurableOrchestrationContext context)
        {
            var input = context.GetInput<int>();
            var output = await context.CallActivityAsync<string>("ClovaDurableFunction_LongTimeActivity", input);
        
            return output;
        }

        [FunctionName("ClovaDurableFunction_LongTimeActivity")]
        public async Task<string> LongTimeActivity([ActivityTrigger]int count, ILogger log)
        {
            log.LogInformation($"Start to count: {count}");

            for (var i = 0; i < count; i++)
            {
                // sleep (1sec)
                log.LogInformation($"count: {i + 1}");
                await Task.Delay(1000);
            }

            return $"Finish to count !: {count}";
        }

    }
}