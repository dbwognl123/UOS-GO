using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Move Speed")]
    [SerializeField] private float minMoveSpeed = 1.2f;
    [SerializeField] private float maxMoveSpeed = 3f;

    [Tooltip("높을수록 최대체력이 낮을 때 속도가 크게 감소합니다.")]
    [SerializeField] private float healthExponent = 1.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Debug")]
    [SerializeField] private float debugMaxHealth;
    [SerializeField] private float debugHealth01;
    [SerializeField] private float debugCurrentMoveSpeed;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.down;

    private float currentMoveSpeed;

    private bool lastAxisHorizontal;
    private int lastHorizontal = 1;
    private int lastVertical = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        CalculateMoveSpeed();
    }

    private void CalculateMoveSpeed()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.CurrentPlayer == null)
        {
            currentMoveSpeed = maxMoveSpeed;

            debugMaxHealth = 100f;
            debugHealth01 = 1f;
            debugCurrentMoveSpeed = currentMoveSpeed;

            return;
        }

        float maxHealth =
            GameManager.Instance.CurrentPlayer.maxHealth;

        float minimumMultiplier =
            maxMoveSpeed > 0f
                ? Mathf.Clamp01(minMoveSpeed / maxMoveSpeed)
                : 0f;

        float speedMultiplier =
            GameManager.Instance.GetMaxHealthSpeedMultiplier(
                minimumMultiplier,
                healthExponent
            );

        currentMoveSpeed =
            maxMoveSpeed * speedMultiplier;

        debugMaxHealth = maxHealth;
        debugHealth01 = Mathf.Clamp01(maxHealth / 100f);
        debugCurrentMoveSpeed = currentMoveSpeed;

        Debug.Log(
            $"[SchoolMoveSpeed] " +
            $"maxHealth={maxHealth}, " +
            $"health01={debugHealth01:F2}, " +
            $"moveSpeed={currentMoveSpeed:F2}"
        );
    }

    private void Update()
    {
        if (IsInputBlocked())
        {
            StopMovement();
            return;
        }

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

        // 대각선 이동 금지
        if (rawX != 0 && rawY != 0)
        {
            if (lastAxisHorizontal)
                rawY = 0;
            else
                rawX = 0;
        }

        moveInput = new Vector2(rawX, rawY);

        bool isMoving =
            moveInput != Vector2.zero;

        if (isMoving)
            facingDirection = moveInput;

        UpdateAnimation(isMoving);
    }

    private bool IsInputBlocked()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.IsSchoolHealthDrainPaused)
        {
            return true;
        }

        if (ConvenienceStoreUI.Instance != null &&
            ConvenienceStoreUI.Instance.IsOpen)
        {
            return true;
        }

        return false;
    }

    private void StopMovement()
    {
        moveInput = Vector2.zero;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        UpdateAnimation(false);
    }

    private void UpdateAnimation(bool isMoving)
    {
        if (animator == null)
            return;

        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("MoveX", facingDirection.x);
        animator.SetFloat("MoveY", facingDirection.y);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity =
            moveInput * currentMoveSpeed;
    }
}