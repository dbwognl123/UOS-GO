using UnityEngine;

public class SchoolFatigueController : MonoBehaviour
{
    [SerializeField] private float baseDrainInterval = 8f;
    [SerializeField] private int drainAmount = 5;

    private float timer;

    private void Update()
    {
        if (ConvenienceStoreUI.Instance != null && ConvenienceStoreUI.Instance.IsOpen)
            return;
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null)
            return;

        float drainMultiplier = GameManager.Instance.GetSchoolHealthDrainMultiplier();

        // 에너지드링크면 0.5배 속도 = 간격은 2배
        float currentDrainInterval = baseDrainInterval / drainMultiplier;

        timer += Time.deltaTime;

        if (timer >= currentDrainInterval)
        {
            timer = 0f;
            GameManager.Instance.AddCurrentHealth(-drainAmount);

            if (GameManager.Instance.CurrentPlayer.currentHealth <= 0)
            {
                GameManager.Instance.GoToEvening();
            }
        }
    }
}