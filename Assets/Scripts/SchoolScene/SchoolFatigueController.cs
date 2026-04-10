using UnityEngine;

public class SchoolFatigueController : MonoBehaviour
{
    [SerializeField] private float drainInterval = 8f;
    [SerializeField] private int drainAmount = 1;

    private float timer;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null)
            return;

        timer += Time.deltaTime;

        if (timer >= drainInterval)
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