using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchoolNPCUI : MonoBehaviour
{
    public static SchoolNPCUI Instance { get; private set; }

    [Header("Root")]
    [SerializeField] private GameObject rootPanel;

    [Header("Main UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text resultText;

    [Header("Choices")]
    [SerializeField] private Transform choicesRoot;
    [SerializeField] private Button choiceButtonPrefab;

    [Header("Close")]
    [SerializeField] private Button closeButton;

    [Header("Optional Control Lock")]
    [SerializeField] private MonoBehaviour playerMovementScript;
    [SerializeField] private MonoBehaviour schoolFatigueScript;

    private SchoolNPCActor currentActor;
    private readonly List<Button> spawnedButtons = new();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (rootPanel != null)
            rootPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDialogue);
    }

    public void OpenSimpleDialogue(string npcName, Sprite portrait, string line)
    {
        currentActor = null;

        if (rootPanel != null)
            rootPanel.SetActive(true);

        if (portraitImage != null)
            portraitImage.sprite = portrait;

        if (nameText != null)
            nameText.text = npcName;

        if (dialogueText != null)
            dialogueText.text = line;

        if (resultText != null)
            resultText.text = string.Empty;

        ClearChoiceButtons();
        LockPlayer(true);
    }

    public void OpenDialogue(SchoolNPCActor actor)
    {
        if (actor == null || actor.EncounterData == null)
            return;

        currentActor = actor;
        NPCEncounterSO data = actor.EncounterData;

        if (rootPanel != null)
            rootPanel.SetActive(true);

        if (portraitImage != null)
            portraitImage.sprite = data.portrait;

        if (nameText != null)
            nameText.text = data.npcName;

        if (dialogueText != null)
            dialogueText.text = data.openingLine;

        if (resultText != null)
            resultText.text = string.Empty;
        LockPlayer(true);
        RebuildChoiceButtons(data);
    }

    public void CloseDialogue()
    {
        ClearChoiceButtons();

        if (rootPanel != null)
            rootPanel.SetActive(false);

        if (resultText != null)
            resultText.text = string.Empty;

        currentActor = null;

        LockPlayer(false);
    }
    private void RebuildChoiceButtons(NPCEncounterSO data)
    {
        ClearChoiceButtons();

        if (data == null || data.choices == null || data.choices.Length == 0)
            return;

        PlayerRunData player = GameManager.Instance != null ? GameManager.Instance.CurrentPlayer : null;
        if (player == null) return;

        for (int i = 0; i < data.choices.Length; i++)
        {
            NPCChoiceData choice = data.choices[i];
            Button btn = Instantiate(choiceButtonPrefab, choicesRoot);
            spawnedButtons.Add(btn);

            TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
            bool canChoose = NPCChoiceResolver.CanChoose(player, choice, out string reason);

            if (btnText != null)
            {
                btnText.text = canChoose
                    ? choice.choiceText
                    : $"{choice.choiceText} (불가: {reason})";
            }

            btn.interactable = canChoose;

            int capturedIndex = i;
            btn.onClick.AddListener(() => OnChoiceClicked(data.choices[capturedIndex]));
        }
    }

    private void OnChoiceClicked(NPCChoiceData choice)
    {
        PlayerRunData player = GameManager.Instance != null ? GameManager.Instance.CurrentPlayer : null;
        if (player == null) return;

        bool canChoose = NPCChoiceResolver.CanChoose(player, choice, out string reason);
        if (!canChoose)
        {
            if (resultText != null)
                resultText.text = reason;
            return;
        }

        float chance = NPCChoiceResolver.EvaluateSuccessChance(player, choice);
        float roll = Random.Range(0f, 100f);
        bool success = roll <= chance;

        ApplyChoiceResult(choice, success);

        if (dialogueText != null)
            dialogueText.text = success ? choice.successLine : choice.failLine;

        if (resultText != null)
            resultText.text = $"성공확률 {chance:F0}% / 판정 {roll:F0}";

        // 한 번 선택했으면 다시 선택지 못 누르게
        ClearChoiceButtons();

        // NPC를 1회성으로 만들고 싶으면 주석 해제
        // if (currentActor != null)
        //     currentActor.gameObject.SetActive(false);
    

    }

    private void ApplyChoiceResult(NPCChoiceData choice, bool success)
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null || currentActor == null || currentActor.EncounterData == null)
            return;

        SchoolNPCType npcType = currentActor.EncounterData.npcType;

        if (success)
        {
            ApplyMoney(choice.successMoneyDelta);
            ApplyAppearance(choice.successAppearanceDelta);
            ApplyCampusLife(choice.successCampusLifeDelta);
            ApplyIntelligence(choice.successIntelligenceDelta);
            ApplyGrade(choice.successGradeDelta);
            ApplyHappiness(choice.successHappinessDelta);
            ApplyMaxHealth(choice.successMaxHealthDelta);

            if (choice.successSetGirlfriend)
                GameManager.Instance.CurrentPlayer.hasGirlfriend = true;

            if (choice.successSetStageTo >= 1)
                GameManager.Instance.SetNPCStage(npcType, choice.successSetStageTo);

            if (choice.successSetStageCapTo >= 1)
                GameManager.Instance.SetNPCStageCap(npcType, choice.successSetStageCapTo);

            if (choice.successRetireNpcType)
                GameManager.Instance.RetireNPCType(npcType);

            if (choice.successProfessorQuestionCountDelta != 0)
                GameManager.Instance.AddProfessorQuestionCount(choice.successProfessorQuestionCountDelta);

            if (!string.IsNullOrEmpty(choice.successUnlockEndingId))
                GameManager.Instance.UnlockEndingFlag(choice.successUnlockEndingId);

            // 추가: 친구 3단계 성공 선택지 같은 특수 이벤트 예약
            if (choice.successScheduleMeetingScene)
                GameManager.Instance.ScheduleMeetingScene();
        }
        else
        {
            ApplyMoney(choice.failMoneyDelta);
            ApplyAppearance(choice.failAppearanceDelta);
            ApplyCampusLife(choice.failCampusLifeDelta);
            ApplyIntelligence(choice.failIntelligenceDelta);
            ApplyGrade(choice.failGradeDelta);
            ApplyHappiness(choice.failHappinessDelta);
            ApplyMaxHealth(choice.failMaxHealthDelta);

            if (choice.failSetStageTo >= 1)
                GameManager.Instance.SetNPCStage(npcType, choice.failSetStageTo);

            if (choice.failSetStageCapTo >= 1)
                GameManager.Instance.SetNPCStageCap(npcType, choice.failSetStageCapTo);

            if (choice.failRetireNpcType)
                GameManager.Instance.RetireNPCType(npcType);

            if (choice.failProfessorQuestionCountDelta != 0)
                GameManager.Instance.AddProfessorQuestionCount(choice.failProfessorQuestionCountDelta);

            if (!string.IsNullOrEmpty(choice.failUnlockEndingId))
                GameManager.Instance.UnlockEndingFlag(choice.failUnlockEndingId);
        }

        GameManager.Instance.MarkNPCStageUsedToday(
            npcType,
            currentActor.EncounterData.stageIndex
        );
    }

    private void ApplyMoney(int delta)
    {
        if (delta == 0 || GameManager.Instance == null) return;
        GameManager.Instance.AddMoney(delta);
    }

    private void ApplyIntelligence(int delta)
    {
        if (delta == 0 || GameManager.Instance == null) return;
        GameManager.Instance.AddIntelligence(delta);
    }

    private void ApplyGrade(int delta)
    {
        if (delta == 0 || GameManager.Instance == null) return;
        GameManager.Instance.AddGrade(delta);
    }

    private void ApplyAppearance(int delta)
    {
        if (delta == 0 || GameManager.Instance == null) return;
        GameManager.Instance.AddAppearance(delta);
    }

    private void ApplyCampusLife(int delta)
    {
        if (delta == 0 || GameManager.Instance == null) return;
        GameManager.Instance.AddCampusLife(delta);
    }

    private void ApplyHappiness(int delta)
    {
        if (delta == 0 || GameManager.Instance == null) return;
        GameManager.Instance.AddHappiness(delta);
    }

    private void ApplyMaxHealth(int delta)
    {
        if (delta == 0 || GameManager.Instance == null) return;
        GameManager.Instance.AddMaxHealth(delta);
    }
    

    private void RefreshHUDIfPossible()
    {
        // 네 GameManager에 외모/학교생활력 전용 이벤트 함수 없으면,
        // HUD는 돈/지능/체력 쪽만 자동 반영되고
        // 나머지는 저장값만 바뀌는 상태가 된다.
        // 필요하면 GameManager에 AddAppearance / AddCampusLife를 추가하는 게 가장 깔끔함.
    }

    private void ClearChoiceButtons()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            if (spawnedButtons[i] != null)
                Destroy(spawnedButtons[i].gameObject);
        }

        spawnedButtons.Clear();
    }

    private void LockPlayer(bool locked)
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = !locked;

        if (schoolFatigueScript != null)
            schoolFatigueScript.enabled = !locked;
    }
}