using TMPro;
using UnityEngine;

public class WorldTextPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float lifeTime = 1.2f;
    [SerializeField] private float floatSpeed = 0.6f;

    private float timer;
    private Color baseColor;

    public void Init(string message, Color color)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
            baseColor = color;
        }
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;

        if (messageText != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / lifeTime);
            Color c = baseColor;
            c.a = alpha;
            messageText.color = c;
        }

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}