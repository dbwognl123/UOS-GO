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
    public string encounterId;
    public SchoolNPCType npcType;
    public string npcName;
    public Sprite portrait;

    [TextArea]
    public string openingLine;

    public NPCChoiceData[] choices;
}