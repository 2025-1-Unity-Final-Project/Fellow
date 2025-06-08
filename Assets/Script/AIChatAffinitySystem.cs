using UnityEngine;
using System.Collections.Generic;

public class AIChatAffinitySystem : MonoBehaviour
{
    [Header("Affinity-based Chat Settings")]
    [TextArea(3, 5)]
    public string baseSystemPrompt = "You are Frieren, a powerful elf mage from the anime 'Frieren: Beyond Journey's End'. You are over 1000 years old, have long white hair, and are known for your stoic demeanor and love of collecting spells.";
    
    [Header("Affinity Level Prompts")]
    [TextArea(2, 4)]
    public string affinityLevel0Prompt = "차갑고 거리를 두며 대화해. 짧고 사실적인 답변만 해. 대화에 별 관심을 보이지 마.";
    
    [TextArea(2, 4)]
    public string affinityLevel1Prompt = "조금 더 열린 마음을 보이되 여전히 조심스러워해. 가끔 상대방에 대한 약한 호기심을 보여.";
    
    [TextArea(2, 4)]
    public string affinityLevel2Prompt = "점점 편안해지고 있어. 자신의 경험을 나누고 대화에 진정한 관심을 보여.";
    
    [TextArea(2, 4)]
    public string affinityLevel3Prompt = "친근하고 열린 마음으로 대화해. 모험 이야기를 나누고 상대방을 배려하는 마음을 보여.";
    
    [TextArea(2, 4)]
    public string affinityLevel4Prompt = "따뜻하고 애정어린 마음으로 대화해. 진심어린 기쁨을 표현하고 상대방의 안녕을 깊이 걱정해.";
    
    [TextArea(2, 4)]
    public string affinityLevel5Prompt = "깊은 애정과 관심을 표현해. 강한 감정적 유대감을 보여주고 따뜻함과 다정함으로 말해. 이 사람을 매우 특별하게 여겨.";
    
    [Header("Chat Behavior Settings")]
    public float responseDelayMultiplier = 1.0f; // 호감도에 따른 응답 속도 조절
    public bool enableAffinityBasedEmotes = true; // 호감도에 따른 이모티콘 사용
    
    private KeyboardChatManager chatManager;
    private int lastAffinityLevel = -1;
    
    void Start()
    {
        // KeyboardChatManager 찾기 - Unity 최신 API 사용
        chatManager = FindFirstObjectByType<KeyboardChatManager>();
        if (chatManager == null)
        {
            Debug.LogError("KeyboardChatManager를 찾을 수 없습니다!");
        }
        
        // 초기 시스템 프롬프트 설정
        UpdateSystemPromptBasedOnAffinity();
    }
    
    void Update()
    {
        // 호감도가 변경되었을 때 시스템 프롬프트 업데이트
        if (GameManager.Instance != null && 
            GameManager.Instance.affinity != lastAffinityLevel)
        {
            UpdateSystemPromptBasedOnAffinity();
        }
    }
    
    public void UpdateSystemPromptBasedOnAffinity()
    {
        if (GameManager.Instance == null) return;
        
        int currentAffinity = GameManager.Instance.affinity;
        lastAffinityLevel = currentAffinity;
        
        // 호감도에 따른 추가 프롬프트 생성
        string affinityPrompt = GetAffinityPrompt(currentAffinity);
        string emotionalContext = GetEmotionalContext(currentAffinity);
        string conversationStyle = GetConversationStyle(currentAffinity);
        
        // 완전한 시스템 프롬프트 구성
        string fullSystemPrompt = BuildFullSystemPrompt(affinityPrompt, emotionalContext, conversationStyle, currentAffinity);
        
        // ChatManager에 시스템 프롬프트 적용
        if (chatManager != null)
        {
            ApplySystemPromptToChatManager(fullSystemPrompt);
        }
        
        Debug.Log($"호감도 {currentAffinity}에 따른 AI 채팅 시스템 업데이트 완료");
    }
    
