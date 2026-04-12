using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DodgePlayerController : MonoBehaviour
{
    [Header("Move Speed")]
    [SerializeField] private float minMoveSpeed = 0.4f;
    [SerializeField] private float maxMoveSpeed = 6.0f;
    [SerializeField] private float healthExponent = 3.5f;

    [Header("Arena")]
    [SerializeField] private float playerRadius = 0.25f;

    [Header("Debug")]
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

    public void SetupArena(Transform center, float radius)
    {
        arenaCenter = center;
        arenaRadius = radius;
    }

    private void Start()
    {
        float health01 = 1f;

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            var player = GameManager.Instance.CurrentPlayer;
            health01 = Mathf.Clamp01(player.currentHealth / 100f);
        }

        float curved = Mathf.Pow(health01, healthExponent);
        currentMoveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, curved);

        debugHealth01 = health01;
        debugCurrentMoveSpeed = currentMoveSpeed;

        Debug.Log($"[MoveSpeed] currentHealth={health01 * 100f}, health01={health01}, moveSpeed={currentMoveSpeed}");
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y).normalized;
    }

    private void FixedUpdate()
    {
        Vector2 nextPos = rb.position + moveInput * currentMoveSpeed * Time.fixedDeltaTime;

        if (arenaCenter != null)
        {
            Vector2 center = arenaCenter.position;
            Vector2 offset = nextPos - center;
            float maxRadius = arenaRadius - playerRadius;

            if (offset.magnitude > maxRadius)
                nextPos = center + offset.normalized * maxRadius;
        }

        rb.MovePosition(nextPos);
    }
}