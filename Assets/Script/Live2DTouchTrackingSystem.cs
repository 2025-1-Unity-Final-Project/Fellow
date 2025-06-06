using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;

public class Live2DTouchTrackingSystem : MonoBehaviour
{
    [Header("Live2D Components")]
    public CubismModel cubismModel;
    public Live2DCharacterLifeSystem lifeSystem;
    
    [Header("실시간 터치 추적 설정")]
    public bool enableTouchTracking = true;
    public float trackingSpeed = 5f;              // 추적 속도
    public float returnSpeed = 2f;                // 복귀 속도
    
    [Header("범위 제한")]
    public float maxEyeRange = 0.8f;
    public float maxHeadAngle = 20f;
    public float maxBodyAngle = 8f;
    
    [Header("반응 강도 설정")]
    [Range(0f, 1f)]
    public float eyeTrackingStrength = 1f;
    [Range(0f, 1f)]
    public float headTrackingStrength = 0.6f;
    [Range(0f, 1f)]
    public float bodyTrackingStrength = 0.3f;
    
    // 파라미터 캐시
    private CubismParameter eyeBallXParam;
    private CubismParameter eyeBallYParam;
    private CubismParameter angleXParam;
    private CubismParameter angleYParam;
    private CubismParameter angleZParam;
    private CubismParameter bodyAngleXParam;
    private CubismParameter bodyAngleYParam;
    
    // 추적 상태
    private bool isTouchActive = false;
    private bool isTrackingActive = false;
    private Vector3 currentTouchPosition;
    private Coroutine trackingCoroutine;
    
    // 원래 값 저장
    private float originalEyeX;
    private float originalEyeY;
    private float originalAngleX;
    private float originalAngleY;
    private float originalAngleZ;
    private float originalBodyX;
    private float originalBodyY;
    
    // 터치 감지 개선
    private bool wasTouchActive = false;
    private float touchStartTime;
    private Vector3 lastTouchPosition;
    
    void Start()
    {
        InitializeParameters();
        SaveOriginalValues();
    }
    
    void Update()
    {
        if (enableTouchTracking)
        {
            HandleTouchInput();
        }
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
        
        foreach (var param in parameters)
        {
            string id = param.Id;
            
            if (eyeBallXParam == null && (id.Contains("ParamEyeBallX") || (id.Contains("眼球") && id.Contains("X"))))
                eyeBallXParam = param;
            else if (eyeBallYParam == null && (id.Contains("ParamEyeBallY") || (id.Contains("眼球") && id.Contains("Y"))))
                eyeBallYParam = param;
            else if (angleXParam == null && (id.Contains("ParamAngleX") || (id.Contains("角度") && id.Contains("X"))))
                angleXParam = param;
            else if (angleYParam == null && (id.Contains("ParamAngleY") || (id.Contains("角度") && id.Contains("Y"))))
                angleYParam = param;
            else if (angleZParam == null && (id.Contains("ParamAngleZ") || (id.Contains("角도") && id.Contains("Z"))))
                angleZParam = param;
            else if (bodyAngleXParam == null && (id.Contains("ParamBodyAngleX") || (id.Contains("身体旋转") && id.Contains("X"))))
                bodyAngleXParam = param;
            else if (bodyAngleYParam == null && (id.Contains("ParamBodyAngleY") || (id.Contains("身体旋转") && id.Contains("Y"))))
                bodyAngleYParam = param;
        }
        
        Debug.Log($"👀 터치 추적 시스템 초기화 완료");
    }
    
    private void SaveOriginalValues()
    {
        originalEyeX = eyeBallXParam?.Value ?? 0f;
        originalEyeY = eyeBallYParam?.Value ?? 0f;
        originalAngleX = angleXParam?.Value ?? 0f;
        originalAngleY = angleYParam?.Value ?? 0f;
        originalAngleZ = angleZParam?.Value ?? 0f;
        originalBodyX = bodyAngleXParam?.Value ?? 0f;
        originalBodyY = bodyAngleYParam?.Value ?? 0f;
    }
    
