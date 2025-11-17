using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_FirstLevelTutorial : UI_AbstractCloseable
{
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private GameObject topBB;
    [SerializeField] private GameObject bottomBB;
    private const string TUTORIAL_LEVEL_NAME = "1-1";

    void Start()
    {
        BaseStart(false);

        if (SceneManager.GetActiveScene().name != TUTORIAL_LEVEL_NAME)
        {
            Close();
            this.enabled = false;
            return;
        }
        else
        {
            Open();
        }

        textField.text = "";
    }

    public void DisplayText(string text)
    {
        Open();
        textField.text = text;
    }

    public void ShowBlackBars()
    {
        topBB.SetActive(true);
        bottomBB.SetActive(true);
    }

    public void HideBlackBars()
    {
        topBB.SetActive(false);
        bottomBB.SetActive(false);
    }
}
