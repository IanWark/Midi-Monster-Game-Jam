using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField, Tooltip("Text that appears when mousing over interactable.")]
    private string interactionPrompt = "Interact";
    public string InteractionPrompt { get { return interactionPrompt; } }

    private bool canInteract = true;

    public bool IsInteractable()
    {
        return canInteract;
    }

    public void Interact()
    {
        Debug.Log("I have been interacted with.");
    }
}
