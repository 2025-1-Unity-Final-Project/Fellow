using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class KeyboardChatManager : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform chatMessageArea;  // ChatMessageArea
    public GameObject userMessagePrefab;   // UserMessage 프리팹
    public GameObject aiMessagePrefab;     // AIMessage 프리팹
    public Button chatBtn;                 // ChatBtn 버튼
    public Transform characterTransform;   // 캐릭터 위치
    
    [Header("Input Field")]
    public GameObject inputFieldPanel;     // 입력창 패널
    public TMP_InputField chatInputField;  // 채팅 입력 필드
    public Button sendButton;              // 전송 버튼
    
    [Header("OpenAI Assistant API")]
    public OpenAIAssistantAPI assistantAPI;  // Assistant API 참조
    
    [Header("Live2D Systems")]
    public Live2DLipSyncManager lipSyncManager;          // 립싱크 시스템
    public UnifiedLive2DLifeSystem unifiedLifeSystem;    // 통합 생동감 시스템
    
    [Header("Settings")]
    public float messageDuration = 3f;     // 사용자 메시지 표시 시간
    public float animationSpeed = 0.3f;    // UI 애니메이션 속도
    
    private Vector2 originalInputPanelPosition;
    private GameObject currentUserMessage;
    private GameObject currentAIMessage;
    private bool isInputActive = false;
    
    void Start()
    {
        // 초기 위치 저장
        if (inputFieldPanel != null)
        {
            originalInputPanelPosition = inputFieldPanel.GetComponent<RectTransform>().anchoredPosition;
            inputFieldPanel.SetActive(false); // 처음에는 완전히 숨김
        }
        
        // 버튼 이벤트 연결
        chatBtn.onClick.AddListener(OnChatButtonClicked);
        if (sendButton != null)
            sendButton.onClick.AddListener(OnSendButtonClicked);
            
        // InputField 이벤트 연결
        if (chatInputField != null)
        {
            chatInputField.onEndEdit.AddListener(OnInputFieldEndEdit);
        }
    }
    
    void Update()
    {
        // 안드로이드에서 키보드 높이 감지
        HandleKeyboardHeight();
    }
    
    public void OnChatButtonClicked()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("ChatBtn 클릭됨 - 안드로이드 입력창 활성화");
        ShowInputField();
#else
        // 에디터에서 테스트용
        Debug.Log("ChatBtn 클릭됨 - 에디터 테스트 모드");
        string testMessage = "안녕하세요! (에디터 테스트)";
        ProcessUserMessage(testMessage);
#endif
    }
    
    void ShowInputField()
    {
        if (inputFieldPanel != null)
        {
            inputFieldPanel.SetActive(true);
            isInputActive = true;
            
            // InputField 포커스
            if (chatInputField != null)
            {
                chatInputField.Select();
                chatInputField.ActivateInputField();
            }
        }
    }
    
    void HideInputField()
    {
        if (inputFieldPanel != null)
        {
            inputFieldPanel.SetActive(false);
            isInputActive = false;
        }
    }
    
    public void OnSendButtonClicked()
    {
        SendMessage();
    }
    
    void OnInputFieldEndEdit(string text)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 안드로이드에서는 키보드 완료 버튼으로 전송
        if (!string.IsNullOrEmpty(text.Trim()))
        {
            SendMessage();
        }
#else
        // 에디터에서는 로그만
        Debug.Log("InputField 편집 종료 (에디터)");
#endif
    }
    
    void SendMessage()
    {
        if (chatInputField != null && !string.IsNullOrEmpty(chatInputField.text.Trim()))
        {
            string message = chatInputField.text.Trim();
            ProcessUserMessage(message);
            
            // 입력 필드 초기화
            chatInputField.text = "";
            HideInputField();
        }
    }
    
    void HandleKeyboardHeight()
    {
        if (!isInputActive || inputFieldPanel == null) return;
        
#if UNITY_ANDROID && !UNITY_EDITOR
        float keyboardHeight = GetKeyboardHeight_AOS();
        
        if (keyboardHeight > 0)
        {
            // 키보드 위로 InputField 이동
            AdjustInputFieldPosition(keyboardHeight);
        }
        else
        {
            // 키보드 사라짐 - 원래 위치로
            ResetInputFieldPosition();
        }
#endif
    }
    
#if UNITY_ANDROID && !UNITY_EDITOR
    private float GetKeyboardHeight_AOS()
    {
        using (AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject unityView = unityActivity.Get<AndroidJavaObject>("mUnityPlayer");
            
            using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                unityView.Call("getWindowVisibleDisplayFrame", rect);
                return Screen.height - rect.Call<int>("height");
            }
        }
    }
