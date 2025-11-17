using UnityEngine;

public class MoveBackAndForth : MonoBehaviour
{
    public float speed = 2.0f; // Speed of movement
    public float distance = 10.0f; // Maximum distance to move from the starting point
    private Vector3 startPosition;

    void Start()
    {
        // Record the starting position
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new position
        float pingPong = Mathf.PingPong(Time.time * speed, distance);
        transform.position = startPosition + new Vector3(pingPong - distance / 2f, 0, 0);// Move along the x-axis
    }
}
