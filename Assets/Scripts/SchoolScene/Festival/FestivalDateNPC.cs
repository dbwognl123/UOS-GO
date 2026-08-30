using UnityEngine;

public class FestivalDateNPC : MonoBehaviour
{
    private bool playerInRange;
    private bool started;

    private void Update()
    {
        if (!playerInRange)
            return;

        if (started)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager gm = GameManager.Instance;

            if (gm == null)
                return;

            started = true;

            gm.StartFestivalDate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}