using System.Collections;
using UnityEngine;

public class ShadowEntityFlash : MonoBehaviour
{
    private ShadowEntityManager shadowEntityManager;
    private ParticleSystem ps;
    private const float SHADOW_ENTITY_FLASHED_TIME = 5f;
    private ParticleSystem.NoiseModule noise;

    private Color defaultPsColor;

    void Start()
    {
        shadowEntityManager = transform.GetComponent<ShadowEntityManager>();
        ps = transform.GetComponent<ParticleSystem>();
        noise = ps.noise;
        defaultPsColor = ps.main.startColor.color;
    }

    public bool CanFlash()
    {
        return shadowEntityManager.GetSpeed() > 0.15f;
    }
    public void Flash()
    {
        shadowEntityManager.Flash();

        // Disperse Shadow
        noise.enabled = true;
        var mainModule = ps.main;
        mainModule.startColor = Color.white;

        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        yield return new WaitForSeconds(SHADOW_ENTITY_FLASHED_TIME);
        shadowEntityManager.Speed2();

        // Return to Normal
        noise.enabled = false;
        var mainModule = ps.main;
        mainModule.startColor = defaultPsColor;
    }
}