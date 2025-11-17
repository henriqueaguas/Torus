using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Material flickeringMaterial;
    private Color originalEmissionColor;
    private float minFlickerInterval = .08f;
    private float maxFlickerInterval = .15f;
    private float minLightOnInterval = .5f;
    private float maxLightOnInterval = .9f;
    private List<Light> lights = new();

    private Renderer goRenderer;

    void Start()
    {
        if (TryGetComponent<Light>(out var myLight)) lights.Add(myLight);
        lights.AddRange(this.GetComponentsInChildren<Light>(true));

        goRenderer = GetComponent<Renderer>();
        // Clone the original material (to avoid changing other lights using the same material)
        flickeringMaterial = new Material(goRenderer.material);
        goRenderer.material = flickeringMaterial;
        originalEmissionColor = flickeringMaterial.GetColor("_EmissionColor");
        StartCoroutine(FlickerLights());
    }

    IEnumerator FlickerLights()
    {
        while (true)
        {
            bool flickerOn = Random.value > 0.3f;

            Color newEmissionColor = flickerOn ? originalEmissionColor : Color.black;
            flickeringMaterial.SetColor("_EmissionColor", newEmissionColor);
            goRenderer.UpdateGIMaterials(); // Apply material changes

            foreach (var light in lights)
                light.enabled = flickerOn;

            // Light stays ON longer than it stays OFF
            float flickerInterval = flickerOn ? Random.Range(minLightOnInterval, maxLightOnInterval) : Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(flickerInterval);
        }
    }

    void OnDestroy()
    {
        flickeringMaterial.SetColor("_EmissionColor", originalEmissionColor);
        goRenderer.UpdateGIMaterials();
    }
}
