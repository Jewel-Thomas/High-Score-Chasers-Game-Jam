using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        GAME_RUNNING,
        GAME_OVER
    }

    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; }


    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentGameState = GameState.GAME_RUNNING;
    }

    public void ChangeGameState(GameState gameState)
    {
        CurrentGameState = gameState;
    }

    public void SetGameOver()
    {
        CurrentGameState = GameState.GAME_OVER;
        UIManager.Instance.SetGameOverUI(true);
    }
}
