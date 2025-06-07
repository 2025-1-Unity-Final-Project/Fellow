using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;

public class Live2DCharacterLifeSystem : MonoBehaviour
{
    [Header("Live2D Components")]
    public CubismModel cubismModel;
    
    [Header("생동감 설정")]
    public bool enableIdleAnimation = true;        // 자동 움직임 활성화
    public bool enableRandomBlinking = true;       // 랜덤 눈 깜빡임
    public bool enableBreathing = true;            // 호흡 효과
    public bool enableRandomGestures = true;       // 랜덤 제스처
    public bool enableSubtleMovements = true;      // 미묘한 움직임
    public bool enableHairAnimation = true;        // 머리카락 애니메이션
    public bool enableClothAnimation = true;       // 옷 애니메이션
    public bool hyperActiveMode = false;           // 초활성 모드 (매우 빠른 제스처)
    
    [Header("눈 깜빡임 설정")]
    public float blinkMinInterval = 1.2f;          // 최소 깜빡임 간격
    public float blinkMaxInterval = 3.5f;          // 최대 깜빡임 간격
    public float blinkSpeed = 8f;                  // 깜빡임 속도
    
    [Header("호흡 설정")]
    public float breathCycleMin = 3f;              // 최소 호흡 주기
    public float breathCycleMax = 5f;              // 최대 호흡 주기
    public float breathIntensity = 0.3f;           // 호흡 강도
    
    [Header("랜덤 제스처 설정")]
    public float gestureMinInterval = 8f;          // 최소 제스처 간격 (더 자연스럽게)
    public float gestureMaxInterval = 15f;         // 최대 제스처 간격
    public float gestureIntensity = 0.4f;          // 제스처 강도 (더 부드럽게)
    
    [Header("미묘한 움직임 설정")]
    public float microMovementIntensity = 0.1f;    // 미세 움직임 강도
    public float microMovementSpeed = 0.3f;        // 미세 움직임 속도
    
    [Header("머리카락 애니메이션 설정")]
    public float hairSwayIntensity = 0.8f;         // 머리카락 흔들림 강도
    public float hairSwaySpeed = 0.5f;             // 머리카락 흔들림 속도
    public float hairRandomFactor = 0.3f;          // 머리카락 랜덤 요소
    
    [Header("옷 애니메이션 설정")]
    public float clothSwayIntensity = 0.4f;        // 옷 흔들림 강도
    public float clothSwaySpeed = 0.4f;            // 옷 흔들림 속도
    
    // 파라미터 캐시
    private CubismParameter leftEyeOpenParam;
    private CubismParameter rightEyeOpenParam;
    private CubismParameter bodyAngleXParam;
    private CubismParameter bodyAngleYParam;
    private CubismParameter bodyAngleZParam;
    private CubismParameter angleXParam;
    private CubismParameter angleYParam;
    private CubismParameter angleZParam;
    private CubismParameter breathParam;
    private CubismParameter leftEyeSmileParam;
    private CubismParameter rightEyeSmileParam;
    private CubismParameter browParam;
    private CubismParameter eyeBallXParam;
    private CubismParameter eyeBallYParam;
    
    // 머리카락 파라미터들 (여러 개)
    private List<CubismParameter> hairParams = new List<CubismParameter>();
    
    // 옷 파라미터들
    private List<CubismParameter> clothParams = new List<CubismParameter>();
    
    // 귀 장식 파라미터들
    private List<CubismParameter> earAccessoryParams = new List<CubismParameter>();
    
    // 코루틴 관리
    private Coroutine blinkingCoroutine;
    private Coroutine breathingCoroutine;
    private Coroutine gestureCoroutine;
    private Coroutine microMovementCoroutine;
    private Coroutine eyeMovementCoroutine;
    private Coroutine hairAnimationCoroutine;
    private Coroutine clothAnimationCoroutine;
    private Coroutine earAccessoryCoroutine;
    
    // 상태 관리
    private bool isBlinking = false;
    private bool isTalking = false;
    private bool eyeMovementPaused = false;
    
    // 자연스러운 움직임을 위한 노이즈 오프셋
    private float noiseOffsetX;
    private float noiseOffsetY;
    private float hairNoiseOffset;
    private float clothNoiseOffset;
    
