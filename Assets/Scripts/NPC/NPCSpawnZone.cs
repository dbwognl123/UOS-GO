using UnityEngine;

public class NPCSpawnZone : MonoBehaviour
{
    public string zoneId;
    [Range(0f, 1f)] public float spawnChance = 0.3f;
    public Transform spawnPoint;
    public NPCEncounterSO[] possibleEncounters;
}