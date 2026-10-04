using UnityEngine;

public class SchoolFacilityTrigger : MonoBehaviour
{
    [SerializeField] private string facilityName = "학교 시설";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Player Portrait")]
    [SerializeField] private Sprite playerHappyPortrait;
    [SerializeField] private Sprite playerTiredPortrait;

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

        if (gm == null)
            return;

        // =========================
        // 운동 불가능
        // =========================
        if (!gm.CanUseWorkoutFacility())
        {
            string reason =
                gm.GetWorkoutUnavailableReason();

            SchoolNPCUI.Instance?.OpenSimpleDialogue(
                "나",
                playerTiredPortrait,
                reason
            );

            Debug.Log(
                $"[{facilityName}] 이용 실패 / {reason}"
            );

            return;
        }

        // =========================
        // 운동 성공
        // =========================
        if (gm.TryUseWorkoutFacility())
        {
            SchoolNPCUI.Instance?.OpenSimpleDialogue(
                "나",
                playerHappyPortrait,
                "운동하고나니까 뿌듯하네"
            );

            Debug.Log(
                $"{facilityName} 이용 완료! " +
                $"돈 -10 / 현재체력 -10 / " +
                $"최대체력 +10 / 외모 +3"
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        Debug.Log(
            $"{facilityName}: E키를 눌러 이용"
        );
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
    }
}