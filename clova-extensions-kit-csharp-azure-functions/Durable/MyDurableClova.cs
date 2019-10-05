using LineDC.CEK.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    public class MyDurableClova : ClovaBase, IDurableClova
    {
        public IDurableOrchestrationClient DurableOrchestrationClient { get; set ; }
        public ILogger Logger { get; set; }

        public MyDurableClova(ClovaConfiguration config)
        {
            SetDefaultLang(config.DefaultLang);
        }

        protected override async Task OnLaunchRequestAsync(Session session, CancellationToken cancellationToken)
        {
            // check status
            var status = await DurableOrchestrationClient.GetStatusAsync(session.User.UserId);

            if (status?.RuntimeStatus == OrchestrationRuntimeStatus.ContinuedAsNew ||
                status?.RuntimeStatus == OrchestrationRuntimeStatus.Pending ||
                status?.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                Response.AddText("Counting has not finished yet.");
            }
            else
            {
                Response
                    .AddText("How many counts?")
                    .AddRepromptText("Please specify the number of counts.")
                    .KeepListening();
            }
        }

        protected override async Task OnIntentRequestAsync(Intent intent, Session session, CancellationToken cancellationToken)
        {
            if (intent.Name == "CountIntent" && intent.Slots.TryGetValue("count", out var slot) && int.TryParse(slot.Value, out var count))
            {
                // start orchestrator
                await DurableOrchestrationClient.StartNewAsync("ClovaDurableFunction_RunOrchestrator", session.User.UserId, count);
                Response.AddText("Started orchestrator. Please wait.");
            }
            else
            {
                Response
                    .AddText("How many counts?")
                    .AddRepromptText("Please specify the number of counts.")
                    .KeepListening();
            }
        }
    }
}
