using Bloodstone.API;
using Il2CppInterop.Runtime;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace CrimsonFAQ.Services;

public class PlayerService
{
    static readonly ComponentType[] UserComponent =
        {
            ComponentType.ReadOnly(Il2CppType.Of<User>())
        };
    static EntityQuery ActiveUsersQuery;

    static EntityQuery AllUsersQuery;

    public PlayerService()
    {
        AllUsersQuery = VWorld.Server.EntityManager.CreateEntityQuery(new EntityQueryDesc { 
            All = UserComponent,
            Options = EntityQueryOptions.IncludeDisabled
        });

        ActiveUsersQuery = VWorld.Server.EntityManager.CreateEntityQuery(UserComponent);
    }

    public static IEnumerable<Entity> GetUsers(bool includeDisabled = false)
    {
        List<Entity> result = new List<Entity>();
        try
        {
            NativeArray<Entity> userEntities = includeDisabled ? AllUsersQuery.ToEntityArray(Allocator.TempJob) : ActiveUsersQuery.ToEntityArray(Allocator.TempJob);

            try
            {
                foreach (Entity entity in userEntities)
                {
                    if (VWorld.Server.EntityManager.Exists(entity))
                    {
                        result.Add(entity);
                    }
                    else
                    {
                        Plugin.LogInstance.LogWarning($"Entity {entity.Index} does not exist in EntityManager");
                    }
                }
            }
            finally
            {
                userEntities.Dispose();
            }
        }
        catch (Exception ex)
        {
            Plugin.LogInstance.LogError($"Error in GetUsers: {ex.Message}");
            Plugin.LogInstance.LogError($"Stack trace: {ex.StackTrace}");
            return Enumerable.Empty<Entity>();
        }

        return result;
    }

    public static Entity GetUserByName(string playerName, bool includeDisabled = false)
    {
        var users = GetUsers(includeDisabled).ToList();

        Entity userEntity = users.FirstOrDefault(entity =>
        {
            var user = entity.Read<User>();
            return user.CharacterName.Value.ToLower() == playerName.ToLower();
        });

        return userEntity != Entity.Null ? userEntity : Entity.Null;
    }
}