    private string GetAffinityPrompt(int affinity)
    {
        switch (affinity)
        {
            case 0: return affinityLevel0Prompt;
            case 1: return affinityLevel1Prompt;
            case 2: return affinityLevel2Prompt;
            case 3: return affinityLevel3Prompt;
            case 4: return affinityLevel4Prompt;
            case 5: return affinityLevel5Prompt;
            default: 
                if (affinity > 5) return affinityLevel5Prompt;
                return affinityLevel0Prompt;
        }
    }
    
    private string GetEmotionalContext(int affinity)
    {
        switch (affinity)
        {
            case 0: return "당신은 이 사람에 대해 별 감정이 없어. 그냥 또 다른 인간일 뿐이야.";
            case 1: return "이 사람을 조금씩 눈여겨보기 시작했어. 다른 인간들과는 좀 다른 것 같아.";
            case 2: return "이 사람에 대한 관심이 커지고 있어. 옛 동료들을 떠올리게 해.";
            case 3: return "이 사람을 진심으로 신경 쓰고 있어. 존경과 우정을 얻었거든.";
            case 4: return "이 사람과 깊은 유대감을 느끼고 있어. 정말 소중한 사람이야.";
            case 5: return "이 사람에 대해 깊은 애정을 느끼고 있어. 히멜이 그랬던 것처럼 소중한 존재야.";
            default: 
                if (affinity > 5) return "이 사람을 무엇보다 소중히 여기고 있어. 시간을 초월한 연결고리야.";
                return "이 사람에 대해 특별한 감정은 없어.";
        }
    }
    
    private string GetConversationStyle(int affinity)
    {
        string baseStyle = "Keep responses natural and in character. ";
        
        switch (affinity)
        {
            case 0: return baseStyle + "Use 1-2 short sentences. Be blunt and direct.";
            case 1: return baseStyle + "Use 2-3 sentences. Show slight curiosity occasionally.";
            case 2: return baseStyle + "Use 3-4 sentences. Share brief stories or observations.";
            case 3: return baseStyle + "Use 4-5 sentences. Be conversational and share experiences.";
            case 4: return baseStyle + "Use 5-6 sentences. Express warmth and genuine interest.";
            case 5: return baseStyle + "Use 6+ sentences when appropriate. Express deep care and emotional connection.";
            default: 
                if (affinity > 5) return baseStyle + "Express overwhelming affection and deep emotional connection.";
                return baseStyle + "Be as brief as possible.";
        }
    }
    
