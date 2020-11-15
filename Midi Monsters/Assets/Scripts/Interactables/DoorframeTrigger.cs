using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorframeTrigger : Interactable
{
    [SerializeField]
    private Door parentDoor = null;

    public override string GetInteractionPrompt() { return parentDoor.GetInteractionPrompt(); }

    public override bool IsInteractable()
    {
        return parentDoor.IsInteractable();
    }

    public override void Interact(PlayerCharacterController pc)
    {
        parentDoor.Interact(pc);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) // magic number :(
        {
            parentDoor.Open(true);
        }
    }
}
