using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class UI_QualitySettingsMenu : UI_AbstractCloseable
{
    public static bool SHOW_FPS = false;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private TMP_Dropdown overallQualityLevelDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown antiAliasing;
    [SerializeField] private TMP_Dropdown textureMipMap;
    [SerializeField] private TMP_Dropdown shadowResolution;
    [SerializeField] private TMP_Dropdown anisotropicFiltering;
    [SerializeField] private Toggle showFpsToggle;

    private List<int> antiAliasingValues = new List<int> { 0, 2, 4, 8 };


    void Start()
    {
        BaseStart(false);

        // Initial UI State
        Application.targetFrameRate = -1;
        showFpsToggle.isOn = SHOW_FPS;

        overallQualityLevelDropdown.AddOptions(new List<string>(QualitySettings.names));
        antiAliasing.AddOptions(new List<string>(new string[] { "None", "2x", "4x", "8x" }));
        textureMipMap.AddOptions(new List<string>(new string[] { "High", "Medium", "Low", "Very Low" }));
        shadowResolution.AddOptions(new List<string>(new string[] { "Low", "Medium", "High", "Very High" }));
        anisotropicFiltering.AddOptions(new List<string>(new string[] { "OFF", "ON", "Force ON" }));

        overallQualityLevelDropdown.value = QualitySettings.GetQualityLevel();
        UpdateUIValuesExceptOverallQuality();

        // Event Handlers
        vSyncToggle.onValueChanged.AddListener(SetVSync);
        overallQualityLevelDropdown.onValueChanged.AddListener(SetOverallQuality);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        antiAliasing.onValueChanged.AddListener(SetAntiAliasing);
        textureMipMap.onValueChanged.AddListener(SetTextureMipMap);
        shadowResolution.onValueChanged.AddListener(SetShadowResolution);
        anisotropicFiltering.onValueChanged.AddListener(SetAnisotropicFiltering);
        showFpsToggle.onValueChanged.AddListener(SetShowFps);

        canvas = GetComponent<Canvas>();

        Close();
    }

    private void SetShowFps(bool value)
    {
        SHOW_FPS = value;
    }

    private void SetAnisotropicFiltering(int value)
    {
        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)value;
    }

    private void SetShadowResolution(int value)
    {
        QualitySettings.shadowResolution = (ShadowResolution)value;
    }

    private void SetTextureMipMap(int value)
    {
        QualitySettings.globalTextureMipmapLimit = value;
    }

    private void SetAntiAliasing(int value)
    {
        QualitySettings.antiAliasing = antiAliasingValues[value];
    }

    private void SetOverallQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
        UpdateUIValuesExceptOverallQuality();
    }

    private void UpdateUIValuesExceptOverallQuality()
    {
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        antiAliasing.value = QualitySettings.antiAliasing;
        textureMipMap.value = QualitySettings.globalTextureMipmapLimit;
        shadowResolution.value = (int)QualitySettings.shadowResolution;
        anisotropicFiltering.value = (int)QualitySettings.anisotropicFiltering;
    }

    private void SetVSync(bool newValue)
    {
        QualitySettings.vSyncCount = newValue ? 1 : 0;
    }

    void Update()
    {
        if (IsOpen() && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void SetResolution(int index)
    {
        int resolution_width = 1920;
        int resolution_height = 1080;

        switch (index)
        {
            case 0: resolution_width = 1920; resolution_height = 1080; break;
            case 1: resolution_width = 1280; resolution_height = 720; break;
            case 2: resolution_width = 1024; resolution_height = 768; break;
            case 3: resolution_width = 800; resolution_height = 600; break;
        };

        Screen.SetResolution(resolution_width, resolution_height, Screen.fullScreen);
    }
}
