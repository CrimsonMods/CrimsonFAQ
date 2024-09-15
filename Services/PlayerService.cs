using Bloodstone.API;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace CrimsonFAQ.Services;

public class PlayerService
{
    static EntityQuery ActiveUsersQuery;

    static EntityQuery AllUsersQuery;

    public static IEnumerable<Entity> GetUsers(bool includeDisabled = false)
    {
        NativeArray<Entity> userEntities = includeDisabled ? AllUsersQuery.ToEntityArray(Allocator.TempJob) : ActiveUsersQuery.ToEntityArray(Allocator.TempJob);
        try
        {
            foreach (Entity entity in userEntities)
            {
                if (VWorld.Server.EntityManager.Exists(entity))
                {
                    yield return entity;
                }
            }
        }
        finally
        {
            userEntities.Dispose();
        }
    }

    public static Entity GetUserByName(string playerName, bool includeDisabled = false)
    {
        Entity userEntity = GetUsers(includeDisabled).FirstOrDefault(entity => entity.Read<User>().CharacterName.Value.ToLower() == playerName.ToLower());
        return userEntity != Entity.Null ? userEntity : Entity.Null;
    }
}