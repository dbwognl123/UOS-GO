using UnityEngine;

[CreateAssetMenu(menuName = "School/NPC Type Stage Database")]
public class NPCTypeStageDatabaseSO : ScriptableObject
{
    public SchoolNPCType npcType;
    public NPCConversationStageSO[] stages;
}