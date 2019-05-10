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
            var response = await handler.HandleRequestAsync(request);

            // adding session information
            response.AddSession("mySessionKey", "mySessionValue");

            // You can also create your own session object class and pass it to SetSessionAttributesFrom in request.
            // To retrieve the object, call GetSessionAttributesAs<T>().

            return new OkObjectResult(response);
        }
    }

    public class SampleRequestHandler : RequestHandler
    {
        protected override async Task OnLaunchRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            // Single Text Reply
            Response.AddText("Welcome to CEK", Lang.En);
            Response.ShouldEndSession = false;
        }

        protected override async Task OnSessionEndedRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            Response.AddText("Good bye!", Lang.En);
            Response.ShouldEndSession = true;
        }

        protected override async Task OnIntentRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            switch (request.Request.Intent.Name)
            {
                case "Clova.YesIntent":
                    // Add single URL Response and Text Reprompt
                    Response.AddUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    Response.AddRepromptText("Tell me something, please", Lang.En);
                    Response.ShouldEndSession = false;
                    break;
                case "Clova.NoIntent":
                    // Add Brief and Verbose as SpeechSet
                    Response.AddBriefText("Brief explain.", Lang.En);
                    Response.AddVerboseText("Detail explain 1.", Lang.En);
                    Response.AddVerboseText("Detail explain 2.", Lang.En);
                    Response.AddVerboseUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3");
                    Response.ShouldEndSession = false;
                    break;
                case "Clova.GuideIntent":
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