    void Start()
    {
        // 랜덤 노이즈 오프셋 설정
        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetY = Random.Range(0f, 100f);
        hairNoiseOffset = Random.Range(0f, 100f);
        clothNoiseOffset = Random.Range(0f, 100f);
        
        InitializeParameters();
        StartLifeAnimations();
    }
    
    private void InitializeParameters()
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
        
        var parameters = cubismModel.Parameters;
        Debug.Log("🎭 생동감 시스템 초기화 중...");
        Debug.Log("=== 중요 파라미터 검색 ===");
        for (int i = 0; i < parameters.Length; i++)
        {
            string id = parameters[i].Id;
            
            // 중요한 파라미터들만 출력
            if (id.Contains("Eye") || id.Contains("Mouth") || id.Contains("Angle") || 
                id.Contains("Body") || id.Contains("Breath") || id.Contains("Brow") ||
                id.Contains("眼") || id.Contains("嘴") || id.Contains("角度") || 
                id.Contains("身体") || id.Contains("呼吸"))
            {
                Debug.Log($"⭐ [{i:D2}] '{id}' (값: {parameters[i].Value:F2})");
            }
        }
        // 파라미터 찾기
        foreach (var param in parameters)
        {
            string id = param.Id;
            
            // 눈 관련
            if (id.Contains("ParamEyeLOpen") || (id.Contains("左眼") && id.Contains("开闭")))
                leftEyeOpenParam = param;
            else if (id.Contains("ParamEyeROpen") || (id.Contains("右眼") && id.Contains("开闭")))
                rightEyeOpenParam = param;
            else if (id.Contains("ParamEyeLSmile"))
                leftEyeSmileParam = param;
            else if (id.Contains("ParamEyeRSmile"))
                rightEyeSmileParam = param;
            
            // 몸체 각도
            else if (id.Contains("ParamBodyAngleX") || (id.Contains("身体旋转") && id.Contains("X")))
                bodyAngleXParam = param;
            else if (id.Contains("ParamBodyAngleY") || (id.Contains("身体旋转") && id.Contains("Y")))
                bodyAngleYParam = param;
            else if (id.Contains("ParamBodyAngleZ") || (id.Contains("身体旋转") && id.Contains("Z")))
                bodyAngleZParam = param;
            
            // 머리 각도
            else if (id.Contains("ParamAngleX") || (id.Contains("角度") && id.Contains("X")))
                angleXParam = param;
            else if (id.Contains("ParamAngleY") || (id.Contains("角度") && id.Contains("Y")))
                angleYParam = param;
            else if (id.Contains("ParamAngleZ") || (id.Contains("角度") && id.Contains("Z")))
                angleZParam = param;
            
            // 호흡
            else if (id.Contains("ParamBreath") || id.Contains("呼吸"))
                breathParam = param;
            
            // 눈썹
            else if (id.Contains("ParamBrow"))
                browParam = param;
            
            // 눈동자
            else if (id.Contains("ParamEyeBallX") || (id.Contains("眼球") && id.Contains("X")))
                eyeBallXParam = param;
            else if (id.Contains("ParamEyeBallY") || (id.Contains("眼球") && id.Contains("Y")))
                eyeBallYParam = param;
            
            // 머리카락 파라미터들 수집
            else if (id.Contains("头发") || id.Contains("髪") || id.Contains("Hair"))
            {
                hairParams.Add(param);
                Debug.Log($"💇 머리카락 파라미터 발견: {id}");
            }
            
            // 옷 파라미터들 수집
            else if (id.Contains("衣服") || id.Contains("服装") || id.Contains("Clothes") || id.Contains("飘"))
            {
                clothParams.Add(param);
                Debug.Log($"👗 옷 파라미터 발견: {id}");
            }
            
            // 귀 장식 파라미터들 수집
            else if (id.Contains("耳饰") || id.Contains("左耳") || id.Contains("右耳") || id.Contains("Ear"))
            {
                earAccessoryParams.Add(param);
                Debug.Log($"✨ 귀 장식 파라미터 발견: {id}");
            }
        }
        
