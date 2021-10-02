using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GalaxyBrainEditor.Shortcuts
{
    public class MarkMeshCollidersConvex
    {
        [MenuItem("Tools/Make Mesh Colliders Convex")]
        public static void MakeAllMeshCollidersConvex()
        {
            MeshCollider[] colliders = GameObject.FindObjectsOfType<MeshCollider>();

            foreach(MeshCollider collider in colliders)
            {
                if (!collider.convex)
                {
                    Undo.RecordObject(collider, $"Made {collider.gameObject}'s mesh collider convex.");
                    collider.convex = true;
                }
            }

            Debug.Log($"There are now {colliders.Length} convex Mesh Colliders in the scene.");
        }
    }
}
