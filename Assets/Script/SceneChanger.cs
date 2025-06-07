using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 전환할 씬 이름을 Inspector에서 지정할 수 있게 public으로 선언
    public string sceneName;

    // 버튼 클릭 시 호출할 함수
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("씬 전환: " + sceneName);
        }
        else
        {
            Debug.LogWarning("SceneChanger: sceneName이 설정되어 있지 않습니다.");
        }
    }
}
