using System;
using UnityEngine;

[Serializable]
public class NPCChoiceData
{
    [Header("UI")]
    public string choiceText;

    [Header("Button Enable Requirements")]
    public int requiredMoney;
    public int minAppearance;
    public int minCampusLife;
    public int minIntelligence;
    public bool blockIfHasGirlfriend;

    [Header("Success Chance")]
    public bool useFixedChance = true;
    [Range(0f, 100f)] public float fixedSuccessChance = 100f;

    // 이 값보다 낮으면 무조건 실패
    public int autoFailIfAppearanceBelow;
    public int autoFailIfCampusLifeBelow;
    public int autoFailIfIntelligenceBelow;

    // 고정확률을 안 쓰고 가중치 공식을 쓸 때만 사용
    public float baseChance = 20f;
    public float appearanceWeight = 0.5f;
    public float campusLifeWeight = 0.7f;
    public float intelligenceWeight = 0.2f;
    public float minChance = 5f;
    public float maxChance = 95f;

    [Header("Result Text")]
    [TextArea] public string successLine;
    [TextArea] public string failLine;

    [Header("Effects On Success")]
    public int successMoneyDelta;
    public int successAppearanceDelta;
    public int successCampusLifeDelta;
    public int successIntelligenceDelta;
    public int successGradeDelta;
    public int successHappinessDelta;
    public int successMaxHealthDelta;
    public bool successSetGirlfriend;
    public bool successScheduleMeetingScene;

    public bool successRetireNpcType;
    public bool successUnlockRomanceNpc1;
    [Header("Effects On Fail")]
    public int failMoneyDelta;
    public int failAppearanceDelta;
    public int failCampusLifeDelta;
    public int failIntelligenceDelta;
    public int failGradeDelta;
    public int failHappinessDelta;
    public int failMaxHealthDelta;

    [Header("Progression On Success")]
    public int successSetStageTo = -1;
    public int successSetStageCapTo = -1;
    public int successProfessorQuestionCountDelta;
    public string successUnlockEndingId;

    [Header("Progression On Fail")]
    public int failSetStageTo = -1;
    public int failSetStageCapTo = -1;
    public bool failRetireNpcType;
    public int failProfessorQuestionCountDelta;
    public string failUnlockEndingId;

    public bool failLockRomanceNpc1;

    public bool useDynamicRomanceSuccessChance;
    public float romanceBaseChance = 15f;
    public float romanceAppearanceWeight = 1.5f;
    public float romanceCampusLifeWeight = 0.15f;
    public float romanceMaxChance = 85f;

    [Header("Festival Romance")]
    public bool successSetFestivalDatePromise;

    public bool successStartFestivalDate;
}