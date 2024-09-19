using System;
using System.Text.Json.Serialization;

namespace CrimsonFAQ.Structs;

public class KeyResponse
{
    [JsonPropertyName("Key")]
    public string Key { get; set; }

    [JsonPropertyName("Response")]
    public string Response { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }

    [JsonPropertyName("IsGlobal")]
    public bool IsGlobal { get; set; }

    [JsonPropertyName("PermissionLevel")]
    public int PermissionLevel { get; set; } // 0 = User, 1 = Trusted, 2 = Admin

    [JsonPropertyName("GlobalCooldownSeconds")]
    public int GlobalCooldownSeconds { get; set; }

    [JsonIgnore]
    public DateTime GlobalLastUsed { get; set; }

    public KeyResponse()
    {
        GlobalLastUsed = DateTime.MinValue;
    }

    [JsonConstructor]
    public KeyResponse(string key, string response, string description, bool isGlobal = false, int permissionLevel = 0, int globalCooldownSeconds = 0)
    {
        Key = key;
        Response = response;
        Description = description;
        IsGlobal = isGlobal;
        PermissionLevel = permissionLevel;
        GlobalCooldownSeconds = globalCooldownSeconds;
        GlobalLastUsed = DateTime.MinValue;
    }
}