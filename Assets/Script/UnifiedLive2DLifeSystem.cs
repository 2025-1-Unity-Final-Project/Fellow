using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;

/// <summary>
/// 자동 생동감 + 터치 제스처를 통합한 시스템
/// DynamicLive2DLifeSystem + TouchGestureSystem 기능 모두 포함
/// </summary>
public class UnifiedLive2DLifeSystem : MonoBehaviour
{
    [Header("Live2D Components")]
    public CubismModel cubismModel;
    
    [Header("시스템 모드")]
    public bool enableAutoGestures = true;             // 자동 제스처 활성화 (이제 모든 제스처 포함)
    public bool enableTouchGestures = false;           // 터치 제스처 (비활성화 권장 - 자동에 포함됨)
    
    [Header("역동성 레벨")]
    [Range(0.5f, 3f)]
    public float dynamicLevel = 2f;
    public bool hyperDynamicMode = false;
    
    [Header("기본 생동감")]
    public bool enableRandomBlinking = true;
    public bool enableBreathing = true;
    public bool enableMicroMovements = true;
    public bool enableHairAnimation = true;
    public bool enableClothAnimation = true;
    
    [Header("눈 깜빡임")]
    public float blinkMinInterval = 0.8f;
    public float blinkMaxInterval = 2.5f;
    public float blinkSpeed = 12f;
    public bool enableDoubleBlinking = true;
    
    [Header("호흡")]
    public float breathCycleMin = 2f;
    public float breathCycleMax = 4f;
    public float breathIntensity = 0.6f;
    public float breathVariation = 0.3f;
    
    [Header("자동 제스처")]
    public float gestureMinInterval = 3f;
    public float gestureMaxInterval = 7f;
    public float gestureIntensity = 0.8f;
    public bool enableContinuousGestures = true;
    
    [Header("터치 제스처")]
    public float touchCooldown = 1.5f;
    public float touchGestureIntensity = 1f;
    
    [Header("미세 움직임")]
    public float microMovementIntensity = 0.25f;
    public float microMovementSpeed = 0.8f;
    public bool enableMicroVariations = true;
    
    [Header("머리카락")]
    public float hairSwayIntensity = 1.5f;
    public float hairSwaySpeed = 1.2f;
    public bool enableHairBounce = true;
    
    [Header("옷")]
    public float clothSwayIntensity = 0.8f;
    public float clothSwaySpeed = 0.9f;
    public bool enableClothBounce = true;
    
    [Header("눈동자")]
    public float eyeMovementFrequency = 1.5f;
    public float eyeMovementRange = 0.6f;
    public bool enableQuickGlances = true;
    public bool enableHeadEyeSync = true;           // 눈과 고개 연동
    public float headFollowIntensity = 0.4f;        // 고개 따라가는 강도
    public float bodyFollowIntensity = 0.2f;        // 몸체 따라가는 강도
    public bool enableBodyFollow = true;            // 몸체도 같이 움직이기
    
    // 파라미터들
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
    private CubismParameter mouthOpenParam;
    
    private List<CubismParameter> hairParams = new List<CubismParameter>();
    private List<CubismParameter> clothParams = new List<CubismParameter>();
    private List<CubismParameter> earAccessoryParams = new List<CubismParameter>();
    
    // 코루틴들
    private Coroutine blinkingCoroutine;
    private Coroutine breathingCoroutine;
    private Coroutine autoGestureCoroutine;
    private Coroutine microMovementCoroutine;
    private Coroutine eyeMovementCoroutine;
    private Coroutine hairAnimationCoroutine;
    private Coroutine clothAnimationCoroutine;
    private Coroutine earAccessoryCoroutine;
    private Coroutine dynamicBoostCoroutine;
    
    // 상태 관리
    private bool isBlinking = false;
    private bool isTalking = false;
    private bool eyeMovementPaused = false;
    private bool isPerformingGesture = false;        // 제스처 실행 중 (충돌 방지)
    private float lastTouchTime = 0f;
    private float dynamicBoost = 1f;
    
    // 노이즈 오프셋들
    private float noiseOffsetX;
    private float noiseOffsetY;
    private float hairNoiseOffset;
    private float clothNoiseOffset;
    private float breathNoiseOffset;
    
    void Start()
    {
        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetY = Random.Range(0f, 100f);
        hairNoiseOffset = Random.Range(0f, 100f);
        clothNoiseOffset = Random.Range(0f, 100f);
        breathNoiseOffset = Random.Range(0f, 100f);
        
        InitializeParameters();
        StartUnifiedLifeAnimations();
    }
    
    void Update()
    {
        HandleTouchInput();
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
        Debug.Log("🎭 통합 Live2D 생동감 시스템 초기화 중...");
        
        foreach (var param in parameters)
        {
            string id = param.Id;
            
            if (IsLeftEyeOpenParam(id))
                leftEyeOpenParam = param;
            else if (IsRightEyeOpenParam(id))
                rightEyeOpenParam = param;
            else if (IsLeftEyeSmileParam(id))
                leftEyeSmileParam = param;
            else if (IsRightEyeSmileParam(id))
                rightEyeSmileParam = param;
            else if (IsBodyAngleXParam(id))
                bodyAngleXParam = param;
            else if (IsBodyAngleYParam(id))
                bodyAngleYParam = param;
            else if (IsBodyAngleZParam(id))
                bodyAngleZParam = param;
            else if (IsAngleXParam(id))
                angleXParam = param;
            else if (IsAngleYParam(id))
                angleYParam = param;
            else if (IsAngleZParam(id))
                angleZParam = param;
            else if (IsBreathParam(id))
                breathParam = param;
            else if (IsBrowParam(id))
                browParam = param;
            else if (IsEyeBallXParam(id))
                eyeBallXParam = param;
            else if (IsEyeBallYParam(id))
                eyeBallYParam = param;
            else if (IsMouthOpenParam(id))
                mouthOpenParam = param;
            else if (IsHairParam(id))
            {
                hairParams.Add(param);
                Debug.Log($"💇 머리카락 파라미터: {id}");
            }
            else if (IsClothParam(id))
            {
                clothParams.Add(param);
                Debug.Log($"👗 옷 파라미터: {id}");
            }
            else if (IsEarAccessoryParam(id))
            {
                earAccessoryParams.Add(param);
                Debug.Log($"✨ 귀 장식 파라미터: {id}");
            }
        }
        
        Debug.Log("🎭 통합 시스템 초기화 완료!");
    }
    
