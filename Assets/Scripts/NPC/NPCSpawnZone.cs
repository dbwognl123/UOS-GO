using System;
using UnityEngine;

[Serializable]
public class NPCSpawnCandidate
{
    public SchoolNPCType npcType;

    [Tooltip("이 NPC 타입의 단계별 대화 에셋들")]
    public NPCEncounterSO[] stageEncounters;

    [Range(0f, 100f)]
    public float weight = 10f;
}

public class NPCSpawnZone : MonoBehaviour
{
    [Header("Zone Info")]
    public string zoneId;
    public Transform spawnPoint;

    [Header("No Spawn Weight")]
    [Range(0f, 100f)]
    public float noneWeight = 50f;

    [Header("Candidates")]
    public NPCSpawnCandidate[] candidates;
}