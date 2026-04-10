using UnityEngine;

public class PlayerStatPopupSpawner : MonoBehaviour
{
    [SerializeField] private StatPopupWorld popupPrefab;
    [SerializeField] private Transform popupSpawnPoint;

    [Header("Icons")]
    [SerializeField] private Sprite healthUpIcon;
    [SerializeField] private Sprite healthDownIcon;
    [SerializeField] private Sprite intelligenceUpIcon;
    [SerializeField] private Sprite intelligenceDownIcon;
    [SerializeField] private Sprite moneyUpIcon;
    [SerializeField] private Sprite moneyDownIcon;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerStatChanged += HandleStatChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerStatChanged -= HandleStatChanged;
    }

    private void HandleStatChanged(PlayerStatType statType, int delta)
    {
        if (popupPrefab == null || popupSpawnPoint == null) return;
        if (delta == 0) return;

        Sprite icon = GetIcon(statType, delta);

        StatPopupWorld popup = Instantiate(
            popupPrefab,
            popupSpawnPoint.position,
            Quaternion.identity
        );

        popup.Init(icon, delta);
    }

    private Sprite GetIcon(PlayerStatType statType, int delta)
    {
        switch (statType)
        {
            case PlayerStatType.Health:
                return delta >= 0 ? healthUpIcon : healthDownIcon;

            case PlayerStatType.Intelligence:
                return delta >= 0 ? intelligenceUpIcon : intelligenceDownIcon;

            case PlayerStatType.Money:
                return delta >= 0 ? moneyUpIcon : moneyDownIcon;
        }

        return null;
    }
}