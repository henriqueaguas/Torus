using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    public float wobbleAmount = 0.1f;
    public float wobbleSpeed = 1f;

    private Vector3 originalPosition;

    void Start()
    {
        // Store the original position of the camera
        originalPosition = transform.position;
    }

    void Update()
    {
        // Calculate the wobble offset based on sine wave motion
        float wobbleOffsetX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        float wobbleOffsetY = Mathf.Sin(Time.time * wobbleSpeed * 1.5f) * wobbleAmount;
        float wobbleOffsetZ = Mathf.Sin(Time.time * wobbleSpeed * 0.5f) * wobbleAmount;

        // Apply the wobble offset to the camera's position
        Vector3 wobbleOffset = new(wobbleOffsetX, wobbleOffsetY, wobbleOffsetZ);
        transform.position = originalPosition + wobbleOffset;
    }
}
