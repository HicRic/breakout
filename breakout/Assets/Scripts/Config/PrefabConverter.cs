using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[ConverterVersion("PrefabConverter", 1)]
public class PrefabConverter : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [SerializeField] private Config config = null;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var ballConfig = new BallConfig
        {
            Prefab = conversionSystem.GetPrimaryEntity(config.BallPrefab)
        };

        dstManager.AddComponentData(entity, ballConfig);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(config.BallPrefab);
    }
}

