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

        GameManager gm = GameManager.Instance;

        if (gm == null)
        {
            watchingPerformance = false;
            yield break;
        }

        Debug.Log("[Festival] 공연 관람 시작");

        yield return new WaitForSeconds(performanceDuration);

        gm = GameManager.Instance;

        if (gm == null)
        {
            watchingPerformance = false;
            yield break;
        }

        // 그 사이 체력 부족 종료가 시작됐으면 공연 성공 처리 X
        if (gm.festivalDateFinished ||
            gm.festivalDateEnding)
        {
            watchingPerformance = false;
            yield break;
        }

        // 다른 종료 루트가 들어오지 못하도록 잠금
        gm.festivalDateEnding = true;

        if (FestivalDialogueController.Instance != null)
        {
            FestivalDialogueController.Instance.
                ShowConcertSuccessDialogue(
                    () =>
                    {
                        gm.CompleteFestivalDate();
                        gm.GoToEveningSceneAfterFestival();
                    }
                );
        }
        else
        {
            gm.CompleteFestivalDate();
            gm.GoToEveningSceneAfterFestival();
        }

        watchingPerformance = false;
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