using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DodgeProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float exitPadding = 0.15f;

    private Rigidbody2D rb;
    private ClassDodgeGameController controller;

    private Transform arenaCenter;
    private float arenaRadius;
    private bool enteredArena = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(
        Vector2 direction,
        float speed,
        ClassDodgeGameController gameController,
        Transform center,
        float radius)
    {
        controller = gameController;
        arenaCenter = center;
        arenaRadius = radius;

        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (arenaCenter == null) return;

        float dist = Vector2.Distance(transform.position, arenaCenter.position);

        // 안쪽으로 한 번 들어왔는지 체크
        if (!enteredArena && dist < arenaRadius - 0.05f)
            enteredArena = true;

        // 안쪽으로 들어왔다가 다시 경기장 밖으로 나가면 제거
        if (enteredArena && dist > arenaRadius + exitPadding)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (controller != null)
                controller.OnPlayerHit();

            Destroy(gameObject);
        }
    }
}