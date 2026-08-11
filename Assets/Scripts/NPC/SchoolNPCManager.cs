using System.Collections.Generic;
using UnityEngine;

public class SchoolNPCManager : MonoBehaviour
{
    [SerializeField] private NPCSpawnZone[] zones;
    [SerializeField] private GameObject npcPrefab;

    private class WeightedEncounterOption
    {
        public NPCEncounterSO encounter;
        public float weight;
    }

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
           

            NPCEncounterSO selectedEncounter = ResolveRandomEncounterFromZone(zone);
            if (selectedEncounter == null)
            {
                Debug.Log($"[{zone.zoneId}] 조건을 만족하는 NPC encounter가 없음");
                continue;
            }

            GameObject npcObj = Instantiate(
            npcPrefab,
            zone.transform.position,
            Quaternion.identity
            );
            npcObj.name = $"NPC_{selectedEncounter.npcType}_{selectedEncounter.stageIndex}_{zone.zoneId}";

            SchoolNPCActor actor = npcObj.GetComponent<SchoolNPCActor>();
            if (actor != null)
                actor.Setup(selectedEncounter);
        }
    }

    private NPCEncounterSO ResolveRandomEncounterFromZone(NPCSpawnZone zone)
    {
        if (zone == null || zone.candidates == null || zone.candidates.Length == 0)
            return null;

        List<WeightedEncounterOption> options = new List<WeightedEncounterOption>();

        for (int i = 0; i < zone.candidates.Length; i++)
        {
            NPCSpawnCandidate candidate = zone.candidates[i];
            if (candidate == null) continue;
            if (candidate.stageEncounters == null || candidate.stageEncounters.Length == 0) continue;

            List<NPCEncounterSO> eligibleInCandidate = new List<NPCEncounterSO>();

            for (int j = 0; j < candidate.stageEncounters.Length; j++)
            {
                NPCEncounterSO encounter = candidate.stageEncounters[j];
                if (encounter == null) continue;

                if (CanEncounterAppear(encounter))
                    eligibleInCandidate.Add(encounter);
            }

            if (eligibleInCandidate.Count == 0)
                continue;

            float candidateWeight = Mathf.Max(0f, candidate.weight);
            float splitWeight = candidateWeight / eligibleInCandidate.Count;

            for (int j = 0; j < eligibleInCandidate.Count; j++)
            {
                options.Add(new WeightedEncounterOption
                {
                    encounter = eligibleInCandidate[j],
                    weight = splitWeight
                });
            }
        }

        float totalWeight = zone.noneWeight;

        for (int i = 0; i < options.Count; i++)
            totalWeight += options[i].weight;

        if (totalWeight <= 0f)
            return null;

        float roll = Random.Range(0f, totalWeight);

        if (roll < zone.noneWeight)
            return null;

        roll -= zone.noneWeight;

        for (int i = 0; i < options.Count; i++)
        {
            if (roll < options[i].weight)
                return options[i].encounter;

            roll -= options[i].weight;
        }

        return null;
    }

    private bool CanEncounterAppear(NPCEncounterSO encounter)
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null)
            return false;

        PlayerRunData player = GameManager.Instance.CurrentPlayer;

        if (GameManager.Instance.IsNPCRetired(encounter.npcType))
            return false;

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

        if (GameManager.Instance.CurrentWeek < encounter.minWeekToAppear)
            return false;

        if (GameManager.Instance.CurrentWeek > encounter.maxWeekToAppear)
            return false;

        if (encounter.npcType == SchoolNPCType.Romance &&
            encounter.stageIndex == 1 &&
            !GameManager.Instance.romanceNpc1Unlocked)
            return false;

        float roll = Random.Range(0f, 100f);
        return roll <= encounter.appearChance;
    }
}