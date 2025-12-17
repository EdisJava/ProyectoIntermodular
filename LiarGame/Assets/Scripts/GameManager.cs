using UnityEngine;

public enum GameState
{
    Exploration3D,
    Interaction2D
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentState;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        currentState = GameState.Exploration3D;
    }

    public bool IsIn2D()
    {
        return currentState == GameState.Interaction2D;
    }
}