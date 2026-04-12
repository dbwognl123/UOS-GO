using UnityEngine;

public class SchoolNPCManager : MonoBehaviour
{
    [SerializeField] private NPCSpawnZone[] zones;
    [SerializeField] private GameObject npcPrefab;

    private void Start()
    {
        SpawnNPCsForToday();
    }

    private void SpawnNPCsForToday()
    {
        foreach (var zone in zones)
        {
            if (zone == null || zone.spawnPoint == null) continue;
            if (zone.possibleEncounters == null || zone.possibleEncounters.Length == 0) continue;

            if (Random.value > zone.spawnChance)
                continue;

            int index = Random.Range(0, zone.possibleEncounters.Length);
            NPCEncounterSO selected = zone.possibleEncounters[index];

            GameObject npcObj = Instantiate(npcPrefab, zone.spawnPoint.position, Quaternion.identity);
            SchoolNPCActor actor = npcObj.GetComponent<SchoolNPCActor>();
            actor.Setup(selected);
        }
    }
}