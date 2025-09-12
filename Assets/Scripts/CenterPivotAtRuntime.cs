using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CenterPivotAtRuntime : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.mesh == null)
            return;

        Mesh mesh = meshFilter.mesh;
        Bounds bounds = mesh.bounds;

        // Calculate the offset from pivot to mesh center
        Vector3 offset = bounds.center;

        // Shift all vertices so mesh is centered at pivot
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= offset;
        }
        mesh.vertices = vertices;

        // Recalculate bounds/normals
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // Move the object so its world position remains the same
        transform.position += transform.rotation * Vector3.Scale(transform.localScale, offset);
    }
}
