// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI; // Slider ����� ���� �ʿ�
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; // �ִ� ü���� 100���� �����ϰų�, Inspector���� ����
    public float currentHealth;
    // public Image healthBarFill; // Image ��� Slider ��� ����
    public Slider healthSlider;    // Inspector���� Slider ������Ʈ�� �����մϴ�.

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

        // ���� Slider �ʱ� ���� ����
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;         // �����̴� �ּҰ��� 0���� ����
            healthSlider.maxValue = maxHealth;  // �����̴� �ִ밪�� maxHealth(��: 100)�� ����
            healthSlider.value = currentHealth; // ���� ü������ �����̴� �� �ʱ�ȭ
        }
        else
        {
            Debug.LogWarning("PlayerHealth: HealthSlider is not assigned in the Inspector.");
        }
        // ���� Slider �ʱ� ���� ����

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
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // ü���� 0�� maxHealth ���̷� ����
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current HP: " + currentHealth);

        UpdateHealthUI(); // ü�� UI ������Ʈ �Լ� ȣ��

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
        // ���� Slider �� ������Ʈ ����
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth; // Slider�� value�� ���� ü������ ���� ����
        }
        // ���� Slider �� ������Ʈ ����
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

            // Retry ��ư ã�� (�̸����� ã�� ���� ���� ����)
            Button retryButton = gameOverPanel.transform.Find("RetryButton")?.GetComponent<Button>();
            if (retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(() =>
                {
                    GameManager.Instance.RetryGame();
                });
            }

            // NPCButton ã��
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
        UpdateHealthUI(); // ü�� UI ������Ʈ �Լ� ȣ��
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }
}
