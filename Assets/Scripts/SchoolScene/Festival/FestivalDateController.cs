using UnityEngine;

public class FestivalDateController : MonoBehaviour
{
    public static FestivalDateController Instance
    {
        get;
        private set;
    }

    [Header("Health")]
    [SerializeField] private int lowHealthThreshold = 10;

    private void Awake()
    {
        Instance = this;
    }

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

        if (gm.festivalDateEnding)
            return;

        if (gm.CurrentPlayer.currentHealth <=
            lowHealthThreshold)
        {
            TryStartLowHealthEnding();
        }
    }

    public void TryStartLowHealthEnding()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        if (!gm.festivalDateStarted)
            return;

        if (gm.festivalDateFinished)
            return;

        if (gm.festivalDateEnding)
            return;

        // 공연 성공 등의 다른 종료가 들어오지 못하게 잠금
        gm.festivalDateEnding = true;

        Debug.Log(
            "[Festival] 체력 부족 종료 대사 시작"
        );

        if (FestivalDialogueController.Instance != null)
        {
            FestivalDialogueController.Instance.
                ShowLowHealthEndDialogue(
                    () =>
                    {
                        gm.EndFestivalDateByLowHealth();
                        gm.GoToEveningSceneAfterFestival();
                    }
                );
        }
        else
        {
            gm.EndFestivalDateByLowHealth();
            gm.GoToEveningSceneAfterFestival();
        }
    }
}