using UnityEngine;

public class PlayerMessagePopupSpawner : MonoBehaviour
{
    [SerializeField] private WorldTextPopup popupPrefab;
    [SerializeField] private Transform popupSpawnPoint;
    [SerializeField] private Color attendanceColor = Color.white;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnSchoolMessagePopupRequested += HandleMessageRequested;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnSchoolMessagePopupRequested -= HandleMessageRequested;
    }

    private void HandleMessageRequested(string message)
    {
        if (popupPrefab == null || popupSpawnPoint == null) return;

        WorldTextPopup popup = Instantiate(
            popupPrefab,
            popupSpawnPoint.position,
            Quaternion.identity
        );

        popup.Init(message, attendanceColor);
    }
}