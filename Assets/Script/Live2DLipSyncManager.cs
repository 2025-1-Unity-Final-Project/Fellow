using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;

public class Live2DLipSyncManager : MonoBehaviour
{
    [Header("Live2D Components")]
    public CubismModel cubismModel;
    public AudioSource audioSource;
    
    [Header("Voice Settings")]
    public AudioClip[] voiceClips;
    public float voiceInterval = 0.15f;
    public int maxVoicesPerMessage = 8;
    
    [Header("Lip Sync Parameters")]
    public string mouthOpenParamName = "ParamMouthOpenY";
    public string mouthFormParamName = "ParamMouthForm";
    
    [Header("Lip Sync Settings")]
    public float lipSyncSpeed = 15f;      // 속도 증가
    public float baseMouthValue = 0f;
    public float maxMouthOpen = 3f;       // 최대값 증가 (Live2D 파라미터 범위에 따라 조정)
    public float lipSyncIntensity = 2f;   // 립싱크 강도 배율
    public float resetDuration = 0.5f;    // 입모양 복귀 시간
    
    private CubismParameter mouthOpenParam;
    private CubismParameter mouthFormParam;
    private Coroutine currentLipSyncCoroutine;
    private Coroutine currentVoiceCoroutine;
    private Coroutine currentResetCoroutine;
    
    void Start()
    {
        InitializeLipSyncParameters();
        
        // 시작 시 입을 확실히 닫기
        if (mouthOpenParam != null)
        {
            mouthOpenParam.Value = baseMouthValue;
        }
    }
    
    private void InitializeLipSyncParameters()
    {
        if (cubismModel == null)
        {
            cubismModel = GetComponent<CubismModel>();
            if (cubismModel == null)
            {
                Debug.LogError("❌ CubismModel을 찾을 수 없습니다!");
                return;
            }
        }
        
        Debug.Log("🔍 Live2D 파라미터 초기화 시작...");
        
        var parameters = cubismModel.Parameters;
        Debug.Log($"📊 총 파라미터 개수: {parameters.Length}");
        
        // 모든 파라미터 이름 출력하여 정확한 이름 찾기
        Debug.Log("=== 모든 파라미터 목록 ===");
        for (int i = 0; i < parameters.Length; i++)
        {
            string paramId = parameters[i].Id;
            Debug.Log($"[{i:D2}] '{paramId}' (길이: {paramId.Length})");
            
            // 바이트 단위로 출력하여 숨겨진 문자 확인
            if (paramId.Contains("嘴") || paramId.Contains("张"))
            {
                Debug.Log($"     입 관련 파라미터 발견! 바이트: {System.Text.Encoding.UTF8.GetByteCount(paramId)}");
                for (int j = 0; j < paramId.Length; j++)
                {
                    Debug.Log($"     [{j}] '{paramId[j]}' (코드: {(int)paramId[j]})");
                }
            }
        }
        
        // 입 관련 파라미터 찾기
        Debug.Log($"🎯 찾는 파라미터: '{mouthOpenParamName}' (길이: {mouthOpenParamName.Length})");
        
        bool foundMouthOpen = false;
        for (int i = 0; i < parameters.Length; i++)
        {
            string paramId = parameters[i].Id;
            
            // 정확한 매칭
            if (paramId == mouthOpenParamName)
            {
                mouthOpenParam = parameters[i];
                foundMouthOpen = true;
                Debug.Log($"✅ 완전 일치로 입 파라미터 찾음: '{paramId}'");
                break;
            }
            
            // 부분 매칭으로도 시도
            if (paramId.Contains("嘴") && paramId.Contains("张") && paramId.Contains("闭"))
            {
                mouthOpenParam = parameters[i];
                foundMouthOpen = true;
                Debug.Log($"✅ 부분 매칭으로 입 파라미터 찾음: '{paramId}'");
                Debug.Log($"   원래 찾던 이름: '{mouthOpenParamName}'");
                // 정확한 이름으로 업데이트
                mouthOpenParamName = paramId;
                break;
            }
        }
        
        if (!foundMouthOpen)
        {
            Debug.LogError($"❌ 입 열림 파라미터를 찾을 수 없습니다: '{mouthOpenParamName}'");
            
            // 유사한 이름들 출력
            Debug.Log("🔍 입 관련 파라미터 검색 결과:");
            foreach (var param in parameters)
            {
                if (param.Id.Contains("嘴") || param.Id.Contains("mouth") || param.Id.Contains("Mouth") || 
                    param.Id.Contains("张") || param.Id.Contains("open") || param.Id.Contains("Open"))
                {
                    Debug.Log($"   후보: '{param.Id}'");
                }
            }
        }
        else
        {
            Debug.Log($"   현재 값: {mouthOpenParam.Value}, 범위: {mouthOpenParam.MinimumValue} ~ {mouthOpenParam.MaximumValue}");
            
            // baseMouthValue를 파라미터의 최소값으로 설정하여 확실히 닫기
            if (baseMouthValue == 0f)
            {
                baseMouthValue = mouthOpenParam.MinimumValue;
                Debug.Log($"   baseMouthValue를 최소값으로 설정: {baseMouthValue}");
            }
        }
    }
    
