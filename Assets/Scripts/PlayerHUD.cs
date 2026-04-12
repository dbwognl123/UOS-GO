using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthFill;
    [SerializeField] private int maxHealthDisplay = 100;

    [Header("Intelligence")]
    [SerializeField] private Image intelligenceFill;
    [SerializeField] private int maxIntelligence = 200;

    [Header("Money")]
    [SerializeField] private TMP_Text moneyText;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerStatsRefreshed += RefreshHUD;
            GameManager.Instance.OnPlayerStatChanged += OnStatChanged;
        }

        RefreshHUD();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerStatsRefreshed -= RefreshHUD;
            GameManager.Instance.OnPlayerStatChanged -= OnStatChanged;
        }
    }

    private void OnStatChanged(PlayerStatType statType, int delta)
    {
        RefreshHUD();
    }

    public void RefreshHUD()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null)
            return;

        var player = GameManager.Instance.CurrentPlayer;

        // 체력바: 현재체력을 100 기준 전체 바에서 표시
        if (healthFill != null)
        {
            float hpRatio = (float)player.currentHealth / maxHealthDisplay;
            healthFill.fillAmount = Mathf.Clamp01(hpRatio);
        }

        // 지능바: 현재지능을 200 기준 전체 바에서 표시
        if (intelligenceFill != null)
        {
            float intRatio = (float)player.intelligence / maxIntelligence;
            intelligenceFill.fillAmount = Mathf.Clamp01(intRatio);
        }

        if (moneyText != null)
            moneyText.text = player.money.ToString();
    }
}