        // 찾은 파라미터 상세 로그
        Debug.Log($"✅ 눈 깜빡임: {(leftEyeOpenParam != null && rightEyeOpenParam != null ? "O" : "X")}");
        Debug.Log($"✅ 머리 움직임: {(angleXParam != null || angleYParam != null || angleZParam != null ? "O" : "X")}");
        Debug.Log($"✅ 몸체 움직임: {(bodyAngleXParam != null || bodyAngleYParam != null || bodyAngleZParam != null ? "O" : "X")}");
        Debug.Log($"✅ 호흡: {(breathParam != null ? "O" : "X")}");
        Debug.Log($"✅ 눈동자: {(eyeBallXParam != null && eyeBallYParam != null ? "O" : "X")}");
        Debug.Log($"✅ 머리카락: {hairParams.Count}개");
        Debug.Log($"✅ 옷/장신구: {clothParams.Count}개");
        Debug.Log($"✅ 귀 장식: {earAccessoryParams.Count}개");
    }
    
    private void StartLifeAnimations()
    {
        if (enableRandomBlinking && leftEyeOpenParam != null && rightEyeOpenParam != null)
        {
            blinkingCoroutine = StartCoroutine(RandomBlinkingCoroutine());
        }
        
        if (enableBreathing && breathParam != null)
        {
            breathingCoroutine = StartCoroutine(BreathingCoroutine());
        }
        
        if (enableRandomGestures)
        {
            gestureCoroutine = StartCoroutine(RandomGestureCoroutine());
        }
        
        if (enableSubtleMovements)
        {
            microMovementCoroutine = StartCoroutine(MicroMovementCoroutine());
        }
        
        if (eyeBallXParam != null && eyeBallYParam != null)
        {
            eyeMovementCoroutine = StartCoroutine(EyeMovementCoroutine());
        }
        
        if (enableHairAnimation && hairParams.Count > 0)
        {
            hairAnimationCoroutine = StartCoroutine(HairAnimationCoroutine());
        }
        
        if (enableClothAnimation && clothParams.Count > 0)
        {
            clothAnimationCoroutine = StartCoroutine(ClothAnimationCoroutine());
        }
        
        if (earAccessoryParams.Count > 0)
        {
            earAccessoryCoroutine = StartCoroutine(EarAccessoryAnimationCoroutine());
        }
        
        Debug.Log("🎭 생동감 시스템 시작됨!");
    }
    
    private IEnumerator RandomBlinkingCoroutine()
    {
        while (true)
        {
            if (!isTalking && !isBlinking)
            {
                yield return new WaitForSeconds(Random.Range(blinkMinInterval, blinkMaxInterval));
                
                if (!isTalking) // 말하는 중이 아닐 때만 깜빡임
                {
                    yield return StartCoroutine(BlinkAnimation());
                }
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    
    private IEnumerator BlinkAnimation()
    {
        isBlinking = true;
        
        float originalLeftValue = leftEyeOpenParam.Value;
        float originalRightValue = rightEyeOpenParam.Value;
        
        // 눈 감기
        float closeTime = 1f / blinkSpeed;
        float elapsedTime = 0f;
        
        while (elapsedTime < closeTime)
        {
            float t = elapsedTime / closeTime;
            leftEyeOpenParam.Value = Mathf.Lerp(originalLeftValue, 0f, t);
            rightEyeOpenParam.Value = Mathf.Lerp(originalRightValue, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 잠깐 감고 있기
        yield return new WaitForSeconds(0.05f);
        
        // 눈 뜨기
        elapsedTime = 0f;
        while (elapsedTime < closeTime)
        {
            float t = elapsedTime / closeTime;
            leftEyeOpenParam.Value = Mathf.Lerp(0f, originalLeftValue, t);
            rightEyeOpenParam.Value = Mathf.Lerp(0f, originalRightValue, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        leftEyeOpenParam.Value = originalLeftValue;
        rightEyeOpenParam.Value = originalRightValue;
        
        isBlinking = false;
    }
    
    private IEnumerator BreathingCoroutine()
    {
        while (true)
        {
            if (!isTalking) // 말하는 중이 아닐 때만 호흡
            {
                float breathCycle = Random.Range(breathCycleMin, breathCycleMax);
                float elapsedTime = 0f;
                
                while (elapsedTime < breathCycle && !isTalking)
                {
                    float breathValue = Mathf.Sin(elapsedTime / breathCycle * Mathf.PI) * breathIntensity;
                    if (breathParam != null)
                    {
                        breathParam.Value = breathValue;
                    }
                    
                    // 호흡에 따른 자연스러운 몸체 움직임
                    if (bodyAngleXParam != null)
                    {
                        bodyAngleXParam.Value = breathValue * 0.5f;
                    }
                    
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
            
            yield return new WaitForSeconds(Random.Range(0.3f, 1f));
        }
    }
    
    private IEnumerator RandomGestureCoroutine()
    {
        while (true)
        {
            float minInterval = hyperActiveMode ? 3f : gestureMinInterval;
            float maxInterval = hyperActiveMode ? 8f : gestureMaxInterval;
            
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            
            if (!isTalking)
            {
                yield return StartCoroutine(PerformRandomGesture());
            }
        }
    }
    
    private IEnumerator PerformRandomGesture()
    {
        int gestureType = Random.Range(0, 5);
        float duration = Random.Range(2f, 4f);
        
        switch (gestureType)
        {
            case 0: // 고개 기울이기
                yield return StartCoroutine(HeadTiltGesture(duration));
                break;
                
            case 1: // 미소 짓기
                yield return StartCoroutine(SmileGesture(duration));
                break;
                
            case 2: // 눈썹 올리기
                yield return StartCoroutine(EyebrowRaiseGesture(duration));
                break;
                
            case 3: // 몸 움직이기
                yield return StartCoroutine(BodySway(duration));
                break;
                
            case 4: // 깊은 한숨
                yield return StartCoroutine(DeepSighGesture(duration));
                break;
        }
    }
    
    private IEnumerator HeadTiltGesture(float duration)
    {
        if (angleZParam == null) yield break;
        
        float originalValue = angleZParam.Value;
        float targetAngle = Random.Range(-15f, 15f) * gestureIntensity;
        
        Debug.Log($"🎭 고개 기울이기: {targetAngle:F1}도");
        
        // 기울이기
        yield return StartCoroutine(SmoothParameterChange(angleZParam, originalValue, targetAngle, duration * 0.3f));
        
        // 유지
        yield return new WaitForSeconds(duration * 0.4f);
        
        // 복원
        yield return StartCoroutine(SmoothParameterChange(angleZParam, targetAngle, originalValue, duration * 0.3f));
    }
    
    private IEnumerator SmileGesture(float duration)
    {
        if (leftEyeSmileParam == null || rightEyeSmileParam == null) yield break;
        
        float originalLeft = leftEyeSmileParam.Value;
        float originalRight = rightEyeSmileParam.Value;
        float smileIntensity = Random.Range(0.3f, 0.6f) * gestureIntensity;
        
        Debug.Log($"😊 미소 짓기: {smileIntensity:F2}");
        
        // 미소 시작
        yield return StartCoroutine(SmoothParameterChange(leftEyeSmileParam, originalLeft, smileIntensity, duration * 0.2f));
        yield return StartCoroutine(SmoothParameterChange(rightEyeSmileParam, originalRight, smileIntensity, duration * 0.2f));
        
        // 유지
        yield return new WaitForSeconds(duration * 0.6f);
        
        // 복원
        yield return StartCoroutine(SmoothParameterChange(leftEyeSmileParam, smileIntensity, originalLeft, duration * 0.2f));
        yield return StartCoroutine(SmoothParameterChange(rightEyeSmileParam, smileIntensity, originalRight, duration * 0.2f));
    }
    
    private IEnumerator EyebrowRaiseGesture(float duration)
    {
        if (browParam == null) yield break;
        
        float originalValue = browParam.Value;
        float targetValue = Random.Range(0.2f, 0.5f) * gestureIntensity;
        
        Debug.Log($"🤨 눈썹 올리기: {targetValue:F2}");
        
        // 올리기
        yield return StartCoroutine(SmoothParameterChange(browParam, originalValue, targetValue, duration * 0.3f));
        
        // 유지
        yield return new WaitForSeconds(duration * 0.4f);
        
        // 복원
        yield return StartCoroutine(SmoothParameterChange(browParam, targetValue, originalValue, duration * 0.3f));
    }
    
    private IEnumerator BodySway(float duration)
    {
        if (bodyAngleYParam == null) yield break;
        
        float originalValue = bodyAngleYParam.Value;
        float swayAmount = Random.Range(-10f, 10f) * gestureIntensity;
        
        Debug.Log($"💃 몸 움직이기: {swayAmount:F1}도");
        
        // 기울이기
        yield return StartCoroutine(SmoothParameterChange(bodyAngleYParam, originalValue, swayAmount, duration * 0.5f));
        
        // 복원
        yield return StartCoroutine(SmoothParameterChange(bodyAngleYParam, swayAmount, originalValue, duration * 0.5f));
    }
    
    private IEnumerator DeepSighGesture(float duration)
    {
        if (breathParam == null) yield break;
        
        Debug.Log("😮‍💨 깊은 한숨");
        
        // 깊게 숨 들이마시기
        yield return StartCoroutine(SmoothParameterChange(breathParam, 0f, 0.8f, duration * 0.3f));
        
        // 잠시 멈춤
        yield return new WaitForSeconds(duration * 0.2f);
        
        // 천천히 내쉬기
        yield return StartCoroutine(SmoothParameterChange(breathParam, 0.8f, 0f, duration * 0.5f));
    }
    
    private IEnumerator MicroMovementCoroutine()
    {
        while (true)
        {
            if (!isTalking)
            {
                // 미세한 머리 움직임 (Perlin Noise 사용)
                if (angleXParam != null)
                {
                    float microX = (Mathf.PerlinNoise(Time.time * microMovementSpeed + noiseOffsetX, 0f) - 0.5f) * microMovementIntensity;
                    angleXParam.Value = microX;
                }
                
                if (angleYParam != null)
                {
                    float microY = (Mathf.PerlinNoise(0f, Time.time * microMovementSpeed + noiseOffsetY) - 0.5f) * microMovementIntensity;
                    angleYParam.Value = microY;
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private IEnumerator EyeMovementCoroutine()
    {
        while (true)
        {
            if (!isTalking && !isBlinking && !eyeMovementPaused)
            {
                // 자연스러운 눈동자 움직임
                float targetX = Random.Range(-0.3f, 0.3f);
                float targetY = Random.Range(-0.2f, 0.2f);
                
                float duration = Random.Range(0.5f, 1.5f);
                
                yield return StartCoroutine(SmoothParameterChange(eyeBallXParam, eyeBallXParam.Value, targetX, duration));
                yield return StartCoroutine(SmoothParameterChange(eyeBallYParam, eyeBallYParam.Value, targetY, duration));
                
                yield return new WaitForSeconds(Random.Range(2f, 5f));
                
                // 중앙으로 복원
                if (!eyeMovementPaused)
                {
                    yield return StartCoroutine(SmoothParameterChange(eyeBallXParam, targetX, 0f, 1f));
                    yield return StartCoroutine(SmoothParameterChange(eyeBallYParam, targetY, 0f, 1f));
                }
            }
            
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }
    
    private IEnumerator HairAnimationCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < hairParams.Count; i++)
            {
                var hairParam = hairParams[i];
                if (hairParam == null) continue;
                
                // 각 머리카락마다 다른 주기와 강도
                float offset = i * 0.5f + hairNoiseOffset;
                float speed = hairSwaySpeed * (0.8f + Random.Range(-0.2f, 0.2f));
                float intensity = hairSwayIntensity * (0.5f + Random.Range(-0.3f, 0.3f));
                
                // Perlin Noise와 Sin 파형을 조합해서 자연스러운 움직임
                float noiseValue = Mathf.PerlinNoise(Time.time * speed + offset, 0f);
                float sinValue = Mathf.Sin(Time.time * speed * 2f + offset);
                
                float finalValue = (noiseValue - 0.5f) * intensity + sinValue * intensity * 0.3f;
                
                // 호흡과 연동
                if (breathParam != null && !isTalking)
                {
                    finalValue += breathParam.Value * 0.2f;
                }
                
                hairParam.Value = finalValue;
            }
            
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    private IEnumerator ClothAnimationCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < clothParams.Count; i++)
            {
                var clothParam = clothParams[i];
                if (clothParam == null) continue;
                
                // 옷은 머리카락보다 더 부드럽게 움직임
                float offset = i * 0.8f + clothNoiseOffset;
                float speed = clothSwaySpeed * (0.7f + Random.Range(-0.1f, 0.1f));
                float intensity = clothSwayIntensity * (0.4f + Random.Range(-0.2f, 0.2f));
                
                float noiseValue = Mathf.PerlinNoise(Time.time * speed + offset, 0f);
                float finalValue = (noiseValue - 0.5f) * intensity;
                
                // 호흡과 몸 움직임에 연동
                if (breathParam != null && !isTalking)
                {
                    finalValue += breathParam.Value * 0.3f;
                }
                
                if (bodyAngleYParam != null)
                {
                    finalValue += bodyAngleYParam.Value * 0.1f;
                }
                
                clothParam.Value = finalValue;
            }
            
            yield return new WaitForSeconds(0.08f);
        }
    }
    
    private IEnumerator EarAccessoryAnimationCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < earAccessoryParams.Count; i++)
            {
                var earParam = earAccessoryParams[i];
                if (earParam == null) continue;
                
                // 귀 장식은 머리 움직임과 연동
                float offset = i * 1.2f;
                float speed = 0.3f;
                float intensity = 0.2f;
                
                float noiseValue = Mathf.PerlinNoise(Time.time * speed + offset, 0f);
                float finalValue = (noiseValue - 0.5f) * intensity;
                
                // 머리 움직임과 연동
                if (angleXParam != null)
                {
                    finalValue += angleXParam.Value * 0.5f;
                }
                
                if (angleYParam != null)
                {
                    finalValue += angleYParam.Value * 0.3f;
                }
                
                earParam.Value = finalValue;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private IEnumerator SmoothParameterChange(CubismParameter param, float fromValue, float toValue, float duration)
    {
        if (param == null) yield break;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            // 더 부드러운 곡선 적용 (Smoothstep)
            float smoothT = t * t * (3f - 2f * t);
            param.Value = Mathf.Lerp(fromValue, toValue, smoothT);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        param.Value = toValue;
    }
    
    public void SetTalkingState(bool talking)
    {
        isTalking = talking;
        
        if (talking)
        {
            // 말할 때 호흡 정지
            if (breathParam != null)
            {
                breathParam.Value = 0f;
            }
        }
    }
    
    // 터치 추적 시스템과 연동을 위한 메서드들
    public void PauseEyeMovement(bool pause)
    {
        eyeMovementPaused = pause;
        Debug.Log($"👁️ 눈동자 자동 움직임: {(pause ? "일시정지" : "재개")}");
    }
    
    public bool IsEyeMovementPaused()
    {
        return eyeMovementPaused;
    }
    
    // 터치 추적 중일 때 다른 생동감도 조절
    private float originalMicroIntensity;
    private float originalGestureIntensity;
    private bool trackingModeActive = false;
    
    public void SetTrackingMode(bool isTracking)
    {
        if (isTracking && !trackingModeActive)
        {
            // 터치 추적 중일 때는 생동감 크게 줄이기
            originalMicroIntensity = microMovementIntensity;
            originalGestureIntensity = gestureIntensity;
            
            microMovementIntensity *= 0.1f; // 90% 감소
            gestureIntensity *= 0.2f; // 80% 감소
            trackingModeActive = true;
            
            Debug.Log("🎯 터치 추적 모드 활성화 - 생동감 최소화");
        }
        else if (!isTracking && trackingModeActive)
        {
            // 원래대로 복원
            microMovementIntensity = originalMicroIntensity;
            gestureIntensity = originalGestureIntensity;
            trackingModeActive = false;
            
            Debug.Log("🎯 터치 추적 모드 비활성화 - 생동감 복원");
        }
    }
    
    public void StopAllAnimations()
    {
        if (blinkingCoroutine != null) StopCoroutine(blinkingCoroutine);
        if (breathingCoroutine != null) StopCoroutine(breathingCoroutine);
        if (gestureCoroutine != null) StopCoroutine(gestureCoroutine);
        if (microMovementCoroutine != null) StopCoroutine(microMovementCoroutine);
        if (eyeMovementCoroutine != null) StopCoroutine(eyeMovementCoroutine);
        if (hairAnimationCoroutine != null) StopCoroutine(hairAnimationCoroutine);
        if (clothAnimationCoroutine != null) StopCoroutine(clothAnimationCoroutine);
        if (earAccessoryCoroutine != null) StopCoroutine(earAccessoryCoroutine);
    }
    
    void OnDestroy()
    {
        StopAllAnimations();
    }
    
    [ContextMenu("🎪 Test All Gestures")]
    public void TestAllGestures()
    {
        StartCoroutine(TestAllGesturesSequence());
    }
    
    private IEnumerator TestAllGesturesSequence()
    {
        Debug.Log("🎪 모든 제스처 테스트 시작!");
        
        // 1. 고개 기울이기
        Debug.Log("1️⃣ 고개 기울이기 테스트");
        yield return StartCoroutine(HeadTiltGesture(2f));
        yield return new WaitForSeconds(1f);
        
        // 2. 미소 짓기
        Debug.Log("2️⃣ 미소 짓기 테스트");
        yield return StartCoroutine(SmileGesture(2f));
        yield return new WaitForSeconds(1f);
        
        // 3. 눈썹 올리기
        Debug.Log("3️⃣ 눈썹 올리기 테스트");
        yield return StartCoroutine(EyebrowRaiseGesture(2f));
        yield return new WaitForSeconds(1f);
        
        // 4. 몸 움직이기
        Debug.Log("4️⃣ 몸 움직이기 테스트");
        yield return StartCoroutine(BodySway(2f));
        yield return new WaitForSeconds(1f);
        
        // 5. 깊은 한숨
        Debug.Log("5️⃣ 깊은 한숨 테스트");
        yield return StartCoroutine(DeepSighGesture(2f));
        
        Debug.Log("✅ 모든 제스처 테스트 완료!");
    }
    
    [ContextMenu("🔍 Debug All Found Parameters")]
    public void DebugFoundParameters()
    {
        Debug.Log("=== 실제 찾은 파라미터들 ===");
        
        Debug.Log("👁️ 눈 관련:");
        Debug.Log($"   leftEyeOpenParam: {(leftEyeOpenParam != null ? leftEyeOpenParam.Id : "❌ 없음")}");
        Debug.Log($"   rightEyeOpenParam: {(rightEyeOpenParam != null ? rightEyeOpenParam.Id : "❌ 없음")}");
        Debug.Log($"   leftEyeSmileParam: {(leftEyeSmileParam != null ? leftEyeSmileParam.Id : "❌ 없음")}");
        Debug.Log($"   rightEyeSmileParam: {(rightEyeSmileParam != null ? rightEyeSmileParam.Id : "❌ 없음")}");
        Debug.Log($"   eyeBallXParam: {(eyeBallXParam != null ? eyeBallXParam.Id : "❌ 없음")}");
        Debug.Log($"   eyeBallYParam: {(eyeBallYParam != null ? eyeBallYParam.Id : "❌ 없음")}");
        
        Debug.Log("🗣️ 머리 관련:");
        Debug.Log($"   angleXParam: {(angleXParam != null ? angleXParam.Id : "❌ 없음")}");
        Debug.Log($"   angleYParam: {(angleYParam != null ? angleYParam.Id : "❌ 없음")}");
        Debug.Log($"   angleZParam: {(angleZParam != null ? angleZParam.Id : "❌ 없음")}");
        
        Debug.Log("🫁 몸체 관련:");
        Debug.Log($"   bodyAngleXParam: {(bodyAngleXParam != null ? bodyAngleXParam.Id : "❌ 없음")}");
        Debug.Log($"   bodyAngleYParam: {(bodyAngleYParam != null ? bodyAngleYParam.Id : "❌ 없음")}");
        Debug.Log($"   bodyAngleZParam: {(bodyAngleZParam != null ? bodyAngleZParam.Id : "❌ 없음")}");
        Debug.Log($"   breathParam: {(breathParam != null ? breathParam.Id : "❌ 없음")}");
        
        Debug.Log("🤨 표정 관련:");
        Debug.Log($"   browParam: {(browParam != null ? browParam.Id : "❌ 없음")}");
        
        Debug.Log("💇 머리카락 관련:");
        foreach (var hair in hairParams)
        {
            Debug.Log($"   {hair.Id}");
        }
        
        Debug.Log("👗 옷/장신구 관련:");
        foreach (var cloth in clothParams)
        {
            Debug.Log($"   {cloth.Id}");
        }
        
        Debug.Log("✨ 귀 장식 관련:");
        foreach (var ear in earAccessoryParams)
        {
            Debug.Log($"   {ear.Id}");
        }
    }
    
    [ContextMenu("💨 Test Hair Animation")]
    public void TestHairAnimation()
    {
        StartCoroutine(TestHairAnimationSequence());
    }
    
    private IEnumerator TestHairAnimationSequence()
    {
        Debug.Log("💇 머리카락 애니메이션 테스트 시작!");
        
        foreach (var hairParam in hairParams)
        {
            if (hairParam == null) continue;
            
            Debug.Log($"테스트 중: {hairParam.Id}");
            
            // 강한 흔들림
            yield return StartCoroutine(SmoothParameterChange(hairParam, 0f, 1f, 1f));
            yield return StartCoroutine(SmoothParameterChange(hairParam, 1f, -1f, 2f));
            yield return StartCoroutine(SmoothParameterChange(hairParam, -1f, 0f, 1f));
            
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("✅ 머리카락 애니메이션 테스트 완료!");
    }
    
    [ContextMenu("👗 Test Cloth Animation")]
    public void TestClothAnimation()
    {
        StartCoroutine(TestClothAnimationSequence());
    }
    
    private IEnumerator TestClothAnimationSequence()
    {
        Debug.Log("👗 옷 애니메이션 테스트 시작!");
        
        foreach (var clothParam in clothParams)
        {
            if (clothParam == null) continue;
            
            Debug.Log($"테스트 중: {clothParam.Id}");
            
            // 부드러운 흔들림
            yield return StartCoroutine(SmoothParameterChange(clothParam, 0f, 0.5f, 1.5f));
            yield return StartCoroutine(SmoothParameterChange(clothParam, 0.5f, -0.5f, 3f));
            yield return StartCoroutine(SmoothParameterChange(clothParam, -0.5f, 0f, 1.5f));
            
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("✅ 옷 애니메이션 테스트 완료!");
    }
    
    [ContextMenu("🎮 Toggle Hyperactive Mode")]
    public void ToggleHyperactiveMode()
    {
        hyperActiveMode = !hyperActiveMode;
        Debug.Log($"🎮 초활성 모드: {(hyperActiveMode ? "ON" : "OFF")}");
        
        if (hyperActiveMode)
        {
            // 더 빠른 설정
            blinkMinInterval = 0.5f;
            blinkMaxInterval = 1.5f;
            gestureMinInterval = 2f;
            gestureMaxInterval = 5f;
            microMovementIntensity = 0.2f;
            hairSwayIntensity = 1.2f;
        }
        else
        {
            // 기본 설정으로 복원
            blinkMinInterval = 1.2f;
            blinkMaxInterval = 3.5f;
            gestureMinInterval = 8f;
            gestureMaxInterval = 15f;
            microMovementIntensity = 0.1f;
            hairSwayIntensity = 0.8f;
        }
    }
    
    [ContextMenu("🧘 Set Calm Mode")]
    public void SetCalmMode()
    {
        Debug.Log("🧘 차분한 모드 설정");
        
        hyperActiveMode = false;
        blinkMinInterval = 2f;
        blinkMaxInterval = 5f;
        gestureMinInterval = 15f;
        gestureMaxInterval = 30f;
        microMovementIntensity = 0.05f;
        hairSwayIntensity = 0.4f;
        gestureIntensity = 0.2f;
    }
    
    [ContextMenu("⚡ Set Energetic Mode")]
    public void SetEnergeticMode()
    {
        Debug.Log("⚡ 활발한 모드 설정");
        
        hyperActiveMode = true;
        blinkMinInterval = 0.8f;
        blinkMaxInterval = 2f;
        gestureMinInterval = 3f;
        gestureMaxInterval = 8f;
        microMovementIntensity = 0.15f;
        hairSwayIntensity = 1f;
        gestureIntensity = 0.6f;
    }
}