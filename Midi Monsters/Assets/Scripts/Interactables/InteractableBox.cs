using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBox : Interactable
{
    public override bool IsInteractable()
    {
        return true;
    }

    public override void Interact(PlayerCharacterController pc)
    {
        Debug.Log("Box has been interacted with.");
    }
}
