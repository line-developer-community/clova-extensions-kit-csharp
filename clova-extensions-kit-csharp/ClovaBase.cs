using LineDC.CEK.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK
{
    /// <summary>
    /// Inherit this class to plug in code to handle particular request types and/or event types.
    /// </summary>
    public abstract class ClovaBase : IClova
    {
        private static HttpClient _httpClient;
        private static string cert;

        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                    _httpClient = new HttpClient();

                return _httpClient;
            }
        }

        protected CEKRequest Request { get; private set; }
        protected CEKResponse Response { get; private set; }


        /// <summary>
        /// Verify the signature
        /// https://clova-developers.line.me/guide/#/CEK/References/CEK_API.md#RequestMessageValidation
        /// </summary>
        /// <param name="signatureCEK">Signature Header value</param>
        /// <param name="body">Content Body (byte array)</param>
        /// <returns></returns>
        private async Task<bool> VerifySignature(string signatureCEK, byte[] body)
        {
            if (string.IsNullOrEmpty(signatureCEK))
                throw new Exception("Signature missing");

            if (string.IsNullOrEmpty(cert))
                cert = await HttpClient.GetStringAsync("https://clova-cek-requests.line.me/.well-known/signature-public-key.pem");

            var provider = PemKeyUtils.GetRSAProviderFromPemString(cert.Trim());
            if (!provider.VerifyData(body, Convert.FromBase64String(signatureCEK), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                throw new Exception("Invalid Signature");
            else
                return true;
        }

        private byte[] ConvertStreamToByteArray(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[16 * 1024];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Get CEK Request from HttpRequest Body (Stream)
        /// </summary>
        /// <param name="signatureCEK">Signature Header value</param>
        /// <param name="body">Content Body (Stream)</param>
        /// <returns>CEK Response</returns>
        private async Task<CEKRequest> GetRequest(string signatureCEK, Stream body, bool skipValidation = false)
        {
            var bodyContent = ConvertStreamToByteArray(body);
            if (!skipValidation)
                await VerifySignature(signatureCEK, bodyContent);
            return JsonConvert.DeserializeObject<CEKRequest>(Encoding.UTF8.GetString(bodyContent));
        }

        /// <summary>
        /// Handle request and respond to client device.
        /// </summary>
        /// <param name="signatureCEK"></param>
        /// <param name="body"></param>
        /// <param name="skipValidation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CEKResponse> RespondAsync(string signatureCEK, Stream body, bool skipValidation = false, CancellationToken cancellationToken = default)
        {
            Request = await GetRequest(signatureCEK, body, skipValidation);
            Response = new CEKResponse();

            switch(Request.Request.Type)
            {
                case RequestType.LaunchRequest:
                    await OnLaunchRequestAsync(Request.Session, cancellationToken);
                    break;
                case RequestType.IntentRequest:
                    await OnIntentRequestAsync(Request.Request.Intent, Request.Session, cancellationToken);
                    break;
                case RequestType.EventRequest:
                    await OnEventRequestAsync(Request.Request.Event, Request.Session, cancellationToken);
                    break;
                case RequestType.SessionEndedRequest:
                    await OnSessionEndedRequestAsync(Request.Session, cancellationToken);
                    break;
                default:
                    await OnUnrecognizedRequestAsync(Request, cancellationToken);
                    break;
            }
            return Response;
        }

        protected virtual Task OnLaunchRequestAsync(Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnIntentRequestAsync(Intent intent, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnEventRequestAsync(Event ev, Session session, CancellationToken cancellationToken)
        {
            return ev.Name switch
            {
                // namespace: ClovaSkill
                "SkillEnabled"                  => OnSkillEnabledEventAsync(ev, session, cancellationToken),
                "SkillDisabled"                 => OnSkillDisabledEventAsync(ev, session, cancellationToken),
                // namespace: AudioPlayer
                "PlayFinished"                  => OnPlayFinishedEventAsync(ev, session, cancellationToken),
                "PlayPaused"                    => OnPlayPausedEventAsync(ev, session, cancellationToken),
                "PlayResumed"                   => OnPlayResumedEventAsync(ev, session, cancellationToken),
                "PlayStarted"                   => OnPlayStartedEventAsync(ev, session, cancellationToken),
                "PlayStopped"                   => OnPlayStoppedEventAsync(ev, session, cancellationToken),
                "ProgressReportDelayPassed"     => OnProgressReportDelayPassedEventAsync(ev, session, cancellationToken),
                "ProgressReportIntervalPassed"  => OnProgressReportIntervalPassedEventAsync(ev, session, cancellationToken),
                "ProgressReportPositionPassed"  => OnProgressReportPositionPassedEventAsync(ev, session, cancellationToken),
                "StreamRequested"               => OnStreamRequestedEventAsync(ev, session, cancellationToken),
                _                               => Task.CompletedTask
            };
        }

        protected virtual Task OnSkillEnabledEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnSkillDisabledEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayFinishedEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayPausedEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayResumedEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayStartedEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnPlayStoppedEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnProgressReportDelayPassedEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnProgressReportIntervalPassedEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnProgressReportPositionPassedEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnStreamRequestedEventAsync(Event ev, Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnSessionEndedRequestAsync(Session session, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task OnUnrecognizedRequestAsync(CEKRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// Direct the client to play audio stream
        /// </summary>
        /// <param name="playBehavior"></param>
        /// <param name="audioItem"></param>
        public void PlayAudio(Source source, AudioItem audioItem, AudioPlayBehavior playBehavior = AudioPlayBehavior.REPLACE_ALL)
        {
            Response.Response.Directives.Add(new Directive()
            {
                Header = new DirectiveHeader()
                {
                    Namespace = DirectiveHeaderNamespace.AudioPlayer,
                    Name = DirectiveHeaderName.Play
                },
                Payload = new AudioPlayPayload
                {
                    AudioItem = audioItem,
                    PlayBehavior = playBehavior,
                    Source = source
                }
            });
        }

        /// <summary>
        /// Add audio stream to play queue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="audioItems"></param>
        public void EnqueueAudio(Source source, params AudioItem[] audioItems)
        {
            Response.Response.Directives.AddRange(audioItems.Select(item => new Directive()
            {
                Header = new DirectiveHeader()
                {
                    Namespace = DirectiveHeaderNamespace.AudioPlayer,
                    Name = DirectiveHeaderName.Play
                },
                Payload = new AudioPlayPayload
                {
                    AudioItem = item,
                    PlayBehavior = AudioPlayBehavior.ENQUEUE,
                    Source = source
                }
            }));
        }

        /// <summary>
        /// Direct the client to pause audio stream 
        /// </summary>
        public void PauseAudio()
        {
            Response.Response.Directives.Add(new Directive()
            {
                Header = new DirectiveHeader()
                {
                    Namespace = DirectiveHeaderNamespace.PlaybackController,
                    Name = DirectiveHeaderName.Pause
                }
            });
        }

        /// <summary>
        /// Direct the client to resume audio stream
        /// </summary>
        public void ResumeAudio()
        {
            Response.Response.Directives.Add(new Directive()
            {
                Header = new DirectiveHeader()
                {
                    Namespace = DirectiveHeaderNamespace.PlaybackController,
                    Name = DirectiveHeaderName.Resume
                }
            });
        }

        /// <summary>
        /// Direct the client to stop audio stream
        /// </summary>
        public void StopAudio()
        {
            Response.Response.Directives.Add(new Directive()
            {
                Header = new DirectiveHeader()
                {
                    Namespace = DirectiveHeaderNamespace.PlaybackController,
                    Name = DirectiveHeaderName.Stop
                }
            });
        }
    }
}
