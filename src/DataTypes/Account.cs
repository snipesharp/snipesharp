using System.Text.Json.Serialization;

namespace DataTypes
{
    public struct Account
    {
        public bool Prename { get; set; }
        public string Bearer { get; set; }
        public string MicrosoftEmail { get; set; }
        public string MicrosoftPassword { get; set; }
        [JsonPropertyName("sq1")]
        public string SecurityQuestion1 { get; set; }
        [JsonPropertyName("sq2")]
        public string SecurityQuestion2 { get; set; }
        [JsonPropertyName("sq3")]
        public string SecurityQuestion3 { get; set; }
    }
}
