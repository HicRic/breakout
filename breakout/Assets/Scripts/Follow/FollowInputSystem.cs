using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

class FollowInputSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entity inputEntity = Entity.Null;

        // todo could imagine combining all input data here, or accounting for multiple players.
        // For now, just one local player is fine.
        Entities
            .WithAll<InputData>()
            .ForEach((Entity entity) =>
            {
                inputEntity = entity;
            });

        if (inputEntity == Entity.Null)
        {
            return;
        }

        Entities
            .WithAll<FollowInputDataTag>()
            .WithNone<FollowTarget>()
            .ForEach((Entity entity) =>
            {
                PostUpdateCommands.AddComponent(entity, new FollowTarget
                {
                    Value = inputEntity
                });
            });
    }
}

