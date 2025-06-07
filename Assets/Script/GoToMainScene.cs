using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager ����� ���� �ʼ�

public class GOToMainScene : MonoBehaviour
{

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("�� �̸��� ����ְų� null�Դϴ�! �̵��� �� �����ϴ�.");
            return;
        }

        // ���� �ð��� �������� ��츦 ����� �������� �ǵ����ϴ�.
        Time.timeScale = 1f;

        // ������ �̸��� ���� �ε��մϴ�.
        SceneManager.LoadScene(sceneName);
        Debug.Log(sceneName + " ������ �̵��մϴ�.");
    }
}