using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{

    [SerializeField] private Button startBtn;
    [SerializeField] private Slider audioSlider;
    [SerializeField] private AudioReverbZone audioReverbZone;

    void Start()
    {
        // Don't push too much for the CPU/GPU on the main menu
        QualitySettings.vSyncCount = 1;

        startBtn.onClick.AddListener(() => SceneManager.LoadScene("Village"));

        // Use the default slider value
        audioSlider.onValueChanged.AddListener((v) =>
        {
            AudioListener.volume = v;
            if (v < 0.1)
            {
                audioReverbZone.enabled = false;
            }
            else
            {
                audioReverbZone.enabled = true;
            }
        });
        audioSlider.value = AudioListener.volume;
    }
}