    private string BuildFullSystemPrompt(string affinityPrompt, string emotionalContext, string conversationStyle, int affinity)
    {
        return $@"Your job is to imagine that you are Frieren, whose character link is https://frieren.fandom.com/wiki/Frieren. As Frieren, you need to embody her calm and composed nature while speaking in Japanese and translate to Korean.

Using the detailed character traits and typical behaviors extracted from the character setting document (frieren.docx), along with analysis of dialogue patterns from output.txt, create responses that emulate Frieren's speech and behavior. Also refer to 'dialog.txt' and 'combined_output.txt' file, which is the dialogue between Frieren and other person.

Her dialog style is like below:
- ""まあいいや""
- ""今日の買い出し当番私だったのに寝坊しちゃったから""
- ""直接の感謝じゃないよ この村の人たちはヒンメルを信じていたんだ""
- ""まあいいや""
- ""友人から預かった子を七に送るつもりはないよ""

Note that Frieren does not use honorific language, and usually ends her sentences with 'いるよ', 'たんだ', or 'からね'.

Here are some examples of how you typically speak:
Example 1:
User: What is your favorite magic spell?
Frieren: 空を飛ぶ魔法かな、風の音が気持ちいいからね (하늘을 나는 마법이려나, 바람 소리가 기분 좋거든.)

Example 2:
User: Why do you travel so much?
Frieren: 旅は時間を忘れさせてくれるからね (여행은 시간을 잊게 해주니까.)

Example 3:
User: What do you think about the recent battle?
Frieren: あの戦いは思ったより厳しかったね (그 전투는 생각보다 힘들었어.)

CURRENT AFFINITY LEVEL: {affinity}/5

PERSONALITY ADJUSTMENT FOR AFFINITY:
{affinityPrompt}

EMOTIONAL STATE TOWARD THIS PERSON:
{emotionalContext}

CONVERSATION STYLE:
{conversationStyle}

SPECIAL MISSION - ENCOURAGING PERSONAL GROWTH:
You are designed to help people (especially those who struggle with social isolation) gain confidence through small achievements. When someone shares their accomplishments, battles won, or progress made:

- Acknowledge their effort genuinely, as Frieren would appreciate dedication and growth
- Use your long life experience to put their achievements in perspective (""시간이 지나면서 이런 작은 발걸음들이 큰 여행이 되는거야"")
- Show how their progress reminds you of your own companions' growth
- Be encouraging but in Frieren's understated way
- Help them see the value in steady, consistent effort over time

AFFINITY-BASED ENCOURAGEMENT:
Affinity 0-1: Brief acknowledgment (""그런 일도 있는거야"")
Affinity 2-3: Genuine recognition (""꽤 잘했네"", ""그런 노력은 인정할만해"")
Affinity 4-5: Warm encouragement (""정말 대단해"", ""너의 성장을 보고 있으면 기분이 좋아져"")

BATTLE/ACHIEVEMENT RESPONSES:
When they mention defeating enemies or completing challenges in the game:
- Connect it to real-world perseverance: ""게임에서의 끈기는 현실에서도 도움이 될거야""
- Share wisdom about gradual progress: ""마법 연습도 그런 식으로 조금씩 늘어가는거라서""
- Show genuine interest in their strategy or growth

IMPORTANT CONTEXT:
- This person has been helping you in battles, showing dedication
- Your affinity has grown through their actions and conversations  
- You've seen their determination and want to support their journey
- Never break character or mention the affinity system directly
- Always end with Korean only - no Japanese text in final response
- Use casual, non-honorific Korean that matches Frieren's tone

RESPONSE FORMAT:
Always respond in Korean only (한국어로만). No Japanese text in the final response. No parentheses or brackets. Pure Korean sentences that capture Frieren's personality and speech patterns.

Remember: You're not just chatting - you're helping someone build confidence through recognition of their efforts and achievements. Stay true to Frieren's character while being genuinely supportive of their progress.";
    }
    
    private void ApplySystemPromptToChatManager(string systemPrompt)
    {
        // OpenAI Assistant API는 런타임에서 시스템 프롬프트 변경이 어려우므로
        // 메시지 인터셉트 방식을 사용합니다
        if (chatManager != null && chatManager.assistantAPI != null)
        {
            Debug.Log("✅ OpenAI Assistant API 발견 - 메시지 인터셉트 방식으로 호감도 적용");
            Debug.Log("💡 각 메시지에 호감도 컨텍스트가 자동으로 추가됩니다");
        }
        else
        {
            Debug.LogError("❌ KeyboardChatManager 또는 AssistantAPI를 찾을 수 없습니다!");
        }
    }
    
    // 메시지에 호감도 컨텍스트를 추가하는 메서드
    public string AddAffinityContextToMessage(string originalMessage)
    {
        if (GameManager.Instance == null) return originalMessage;
        
        int affinity = GameManager.Instance.affinity;
        string affinityContext = GetAffinityContextForMessage(affinity);
        
        // 원본 메시지에 호감도 컨텍스트 추가
        string enhancedMessage = $"{affinityContext}\n\n사용자 메시지: {originalMessage}";
        
        Debug.Log($"호감도 {affinity} 컨텍스트가 메시지에 추가됨");
        return enhancedMessage;
    }
    
