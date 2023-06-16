using Newtonsoft.Json;

namespace Squad.Bot.Models
{
    public class Config
    {
        [JsonProperty(nameof(Token), NullValueHandling = NullValueHandling.Ignore)]
        public string? Token { get; set; }
        [JsonProperty(nameof(TotalShards), NullValueHandling = NullValueHandling.Ignore)]
        public int? TotalShards { get; set; }
        [JsonProperty(nameof(DbOptions), NullValueHandling = NullValueHandling.Ignore)]
        public string? DbOptions { get; set; }
    }
}
