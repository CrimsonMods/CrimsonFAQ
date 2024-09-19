using Bloodstone.API;
using Bloodstone.Hooks;
using CrimsonFAQ.Structs;
using ProjectM;
using ProjectM.Network;
using System;

namespace CrimsonFAQ.Systems;

public static class Responder
{
    public static void Respond(VChatEvent message)
    {
        if (!message.Message.StartsWith(Settings.Prefix.Value))
            return;

        string useAsKey = message.Message.TrimStart(Settings.Prefix.Value[0]);

        if (!Plugin.DB.GetResponse(useAsKey, out KeyResponse response))
            return;

        var entityManager = VWorld.Server.EntityManager;
        var sender = message.SenderUserEntity.Read<User>();

        if (response.PermissionLevel > 0)
        {
            if (!Plugin.DB.IsTrusted(sender, response.PermissionLevel)) return;
            ServerChatUtils.SendSystemMessageToAllClients(entityManager, response.Response);
            response.GlobalLastUsed = DateTime.Now;
            message.Cancel();
            return;
        }

        if (response.IsGlobal)
        {
            var timeSinceLastUse = DateTime.Now - response.GlobalLastUsed;
            if (timeSinceLastUse.TotalMinutes > response.GlobalCooldownMinutes)
            {
                ServerChatUtils.SendSystemMessageToAllClients(entityManager, response.Response);
                response.GlobalLastUsed = DateTime.Now;
            }
            else
            {
                ServerChatUtils.SendSystemMessageToClient(entityManager, sender, response.Response);
                message.Cancel();
            }
        }
        else
        {
            ServerChatUtils.SendSystemMessageToClient(entityManager, sender, response.Response);
            message.Cancel();
        }
    }
}
