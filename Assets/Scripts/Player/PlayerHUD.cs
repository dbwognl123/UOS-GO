using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthText;

    [Header("Intelligence")]
    [SerializeField] private Image intelligenceFill;
    [SerializeField] private TMP_Text intelligenceText;

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

    private void OnStatChanged(
        PlayerStatType statType,
        int delta)
    {
        RefreshHUD();
    }

    public void RefreshHUD()
    {
        GameManager gm =
            GameManager.Instance;

        if (gm == null ||
            gm.CurrentPlayer == null)
        {
            return;
        }

        PlayerRunData player =
            gm.CurrentPlayer;

        // ==============================
        // 체력
        // 현재체력 / 현재 최대체력
        // ==============================

        if (healthFill != null)
        {
            float hpRatio = 0f;

            if (player.maxHealth > 0)
            {
                hpRatio =
                    (float)player.currentHealth /
                    player.maxHealth;
            }

            healthFill.fillAmount =
                Mathf.Clamp01(hpRatio);
        }

        if (healthText != null)
        {
            healthText.text =
                $"{player.currentHealth} / {player.maxHealth}";
        }

        // ==============================
        // 지능
        // 현재 지능 / 이번 주 목표 지능
        // ==============================

        int requiredIntelligence =
            gm.GetCurrentRequiredIntelligence();

        // 0으로 나누는 것 방지
        int safeRequiredIntelligence =
            Mathf.Max(
                1,
                requiredIntelligence
            );

        float intelligenceRatio =
            (float)player.intelligence /
            safeRequiredIntelligence;

        if (intelligenceFill != null)
        {
            intelligenceFill.fillAmount =
                Mathf.Clamp01(
                    intelligenceRatio
                );
        }

        if (intelligenceText != null)
        {
            int percentage =
                Mathf.RoundToInt(
                    intelligenceRatio * 100f
                );

            intelligenceText.text =
                $"{player.intelligence} / " +
                $"{requiredIntelligence} " +
                $"({percentage}%)";
        }

        // ==============================
        // 돈
        // ==============================

        if (moneyText != null)
        {
            moneyText.text =
                player.money.ToString();
        }
    }
}