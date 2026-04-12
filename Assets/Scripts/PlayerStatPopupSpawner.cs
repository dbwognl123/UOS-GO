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

    [Header("Popup Spread")]
    [SerializeField] private float horizontalSpacing = 0.45f;
    [SerializeField] private float verticalSpacing = 0.2f;
    [SerializeField] private float comboResetTime = 0.15f;

    private int burstIndex = 0;
    private float lastSpawnTime = -999f;

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
        if (icon == null) return;

        if (Time.time - lastSpawnTime > comboResetTime)
            burstIndex = 0;

        Vector3 offset = GetSpawnOffset(burstIndex);
        burstIndex++;
        lastSpawnTime = Time.time;

        StatPopupWorld popup = Instantiate(
            popupPrefab,
            popupSpawnPoint.position + offset,
            Quaternion.identity
        );

        popup.Init(icon);
    }

    private Vector3 GetSpawnOffset(int index)
    {
        switch (index % 3)
        {
            case 0:
                return new Vector3(-horizontalSpacing, 0f, 0f); // 왼쪽
            case 1:
                return new Vector3(horizontalSpacing, 0f, 0f);  // 오른쪽
            default:
                return new Vector3(0f, verticalSpacing, 0f);    // 위쪽
        }
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