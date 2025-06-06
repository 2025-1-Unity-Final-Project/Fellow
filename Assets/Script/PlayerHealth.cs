// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI; // HP �� UI�� ����Ѵٸ� �ʿ�
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

    [Header("Game Over UI")] // ���� [���� �߰�] ���� ���� UI ���� ����
    public GameObject gameOverPanel; // Inspector���� "GameOverPanel" ������Ʈ�� �����մϴ�.

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

        // ���� [���� �߰�] ���� ���� �� ���� ���� �г� ��Ȱ��ȭ (������ ����) ����
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
            anim.SetTrigger("hurt"); // ��ũ��Ʈ�� �Ķ���� �̸��� Animator Controller�� �Ķ���� �̸� ��ġ Ȯ��
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
            anim.SetTrigger("die"); // ��ũ��Ʈ�� �Ķ���� �̸��� Animator Controller�� �Ķ���� �̸� ��ġ Ȯ��
        }

        // ���� [���� �߰�] ���� ���� �г� Ȱ��ȭ ����
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        // ���� [���� �߰�] ���� ���� �г� Ȱ��ȭ ����

        // (���� ����) ���� �ð� ����
        // Time.timeScale = 0f; // ������ ��� �ð� �帧�� ����ϴ�. UI �ִϸ��̼ǵ� ���� �� ������ ����.

        // (���� ����) �÷��̾� ��Ʈ�� ��Ȱ��ȭ ��
        // SimplePlayerController spc = GetComponent<SimplePlayerController>();
        // if (spc != null)
        // {
        //     spc.enabled = false;
        // }
        // if (rb != null)
        // {
        //     rb.velocity = Vector2.zero; // ������ ����
        // }
        // gameObject.SetActive(false); // �÷��̾� ������Ʈ ��ü�� ��Ȱ��ȭ�� ���� ����
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        float endTime = Time.time + invincibilityDuration;
        Color originalColor = Color.white; // ���� ���� ���� (������ �� ������)
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
            spriteRenderer.color = originalColor; // ������ �� ���� �������� ���� ����
        }
        isInvincible = false;
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spriteRenderer != null && !isInvincible) // ���� �������� ���۵Ǳ� �� �Ǵ� ���� ���°� �ƴ϶��
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D �߻�! �浹�� ������Ʈ: " + collision.gameObject.name);

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
        // (���� ����) ���� ���� �г� ��Ȱ��ȭ
        // if (gameOverPanel != null)
        // {
        //     gameOverPanel.SetActive(false);
        // }
        // (���� ����) ���� �ð� �ٽ� �帣��
        // Time.timeScale = 1f;
    }
}