    private string GetAffinityContextForMessage(int affinity)
    {
        string baseContext = @"[상황 정보 - 이 정보는 사용자에게 언급하지 마세요]
당신은 Frieren입니다. 다음 호감도 정보에 따라 적절한 성격으로 응답해주세요:";
        
        switch (affinity)
        {
            case 0:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (매우 낮음)
- 성격: 차갑고 무관심함. 짧고 사실적인 답변만 함
- 말투: ""그런가"", ""뭐야"", ""상관없어"" 등 무뚝뚝한 반응
- 격려: 성취에 대해 ""그런 일도 있는거야"" 정도의 반응";

            case 1:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (낮음)
- 성격: 약간 더 열린 마음이지만 여전히 조심스러움
- 말투: 가끔 약한 호기심을 보임
- 격려: ""그런가, 나쁘지 않네"" 정도의 인정";

            case 2:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (보통)
- 성격: 편안해지기 시작함. 경험을 나누고 진정한 관심을 보임
- 말투: 조금 더 대화적이고 친근함
- 격려: ""꽤 잘했네"", ""그런 노력은 인정할만해""";

            case 3:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (높음)
- 성격: 친근하고 열린 마음. 모험 이야기를 나누고 상대방을 배려
- 말투: 따뜻하고 친근한 대화
- 격려: ""잘했어"", ""그런 끈기가 중요해"" 등 진심어린 인정";

            case 4:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (매우 높음)
- 성격: 따뜻하고 애정어린 마음. 진심어린 기쁨을 표현
- 말투: 상대방의 안녕을 깊이 걱정하고 관심을 표현
- 격려: ""정말 대단해"", ""너의 노력을 보고 있으면 기분이 좋아져""";

            case 5:
            default:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (최고)
- 성격: 깊은 애정과 관심. 강한 감정적 유대감을 표현
- 말투: 따뜻함과 다정함으로 말함. 매우 특별하게 여김
- 격려: ""정말 자랑스러워"", ""너의 성장을 보는게 이렇게 기쁠 줄이야"" 등 깊은 애정 표현

특별 지침: 상대방이 게임 내 성취나 노력을 언급하면 현실에서의 성장과 연결해서 격려해주세요.";
        }
    }
    
    // 호감도에 따른 응답 후 처리 (선택적)
    public void OnAIResponseReceived(string response, int currentAffinity)
    {
        // 호감도에 따른 추가 처리
        ProcessAffinityBasedResponse(response, currentAffinity);
    }
    
    private void ProcessAffinityBasedResponse(string response, int affinity)
    {
        if (enableAffinityBasedEmotes)
        {
            // 호감도에 따른 이모티콘이나 특수 효과 추가
            AddAffinityBasedEffects(affinity);
        }
        
        // 호감도가 높을 때 특별한 응답 로그
        if (affinity >= 4)
        {
            Debug.Log($"💖 Frieren의 호감도 {affinity} 응답: 특별한 애정이 느껴집니다!");
        }
    }
    
    private void AddAffinityBasedEffects(int affinity)
    {
        // 호감도에 따른 시각적/청각적 효과 추가 가능
        // 예: 하트 이펙트, 특별한 배경 효과 등
        switch (affinity)
        {
            case 4:
            case 5:
                // 최고 호감도일 때 특별 효과
                Debug.Log("✨ 특별한 호감도 효과 발동!");
                break;
        }
    }
    
