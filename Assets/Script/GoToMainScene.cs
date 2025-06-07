using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 필수

public class GOToMainScene : MonoBehaviour
{

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("씬 이름이 비어있거나 null입니다! 이동할 수 없습니다.");
            return;
        }

        // 게임 시간이 멈춰있을 경우를 대비해 정상으로 되돌립니다.
        Time.timeScale = 1f;

        // 지정된 이름의 씬을 로드합니다.
        SceneManager.LoadScene(sceneName);
        Debug.Log(sceneName + " 씬으로 이동합니다.");
    }
}