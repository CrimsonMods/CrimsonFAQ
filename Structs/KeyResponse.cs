using System;
using System.Text.Json.Serialization;

namespace CrimsonFAQ.Structs;

public class KeyResponse
{
    [JsonPropertyName("Key")]
    public string Key { get; set; }

    [JsonPropertyName("Response")]
    public string Response { get; set; }

    [JsonPropertyName("IsGlobal")]
    public bool IsGlobal { get; set; }

    [JsonPropertyName("IsAdmin")]
    public bool IsAdmin { get; set; }

    [JsonPropertyName("GlobalCooldownMinutes")]
    public int GlobalCooldownMinutes { get; set; }

    [JsonPropertyName("GlobalLastUsed")]
    public DateTime GlobalLastUsed { get; set; }

    public KeyResponse()
    {
        GlobalLastUsed = DateTime.MinValue;
    }

    [JsonConstructor]
    public KeyResponse(string key, string response, bool isGlobal = false, bool isAdmin = false, int globalCooldownMinutes = 0)
    {
        Key = key;
        Response = response;
        IsGlobal = isGlobal;
        IsAdmin = isAdmin;
        GlobalCooldownMinutes = globalCooldownMinutes;
        GlobalLastUsed = DateTime.MinValue;
    }
}