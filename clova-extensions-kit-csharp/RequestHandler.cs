using LineDC.CEK.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    /// <summary>
    /// Inherit this class to plug in code to handle particular request types and/or event types.
    /// </summary>
    public abstract class RequestHandler
    {
        protected CEKResponse Response { get; set; }

        public async Task<CEKResponse> HandleRequestAsync(CEKRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Response = new CEKResponse();

            switch(request.Request.Type)
            {
                case RequestType.LaunchRequest:
                    await OnLaunchRequestAsync(request, cancellationToken);
                    break;
                case RequestType.IntentRequest:
                    await OnIntentRequestAsync(request, cancellationToken);
                    break;
                case RequestType.EventRequest:
                    await OnEventRequestAsync(request, cancellationToken);
                    break;
                case RequestType.SessionEndedRequest:
                    await OnSessionEndedRequestAsync(request, cancellationToken);
                    break;
                default:
                    await OnUnrecognizedEventTypeAsync(request, cancellationToken);
                    break;
            }
            return Response;
        }

        protected virtual Task OnLaunchRequestAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnIntentRequestAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnEventRequestAsync(CEKRequest request, CancellationToken cancellationToken)
        {
            return request.Request.Event.Name switch
            {
                // namespace: ClovaSkill
                "SkillEnabled"                  => OnSkillEnabledEventAsync(request, cancellationToken),
                "SkillDisabled"                 => OnSkillDisabledEventAsync(request, cancellationToken),
                // namespace: AudioPlayer
                "PlayFinished"                  => OnPlayFinishedEventAsync(request, cancellationToken),
                "PlayPaused"                    => OnPlayPausedEventAsync(request, cancellationToken),
                "PlayResumed"                   => OnPlayResumedEventAsync(request, cancellationToken),
                "PlayStarted"                   => OnPlayStartedEventAsync(request, cancellationToken),
                "PlayStopped"                   => OnPlayStoppedEventAsync(request, cancellationToken),
                "ProgressReportDelayPassed"     => OnProgressReportDelayPassedEventAsync(request, cancellationToken),
                "ProgressReportIntervalPassed"  => OnProgressReportIntervalPassedEventAsync(request, cancellationToken),
                "ProgressReportPositionPassed"  => OnProgressReportPositionPassedEventAsync(request, cancellationToken),
                "StreamRequested"               => OnStreamRequestedEventAsync(request, cancellationToken),
                _                               => Task.CompletedTask
            };
        }

        protected virtual Task OnSkillEnabledEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnSkillDisabledEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayFinishedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayPausedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayResumedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayStartedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayStoppedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnProgressReportDelayPassedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnProgressReportIntervalPassedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnProgressReportPositionPassedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnStreamRequestedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnSessionEndedRequestAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnUnrecognizedEventTypeAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
