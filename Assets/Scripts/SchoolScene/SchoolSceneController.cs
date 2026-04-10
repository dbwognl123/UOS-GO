using UnityEngine;

public class SchoolSceneController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private SchoolSpawnPoint[] spawnPoints;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("player가 연결되지 않았습니다.");
            return;
        }

        if (GameManager.Instance.HasSavedSchoolPlayerPosition)
        {
            player.position = GameManager.Instance.SavedSchoolPlayerPosition;
            return;
        }

        SchoolEntryType entryType = GameManager.Instance.CurrentSchoolEntry;

        foreach (var point in spawnPoints)
        {
            if (point == null) continue;

            if (point.entryType == entryType)
            {
                player.position = point.transform.position;
                return;
            }
        }

        Debug.LogWarning("해당 entryType에 맞는 스폰 위치를 찾지 못했습니다.");
    }
}