    // 파라미터 판별 메서드들
    private bool IsLeftEyeOpenParam(string id)
    {
        return id.Contains("좌안") && (id.Contains("개폐") || id.Contains("开闭")) ||
               id.Contains("左眼") && id.Contains("开闭") ||
               id.Contains("ParamEyeLOpen") ||
               id.Contains("左眼") && id.Contains("开闭");
    }
    
    private bool IsRightEyeOpenParam(string id)
    {
        return id.Contains("우안") && (id.Contains("개폐") || id.Contains("开闭")) ||
               id.Contains("右眼") && id.Contains("开闭") ||
               id.Contains("ParamEyeROpen") ||
               id.Contains("右眼") && id.Contains("开闭");
    }
    
    private bool IsLeftEyeSmileParam(string id)
    {
        return id.Contains("좌안") && id.Contains("미소") ||
               id.Contains("左眼") && id.Contains("笑") ||
               id.Contains("ParamEyeLSmile") ||
               id.Contains("左眼") && id.Contains("微笑");
    }
    
    private bool IsRightEyeSmileParam(string id)
    {
        return id.Contains("우안") && id.Contains("미소") ||
               id.Contains("右眼") && id.Contains("笑") ||
               id.Contains("ParamEyeRSmile") ||
               id.Contains("右眼") && id.Contains("微笑");
    }
    
    private bool IsBodyAngleXParam(string id)
    {
        return (id.Contains("몸체") || id.Contains("신체") || id.Contains("身体")) && id.Contains("X") ||
               id.Contains("ParamBodyAngleX") ||
               id.Contains("身体旋转") && id.Contains("X") ||
               id.Contains("身体转向") && id.Contains("X");
    }
    
    private bool IsBodyAngleYParam(string id)
    {
        return (id.Contains("몸체") || id.Contains("신체") || id.Contains("身体")) && id.Contains("Y") ||
               id.Contains("ParamBodyAngleY") ||
               id.Contains("身体旋转") && id.Contains("Y") ||
               id.Contains("身体转向") && id.Contains("Y");
    }
    
    private bool IsBodyAngleZParam(string id)
    {
        return (id.Contains("몸체") || id.Contains("신체") || id.Contains("身体")) && id.Contains("Z") ||
               id.Contains("ParamBodyAngleZ") ||
               id.Contains("身体旋转") && id.Contains("Z") ||
               id.Contains("身体转向") && id.Contains("Z");
    }
    
    private bool IsAngleXParam(string id)
    {
        return (id.Contains("각도") && id.Contains("X")) ||
               (id.Contains("角度") && id.Contains("X")) ||
               id.Contains("ParamAngleX") ||
               id.Contains("角度") && id.Contains("X") ||
               id.Contains("头部") && id.Contains("X") ||
               id.Contains("面部") && id.Contains("X");
    }
    
    private bool IsAngleYParam(string id)
    {
        return (id.Contains("각도") && id.Contains("Y")) ||
               (id.Contains("角度") && id.Contains("Y")) ||
               id.Contains("ParamAngleY") ||
               id.Contains("角度") && id.Contains("Y") ||
               id.Contains("头部") && id.Contains("Y") ||
               id.Contains("面部") && id.Contains("Y");
    }
    
    private bool IsAngleZParam(string id)
    {
        return (id.Contains("각도") && id.Contains("Z")) ||
               (id.Contains("角度") && id.Contains("Z")) ||
               id.Contains("ParamAngleZ") ||
               id.Contains("角度") && id.Contains("Z") ||
               id.Contains("头部") && id.Contains("Z") ||
               id.Contains("面部") && id.Contains("Z");
    }
    
    private bool IsBreathParam(string id)
    {
        return id.Contains("호흡") || id.Contains("呼吸") || id.Contains("ParamBreath") || 
               id.Equals("生气") || id.Contains("呼吸") || id.Contains("breathing");
    }
    
    private bool IsBrowParam(string id)
    {
        return id.Contains("눈썹") || id.Contains("眉") || id.Contains("ParamBrow") ||
               id.Contains("眉毛") || id.Contains("眉头");
    }
    
    private bool IsEyeBallXParam(string id)
    {
        return (id.Contains("눈동자") || id.Contains("眼球")) && id.Contains("X") ||
               id.Contains("ParamEyeBallX") ||
               id.Contains("眼球") && id.Contains("X") ||
               id.Contains("瞳孔") && id.Contains("X");
    }
    
    private bool IsEyeBallYParam(string id)
    {
        return (id.Contains("눈동자") || id.Contains("眼球")) && id.Contains("Y") ||
               id.Contains("ParamEyeBallY") ||
               id.Contains("眼球") && id.Contains("Y") ||
               id.Contains("瞳孔") && id.Contains("Y");
    }
    
    private bool IsMouthOpenParam(string id)
    {
        return id.Contains("입") && (id.Contains("열기") || id.Contains("개폐")) ||
               id.Contains("嘴") && id.Contains("开") ||
               id.Contains("ParamMouthOpen") ||
               id.Contains("嘴巴") && id.Contains("张开") ||
               id.Contains("口") && id.Contains("开");
    }
    
    private bool IsHairParam(string id)
    {
        return id.Contains("머리카락") || id.Contains("头发") || id.Contains("髪") || 
               id.Contains("Hair") || id.Contains("头发") || 
               (id.Contains("머리") && !id.Contains("각도")) ||
               (id.Contains("头") && id.Contains("发")) ||
               id.Contains("发丝") || id.Contains("发") ||
               (id.Contains("发") && (id.Contains("[") || id.Contains("左") || id.Contains("右")));
    }
    
