using UnityEngine;

public class EveningPlayerInteraction : MonoBehaviour
{
    [SerializeField] private EveningSceneUI eveningSceneUI;

    private EveningInteractable currentInteractable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        EveningInteractable interactable = other.GetComponent<EveningInteractable>();
        if (interactable == null) return;

        currentInteractable = interactable;

        if (eveningSceneUI != null)
            eveningSceneUI.OpenPopup(interactable.InteractionType);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        EveningInteractable interactable = other.GetComponent<EveningInteractable>();
        if (interactable == null) return;

        if (currentInteractable == interactable)
        {
            currentInteractable = null;

            if (eveningSceneUI != null)
                eveningSceneUI.ClosePopup();
        }
    }
}