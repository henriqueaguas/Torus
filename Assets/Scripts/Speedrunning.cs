using UnityEngine;
using UnityEngine.SceneManagement;

public class Speedrunning : MonoBehaviour
{
    void Start()
    {
        if (!SceneManager.GetActiveScene().name.Contains("-"))
            Destroy(this);
    }

    void Update()
    {
        if (GlobalManager.Instance != null && Input.GetKeyDown(KeyCode.R))
        {
            GlobalManager.Instance.StopTimer();
            var sceneLoading = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name.Split("-")[0] + "-" + 1);
            sceneLoading.completed += (_) => GlobalManager.Instance.StartTimer();
        }
    }
}
