using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;

namespace CrimsonFAQ.Structs;

public readonly struct Settings
{
    // base configs
    public static ConfigEntry<bool> FAQ { get; private set; }
    public static ConfigEntry<string> Prefix { get; private set; }

    public static ConfigEntry<string> HexKey { get; private set; }
    public static ConfigEntry<string> HexDescription { get; private set; }
    public static ConfigEntry<string> HexMisc { get; private set; }
    public static ConfigEntry<string> HexResponse { get; private set; }


    public static void InitConfig()
    {
        foreach (string path in directoryPaths) // make sure directories exist
        {
            CreateDirectories(path);
        }

        // Set Base Configs
        FAQ = InitConfigEntry("_Config", "FAQEnable", true,
            "Enable or disable the mod.");

        Prefix = InitConfigEntry("_Config", "Prefix", "?",
            "The prefix before information requests i.e. \"?discord\"");

        // Hex Configs
        HexKey = InitConfigEntry("Response Colors", "KeyColor", "#9cb730", 
            "The hex value color that will be displayed for keys in .faq list");

        HexDescription = InitConfigEntry("Response Colors", "DescriptionColor", "#309CB7",
            "The hex value color that will be displayed for descriptions in .faq list");

        HexMisc = InitConfigEntry("Response Colors", "MiscColor", "#b7309c",
            "The hex value color that will be used for formatting elements like the dash in .faq list");

        HexResponse = InitConfigEntry("Response Colors", "ResponseColor", "#9cb730",
            "The hex value color that will be used for responding to user key queries");
    }

    static ConfigEntry<T> InitConfigEntry<T>(string section, string key, T defaultValue, string description)
    {
        // Bind the configuration entry and get its value
        var entry = Plugin.Instance.Config.Bind(section, key, defaultValue, description);

        // Check if the key exists in the configuration file and retrieve its current value
        var newFile = Path.Combine(Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}.cfg");

        if (File.Exists(newFile))
        {
            var config = new ConfigFile(newFile, true);
            if (config.TryGetEntry(section, key, out ConfigEntry<T> existingEntry))
            {
                // If the entry exists, update the value to the existing value
                entry.Value = existingEntry.Value;
            }
        }
        return entry;
    }

    static void CreateDirectories(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    static readonly List<string> directoryPaths = new List<string>
    {
        Plugin.ConfigFiles,
    };
}
