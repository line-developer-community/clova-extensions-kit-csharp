using LineDC.CEK.Models;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    public class MySimpleClova : ClovaBase, ILoggableClova
    {
        public ILogger Logger { get; set; }

        protected override async Task OnLaunchRequestAsync(Session session, CancellationToken cancellationToken)
        {
            Logger.LogInformation("LaunchRequest");

            // Single Text Reply
            Response.AddText("Welcome to CEK").KeepListen();
        }

        protected override async Task OnIntentRequestAsync(Intent intent, Session session, CancellationToken cancellationToken)
        {
            switch (intent.Name)
            {
                case "Clova.YesIntent":
                    Logger.LogInformation("YesIntent");

                    Response
                        // adding session information
                        .SetSession("mySessionKey", "Yes")
                        // Add single URL Response and Text Reprompt
                        .AddUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3")
                        .AddRepromptText("Tell me something, please")
                        .KeepListen();
                    break;

                case "Clova.NoIntent":
                    Logger.LogInformation("NoIntent");

                    Response
                        // adding session information
                        .SetSession("mySessionKey", "No")
                        // Add Brief and Verbose as SpeechSet
                        .AddBriefText("Brief explain.")
                        .AddVerboseText("Detail explain 1.")
                        .AddVerboseText("Detail explain 2.")
                        .AddVerboseUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3")
                        .KeepListen();
                    break;

                case "Clova.GuideIntent":
                    Logger.LogInformation("GuideIntent");

                    Response
                        // adding session information
                        .SetSession("mySessionKey", "Guide")
                        // Add multiple Reposonses and Reprompts
                        .AddText("Sure!")
                        .AddUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3")
                        .AddText("Let me explain how to use it!")
                        .AddRepromptText("Did you understand?")
                        .AddRepromptText("Now tell me what you want.")
                        .AddRepromptUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3")
                        .KeepListen();
                    break;
            }
        }

        protected override async Task OnSessionEndedRequestAsync(Session session, CancellationToken cancellationToken)
        {
            Logger.LogInformation("SessionEndedRequest");

            // get session information by specifying default value in case no session information
            var mySessionValue = session.GetAttribute("mySessionKey", "defaultValue");

            // You can also create your own session object class and pass it to SetSessionAttributesFrom in request.
            // To retrieve the object, call GetSessionAttributesAs<T>().

            Response.AddText($"Good bye! Session value is {mySessionValue}.");
        }
    }
}
