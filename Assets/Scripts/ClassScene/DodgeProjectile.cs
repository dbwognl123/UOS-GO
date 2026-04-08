using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DodgeProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody2D rb;
    private ClassDodgeGameController controller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float speed, ClassDodgeGameController gameController)
    {
        controller = gameController;
        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifeTime);
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