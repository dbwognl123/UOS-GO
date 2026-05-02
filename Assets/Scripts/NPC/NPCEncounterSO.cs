using UnityEngine;

public enum SchoolNPCType
{
    Professor,
    Senior,
    Friend,
    Romance
}

[CreateAssetMenu(menuName = "School/NPC Encounter")]
public class NPCEncounterSO : ScriptableObject
{
    [Header("Basic")]
    public string encounterId;
    public SchoolNPCType npcType;
    public string npcName;
    public Sprite portrait;

    [TextArea]
    public string openingLine;

    [Header("Stage")]
    public int stageIndex = 1;

    [Header("Spawn Conditions")]
    public int minAppearanceToAppear;
    public int minCampusLifeToAppear;
    public int minIntelligenceToAppear;
    public int minMoneyToAppear;
    public bool requireNoGirlfriend;
    public int requiredProfessorQuestionCount;

    [Range(0f, 100f)]
    public float appearChance = 100f;

    [Header("Choices")]
    public NPCChoiceData[] choices;
}