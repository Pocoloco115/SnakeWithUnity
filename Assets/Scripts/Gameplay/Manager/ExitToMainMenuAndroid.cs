using UnityEngine;

public class ExitToMainMenuAndroid : MonoBehaviour
{
    public void ExitToMenu()
    {
        // Este log aparecerá en el Logcat de Android Studio
        Debug.Log(">>> UNITY: Intentando llamar a returnToPreviousScreen..."); 
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        try {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                jo.Call("returnToPreviousScreen");
            }
        } catch (System.Exception e) {
            Debug.LogError(">>> UNITY ERROR: " + e.Message);
            // Quita el Application.Quit() temporalmente para que no mate la app si falla
        }
        #endif
    }
}
