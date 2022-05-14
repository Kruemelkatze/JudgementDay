using System;
using UnityEditor;
using UnityEngine;

namespace Helpers
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DynamicPlane : MonoBehaviour
    {
        [SerializeField] protected internal Mesh mesh;

        private void Awake()
        {
            CreateInitialQuad();
        }

        protected internal void CreateInitialQuad(bool force = false)
        {
            var mf = GetComponent<MeshFilter>();

            if (mesh && !force)
            {
                if (mf.sharedMesh != mesh)
                {
                    mf.sharedMesh = mesh;
                }

                return;
            }

            mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
                new(-0.5f, -0.5f, 0),
                new(0.5f, -0.5f, 0),
                new(-0.5f, 0.5f, 0),
                new(0.5f, 0.5f, 0),
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.uv = uv;

            mf.sharedMesh = mesh;
        }
    }

#if UNITY_EDITOR
    // A tiny custom editor for ExampleScript component
    [CustomEditor(typeof(DynamicPlane))]
    public class ExampleEditor : Editor
    {
        // Custom in-scene UI for when ExampleScript
        // component is selected.
        public void OnSceneGUI()
        {
            if (target is not DynamicPlane t)
                return;

            var dirty = false;
            var vertices = t.mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                var v = vertices[i];
                var vw = t.transform.TransformPoint(v);
                var vwEdited = Handles.PositionHandle(vw, Quaternion.identity);

                if (vwEdited != vw)
                {
                    vertices[i] = t.transform.InverseTransformPoint(vwEdited);
                    dirty = true;
                }
            }

            if (dirty)
            {
                t.mesh.vertices = vertices;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (target is not DynamicPlane t)
                return;
            
            if (GUILayout.Button("Reset"))
            {
                t.CreateInitialQuad(true);
            }
        }
    }
#endif
}