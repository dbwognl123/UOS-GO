using System.Collections;
using UnityEngine;

public class FestivalStageInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float performanceDuration = 5f;

    private bool playerInside;
    private bool watchingPerformance;

    private void Update()
    {
        if (!playerInside)
            return;

        if (watchingPerformance)
            return;

        if (Input.GetKeyDown(interactKey))
            TryWatchPerformance();
    }

    private void TryWatchPerformance()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        if (!gm.festivalDateStarted ||
            gm.festivalDateFinished)
            return;

        if (gm.festivalDatePhase != FestivalDatePhase.Concert)
        {
            Debug.Log("아직 공연을 볼 때가 아닌 것 같다.");
            return;
        }

        StartCoroutine(WatchPerformance());
    }

    private IEnumerator WatchPerformance()
    {
        watchingPerformance = true;

        Debug.Log("[Festival] 공연 관람 시작");

        yield return new WaitForSeconds(5f);

        GameManager gm = GameManager.Instance;

        if (gm == null ||
            gm.festivalDateFinished)
        {
            watchingPerformance = false;
            yield break;
        }

        if (FestivalDialogueController.Instance != null)
        {
            FestivalDialogueController.Instance.
                ShowConcertSuccessDialogue(
                    () =>
                    {
                        gm.CompleteFestivalDate();
                        watchingPerformance = false;
                    }
                );
        }
        else
        {
            gm.CompleteFestivalDate();
            watchingPerformance = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
    }
}