    private bool IsClothParam(string id)
    {
        return id.Contains("옷") || id.Contains("의상") || id.Contains("衣服") || 
               id.Contains("服装") || id.Contains("Clothes") || id.Contains("飘") ||
               id.Contains("裙") || id.Contains("袖") || id.Contains("领") ||
               id.Contains("飘动") || id.Contains("摆动");
    }
    
    private bool IsEarAccessoryParam(string id)
    {
        return id.Contains("귀") || id.Contains("耳") || id.Contains("Ear") ||
               id.Contains("귀걸이") || id.Contains("耳环") || id.Contains("耳饰");
    }
    
    private void StartUnifiedLifeAnimations()
    {
        // 기본 생동감 (항상 작동)
        if (enableRandomBlinking && leftEyeOpenParam != null && rightEyeOpenParam != null)
        {
            blinkingCoroutine = StartCoroutine(BlinkingCoroutine());
        }
        
        if (enableBreathing && breathParam != null)
        {
            breathingCoroutine = StartCoroutine(BreathingCoroutine());
        }
        
        if (enableMicroMovements)
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
            earAccessoryCoroutine = StartCoroutine(EarAccessoryCoroutine());
        }
        
        // 자동 제스처 (선택적)
        if (enableAutoGestures)
        {
            dynamicBoostCoroutine = StartCoroutine(DynamicBoostCoroutine());
            autoGestureCoroutine = StartCoroutine(AutoGestureCoroutine());
        }
        
