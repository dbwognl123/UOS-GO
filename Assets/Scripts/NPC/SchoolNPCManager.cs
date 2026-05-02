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
        if (npcPrefab == null)
        {
            Debug.LogWarning("SchoolNPCManager: npcPrefab이 비어 있음");
            return;
        }

        for (int i = 0; i < zones.Length; i++)
        {
            NPCSpawnZone zone = zones[i];
            if (zone == null) continue;
            if (zone.spawnPoint == null) continue;

            NPCSpawnCandidate selected = PickCandidate(zone);
            if (selected == null || selected.encounterData == null)
                continue;

            GameObject npcObj = Instantiate(
                npcPrefab,
                zone.spawnPoint.position,
                Quaternion.identity
            );

            npcObj.name = $"NPC_{selected.npcType}_{zone.zoneId}";

            SchoolNPCActor actor = npcObj.GetComponent<SchoolNPCActor>();
            if (actor != null)
            {
                actor.Setup(selected.encounterData);
            }
            else
            {
                Debug.LogWarning("SchoolNPCManager: npcPrefab에 SchoolNPCActor가 없음");
            }
        }
    }

    private NPCSpawnCandidate PickCandidate(NPCSpawnZone zone)
    {
        if (zone.candidates == null || zone.candidates.Length == 0)
            return null;

        float totalWeight = zone.noneWeight;

        for (int i = 0; i < zone.candidates.Length; i++)
        {
            NPCSpawnCandidate c = zone.candidates[i];
            if (c == null) continue;
            if (c.encounterData == null) continue;
            totalWeight += Mathf.Max(0f, c.weight);
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.Range(0f, totalWeight);

        // "아무도 안 뜸"
        if (roll < zone.noneWeight)
        {
            Debug.Log($"[{zone.zoneId}] 아무도 스폰되지 않음");
            return null;
        }

        roll -= zone.noneWeight;

        for (int i = 0; i < zone.candidates.Length; i++)
        {
            NPCSpawnCandidate c = zone.candidates[i];
            if (c == null) continue;
            if (c.encounterData == null) continue;

            float w = Mathf.Max(0f, c.weight);
            if (roll < w)
            {
                Debug.Log($"[{zone.zoneId}] 선택된 NPC 타입: {c.npcType}");
                return c;
            }

            roll -= w;
        }

        return null;
    }
}