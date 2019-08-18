using Newtonsoft.Json;

namespace LineDC.CEK.Models
{
    /// <summary>
    /// クライアントがオーディオストリームの再生を終了/一時停止/再開/開始/停止するとき、そのオーディオストリームの情報をCICにレポートするために使用します。
    /// </summary>
    public class AudioEventPayload
    {
        /// <summary>
        /// オーディオストリームのトークン
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }
        /// <summary>
        /// 再生しているストリームの現在のオフセット。ミリ秒単位です。
        /// </summary>
        [JsonProperty("offsetInMilliseconds	")]
        public int OffsetInMilliseconds { get; set; }
    }
}