#endif
    
    void AdjustInputFieldPosition(float keyboardHeight)
    {
        RectTransform inputRect = inputFieldPanel.GetComponent<RectTransform>();
        Vector2 newPosition = originalInputPanelPosition;
        newPosition.y = keyboardHeight + 50f; // 키보드 위 50픽셀
        
        StartCoroutine(AnimatePosition(inputRect, newPosition));
    }
    
    void ResetInputFieldPosition()
    {
        RectTransform inputRect = inputFieldPanel.GetComponent<RectTransform>();
        StartCoroutine(AnimatePosition(inputRect, originalInputPanelPosition));
    }
    
    IEnumerator AnimatePosition(RectTransform target, Vector2 targetPosition)
    {
        Vector2 startPosition = target.anchoredPosition;
        float elapsedTime = 0f;
        
        while (elapsedTime < animationSpeed)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationSpeed;
            target.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }
        
        target.anchoredPosition = targetPosition;
    }
    
    void ProcessUserMessage(string message)
    {
        Debug.Log($"메시지 처리: {message}");
        
        // 사용자 메시지 표시
        ShowUserMessage(message);
        
        // AI 응답 생성
        StartCoroutine(GenerateAIResponse(message));
    }
    
    void ShowUserMessage(string message)
    {
        if (currentUserMessage != null)
        {
            Destroy(currentUserMessage);
        }
        
        currentUserMessage = Instantiate(userMessagePrefab, chatMessageArea);
        TextMeshProUGUI messageText = currentUserMessage.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = message;
        
        // 화면 하단에 위치
        RectTransform msgRect = currentUserMessage.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0f);
        msgRect.anchorMax = new Vector2(0.5f, 0f);
        msgRect.anchoredPosition = new Vector2(0f, 150f);
        
        Debug.Log($"사용자 메시지 표시: {message}");
        
        StartCoroutine(RemoveMessageAfterTime(currentUserMessage, messageDuration));
    }
    
    void ShowAIMessage(string message)
    {
        if (currentAIMessage != null)
        {
            Destroy(currentAIMessage);
        }

        currentAIMessage = Instantiate(aiMessagePrefab, characterTransform);
        TextMeshProUGUI messageText = currentAIMessage.GetComponentInChildren<TextMeshProUGUI>();
        
        // 타이핑 효과 시간 계산
        float typingDuration = message.Length * 0.05f;
        typingDuration = Mathf.Clamp(typingDuration, 2f, 8f);
        
        // 🎭 통합 생동감 시스템에 대화 시작 알림
        if (unifiedLifeSystem != null)
        {
            unifiedLifeSystem.SetTalkingState(true);
            Debug.Log("통합 생동감 시스템 일시정지 - 대화 시작");
        }
        
        // 🎤 립싱크 시작
        if (lipSyncManager != null)
        {
            lipSyncManager.StartLipSyncWithMessage(message, typingDuration);
            Debug.Log($"립싱크 시작: {message} (지속시간: {typingDuration}초)");
        }
        else
        {
            Debug.LogWarning("LipSyncManager가 할당되지 않았습니다!");
        }
        
        // 타이핑 효과 시작 (통합 생동감 시스템과 연동)
        StartCoroutine(TypeAIMessageWithUnifiedSystem(messageText, message, typingDuration));
        
        RectTransform msgRect = currentAIMessage.GetComponent<RectTransform>();
        msgRect.anchoredPosition = new Vector2(0f, 150f);
        
        Debug.Log($"AI 메시지 표시 시작: {message}");
    }
    
    /// <summary>
    /// 통합 생동감 시스템과 연동된 타이핑 효과
    /// </summary>
    IEnumerator TypeAIMessageWithUnifiedSystem(TextMeshProUGUI textComponent, string fullText, float duration)
    {
        textComponent.text = "";
        float timePerChar = duration / fullText.Length;
        
        for (int i = 0; i <= fullText.Length; i++)
        {
            textComponent.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(timePerChar);
        }
        
        textComponent.text = fullText;
        
        // 🎭 대화 종료 - 통합 생동감 시스템 재개 (기본 생동감은 계속 작동)
        if (unifiedLifeSystem != null)
        {
            unifiedLifeSystem.SetTalkingState(false);
            Debug.Log("대화 완료 - 통합 생동감 시스템 재개");
        }
        
        Debug.Log("AI 메시지 타이핑 완료");
    }
    
    IEnumerator RemoveMessageAfterTime(GameObject message, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (message != null)
        {
            Destroy(message);
        }
    }
    
    IEnumerator GenerateAIResponse(string userMessage)
    {
        if (assistantAPI != null)
        {
            bool responseReceived = false;
            string aiResponse = "";
            
            // 로딩 표시 (선택사항)
            Debug.Log("AI 응답 생성 중...");
            
            assistantAPI.SendMessage(userMessage, 
                (response) => {
                    aiResponse = response;
                    responseReceived = true;
                    Debug.Log($"AI 응답 받음: {response}");
                },
                (error) => {
                    Debug.LogError($"Assistant API 오류: {error}");
                    aiResponse = "죄송합니다. 응답을 생성할 수 없습니다.";
                    responseReceived = true;
                }
            );
            
            // 응답 대기
            yield return new WaitUntil(() => responseReceived);
            
            ShowAIMessage(aiResponse);
        }
        else
        {
            Debug.LogError("Assistant API가 할당되지 않았습니다!");
            yield return new WaitForSeconds(1f);
            ShowAIMessage("Assistant API가 연결되지 않았습니다.");
        }
    }
}