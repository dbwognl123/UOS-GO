using UnityEngine;

public class ConvenienceStoreTrigger : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside = false;

    private void Update()
    {
        if (!playerInside) return;
        if (ConvenienceStoreUI.Instance == null) return;

        if (Input.GetKeyDown(interactKey) && !ConvenienceStoreUI.Instance.IsOpen)
        {
            ConvenienceStoreUI.Instance.OpenStore();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
    }
}