using System.Collections;
using MenteBacata.ScivoloCharacterControllerDemo;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LightManager : MonoBehaviour
{
    private const float FOV_WHEN_RUNNING = 10f;
    [SerializeField] Volume postProcessingVolume;
    [SerializeField] private Light playerLight;
    [SerializeField] private float lightIntensityDecayRate;
    [SerializeField] private float lightSpentFlashingShadowEntityInPercentage;
    [SerializeField] private AudioSource breatheAS;
    [SerializeField] private AudioSource heartBeatAS;
    [SerializeField] private Image lightEnergyBarColor;
    [SerializeField] private Color fullLightEnergyBarColor;
    [SerializeField] private Color halfLightEnergyBarColor;
    [SerializeField] private Color quarterLightEnergyBarColor;
    [SerializeField] private CanvasGroup ingameUI;
    private enum LightState
    {
        FULL,
        HALF,
        QUARTER
    }
    private LightState currentLightState;
    private LightState previousLightState;

    private Animator animator;
    private Vignette _vignetteEffect;
    private ColorAdjustments _colorAdjustmentEffect;
    private ChromaticAberration _chromaticAberration;
    private float minVignetteIntensity = 0.2f;
    private float maxVignetteIntensity = 0.4f;
    private float minIdleAnimationSpeed = 1f;
    private float maxIdleAnimationSpeed = 3f;
    private float minFOV = 65f;
    private float maxFOV = 95f;
    private float minFogDensity = 80f;
    private float maxFogDensity = 30f;
    private float minChromaticAberration = .2f;
    private float maxChromaticAberration = 1f;

    private float initialIntensity;
    private UI_InGame UI;
    private bool isFrozen = false;
    private SimpleCharacterController controller;
    private float targetFOV;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<SimpleCharacterController>();

        initialIntensity = playerLight.intensity;
        UI = transform.parent.Find("UI_InGame").GetComponent<UI_InGame>();
        UI.lightEnergyBar.minValue = 0;
        UI.lightEnergyBar.maxValue = initialIntensity;
        UI.lightEnergyBar.value = initialIntensity;

        if (GlobalManager.Instance != null && GlobalManager.Instance.HasCompletedAllHouses() && SceneManager.GetActiveScene().name.Equals("Village"))
        {
            playerLight.enabled = false;
        }

        // Only decreases light over time on Parkour levels
        if (!SceneManager.GetActiveScene().name.Contains("-"))
            isFrozen = true;

        if (!postProcessingVolume.profile.TryGet(out _vignetteEffect))
            Debug.LogError("Could not find Vignette post-processing effect!");

        if (!postProcessingVolume.profile.TryGet(out _colorAdjustmentEffect))
            Debug.LogError("Could not find Color Adjustment post-processing effect!");

        if (!postProcessingVolume.profile.TryGet(out _chromaticAberration))
            Debug.LogError("Could not find Chromatic Aberration post-processing effect!");
    }

    void Update()
    {
        if (!isFrozen)
            DecreaseLightOverTime();

        UpdateUIBar();

        // bigger means more light
        float lightRatio = playerLight.intensity / initialIntensity;

        // The UI also becomes less visible
        ingameUI.alpha = lightRatio + .25f;

        currentLightState = lightRatio <= .25f ? LightState.QUARTER : lightRatio <= .6f ? LightState.HALF : LightState.FULL;

        if (currentLightState != previousLightState)
        {
            lightEnergyBarColor.color =
                (currentLightState == LightState.FULL) ? fullLightEnergyBarColor :
                (currentLightState == LightState.HALF) ? halfLightEnergyBarColor :
                quarterLightEnergyBarColor;

            if (currentLightState != LightState.FULL)
                StartCoroutine(FlickerLight());
        }
        previousLightState = currentLightState;

        _vignetteEffect.intensity.value = Mathf.Lerp(maxVignetteIntensity, minVignetteIntensity, lightRatio);
        _colorAdjustmentEffect.colorFilter.value = Color.Lerp(Color.red, Color.white, lightRatio);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            animator.SetFloat("IdleSpeed", Mathf.Lerp(maxIdleAnimationSpeed, minIdleAnimationSpeed, lightRatio));
        else
            animator.SetFloat("IdleSpeed", 1);

        UI_InGameMenu.cameraFOV = Mathf.Lerp(UI_InGameMenu.cameraFOV, targetFOV, 6f * Time.deltaTime);
        targetFOV = Mathf.Lerp(minFOV, maxFOV, lightRatio) + ((controller.isRunning && controller.velocity.magnitude > .2f) ? FOV_WHEN_RUNNING : 0f);

        RenderSettings.fogEndDistance = Mathf.Lerp(maxFogDensity, minFogDensity, lightRatio);
        _chromaticAberration.intensity.value = Mathf.Lerp(maxChromaticAberration, minChromaticAberration, lightRatio);
        if (!controller.isGrounded)
        {
            breatheAS.volume = .04f;
        }
        else
        {
            breatheAS.pitch = Mathf.Lerp(1.1f, .8f, lightRatio);
            breatheAS.volume = Mathf.Lerp(.2f, .1f, lightRatio);
        }

        lightEnergyBarColor.enabled = lightRatio > .05f;

        if (lightRatio <= .25f)
        {
            if (!heartBeatAS.isPlaying)
                heartBeatAS.Play();
        }
        else
        {
            heartBeatAS.Stop();
        }
    }

    private IEnumerator FlickerLight()
    {
        playerLight.enabled = true;

        // Flicker 3 times
        playerLight.enabled = false;
        yield return new WaitForSeconds(.5f);
        playerLight.enabled = true;
        yield return new WaitForSeconds(.5f);
        playerLight.enabled = false;
        yield return new WaitForSeconds(.5f);
        playerLight.enabled = true;
        yield return new WaitForSeconds(.5f);
        playerLight.enabled = false;
        yield return new WaitForSeconds(1f);
        playerLight.enabled = true;
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }

    public void UnfreezeLightDecay()
    {
        isFrozen = false;
    }

    public void FreezeLightDecay()
    {
        isFrozen = true;
    }

    public void RechargeLight()
    {
        playerLight.intensity = initialIntensity;
        currentLightState = LightState.FULL;
    }

    private void DecreaseLightOverTime()
    {
        playerLight.intensity -= lightIntensityDecayRate * Time.deltaTime;
        // Don't make it less than 0
        playerLight.intensity = Mathf.Max(playerLight.intensity, 0f);
    }

    private void UpdateUIBar()
    {
        UI.lightEnergyBar.value = playerLight.intensity;
    }

    public void UseFlash()
    {
        playerLight.intensity -= initialIntensity * lightSpentFlashingShadowEntityInPercentage;
        playerLight.intensity = Mathf.Max(playerLight.intensity, 0f);
    }

    public bool CanFlash()
    {
        return playerLight.intensity >= initialIntensity * lightSpentFlashingShadowEntityInPercentage;
    }
}
