using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.down;

    private bool lastAxisHorizontal = false;

    // 같은 축에서 마지막으로 누른 방향 기억
    private int lastHorizontal = 1; // 오른쪽 시작
    private int lastVertical = -1;  // 아래 시작

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (ConvenienceStoreUI.Instance != null && ConvenienceStoreUI.Instance.IsOpen)
            return;
        bool a = Input.GetKey(KeyCode.A);
        bool d = Input.GetKey(KeyCode.D);
        bool w = Input.GetKey(KeyCode.W);
        bool s = Input.GetKey(KeyCode.S);

        if (Input.GetKeyDown(KeyCode.A))
        {
            lastHorizontal = -1;
            lastAxisHorizontal = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            lastHorizontal = 1;
            lastAxisHorizontal = true;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            lastVertical = 1;
            lastAxisHorizontal = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            lastVertical = -1;
            lastAxisHorizontal = false;
        }

        int rawX = 0;
        int rawY = 0;

        // 같은 축 반대키 동시 입력 시 마지막으로 누른 방향 우선
        if (a && d)
            rawX = lastHorizontal;
        else if (a)
            rawX = -1;
        else if (d)
            rawX = 1;

        if (w && s)
            rawY = lastVertical;
        else if (w)
            rawY = 1;
        else if (s)
            rawY = -1;

        // 대각선 금지: 최근에 누른 축만 살림
        if (rawX != 0 && rawY != 0)
        {
            if (lastAxisHorizontal)
                rawY = 0;
            else
                rawX = 0;
        }

        moveInput = new Vector2(rawX, rawY);

        bool isMoving = moveInput != Vector2.zero;

        if (isMoving)
            facingDirection = moveInput;

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetFloat("MoveX", facingDirection.x);
            animator.SetFloat("MoveY", facingDirection.y);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}