using System;
using UnityEngine;

[Serializable]
public class NPCChoiceData
{
    [Header("UI")]
    public string choiceText;

    [Header("Hard Requirements")]
    public int requiredMoney;
    public int minAppearance;
    public int minCampusLife;
    public int minIntelligence;
    public bool blockIfHasGirlfriend;

    [Header("Success Formula")]
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
    public bool successSetGirlfriend;

    [Header("Effects On Fail")]
    public int failMoneyDelta;
    public int failAppearanceDelta;
    public int failCampusLifeDelta;
    public int failIntelligenceDelta;
    public int failGradeDelta;
}