using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthFill;
    [SerializeField] private int maxHealth = 100;

    [Header("Intelligence")]
    [SerializeField] private Image intelligenceFill;
    [SerializeField] private int maxIntelligence = 100;

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

        if (healthFill != null)
        {
            float value = 1f;

            if (player.maxHealth > 0)
                value = (float)player.currentHealth / player.maxHealth;

            healthFill.fillAmount = Mathf.Clamp01(value);
        }
        if (intelligenceFill != null)
            intelligenceFill.fillAmount = Mathf.Clamp01((float)player.intelligence / maxIntelligence);

        if (moneyText != null)
            moneyText.text = player.money.ToString();
    }
}