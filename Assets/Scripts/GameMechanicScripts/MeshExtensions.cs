using UnityEngine;

public static class MeshExtensions
{
    public static float SurfaceArea(this Mesh mesh)
    {
        var vertices = mesh.vertices;
        var tris = mesh.triangles;
        float total = 0f;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 a = vertices[tris[i]];
            Vector3 b = vertices[tris[i + 1]];
            Vector3 c = vertices[tris[i + 2]];
            total += Vector3.Cross(b - a, c - a).magnitude * 0.5f;
        }
        return total;
    }

    public static Vector3 Centroid(this Mesh mesh)
    {
        var vertices = mesh.vertices;
        var tris = mesh.triangles;
        Vector3 centroidSum = Vector3.zero;
        float areaSum = 0f;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 a = vertices[tris[i]];
            Vector3 b = vertices[tris[i + 1]];
            Vector3 c = vertices[tris[i + 2]];

            float area = Vector3.Cross(b - a, c - a).magnitude * 0.5f;
            Vector3 triCentroid = (a + b + c) / 3f;

            centroidSum += triCentroid * area;
            areaSum += area;
        }

        return areaSum > 0f ? centroidSum / areaSum : Vector3.zero;
    }
}