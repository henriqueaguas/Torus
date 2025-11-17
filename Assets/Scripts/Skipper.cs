using UnityEngine;
using UnityEngine.SceneManagement;

public class Skipper : MonoBehaviour
{
    public static bool isSkipping = false;
    void Start()
    {
        if (Skipper.isSkipping)
        {
            this.transform.position = GameObject.Find("TPEND").transform.position;
            Skipper.isSkipping = false;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (GlobalManager.Instance != null && GlobalManager.Instance.playerAbilities.Count != GlobalManager.Instance.abilities.Count)
                GlobalManager.Instance.CaptureAllAbilitiesHACK();
        }

        // Go to the end of current house
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (SceneManager.GetActiveScene().name.StartsWith("1-"))
            {
                Skipper.isSkipping = true;
                SceneManager.LoadScene("1-3");
            }
            else if (SceneManager.GetActiveScene().name.StartsWith("2-"))
            {
                Skipper.isSkipping = true;
                SceneManager.LoadScene("2-2");
            }
        }

        // Go to the end of current level
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (SceneManager.GetActiveScene().name.Contains("-"))
            {
                this.transform.position = GameObject.Find("TPEND").transform.position;
            }
        }
    }
}