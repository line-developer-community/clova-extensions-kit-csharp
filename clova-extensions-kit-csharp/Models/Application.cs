using Newtonsoft.Json;

namespace LineDC.CEK.Models
{
    /// <summary>
    /// ユーザーの意図によって実行されるExtensionの情報を持っているオブジェクト
    /// </summary>
    public class Application
    {
        /// <summary>
        /// ExtensionのID
        /// </summary>
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }
    }
}
