using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : Interactable, Lockable
{
    [SerializeField]
    private string isOpenInteractionPrompt = "Close";
    [SerializeField]
    private string isClosedInteractionPrompt = "Open";
    [SerializeField]
    private string isLockedInteractionPrompt = "Locked. You need a key!";
    [SerializeField]
    private string unlockInteractionPrompt = "Unlock";
    [SerializeField]
    private bool isLocked = false;
    [SerializeField]
    private Key.KeyType keyType = Key.KeyType.Normal;

    [SerializeField]
    private Transform doorVisual;
    private DoorHandle doorHandle;
    private MonsterManager monsterManager;
    private NavMeshObstacle navObstacle;

    private PlayerCharacterController m_PlayerController; // need for keys

    // Angular speed in radians per sec.
    public float speed = 100f;

    [SerializeField, Tooltip("Door starts open.")]
    public bool m_Open = false;

    private bool m_Opening = false;
    private bool m_Closing = false;
    
    private Quaternion m_ClosedPosition, m_OpenPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_ClosedPosition = doorVisual.rotation;
        m_OpenPosition = m_ClosedPosition * Quaternion.Euler(0, 0, -90);

        m_PlayerController = FindObjectOfType<PlayerCharacterController>();
        monsterManager = FindObjectOfType<MonsterManager>();
        doorHandle = GetComponentInChildren<DoorHandle>();
        navObstacle = GetComponent<NavMeshObstacle>();

        doorHandle.SetLock(isLocked);
        navObstacle.enabled = isLocked;
        Open(m_Open);
    }

    // Used https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Vector3.RotateTowards.html
    void Update()
    {
        float singleStep = 90 * speed * Time.deltaTime;
        Quaternion destination = m_Open ? m_OpenPosition : m_ClosedPosition;

        doorVisual.rotation = Quaternion.RotateTowards(doorVisual.rotation, destination, singleStep);
    }

    public bool IsMoving()
    {
        return m_Opening || m_Closing;
    }

    public override void Interact(PlayerCharacterController pc)
    {
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
        }
    }

    // Does it not open when interacted with?
    public bool IsLocked()
    {
        return isLocked;
    }

    // Does it show interaction prompt?
    public override bool IsInteractable()
    {
        return true;
    }

    public override string GetInteractionPrompt()
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
            
            if (m_PlayerController != null)
            {
                if (m_PlayerController.HasKey(keyType))
                {
                    interactionPrompt = unlockInteractionPrompt;
                }
            }
        }
        
        return interactionPrompt;
    }

    public bool Unlock()
    {
        if (isLocked && m_PlayerController.HasKey(keyType))
        {
            m_PlayerController.UseKey(keyType);
            isLocked = false;
            navObstacle.enabled = isLocked;
            doorHandle.SetLock(isLocked);
            monsterManager.LevelUpMonster();
            return true;
        }
        return false;
    }
}
