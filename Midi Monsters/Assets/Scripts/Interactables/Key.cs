public class Key : Interactable
{
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