using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField]
    public Transform doorVisual;

    // The target marker.
    //[SerializeField, Tooltip("Transform for open position")]
    // public Transform m_OpenTransform;

    // Angular speed in radians per sec.
    public float speed = 100f;

    [SerializeField, Tooltip("Door starts open.")]
    public bool m_Open = false;

    private bool m_Opening = false;
    private bool m_Closing = false;

    //private Vector3 openRotation;
    //private Vector3 closeRotation;
    //private Vector3 close;
    private Quaternion m_ClosedPosition, m_OpenPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_ClosedPosition = doorVisual.rotation;
        m_OpenPosition = m_ClosedPosition * Quaternion.Euler(0, 0, -90);
    }

    // Used https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Vector3.RotateTowards.html
    void Update()
    {
        float singleStep = speed * Time.deltaTime;
        Quaternion destination = m_Open ? m_OpenPosition : m_ClosedPosition;

        doorVisual.rotation = Quaternion.RotateTowards(doorVisual.rotation, destination, speed);

        //if (IsMoving())
        //{
        //    float singleStep = speed * Time.deltaTime;
        //    Quaternion newDirection = Quaternion.identity;


        //    if (m_Opening)
        //    {
        //        newDirection = Quaternion.RotateTowards(transform.rotation, m_OpenPosition, speed);

        //    }
        //    else if (m_Closing)
        //    {
        //        newDirection = Quaternion.RotateTowards(transform.rotation, m_ClosedPosition, speed);
        //    }

        //    if (transform.rotation != newDirection)
        //    {
        //        transform.rotation = newDirection;
        //        Debug.Log("Moving!");
        //    }
        //    else
        //    {
        //        Debug.Log("Door moving.");
        //        m_Open = !m_Open;
        //        m_Opening = false;
        //        m_Closing = false;
        //    }
        //}
    }

    public bool IsMoving()
    {
        return m_Opening || m_Closing;
    }

    public override void Interact()
    {
        //Debug.Log("Door Toggle.");
        //if (m_Open)
        //{
        //    m_Closing = true;

        //}
        //else
        //{
        //    m_Opening = true;
        //}
        ////this.enabled = true;
        ///
        m_Open = !m_Open;
    }

    public override bool IsInteractable()
    {
        return true;
    }
}
