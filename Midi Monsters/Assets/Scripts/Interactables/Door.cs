using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable, Lockable
{
    [SerializeField]
    private string isOpenInteractionPrompt = "Close";
    [SerializeField]
    private string isClosedInteractionPrompt = "Open";
    [SerializeField]
    private string isLockedInteractionPrompt = "Locked";
    [SerializeField]
    private bool isLocked = false;

    [SerializeField]
    public Transform doorVisual;
    [SerializeField]
    public DoorHandle doorHandle;

    private PlayerCharacterController m_PlayerController; // need for keys
    // The target marker.
    //[SerializeField, Tooltip("Transform for open position")]
    // public Transform m_OpenTransform;

    // Angular speed in radians per sec.
    public float speed = 100f;

    [SerializeField, Tooltip("Door starts open.")]
    public bool m_Open = false;

    private bool m_Opening = false;
    private bool m_Closing = false;

    List<DoorframeTrigger> doorframeTriggers = new List<DoorframeTrigger>();

    //private Vector3 openRotation;
    //private Vector3 closeRotation;
    //private Vector3 close;
    private Quaternion m_ClosedPosition, m_OpenPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_ClosedPosition = doorVisual.rotation;
        m_OpenPosition = m_ClosedPosition * Quaternion.Euler(0, 0, -90);

        doorHandle.SetLock(isLocked);
        Open(m_Open);
        SetInteractionPrompt();
    }

    public void RegisterDoorframeTrigger(DoorframeTrigger trigger)
    {
        doorframeTriggers.Add(trigger);
    }

    // Used https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Vector3.RotateTowards.html
    void Update()
    {
        float singleStep = speed * Time.deltaTime;
        Quaternion destination = m_Open ? m_OpenPosition : m_ClosedPosition;

        doorVisual.rotation = Quaternion.RotateTowards(doorVisual.rotation, destination, speed);
    }

    public bool IsMoving()
    {
        return m_Opening || m_Closing;
    }

    public override void Interact(PlayerCharacterController pc)
    {
        m_PlayerController = pc;
        if (!IsLocked()) { 
            Open(!m_Open);
        }
        else
        {
            Unlock();
        }
        
    }

    // Attempt to set m_open to the bool
    public void Open(bool isOpen)
    {
        if (!IsLocked())
        {
            m_Open = isOpen;
            SetInteractionPrompt();
        }
    }

    private void SetInteractionPrompt()
    {
        if (!IsLocked())
        {
            if (m_Open)
            {
                interactionPrompt = isOpenInteractionPrompt;
            }
            else
            {
                interactionPrompt = isClosedInteractionPrompt;
            }
        }
        else
        {
            interactionPrompt = isLockedInteractionPrompt;
        }

        foreach(DoorframeTrigger trigger in doorframeTriggers)
        {
            trigger.SetInteractionPrompt(interactionPrompt);
        }
    }

    // Does it open when interacted with?
    public bool IsLocked()

    {
        return isLocked;
    }

    // Does it show interaction prompt?
    public override bool IsInteractable()
    {
        return true;
    }

    public bool Unlock()
    {
        if (isLocked && m_PlayerController.HasKey())
        {
            m_PlayerController.UseKey();
            isLocked = false;
            SetInteractionPrompt();
            doorHandle.SetLock(isLocked);
            return true;
        }
        return false;
    }
}
