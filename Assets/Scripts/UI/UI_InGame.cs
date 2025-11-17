using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_InGame : UI_AbstractCloseable
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI commandInfoText;
    [SerializeField] private TextMeshProUGUI houseTimerText;
    [SerializeField] public GameObject lightEnergyEnabler;
    [SerializeField] public Slider lightEnergyBar;
    [SerializeField] public Image loadBar;
    [SerializeField] private TextMeshProUGUI keybindIndications;
    [SerializeField] private Animator playerDeadUIAnimator;

    [HideInInspector] public static string prevTimerString = "";
    private string timerSceneName = null;
    public float updateInterval = 0.5f;
    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;
    private float clearDelay = 0.20f;
    private float lastUpdateTime;

    private void Start()
    {
        BaseStart(true);

        if (SceneManager.GetActiveScene().name == "Village")
        {
            lightEnergyEnabler.SetActive(false);
            keybindIndications.enabled = false;
        }
        else
        {
            lightEnergyEnabler.SetActive(true);
            keybindIndications.enabled = true;
        }

        timeleft = updateInterval;
        ClearText();
    }

    private void Update()
    {
        if (UI_QualitySettingsMenu.SHOW_FPS)
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            frames++;

            if (timeleft <= 0.0)
            {
                float fps = accum / frames;
                fpsText.text = fps.ToString("F2");

                // Reset variables for next interval
                timeleft = updateInterval;
                accum = 0.0f;
                frames = 0;
            }
        }
        else
        {
            fpsText.text = "";
        }

        if (Time.time - lastUpdateTime > clearDelay)
            ClearText();

        if (GlobalManager.Instance != null && GlobalManager.Instance.isTimerRunning && GlobalManager.Instance.currentHouseTimerMs > 0)
        {
            int totalMilliseconds = GlobalManager.Instance.currentHouseTimerMs;
            int totalSeconds = Mathf.FloorToInt(totalMilliseconds / 1000);
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;
            int milliseconds = totalMilliseconds % 1000;

            if (SceneManager.GetActiveScene().name.Contains("-"))
            {
                timerSceneName = SceneManager.GetActiveScene().name;

                var parts = timerSceneName.Split("-");
                prevTimerString = string.Format("[House: {0} Level: {1}]\n {2:D2}:{3:D2}:{4:D2}.{5:D3}", parts[0], parts[1], hours, minutes, seconds, milliseconds);
                // prevTimerString = string.Format("[Playtesting Stats]\n [Level: {0}] House Time: {1:D2}:{2:D2}:{3:D2}\n Deaths: {4}; Checkpoints: {5};\n Jumps: {6}, Grabs: {7}\n Flash Entity: {8}", timerSceneName, hours, minutes, seconds, GlobalManager.Instance.instrDeaths, GlobalManager.Instance.instrCheckpoints, GlobalManager.Instance.instrJumpAbilityUsed, GlobalManager.Instance.instrGrabAbilityUsed, GlobalManager.Instance.instrFlashedEntity);
            }
        }

        if (GlobalManager.Instance != null && GlobalManager.Instance.currentHouseTimerMs == 0f)
        {
            houseTimerText.text = "";
        }
        else
        {
            houseTimerText.text = prevTimerString;
        }
    }

    public void PlayerDeadFadeToBlack()
    {
        playerDeadUIAnimator.Play("PlayerDeadUI");
    }

    public void UpdateText(string newText)
    {
        commandInfoText.text = newText;
        lastUpdateTime = Time.time;
    }

    public void ClearText()
    {
        commandInfoText.text = "";
    }
}