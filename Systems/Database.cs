using CrimsonFAQ.Structs;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CrimsonFAQ.Systems;

public class Database
{
    public static string ResponsesFile = Path.Combine(Plugin.ConfigFiles, "responses.json");
    public static string TrustedFile = Path.Combine(Plugin.ConfigFiles, "trusted.json");

    public List<KeyResponse> Responses { get; set; }
    public List<string> Trusted { get; set; }

    public Database()
    {
        CreateDatabaseFiles();
        LoadDatabase();
    }

    public bool LoadDatabase()
    {
        try
        {
            string json = File.ReadAllText(ResponsesFile);
            Responses = JsonSerializer.Deserialize<List<KeyResponse>>(json);
            Plugin.LogInstance.LogInfo($"Loaded Responses Database: {Responses.Count} entries.");

            string jsonTrusted = File.ReadAllText(TrustedFile);
            Trusted = JsonSerializer.Deserialize<List<string>>(jsonTrusted);
            Plugin.LogInstance.LogInfo($"Loaded Trusted Database: {Trusted.Count} entries.");
            return true;
        }
        catch (Exception e)
        { 
            Plugin.LogInstance.LogError(e);
            return false;
        }
    }

    public void SaveDatabase()
    {
        var json = JsonSerializer.Serialize(Trusted);
        File.WriteAllText(TrustedFile, json);
    }

    public static void CreateDatabaseFiles()
    { 
        if(!File.Exists(ResponsesFile)) 
        {
            List<KeyResponse> template = new List<KeyResponse>();

            KeyResponse response = new KeyResponse("discord", "Join our discord at discord.gg/RBPesMj", "discord link", true, 0, 30);
            template.Add(response);

            var json = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true});

            File.WriteAllText(ResponsesFile, json);
        }

        if(!File.Exists(TrustedFile))
        {
            List<string> template = new List<string>();

            var json = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TrustedFile, json);
        }
    }

    public bool GetResponse(string key, out KeyResponse response)
    {
        response = null;

        if(Responses.Exists(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)))
        {
            response = Responses.Find(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
            return true;
        }
        
        return false;
    }

    public bool IsTrusted(User user, int keyLevel)
    {
        if (keyLevel == 1)
        {
            if (user.IsAdmin || Trusted.Contains(user.PlatformId.ToString())) return true;
        }

        if (keyLevel == 2)
        {
            if (user.IsAdmin) return true;
        }

        return false;
    }

    public bool AddTrusted(User user)
    {
        if (Trusted.Contains(user.PlatformId.ToString())) return false;

        Trusted.Add(user.PlatformId.ToString());
        SaveDatabase();
        return true;
    }

    public bool RemoveTrusted(User user)
    {
        if(!Trusted.Contains(user.PlatformId.ToString())) return false;

        Trusted.Remove(user.PlatformId.ToString());
        SaveDatabase();
        return true;
    }
}
