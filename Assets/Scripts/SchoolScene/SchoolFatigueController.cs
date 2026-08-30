using UnityEngine;

public class SchoolFatigueController : MonoBehaviour
{
    [SerializeField] private float baseDrainInterval = 8f;
    [SerializeField] private int drainAmount = 5;

    private float timer;
    private bool isLeavingSchool = false;

    private void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.CurrentPlayer == null)
            return;

        if (isLeavingSchool)
            return;

        if (ShouldPauseDrain())
            return;

        float drainMultiplier =
            GameManager.Instance.GetSchoolHealthDrainMultiplier();

        drainMultiplier = Mathf.Max(0.01f, drainMultiplier);

        float currentDrainInterval =
            baseDrainInterval / drainMultiplier;

        timer += Time.deltaTime;

        if (timer >= currentDrainInterval)
        {
            timer -= currentDrainInterval;

            GameManager.Instance.AddCurrentHealth(-drainAmount);

            if (GameManager.Instance.CurrentPlayer.currentHealth <= 0)
            {
                isLeavingSchool = true;
                GameManager.Instance.GoToEvening();
            }
        }
    }

    private bool ShouldPauseDrain()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return true;

        if (Time.timeScale <= 0f)
            return true;

        // NPC 대화 + 등록된 팝업
        if (gm.IsSchoolHealthDrainPaused)
            return true;

        // 편의점
        if (ConvenienceStoreUI.Instance != null &&
            ConvenienceStoreUI.Instance.IsOpen)
            return true;

        return false;
    }
}