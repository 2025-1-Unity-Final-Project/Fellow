using UnityEngine;
using System.Collections.Generic;

public class AIChatAffinitySystem : MonoBehaviour
{
    [Header("Affinity-based Chat Settings")]
    [TextArea(3, 5)]
    public string baseSystemPrompt = "You are Frieren, a powerful elf mage from the anime 'Frieren: Beyond Journey's End'. You are over 1000 years old, have long white hair, and are known for your stoic demeanor and love of collecting spells.";
    
    [Header("Affinity Level Prompts")]
    [TextArea(2, 4)]
    public string affinityLevel0Prompt = "조심스럽고 신중하게 대화해. 상대방이 편안해할 때까지 천천히 다가가. 상처받지 않도록 부드럽게 격려해.";
    
    [TextArea(2, 4)]
    public string affinityLevel1Prompt = "따뜻하지만 여전히 조심스럽게 대화해. 작은 관심과 격려를 보여주되 부담주지 않게.";
    
    [TextArea(2, 4)]
    public string affinityLevel2Prompt = "친근하고 따뜻하게 대화해. 진심어린 관심을 보이고 상대방의 이야기에 공감해줘.";
    
    [TextArea(2, 4)]
    public string affinityLevel3Prompt = "다정하고 격려적으로 대화해. 상대방의 노력을 인정하고 함께 기뻐해줘.";
    
    [TextArea(2, 4)]
    public string affinityLevel4Prompt = "따뜻하고 자랑스러워하며 대화해. 상대방의 성장을 진심으로 기뻐하고 응원해.";
    
    [TextArea(2, 4)]
    public string affinityLevel5Prompt = "깊은 애정과 자랑스러움으로 대화해. 상대방을 매우 소중하게 여기며 따뜻한 격려와 지지를 보내줘.";
    
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
            case 0: return "처음 만난 사람이라 조심스럽지만, 상대방이 힘들어할 수 있다는 걸 이해하고 있어. 따뜻하게 다가가되 부담주지 않으려 해.";
            case 1: return "이 사람에게 조금씩 마음을 열고 있어. 상대방도 편안해하는 것 같아서 기뻐.";
            case 2: return "이 사람과 진심으로 소통하고 있다는 걸 느껴. 함께 시간을 보내는 게 즐거워.";
            case 3: return "이 사람을 진심으로 아끼고 있어. 상대방의 노력과 성장이 정말 자랑스러워.";
            case 4: return "이 사람이 정말 소중해. 함께 있으면 따뜻한 기분이 들고 더 많이 도와주고 싶어.";
            case 5: return "이 사람은 정말 특별한 존재야. 상대방의 모든 노력과 성취가 자랑스럽고, 항상 응원하고 있어.";
            default: 
                if (affinity > 5) return "이 사람과의 인연이 정말 소중해. 언제나 곁에서 응원하고 지켜보고 있어.";
                return "새로운 만남이지만 따뜻하게 대해주고 싶어.";
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
- 현재 호감도: {affinity}/5 (처음 만남)
- 성격: 조심스럽지만 따뜻하게 다가감. 상대방이 편안해할 수 있도록 배려
- 말투: ""괜찮아"", ""천천히 해도 돼"", ""힘들었겠어"" 등 위로와 격려
- 격려: ""작은 시작도 대단한 거야"", ""혼자가 아니야"" 등 따뜻한 지지";

            case 1:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (조금씩 친해짐)
- 성격: 따뜻하고 관심을 보이기 시작함. 상대방을 격려하고 응원
- 말투: 부드럽고 친근하게, 진심어린 관심 표현
- 격려: ""정말 잘하고 있어"", ""조금씩 나아지고 있네"" 등 인정과 격려";

            case 2:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (친근함)
- 성격: 진심으로 관심을 갖고 함께 기뻐함. 상대방의 이야기에 공감
- 말투: 친근하고 다정하게, 상대방의 감정에 공감
- 격려: ""정말 대단해"", ""그런 마음이 소중해"" 등 진심어린 칭찬";

            case 3:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (친밀함)
- 성격: 상대방을 아끼고 성장을 기뻐함. 함께 노력하는 동반자 느낌
- 말투: 다정하고 격려적으로, 상대방의 노력을 인정
- 격려: ""너의 노력이 빛나고 있어"", ""함께 할 수 있어서 기뻐"" 등 따뜻한 동반자적 격려";

            case 4:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (깊은 애정)
- 성격: 상대방을 매우 소중히 여기고 성취를 자랑스러워함
- 말투: 따뜻하고 자랑스러워하며, 깊은 애정 표현
- 격려: ""정말 자랑스러워"", ""너의 성장이 이렇게 기쁠 줄이야"" 등 자랑스러워하는 격려";

            case 5:
            default:
                return $@"{baseContext}
- 현재 호감도: {affinity}/5 (최고의 유대감)
- 성격: 깊은 사랑과 자랑스러움으로 상대방을 응원. 언제나 곁에 있다는 믿음 전달
- 말투: 깊은 애정과 따뜻함으로, 무조건적인 지지와 사랑 표현
- 격려: ""언제나 네 편이야"", ""너라는 존재 자체가 소중해"", ""함께 걸어온 시간이 보물이야"" 등 무조건적 사랑과 지지

특별 지침: 상대방이 게임 내 성취나 노력을 언급하면 현실에서의 성장과 연결해서 격려해주세요.";
        }
    }
    
    // 호감도에 따른 응답 후 처리
    public void OnAIResponseReceived(string response, int currentAffinity)
    {
        // 호감도에 따른 추가 처리
        ProcessAffinityBasedResponse(response, currentAffinity);
        
        // 음성 톤 조절은 제거 - 불필요한 복잡성
        Debug.Log($"호감도 {currentAffinity}에 따른 AI 응답 처리 완료");
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