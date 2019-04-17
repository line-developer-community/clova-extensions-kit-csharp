using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LineDC.CEK.Models
{
    /// <summary>
    /// ディレクティブのAPI名前空間
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DirectiveHeaderNamespace
    {
        AudioPlayer,
        PlaybackController
    }
}
