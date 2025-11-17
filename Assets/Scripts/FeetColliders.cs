using System.Collections.Generic;
using UnityEngine;

public class FeetColliders : MonoBehaviour
{
    private HashSet<Collider> collidingObjects = new HashSet<Collider>();

    public bool areFeetOnGround
    {
        get { return collidingObjects.Count > 0; }
    }

    private void OnTriggerEnter(Collider other)
    {
        collidingObjects.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        collidingObjects.Remove(other);
    }
}
