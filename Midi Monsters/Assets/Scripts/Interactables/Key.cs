public class Key : Interactable
{
    public override string GetInteractionPrompt() { return interactionPrompt; }

    public override void Interact(PlayerCharacterController pc)
    {
        pc.AddKey();
        Destroy(this.gameObject);
    }

    public override bool IsInteractable()
    {
        return true;
    }
}