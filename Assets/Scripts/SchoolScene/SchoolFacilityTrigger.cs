using UnityEngine;

public class SchoolFacilityTrigger : MonoBehaviour
{
    [SerializeField] private string facilityName = "학교 시설";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside = false;

    private void Update()
    {
        if (!playerInside)
            return;

        if (GameManager.Instance == null)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            TryUseFacility();
        }
    }

    private void TryUseFacility()
    {
        GameManager gm = GameManager.Instance;

        if (!gm.CanUseWorkoutFacility())
        {
            Debug.Log(gm.GetWorkoutUnavailableReason());
            return;
        }

        if (gm.TryUseWorkoutFacility())
        {
            Debug.Log(
                $"{facilityName} 이용 완료! " +
                $"돈 -10 / 최대체력 +2 / 외모 +1"
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        Debug.Log($"{facilityName}: E키를 눌러 이용");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
    }
}