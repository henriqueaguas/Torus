using System.Collections.Generic;
using UnityEngine;

public class CheckpointColider : MonoBehaviour
{
    CheckpointManager checkpointManager;
    LightManager lightManager;
    [SerializeField] Transform CPPosition;

    [SerializeField] List<GameObject> lights;

    void GetCheckpointManagerIfNotExists()
    {

        if (checkpointManager == null)
        {
            checkpointManager = GameObject.FindWithTag("Player").GetComponent<CheckpointManager>();
        }
        if (checkpointManager == null)
        {
            Debug.LogError("Checkpoint Manager not found");
        }

        if (lightManager == null)
        {
            lightManager = GameObject.FindWithTag("Player").GetComponent<LightManager>();
        }
        if (lightManager == null)
        {
            Debug.LogError("Light Manager not found");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GetCheckpointManagerIfNotExists();

        if (other.CompareTag("Player"))
        {
            checkpointManager.UpdateCheckpoint(CPPosition.position, CPPosition.rotation);

            //Turn off light
            foreach (var light in lights)
            {
                foreach (var l in light.GetComponentsInChildren<Light>())
                    l.enabled = false;
                light.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
            }

            lightManager.FreezeLightDecay();
            lightManager.RechargeLight();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // start discharging the light battery
            lightManager.UnfreezeLightDecay();
        }
    }
}
