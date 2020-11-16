using UnityEngine;

public class Key : Interactable
{
    public enum KeyType
    {
        Normal,
        Red,
    }

    [SerializeField]
    private KeyType keyType = KeyType.Normal;

    public override string GetInteractionPrompt() { return interactionPrompt; }

    public override void Interact(PlayerCharacterController pc)
    {
        pc.AddKey(keyType);
        Destroy(this.gameObject);
    }

    public override bool IsInteractable()
    {
        return true;
    }
}