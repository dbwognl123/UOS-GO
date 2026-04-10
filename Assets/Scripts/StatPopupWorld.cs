using TMPro;
using UnityEngine;

public class StatPopupWorld : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float floatSpeed = 1f;

    private float timer;

    public void Init(Sprite icon, int delta)
    {
        if (iconRenderer != null)
            iconRenderer.sprite = icon;

        if (valueText != null)
        {
            string sign = delta > 0 ? "+" : "";
            valueText.text = sign + delta.ToString();
        }
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}