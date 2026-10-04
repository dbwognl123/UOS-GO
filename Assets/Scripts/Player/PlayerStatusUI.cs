using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject rootPanel;

    [Header("Portrait")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite playerPortrait;

    [Header("Stats")]
    [SerializeField] private TMP_Text weekText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text intelligenceText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text appearanceText;
    [SerializeField] private TMP_Text campusLifeText;
    [SerializeField] private TMP_Text gradeText;
    [SerializeField] private TMP_Text happinessText;
    [SerializeField] private TMP_Text romanceText;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    private bool isOpen = false;
    private float previousTimeScale = 1f;

    private void Start()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);

        if (portraitImage != null &&
            playerPortrait != null)
        {
            portraitImage.sprite = playerPortrait;
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(toggleKey))
            return;

        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        if (!isOpen && gm.IsDialogueOpen)
            return;

        if (isOpen)
            Close();
        else
            Open();
    }

    private void Open()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null ||
            gm.CurrentPlayer == null)
            return;

        isOpen = true;

        Refresh();

        if (rootPanel != null)
            rootPanel.SetActive(true);

        gm.SetDialogueOpen(true);

        previousTimeScale =
            Time.timeScale;

        Time.timeScale = 0f;
    }

    private void Close()
    {
        isOpen = false;

        if (rootPanel != null)
            rootPanel.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.SetDialogueOpen(false);

        Time.timeScale =
            previousTimeScale;
    }

    private void Refresh()
    {
        GameManager gm =
            GameManager.Instance;

        if (gm == null ||
            gm.CurrentPlayer == null)
            return;

        PlayerRunData player =
            gm.CurrentPlayer;

        if (weekText != null)
        {
            weekText.text =
                $"{gm.CurrentWeek}주차";
        }

        if (healthText != null)
        {
            healthText.text =
                $"체력:" +
                $"{player.currentHealth} / {player.maxHealth}";
        }

        int requiredIntelligence =
            gm.GetCurrentRequiredIntelligence();

        if (intelligenceText != null)
        {
            intelligenceText.text =
                $"지능: " +
                $"{player.intelligence} / {requiredIntelligence}";
        }

        if (moneyText != null)
        {
            moneyText.text =
                $"돈: {player.money}";
        }

        if (appearanceText != null)
        {
            appearanceText.text =
                $"외모: {player.appearance}";
        }

        if (campusLifeText != null)
        {
            campusLifeText.text =
                $"학교생활: {player.campusLife}";
        }

        if (gradeText != null)
        {
            gradeText.text =
                $"학점: {player.grade}";
        }

        if (happinessText != null)
        {
            happinessText.text =
                $"행복: {player.happiness}";
        }

        if (romanceText != null)
        {
            romanceText.text =
                $"호감도: {player.romanceAffection}";
        }
    }

    private void OnDisable()
    {
        if (!isOpen)
            return;

        isOpen = false;

        Time.timeScale =
            previousTimeScale;

        if (GameManager.Instance != null)
            GameManager.Instance.SetDialogueOpen(false);
    }
}