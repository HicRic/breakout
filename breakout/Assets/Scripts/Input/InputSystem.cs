using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerInput input = null;

    private Entity localPlayerInput;
    private Vector2 lastInputPoint;

    void Start()
    {
        localPlayerInput = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(InputData), typeof(Translation));
    }

    public void HandlePoint(InputAction.CallbackContext context)
    {
        lastInputPoint = context.ReadValue<Vector2>();
    }

    void Update()
    {
        Vector2 normalizedScreenPos = new Vector2(lastInputPoint.x / input.camera.pixelWidth, lastInputPoint.y / input.camera.pixelHeight);
        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(localPlayerInput, new InputData
        {
            ScreenPos = normalizedScreenPos
        });

        Vector3 worldPos = input.camera.ScreenToWorldPoint(lastInputPoint);
        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(localPlayerInput, new Translation
        {
            Value = worldPos
        });
    }
}
