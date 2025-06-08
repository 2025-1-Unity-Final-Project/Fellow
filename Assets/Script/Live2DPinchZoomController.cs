using System.Collections;
using UnityEngine;
using Live2D.Cubism.Core;

/// <summary>
/// Live2D 캐릭터 영역 제한 핀치 줌 컨트롤러
/// 캐릭터 범위를 벗어나지 않게 이동 제한하고, 캐릭터 영역 내에서만 터치 인식
/// </summary>
public class Live2DRestrictedZoomController : MonoBehaviour
{
    [Header("타겟 설정")]
    public Transform live2dCharacter;           // Live2D 캐릭터 Transform
    public Camera targetCamera;                 // 대상 카메라
    public CubismModel cubismModel;             // Live2D 모델 (영역 감지용)
    
    [Header("영역 제한 설정")]
    public bool useCharacterBounds = true;      // 캐릭터 경계 자동 감지
    public Rect manualBounds = new Rect(-2, -3, 4, 6);  // 수동 영역 설정
    public LayerMask touchLayer = -1;           // 터치 가능한 레이어
    
    [Header("줌 설정")]
    public float minScale = 0.5f;               // 최소 크기
    public float maxScale = 2.5f;               // 최대 크기
    public float zoomSensitivity = 1f;          // 줌 민감도
    public float smoothTime = 0.1f;             // 부드러운 줌 시간
    
    [Header("이동 제한")]
    public bool restrictToCharacterArea = true;  // 캐릭터 영역 내 이동만 허용
    public float boundsPadding = 0.5f;          // 경계 여백
    public bool enableDrag = true;              // 드래그 활성화
    public float dragSensitivity = 1f;          // 드래그 민감도
    
    [Header("터치 영역 제한")]
    public bool onlyTouchCharacter = true;      // 캐릭터 터치 시에만 반응
    public float touchDetectionRadius = 1f;     // 터치 감지 반경
    
    [Header("더블탭 설정")]
    public bool enableDoubleTap = true;         // 더블탭 활성화
    public float doubleTapTime = 0.3f;          // 더블탭 인식 시간
    public float doubleTapResetScale = 1f;      // 더블탭 시 리셋 크기
    
    [Header("시각적 피드백")]
    public bool showBounds = true;              // 경계 표시
    public Color boundsColor = Color.yellow;    // 경계 색상
    public bool showTouchArea = true;           // 터치 영역 표시
    public Color touchAreaColor = Color.green;  // 터치 영역 색상
    
    // 내부 변수들
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Vector3 targetScale;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private Rect characterBounds;
    private Rect allowedMovementArea;
    
    // 터치 상태 관리
    private bool isPinching = false;
    private bool isDragging = false;
    private bool isValidTouch = false;
    private float lastPinchDistance = 0f;
    private Vector2 lastTouchPosition;
    
    // 더블탭 감지
    private float lastTapTime = 0f;
    private int tapCount = 0;
    
    // 컴포넌트 참조
    private Collider2D characterCollider;
    private Renderer characterRenderer;
    
    void Start()
    {
        InitializeController();
        CalculateCharacterBounds();
        UpdateMovementArea();
    }
    
    void Update()
    {
        HandleTouchInput();
        UpdateTransform();
        
        if (Application.isEditor && showBounds)
        {
            DrawDebugBounds();
        }
    }
    
    private void InitializeController()
    {
        // 타겟 설정
        if (live2dCharacter == null)
        {
            live2dCharacter = transform;
        }
        
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        
        if (cubismModel == null)
        {
            cubismModel = live2dCharacter.GetComponent<CubismModel>();
        }
        
        // 컴포넌트 찾기
        characterCollider = live2dCharacter.GetComponent<Collider2D>();
        characterRenderer = live2dCharacter.GetComponent<Renderer>();
        
        // 콜라이더가 없으면 자동 생성
        if (characterCollider == null && onlyTouchCharacter)
        {
            CreateCharacterCollider();
        }
        
        // 초기값 저장
        initialScale = live2dCharacter.localScale;
        initialPosition = live2dCharacter.position;
        targetScale = initialScale;
        targetPosition = initialPosition;
        
        Debug.Log("🎮 Live2D 영역 제한 줌 컨트롤러 초기화 완료!");
    }
    
    private void CreateCharacterCollider()
    {
        // BoxCollider2D 자동 생성
        var boxCollider = live2dCharacter.gameObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        
        // Live2D 모델 크기에 맞춰 콜라이더 크기 설정
        if (characterRenderer != null)
        {
            boxCollider.size = characterRenderer.bounds.size;
        }
        else
        {
            boxCollider.size = new Vector2(2f, 3f); // 기본 크기
        }
        
        characterCollider = boxCollider;
        Debug.Log("📦 캐릭터 콜라이더 자동 생성됨");
    }
    
