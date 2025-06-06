using UnityEngine;
using UnityEngine.EventSystems; // EventSystem 사용을 위해 추가

public class SuccessPanelController : MonoBehaviour
{
    void OnEnable() // SuccessPanel이 활성화될 때 호출
    {
        // EventSystem이 있는지 확인하고, 없다면 새로 생성
        if (EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Debug.LogWarning("EventSystem이 없어 새로 생성했습니다.");
        }
        else
        {
            Debug.Log("EventSystem이 이미 존재합니다.");
        }
       
    }
}
