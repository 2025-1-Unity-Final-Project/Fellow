// EnemyHealth.cs
using UnityEngine;
using UnityEngine.UI; // Slider 사용을 위해 필요

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public int attackDamageFromPlayer = 25; // 플레이어로부터 받을 기본 데미지

    [Header("체력 바 UI")] // ▼▼▼ Inspector에서 연결할 변수들 ▼▼▼
    public Slider healthBar; // 씬에 배치된 체력 바 슬라이더 (Inspector에서 연결)
    public Vector3 healthBarOffset = new Vector3(0, 2, 0); // 체력 바 위치 오프셋

 

    void Start()
    {
        currentHealth = maxHealth;

        // ▼▼▼ 씬에 있는 HealthBar를 직접 사용하므로 SpawnHealthBar() 함수 대신 여기서 초기화 ▼▼▼
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;

            // 캔버스가 World Space인지 확인하고, 아니라면 경고 메시지 출력
            Canvas canvas = healthBar.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogWarning("EnemyHealth: HealthBar가 World Space Canvas에 있지 않습니다. 렌더링 문제가 발생할 수 있습니다.");
            }

           
        }
        else
        {
            Debug.LogError("EnemyHealth: HealthBar 슬라이더가 Inspector에 할당되지 않았습니다!");
        }
        
    }

    void Update()
    {
        // 씬에 배치된 UI를 사용하므로, 매 프레임 위치를 업데이트
        if (healthBar != null)
        {
            // 캐릭터 머리 위에 체력 바 위치 설정 (월드 좌표)
            healthBar.transform.position = transform.position + healthBarOffset;

            
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current HP: " + currentHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died.");

        // ▼▼▼ GameManager.Instance.EnemyDestroyed() 호출 ▼▼▼
        // GameManager가 존재한다면, EnemyDestroyed 함수 호출
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyDestroyed();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
        // ▲▲▲ GameManager.Instance.EnemyDestroyed() 호출 ▲▲▲

        Destroy(gameObject);
    }
}
