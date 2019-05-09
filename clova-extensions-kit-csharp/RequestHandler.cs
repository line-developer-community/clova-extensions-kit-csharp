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
        public async Task<CEKResponse> OnRequestAsync(CEKRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Request.Type switch
            {
                RequestType.LaunchRequest       => await OnLaunchRequestAsync(request, cancellationToken),
                RequestType.IntentRequest       => await OnIntentRequestAsync(request, cancellationToken),
                RequestType.EventRequest        => await OnEventRequestAsync(request, cancellationToken),
                RequestType.SessionEndedRequest => await OnSessionEndedRequestAsync(request, cancellationToken),
                _                               => await OnUnrecognizedEventTypeAsync(request, cancellationToken)
            };
        }

        protected virtual Task<CEKResponse> OnLaunchRequestAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnIntentRequestAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnEventRequestAsync(CEKRequest request, CancellationToken cancellationToken)
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
                _                               => Task.Run(() => new CEKResponse())
            };
        }

        protected virtual Task<CEKResponse> OnSkillEnabledEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnSkillDisabledEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnPlayFinishedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnPlayPausedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnPlayResumedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnPlayStartedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnPlayStoppedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnProgressReportDelayPassedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnProgressReportIntervalPassedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnProgressReportPositionPassedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnStreamRequestedEventAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnSessionEndedRequestAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());

        protected virtual Task<CEKResponse> OnUnrecognizedEventTypeAsync(CEKRequest request, CancellationToken cancellationToken) => Task.Run(() => new CEKResponse());
    }
}
