
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MorningSceneUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gateChoicePanel;
    [SerializeField] private GameObject schedulePanel;

    [Header("Gate Dialogue")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Character Portraits")]
    [SerializeField] private Sprite defaultPortrait;
    [SerializeField] private Sprite athletePortrait;
    [SerializeField] private Sprite honorStudentPortrait;
    [SerializeField] private Sprite popularStudentPortrait;

    [Header("Schedule Texts")]
    [SerializeField] private TMP_Text[] classTexts;

    [Header("Rest Button")]
    [SerializeField] private GameObject restButton;
    [SerializeField] private float restDelay = 10f;

    private bool restChosen = false;

    private float idleTimer = 0f;
    private bool restButtonUnlocked = false;
    private Vector3 lastMousePosition;

    private void Start()
    {
        if (gateChoicePanel != null)
            gateChoicePanel.SetActive(false);

        if (schedulePanel != null)
            schedulePanel.SetActive(false);

        if (restButton != null)
            restButton.SetActive(false);

        lastMousePosition = Input.mousePosition;

        RefreshScheduleUI();

        // 초상화 연결 테스트
        Debug.Log("[Morning] MorningSceneUI Start 실행됨");
        RefreshGateDialogue();
    }

    // ======================================
    // 10초 동안 입력이 없으면 휴식 등장
    // ======================================

    private void Update()
    {
        if (gateChoicePanel == null ||
            !gateChoicePanel.activeInHierarchy)
            return;

        if (restButtonUnlocked)
            return;

        Vector3 currentMousePosition = Input.mousePosition;

        bool mouseMoved =
            (currentMousePosition - lastMousePosition).sqrMagnitude > 4f;

        lastMousePosition = currentMousePosition;

        bool hasInput =
            Input.anyKey ||
            mouseMoved ||
            Input.mouseScrollDelta.sqrMagnitude > 0f;

        if (hasInput)
        {
            idleTimer = 0f;
            return;
        }

        idleTimer += Time.unscaledDeltaTime;

        if (idleTimer >= restDelay)
        {
            restButtonUnlocked = true;

            if (restButton != null)
                restButton.SetActive(true);

            Debug.Log("[Morning] 휴식 버튼 활성화");
        }
    }

    // ======================================
    // 캐릭터 초상화와 대사
    // ======================================

    private void RefreshGateDialogue()
    {
        if (dialogueText != null)
        {
            dialogueText.text =
                "음... 오늘은 어디로 가야 할까...";
        }

        if (portraitImage == null)
        {
            Debug.LogError(
                "[Morning] Portrait Image가 연결되지 않았습니다."
            );
            return;
        }

        GameManager gm = GameManager.Instance;

        if (gm == null || gm.CurrentPlayer == null)
        {
            Debug.LogError(
                "[Morning] 플레이어 데이터가 없습니다."
            );
            return;
        }

        string characterId =
            gm.CurrentPlayer.activeCharacterId;

        Sprite selectedPortrait = defaultPortrait;

        switch (characterId)
        {
            case "운동부":
                selectedPortrait = athletePortrait;
                break;

            case "우등생":
                selectedPortrait = honorStudentPortrait;
                break;

            case "엄친아":
                selectedPortrait = popularStudentPortrait;
                break;

            default:
                selectedPortrait = defaultPortrait;
                break;
        }

        if (selectedPortrait == null)
        {
            Debug.LogError(
                $"[Morning] {characterId} 초상화 Sprite가 없습니다."
            );
            return;
        }

        portraitImage.sprite = selectedPortrait;
        portraitImage.color = Color.white;
        portraitImage.preserveAspect = true;

        Debug.Log(
            $"[Morning] ID={characterId}, " +
            $"Sprite={selectedPortrait.name}, " +
            $"Image={portraitImage.gameObject.name}"
        );
    }

    // ======================================
    // 문 선택창
    // ======================================

    public void OpenGateChoiceUI()
    {
        idleTimer = 0f;
        restButtonUnlocked = false;
        restChosen = false;
        lastMousePosition = Input.mousePosition;

        if (restButton != null)
            restButton.SetActive(false);

        if (gateChoicePanel != null)
            gateChoicePanel.SetActive(true);

        RefreshGateDialogue();
    }

    public void CloseGateChoiceUI()
    {
        if (gateChoicePanel != null)
            gateChoicePanel.SetActive(false);

        idleTimer = 0f;
        restButtonUnlocked = false;

        if (restButton != null)
            restButton.SetActive(false);
    }

    // ======================================
    // 시간표
    // ======================================

    public void RefreshScheduleUI()
    {
        if (GameManager.Instance == null)
            return;

        if (classTexts == null || classTexts.Length < 5)
            return;

        var schedule =
            GameManager.Instance.TodaySchedule;

        for (int i = 0; i < classTexts.Length; i++)
        {
            if (classTexts[i] == null)
                continue;

            if (i < schedule.Count)
            {
                classTexts[i].text =
                    GameManager.Instance
                        .GetClassroomDisplayMultiline(schedule[i]);
            }
            else
            {
                classTexts[i].text = "공강";
            }
        }
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

    // ======================================
    // 학교 출입구 선택
    // ======================================

    public void OnClickFrontGate()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.EnterSchool(
            SchoolEntryType.FrontGate
        );
    }

    public void OnClickSideGate()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.EnterSchool(
            SchoolEntryType.SideGate
        );
    }

    public void OnClickBackGate()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.EnterSchool(
            SchoolEntryType.BackGate
        );
    }

    // ======================================
    // 10초 후 휴식 선택
    // ======================================


    public void OnClickSkipSchool()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null || gm.CurrentPlayer == null)
            return;

        // 10초가 지나야 휴식 가능
        if (!restButtonUnlocked)
            return;

        // 중복 클릭 방지
        if (restChosen)
            return;

        restChosen = true;

        // 최대체력 +20 (캐릭터 배율 미적용)
        gm.AddMaxHealth(20);

        // 행복 +10
        gm.AddHappiness(10);

        Debug.Log(
            $"[Morning] 휴식 완료! " +
            $"최대체력: {gm.CurrentPlayer.maxHealth}, " +
            $"행복: {gm.CurrentPlayer.happiness}"
        );

        // 변경된 스탯 저장
        gm.SaveGame();

        // 저녁으로 이동
        gm.SkipSchool();
    }

}
