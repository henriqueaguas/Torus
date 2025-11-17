using System;
using System.Collections;
using UnityEngine;

// Fog teleports you if you try to exist through the bridge
public class FogPortalTeleportScript : MonoBehaviour
{
    //boxColliders that the player exist
    public BoxCollider exitBoxCollider;

    //coordinates to teleport the player to
    private Vector3 TeleportVector;

    public float tpOffset;

    private bool canTrigger = true;
    public float cooldownDuration = 1f;

    private IEnumerator StartCooldown()
    {
        canTrigger = false;
        yield return new WaitForSeconds(cooldownDuration);
        canTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canTrigger)
        {
            //if the game object entering is the player
            if (other.gameObject.CompareTag("Vehicle") || other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(StartCooldown());

                if (other.gameObject.CompareTag("Vehicle"))
                {
                    var rigidbody = other.GetComponentInParent<Rigidbody>();
                    if (rigidbody == null)
                    {
                        Debug.LogError("OnTriggerEnter(): Could not find Rigidbody in other nor its parent!");
                    }
                    else
                    {
                        rigidbody.MovePosition(rigidbody.position + tpOffset * exitBoxCollider.transform.position - transform.position);
                    }
                }

                if (other.gameObject.CompareTag("Player"))
                {
                    other.transform.position = other.transform.position + tpOffset * exitBoxCollider.transform.position - transform.position;
                }
            }
        }
    }
}

