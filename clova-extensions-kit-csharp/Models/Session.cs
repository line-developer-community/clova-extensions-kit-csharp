using Newtonsoft.Json;
using System.Collections.Generic;

namespace LineDC.CEK.Models
{
    /// <summary>
    /// セッション情報を持っているオブジェクト。ここでいうセッションとは、ユーザーのリクエストを区分する単位です。
    /// </summary>
    public class Session
    {
        /// <summary>
        /// リクエストメッセージが新しいセッションに対するものか、それとも既存のセッションに対するものかを区分します。
        /// true：新しいセッション
        /// false：既存のセッション
        /// </summary>
        [JsonProperty("new")]
        public bool New { get; set; }
        /// <summary>
        /// ユーザーとのマルチターン対話に必要な情報を保存したオブジェクト。Custom Extensionは、レスポンスメッセージのresponse.sessionAttributesフィールドを使用して中間情報をCEKに渡します。ユーザーの追加のリクエストを受け付けると、その情報は再びリクエストメッセージのsession.sessionAttributesフィールドで渡されます。オブジェクトはキー(key)と値(value)のペアで構成され、Custom Extensionを実装する際、任意に定義できます。保存された値がない場合、空のオブジェクトが渡されます。
        /// </summary>
        [JsonProperty("sessionAttributes")]
        public Dictionary<string, object> SessionAttributes { get; set; }
        /// <summary>
        /// セッションID
        /// </summary>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        /// <summary>
        /// 現在のユーザーの情報を持っているオブジェクト
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }

        /// <summary>
        /// セッション情報から特定の値を取得する
        /// </summary>
        /// <param name="key">セッション情報のキー</param>
        /// <param name="defaultValue">セッション情報の既定値</param>
        /// <returns>セッション情報の値</returns>
        public object GetAttribute(string key, object defaultValue = null)
        {
            if (this.SessionAttributes.ContainsKey(key))
                return this.SessionAttributes[key];
            else
                return defaultValue;
        }
    }
}
