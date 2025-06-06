// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager ����� ���� �߰�

public class GameManager : MonoBehaviour
{
    public string npcSceneName = "NPCScene"; // Inspector���� NPC �� �̸��� ������ �� �ֵ��� �մϴ�.

    // "RetryButton" Ŭ�� �� ȣ��� �Լ�
    public void RetryGame()
    {
        // ���� Ȱ��ȭ�� ���� �̸��� �����ɴϴ�.
        string currentSceneName = SceneManager.GetActiveScene().name;
        // ���� �ٽ� �ε��մϴ�.
        SceneManager.LoadScene(currentSceneName);
        // (���� ����) ���� �ð� �ٽ� �帣�� �ϱ� (���� �����־��ٸ�)
        Time.timeScale = 1f;
        Debug.Log("Retry ��ư Ŭ����: " + currentSceneName + " �� �����");
    }

    // (���� ����) �ٸ� ������ �̵��ϴ� �Լ� (��: ���� �޴�)
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuSceneName"); // ���� �޴� �� �̸����� ����
    }

    // "NPCButton" Ŭ�� �� ȣ��� �Լ�
    public void GoToNPCScene()
    {
        // �� �̸��� ���� �����ϴ� ���, Inspector���� ������ �� �ֵ��� ������ ����մϴ�.
        SceneManager.LoadScene(npcSceneName);
        Debug.Log("NPC ��ư Ŭ����: " + npcSceneName + " ������ ��ȯ");
    }

    // (���� ����) NPCButton Ŭ�� �� ȣ��� �Լ� (�ʿ信 ���� ����)
    public void OnNPCButtonClick()
    {
        Debug.Log("NPC ��ư Ŭ����");
        // ���⿡ NPC ���� ��� ���� (��: NPC ��ȭâ ����)
        // �� �Լ��� �� �̻� ������ ������, �ʿ��ϴٸ� �ٸ� ����� ���⿡ �߰��� �� �ֽ��ϴ�.
    }
}
