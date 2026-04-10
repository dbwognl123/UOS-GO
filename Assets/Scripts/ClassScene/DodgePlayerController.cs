using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DodgePlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeedMin = 2.5f;
    [SerializeField] private float moveSpeedMax = 5.5f;
    [SerializeField] private float playerRadius = 0.25f;

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
            health01 = (float)player.currentHealth / player.maxHealth;
        }

        health01 = Mathf.Clamp01(health01);
        currentMoveSpeed = Mathf.Lerp(moveSpeedMin, moveSpeedMax, health01);
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