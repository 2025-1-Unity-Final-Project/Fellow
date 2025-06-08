using UnityEngine;
using Live2D.Cubism.Framework.Expression;

public class Live2DAffinityExpression : MonoBehaviour
{
    [Header("Live2D Components")]
    public CubismExpressionController expressionController;
    
    [Header("Affinity Expression Ranges")]
    public int[] affinityLevel0Expressions = {0, 1}; // 호감도 0: 무표정 계열
    public int[] affinityLevel1Expressions = {2, 3, 4}; // 호감도 1: 약간 호의적
    public int[] affinityLevel2Expressions = {5, 6, 7}; // 호감도 2: 친근함
    public int[] affinityLevel3Expressions = {8, 9}; // 호감도 3: 기쁨
    public int[] affinityLevel4Expressions = {10}; // 호감도 4: 매우 기쁨
    public int[] affinityLevel5Expressions = {11}; // 호감도 5: 최고 호감
    
    private int currentAffinity = -1; // 이전 호감도 체크용
    
    void Start()
    {
        // CubismExpressionController가 없으면 자동으로 찾기
        if (expressionController == null)
        {
            expressionController = GetComponent<CubismExpressionController>();
        }
        
        if (expressionController == null)
        {
            Debug.LogError("CubismExpressionController를 찾을 수 없습니다!");
            return;
        }
        
        // 초기 표정 설정
        UpdateExpressionByAffinity();
    }
    
    void Update()
    {
        // 호감도가 변경되었을 때만 표정 업데이트
        if (GameManager.Instance != null && 
            GameManager.Instance.affinity != currentAffinity)
        {
            UpdateExpressionByAffinity();
        }
    }
    
    public void UpdateExpressionByAffinity()
    {
        if (GameManager.Instance == null || expressionController == null) return;
        
        int affinity = GameManager.Instance.affinity;
        currentAffinity = affinity;
        
        // 호감도에 따른 표정 배열 선택
        int[] availableExpressions = GetExpressionsForAffinity(affinity);
        
        if (availableExpressions.Length > 0)
        {
            // 랜덤하게 표정 선택
            int randomIndex = Random.Range(0, availableExpressions.Length);
            int expressionIndex = availableExpressions[randomIndex];
            
            // 표정 적용 - Live2D SDK 올바른 방법
            SetExpression(expressionIndex);
            
            Debug.Log($"호감도 {affinity}: 표정 인덱스 {expressionIndex} 적용 (랜덤 선택)");
        }
    }
    
    private int[] GetExpressionsForAffinity(int affinity)
    {
        switch (affinity)
        {
            case 0: return affinityLevel0Expressions;
            case 1: return affinityLevel1Expressions;
            case 2: return affinityLevel2Expressions;
            case 3: return affinityLevel3Expressions;
            case 4: return affinityLevel4Expressions;
            case 5: return affinityLevel5Expressions;
            default: 
                // 호감도 5 초과시 최고 레벨 표정 사용
                if (affinity > 5) return affinityLevel5Expressions;
                // 호감도 0 미만시 기본 표정 사용
                return affinityLevel0Expressions;
        }
    }
    
    private void SetExpression(int expressionIndex)
    {
        if (expressionController == null) return;
        
        // ExpressionsList가 있는지 확인
        if (expressionController.ExpressionsList == null || 
            expressionController.ExpressionsList.CubismExpressionObjects == null)
        {
            Debug.LogError("ExpressionsList가 설정되지 않았습니다!");
            return;
        }
        
        // 유효한 인덱스인지 확인
        var expressionObjects = expressionController.ExpressionsList.CubismExpressionObjects;
        if (expressionIndex < 0 || expressionIndex >= expressionObjects.Length)
        {
            Debug.LogError($"잘못된 표정 인덱스: {expressionIndex}, 최대: {expressionObjects.Length - 1}");
            return;
        }
        
        // CurrentExpressionIndex 변경으로 표정 적용 (Live2D SDK 공식 방법)
        expressionController.CurrentExpressionIndex = expressionIndex;
        
        Debug.Log($"표정 적용됨: 인덱스 {expressionIndex}");
    }
    
    // 수동으로 특정 표정 설정 (디버깅용)
    [ContextMenu("Test Random Expression")]
    public void TestRandomExpression()
    {
        if (expressionController != null && 
            expressionController.ExpressionsList != null &&
            expressionController.ExpressionsList.CubismExpressionObjects != null)
        {
            int maxExpressions = expressionController.ExpressionsList.CubismExpressionObjects.Length;
            if (maxExpressions > 0)
            {
                int randomIndex = Random.Range(0, maxExpressions);
                SetExpression(randomIndex);
            }
        }
    }
    
    // 표정 개수 확인 (디버깅용)
    [ContextMenu("Check Expression Count")]
    public void CheckExpressionCount()
    {
        if (expressionController != null && 
            expressionController.ExpressionsList != null &&
            expressionController.ExpressionsList.CubismExpressionObjects != null)
        {
            int count = expressionController.ExpressionsList.CubismExpressionObjects.Length;
            Debug.Log($"사용 가능한 표정 개수: {count}");
            
            for (int i = 0; i < count; i++)
            {
                var expression = expressionController.ExpressionsList.CubismExpressionObjects[i];
                Debug.Log($"표정 {i}: {(expression != null ? expression.name : "null")}");
            }
        }
        else
        {
            Debug.LogError("ExpressionsList가 설정되지 않았습니다!");
        }
    }
}