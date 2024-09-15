using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Bloodstone.API;
using CrimsonFAQ.Structs;
using CrimsonFAQ.Systems;
using HarmonyLib;
using System.IO;
using System.Reflection;
using VampireCommandFramework;

namespace CrimsonFAQ;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
public class Plugin : BasePlugin
{
    Harmony _harmony;
    internal static Plugin Instance { get; private set; }
    public static Settings Settings { get; private set; }
    public static Harmony Harmony => Instance._harmony;
    public static ManualLogSource LogInstance => Instance.Log;
    public static Database DB { get; internal set; }

    public static string ConfigFiles => Path.Combine(Paths.ConfigPath, "CrimsonFAQ");

    public override void Load()
    {
        Instance = this;
        Settings = new Settings();
        Settings.InitConfig();

        if (!VWorld.IsServer) Log.LogWarning("This plugin is a server-only plugin");

        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        CommandRegistry.RegisterAll();
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        DB = new Database();

        Bloodstone.Hooks.Chat.OnChatMessage += ((x) =>
        {
            if (x.Type == ProjectM.Network.ChatMessageType.System) return;
            if (!Settings.FAQ.Value) return;
            Responder.Respond(x);
        });
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        return true;
    }

}
