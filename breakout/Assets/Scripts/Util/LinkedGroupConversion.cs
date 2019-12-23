using Unity.Entities;
using UnityEngine;

public class LinkedGroupConversion : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        conversionSystem.DeclareLinkedEntityGroup(gameObject);
    }
}