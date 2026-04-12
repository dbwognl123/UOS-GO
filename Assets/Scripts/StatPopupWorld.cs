using UnityEngine;

public class StatPopupWorld : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private float lifeTime = 0.7f;
    [SerializeField] private float floatSpeed = 0.7f;

    private float timer;

    public void Init(Sprite icon)
    {
        if (iconRenderer != null)
            iconRenderer.sprite = icon;
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}