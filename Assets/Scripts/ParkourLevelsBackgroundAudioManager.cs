using UnityEngine;
using UnityEngine.SceneManagement;

public class ParkourLevelsBackgroundAudioManager : MonoBehaviour
{
    private static ParkourLevelsBackgroundAudioManager _instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);

            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogError("No AudioSource component found on this GameObject.");
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (_instance == this)
        {
            PlayAudioIfSceneNameContainsHyphen(SceneManager.GetActiveScene());
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayAudioIfSceneNameContainsHyphen(scene);
    }

    private void PlayAudioIfSceneNameContainsHyphen(Scene scene)
    {
        if (scene.name.Contains("-"))
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}