    private void CalculateCharacterBounds()
    {
        if (useCharacterBounds)
        {
            if (characterRenderer != null)
            {
                Bounds bounds = characterRenderer.bounds;
                characterBounds = new Rect(
                    bounds.center.x - bounds.size.x / 2,
                    bounds.center.y - bounds.size.y / 2,
                    bounds.size.x,
                    bounds.size.y
                );
            }
            else if (characterCollider != null)
            {
                Bounds bounds = characterCollider.bounds;
                characterBounds = new Rect(
                    bounds.center.x - bounds.size.x / 2,
                    bounds.center.y - bounds.size.y / 2,
                    bounds.size.x,
                    bounds.size.y
                );
            }
            else
            {
                // Live2D 모델의 파라미터 범위 기반으로 추정
                characterBounds = new Rect(
                    initialPosition.x - 1f,
                    initialPosition.y - 1.5f,
                    2f,
                    3f
                );
            }
        }
        else
        {
            characterBounds = manualBounds;
        }
        
        Debug.Log($"📏 캐릭터 영역: {characterBounds}");
    }
    
    private void UpdateMovementArea()
    {
        // 현재 스케일에 따른 이동 가능 영역 계산
        float currentScale = targetScale.x / initialScale.x;
        
        // 스케일이 클수록 이동 범위 축소
        float scaledPadding = boundsPadding / currentScale;
        
        allowedMovementArea = new Rect(
            characterBounds.x + scaledPadding,
            characterBounds.y + scaledPadding,
            characterBounds.width - scaledPadding * 2,
            characterBounds.height - scaledPadding * 2
        );
    }
    
