// EnemyHealth.cs
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public int attackDamageFromPlayer = 25; // �÷��̾�κ��� ���� �⺻ ������ (���߿� ���� �ݶ��̴����� ���� ����)

    // (���� ����) HP �� UI�� ���� ����
    // public Slider healthBar; 

    void Start()
    {
        currentHealth = maxHealth;
        // if (healthBar != null) healthBar.value = CalculateHealthPercentage();
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // HP�� 0 �̸� �Ǵ� maxHealth �ʰ��� ���� �ʵ���

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
        // ���⿡ �� ��� �� ó�� (��: �ִϸ��̼� ���, ������ ���, ������Ʈ �ı� ��)
        Destroy(gameObject); // �����ϰ� ������Ʈ �ı�
    }

    // �� ��ũ��Ʈ�� ���� �� ������Ʈ���� Collider2D�� �־�� �մϴ�.
    // �׸��� ���� �ĺ��ϱ� ���� Tag�� "Enemy"�� �����ϴ� ���� �����ϴ�.
}
