using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{

    // The target marker.
    [SerializeField, Tooltip("Transform for open position")]
    public Transform m_OpenTransform;

    // Angular speed in radians per sec.
    public float speed = 1.0f;

    [SerializeField, Tooltip("Door starts open.")]
    public bool m_Open = false;

    private bool m_Opening = false;
    private bool m_Closing = false;


    private Vector3 m_InitialPosition, m_OpenPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_InitialPosition = transform.position;
    }

    // Used https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Vector3.RotateTowards.html
    void Update()
    {
        float singleStep = speed * Time.deltaTime;
        Vector3 newDirection = Vector3.zero;
        if (m_Opening)
        {
            newDirection = Vector3.RotateTowards(transform.forward, m_OpenPosition, singleStep, 0.0f);
        }
        if (m_Closing)
        {
            newDirection = Vector3.RotateTowards(transform.forward * (-1), m_InitialPosition, singleStep, 0.0f);
        }

        if (newDirection.magnitude < 0.001)
        {
            m_Opening = m_Closing = false;
            m_Open = !m_Open;
            this.enabled = false; // stop updating
        }

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        if (IsMoving()) { 
        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    public void toggle()
    {
        if (m_Open)
        {
            m_Opening = false;
            m_Closing = true;

        }
        else
        {
            m_Opening = true;
            m_Closing = false;
        }
        this.enabled = true;
    }

    public bool IsMoving()
    {
        return m_Opening || m_Closing;
    }

    public override void Interact()
    {
        toggle();
    }

    public override bool IsInteractable()
    {
        return !IsMoving();
    }
}
