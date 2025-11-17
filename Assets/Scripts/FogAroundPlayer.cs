using UnityEngine;

public class FogAroundPlayer : MonoBehaviour
{
    [SerializeField] Transform playerGameObject;
    const float rotationSpeed = 6f;

    void Start()
    {
        // TODO disable this when on BaseLevel
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, playerGameObject.position, 2.5f * Time.deltaTime);
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