        Debug.Log($"🎭 통합 생동감 시스템 시작! (자동제스처: {enableAutoGestures} - 10종 포함, 터치제스처: {enableTouchGestures})");
    }
    
    // 터치 입력 처리
    private void HandleTouchInput()
    {
        if (!enableTouchGestures || isPerformingGesture) return;
        
        bool inputDetected = false;
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                inputDetected = true;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
        }
        
        if (inputDetected && Time.time - lastTouchTime > touchCooldown)
        {
            lastTouchTime = Time.time;
            TriggerTouchGesture();
        }
    }
    
    public void TriggerTouchGesture()
    {
        if (isTalking || isPerformingGesture) return;
        
        StartCoroutine(PerformTouchGesture());
    }
    
    // 충돌 방지된 제스처 시스템
    private IEnumerator PerformTouchGesture()
    {
        isPerformingGesture = true;
        
        int gestureType = Random.Range(0, 8);
        float duration = Random.Range(1f, 2.5f);
        float intensity = touchGestureIntensity;
        
        Debug.Log($"👆 터치 제스처: {GetGestureName(gestureType)}");
        
        switch (gestureType)
        {
            case 0:
                yield return StartCoroutine(HeadTiltGesture(duration, intensity));
                break;
            case 1:
                yield return StartCoroutine(SmileGesture(duration, intensity));
                break;
            case 2:
                yield return StartCoroutine(EyebrowRaiseGesture(duration, intensity));
                break;
            case 3:
                yield return StartCoroutine(BodySwayGesture(duration, intensity));
                break;
            case 4:
                yield return StartCoroutine(WinkGesture(duration));
                break;
            case 5:
                yield return StartCoroutine(SurpriseGesture(duration, intensity));
                break;
            case 6:
                yield return StartCoroutine(NodGesture(duration, intensity));
                break;
            case 7:
                yield return StartCoroutine(ShyGesture(duration, intensity));
                break;
        }
        
        isPerformingGesture = false;
    }
    
    private IEnumerator AutoGestureCoroutine()
    {
        while (true)
        {
            float minInterval = hyperDynamicMode ? 1.5f : gestureMinInterval;
            float maxInterval = hyperDynamicMode ? 4f : gestureMaxInterval;
            
            float interval = Random.Range(minInterval, maxInterval) / dynamicLevel;
            yield return new WaitForSeconds(interval);
            
            if (!isTalking && !isPerformingGesture && enableAutoGestures)
            {
                yield return StartCoroutine(PerformAutoGesture());
                
                if (enableContinuousGestures && Random.Range(0f, 1f) < 0.4f)
                {
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
                    if (!isPerformingGesture)
                    {
                        yield return StartCoroutine(PerformAutoGesture());
                    }
                }
            }
        }
    }
    
    private IEnumerator PerformAutoGesture()
    {
        isPerformingGesture = true;
        
        int gestureType = Random.Range(0, 5); // 자동 제스처는 기본 5종만
        float duration = Random.Range(1.5f, 3f) / dynamicLevel;
        float intensity = gestureIntensity * dynamicLevel * dynamicBoost;
        
        Debug.Log($"🤖 자동 제스처: {GetAutoGestureName(gestureType)}");
        
        switch (gestureType)
        {
            case 0:
                yield return StartCoroutine(HeadTiltGesture(duration, intensity));
                break;
            case 1:
                yield return StartCoroutine(SmileGesture(duration, intensity));
                break;
            case 2:
                yield return StartCoroutine(EyebrowRaiseGesture(duration, intensity));
                break;
            case 3:
                yield return StartCoroutine(BodySwayGesture(duration, intensity));
                break;
            case 4:
                yield return StartCoroutine(DeepSighGesture(duration, intensity));
                break;
        }
        
        isPerformingGesture = false;
    }
    
    private string GetGestureName(int gestureType)
    {
        switch (gestureType)
        {
            case 0: return "고개 기울이기";
            case 1: return "미소 짓기";
            case 2: return "눈썹 올리기";
            case 3: return "몸 흔들기";
            case 4: return "윙크하기";
            case 5: return "놀라기";
            case 6: return "고개 끄덕이기";
            case 7: return "수줍어하기";
            default: return "알 수 없음";
        }
    }
    
    private string GetAutoGestureName(int gestureType)
    {
        switch (gestureType)
        {
            case 0: return "고개 기울이기";
            case 1: return "미소 짓기";
            case 2: return "눈썹 올리기";
            case 3: return "몸 흔들기";
            case 4: return "깊은 한숨";
            default: return "알 수 없음";
        }
    }
    
    // 제스처 구현들
    private IEnumerator HeadTiltGesture(float duration, float intensity)
    {
        if (angleZParam == null) yield break;
        
        float originalValue = angleZParam.Value;
        float targetAngle = Random.Range(-20f, 20f) * intensity;
        
        yield return StartCoroutine(SmoothParameterChange(angleZParam, originalValue, targetAngle, duration * 0.3f));
        yield return new WaitForSeconds(duration * 0.4f);
        yield return StartCoroutine(SmoothParameterChange(angleZParam, targetAngle, originalValue, duration * 0.3f));
    }
    
    private IEnumerator SmileGesture(float duration, float intensity)
    {
        if (leftEyeSmileParam == null || rightEyeSmileParam == null) yield break;
        
        float smileIntensity = Random.Range(0.5f, 1f) * intensity;
        
        yield return StartCoroutine(SmoothParameterChange(leftEyeSmileParam, leftEyeSmileParam.Value, smileIntensity, duration * 0.2f));
        yield return StartCoroutine(SmoothParameterChange(rightEyeSmileParam, rightEyeSmileParam.Value, smileIntensity, duration * 0.2f));
        yield return new WaitForSeconds(duration * 0.6f);
        yield return StartCoroutine(SmoothParameterChange(leftEyeSmileParam, smileIntensity, 0f, duration * 0.2f));
        yield return StartCoroutine(SmoothParameterChange(rightEyeSmileParam, smileIntensity, 0f, duration * 0.2f));
    }
    
    private IEnumerator EyebrowRaiseGesture(float duration, float intensity)
    {
        if (browParam == null) yield break;
        
        float targetValue = Random.Range(0.4f, 0.8f) * intensity;
        
        yield return StartCoroutine(SmoothParameterChange(browParam, browParam.Value, targetValue, duration * 0.3f));
        yield return new WaitForSeconds(duration * 0.4f);
        yield return StartCoroutine(SmoothParameterChange(browParam, targetValue, 0f, duration * 0.3f));
    }
    
    private IEnumerator BodySwayGesture(float duration, float intensity)
    {
        if (bodyAngleYParam == null) yield break;
        
        float swayAmount = Random.Range(-15f, 15f) * intensity;
        
        yield return StartCoroutine(SmoothParameterChange(bodyAngleYParam, bodyAngleYParam.Value, swayAmount, duration * 0.5f));
        yield return StartCoroutine(SmoothParameterChange(bodyAngleYParam, swayAmount, 0f, duration * 0.5f));
    }
    
    private IEnumerator DeepSighGesture(float duration, float intensity)
    {
        if (breathParam == null) yield break;
        
        float sighIntensity = 1f * intensity;
        
        yield return StartCoroutine(SmoothParameterChange(breathParam, 0f, sighIntensity, duration * 0.3f));
        yield return new WaitForSeconds(duration * 0.2f);
        yield return StartCoroutine(SmoothParameterChange(breathParam, sighIntensity, 0f, duration * 0.5f));
    }
    
    private IEnumerator WinkGesture(float duration)
    {
        bool winkLeft = Random.Range(0f, 1f) > 0.5f;
        CubismParameter targetEye = winkLeft ? leftEyeOpenParam : rightEyeOpenParam;
        
        if (targetEye == null) yield break;
        
        float originalValue = targetEye.Value;
        
        yield return StartCoroutine(SmoothParameterChange(targetEye, originalValue, 0f, duration * 0.15f));
        yield return new WaitForSeconds(duration * 0.3f);
        yield return StartCoroutine(SmoothParameterChange(targetEye, 0f, originalValue, duration * 0.55f));
    }
    
    private IEnumerator SurpriseGesture(float duration, float intensity)
    {
        List<Coroutine> actions = new List<Coroutine>();
        
        if (leftEyeOpenParam != null && rightEyeOpenParam != null)
        {
            actions.Add(StartCoroutine(SmoothParameterChange(leftEyeOpenParam, leftEyeOpenParam.Value, 1.2f, duration * 0.2f)));
            actions.Add(StartCoroutine(SmoothParameterChange(rightEyeOpenParam, rightEyeOpenParam.Value, 1.2f, duration * 0.2f)));
        }
        
        if (browParam != null)
        {
            actions.Add(StartCoroutine(SmoothParameterChange(browParam, browParam.Value, 0.8f * intensity, duration * 0.2f)));
        }
        
        yield return new WaitForSeconds(duration * 0.2f);
        yield return new WaitForSeconds(duration * 0.5f);
        
        if (leftEyeOpenParam != null && rightEyeOpenParam != null)
        {
            StartCoroutine(SmoothParameterChange(leftEyeOpenParam, 1.2f, 1f, duration * 0.3f));
            StartCoroutine(SmoothParameterChange(rightEyeOpenParam, 1.2f, 1f, duration * 0.3f));
        }
        
        if (browParam != null)
        {
            StartCoroutine(SmoothParameterChange(browParam, 0.8f * intensity, 0f, duration * 0.3f));
        }
        
        yield return new WaitForSeconds(duration * 0.3f);
    }
    
    private IEnumerator NodGesture(float duration, float intensity)
    {
        if (angleXParam == null) yield break;
        
        float originalValue = angleXParam.Value;
        float nodAmount = 12f * intensity;
        
        float elapsedTime = 0f;
        float downDuration = duration * 0.4f;
        
        while (elapsedTime < downDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / downDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            angleXParam.Value = Mathf.Lerp(originalValue, nodAmount, smoothT);
            yield return null;
        }
        
        yield return new WaitForSeconds(duration * 0.2f);
        
        elapsedTime = 0f;
        float upDuration = duration * 0.4f;
        
        while (elapsedTime < upDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / upDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            angleXParam.Value = Mathf.Lerp(nodAmount, originalValue, smoothT);
            yield return null;
        }
        
        angleXParam.Value = originalValue;
    }
    
    private IEnumerator ShyGesture(float duration, float intensity)
    {
        List<Coroutine> actions = new List<Coroutine>();
        
        if (angleXParam != null)
        {
            actions.Add(StartCoroutine(SmoothParameterChange(angleXParam, angleXParam.Value, 10f * intensity, duration * 0.4f)));
        }
        
        if (angleZParam != null)
        {
            float tiltDirection = Random.Range(0f, 1f) > 0.5f ? 1f : -1f;
            actions.Add(StartCoroutine(SmoothParameterChange(angleZParam, angleZParam.Value, 12f * intensity * tiltDirection, duration * 0.4f)));
        }
        
        yield return new WaitForSeconds(duration * 0.4f);
        yield return new WaitForSeconds(duration * 0.2f);
        
        if (angleXParam != null)
        {
            StartCoroutine(SmoothParameterChange(angleXParam, 10f * intensity, 0f, duration * 0.4f));
        }
        
        if (angleZParam != null)
        {
            StartCoroutine(SmoothParameterChange(angleZParam, angleZParam.Value, 0f, duration * 0.4f));
        }
        
        yield return new WaitForSeconds(duration * 0.4f);
    }
    
    // 새로운 제스처: 기지개 켜기
    private IEnumerator StretchGesture(float duration, float intensity)
    {
        Debug.Log("🙆 기지개 켜기!");
        
        List<Coroutine> actions = new List<Coroutine>();
        
        // 몸을 뒤로 젖히고 팔을 뻗는 듯한 동작
        if (bodyAngleXParam != null)
        {
            actions.Add(StartCoroutine(SmoothParameterChange(bodyAngleXParam, bodyAngleXParam.Value, -8f * intensity, duration * 0.3f)));
        }
        
        // 고개를 살짝 뒤로
        if (angleXParam != null)
        {
            actions.Add(StartCoroutine(SmoothParameterChange(angleXParam, angleXParam.Value, -5f * intensity, duration * 0.3f)));
        }
        
        // 눈을 살짝 감기 (편안한 표정)
        if (leftEyeOpenParam != null && rightEyeOpenParam != null)
        {
            actions.Add(StartCoroutine(SmoothParameterChange(leftEyeOpenParam, leftEyeOpenParam.Value, 0.7f, duration * 0.3f)));
            actions.Add(StartCoroutine(SmoothParameterChange(rightEyeOpenParam, rightEyeOpenParam.Value, 0.7f, duration * 0.3f)));
        }
        
        yield return new WaitForSeconds(duration * 0.3f);
        
        // 기지개 유지
        yield return new WaitForSeconds(duration * 0.4f);
        
        // 원상복구
        if (bodyAngleXParam != null)
        {
            StartCoroutine(SmoothParameterChange(bodyAngleXParam, -8f * intensity, 0f, duration * 0.3f));
        }
        
        if (angleXParam != null)
        {
            StartCoroutine(SmoothParameterChange(angleXParam, -5f * intensity, 0f, duration * 0.3f));
        }
        
        if (leftEyeOpenParam != null && rightEyeOpenParam != null)
        {
            StartCoroutine(SmoothParameterChange(leftEyeOpenParam, 0.7f, 1f, duration * 0.3f));
            StartCoroutine(SmoothParameterChange(rightEyeOpenParam, 0.7f, 1f, duration * 0.3f));
        }
        
        yield return new WaitForSeconds(duration * 0.3f);
    }
    
    // 기본 생동감 애니메이션들
    private IEnumerator BlinkingCoroutine()
    {
        while (true)
        {
            if (!isTalking && !isBlinking && !isPerformingGesture)
            {
                float interval = Random.Range(blinkMinInterval, blinkMaxInterval) / dynamicLevel;
                yield return new WaitForSeconds(interval);
                
                if (!isTalking && !isPerformingGesture)
                {
                    yield return StartCoroutine(BlinkAnimation());
                    
                    if (enableDoubleBlinking && Random.Range(0f, 1f) < 0.3f)
                    {
                        yield return new WaitForSeconds(0.2f);
                        if (!isPerformingGesture)
                        {
                            yield return StartCoroutine(BlinkAnimation());
                        }
                    }
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
        
        float closeTime = 1f / (blinkSpeed * dynamicLevel * dynamicBoost);
        float elapsedTime = 0f;
        
        while (elapsedTime < closeTime)
        {
            float t = elapsedTime / closeTime;
            float curve = t * t * (3f - 2f * t);
            leftEyeOpenParam.Value = Mathf.Lerp(originalLeftValue, 0f, curve);
            rightEyeOpenParam.Value = Mathf.Lerp(originalRightValue, 0f, curve);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(0.03f);
        
        elapsedTime = 0f;
        while (elapsedTime < closeTime)
        {
            float t = elapsedTime / closeTime;
            float curve = t * t * (3f - 2f * t);
            leftEyeOpenParam.Value = Mathf.Lerp(0f, originalLeftValue, curve);
            rightEyeOpenParam.Value = Mathf.Lerp(0f, originalRightValue, curve);
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
            if (!isTalking)
            {
                float breathCycle = Random.Range(breathCycleMin, breathCycleMax) / dynamicLevel;
                float elapsedTime = 0f;
                
                float breathVariationOffset = Random.Range(-breathVariation, breathVariation);
                
                while (elapsedTime < breathCycle && !isTalking)
                {
                    float baseBreath = Mathf.Sin(elapsedTime / breathCycle * Mathf.PI);
                    float noiseBreath = Mathf.PerlinNoise(Time.time * 0.5f + breathNoiseOffset, 0f) - 0.5f;
                    float finalBreath = (baseBreath + noiseBreath * 0.3f + breathVariationOffset) * breathIntensity * dynamicLevel * dynamicBoost;
                    
                    if (breathParam != null && !isPerformingGesture)
                    {
                        breathParam.Value = finalBreath;
                    }
                    
                    if (bodyAngleXParam != null && !isPerformingGesture)
                    {
                        bodyAngleXParam.Value = finalBreath * 0.8f;
                    }
                    
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
            
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    }
    
    private IEnumerator MicroMovementCoroutine()
    {
        while (true)
        {
            if (!isTalking && !isPerformingGesture)
            {
                float currentIntensity = microMovementIntensity * dynamicLevel * dynamicBoost;
                float currentSpeed = microMovementSpeed * dynamicLevel;
                
                // 눈-고개 연동이 비활성화되어 있을 때만 미세움직임 적용
                if (angleXParam != null && !enableHeadEyeSync)
                {
                    float microX = (Mathf.PerlinNoise(Time.time * currentSpeed + noiseOffsetX, 0f) - 0.5f) * currentIntensity;
                    if (enableMicroVariations)
                    {
                        microX += Mathf.Sin(Time.time * currentSpeed * 3f) * currentIntensity * 0.3f;
                    }
                    angleXParam.Value = microX;
                }
                
                if (angleYParam != null && !enableHeadEyeSync)
                {
                    float microY = (Mathf.PerlinNoise(0f, Time.time * currentSpeed + noiseOffsetY) - 0.5f) * currentIntensity;
                    if (enableMicroVariations)
                    {
                        microY += Mathf.Cos(Time.time * currentSpeed * 2.5f) * currentIntensity * 0.3f;
                    }
                    angleYParam.Value = microY;
                }
                
                // 몸체 미세움직임 (눈-고개 연동과 별개로 작동)
                if (bodyAngleXParam != null && (!enableHeadEyeSync || !enableBodyFollow))
                {
                    float bodyMicroX = (Mathf.PerlinNoise(Time.time * currentSpeed * 0.3f + noiseOffsetX + 10f, 0f) - 0.5f) * currentIntensity * 0.5f;
                    bodyAngleXParam.Value += bodyMicroX * 0.1f; // 매우 약하게
                }
            }
            
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    private IEnumerator EyeMovementCoroutine()
    {
        while (true)
        {
            if (!isTalking && !isBlinking && !eyeMovementPaused && !isPerformingGesture)
            {
                float movementRange = eyeMovementRange * dynamicLevel;
                float targetX = Random.Range(-movementRange, movementRange);
                float targetY = Random.Range(-movementRange * 0.7f, movementRange * 0.7f);
                
                float duration = Random.Range(0.3f, 1f) / eyeMovementFrequency;
                
                if (enableQuickGlances && Random.Range(0f, 1f) < 0.3f)
                {
                    duration *= 0.5f;
                }
                
                // 고급 눈-고개-몸체 연동 시스템
                if (enableHeadEyeSync)
                {
                    // 고개 회전 계산 (더 자연스럽게)
                    float headRotationX = targetY * headFollowIntensity * 15f;  // 위아래 고개
                    float headRotationY = targetX * headFollowIntensity * 20f;  // 좌우 고개
                    
                    // 몸체 회전 계산 (큰 움직임일 때만)
                    float bodyRotationX = 0f;
                    float bodyRotationY = 0f;
                    float bodyRotationZ = 0f;
                    
                    if (enableBodyFollow && Mathf.Abs(targetX) > movementRange * 0.6f)
                    {
                        bodyRotationY = targetX * bodyFollowIntensity * 12f;
                        bodyRotationX = targetY * bodyFollowIntensity * 8f;
                        bodyRotationZ = targetX * bodyFollowIntensity * 5f; // 살짝 기울이기
                    }
                    
                    Debug.Log($"👁️ 시선 이동: 눈({targetX:F2}, {targetY:F2}) → 고개({headRotationY:F1}°, {headRotationX:F1}°) → 몸체({bodyRotationY:F1}°)");
                    
                    // 고개 움직임 적용
                    if (angleXParam != null)
                    {
                        StartCoroutine(SmoothParameterChange(angleXParam, angleXParam.Value, headRotationX, duration));
                    }
                    if (angleYParam != null)
                    {
                        StartCoroutine(SmoothParameterChange(angleYParam, angleYParam.Value, headRotationY, duration));
                    }
                    
                    // 몸체 움직임 적용 (선택적)
                    if (enableBodyFollow)
                    {
                        if (bodyAngleXParam != null)
                        {
                            StartCoroutine(SmoothParameterChange(bodyAngleXParam, bodyAngleXParam.Value, bodyRotationX, duration * 1.2f));
                        }
                        if (bodyAngleYParam != null)
                        {
                            StartCoroutine(SmoothParameterChange(bodyAngleYParam, bodyAngleYParam.Value, bodyRotationY, duration * 1.2f));
                        }
                        if (bodyAngleZParam != null && Mathf.Abs(bodyRotationZ) > 0.1f)
                        {
                            StartCoroutine(SmoothParameterChange(bodyAngleZParam, bodyAngleZParam.Value, bodyRotationZ, duration * 1.2f));
                        }
                    }
                }
                
                // 눈동자 움직임 (항상 적용)
                yield return StartCoroutine(SmoothParameterChange(eyeBallXParam, eyeBallXParam.Value, targetX, duration));
                yield return StartCoroutine(SmoothParameterChange(eyeBallYParam, eyeBallYParam.Value, targetY, duration));
                
                // 시선 유지 시간
                yield return new WaitForSeconds(Random.Range(1f, 3f) / dynamicLevel);
                
                if (!eyeMovementPaused && !isPerformingGesture)
                {
                    // 모든 것을 원위치로 복귀
                    float returnDuration = 0.8f;
                    
                    // 눈동자 복귀
                    yield return StartCoroutine(SmoothParameterChange(eyeBallXParam, targetX, 0f, returnDuration));
                    yield return StartCoroutine(SmoothParameterChange(eyeBallYParam, targetY, 0f, returnDuration));
                    
                    // 고개와 몸체도 함께 복귀
                    if (enableHeadEyeSync)
                    {
                        if (angleXParam != null)
                        {
                            StartCoroutine(SmoothParameterChange(angleXParam, angleXParam.Value, 0f, returnDuration));
                        }
                        if (angleYParam != null)
                        {
                            StartCoroutine(SmoothParameterChange(angleYParam, angleYParam.Value, 0f, returnDuration));
                        }
                        
                        if (enableBodyFollow)
                        {
                            if (bodyAngleXParam != null)
                            {
                                StartCoroutine(SmoothParameterChange(bodyAngleXParam, bodyAngleXParam.Value, 0f, returnDuration * 1.3f));
                            }
                            if (bodyAngleYParam != null)
                            {
                                StartCoroutine(SmoothParameterChange(bodyAngleYParam, bodyAngleYParam.Value, 0f, returnDuration * 1.3f));
                            }
                            if (bodyAngleZParam != null)
                            {
                                StartCoroutine(SmoothParameterChange(bodyAngleZParam, bodyAngleZParam.Value, 0f, returnDuration * 1.3f));
                            }
                        }
                    }
                }
            }
            
            yield return new WaitForSeconds(Random.Range(2f, 5f) / dynamicLevel);
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
                
                float offset = i * 0.4f + hairNoiseOffset;
                float speed = hairSwaySpeed * dynamicLevel * (0.8f + Random.Range(-0.2f, 0.2f));
                float intensity = hairSwayIntensity * dynamicLevel * dynamicBoost * (0.5f + Random.Range(-0.3f, 0.3f));
                
                float noiseValue1 = Mathf.PerlinNoise(Time.time * speed + offset, 0f);
                float noiseValue2 = Mathf.PerlinNoise(Time.time * speed * 0.5f + offset + 50f, 0f);
                float sinValue = Mathf.Sin(Time.time * speed * 2f + offset);
                
                float finalValue = ((noiseValue1 - 0.5f) * 0.6f + (noiseValue2 - 0.5f) * 0.3f + sinValue * 0.1f) * intensity;
                
                if (breathParam != null && !isTalking && !isPerformingGesture)
                {
                    finalValue += breathParam.Value * 0.3f;
                }
                
                if (enableHairBounce && Random.Range(0f, 1f) < 0.005f)
                {
                    finalValue += Random.Range(-0.5f, 0.5f) * intensity;
                }
                
                hairParam.Value = finalValue;
            }
            
            yield return new WaitForSeconds(0.03f);
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
                
                float offset = i * 0.6f + clothNoiseOffset;
                float speed = clothSwaySpeed * dynamicLevel * (0.7f + Random.Range(-0.1f, 0.1f));
                float intensity = clothSwayIntensity * dynamicLevel * dynamicBoost * (0.4f + Random.Range(-0.2f, 0.2f));
                
                float noiseValue = Mathf.PerlinNoise(Time.time * speed + offset, 0f);
                float sinValue = Mathf.Sin(Time.time * speed * 1.5f + offset);
                
                float finalValue = ((noiseValue - 0.5f) * 0.8f + sinValue * 0.2f) * intensity;
                
                if (breathParam != null && !isTalking && !isPerformingGesture)
                {
                    finalValue += breathParam.Value * 0.4f;
                }
                
                if (bodyAngleYParam != null && !isPerformingGesture)
                {
                    finalValue += bodyAngleYParam.Value * 0.2f;
                }
                
                if (enableClothBounce && Random.Range(0f, 1f) < 0.003f)
                {
                    finalValue += Random.Range(-0.3f, 0.3f) * intensity;
                }
                
                clothParam.Value = finalValue;
            }
            
            yield return new WaitForSeconds(0.06f);
        }
    }
    
    private IEnumerator EarAccessoryCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < earAccessoryParams.Count; i++)
            {
                var earParam = earAccessoryParams[i];
                if (earParam == null) continue;
                
                float offset = i * 1f;
                float speed = 0.4f * dynamicLevel;
                float intensity = 0.3f * dynamicLevel * dynamicBoost;
                
                float noiseValue = Mathf.PerlinNoise(Time.time * speed + offset, 0f);
                float finalValue = (noiseValue - 0.5f) * intensity;
                
                if (angleXParam != null && !isPerformingGesture)
                {
                    finalValue += angleXParam.Value * 0.8f;
                }
                
                if (angleYParam != null && !isPerformingGesture)
                {
                    finalValue += angleYParam.Value * 0.5f;
                }
                
                earParam.Value = finalValue;
            }
            
            yield return new WaitForSeconds(0.08f);
        }
    }
    
    private IEnumerator DynamicBoostCoroutine()
    {
        while (true)
        {
            if (!isTalking && enableAutoGestures)
            {
                dynamicBoost = Random.Range(0.8f, 1.5f);
                Debug.Log($"⚡ 역동성 부스트: {dynamicBoost:F2}x");
            }
            
            yield return new WaitForSeconds(Random.Range(5f, 15f));
        }
    }
    
    private IEnumerator SmoothParameterChange(CubismParameter param, float fromValue, float toValue, float duration)
    {
        if (param == null) yield break;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float smoothT = t * t * (3f - 2f * t);
            param.Value = Mathf.Lerp(fromValue, toValue, smoothT);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        param.Value = toValue;
    }
    
    // 외부 인터페이스
    public void SetTalkingState(bool talking)
    {
        isTalking = talking;
        
        if (talking)
        {
            if (breathParam != null)
            {
                breathParam.Value = 0f;
            }
        }
    }
    
    public void PauseEyeMovement(bool pause)
    {
        eyeMovementPaused = pause;
        Debug.Log($"👁️ 눈동자 자동 움직임: {(pause ? "일시정지" : "재개")}");
    }
    
    public void SetDynamicLevel(float level)
    {
        dynamicLevel = Mathf.Clamp(level, 0.5f, 3f);
        Debug.Log($"⚡ 역동성 레벨 설정: {dynamicLevel:F1}x");
    }
    
    public void ToggleAutoGestures(bool enabled)
    {
        enableAutoGestures = enabled;
        Debug.Log($"🤖 자동 제스처: {(enabled ? "ON" : "OFF")}");
    }
    
    public void ToggleTouchGestures(bool enabled)
    {
        enableTouchGestures = enabled;
        Debug.Log($"👆 터치 제스처: {(enabled ? "ON" : "OFF")}");
    }
    
    public void ToggleHeadEyeSync(bool enabled)
    {
        enableHeadEyeSync = enabled;
        Debug.Log($"👁️➡️🎭 눈-고개 연동: {(enabled ? "ON" : "OFF")}");
        
        // 연동 해제 시 고개와 몸체를 중앙으로 복귀
        if (!enabled && !isPerformingGesture)
        {
            if (angleXParam != null)
            {
                StartCoroutine(SmoothParameterChange(angleXParam, angleXParam.Value, 0f, 0.5f));
            }
            if (angleYParam != null)
            {
                StartCoroutine(SmoothParameterChange(angleYParam, angleYParam.Value, 0f, 0.5f));
            }
            if (bodyAngleXParam != null)
            {
                StartCoroutine(SmoothParameterChange(bodyAngleXParam, bodyAngleXParam.Value, 0f, 0.8f));
            }
            if (bodyAngleYParam != null)
            {
                StartCoroutine(SmoothParameterChange(bodyAngleYParam, bodyAngleYParam.Value, 0f, 0.8f));
            }
            if (bodyAngleZParam != null)
            {
                StartCoroutine(SmoothParameterChange(bodyAngleZParam, bodyAngleZParam.Value, 0f, 0.8f));
            }
        }
    }
    
    public void SetHeadFollowIntensity(float intensity)
    {
        headFollowIntensity = Mathf.Clamp(intensity, 0f, 1f);
        Debug.Log($"🎭 고개 따라가기 강도: {headFollowIntensity:F2}");
    }
    
    public void SetBodyFollowIntensity(float intensity)
    {
        bodyFollowIntensity = Mathf.Clamp(intensity, 0f, 0.5f);
        Debug.Log($"🏃 몸체 따라가기 강도: {bodyFollowIntensity:F2}");
    }
    
    public void ToggleBodyFollow(bool enabled)
    {
        enableBodyFollow = enabled;
        Debug.Log($"🏃 몸체 연동: {(enabled ? "ON" : "OFF")}");
        
        // 몸체 연동 해제 시 몸체를 중앙으로 복귀
        if (!enabled && !isPerformingGesture)
        {
            if (bodyAngleXParam != null)
            {
                StartCoroutine(SmoothParameterChange(bodyAngleXParam, bodyAngleXParam.Value, 0f, 1f));
            }
            if (bodyAngleYParam != null)
            {
                StartCoroutine(SmoothParameterChange(bodyAngleYParam, bodyAngleYParam.Value, 0f, 1f));
            }
            if (bodyAngleZParam != null)
            {
                StartCoroutine(SmoothParameterChange(bodyAngleZParam, bodyAngleZParam.Value, 0f, 1f));
            }
        }
    }
    
    public void StopAllAnimations()
    {
        if (blinkingCoroutine != null) StopCoroutine(blinkingCoroutine);
        if (breathingCoroutine != null) StopCoroutine(breathingCoroutine);
        if (autoGestureCoroutine != null) StopCoroutine(autoGestureCoroutine);
        if (microMovementCoroutine != null) StopCoroutine(microMovementCoroutine);
        if (eyeMovementCoroutine != null) StopCoroutine(eyeMovementCoroutine);
        if (hairAnimationCoroutine != null) StopCoroutine(hairAnimationCoroutine);
        if (clothAnimationCoroutine != null) StopCoroutine(clothAnimationCoroutine);
        if (earAccessoryCoroutine != null) StopCoroutine(earAccessoryCoroutine);
        if (dynamicBoostCoroutine != null) StopCoroutine(dynamicBoostCoroutine);
    }
    
    void OnDestroy()
    {
        StopAllAnimations();
    }
    
    // 디버그 메서드들
    [ContextMenu("👆 Test Touch Gesture")]
    public void TestTouchGesture()
    {
        TriggerTouchGesture();
    }
    
    [ContextMenu("🤖 Test Auto Gesture")]
    public void TestAutoGesture()
    {
        if (!isPerformingGesture)
        {
            StartCoroutine(PerformAutoGesture());
        }
    }
    
    [ContextMenu("🎲 Test Random Auto Gesture")]
    public void TestRandomAutoGesture()
    {
        if (!isPerformingGesture)
        {
            StartCoroutine(PerformAutoGesture());
        }
    }
    
    [ContextMenu("📊 Show System Status")]
    public void ShowSystemStatus()
    {
        Debug.Log("=== 통합 생동감 시스템 상태 ===");
        Debug.Log($"자동 제스처: {(enableAutoGestures ? "ON" : "OFF")}");
        Debug.Log($"터치 제스처: {(enableTouchGestures ? "ON" : "OFF")}");
        Debug.Log($"눈-고개 연동: {(enableHeadEyeSync ? "ON" : "OFF")}");
        Debug.Log($"몸체 연동: {(enableBodyFollow ? "ON" : "OFF")}");
        Debug.Log($"고개 따라가기 강도: {headFollowIntensity:F2}");
        Debug.Log($"몸체 따라가기 강도: {bodyFollowIntensity:F2}");
        Debug.Log($"역동성 레벨: {dynamicLevel:F1}x");
        Debug.Log($"현재 부스트: {dynamicBoost:F2}x");
        Debug.Log($"제스처 실행 중: {isPerformingGesture}");
        Debug.Log($"대화 중: {isTalking}");
        Debug.Log($"마지막 터치: {Time.time - lastTouchTime:F1}초 전");
    }
}