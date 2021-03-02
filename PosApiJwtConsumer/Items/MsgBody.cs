
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PosApiJwtConsumer.Items
{
    public class MsgBody
    {
        [JsonPropertyName("msg")]
        public string Msg { get; set; }
        [JsonPropertyName("stamp")]
        public string Stamp { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static MsgBody FromJson(string json)
        {
            return JsonSerializer.Deserialize<MsgBody>(json);
        }

    }

}
