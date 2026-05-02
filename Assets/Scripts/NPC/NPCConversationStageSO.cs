using UnityEngine;

[CreateAssetMenu(menuName = "School/NPC Conversation Stage")]
public class NPCConversationStageSO : ScriptableObject
{
    public string stageId;                  // friend_stage_01
    public SchoolNPCType npcType;           // Friend
    public int stageIndex;                  // 1, 2, 3 ...

    [Header("Display")]
    public string npcName;
    public Sprite portrait;

    [TextArea]
    public string openingLine;

    [Header("Show Conditions")]
    public int requiredPreviousSuccessStage; // 이전 성공 단계
    public int minAppearance;
    public int minCampusLife;
    public int minIntelligence;
    public int minMoney;
    public bool requireNoGirlfriend;
    [Range(0f, 100f)] public float appearChance = 100f;

    [Header("Choices")]
    public NPCChoiceData[] choices;
}