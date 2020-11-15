using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : MonoBehaviour
{   
    [SerializeField]
    private Material normalMat = null;
    [SerializeField]
    private Material lockedMat = null;

    private MeshRenderer meshRenderer = null;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetLock(bool isLocked)
    {
        meshRenderer.material = isLocked ? lockedMat : normalMat; 
    }
}
