using UnityEngine;

public class FestivalFoodTruck : MonoBehaviour
{
    [SerializeField] private FestivalFoodType foodType;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside;

    private void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(interactKey))
            Interact();
    }

    private void Interact()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        if (!gm.festivalDateStarted ||
            gm.festivalDateFinished)
            return;

        if (gm.festivalDatePhase != FestivalDatePhase.Food)
            return;

        // 잘못된 푸드트럭
        if (foodType != gm.targetFestivalFoodType)
        {
            FestivalDialogueController.Instance?.
ShowWrongFoodDialogue(foodType);

            return;
        }

        // 정답 푸드트럭
        string targetFood =
            gm.GetFestivalFoodName(foodType);

        FestivalDialogueController.Instance?.
    ShowCorrectFoodDialogue(foodType);

        gm.StartFestivalConcertPhase();
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