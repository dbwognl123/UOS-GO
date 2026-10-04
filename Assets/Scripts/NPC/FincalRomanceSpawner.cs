using UnityEngine;

public class FinalRomanceSpawner : MonoBehaviour
{
    [Header("Final Romance")]
    [SerializeField]
    private GameObject romanceNpcPrefab;

    [SerializeField]
    private NPCEncounterSO finalEncounter;

    [SerializeField]
    private Transform spawnPoint;

    private GameObject spawnedNpc;

    private void Update()
    {
        if (spawnedNpc != null)
            return;

        GameManager gm =
            GameManager.Instance;

        if (gm == null)
            return;

        // 16주차만
        if (gm.CurrentWeek != 16)
            return;

        // 수업 전부 완료해야 등장
        if (!gm.IsAllClassesFinished)
            return;

        // 이미 고백 이벤트 끝났으면 등장 X
        if (gm.finalConfessionResolved)
            return;

        SpawnNpc();
    }

    private void SpawnNpc()
    {
        if (romanceNpcPrefab == null ||
            finalEncounter == null ||
            spawnPoint == null)
        {
            Debug.LogWarning(
                "[FinalRomanceSpawner] Inspector 설정 누락"
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
                "[FinalRomanceSpawner] " +
                "SchoolNPCActor가 없습니다."
            );

            Destroy(spawnedNpc);
            spawnedNpc = null;

            return;
        }

        actor.Setup(finalEncounter);

        Debug.Log(
            "[Final Romance] " +
            "16주차 고백 NPC 생성"
        );
    }
}