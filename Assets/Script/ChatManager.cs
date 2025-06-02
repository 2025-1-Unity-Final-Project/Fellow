using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private RectTransform chatRectTransform;
    [SerializeField] private float animationDuration = 0.3f;

    private Vector3 hiddenPosition;
    private Vector3 showPosition;
    private bool isVisible = false;

    void Start()
    {
        // 초기 위치 설정 (화면 밖으로 숨김)
        hiddenPosition = new Vector3(0, -Screen.height, 0);
        showPosition = Vector3.zero;

        chatRectTransform.anchoredPosition = hiddenPosition;
        chatPanel.SetActive(false);
    }
    
    // DOTween 없이 Coroutine 사용
private void ShowChatPanel()
{
    chatPanel.SetActive(true);
    isVisible = true;
    StartCoroutine(AnimateToPosition(showPosition));
}

private void HideChatPanel()
{
    isVisible = false;
    StartCoroutine(AnimateToPosition(hiddenPosition, true));
}

private System.Collections.IEnumerator AnimateToPosition(Vector2 targetPos, bool hideOnComplete = false)
{
    Vector2 startPos = chatRectTransform.anchoredPosition;
    float elapsed = 0f;
    
    while (elapsed < animationDuration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / animationDuration;
        t = Mathf.SmoothStep(0f, 1f, t);
        
        chatRectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
        yield return null;
    }
    
    chatRectTransform.anchoredPosition = targetPos;
    
    if (hideOnComplete)
        chatPanel.SetActive(false);
}
}

