using System;
using Unity.Entities;
using UnityEngine;

public class GamePhaseFeedback : MonoBehaviour
{
    [SerializeField] private GameObject WinObject = null;

    private EntityQuery gameStateQuery;
    private GameState currentVisualGameState;

    void Awake()
    {
        if (WinObject)
        {
            WinObject.SetActive(false);
        }
    }

    void Start()
    {
        gameStateQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GameState));
    }

    void OnDestroy()
    {
        gameStateQuery?.Dispose();
    }

    void Update()
    {
        GameState state = gameStateQuery.GetSingleton<GameState>();
        ChangeState(state);
    }

    private void ChangeState(GameState state)
    {
        if (state.CurrentPhase != currentVisualGameState.CurrentPhase)
        {
            currentVisualGameState = state;
            WinObject.SetActive(currentVisualGameState.CurrentPhase == GameState.Phase.Won);
        }
    }
}

