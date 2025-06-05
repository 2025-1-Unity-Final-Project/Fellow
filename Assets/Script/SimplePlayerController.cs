using UnityEngine;

namespace ClearSky
{
    public class SimplePlayerController : MonoBehaviour
    {
        public float movePower = 10f;
        public float jumpPower = 15f;

        private Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        private int direction = 1;
        bool isJumping = false;
        private bool alive = true;

        private bool moveLeft = false;
        private bool moveRight = false;
        private bool jumpPressed = false;

        private Vector3 _initialLocalScale; // 초기 스케일 저장 변수

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            _initialLocalScale = transform.localScale; // 현재 로컬 스케일 저장
            // 초기 X 스케일이 음수일 경우를 대비해 절대값으로 저장 (선택적, 혹은 의도에 따라 조절)
            // 만약 항상 양수로 시작한다면 아래 줄은 필요 없을 수 있습니다.
            _initialLocalScale.x = Mathf.Abs(_initialLocalScale.x);
        }

        private void Update()
        {
            Restart();
            if (alive)
            {
                Hurt();
                Die();
                Attack();
                Jump(); // 점프 로직은 스케일을 변경하지 않음
                Run();  // 이동 로직에서 스케일 변경
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            anim.SetBool("isJump", false);
        }

        void Run()
        {
            Vector3 moveVelocity = Vector3.zero;
            anim.SetBool("isRun", false);

            float horizontalInput = Input.GetAxisRaw("Horizontal"); // 키보드 입력 미리 받아두기

            if (horizontalInput < 0 || moveLeft)
            {
                direction = -1;
                moveVelocity = Vector3.left;

                // 수정된 부분: 초기 스케일을 기준으로 X축 방향만 변경
                transform.localScale = new Vector3(_initialLocalScale.x * direction, _initialLocalScale.y, _initialLocalScale.z);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
            if (horizontalInput > 0 || moveRight) // else if 가 아닌 if를 사용하면 양쪽 버튼 동시 입력 시 오른쪽 우선 가능성 있음 (현재 로직 유지)
            {
                direction = 1;
                moveVelocity = Vector3.right;

                // 수정된 부분: 초기 스케일을 기준으로 X축 방향만 변경
                transform.localScale = new Vector3(_initialLocalScale.x * direction, _initialLocalScale.y, _initialLocalScale.z);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
            transform.position += moveVelocity * movePower * Time.deltaTime;
        }

        // Jump, Attack, Hurt, Die, Restart, UI Button handlers (OnLeftDown 등) 함수들은 기존과 동일
        // ... (나머지 코드 생략) ...
        void Jump()
        {
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0 || jumpPressed)
                && !anim.GetBool("isJump"))
            {
                isJumping = true;
                anim.SetBool("isJump", true);
                jumpPressed = false;
            }
            if (!isJumping)
            {
                return;
            }

            rb.linearVelocity = Vector2.zero;
            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rb.AddForce(jumpVelocity, ForceMode2D.Impulse);
            isJumping = false;
        }

        void Attack()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                anim.SetTrigger("attack");
            }
        }

        void Hurt()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                anim.SetTrigger("hurt");
                if (direction == 1)
                    rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
                else
                    rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
            }
        }

        void Die()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                anim.SetTrigger("die");
                alive = false;
            }
        }

        void Restart()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                anim.SetTrigger("idle");
                alive = true;
            }
        }

        public void OnLeftDown() { moveLeft = true; }
        public void OnLeftUp() { moveLeft = false; }
        public void OnRightDown() { moveRight = true; }
        public void OnRightUp() { moveRight = false; }
        public void OnJumpDown() { jumpPressed = true; }
    }
}
