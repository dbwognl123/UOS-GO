using UnityEngine;

public class ComputerTrigger : MonoBehaviour
{
    [SerializeField] private GameObject scheduleUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        scheduleUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        scheduleUI.SetActive(false);
    }
}