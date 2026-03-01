using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    private bool _isPaused = false;
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _isPaused = Time.timeScale == 0f;
    }
    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
    }
    public void ShowGameOverPanel()
    {
        startPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        pausePanel.SetActive(false);
    }
    public void ShowPausePanel()
    {
        pausePanel.SetActive(true);
        gameOverPanel.SetActive(false);
        startPanel.SetActive(false);
    }
    public void HideAllPanels()
    {
        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
    }
}
