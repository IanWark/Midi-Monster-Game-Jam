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

    [MenuItem("TOM/Fix light instances")]
    public static void ReplaceLightsInstances()
    {
        int prefabCount = 0;
        int looseCount = 0;
        var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in roots)
        {
            var allMeshes = root.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer mesh in allMeshes)
            {
                if (mesh.name.StartsWith("Wall Light"))
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(mesh))
                    {
                        prefabCount++;
                    }
                    else
                    {
                        var prefab = AssetDatabase.LoadAssetAtPath<MeshRenderer>("Assets/Prefabs/Level/Studio Objects/Wall Light.prefab");
                        MeshRenderer replacement = (MeshRenderer)PrefabUtility.InstantiatePrefab(prefab, mesh.transform.parent);
                        replacement.name = "ReplacementLight";
                        replacement.transform.SetPositionAndRotation(mesh.transform.position, mesh.transform.rotation);
                        replacement.transform.SetSiblingIndex(mesh.transform.GetSiblingIndex());
                        looseCount++;
                    }
                }
            }
        }

        Debug.Log("Prefabs:" + prefabCount);
        Debug.Log("Loose:" + looseCount);
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
