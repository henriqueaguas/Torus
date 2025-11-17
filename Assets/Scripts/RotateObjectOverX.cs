using UnityEngine;

public class RotateObjectOverX : MonoBehaviour
{
    private Vector3 rotationSpeed = new Vector3(50, 0, 0);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
