// EnemyPatrol.cs
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;                // 이동 속도
    public float startWaitTime = 2f;        // 목표 지점 도달 후 대기 시간
    private float waitTime;                 // 현재 대기 시간 카운터

    public Transform moveSpot;              // 이동 목표 지점을 나타내는 빈 오브젝트 (선택 사항)
    public Vector2 patrolAreaMin;           // 순찰 범위의 최소 X, Y 좌표
    public Vector2 patrolAreaMax;           // 순찰 범위의 최대 X, Y 좌표

    private Vector2 randomTargetPosition;   // 다음 이동할 랜덤 목표 위치
    private Rigidbody2D rb;
    private Animator anim;                  // Animator 변수
    private bool facingRight = true;        // 현재 오른쪽을 보고 있는지 여부 (오른쪽: Y=0, 왼쪽: Y=180)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();      // Animator 컴포넌트 가져오기
        waitTime = startWaitTime;
        SetNewRandomTargetPosition();

        if (moveSpot != null)
        {
            moveSpot.position = randomTargetPosition;
        }

        // 초기 방향 설정 (오른쪽을 보도록 기본 설정)
        if (facingRight)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else // 만약 기본이 왼쪽을 보는 것이라면
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        // 목표 지점으로 이동
        transform.position = Vector2.MoveTowards(currentPosition, randomTargetPosition, speed * Time.deltaTime);

        // 이동 방향에 따른 좌우 반전 로직 (회전 사용)
        float moveDirectionX = randomTargetPosition.x - currentPosition.x;

        if (moveDirectionX > 0.01f && !facingRight) // 오른쪽으로 이동해야 하는데 현재 왼쪽을 보고 있다면
        {
            Flip();
        }
        else if (moveDirectionX < -0.01f && facingRight) // 왼쪽으로 이동해야 하는데 현재 오른쪽을 보고 있다면
        {
            Flip();
        }

        // 애니메이션 제어 로직
        bool isMoving = Vector2.Distance(transform.position, randomTargetPosition) > 0.05f;
        if (anim != null)
        {
            anim.SetBool("Walk", isMoving); // "Walk" 파라미터 설정 (isMoving이 true면 Walk, false면 Idle)
        }

        // 목표 지점에 거의 도달했는지 확인 (isMoving이 false일 때 대기 로직 실행)
        if (!isMoving)
        {
            if (waitTime <= 0)
            {
                SetNewRandomTargetPosition();
                if (moveSpot != null)
                {
                    moveSpot.position = randomTargetPosition;
                }
                waitTime = startWaitTime; // 대기 시간 초기화
            }
            else
            {
                waitTime -= Time.deltaTime; // 대기 시간 감소
            }
        }
    }

    void SetNewRandomTargetPosition()
    {
        float randomX = Random.Range(patrolAreaMin.x, patrolAreaMax.x);
        float randomY = Random.Range(patrolAreaMin.y, patrolAreaMax.y);
        randomTargetPosition = new Vector2(randomX, randomY);
    }

    // 좌우 반전 함수 (회전 사용)
    void Flip()
    {
        facingRight = !facingRight; // 바라보는 방향 토글

        if (facingRight)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // 오른쪽 바라보기
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // 왼쪽 바라보기
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((patrolAreaMin.x + patrolAreaMax.x) / 2, (patrolAreaMin.y + patrolAreaMax.y) / 2, 0);
        Vector3 size = new Vector3(patrolAreaMax.x - patrolAreaMin.x, patrolAreaMax.y - patrolAreaMin.y, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
