using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // ��ȯ�� �� �̸��� Inspector���� ������ �� �ְ� public���� ����
    public string sceneName;

    // ��ư Ŭ�� �� ȣ���� �Լ�
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("�� ��ȯ: " + sceneName);
        }
        else
        {
            Debug.LogWarning("SceneChanger: sceneName�� �����Ǿ� ���� �ʽ��ϴ�.");
        }
    }
}
