<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestEmitter : MonoBehaviour
{
    public Emitter emitter;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            emitter.PlaySound();
        }
    } 
}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestEmitter : MonoBehaviour
{
    public Emitter emitter;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            emitter.PlaySound();
        }
    } 
}
>>>>>>> 89d8615... Build fix
