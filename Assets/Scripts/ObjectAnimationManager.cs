using UnityEngine;
using System.Collections;

public class ObjectAnimationManager : MonoBehaviour
{
    [SerializeField] GameObject[] movableObjects;
    // TODO use ENUM
    [SerializeField] private int blockAnimation; // 0 = back/forth, 1 = up/down, 2 = rotateX, 3 = rotateY, 4 = fall
    private bool canTrigger = true;
    private float cooldownDuration = 1f;

    private IEnumerator StartCooldown()
    {
        canTrigger = false;
        yield return new WaitForSeconds(cooldownDuration);
        canTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (canTrigger)
        {
            if (other.gameObject.tag == "Player")
            {
                foreach (GameObject obj in movableObjects)
                {
                    if (blockAnimation == 0)
                    {
                        Debug.Log("Remove isStatic from gameObject: " + obj.name + " if not working");
                        obj.AddComponent<MoveBackAndForth>();
                    }
                    else if (blockAnimation == 1)
                    {
                        Debug.Log("Remove isStatic from gameObject: " + obj.name + " if not working");
                        obj.AddComponent<MoveUpAndDown>();
                    }
                    else if (blockAnimation == 2)
                    {
                        Debug.Log("Remove isStatic from gameObject: " + obj.name + " if not working");
                        obj.AddComponent<RotateObjectOverX>();
                    }
                    else if (blockAnimation == 3)
                    {
                        Debug.Log("Remove isStatic from gameObject: " + obj.name + " if not working");
                        obj.AddComponent<RotateObjectOverY>();
                    }
                    else
                    {
                        Debug.Log("Remove isStatic from gameObject: " + obj.name + " if not working");
                        obj.AddComponent<Rigidbody>();
                    }
                }
            }
            StartCoroutine(StartCooldown());
        }
    }

    void OnTriggerExit(Collider other)
    {
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
