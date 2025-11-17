using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalCutsceneManager : MonoBehaviour
{
    public static bool hasPlayed = false;
    private const float TARGET_EXPOSURE = 5.5f;
    private const float TARGET_ROTATION = 270;
    private const float TARGET_TORUS_FOG_Y_OFFSET = 90f;
    private const float SKYBOX_ANIMATION_ROTATION_DURATION = 13f;
    private const float SKYBOX_ANIMATION_EXPOSURE_DURATION = 27f;
    private const float BLACK_ANIMATION_DURATION = 13f;
    private const float TORUS_LIFT_ANIMATION_DURATION = 13f;
    private const float TO_WHITE_ANIMATION_DURATION = 5f;

    [SerializeField] private GameObject sunGo;
    [SerializeField] private List<GameObject> GOSsToDisable;
    [SerializeField] private GameObject TorusFog;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject cameras;
    [SerializeField] private GameObject clouds;
    [SerializeField] private List<AudioSource> SoundsToDisable;
    [SerializeField] private AudioSource birdSounds;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject thankYouText;
    private float initialExposure;
    private float initialRotation;

    void Start()
    {
        initialExposure = RenderSettings.skybox.GetFloat("_Exposure");
        initialRotation = RenderSettings.skybox.GetFloat("_Rotation");

        if (GlobalManager.Instance != null && GlobalManager.Instance.HasCompletedAllHouses())
            Play();
    }

    // to be called via _instance
    public void Play()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // Clone the original skybox
        RenderSettings.skybox = new Material(RenderSettings.skybox);
        RenderSettings.fog = false;
        sunGo.SetActive(true);
        foreach (var fog in GOSsToDisable)
            fog.SetActive(false);
        clouds.SetActive(false);
        TorusFog.SetActive(false);
        birdSounds.Play();
        if (!hasPlayed)
        {
            ui.SetActive(true);
            cameras.SetActive(true);
        }

        foreach (var sound in SoundsToDisable)
            sound.Stop();

        if (!hasPlayed)
        {
            StartCoroutine(FadeOutBlack());
            StartCoroutine(LerpSkyboxExposure());
            StartCoroutine(LerpSkyboxRotation());
            StartCoroutine(LerpTorusFog());
        }
        else
        {
            RenderSettings.skybox.SetFloat("_Exposure", TARGET_EXPOSURE);
            RenderSettings.skybox.SetFloat("_Rotation", TARGET_ROTATION);
        }
    }

    private bool hasDisabledRain;

    void Update()
    {
        GameObject playerModel = GameObject.FindWithTag("Player");
        if (!hasDisabledRain && playerModel != null && GlobalManager.Instance.HasCompletedAllHouses())
        {
            playerModel.transform.Find("WeatherMakerRainZone").gameObject.SetActive(false);
            hasDisabledRain = true;
        }
    }

    IEnumerator LerpSkyboxRotation()
    {
        float elapsedTime = 0f;

        while (elapsedTime < SKYBOX_ANIMATION_ROTATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / SKYBOX_ANIMATION_ROTATION_DURATION);

            float currentRotation = Mathf.Lerp(initialRotation, TARGET_ROTATION, t);

            RenderSettings.skybox.SetFloat("_Rotation", currentRotation);

            yield return null;
        }

        RenderSettings.skybox.SetFloat("_Rotation", TARGET_ROTATION);
    }

    IEnumerator LerpSkyboxExposure()
    {
        float elapsedTime = 0f;

        while (elapsedTime < SKYBOX_ANIMATION_EXPOSURE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / SKYBOX_ANIMATION_EXPOSURE_DURATION);

            float currentExposure = Mathf.Lerp(initialExposure, TARGET_EXPOSURE, t);

            RenderSettings.skybox.SetFloat("_Exposure", currentExposure);

            yield return null;
        }

        RenderSettings.skybox.SetFloat("_Exposure", TARGET_EXPOSURE);
    }

    IEnumerator FadeOutBlack()
    {
        RawImage fadeImage = ui.GetComponentInChildren<RawImage>();
        Color originalColor = fadeImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < BLACK_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / BLACK_ANIMATION_DURATION);
            fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        yield return new WaitForSeconds(2f);

        StartCoroutine(FadeOutWhite());
    }

    IEnumerator LerpTorusFog()
    {
        float elapsedTime = 0f;
        float initialY = TorusFog.transform.position.y;
        float targetY = initialY + TARGET_TORUS_FOG_Y_OFFSET;
        TorusFog.SetActive(true);

        while (elapsedTime < TORUS_LIFT_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / TORUS_LIFT_ANIMATION_DURATION);

            float newY = Mathf.Lerp(initialY, targetY, t);
            TorusFog.transform.position = new Vector3(TorusFog.transform.position.x, newY, TorusFog.transform.position.z);

            yield return null;
        }

        TorusFog.transform.position = new Vector3(TorusFog.transform.position.x, targetY, TorusFog.transform.position.z);
    }

    IEnumerator FadeOutWhite()
    {
        RawImage fadeImage = ui.GetComponentInChildren<RawImage>();
        Color originalColor = Color.white;
        fadeImage.color = originalColor;
        float elapsedTime = 0f;
        thankYouText.SetActive(true);

        while (elapsedTime < TO_WHITE_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / TO_WHITE_ANIMATION_DURATION);
            fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        // CUTSCENE END
        cameras.SetActive(false);
        ui.SetActive(false);
        TorusFog.SetActive(false);
        thankYouText.SetActive(false);
        hasPlayed = true;

        // SPAWN PLAYER AT CORRECT POSITION
        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        GameObject playerGO = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        Transform playerModel = playerGO.transform.Find("PlayerModel");
        playerModel.GetComponent<CheckpointManager>().levelSpawnPoint = playerSpawnPoint.position;
        playerModel.GetComponent<CheckpointManager>().levelSpawnPointRot = playerSpawnPoint.rotation;
        playerModel.transform.position = playerSpawnPoint.position;
        GameObject.FindWithTag("MainCamera").transform.rotation = playerSpawnPoint.rotation;
        playerModel.Find("WeatherMakerRainZone").gameObject.SetActive(false);
    }
}