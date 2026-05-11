using UnityEngine;
using UnityEngine.UI;

public class StudyGridCell : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public Vector2Int Coord { get; private set; }

    private void Reset()
    {
        fillImage = GetComponent<Image>();
    }

    private void Awake()
    {
        if (fillImage == null)
            fillImage = GetComponent<Image>();
    }

    public void Setup(Vector2Int coord)
    {
        Coord = coord;
    }

    public void SetColor(Color color)
    {
        if (fillImage != null)
            fillImage.color = color;
    }
}