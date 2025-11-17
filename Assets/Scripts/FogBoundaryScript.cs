using UnityEngine;

public class FogBoundaryScript : MonoBehaviour
{
    public SphereCollider sphereCollider;
    public Transform playerSpawner;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.gameObject.transform.position = playerSpawner.position;
    }
}
