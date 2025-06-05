// PlayerAttack.cs (또는 AttackHitboxController.cs)
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

                // (선택 사항) 한 번의 공격 애니메이션에 여러 번 데미지가 들어가지 않도록
                // 공격 콜라이더를 비활성화하거나, 이미 맞은 적을 기록하는 로직 추가 가능.
                // 간단하게는 이 콜라이더를 바로 비활성화 할 수도 있습니다.
                // gameObject.SetActive(false); 
            }
        }
    }

    // 참고: 이 스크립트가 붙은 AttackHitbox 오브젝트는 
    // IsTrigger가 체크된 Collider2D를 가지고 있어야 하며,
    // 공격 애니메이션 중에만 활성화되어야 합니다.
}
