using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool firstLaunch = true;

    private enum GameState
    {
        StartMenu,
        Playing,
        Paused,
        GameOver
    }

    private GameState currentState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeGame();
    }
    private void InitializeGame()
    {
        if (firstLaunch)
        {
            SetState(GameState.StartMenu);
            firstLaunch = false;
        }
        else
        {
            SetState(GameState.Playing);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }
    private void SetState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.StartMenu:
                Time.timeScale = 0f;
                UIManager.Instance.ShowStartPanel();
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                UIManager.Instance.HideAllPanels();
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                UIManager.Instance.ShowPausePanel();
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                UIManager.Instance.ShowGameOverPanel();
                break;
        }
    }
    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void PauseGame()
    {
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        SetState(GameState.Playing);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
