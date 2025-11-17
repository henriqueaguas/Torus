using UnityEngine;

public class RotateObjectOverY : MonoBehaviour
{
    private Vector3 rotationSpeed = new Vector3(0, 50, 0);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
