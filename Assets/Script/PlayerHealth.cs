// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI; // HP �� UI�� ����Ѵٸ� �ʿ�
using System.Collections; // �ڷ�ƾ ����� ���� �߰� (BecomeTemporarilyInvincible)

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill; // (���� ����) HP �� UI�� Fill Image ����

    [Header("Invincibility Settings")]
    public bool isInvincible = false;
    public float invincibilityDuration = 1f; // �ǰ� �� ���� ���� �ð�

    [Header("Damage from Enemy Settings")] // ���� [���� �߰�] ����
    public float damageFromEnemy = 10f; // �����κ��� ���� �⺻ ������
    public string enemyTag = "Enemy"; // �� ĳ������ �±�

    private Animator anim; // (���� ����) �ǰ�/��� �ִϸ��̼��� ���� Animator ����
    private Rigidbody2D rb; // (���� ����) �ǰ� �� �˹� ���� ���� Rigidbody2D ����

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        anim = GetComponent<Animator>(); // Animator ������Ʈ ��������
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ������Ʈ ��������
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
            anim.SetTrigger("Hurt"); // "Hurt" �ִϸ��̼� ��� (Animator Controller�� �ش� Ʈ���� �ʿ�)
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
            anim.SetTrigger("Die"); // "Die" �ִϸ��̼� ��� (Animator Controller�� �ش� Ʈ���� �ʿ�)
        }
        // ���⿡ �÷��̾� ��� �� ó�� (��: ���� ���� ȭ��, ������, ��Ʈ�� ��Ȱ��ȭ ��)
        // ��: GetComponent<SimplePlayerController>().enabled = false; // �÷��̾� ��Ʈ�� ��ũ��Ʈ ��Ȱ��ȭ
        // ��: if(rb != null) rb.velocity = Vector2.zero; // ������ ����
        // ��: gameObject.SetActive(false); // �����ϰ� ��Ȱ��ȭ (�ٸ� ��ũ��Ʈ���� ��Ȱ ���� �ʿ�)
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        // ���⿡ �ǰ� �� �ð��� ȿ�� (��: ��������Ʈ ������) �߰� ����
        // ����: SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // if (spriteRenderer != null)
        // {
        // for (int i = 0; i < 5; i++) // 5�� ������
        // {
        // spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); // ������
        // yield return new WaitForSeconds(invincibilityDuration / 10);
        // spriteRenderer.color = Color.white; // ���� ��
        // yield return new WaitForSeconds(invincibilityDuration / 10);
        // }
        // }
        // else // �� �ڷ�ƾ ���� ��� ������ �ð��� ��ٸ�
        // {
        yield return new WaitForSeconds(invincibilityDuration);
        // }
        isInvincible = false;
    }

    // ���� [���� �߰�] ������ ������ �浹 �� ������ ó�� ����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D �߻�! �浹�� ������Ʈ: " + collision.gameObject.name); // <--- �� �α׸� ���� ���� Ȯ��!

        // �浹�� ������Ʈ�� "Enemy" �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag(enemyTag))
        {
            Debug.Log("Player collided with an enemy: " + collision.gameObject.name);
            TakeDamage(damageFromEnemy); // �ڱ� �ڽ��� TakeDamage �Լ� ȣ��
        }

        // ���� �ٴ�(Ground) �±׿� �浹���� �� ���� ���¸� �����ϰ� �ʹٸ�,
        // �� ������ SimplePlayerController.cs�� �ΰų�, ���⼭ Animator �Ķ���͸� ���� ������ �� �ֽ��ϴ�.
        // ��: if (collision.gameObject.CompareTag("Ground") && anim != null) { anim.SetBool("isJump", false); }
    }
    // ���� [���� �߰�] ������ ������ �浹 �� ������ ó�� ����

    // (���� ����) HP �ʱ�ȭ �Լ� (Restart ��� ȣ�� ����)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isInvincible = false;
        UpdateHealthUI();
        // �ʿ��ϴٸ� SimplePlayerController�� alive ���µ� ���⼭ true�� ����
        // SimplePlayerController spc = GetComponent<SimplePlayerController>();
        // if (spc != null) spc.SetAlive(true); // SimplePlayerController�� SetAlive(bool status) �Լ� �ʿ�
    }
}