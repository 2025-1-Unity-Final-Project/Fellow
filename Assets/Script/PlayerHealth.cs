// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI; // Slider 사용을 위해 필요
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; // 최대 체력을 100으로 고정하거나, Inspector에서 설정
    public float currentHealth;
    // public Image healthBarFill; // Image 대신 Slider 사용 ▼▼▼
    public Slider healthSlider;    // Inspector에서 Slider 컴포넌트를 연결합니다.

    [Header("Invincibility Settings")]
    public bool isInvincible = false;
    public float invincibilityDuration = 1f;

    [Header("Damage from Enemy Settings")]
    public float damageFromEnemy = 10f;
    public string enemyTag = "Enemy";

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public float damageFlashDuration = 0.2f;

    void Start()
    {
        currentHealth = maxHealth;

        // ▼▼▼ Slider 초기 설정 ▼▼▼
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;         // 슬라이더 최소값을 0으로 설정
            healthSlider.maxValue = maxHealth;  // 슬라이더 최대값을 maxHealth(예: 100)로 설정
            healthSlider.value = currentHealth; // 현재 체력으로 슬라이더 값 초기화
        }
        else
        {
            Debug.LogWarning("PlayerHealth: HealthSlider is not assigned in the Inspector.");
        }
        // ▲▲▲ Slider 초기 설정 ▲▲▲

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("PlayerHealth: SpriteRenderer component not found. Damage flash will not work.");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("PlayerHealth: GameOverPanel is not assigned in the Inspector.");
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isInvincible) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // 체력은 0과 maxHealth 사이로 유지
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current HP: " + currentHealth);

        UpdateHealthUI(); // 체력 UI 업데이트 함수 호출

        if (anim != null)
        {
            anim.SetTrigger("hurt");
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            StartCoroutine(ResetColorAfterDelay(damageFlashDuration));
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeTemporarilyInvincible());
        }
    }

    void UpdateHealthUI()
    {
        // ▼▼▼ Slider 값 업데이트 ▼▼▼
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth; // Slider의 value를 현재 체력으로 직접 설정
        }
        // ▲▲▲ Slider 값 업데이트 ▲▲▲
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        if (anim != null)
        {
            anim.SetTrigger("die");
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Retry 버튼 찾기 (이름으로 찾는 것이 가장 안전)
            Button retryButton = gameOverPanel.transform.Find("RetryButton")?.GetComponent<Button>();
            if (retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(() =>
                {
                    GameManager.Instance.RetryGame();
                });
            }

            // NPCButton 찾기
            Button npcButton = gameOverPanel.transform.Find("NPCButton")?.GetComponent<Button>();
            if (npcButton != null)
            {
                npcButton.onClick.RemoveAllListeners();
                npcButton.onClick.AddListener(() =>
                {
                    GameManager.Instance.LoadNPCScene();
                });
            }
        }
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        float endTime = Time.time + invincibilityDuration;
        Color originalColor = Color.white;
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        while (Time.time < endTime)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return new WaitForSeconds(0.1f);
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }
        isInvincible = false;
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spriteRenderer != null && !isInvincible)
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D 발생! 충돌한 오브젝트: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag(enemyTag))
        {
            Debug.Log("Player collided with an enemy: " + collision.gameObject.name);
            TakeDamage(damageFromEnemy);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isInvincible = false;
        UpdateHealthUI(); // 체력 UI 업데이트 함수 호출
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }
}
