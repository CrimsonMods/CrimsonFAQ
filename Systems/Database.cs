using CrimsonFAQ.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CrimsonFAQ.Systems;

public class Database
{
    public static string ResponsesFile = Path.Combine(Plugin.ConfigFiles, "responses.json");

    public List<KeyResponse> Responses { get; set; }

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
            return true;
        }
        catch (Exception e)
        { 
            Plugin.LogInstance.LogError(e);
            return false;
        }
    }

    public static void CreateDatabaseFiles()
    { 
        if(!File.Exists(ResponsesFile)) 
        {
            List<KeyResponse> template = new List<KeyResponse>();

            KeyResponse response = new KeyResponse("discord", "Join our discord at discord.gg/abc", true, 30);
            template.Add(response);

            var json = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true});

            File.WriteAllText(ResponsesFile, json);
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
}
