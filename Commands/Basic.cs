using CrimsonFAQ.Structs;
using CrimsonFAQ.Services;
using VampireCommandFramework;
using Unity.Entities;
using ProjectM.Network;
using System.Linq;

namespace CrimsonFAQ.Commands;

[CommandGroup("faq")]
internal class Basic
{
    [Command("list", shortHand: "l", description: "shows the list of FAQ requests that can be queried")]
    public void ListFAQs(ChatCommandContext ctx)
    {
        string reply = "\n";
        if (Plugin.DB.Responses.Count == 0)
        {
            reply += "There are no FAQs setup on the server.";
        }
        else
        { 
            foreach(KeyResponse kp in Plugin.DB.Responses) 
            {
                if(kp.PermissionLevel > 0 && !Plugin.DB.IsTrusted(ctx.User, kp.PermissionLevel)) continue;
                string desc = !string.IsNullOrEmpty(kp.Description) ? $"<color={Settings.HexMisc.Value}>-</color> <color={Settings.HexDescription.Value}>{kp.Description}</color>" : "";
                reply += $"<color={Settings.HexKey.Value}>{Settings.Prefix.Value}{kp.Key}</color> {desc}\n";
            }
        }

        ctx.Reply(reply);
    }

    [Command("trust", shortHand: "t", description: "adds a player to the list of trusted users", adminOnly: true)]
    public void AddTrusted(ChatCommandContext ctx, string playerName = "")
    {
        if (string.IsNullOrEmpty(playerName)) ctx.Reply("Must input a player name.");

        var entity = PlayerService.GetUserByName(playerName, true);
        if (!entity.Equals(Entity.Null) && entity.Has<User>())
        {
            if (Plugin.DB.AddTrusted(entity.Read<User>()))
            {
                ctx.Reply($"{playerName} added to trusted FAQ users.");
            }
            else
            {
                ctx.Reply($"{playerName} is already a trusted user.");
            }
        }
        else
        {
            ctx.Reply($"Unable to find player named {playerName}");
        }
    }

    [Command("untrust", shortHand: "ut", description: "removes a player from the list of trusted users", adminOnly: true)]
    public void RemoveTrusted(ChatCommandContext ctx, string playerName = "")
    {
        if (string.IsNullOrEmpty(playerName)) ctx.Reply("Must input a player name.");

        var entity = PlayerService.GetUserByName(playerName, true);

        if (!entity.Equals(Entity.Null) && entity.Has<User>())
        {
            if (Plugin.DB.RemoveTrusted(entity.Read<User>()))
            {
                ctx.Reply($"{playerName} removed from trusted FAQ users.");
            }
            else
            {
                ctx.Reply($"{playerName} is not in the trusted list");
            }
        }
        else
        {
            ctx.Reply($"Unable to find player named {playerName}");
        }
    }

    [Command("reload", shortHand: "r", description: "reloads the FAQ KeyResponse entries from json", adminOnly: true)]
    public void ReloadJSON(ChatCommandContext ctx)
    {
        var oldResponses = Plugin.DB.Responses.ToArray();

        if (Plugin.DB.LoadDatabase())
        {
            var newResponses = Plugin.DB.Responses;
            int updatedResponses = 0;
            
            foreach (var newResponse in newResponses)
            {
                var oldResponse = oldResponses.FirstOrDefault(r => r.Key == newResponse.Key);
                if (oldResponse == null || !oldResponse.Equals(newResponse))
                {
                    updatedResponses++;
                }
            }

            ctx.Reply($"{updatedResponses} responses loaded.");
        }
        else
        {
            ctx.Reply($"Failed to retreive a valid collection from the responses.json file, please validate your formatting and values");
        }
    }
}
