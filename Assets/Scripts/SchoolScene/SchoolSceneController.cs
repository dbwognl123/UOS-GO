using UnityEngine;

public class SchoolSceneController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private SchoolSpawnPoint[] spawnPoints;

    private void Start()
    {
        SpawnPlayerAtEntryPoint();
    }

    private void SpawnPlayerAtEntryPoint()
    {
        SchoolEntryType entryType = GameManager.Instance.CurrentSchoolEntry;

        foreach (var point in spawnPoints)
        {
            if (point.entryType == entryType)
            {
                player.position = point.transform.position;
                return;
            }
        }

        Debug.LogWarning("해당 entryType에 맞는 스폰 위치를 찾지 못했습니다.");
    }
}