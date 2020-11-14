using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField, Tooltip("Text that appears when mousing over interactable.")]
    private string interactionPrompt = "Interact";
    public string InteractionPrompt { get { return interactionPrompt; } }

    public abstract bool IsInteractable();

    public abstract void Interact();
}
