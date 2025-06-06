using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 25; // 이 공격으로 입힐 데미지
    public string enemyTag = "Enemy"; // 데미지를 입힐 대상의 태그

    // 이 콜라이더가 다른 트리거 콜라이더와 충돌했을 때 호출됩니다.
    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Debug.Log("AttackHitbox와 " + otherCollider.gameObject.name + " 충돌 발생!");
        // 충돌한 오브젝트가 "Enemy" 태그를 가지고 있는지 확인
        if (otherCollider.gameObject.CompareTag(enemyTag))
        {
            // 적 오브젝트에서 EnemyHealth 스크립트를 가져옴
            EnemyHealth enemyHealth = otherCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // 적에게 데미지를 입힘
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log("Hit " + otherCollider.name + " for " + attackDamage + " damage.");

            }
        }
    }

}
