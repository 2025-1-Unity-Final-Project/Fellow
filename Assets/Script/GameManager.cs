// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 추가

public class GameManager : MonoBehaviour
{
    public string npcSceneName = "NPCScene"; // Inspector에서 NPC 씬 이름을 설정할 수 있도록 합니다.

    // "RetryButton" 클릭 시 호출될 함수
    public void RetryGame()
    {
        // 현재 활성화된 씬의 이름을 가져옵니다.
        string currentSceneName = SceneManager.GetActiveScene().name;
        // 씬을 다시 로드합니다.
        SceneManager.LoadScene(currentSceneName);
        // (선택 사항) 게임 시간 다시 흐르게 하기 (만약 멈춰있었다면)
        Time.timeScale = 1f;
        Debug.Log("Retry 버튼 클릭됨: " + currentSceneName + " 씬 재시작");
    }

    // (선택 사항) 다른 씬으로 이동하는 함수 (예: 메인 메뉴)
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuSceneName"); // 메인 메뉴 씬 이름으로 변경
    }

    // "NPCButton" 클릭 시 호출될 함수
    public void GoToNPCScene()
    {
        // 씬 이름을 직접 지정하는 대신, Inspector에서 설정할 수 있도록 변수를 사용합니다.
        SceneManager.LoadScene(npcSceneName);
        Debug.Log("NPC 버튼 클릭됨: " + npcSceneName + " 씬으로 전환");
    }

    // (선택 사항) NPCButton 클릭 시 호출될 함수 (필요에 따라 구현)
    public void OnNPCButtonClick()
    {
        Debug.Log("NPC 버튼 클릭됨");
        // 여기에 NPC 관련 기능 구현 (예: NPC 대화창 열기)
        // 이 함수는 더 이상 사용되지 않지만, 필요하다면 다른 기능을 여기에 추가할 수 있습니다.
    }
}
