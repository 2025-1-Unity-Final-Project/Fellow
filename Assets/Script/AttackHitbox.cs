using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 25; // �� �������� ���� ������
    public string enemyTag = "Enemy"; // �������� ���� ����� �±�

    // �� �ݶ��̴��� �ٸ� Ʈ���� �ݶ��̴��� �浹���� �� ȣ��˴ϴ�.
    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Debug.Log("AttackHitbox�� " + otherCollider.gameObject.name + " �浹 �߻�!");
        // �浹�� ������Ʈ�� "Enemy" �±׸� ������ �ִ��� Ȯ��
        if (otherCollider.gameObject.CompareTag(enemyTag))
        {
            // �� ������Ʈ���� EnemyHealth ��ũ��Ʈ�� ������
            EnemyHealth enemyHealth = otherCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // ������ �������� ����
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log("Hit " + otherCollider.name + " for " + attackDamage + " damage.");

            }
        }
    }

}
