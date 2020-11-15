using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField, Tooltip("Text that appears when mousing over interactable.")]
    protected string interactionPrompt = "Interact";
    public abstract string GetInteractionPrompt();

    public abstract bool IsInteractable();

    public abstract void Interact(PlayerCharacterController pc);
}