    private void HandleTouchInput()
    {
        bool currentTouchActive = false;
        Vector3 touchPosition = Vector3.zero;
        
        // 터치 입력 감지 개선
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            // 터치가 시작되거나 움직이거나 유지될 때
            if (touch.phase == TouchPhase.Began || 
                touch.phase == TouchPhase.Moved || 
                touch.phase == TouchPhase.Stationary)
            {
                currentTouchActive = true;
                touchPosition = touch.position;
                
                // 터치 시작 시간 기록
                if (touch.phase == TouchPhase.Began)
                {
                    touchStartTime = Time.time;
                    Debug.Log("👆 터치 시작!");
                }
            }
            // 터치가 끝날 때
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                currentTouchActive = false;
                Debug.Log("👆 터치 종료!");
            }
        }
        // 에디터에서 마우스 테스트
        else if (Application.isEditor)
        {
            if (Input.GetMouseButton(0))
            {
                currentTouchActive = true;
                touchPosition = Input.mousePosition;
                
                if (Input.GetMouseButtonDown(0))
                {
                    touchStartTime = Time.time;
                    Debug.Log("🖱️ 마우스 클릭 시작!");
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                currentTouchActive = false;
                Debug.Log("🖱️ 마우스 클릭 종료!");
            }
        }
        
        // 터치 상태 변화 처리
        if (currentTouchActive != wasTouchActive)
        {
            if (currentTouchActive)
            {
                // 터치 시작
                StartTracking(touchPosition);
            }
            else
            {
                // 터치 종료
                StopTracking();
            }
            wasTouchActive = currentTouchActive;
        }
        
        // 터치 중일 때 위치 업데이트
        if (currentTouchActive && isTrackingActive)
        {
            // 위치가 실제로 변했을 때만 업데이트
            if (Vector3.Distance(touchPosition, lastTouchPosition) > 10f) // 10픽셀 이상 차이날 때
            {
                currentTouchPosition = touchPosition;
                lastTouchPosition = touchPosition;
            }
        }
        
        isTouchActive = currentTouchActive;
    }
    
    private void StartTracking(Vector3 touchPosition)
    {
        currentTouchPosition = touchPosition;
        lastTouchPosition = touchPosition;
        
        // 기존 추적 중단
        if (trackingCoroutine != null)
        {
            StopCoroutine(trackingCoroutine);
        }
        
        // 생동감 시스템 완전히 일시정지
        if (lifeSystem != null)
        {
            lifeSystem.PauseEyeMovement(true);
            lifeSystem.SetTrackingMode(true); // 다른 생동감도 줄이기
        }
        
        isTrackingActive = true;
        trackingCoroutine = StartCoroutine(RealTimeTrackingCoroutine());
        
        Debug.Log("🎯 실시간 추적 시작!");
    }
    
    private void StopTracking()
    {
        isTrackingActive = false;
        
        // 추적 코루틴 중단
        if (trackingCoroutine != null)
        {
            StopCoroutine(trackingCoroutine);
        }
        
        // 복귀 애니메이션 시작
        trackingCoroutine = StartCoroutine(ReturnToOriginalCoroutine());
        
        Debug.Log("🔄 추적 종료 - 복귀 시작!");
    }
    
    private IEnumerator RealTimeTrackingCoroutine()
    {
        while (isTrackingActive && isTouchActive)
        {
            // 화면 좌표를 -1~1 범위로 정규화
            float normalizedX = (currentTouchPosition.x / Screen.width - 0.5f) * 2f;
            float normalizedY = (currentTouchPosition.y / Screen.height - 0.5f) * 2f;
            
            // 범위 제한
            normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);
            normalizedY = Mathf.Clamp(normalizedY, -1f, 1f);
            
            // 목표값 계산
            float targetEyeX = normalizedX * maxEyeRange * eyeTrackingStrength;
            float targetEyeY = normalizedY * maxEyeRange * eyeTrackingStrength;
            float targetAngleY = normalizedX * maxHeadAngle * headTrackingStrength;
            float targetAngleX = -normalizedY * maxHeadAngle * 0.4f * headTrackingStrength;
            float targetAngleZ = normalizedX * 8f * headTrackingStrength;
            float targetBodyY = normalizedX * maxBodyAngle * bodyTrackingStrength;
            float targetBodyX = -normalizedY * maxBodyAngle * 0.3f * bodyTrackingStrength;
            
            // 현재값에서 목표값으로 부드럽게 보간
            float lerpSpeed = trackingSpeed * Time.deltaTime;
            
            if (eyeBallXParam != null)
                eyeBallXParam.Value = Mathf.Lerp(eyeBallXParam.Value, targetEyeX, lerpSpeed);
            if (eyeBallYParam != null)
                eyeBallYParam.Value = Mathf.Lerp(eyeBallYParam.Value, targetEyeY, lerpSpeed);
            
            if (angleXParam != null)
                angleXParam.Value = Mathf.Lerp(angleXParam.Value, targetAngleX, lerpSpeed * 0.7f);
            if (angleYParam != null)
                angleYParam.Value = Mathf.Lerp(angleYParam.Value, targetAngleY, lerpSpeed * 0.7f);
            if (angleZParam != null)
                angleZParam.Value = Mathf.Lerp(angleZParam.Value, targetAngleZ, lerpSpeed * 0.5f);
                
            if (bodyAngleXParam != null)
                bodyAngleXParam.Value = Mathf.Lerp(bodyAngleXParam.Value, targetBodyX, lerpSpeed * 0.3f);
            if (bodyAngleYParam != null)
                bodyAngleYParam.Value = Mathf.Lerp(bodyAngleYParam.Value, targetBodyY, lerpSpeed * 0.3f);
            
            yield return null; // 매 프레임마다 업데이트
        }
    }
    
    private IEnumerator ReturnToOriginalCoroutine()
    {
        float elapsedTime = 0f;
        float returnDuration = 1f / returnSpeed;
        
        // 현재 파라미터 값들을 시작점으로
        float startEyeX = eyeBallXParam?.Value ?? 0f;
        float startEyeY = eyeBallYParam?.Value ?? 0f;
        float startAngleX = angleXParam?.Value ?? 0f;
        float startAngleY = angleYParam?.Value ?? 0f;
        float startAngleZ = angleZParam?.Value ?? 0f;
        float startBodyX = bodyAngleXParam?.Value ?? 0f;
        float startBodyY = bodyAngleYParam?.Value ?? 0f;
        
        while (elapsedTime < returnDuration)
        {
            float t = elapsedTime / returnDuration;
            float smoothT = t * t * (3f - 2f * t); // Smoothstep
            
            // 원래값으로 부드럽게 복귀
            if (eyeBallXParam != null)
                eyeBallXParam.Value = Mathf.Lerp(startEyeX, originalEyeX, smoothT);
            if (eyeBallYParam != null)
                eyeBallYParam.Value = Mathf.Lerp(startEyeY, originalEyeY, smoothT);
            if (angleXParam != null)
                angleXParam.Value = Mathf.Lerp(startAngleX, originalAngleX, smoothT);
            if (angleYParam != null)
                angleYParam.Value = Mathf.Lerp(startAngleY, originalAngleY, smoothT);
            if (angleZParam != null)
                angleZParam.Value = Mathf.Lerp(startAngleZ, originalAngleZ, smoothT);
            if (bodyAngleXParam != null)
                bodyAngleXParam.Value = Mathf.Lerp(startBodyX, originalBodyX, smoothT);
            if (bodyAngleYParam != null)
                bodyAngleYParam.Value = Mathf.Lerp(startBodyY, originalBodyY, smoothT);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 최종 원래값 설정
        if (eyeBallXParam != null) eyeBallXParam.Value = originalEyeX;
        if (eyeBallYParam != null) eyeBallYParam.Value = originalEyeY;
        if (angleXParam != null) angleXParam.Value = originalAngleX;
        if (angleYParam != null) angleYParam.Value = originalAngleY;
        if (angleZParam != null) angleZParam.Value = originalAngleZ;
        if (bodyAngleXParam != null) bodyAngleXParam.Value = originalBodyX;
        if (bodyAngleYParam != null) bodyAngleYParam.Value = originalBodyY;
        
        // 생동감 시스템 재개
        if (lifeSystem != null)
        {
            lifeSystem.PauseEyeMovement(false);
            lifeSystem.SetTrackingMode(false);
        }
        
        Debug.Log("✅ 원래 자세로 복귀 완료!");
    }
    
    // 외부에서 추적 상태 확인
    public bool IsTracking()
    {
        return isTrackingActive;
    }
    
    // 강제 추적 중단
    public void ForceStopTracking()
    {
        if (isTrackingActive)
        {
            StopTracking();
        }
    }
    
    void OnDestroy()
    {
        if (trackingCoroutine != null) StopCoroutine(trackingCoroutine);
    }
    
    // 디버깅용
    void OnGUI()
    {
        if (Application.isEditor)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"Touch Active: {isTouchActive}");
            GUI.Label(new Rect(10, 30, 300, 20), $"Tracking Active: {isTrackingActive}");
            GUI.Label(new Rect(10, 50, 300, 20), $"Touch Position: {currentTouchPosition}");
        }
    }
}