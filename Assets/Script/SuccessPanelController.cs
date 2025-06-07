using UnityEngine;
using UnityEngine.EventSystems; // EventSystem ����� ���� �߰�

public class SuccessPanelController : MonoBehaviour
{
    void OnEnable() // SuccessPanel�� Ȱ��ȭ�� �� ȣ��
    {
        // EventSystem�� �ִ��� Ȯ���ϰ�, ���ٸ� ���� ����
        if (EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Debug.LogWarning("EventSystem�� ���� ���� �����߽��ϴ�.");
        }
        else
        {
            Debug.Log("EventSystem�� �̹� �����մϴ�.");
        }
       
    }
}
