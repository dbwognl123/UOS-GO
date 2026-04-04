using TMPro;
using UnityEngine;

public class MorningSceneUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gateChoicePanel;
    [SerializeField] private GameObject schedulePanel;

    [Header("Schedule Texts")]
    [SerializeField] private TMP_Text[] classTexts; // 5칸 연결

    private void Start()
    {
        if (gateChoicePanel != null)
            gateChoicePanel.SetActive(false);

        if (schedulePanel != null)
            schedulePanel.SetActive(false);

        RefreshScheduleUI();
    }

    public void RefreshScheduleUI()
    {
        if (GameManager.Instance == null) return;
        if (classTexts == null || classTexts.Length < 5) return;

        var schedule = GameManager.Instance.TodaySchedule;

        for (int i = 0; i < classTexts.Length; i++)
        {
            if (classTexts[i] == null) continue;

            if (i < schedule.Count)
                classTexts[i].text = schedule[i].ToString();
            else
                classTexts[i].text = "-";
        }
    }

    public void OpenGateChoiceUI()
    {
        if (gateChoicePanel != null)
            gateChoicePanel.SetActive(true);
    }

    public void CloseGateChoiceUI()
    {
        if (gateChoicePanel != null)
            gateChoicePanel.SetActive(false);
    }

    public void OpenScheduleUI()
    {
        RefreshScheduleUI();

        if (schedulePanel != null)
            schedulePanel.SetActive(true);
    }

    public void CloseScheduleUI()
    {
        if (schedulePanel != null)
            schedulePanel.SetActive(false);
    }

    public void OnClickSkipSchool()
    {
        GameManager.Instance.SkipSchool();
    }

    public void OnClickFrontGate()
    {
        GameManager.Instance.EnterSchool(SchoolEntryType.FrontGate);
    }

    public void OnClickSideGate()
    {
        GameManager.Instance.EnterSchool(SchoolEntryType.SideGate);
    }

    public void OnClickBackGate()
    {
        GameManager.Instance.EnterSchool(SchoolEntryType.BackGate);
    }
}