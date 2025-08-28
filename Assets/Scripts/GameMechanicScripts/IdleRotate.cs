using UnityEngine;

public class IdleRotate : MonoBehaviour
{
    public float idleTimeThreshold = 3f;       // Time in seconds before rotation starts
    public float rotationSpeed = 20f;          // Degrees per second
    private float idleTimer = 0f;              // Tracks how long the user hasn't touched
    private bool isRotating = false;

    void Update()
    {
        // Check for touch or mouse input (to support editor testing)
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            idleTimer = 0f;
            isRotating = false;
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeThreshold)
            {
                isRotating = true;
            }
        }

        // Apply rotation if idle
        if (isRotating)
        {
            ModelController.Instance.Model.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}