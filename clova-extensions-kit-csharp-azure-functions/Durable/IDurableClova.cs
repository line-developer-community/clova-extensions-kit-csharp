using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace LineDC.CEK
{
    public interface IDurableClova : ILoggableClova
    {
        IDurableOrchestrationClient DurableOrchestrationClient { get; set; }
    }
}
