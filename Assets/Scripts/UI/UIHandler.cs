using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Button start;
    [SerializeField] private Button resume;
    [SerializeField] private Button restart;
    [SerializeField] private Button exit1;
    [SerializeField] private Button exit2;
    [SerializeField] private Button exit3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start.onClick.RemoveAllListeners();
        resume.onClick.RemoveAllListeners();
        restart.onClick.RemoveAllListeners();
        exit1.onClick.RemoveAllListeners();
        exit2.onClick.RemoveAllListeners();
        exit3.onClick.RemoveAllListeners();

        start.onClick.AddListener(() =>
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
        });

        resume.onClick.AddListener(() =>
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        });

        restart.onClick.AddListener(() =>
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        });

        exit1.onClick.AddListener(() =>
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        });

        exit2.onClick.AddListener(() =>
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        });

        exit3.onClick.AddListener(() =>
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
