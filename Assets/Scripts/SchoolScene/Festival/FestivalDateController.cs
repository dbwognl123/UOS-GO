using UnityEngine;

public class FestivalDateController : MonoBehaviour
{
    [SerializeField] private int lowHealthThreshold = 10;
    private bool lowHealthEndingStarted;
    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null ||
            gm.CurrentPlayer == null)
            return;

        if (!gm.festivalDateStarted)
            return;

        if (gm.festivalDateFinished)
            return;

        if (gm.CurrentPlayer.currentHealth <=
            lowHealthThreshold)
        {
            EndDateByLowHealth();
        }
    }

    private void EndDateByLowHealth()
    {
        if (lowHealthEndingStarted)
            return;

        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        if (gm.festivalDateFinished)
            return;

        lowHealthEndingStarted = true;

        if (FestivalDialogueController.Instance != null)
        {
            FestivalDialogueController.Instance.
                ShowLowHealthEndDialogue(
                    () =>
                    {
                        gm.EndFestivalDateByLowHealth();
                    }
                );
        }
        else
        {
            gm.EndFestivalDateByLowHealth();
        }
    }
}