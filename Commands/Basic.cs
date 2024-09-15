using CrimsonFAQ.Structs;
using VampireCommandFramework;

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
}
