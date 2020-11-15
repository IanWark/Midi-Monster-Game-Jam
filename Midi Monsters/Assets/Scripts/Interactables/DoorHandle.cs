using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayHandle : Interactable
{
    private Door m_Door;

    // Start is called before the first frame update
    void Start()
    {
        m_Door = transform.parent.gameObject.GetComponent<Door>();
    }



}
