using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{

    // The target marker.
    //[SerializeField, Tooltip("Transform for open position")]
    // public Transform m_OpenTransform;

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
        m_ClosedPosition = transform.rotation;
        m_OpenPosition = Quaternion.Euler(-90, -90, 0);
    }

    // Used https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Vector3.RotateTowards.html
    void Update()
    {
        if (IsMoving())
        {
            float singleStep = speed * Time.deltaTime;
            Quaternion newDirection = Quaternion.identity;


            if (m_Opening)
            {
                newDirection = Quaternion.RotateTowards(transform.rotation, m_OpenPosition, speed);

            }
            else if (m_Closing)
            {
                newDirection = Quaternion.RotateTowards(transform.rotation, m_ClosedPosition, speed);
            }

            if (newDirection.eulerAngles.magnitude < 0.01)
            {
                m_Open = !m_Open;
                m_Opening = false;
                m_Closing = false;
            }
            // Draw a ray pointing at our target in
            // Debug.DrawRay(transform.position, newDirection, Color.red);
            Debug.Log("Door moving.");

            transform.rotation = newDirection;
        }
    }

    public bool IsMoving()
    {
        return m_Opening || m_Closing;
    }

    public override void Interact()
    {
        Debug.Log("Door Toggle.");
        if (m_Open)
        {
            m_Closing = true;

        }
        else
        {
            m_Opening = true;
        }
        //this.enabled = true;
    }

    public override bool IsInteractable()
    {
        return !IsMoving();
    }
}
