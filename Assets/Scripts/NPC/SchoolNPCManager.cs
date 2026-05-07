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

            NPCSpawnCandidate selectedCandidate = PickCandidate(zone);
            if (selectedCandidate == null)
                continue;

            NPCEncounterSO selectedEncounter = ResolveEncounterForCurrentProgress(selectedCandidate);
            if (selectedEncounter == null)
            {
                Debug.Log($"[{zone.zoneId}] 조건을 만족하는 단계형 대화가 없음");
                continue;
            }

            GameObject npcObj = Instantiate(
                npcPrefab,
                zone.spawnPoint.position,
                Quaternion.identity
            );

            npcObj.name = $"NPC_{selectedEncounter.npcType}_{selectedEncounter.stageIndex}_{zone.zoneId}";

            SchoolNPCActor actor = npcObj.GetComponent<SchoolNPCActor>();
            if (actor != null)
                actor.Setup(selectedEncounter);
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
            totalWeight += Mathf.Max(0f, c.weight);
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.Range(0f, totalWeight);

        if (roll < zone.noneWeight)
            return null;

        roll -= zone.noneWeight;

        for (int i = 0; i < zone.candidates.Length; i++)
        {
            NPCSpawnCandidate c = zone.candidates[i];
            if (c == null) continue;

            float w = Mathf.Max(0f, c.weight);
            if (roll < w)
                return c;

            roll -= w;
        }

        return null;
    }

    private NPCEncounterSO ResolveEncounterForCurrentProgress(NPCSpawnCandidate candidate)
    {
        if (candidate == null || candidate.stageEncounters == null || candidate.stageEncounters.Length == 0)
            return null;

        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null)
            return null;

        if (GameManager.Instance.IsNPCRetired(candidate.npcType))
            return null;

        int currentStage = GameManager.Instance.GetNPCCurrentStage(candidate.npcType);
        int stageCap = GameManager.Instance.GetNPCStageCap(candidate.npcType);

        int targetStage = Mathf.Min(currentStage, stageCap);

        NPCEncounterSO fallback = null;

        for (int i = 0; i < candidate.stageEncounters.Length; i++)
        {
            NPCEncounterSO encounter = candidate.stageEncounters[i];
            if (encounter == null) continue;

            if (encounter.stageIndex == targetStage)
            {
                if (CanEncounterAppear(encounter))
                    return encounter;
            }

            if (encounter.stageIndex == 1)
                fallback = encounter;
        }

        return fallback;
    }

    private bool CanEncounterAppear(NPCEncounterSO encounter)
    {
        PlayerRunData player = GameManager.Instance.CurrentPlayer;
        if (player == null) return false;

        if (player.appearance < encounter.minAppearanceToAppear)
            return false;

        if (player.campusLife < encounter.minCampusLifeToAppear)
            return false;

        if (player.intelligence < encounter.minIntelligenceToAppear)
            return false;

        if (player.money < encounter.minMoneyToAppear)
            return false;

        if (encounter.requireNoGirlfriend && player.hasGirlfriend)
            return false;

        if (encounter.requiredProfessorQuestionCount > 0 &&
            GameManager.Instance.professorQuestionCount < encounter.requiredProfessorQuestionCount)
            return false;

        float roll = Random.Range(0f, 100f);
        return roll <= encounter.appearChance;
    }
}