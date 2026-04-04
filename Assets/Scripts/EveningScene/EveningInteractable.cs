using UnityEngine;

public class EveningInteractable : MonoBehaviour
{
    [SerializeField] private EveningInteractionType interactionType;

    public EveningInteractionType InteractionType => interactionType;
}