    private bool IsValidTouchPosition(Vector2 screenPos)
    {
        if (!onlyTouchCharacter) return true;
        
        // 스크린 좌표를 월드 좌표로 변환
        Vector3 worldPos = targetCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, targetCamera.nearClipPlane));
        
        // 캐릭터 영역 내인지 확인
        if (characterBounds.Contains(new Vector2(worldPos.x, worldPos.y)))
        {
            return true;
        }
        
        // 콜라이더가 있으면 콜라이더 검사
        if (characterCollider != null)
        {
            return characterCollider.bounds.Contains(worldPos);
        }
        
        // 터치 감지 반경 내인지 확인
        float distance = Vector2.Distance(new Vector2(worldPos.x, worldPos.y), new Vector2(initialPosition.x, initialPosition.y));
        return distance <= touchDetectionRadius;
    }
    
    private void HandleTouchInput()
    {
        int touchCount = Input.touchCount;
        
        if (touchCount == 0)
        {
            if (isPinching || isDragging)
            {
                OnTouchEnd();
            }
            return;
        }
        
        // 첫 번째 터치가 유효한 영역인지 확인
        if (!isValidTouch && touchCount > 0)
        {
            isValidTouch = IsValidTouchPosition(Input.GetTouch(0).position);
            if (!isValidTouch) return;
        }
        
        if (touchCount == 1)
        {
            HandleSingleTouch();
        }
        else if (touchCount >= 2)
        {
            HandlePinchZoom();
        }
    }
    
    private void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);
        Vector2 touchPos = touch.position;
        
        switch (touch.phase)
        {
            case TouchPhase.Began:
                OnSingleTouchBegan(touchPos);
                break;
                
            case TouchPhase.Moved:
                if (enableDrag && isDragging)
                {
                    HandleRestrictedDrag(touchPos);
                }
                break;
                
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                OnSingleTouchEnded(touchPos);
                break;
        }
    }
    
    private void OnSingleTouchBegan(Vector2 touchPos)
    {
        lastTouchPosition = touchPos;
        isDragging = false;
        
        // 더블탭 감지
        if (enableDoubleTap)
        {
            float timeSinceLastTap = Time.time - lastTapTime;
            
            if (timeSinceLastTap < doubleTapTime)
            {
                tapCount++;
                if (tapCount >= 2)
                {
                    OnDoubleTap();
                    tapCount = 0;
                    return;
                }
            }
            else
            {
                tapCount = 1;
            }
            
            lastTapTime = Time.time;
        }
        
        // 드래그 시작 준비
        StartCoroutine(DetectDragStart());
    }
    
    private void OnSingleTouchEnded(Vector2 touchPos)
    {
        isDragging = false;
    }
    
    private IEnumerator DetectDragStart()
    {
        float startTime = Time.time;
        Vector2 startPos = lastTouchPosition;
        
        while (Input.touchCount > 0 && Time.time - startTime < 0.1f)
        {
            Touch touch = Input.GetTouch(0);
            float distance = Vector2.Distance(touch.position, startPos);
            
            if (distance > 30f)
            {
                isDragging = true;
                yield break;
            }
            
            yield return null;
        }
    }
    
    private void HandleRestrictedDrag(Vector2 currentTouchPos)
    {
        Vector2 deltaPos = currentTouchPos - lastTouchPosition;
        
        // 스크린 좌표를 월드 좌표로 변환
        Vector3 worldDelta = targetCamera.ScreenToWorldPoint(new Vector3(deltaPos.x, deltaPos.y, targetCamera.nearClipPlane));
        worldDelta *= dragSensitivity;
        
        // 새 위치 계산
        Vector3 newPosition = targetPosition + new Vector3(worldDelta.x, worldDelta.y, 0);
        
        // 이동 영역 제한 적용
        if (restrictToCharacterArea)
        {
            UpdateMovementArea();
            
            newPosition.x = Mathf.Clamp(newPosition.x, 
                allowedMovementArea.x, 
                allowedMovementArea.x + allowedMovementArea.width);
            newPosition.y = Mathf.Clamp(newPosition.y, 
                allowedMovementArea.y, 
                allowedMovementArea.y + allowedMovementArea.height);
        }
        
        targetPosition = newPosition;
        lastTouchPosition = currentTouchPos;
        
        Debug.Log($"🖱️ 제한된 드래그: {targetPosition}");
    }
    
    private void HandlePinchZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);
        
        // 두 터치 모두 유효한 영역인지 확인
        if (!IsValidTouchPosition(touch1.position) || !IsValidTouchPosition(touch2.position))
        {
            return;
        }
        
        Vector2 touch1Pos = touch1.position;
        Vector2 touch2Pos = touch2.position;
        float currentPinchDistance = Vector2.Distance(touch1Pos, touch2Pos);
        
        if (!isPinching)
        {
            isPinching = true;
            lastPinchDistance = currentPinchDistance;
            isDragging = false;
            return;
        }
        
        // 핀치 줌 계산
        if (lastPinchDistance > 0)
        {
            float pinchRatio = currentPinchDistance / lastPinchDistance;
            Vector3 newScale = targetScale * pinchRatio;
            
            // 스케일 제한 적용
            float scaleMagnitude = newScale.magnitude / initialScale.magnitude;
            scaleMagnitude = Mathf.Clamp(scaleMagnitude, minScale, maxScale);
            
            targetScale = initialScale * scaleMagnitude;
            
            // 스케일 변경 시 이동 영역 업데이트
            UpdateMovementArea();
            
            // 현재 위치가 새로운 영역을 벗어나면 조정
            if (restrictToCharacterArea)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, 
                    allowedMovementArea.x, 
                    allowedMovementArea.x + allowedMovementArea.width);
                targetPosition.y = Mathf.Clamp(targetPosition.y, 
                    allowedMovementArea.y, 
                    allowedMovementArea.y + allowedMovementArea.height);
            }
            
            Debug.Log($"🔍 제한된 핀치 줌: {scaleMagnitude:F2}x");
        }
        
        lastPinchDistance = currentPinchDistance;
    }
    
    private void OnTouchEnd()
    {
        isPinching = false;
        isDragging = false;
        isValidTouch = false;
    }
    
    private void OnDoubleTap()
    {
        Debug.Log("👆👆 더블탭 리셋!");
        
        targetScale = initialScale * doubleTapResetScale;
        targetPosition = initialPosition;
        
        StartCoroutine(SmoothResetAnimation());
    }
    
    private IEnumerator SmoothResetAnimation()
    {
        Vector3 startScale = live2dCharacter.localScale;
        Vector3 startPosition = live2dCharacter.position;
        
        float elapsedTime = 0f;
        float duration = 0.3f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            live2dCharacter.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
            live2dCharacter.position = Vector3.Lerp(startPosition, targetPosition, smoothT);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        live2dCharacter.localScale = targetScale;
        live2dCharacter.position = targetPosition;
    }
    
    private void UpdateTransform()
    {
        live2dCharacter.localScale = Vector3.SmoothDamp(
            live2dCharacter.localScale, 
            targetScale, 
            ref velocity, 
            smoothTime
        );
        
        live2dCharacter.position = Vector3.Lerp(
            live2dCharacter.position, 
            targetPosition, 
            Time.deltaTime / smoothTime
        );
    }
    
    private void DrawDebugBounds()
    {
        if (showBounds)
        {
            // 캐릭터 영역 그리기
            Debug.DrawLine(
                new Vector3(characterBounds.x, characterBounds.y),
                new Vector3(characterBounds.x + characterBounds.width, characterBounds.y),
                boundsColor
            );
            Debug.DrawLine(
                new Vector3(characterBounds.x + characterBounds.width, characterBounds.y),
                new Vector3(characterBounds.x + characterBounds.width, characterBounds.y + characterBounds.height),
                boundsColor
            );
            Debug.DrawLine(
                new Vector3(characterBounds.x + characterBounds.width, characterBounds.y + characterBounds.height),
                new Vector3(characterBounds.x, characterBounds.y + characterBounds.height),
                boundsColor
            );
            Debug.DrawLine(
                new Vector3(characterBounds.x, characterBounds.y + characterBounds.height),
                new Vector3(characterBounds.x, characterBounds.y),
                boundsColor
            );
        }
        
        if (showTouchArea && onlyTouchCharacter)
        {
            // 터치 감지 영역 그리기
            Vector3 center = new Vector3(initialPosition.x, initialPosition.y);
            int segments = 32;
            for (int i = 0; i < segments; i++)
            {
                float angle1 = (float)i / segments * Mathf.PI * 2;
                float angle2 = (float)(i + 1) / segments * Mathf.PI * 2;
                
                Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1)) * touchDetectionRadius;
                Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2)) * touchDetectionRadius;
                
                Debug.DrawLine(point1, point2, touchAreaColor);
            }
        }
    }
    
    // 외부 인터페이스
    public void SetCharacterBounds(Rect bounds)
    {
        manualBounds = bounds;
        characterBounds = bounds;
        useCharacterBounds = false;
        UpdateMovementArea();
        Debug.Log($"📏 수동 영역 설정: {bounds}");
    }
    
    public void SetScale(float scale)
    {
        scale = Mathf.Clamp(scale, minScale, maxScale);
        targetScale = initialScale * scale;
        UpdateMovementArea();
        Debug.Log($"🔧 스케일 설정: {scale:F2}x");
    }
    
    public void ResetTransform()
    {
        targetScale = initialScale * doubleTapResetScale;
        targetPosition = initialPosition;
        Debug.Log("🔄 Transform 리셋");
    }
    
    public float GetCurrentScale()
    {
        return live2dCharacter.localScale.magnitude / initialScale.magnitude;
    }
    
    public Rect GetCharacterBounds()
    {
        return characterBounds;
    }
    
    public void RecalculateBounds()
    {
        CalculateCharacterBounds();
        UpdateMovementArea();
        Debug.Log("🔄 영역 재계산 완료");
    }
    
    // 디버그 메서드들
    [ContextMenu("🔍 Test Zoom In")]
    public void TestZoomIn()
    {
        SetScale(GetCurrentScale() * 1.5f);
    }
    
    [ContextMenu("🔍 Test Zoom Out")]
    public void TestZoomOut()
    {
        SetScale(GetCurrentScale() * 0.75f);
    }
    
    [ContextMenu("🔄 Test Reset")]
    public void TestReset()
    {
        ResetTransform();
    }
    
    [ContextMenu("📏 Recalculate Bounds")]
    public void TestRecalculateBounds()
    {
        RecalculateBounds();
    }
    
    [ContextMenu("📊 Show Status")]
    public void ShowStatus()
    {
        Debug.Log("=== Live2D 제한된 줌 상태 ===");
        Debug.Log($"현재 스케일: {GetCurrentScale():F2}x");
        Debug.Log($"현재 위치: {live2dCharacter.position}");
        Debug.Log($"캐릭터 영역: {characterBounds}");
        Debug.Log($"이동 가능 영역: {allowedMovementArea}");
        Debug.Log($"핀치 중: {isPinching}");
        Debug.Log($"드래그 중: {isDragging}");
        Debug.Log($"유효 터치: {isValidTouch}");
    }
    
    void OnDrawGizmosSelected()
    {
        if (showBounds)
        {
            // 캐릭터 영역 표시
            Gizmos.color = boundsColor;
            Vector3 center = new Vector3(characterBounds.center.x, characterBounds.center.y, 0);
            Vector3 size = new Vector3(characterBounds.width, characterBounds.height, 0);
            Gizmos.DrawWireCube(center, size);
            
            // 이동 가능 영역 표시
            Gizmos.color = Color.blue;
            Vector3 moveCenter = new Vector3(allowedMovementArea.center.x, allowedMovementArea.center.y, 0);
            Vector3 moveSize = new Vector3(allowedMovementArea.width, allowedMovementArea.height, 0);
            Gizmos.DrawWireCube(moveCenter, moveSize);
        }
        
        if (showTouchArea && onlyTouchCharacter)
        {
            // 터치 감지 영역 표시
            Gizmos.color = touchAreaColor;
            if (live2dCharacter != null)
            {
                Gizmos.DrawWireSphere(initialPosition, touchDetectionRadius);
            }
        }
        
        // 현재 위치 표시
        if (live2dCharacter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(live2dCharacter.position, 0.1f);
        }
    }
}