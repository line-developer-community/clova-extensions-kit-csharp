using LineDC.CEK.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK.AzureFunctions
{
    public static class ClovaSimpleFunction
    {
        [FunctionName(nameof(SimpleHttpStart))]
        public static async Task<IActionResult> SimpleHttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var client = new CEKClient();
            var request = await client.GetRequest(req.Headers["SignatureCEK"], req.Body, true);

            // handle request
            var handler = new SampleRequestHandler(log);
            var response = await handler.HandleRequestAsync(request);

            // adding session information
            response.AddSession("mySessionKey", "mySessionValue");
            return new OkObjectResult(response);
        }
    }

    public class SampleRequestHandler : RequestHandler
    {
        private ILogger log;

        public SampleRequestHandler(ILogger log)
        {
            this.log = log;
        }

        protected override async Task OnLaunchRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            log.LogInformation("LaunchRequest");

            // Single Text Reply
            Response.AddText("Welcome to CEK", Lang.En);
            Response.ShouldEndSession = false;
        }

        protected override async Task OnSessionEndedRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            log.LogInformation("SessionEndedRequest");

            Response.AddText("Good bye!", Lang.En);
            Response.ShouldEndSession = true;
        }

        protected override async Task OnIntentRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            // get session information by specifying default value in case no session information
            var mySessionValue = request.GetSessionAttribute("mySessionKey", "defaultValue");
            log.LogInformation($"mySessionValue: {mySessionValue}");

            switch (request.Request.Intent.Name)
            {
                case "Clova.YesIntent":
                    log.LogInformation("YesIntent");

                    // adding session information
                    Response.AddSession("mySessionKey", "Yes");

                    // Add single URL Response and Text Reprompt
                    Response.AddUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    Response.AddRepromptText("Tell me something, please", Lang.En);
                    Response.ShouldEndSession = false;
                    break;
                case "Clova.NoIntent":
                    log.LogInformation("NoIntent");

                    // adding session information
                    Response.AddSession("mySessionKey", "No");

                    // Add Brief and Verbose as SpeechSet
                    Response.AddBriefText("Brief explain.", Lang.En);
                    Response.AddVerboseText("Detail explain 1.", Lang.En);
                    Response.AddVerboseText("Detail explain 2.", Lang.En);
                    Response.AddVerboseUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    Response.ShouldEndSession = false;
                    break;
                case "Clova.GuideIntent":
                    log.LogInformation("GuideIntent");

                    // adding session information
                    Response.AddSession("mySessionKey", "Guide");

                    // Add multiple Reposonses and Reprompts
                    Response.AddText("Sure!", Lang.En);
                    Response.AddUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    Response.AddText("Let me explain how to use it!", Lang.En);
                    Response.AddRepromptText("Did you understand?", Lang.En);
                    Response.AddRepromptText("Now tell me what you want.", Lang.En);
                    Response.AddRepromptUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    Response.ShouldEndSession = false;
                    break;
            }
        }

    }
}
