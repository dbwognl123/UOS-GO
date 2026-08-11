using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DodgePlayerController : MonoBehaviour
{
    [Header("Move Speed")]
    [SerializeField] private float minMoveSpeed = 0.4f;
    [SerializeField] private float maxMoveSpeed = 6.0f;

    // 값이 높을수록 최대체력이 감소할 때 속도가 더 가파르게 감소
    [SerializeField] private float healthExponent = 2.0f;

    [Header("Arena")]
    [SerializeField] private float playerRadius = 0.25f;

    [Header("Debug")]
    [SerializeField] private float debugMaxHealth;
    [SerializeField] private float debugHealth01;
    [SerializeField] private float debugCurrentMoveSpeed;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float currentMoveSpeed;

    private Transform arenaCenter;
    private float arenaRadius;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        CalculateMoveSpeed();
    }

    public void SetupArena(Transform center, float radius)
    {
        arenaCenter = center;
        arenaRadius = radius;
    }

    private void CalculateMoveSpeed()
    {
        // GameManager나 플레이어 데이터가 없으면 최대 속도 사용
        if (GameManager.Instance == null ||
            GameManager.Instance.CurrentPlayer == null)
        {
            currentMoveSpeed = maxMoveSpeed;

            debugMaxHealth = 100f;
            debugHealth01 = 1f;
            debugCurrentMoveSpeed = currentMoveSpeed;

            Debug.LogWarning(
                "[DodgeMoveSpeed] 플레이어 데이터가 없어 최대 속도를 사용합니다."
            );

            return;
        }

        float maxHealth =
            GameManager.Instance.CurrentPlayer.maxHealth;

        float minimumMultiplier =
            maxMoveSpeed > 0f
                ? Mathf.Clamp01(minMoveSpeed / maxMoveSpeed)
                : 0f;

        float multiplier =
            GameManager.Instance.GetMaxHealthSpeedMultiplier(
                minimumMultiplier,
                healthExponent
            );

        currentMoveSpeed =
            maxMoveSpeed * multiplier;

        debugMaxHealth = maxHealth;
        debugHealth01 = Mathf.Clamp01(maxHealth / 100f);
        debugCurrentMoveSpeed = currentMoveSpeed;

        Debug.Log(
            $"[DodgeMoveSpeed] " +
            $"maxHealth={maxHealth}, " +
            $"health01={debugHealth01:F2}, " +
            $"multiplier={multiplier:F2}, " +
            $"moveSpeed={currentMoveSpeed:F2}"
        );
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(x, y).normalized;
    }

    private void FixedUpdate()
    {
        Vector2 nextPos =
            rb.position +
            moveInput *
            currentMoveSpeed *
            Time.fixedDeltaTime;

        if (arenaCenter != null)
        {
            Vector2 center = arenaCenter.position;
            Vector2 offset = nextPos - center;

            float maxRadius = Mathf.Max(
                0f,
                arenaRadius - playerRadius
            );

            if (offset.sqrMagnitude > maxRadius * maxRadius)
            {
                nextPos =
                    center +
                    offset.normalized * maxRadius;
            }
        }

        rb.MovePosition(nextPos);
    }
}