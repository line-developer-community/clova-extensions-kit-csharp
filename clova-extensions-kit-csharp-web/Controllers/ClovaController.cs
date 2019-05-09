using LineDC.CEK.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK.Controllers
{
    [Route("api/[controller]")]
    public class ClovaController : Controller
    {
        private ICEKClient client;
        public ClovaController(ICEKClient client)
        {
            this.client = client;
        }

        // POST api/clova
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var request = await client.GetRequest(Request.Headers["SignatureCEK"], Request.Body, true);
            
            // get session information by specifying default value in case no session information
            var mySessionValue = request.GetSessionAttribute("mySessionKey", "defaultValue");

            // handle intents and events
            var handler = new SampleRequestHandler();
            var response = await handler.OnRequestAsync(request);

            // adding session information
            response.AddSession("mySessionKey", "mySessionValue");

            // You can also create your own session object class and pass it to SetSessionAttributesFrom in request.
            // To retrieve the object, call GetSessionAttributesAs<T>().

            return new OkObjectResult(response);
        }
    }

    public class SampleRequestHandler : RequestHandler
    {
        protected override async Task<CEKResponse> OnLaunchRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            var response = new CEKResponse();
            // Single Text Reply
            response.AddText("Welcome to CEK", Lang.En);
            response.ShouldEndSession = false;
            return response;
        }

        protected override async Task<CEKResponse> OnSessionEndedRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            var response = new CEKResponse();
            response.AddText("Good bye!", Lang.En);
            response.ShouldEndSession = true;
            return response;
        }

        protected override async Task<CEKResponse> OnIntentRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            var response = new CEKResponse();
            switch (request.Request.Intent.Name)
            {
                case "Clova.YesIntent":
                    // Add single URL Response and Text Reprompt
                    response.AddUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    response.AddRepromptText("Tell me something, please", Lang.En);
                    response.ShouldEndSession = false;
                    break;
                case "Clova.NoIntent":
                    // Add Brief and Verbose as SpeechSet
                    response.AddBriefText("Brief explain.", Lang.En);
                    response.AddVerboseText("Detail explain 1.", Lang.En);
                    response.AddVerboseText("Detail explain 2.", Lang.En);
                    response.AddVerboseUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    response.ShouldEndSession = false;
                    break;
                case "Clova.GuideIntent":
                    // Add multiple Reposonses and Reprompts
                    response.AddText("Sure!", Lang.En);
                    response.AddUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    response.AddText("Let me explain how to use it!", Lang.En);
                    response.AddRepromptText("Did you understand?", Lang.En);
                    response.AddRepromptText("Now tell me what you want.", Lang.En);
                    response.AddRepromptUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    response.ShouldEndSession = false;
                    break;
            }
            return response;
        }
    }
}
