using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("Heart Images")]
    public Image[] hearts; // Inspector에서 Heart1, Heart2, Heart3, Heart4, Heart5 할당
    
    [Header("Heart Sprites")]
    public Sprite emptyHeart; // 빈 하트 스프라이트 (회색 또는 투명)
    public Sprite fullHeart;  // 찬 하트 스프라이트 (빨간색)
    
    [Header("Settings")]
    public int maxHearts = 5; // 최대 하트 개수

    // 강제 테스트 함수
    [ContextMenu("Force Test")]
    void ForceTest()
    {
        Debug.Log($"Hearts 배열 크기: {hearts.Length}");
        Debug.Log($"Empty Heart 스프라이트: {(emptyHeart != null ? emptyHeart.name : "NULL")}");
        Debug.Log($"Full Heart 스프라이트: {(fullHeart != null ? fullHeart.name : "NULL")}");
        
        if (hearts[0] != null)
        {
            Debug.Log($"Heart1 현재 스프라이트: {(hearts[0].sprite != null ? hearts[0].sprite.name : "NULL")}");
            
            // 스프라이트만 변경
            if (fullHeart != null)
            {
                hearts[0].sprite = fullHeart;
                Debug.Log($"Heart1 스프라이트를 {fullHeart.name}로 변경");
            }
            
            Debug.Log($"Heart1 변경 후 스프라이트: {(hearts[0].sprite != null ? hearts[0].sprite.name : "NULL")}");
        }
    }
    
    void Start()
    {
        // 초기에는 모든 하트 숨김
        HideAllHearts();
        // 그 다음 호감도에 따라 업데이트
        UpdateHearts();
    }
    
    void Update()
    {
        // 실시간으로 호감도 변화 감지
        UpdateHearts();
    }
    
    void HideAllHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].gameObject.SetActive(false);
            }
        }
        Debug.Log("모든 하트 숨김 처리 완료");
    }
    
    void UpdateHearts()
    {
        if (GameManager.Instance == null) 
        {
            Debug.LogWarning("GameManager.Instance가 null입니다!");
            return;
        }
        
        int currentAffinity = GameManager.Instance.affinity;
        Debug.Log($"현재 호감도: {currentAffinity}");
        
        // 호감도가 0이면 모든 하트 숨김
        if (currentAffinity <= 0)
        {
            HideAllHearts();
            return;
        }
        
        // 각 하트 이미지 업데이트
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) 
            {
                Debug.LogWarning($"Heart{i+1}이 할당되지 않았습니다!");
                continue;
            }
            
            if (i < currentAffinity)
            {
                // 호감도만큼 하트 보이기 + 스프라이트 변경
                hearts[i].gameObject.SetActive(true);
                
                if (fullHeart != null)
                {
                    hearts[i].sprite = fullHeart;
                    Debug.Log($"Heart{i+1} 보임 + Full Heart 스프라이트 적용");
                }
                else
                {
                    Debug.LogError("Full Heart 스프라이트가 할당되지 않았습니다!");
                }
            }
            else
            {
                // 나머지는 숨김
                hearts[i].gameObject.SetActive(false);
                Debug.Log($"Heart{i+1} 숨김");
            }
        }
        
        // 최대 하트 수를 넘으면 모든 하트 보이기
        if (currentAffinity >= maxHearts)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] != null)
                {
                    hearts[i].gameObject.SetActive(true);
                    hearts[i].sprite = fullHeart;
                }
            }
        }
    }
    
    // 테스트용 - Inspector에서 호감도 강제 변경
    [ContextMenu("Test Affinity +1")]
    void TestIncreaseAffinity()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.affinity++;
            Debug.Log($"테스트: 호감도 증가 -> {GameManager.Instance.affinity}");
        }
    }
    
    [ContextMenu("Test Reset Affinity")]
    void TestResetAffinity()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.affinity = 0;
            Debug.Log("테스트: 호감도 리셋");
        }
    }
}