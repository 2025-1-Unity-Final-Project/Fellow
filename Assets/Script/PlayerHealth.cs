// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI; // HP 바 UI를 사용한다면 필요
using System.Collections; // 코루틴 사용을 위해 추가 (BecomeTemporarilyInvincible)

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill; // (선택 사항) HP 바 UI의 Fill Image 연결

    [Header("Invincibility Settings")]
    public bool isInvincible = false;
    public float invincibilityDuration = 1f; // 피격 후 무적 지속 시간

    [Header("Damage from Enemy Settings")] // ▼▼▼ [새로 추가] ▼▼▼
    public float damageFromEnemy = 10f; // 적으로부터 받을 기본 데미지
    public string enemyTag = "Enemy"; // 적 캐릭터의 태그

    private Animator anim; // (선택 사항) 피격/사망 애니메이션을 위해 Animator 참조
    private Rigidbody2D rb; // (선택 사항) 피격 시 넉백 등을 위해 Rigidbody2D 참조

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        anim = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
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
            anim.SetTrigger("Hurt"); // "Hurt" 애니메이션 재생 (Animator Controller에 해당 트리거 필요)
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
            anim.SetTrigger("Die"); // "Die" 애니메이션 재생 (Animator Controller에 해당 트리거 필요)
        }
        // 여기에 플레이어 사망 시 처리 (예: 게임 오버 화면, 리스폰, 컨트롤 비활성화 등)
        // 예: GetComponent<SimplePlayerController>().enabled = false; // 플레이어 컨트롤 스크립트 비활성화
        // 예: if(rb != null) rb.velocity = Vector2.zero; // 움직임 멈춤
        // 예: gameObject.SetActive(false); // 간단하게 비활성화 (다른 스크립트에서 부활 로직 필요)
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        // 여기에 피격 시 시각적 효과 (예: 스프라이트 깜빡임) 추가 가능
        // 예시: SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // if (spriteRenderer != null)
        // {
        // for (int i = 0; i < 5; i++) // 5번 깜빡임
        // {
        // spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); // 반투명
        // yield return new WaitForSeconds(invincibilityDuration / 10);
        // spriteRenderer.color = Color.white; // 원래 색
        // yield return new WaitForSeconds(invincibilityDuration / 10);
        // }
        // }
        // else // 위 코루틴 로직 대신 간단히 시간만 기다림
        // {
        yield return new WaitForSeconds(invincibilityDuration);
        // }
        isInvincible = false;
    }

    // ▼▼▼ [새로 추가] 적과의 물리적 충돌 시 데미지 처리 ▼▼▼
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D 발생! 충돌한 오브젝트: " + collision.gameObject.name); // <--- 이 로그를 가장 먼저 확인!

        // 충돌한 오브젝트가 "Enemy" 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag(enemyTag))
        {
            Debug.Log("Player collided with an enemy: " + collision.gameObject.name);
            TakeDamage(damageFromEnemy); // 자기 자신의 TakeDamage 함수 호출
        }

        // 만약 바닥(Ground) 태그와 충돌했을 때 점프 상태를 리셋하고 싶다면,
        // 이 로직은 SimplePlayerController.cs에 두거나, 여기서 Animator 파라미터를 직접 제어할 수 있습니다.
        // 예: if (collision.gameObject.CompareTag("Ground") && anim != null) { anim.SetBool("isJump", false); }
    }
    // ▲▲▲ [새로 추가] 적과의 물리적 충돌 시 데미지 처리 ▲▲▲

    // (선택 사항) HP 초기화 함수 (Restart 등에서 호출 가능)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isInvincible = false;
        UpdateHealthUI();
        // 필요하다면 SimplePlayerController의 alive 상태도 여기서 true로 설정
        // SimplePlayerController spc = GetComponent<SimplePlayerController>();
        // if (spc != null) spc.SetAlive(true); // SimplePlayerController에 SetAlive(bool status) 함수 필요
    }
}