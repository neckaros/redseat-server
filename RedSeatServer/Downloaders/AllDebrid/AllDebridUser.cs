
using System.Collections.Generic;
using System.Text.Json.Serialization;

public partial class AllDebridUser
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("isTrial")]
        public bool IsTrial { get; set; }

        [JsonPropertyName("premiumUntil")]
        public long PremiumUntil { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; }

        [JsonPropertyName("preferedDomain")]
        public string PreferedDomain { get; set; }

        [JsonPropertyName("fidelityPoints")]
        public long FidelityPoints { get; set; }

        [JsonPropertyName("limitedHostersQuotas")]
        public Dictionary<string, string> LimitedHostersQuotas { get; set; }
    }