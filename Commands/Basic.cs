using CrimsonFAQ.Structs;
using CrimsonFAQ.Services;
using VampireCommandFramework;
using Unity.Entities;
using ProjectM.Network;

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
                reply += kp.Key + "\n";
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
}
