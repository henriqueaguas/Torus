using UnityEngine;

public class SingletonDestroyer : MonoBehaviour
{
    void Start()
    {
        var go = GameObject.Find("(EN) Timeline");
        if (go != null)
        {
            Destroy(go);
            InitialCutsceneManager.hasPassedCutscene = false;
            InitialCutsceneManager.canSkipCutscene = true;
        }

        go = GameObject.Find("GlobalManager");
        if (go != null) Destroy(go);
    }
}