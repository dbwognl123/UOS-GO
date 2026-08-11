using UnityEngine;

public class WorkoutInteraction : MonoBehaviour
{
    private bool playerInside;

    private void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryWorkout();
        }
    }

    private void TryWorkout()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        if (!gm.CanUseWorkoutFacility())
        {
            Debug.Log(gm.GetWorkoutUnavailableReason());
            return;
        }

        bool success = gm.TryUseWorkoutFacility();

        if (success)
        {
            Debug.Log(
                $"운동 완료! " +
                $"돈={gm.CurrentPlayer.money}, " +
                $"최대체력={gm.CurrentPlayer.maxHealth}, " +
                $"외모={gm.CurrentPlayer.appearance}"
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        Debug.Log("E키를 눌러 운동할 수 있습니다.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
    }
}