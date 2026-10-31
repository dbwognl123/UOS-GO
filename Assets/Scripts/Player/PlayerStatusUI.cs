
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
    [SerializeField] private Sprite athletePortrait;
    [SerializeField] private Sprite honorStudentPortrait;
    [SerializeField] private Sprite popularStudentPortrait;

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

        if (gm == null || gm.CurrentPlayer == null)
            return;

        isOpen = true;

        // 먼저 패널 활성화
        if (rootPanel != null)
            rootPanel.SetActive(true);

        // 패널이 활성화된 뒤 초상화 및 스탯 갱신
        Refresh();

        gm.SetDialogueOpen(true);

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void Close()
    {
        isOpen = false;

        if (rootPanel != null)
            rootPanel.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.SetDialogueOpen(false);

        Time.timeScale = previousTimeScale;
    }

    // ========================================
    // 현재 플레이 캐릭터에 따른 초상화 갱신
    // ========================================


    private void RefreshPortrait(PlayerRunData player)
    {
        if (portraitImage == null || player == null)
        {
            Debug.LogError("[Portrait] Image 또는 Player가 NULL");
            return;
        }

        string id = player.activeCharacterId;

        Sprite selected = playerPortrait;

        switch (id)
        {
            case "운동부":
                selected = athletePortrait;
                break;

            case "우등생":
                selected = honorStudentPortrait;
                break;

            case "엄친아":
                selected = popularStudentPortrait;
                break;
        }

        if (selected == null)
        {
            Debug.LogError($"[Portrait] {id} Sprite가 NULL");
            return;
        }

        portraitImage.sprite = selected;
        portraitImage.preserveAspect = true;

        Debug.Log(
            $"[Portrait] ID={id}, " +
            $"Sprite={selected.name}, " +
            $"Image={portraitImage.gameObject.name}"
        );
    }


    private void Refresh()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null ||
            gm.CurrentPlayer == null)
            return;

        PlayerRunData player = gm.CurrentPlayer;

        // 캐릭터 초상화 갱신
        RefreshPortrait(player);

        if (weekText != null)
        {
            weekText.text = $"{gm.CurrentWeek}주차";
        }

        if (healthText != null)
        {
            healthText.text =
                $"체력: {player.currentHealth} / {player.maxHealth}";
        }

        int requiredIntelligence =
            gm.GetCurrentRequiredIntelligence();

        if (intelligenceText != null)
        {
            intelligenceText.text =
                $"지능: {player.intelligence} / {requiredIntelligence}";
        }

        if (moneyText != null)
        {
            moneyText.text = $"돈: {player.money}";
        }

        if (appearanceText != null)
        {
            appearanceText.text = $"외모: {player.appearance}";
        }

        if (campusLifeText != null)
        {
            campusLifeText.text = $"학교생활: {player.campusLife}";
        }

        if (gradeText != null)
        {
            gradeText.text = $"학점: {player.grade}";
        }

        if (happinessText != null)
        {
            happinessText.text = $"행복: {player.happiness}";
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

        Time.timeScale = previousTimeScale;

        if (GameManager.Instance != null)
            GameManager.Instance.SetDialogueOpen(false);
    }
}
