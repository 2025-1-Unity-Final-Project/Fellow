using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("Scene Changer: " + sceneName);
        }
        else
        {
            Debug.LogWarning("Scene Changer: sceneName is not set.");
        }
    }
}
