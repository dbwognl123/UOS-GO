using UnityEngine;
using UnityEngine.UI;

public class PartTimeTileSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public PartTimeTileType CurrentType { get; private set; }

    private void Reset()
    {
        iconImage = GetComponent<Image>();
    }

    private void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponent<Image>();
    }

    public void SetTile(PartTimeTileType type, Sprite sprite)
    {
        CurrentType = type;

        if (iconImage != null)
        {
            iconImage.sprite = sprite;
            iconImage.enabled = true;
        }
    }

    public void Clear()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}