// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI; // HP 바 UI를 사용한다면 필요
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill;

    [Header("Invincibility Settings")]
    public bool isInvincible = false;
    public float invincibilityDuration = 1f;

    [Header("Damage from Enemy Settings")]
    public float damageFromEnemy = 10f;
    public string enemyTag = "Enemy";

    [Header("Game Over UI")] // ▼▼▼ [새로 추가] 게임 오버 UI 참조 ▼▼▼
    public GameObject gameOverPanel; // Inspector에서 "GameOverPanel" 오브젝트를 연결합니다.

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public float damageFlashDuration = 0.2f;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("PlayerHealth: SpriteRenderer component not found on " + gameObject.name + ". Damage flash will not work.");
        }

        // ▼▼▼ [새로 추가] 게임 시작 시 게임 오버 패널 비활성화 (만약을 위해) ▼▼▼
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
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current HP: " + currentHealth);
        UpdateHealthUI();

        if (anim != null)
        {
            anim.SetTrigger("hurt"); // 스크립트의 파라미터 이름과 Animator Controller의 파라미터 이름 일치 확인
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
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        if (anim != null)
        {
            anim.SetTrigger("die"); // 스크립트의 파라미터 이름과 Animator Controller의 파라미터 이름 일치 확인
        }

        // ▼▼▼ [새로 추가] 게임 오버 패널 활성화 ▼▼▼
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        // ▲▲▲ [새로 추가] 게임 오버 패널 활성화 ▲▲▲

        // (선택 사항) 게임 시간 정지
        // Time.timeScale = 0f; // 게임의 모든 시간 흐름을 멈춥니다. UI 애니메이션도 멈출 수 있으니 주의.

        // (선택 사항) 플레이어 컨트롤 비활성화 등
        // SimplePlayerController spc = GetComponent<SimplePlayerController>();
        // if (spc != null)
        // {
        //     spc.enabled = false;
        // }
        // if (rb != null)
        // {
        //     rb.velocity = Vector2.zero; // 움직임 멈춤
        // }
        // gameObject.SetActive(false); // 플레이어 오브젝트 자체를 비활성화할 수도 있음
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        float endTime = Time.time + invincibilityDuration;
        Color originalColor = Color.white; // 원래 색상 저장 (깜빡임 후 복원용)
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
            spriteRenderer.color = originalColor; // 깜빡임 후 원래 색상으로 최종 복원
        }
        isInvincible = false;
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spriteRenderer != null && !isInvincible) // 무적 깜빡임이 시작되기 전 또는 무적 상태가 아니라면
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
        UpdateHealthUI();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        // (선택 사항) 게임 오버 패널 비활성화
        // if (gameOverPanel != null)
        // {
        //     gameOverPanel.SetActive(false);
        // }
        // (선택 사항) 게임 시간 다시 흐르게
        // Time.timeScale = 1f;
    }
}
