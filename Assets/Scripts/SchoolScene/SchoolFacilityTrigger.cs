using UnityEngine;

public class SchoolFacilityTrigger : MonoBehaviour
{
    [SerializeField] private string facilityName = "학교 시설";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside = false;

    private void Update()
    {
        if (!playerInside) return;
        if (GameManager.Instance == null) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (!GameManager.Instance.IsAllClassesFinished)
            {
                Debug.Log("수업을 모두 마친 뒤에 이용할 수 있습니다.");
                return;
            }

            if (GameManager.Instance.usedSchoolFacilityToday)
            {
                Debug.Log("오늘은 이미 운동장/웰니스센터를 이용했습니다.");
                return;
            }

            if (GameManager.Instance.CurrentPlayer == null ||
                GameManager.Instance.CurrentPlayer.currentHealth < 10)
            {
                Debug.Log("현재체력이 10 이상이어야 이용할 수 있습니다.");
                return;
            }

            bool success = GameManager.Instance.TryUseWorkoutFacility();

            if (success)
            {
                Debug.Log($"{facilityName} 이용 완료! 현재체력 -10 / 최대체력 +2 / 외모 +1");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
    }
}