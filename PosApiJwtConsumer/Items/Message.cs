using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PosApiJwtConsumer.Items
{
    public class Message
    {
        [JsonPropertyName("messageId")]
        public int MessageId { get; set; }
        [JsonPropertyName("to")]
        public string To { get; set; }
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("msgBody")]
        public MsgBody MsgBody { get; set; }


        public string MsgHeader()
        {
            return $"{MessageId} From: {From}";
        }

        public override string ToString()
        {
            return $"{MessageId}\nFrom: {From}\nTo: {To}\n{MsgBody.Msg}\n{MsgBody.Stamp}";
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Message FromJson(string json)
        {
            return JsonSerializer.Deserialize<Message>(json);
        }

        public static List<Message> ListFromJson(string json)
        {
            return JsonSerializer.Deserialize<List<Message>>(json);
        }

    }

}
