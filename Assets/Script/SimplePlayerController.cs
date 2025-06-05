using UnityEngine;

namespace ClearSky
{
    public class SimplePlayerController : MonoBehaviour
    {
        public float movePower = 10f;
        public float jumpPower = 15f;

        private Rigidbody2D rb;
        private Animator anim;
        // Vector3 movement; // 사용되지 않음
        private int direction = 1;
        bool isJumping = false;
        private bool alive = true;

        // UI 버튼 관련 상태 변수
        private bool moveLeft = false;
        private bool moveRight = false;
        private bool jumpPressed = false;
        private bool interactPressed = false; // UI 상호작용 버튼 상태 변수

        private Vector3 _initialLocalScale;
        private NPCDialogue currentInteractableNPC; // NPCDialogue.cs 필요

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            _initialLocalScale = transform.localScale;
            _initialLocalScale.x = Mathf.Abs(_initialLocalScale.x);
        }

        private void Update()
        {
            Restart();
            if (alive)
            {
                HandleInteractionInput();
                Hurt();
                Die();
                Attack();
                Jump();
                Run();
            }
        }

        void HandleInteractionInput()
        {
            if (interactPressed && currentInteractableNPC != null)
            {
                currentInteractableNPC.StartDialogue(); // NPCDialogue 스크립트의 함수 호출
                interactPressed = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            anim.SetBool("isJump", false);

            NPCDialogue npc = other.GetComponent<NPCDialogue>();
            if (npc != null)
            {
                currentInteractableNPC = npc;
                // NPC 범위 진입 시 UI 알림 로직 (필요시)
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            NPCDialogue npc = other.GetComponent<NPCDialogue>();
            if (npc != null && npc == currentInteractableNPC)
            {
                currentInteractableNPC = null;
                // NPC 범위 이탈 시 UI 알림 숨김 로직 (필요시)
            }
        }

        void Run()
        {
            Vector3 moveVelocity = Vector3.zero;
            anim.SetBool("isRun", false);

            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (horizontalInput < 0 || moveLeft)
            {
                direction = -1;
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(_initialLocalScale.x * direction, _initialLocalScale.y, _initialLocalScale.z);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
            if (horizontalInput > 0 || moveRight)
            {
                direction = 1;
                moveVelocity = Vector3.right;
                transform.localScale = new Vector3(_initialLocalScale.x * direction, _initialLocalScale.y, _initialLocalScale.z);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
            transform.position += moveVelocity * movePower * Time.deltaTime;
        }

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

        // --- UI Button Handlers ---
        public void OnLeftDown() { moveLeft = true; }
        public void OnLeftUp() { moveLeft = false; }
        public void OnRightDown() { moveRight = true; }
        public void OnRightUp() { moveRight = false; }
        public void OnJumpDown() { jumpPressed = true; }

        public void OnInteractDown()
        {
            interactPressed = true;
        }
    }
}
