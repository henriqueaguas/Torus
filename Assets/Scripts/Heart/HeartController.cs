using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class HeartController : MonoBehaviour
{
    public static Action onFinishedStage = null;
    private static float currentHeartSize = 4;
    public static bool IsHeartFinished = false;

    private AudioSource hurtAS;
    private AudioSource bloodChokeAS;
    private AudioSource entityLoopAS;
    private AudioSource hearBeatAS;
    private GameObject redDustPS;
    private ParticleSystem bloodSplashPS;
    private Vignette _vignetteEffect;

    private const float HEART_SHRINK_DURATION_S = 2f;
    private float[] HEART_SHRINK_STAGE_SIZES = new float[] { 2f, 0f };
    private Vector3 initialScale;
    private Vector3 targetScale;
    private bool isLastStage = false;
    private bool isShrinking = false;
    private bool isDoneShrinking = false;
    private float elapsedTime;
    private float timeSinceAnimationEnd;

    public static void Shrink(Action onFinished)
    {
        onFinishedStage = onFinished;
        SceneManager.LoadScene("Heart");
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Initial Heart Size: " + currentHeartSize);

        hurtAS = GameObject.Find("Pain").GetComponent<AudioSource>();
        hurtAS.Play();

        bloodChokeAS = GameObject.Find("BloodChoke").GetComponent<AudioSource>();

        entityLoopAS = GameObject.Find("EntityLoop").GetComponent<AudioSource>();
        hearBeatAS = GameObject.Find("HeartBeat").GetComponent<AudioSource>();
        redDustPS = GameObject.Find("RedDust");
        bloodSplashPS = GameObject.Find("BloodSplash").GetComponent<ParticleSystem>();
        GameObject.Find("PostProcessing").GetComponent<Volume>().profile.TryGet(out _vignetteEffect);

        int shrinkStage = GlobalManager.Instance.lastCompletedHouse - 1;
        initialScale = new Vector3(currentHeartSize, currentHeartSize, currentHeartSize);

        Debug.Log("Starting Shrink Stage nr. " + shrinkStage);
        currentHeartSize = HEART_SHRINK_STAGE_SIZES[shrinkStage];

        isLastStage = HEART_SHRINK_STAGE_SIZES.Length == shrinkStage + 1;

        targetScale = new Vector3(currentHeartSize, currentHeartSize, currentHeartSize);
        isShrinking = true;
    }

    void Update()
    {
        if (isShrinking)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / HEART_SHRINK_DURATION_S;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            if (isLastStage && !bloodSplashPS.isPlaying && elapsedTime >= HEART_SHRINK_DURATION_S / 1.2f)
            {
                bloodSplashPS.Play();
                _vignetteEffect.intensity.value = 0f;
            }

            if (elapsedTime >= HEART_SHRINK_DURATION_S)
            {
                isShrinking = false;
                transform.localScale = targetScale;
                timeSinceAnimationEnd = Time.time;
                isDoneShrinking = true;

                Debug.Log("Done Shrinking. New size is: " + currentHeartSize);
            }
        }

        if (isLastStage && isDoneShrinking && !bloodChokeAS.isPlaying)
        {
            Debug.Log("Playing last blood choke");
            entityLoopAS.Stop();
            hearBeatAS.Stop();
            redDustPS.SetActive(false);
            bloodChokeAS.Play();
        }

        if (isDoneShrinking && !hurtAS.isPlaying && Time.time - timeSinceAnimationEnd >= 3.0f)
        {
            isShrinking = false;
            isDoneShrinking = false;
            elapsedTime = 0;
            timeSinceAnimationEnd = 0;

            Debug.Log("Finished Stage. Invoking callback");
            onFinishedStage?.Invoke();
            onFinishedStage = null;
            if (currentHeartSize == 0)
                IsHeartFinished = true;
        }
    }
}
