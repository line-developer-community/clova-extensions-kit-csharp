[![Build Status](https://dev.azure.com/LINEDeveloperCommunity/clova-extensions-kit/_apis/build/status/clova-extensions-kit-CI?branchName=master)](https://dev.azure.com/LINEDeveloperCommunity/clova-extensions-kit/_build/latest?definitionId=3&branchName=master)

# Clova CEK SDK C#

C# SDK for Clova Extension Kit.
Available on NuGet: https://www.nuget.org/packages/CEK.CSharp/

# Usage
## Inherit ClovaBase abstract class

Create a class that inherits `ClovaBase`.

```csharp
public class MyClova : ClovaBase
{
}
```

*If you need more properties or methods, use an extended interface.

```csharp
public interface ILoggableClova : IClova
{
    ILogger Logger { get; set; }
}

public class MyClova : ClovaBase, ILoggableClova
{
    public ILogger Logger { get; set; }
}
```

## Instantiate or Dependency Injection

Instantiate the derived class.

```csharp
var clova = new MyClova();
```

If you want to use Dependency Injection, call `AddClova` extension method in the Startup class.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddClova<IClova, MyClova>();
    services.AddMvc();
}
```

Default language is `Lang.Ja`, but you can change it.

```csharp
clova.SetDefaultLang(Lang.En);
```

or `AddClova` overload:

```csharp
services.AddClova<IClova, MyClova>(Lang.En);
```

## Call RespondAsync method

Pass Signature Header and Body to handle request and create response.

```csharp
var response = await clova.RespondAsync(Request.Headers["SignatureCEK"], Request.Body);
return new OkObjectResult(response);
```

*If you want to skip validation, pass true at the end.

```csharp
var response = await clova.RespondAsync(Request.Headers["SignatureCEK"], Request.Body, true);
```

## Override Methods

Override methods executed for each request type or event.

```csharp
public class MyClova : ClovaBase
{
    protected override async Task OnLaunchRequestAsync(
        Session session, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
```

*Available virtual methods

Method|Parameters
---|---
OnLaunchRequestAsync|Session session, CancellationToken cancellationToken
OnIntentRequestAsync|Intent intent, Session session, CancellationToken cancellationToken
OnEventRequestAsync|Event ev, Session session, CancellationToken cancellationToken
OnSkillEnabledEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnSkillDisabledEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnPlayFinishedEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnPlayPausedEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnPlayResumedEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnPlayStartedEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnPlayStoppedEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnProgressReportDelayPassedEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnProgressReportIntervalPassedEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnProgressReportPositionPassedEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnStreamRequestedEventAsync|Event ev, Session session, CancellationToken cancellationToken
OnSessionEndedRequestAsync|Session session, CancellationToken cancellationToken
OnUnrecognizedRequestAsync|CEKRequest request, CancellationToken cancellationToken


## Add content to Response property

You can add content for response of Clova Extension to `Response` property with method chaining.

When you add the text, you can also set the language to overwrite default language.

1\. Add Reply. 

```csharp
Response
    .AddText("こんにちは!");
    .AddUrl("https://dummy.domain/myaudio.mp3");
    .AddText("Hi!", Lang.En);
    .AddUrl("https://dummy.domain/myaudio.mp3", Lang.En);
```

2\. Add Brief/Verbose.

```csharp
Response
    .AddBriefText("Brief explain.", Lang.En);
    .AddVerboseText("Detail explain 1.", Lang.En);
    .AddVerboseText("Detail explain 2.", Lang.En);
    .AddVerboseUrl("https://dummy.domain/myaudio.mp3");
```

3\. Add Reprompt.

```csharp
Response
    .AddRepromptText("Tell me something, please", Lang.En);
    .AddRepromptUrl("https://dummy.domain/myaudio.mp3");
```
4\. Add session value.

```csharp
Response.SetSession("mySessionKey", "mySessionValue");
```

5\. Keep listening for multi-turn session.

```csharp
Response
    .AddText("What do you want?", Lang.En)
    .KeepListening();
```

## Use AudioPlayer 

You can use CEK's AudoPlayer through the simple methods.

Method|Parameters
---|---
PlayAudio|Source source, AudioItem audioItem, AudioPlayBehavior playBehavior
EnqueueAudio|Source source, params AudioItem[] audioItems
PauseAudio|-
ResumeAudio|-
StopAudio|-

# Samples

- [Azure Functions](clova-extensions-kit-csharp-azure-functions/)
- [ASP.NET Core](clova-extensions-kit-csharp-web/)

# LISENCE

[MIT](./LICENSE)
