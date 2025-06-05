// EnemyHealth.cs
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public int attackDamageFromPlayer = 25; // 플레이어로부터 받을 기본 데미지 (나중에 공격 콜라이더에서 설정 가능)

    // (선택 사항) HP 바 UI를 위한 참조
    // public Slider healthBar; 

    void Start()
    {
        currentHealth = maxHealth;
        // if (healthBar != null) healthBar.value = CalculateHealthPercentage();
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // HP가 0 미만 또는 maxHealth 초과가 되지 않도록

        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current HP: " + currentHealth);
        // if (healthBar != null) healthBar.value = CalculateHealthPercentage();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // float CalculateHealthPercentage()
    // {
    //     return currentHealth / maxHealth;
    // }

    void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        // 여기에 적 사망 시 처리 (예: 애니메이션 재생, 아이템 드랍, 오브젝트 파괴 등)
        Destroy(gameObject); // 간단하게 오브젝트 파괴
    }

    // 이 스크립트가 붙은 적 오브젝트에는 Collider2D가 있어야 합니다.
    // 그리고 적을 식별하기 위해 Tag를 "Enemy"로 설정하는 것이 좋습니다.
}
