using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_InGameMenu : UI_AbstractCloseable
{
    [SerializeField] UI_InGame InGameUI;
    [SerializeField] UI_QualitySettingsMenu QualityMenu;
    [SerializeField] Button openQualityMenuBtn;

    [SerializeField] Button prototypeVillageBtn;
    [SerializeField] Button prototypeMovementBtn;
    [SerializeField] Button prototypeGrabbingObjectsBtn;
    [SerializeField] Button prototypeCheckpointsBtn;
    [SerializeField] Button prototypeShadowEntityBtn;
    [SerializeField] Button prototypeCarControllerBtn;
    [SerializeField] Button prototypeLoopsBtn;
    [SerializeField] Button backToMainMenuBtn;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider fovSlider;
    [SerializeField] Slider audioVolumeSlider;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Volume postProcessingVolume;
    private UI_FirstLevelTutorial TutorialUI;
    private ColorAdjustments _colorAdjustmentEffect;
    private ShadowsMidtonesHighlights _shadowsMidtonesHighlightsEffect;
    public static float mouseLookSensitivity;
    public static float cameraFOV;
    void Start()
    {
        BaseStart(false);

        if (!postProcessingVolume.profile.TryGet(out _colorAdjustmentEffect))
            Debug.LogError("Could not find Color Adjustment post-processing effect!");

        if (!postProcessingVolume.profile.TryGet(out _shadowsMidtonesHighlightsEffect))
            Debug.LogError("Could not find Shadows-Midtones-Highlights post-processing effect!");

        // Default Values
        mouseLookSensitivity = sensitivitySlider.value;

        // Handlers
        openQualityMenuBtn.onClick.AddListener(() =>
        {
            canvas.enabled = false;
            QualityMenu.Open();
        });
        prototypeVillageBtn.onClick.AddListener(() => SceneManager.LoadScene("Village"));
        prototypeCarControllerBtn.onClick.AddListener(() => SceneManager.LoadScene("CarController"));
        prototypeMovementBtn.onClick.AddListener(() => SceneManager.LoadScene("PlayerMovement"));
        prototypeGrabbingObjectsBtn.onClick.AddListener(() => SceneManager.LoadScene("ObjectGrabbing"));
        prototypeCheckpointsBtn.onClick.AddListener(() => SceneManager.LoadScene("Checkpoints-"));
        prototypeShadowEntityBtn.onClick.AddListener(() => SceneManager.LoadScene("ShadowEntity"));
        prototypeLoopsBtn.onClick.AddListener(() => SceneManager.LoadScene("Loops"));
        sensitivitySlider.onValueChanged.AddListener((v) => mouseLookSensitivity = v);
        // cameraFOV = fovSlider.value;
        // fovSlider.onValueChanged.AddListener((v) => cameraFOV = v);
        audioVolumeSlider.onValueChanged.AddListener((v) => AudioListener.volume = v);
        audioVolumeSlider.value = AudioListener.volume;
        backToMainMenuBtn.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        brightnessSlider.onValueChanged.AddListener((v) =>
        {
            _colorAdjustmentEffect.postExposure.value = Mathf.Lerp(0, 2.5f, v);
            _colorAdjustmentEffect.contrast.value = Mathf.Lerp(4, -30, v);
        });

        var tutUI = GameObject.FindWithTag("TutorialUI");
        if (tutUI != null)
            TutorialUI = tutUI.GetComponent<UI_FirstLevelTutorial>();
    }

    void Update()
    {
        if (!QualityMenu.IsOpen() && Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.enabled = !canvas.enabled;

            if (canvas.enabled)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            InGameUI.Close();

            if (TutorialUI != null && TutorialUI.enabled)
                TutorialUI.Close();
        }

        // Got back to game
        if (!QualityMenu.IsOpen() && !IsOpen())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            InGameUI.Open();

            if (TutorialUI != null && TutorialUI.enabled)
                TutorialUI.Open();
        }
    }
}