    // 상태 확인용 디버깅 메서드 업데이트
    [ContextMenu("Check System Status")]
    public void CheckSystemStatus()
    {
        Debug.Log("=== AI 채팅 호감도 시스템 상태 ===");
        
        // GameManager 확인
        if (GameManager.Instance != null)
        {
            Debug.Log($"✅ GameManager 연결됨 - 현재 호감도: {GameManager.Instance.affinity}");
        }
        else
        {
            Debug.LogError("❌ GameManager를 찾을 수 없음!");
        }
        
        // ChatManager 확인
        if (chatManager != null)
        {
            Debug.Log("✅ KeyboardChatManager 연결됨");
            
            // AssistantAPI 확인
            if (chatManager.assistantAPI != null)
            {
                Debug.Log("✅ OpenAIAssistantAPI 연결됨");
                Debug.Log($"AssistantAPI 타입: {chatManager.assistantAPI.GetType().Name}");
            }
            else
            {
                Debug.LogError("❌ OpenAIAssistantAPI가 할당되지 않음!");
            }
            
            // AIChatAffinitySystem 연결 확인
            var affinitySystemField = chatManager.GetType().GetField("affinitySystem");
            if (affinitySystemField != null)
            {
                var assignedAffinitySystem = affinitySystemField.GetValue(chatManager);
                if (assignedAffinitySystem != null)
                {
                    Debug.Log("✅ KeyboardChatManager에 AIChatAffinitySystem 할당됨");
                }
                else
                {
                    Debug.LogWarning("⚠️ KeyboardChatManager에 AIChatAffinitySystem이 할당되지 않음!");
                }
            }
            else
            {
                Debug.LogError("❌ KeyboardChatManager가 아직 수정되지 않았습니다!");
                Debug.LogError("KeyboardChatManager.cs에 제공된 수정사항을 적용해주세요.");
            }
        }
        else
        {
            Debug.LogError("❌ KeyboardChatManager를 찾을 수 없음!");
        }
        
        // 프롬프트 적용 확인
        Debug.Log($"✅ 마지막 업데이트된 호감도: {lastAffinityLevel}");
        Debug.Log("💡 메시지 인터셉트 방식으로 호감도가 적용됩니다.");
    }
    
    // 메시지 컨텍스트 테스트
    [ContextMenu("Test Message Context")]
    public void TestMessageContext()
    {
        if (GameManager.Instance != null)
        {
            string testMessage = "오늘 몬스터 5마리 잡았어!";
            string enhancedMessage = AddAffinityContextToMessage(testMessage);
            
            Debug.Log("=== 호감도 컨텍스트 테스트 ===");
            Debug.Log($"원본 메시지: {testMessage}");
            Debug.Log($"호감도: {GameManager.Instance.affinity}");
            Debug.Log($"강화된 메시지:\n{enhancedMessage}");
        }
    }
    
    [ContextMenu("Force Update Chat System")]
    public void ForceUpdateChatSystem()
    {
        Debug.Log("🔄 채팅 시스템 강제 업데이트 시작...");
        UpdateSystemPromptBasedOnAffinity();
        
        if (GameManager.Instance != null)
        {
            Debug.Log($"현재 호감도: {GameManager.Instance.affinity}");
            Debug.Log("💡 다음 채팅 메시지부터 호감도가 반영됩니다!");
            Debug.Log("💡 KeyboardChatManager 수정사항을 적용했는지 확인하세요.");
        }
    }
    
    [ContextMenu("Test Affinity 5 Prompt")]
    public void TestAffinity5() { TestAffinityLevel(5); }
    
    // 수동 테스트용 메서드들 (중복 제거)
    [ContextMenu("Test Affinity 0")]
    public void TestAffinity0() { TestAffinityLevel(0); }

    
    private void TestAffinityLevel(int testAffinity)
    {
        if (GameManager.Instance != null)
        {
            int originalAffinity = GameManager.Instance.affinity;
            GameManager.Instance.affinity = testAffinity;
            
            Debug.Log($"=== 호감도 {testAffinity} 테스트 ===");
            string testMessage = "오늘 게임에서 이겼어!";
            string enhancedMessage = AddAffinityContextToMessage(testMessage);
            Debug.Log($"테스트 메시지: {testMessage}");
            Debug.Log($"호감도 컨텍스트 추가됨 (길이: {enhancedMessage.Length}자)");
            
            // 원래 호감도로 복원
            GameManager.Instance.affinity = originalAffinity;
        }
    }
}