    [ContextMenu("🔍 Debug Parameters")]
    public void DebugParameters()
    {
        if (cubismModel == null) return;
        
        Debug.Log("=== 모든 파라미터 목록 ===");
        var parameters = cubismModel.Parameters;
        
        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            Debug.Log($"[{i:D2}] '{param.Id}' (값: {param.Value:F2}, 범위: {param.MinimumValue:F1}~{param.MaximumValue:F1})");
        }
    }
    
    [ContextMenu("🔥 Test Strong Mouth Movement")]
    public void TestStrongMouthMovement()
    {
        if (mouthOpenParam != null)
        {
            Debug.Log("🔥 강한 입 움직임 테스트!");
            StartCoroutine(TestStrongMouthAnimation());
        }
        else
        {
            Debug.LogError("❌ 입 파라미터가 없습니다!");
        }
    }
    
    private IEnumerator TestStrongMouthAnimation()
    {
        if (mouthOpenParam == null) yield break;
        
        float originalValue = mouthOpenParam.Value;
        Debug.Log($"🔄 원래 값: {originalValue}");
        Debug.Log($"🔄 파라미터 범위: {mouthOpenParam.MinimumValue} ~ {mouthOpenParam.MaximumValue}");
        
        // 점진적으로 강하게 테스트
        float[] testValues = { 0.5f, 1.0f, 1.5f, 2.0f, 3.0f };
        
        foreach (float testValue in testValues)
        {
            mouthOpenParam.Value = testValue;
            Debug.Log($"   입 값: {testValue} (실제 적용값: {mouthOpenParam.Value})");
            yield return new WaitForSeconds(0.5f);
        }
        
        // 원래대로 복원
        mouthOpenParam.Value = originalValue;
        Debug.Log($"✅ 테스트 완료, 원래 값 복원: {originalValue}");
    }
    
    [ContextMenu("🎤 Test Korean Lip Sync")]
    public void TestKoreanLipSync()
    {
        if (mouthOpenParam == null)
        {
            Debug.LogError("❌ 입 파라미터가 설정되지 않았습니다!");
            InitializeLipSyncParameters();
            return;
        }
        
        string testMessage = "안녕하세요! 저는 프리렌이에요";
        Debug.Log($"🎤 립싱크 테스트 시작: '{testMessage}'");
        StartLipSyncWithMessage(testMessage, 4f);
    }
    
    [ContextMenu("🎵 Test Random Voice")]
    public void TestRandomVoice()
    {
        PlayRandomVoice();
    }
    
    // 🎵 메인 함수 - 음성 재생 추가
    public void StartLipSyncWithMessage(string message, float displayDuration)
    {
        if (mouthOpenParam == null)
        {
            Debug.LogError("❌ 립싱크 불가: 입 파라미터가 없습니다!");
            return;
        }
        
        StopCurrentLipSync();
        
        Debug.Log($"🎬 립싱크 시작: '{message}' ({displayDuration}초)");
        currentLipSyncCoroutine = StartCoroutine(LipSyncCoroutine(message, displayDuration));
        
        // 🎵 간단한 음성 재생 추가
        PlayRandomVoice();
    }
    
    // 🎵 랜덤 음성 재생 함수
    private void PlayRandomVoice()
    {
        if (audioSource == null || voiceClips == null || voiceClips.Length == 0)
        {
            Debug.LogWarning("⚠️ 음성 재생 불가: AudioSource나 VoiceClips가 없습니다!");
            return;
        }
        
        // 랜덤 음성 클립 선택
        AudioClip randomVoice = voiceClips[Random.Range(0, voiceClips.Length)];
        
        // 음성 길이를 1-2초로 제한
        audioSource.clip = randomVoice;
        audioSource.Play();
        
        // 1-2초 후 자동 정지
        float maxDuration = Random.Range(1f, 2f);
        StartCoroutine(StopVoiceAfterTime(maxDuration));
        
        Debug.Log($"🔊 랜덤 음성 재생: {randomVoice.name} ({maxDuration:F1}초)");
    }
    
    // 🎵 음성 자동 정지 함수
    private IEnumerator StopVoiceAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("🔇 음성 재생 자동 정지");
        }
    }
    
    private IEnumerator LipSyncCoroutine(string message, float duration)
    {
        float elapsedTime = 0f;
        float charIndex = 0f;
        float charsPerSecond = message.Length / duration;
        
        while (elapsedTime < duration)
        {
            int currentCharIndex = Mathf.FloorToInt(charIndex);
            
            if (currentCharIndex < message.Length)
            {
                char currentChar = message[currentCharIndex];
                float targetMouthValue = GetMouthValueForCharacter(currentChar);
                
                float currentValue = mouthOpenParam.Value;
                float newValue = Mathf.Lerp(currentValue, targetMouthValue, Time.deltaTime * lipSyncSpeed);
                mouthOpenParam.Value = newValue;
                
                // 처음 몇 글자만 로그 출력
                if (currentCharIndex < 3)
                {
                    Debug.Log($"   '{currentChar}' → 입값: {newValue:F2}");
                }
            }
            
            elapsedTime += Time.deltaTime;
            charIndex += charsPerSecond * Time.deltaTime;
            
            yield return null;
        }
        
        // 립싱크 완료 후 부드럽게 원상복귀
        float resetElapsed = 0f;
        float startValue = mouthOpenParam.Value;
        
        while (resetElapsed < resetDuration)
        {
            float t = resetElapsed / resetDuration;
            mouthOpenParam.Value = Mathf.Lerp(startValue, baseMouthValue, t);
            resetElapsed += Time.deltaTime;
            yield return null;
        }
        
        mouthOpenParam.Value = baseMouthValue;
        Debug.Log("🎬 립싱크 완료 및 입모양 복귀");
    }
    
    private float GetMouthValueForCharacter(char character)
    {
        float baseValue = 0.3f; // 기본 입 열림을 더 크게
        
        // 한국어 모음별 강화된 입모양
        if (character == '안' || character == 'ㅏ' || character == '아') return 1.5f * lipSyncIntensity;
        if (character == '녕' || character == 'ㅕ' || character == '어') return 1.2f * lipSyncIntensity;
        if (character == '하' || character == 'ㅏ') return 1.4f * lipSyncIntensity;
        if (character == '세' || character == 'ㅔ' || character == '에') return 1.1f * lipSyncIntensity;
        if (character == '요' || character == 'ㅛ' || character == '오') return 1.0f * lipSyncIntensity;
        if (character == '이' || character == 'ㅣ') return 0.4f * lipSyncIntensity;
        if (character == '우' || character == 'ㅜ') return 0.8f * lipSyncIntensity;
        
        // 영어 모음
        if (character == 'a' || character == 'A') return 1.4f * lipSyncIntensity;
        if (character == 'e' || character == 'E') return 1.1f * lipSyncIntensity;
        if (character == 'i' || character == 'I') return 0.4f * lipSyncIntensity;
        if (character == 'o' || character == 'O') return 1.0f * lipSyncIntensity;
        if (character == 'u' || character == 'U') return 0.8f * lipSyncIntensity;
        
        // 자음이나 기타 문자
        if (char.IsLetter(character)) return baseValue * lipSyncIntensity;
        
        return 0.1f; // 공백이나 특수문자
    }
    
    public void StopCurrentLipSync()
    {
        if (currentLipSyncCoroutine != null)
        {
            StopCoroutine(currentLipSyncCoroutine);
            currentLipSyncCoroutine = null;
        }
        
        // 🎵 음성 재생 정지 추가
        if (currentVoiceCoroutine != null)
        {
            StopCoroutine(currentVoiceCoroutine);
            currentVoiceCoroutine = null;
        }
        
        // 오디오 정지
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("🔇 음성 재생 정지됨");
        }
        
        // 기존 리셋 코루틴 중단
        if (currentResetCoroutine != null)
        {
            StopCoroutine(currentResetCoroutine);
            currentResetCoroutine = null;
        }
        
        // 부드럽게 원상복귀
        if (mouthOpenParam != null)
        {
            currentResetCoroutine = StartCoroutine(SmoothResetMouth());
        }
    }
    
    private IEnumerator SmoothResetMouth()
    {
        if (mouthOpenParam == null) yield break;
        
        float startValue = mouthOpenParam.Value;
        float elapsed = 0f;
        
        while (elapsed < resetDuration)
        {
            float t = elapsed / resetDuration;
            mouthOpenParam.Value = Mathf.Lerp(startValue, baseMouthValue, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        mouthOpenParam.Value = baseMouthValue;
        currentResetCoroutine = null;
        Debug.Log("🔒 입모양 완전 복귀 완료");
    }
}