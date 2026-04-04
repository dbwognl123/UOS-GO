using UnityEngine;

public class ExitPadTrigger : MonoBehaviour
{
    [SerializeField] private GameObject gateChoiceUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (gateChoiceUI == null) return;

        gateChoiceUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (gateChoiceUI == null) return;

        gateChoiceUI.SetActive(false);
    }
}