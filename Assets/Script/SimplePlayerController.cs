using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 추가

namespace ClearSky
{
    public class SimplePlayerController : MonoBehaviour
    {
        public float movePower = 10f;
        public float jumpPower = 15f;

        public GameObject attackHitbox; // Inspector에서 AttackHitbox 오브젝트 연결
        public float attackDuration = 0.2f; // 공격 지속 시간 (초)

        private Rigidbody2D rb;
        private Animator anim;
        private int direction = 1;
        bool isJumping = false;
        private bool alive = true;

        // UI 버튼 관련 상태 변수
        private bool moveLeft = false;
        private bool moveRight = false;
        private bool jumpPressed = false;
        private bool interactPressed = false;
        private bool attackPressed = false;

        private Vector3 _initialLocalScale;
        private NPCDialogue currentInteractableNPC;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            _initialLocalScale = transform.localScale;
            _initialLocalScale.x = Mathf.Abs(_initialLocalScale.x);

            if (attackHitbox != null)
            {
                attackHitbox.SetActive(false);
            }
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
                currentInteractableNPC.StartDialogue();
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
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            NPCDialogue npc = other.GetComponent<NPCDialogue>();
            if (npc != null && npc == currentInteractableNPC)
            {
                currentInteractableNPC = null;
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
            if (Input.GetKeyDown(KeyCode.Alpha1) || attackPressed)
            {
                anim.SetTrigger("attack");
                if (attackPressed)
                {
                    attackPressed = false;
                }
                StartCoroutine(ActivateHitboxRoutine());
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
        public void OnInteractDown() { interactPressed = true; }
        public void OnAttackDown() { attackPressed = true; }

        IEnumerator ActivateHitboxRoutine()
        {
            if (attackHitbox != null)
            {
                attackHitbox.SetActive(true);
                yield return new WaitForSeconds(attackDuration);
                attackHitbox.SetActive(false);
            }
        }
    }
}
