using UnityEngine;

public class ForceSquareWindow : MonoBehaviour
{
    [SerializeField] private int squareSize = 800;          
    [SerializeField] private bool centerWindow = true;      

    void Start()
    {
        Screen.fullScreen = false;
        Screen.SetResolution(squareSize, squareSize, false);

        if (centerWindow)
        {
            var res = Screen.currentResolution;
            int x = (res.width - squareSize) / 2;
            int y = (res.height - squareSize) / 2;
        }
    }

    void Update()
    {
        if (!Screen.fullScreen)
        {
            if (Screen.width != Screen.height)
            {
                int newSize = Mathf.Min(Screen.width, Screen.height);
                Screen.SetResolution(newSize, newSize, false);
            }
        }
    }
}
