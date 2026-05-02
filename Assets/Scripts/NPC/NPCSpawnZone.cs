using System;
using UnityEngine;

[Serializable]
public class NPCSpawnCandidate
{
    public SchoolNPCType npcType;
    public NPCEncounterSO encounterData;
    [Range(0f, 100f)] public float weight = 10f;
}

public class NPCSpawnZone : MonoBehaviour
{
    [Header("Zone Info")]
    public string zoneId;
    public Transform spawnPoint;

    [Header("No Spawn Weight")]
    [Range(0f, 100f)] public float noneWeight = 50f;

    [Header("Candidates")]
    public NPCSpawnCandidate[] candidates;
}