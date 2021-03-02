using System.Text.Json;
using System.Text.Json.Serialization;

namespace PosApiJwtConsumer.Items
{
    public class LoginRequest
    {
        [JsonPropertyName("UserName")]
        public string UserName { get; set; }
        [JsonPropertyName("Password")]
        public string Password { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static LoginRequest FromJson(string json)
        {
            return JsonSerializer.Deserialize<LoginRequest>(json);
        }

    }

}
