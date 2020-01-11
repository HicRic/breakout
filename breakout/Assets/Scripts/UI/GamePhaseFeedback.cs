using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class GamePhaseFeedback : MonoBehaviour
{
    [SerializeField] private GameObject WinObject = null;
    [SerializeField] private GameObject LoseObject = null;
    [SerializeField] private Button RetryButton = null;

    private EntityQuery gameStateQuery;
    private GameState currentVisualGameState;

    void Awake()
    {
        if (WinObject)
        {
            WinObject.SetActive(false);
        }

        if (LoseObject)
        {
            LoseObject.SetActive(false);
        }

        if (RetryButton)
        {
            RetryButton.gameObject.SetActive(false);
            RetryButton.onClick.AddListener(HandleRetryClick);
        }
    }

    private void HandleRetryClick()
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(ResetLevelCommand));
        RetryButton.gameObject.SetActive(false);
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
            LoseObject.SetActive(currentVisualGameState.CurrentPhase == GameState.Phase.Lost);

            bool showRetry = currentVisualGameState.CurrentPhase == GameState.Phase.Lost ||
                             currentVisualGameState.CurrentPhase == GameState.Phase.Won;

            RetryButton.gameObject.SetActive(showRetry);
        }
    }
}

