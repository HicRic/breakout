using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[ConverterVersion("ballconvert", 1)]
public class BallAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [SerializeField] private GameObject BallPrefab = null;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var ballConfig = new BallConfig
        {
            Prefab = conversionSystem.GetPrimaryEntity(BallPrefab)
        };

        dstManager.AddComponentData(entity, ballConfig);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BallPrefab);
    }
}

