// EnemyHealth.cs
using UnityEngine;
using UnityEngine.UI; // Slider ����� ���� �ʿ�

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public int attackDamageFromPlayer = 25; // �÷��̾�κ��� ���� �⺻ ������

    [Header("ü�� �� UI")] // ���� Inspector���� ������ ������ ����
    public Slider healthBar; // ���� ��ġ�� ü�� �� �����̴� (Inspector���� ����)
    public Vector3 healthBarOffset = new Vector3(0, 2, 0); // ü�� �� ��ġ ������

 

    void Start()
    {
        currentHealth = maxHealth;

        // ���� ���� �ִ� HealthBar�� ���� ����ϹǷ� SpawnHealthBar() �Լ� ��� ���⼭ �ʱ�ȭ ����
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;

            // ĵ������ World Space���� Ȯ���ϰ�, �ƴ϶�� ��� �޽��� ���
            Canvas canvas = healthBar.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogWarning("EnemyHealth: HealthBar�� World Space Canvas�� ���� �ʽ��ϴ�. ������ ������ �߻��� �� �ֽ��ϴ�.");
            }

           
        }
        else
        {
            Debug.LogError("EnemyHealth: HealthBar �����̴��� Inspector�� �Ҵ���� �ʾҽ��ϴ�!");
        }
        
    }

    void Update()
    {
        // ���� ��ġ�� UI�� ����ϹǷ�, �� ������ ��ġ�� ������Ʈ
        if (healthBar != null)
        {
            // ĳ���� �Ӹ� ���� ü�� �� ��ġ ���� (���� ��ǥ)
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

        // ���� GameManager.Instance.EnemyDestroyed() ȣ�� ����
        // GameManager�� �����Ѵٸ�, EnemyDestroyed �Լ� ȣ��
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyDestroyed();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
        // ���� GameManager.Instance.EnemyDestroyed() ȣ�� ����

        Destroy(gameObject);
    }
}
