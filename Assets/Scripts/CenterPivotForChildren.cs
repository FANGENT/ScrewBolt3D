using UnityEngine;

public class CenterPivotForChildren : MonoBehaviour
{
    void Start()
    {
        // Get all renderers in children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        // Calculate combined bounds
        Bounds combinedBounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            combinedBounds.Encapsulate(rend.bounds);
        }

        // Find the center in world space
        Vector3 worldCenter = combinedBounds.center;

        // Convert to local space relative to this object
        Vector3 localCenter = transform.InverseTransformPoint(worldCenter);

        // Shift all children so their center matches parent pivot
        foreach (Transform child in transform)
        {
            child.localPosition -= localCenter;
        }

        // Move parent to keep world position unchanged
        transform.position = worldCenter;
    }
}
