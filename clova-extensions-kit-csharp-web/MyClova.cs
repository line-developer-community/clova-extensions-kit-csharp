using LineDC.CEK.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    public class MyClova : ClovaBase
    {
        protected override async Task OnLaunchRequestAsync(Session session, CancellationToken cancellationToken)
        {
            // Single Text Reply
            Response.AddText("Welcome to CEK", Lang.En).KeepListen();
        }

        protected override async Task OnIntentRequestAsync(Intent intent, Session session, CancellationToken cancellationToken)
        {
            switch (intent.Name)
            {
                case "Clova.YesIntent":
                    Response
                        // adding session information
                        .SetSession("mySessionKey", "Yes")
                        // Add single URL Response and Text Reprompt
                        .AddUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3")
                        .AddRepromptText("Tell me something, please", Lang.En)
                        .KeepListen();
                    break;

                case "Clova.NoIntent":
                    Response
                        // adding session information
                        .SetSession("mySessionKey", "No")
                        // Add Brief and Verbose as SpeechSet
                        .AddBriefText("Brief explain.", Lang.En)
                        .AddVerboseText("Detail explain 1.", Lang.En)
                        .AddVerboseText("Detail explain 2.", Lang.En)
                        .AddVerboseUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3")
                        .KeepListen();
                    break;

                case "Clova.GuideIntent":
                    // Add multiple Reposonses and Reprompts
                    Response
                        .AddText("Sure!", Lang.En)
                        .AddUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3")
                        .AddText("Let me explain how to use it!", Lang.En)
                        .AddRepromptText("Did you understand?", Lang.En)
                        .AddRepromptText("Now tell me what you want.", Lang.En)
                        .AddRepromptUrl("https://clova-common.line-scdn.net/dice/rolling_dice_sound.mp3")
                        .KeepListen();
                    break;
            }
        }

        protected override async Task OnSessionEndedRequestAsync(Session session, CancellationToken cancellationToken)
        {
            // get session information by specifying default value in case no session information
            var mySessionValue = session.GetAttribute("mySessionKey", "defaultValue");

            // You can also create your own session object class and pass it to SetSessionAttributesFrom in request.
            // To retrieve the object, call GetSessionAttributesAs<T>().

            Response.AddText($"Good bye! Session value is {mySessionValue}.", Lang.En);
        }
    }
}
