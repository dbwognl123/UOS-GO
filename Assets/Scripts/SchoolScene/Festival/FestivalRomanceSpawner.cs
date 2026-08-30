using UnityEngine;

public class FestivalRomanceSpawner : MonoBehaviour
{
    [Header("Festival Romance")]
    [SerializeField] private GameObject romanceNpcPrefab;
    [SerializeField] private NPCEncounterSO festivalEncounter;
    [SerializeField] private Transform spawnPoint;

    private GameObject spawnedNpc;

    private void Update()
    {
        if (spawnedNpc != null)
            return;

        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        // 10주차에만
        if (gm.CurrentWeek != 10)
            return;

        // 축제 약속을 잡았어야 함
        if (!gm.hasFestivalDatePromise)
            return;

        // 수업을 모두 끝낸 뒤에만 등장
        if (!gm.IsAllClassesFinished)
            return;

        SpawnRomanceNpc();
    }

    private void SpawnRomanceNpc()
    {
        if (romanceNpcPrefab == null)
        {
            Debug.LogWarning(
                "[FestivalRomanceSpawner] Romance NPC Prefab이 없습니다."
            );
            return;
        }

        if (festivalEncounter == null)
        {
            Debug.LogWarning(
                "[FestivalRomanceSpawner] Festival Encounter가 없습니다."
            );
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning(
                "[FestivalRomanceSpawner] SpawnPoint가 없습니다."
            );
            return;
        }

        spawnedNpc = Instantiate(
            romanceNpcPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        SchoolNPCActor actor =
            spawnedNpc.GetComponent<SchoolNPCActor>();

        if (actor == null)
        {
            Debug.LogWarning(
                "[FestivalRomanceSpawner] " +
                "Prefab에 SchoolNPCActor가 없습니다."
            );

            Destroy(spawnedNpc);
            spawnedNpc = null;

            return;
        }

        actor.Setup(festivalEncounter);

        Debug.Log(
            "[Festival] 10주차 축제 Romance NPC가 " +
            "학생회관 앞에 생성되었습니다."
        );
    }
}