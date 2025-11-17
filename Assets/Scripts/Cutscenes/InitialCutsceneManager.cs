using MenteBacata.ScivoloCharacterControllerDemo;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class InitialCutsceneManager : MonoBehaviour
{
    [SerializeField] private RadioSongController radioSongController;
    [SerializeField] private Transform playerSpawnPoint;
    public static bool hasPassedCutscene = false;
    public static bool canSkipCutscene = true;
    private int prevVSYNC;
    private bool CutsceneEnded = false;

    void Awake()
    {
        if (!hasPassedCutscene)
        {
            DontDestroyOnLoad(gameObject);
            prevVSYNC = QualitySettings.vSyncCount;

            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            hasPassedCutscene = true;
            this.GetComponent<PlayableDirector>().enabled = true;
        }
        else
        {
            Debug.Log("Destroy Cutscene Manager");
            radioSongController.Play();
            Destroy(this.gameObject);
        }
    }

    public void OnEnablePlayerMovement()
    {
        GameObject.FindWithTag("Player").GetComponent<SimpleCharacterController>().EnableMovement();
    }

    public void OnCutsceneEnded()
    {
        if (CutsceneEnded) return;

        CutsceneEnded = true;

        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        GameObject playerGO = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        Transform playerModel = playerGO.transform.Find("PlayerModel");
        playerModel.GetComponent<CheckpointManager>().levelSpawnPoint = playerSpawnPoint.position;
        playerModel.GetComponent<CheckpointManager>().levelSpawnPointRot = playerSpawnPoint.rotation;
        playerModel.transform.position = playerSpawnPoint.position;
        GameObject.FindWithTag("MainCamera").transform.rotation = playerSpawnPoint.rotation;
        playerModel.GetComponent<SimpleCharacterController>().DisableMovement();

        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = prevVSYNC;
        canSkipCutscene = false;

        GlobalManager.Instance.houseAbilities.TryGetValue("house 1", out Ability.AbilityType ability);
        GlobalManager.Instance.abilities.TryGetValue(ability, out string abilityTag);
        GlobalManager.Instance.nextTarget = abilityTag;
    }

    public void OnCustsceneSongEnded()
    {
        radioSongController.Play();
        this.GetComponent<PlayableDirector>().enabled = false;
    }

    public void Update()
    {
        if (canSkipCutscene && Input.GetKeyDown(KeyCode.Space))
        {
            var pd = this.GetComponent<PlayableDirector>();
            pd.time = 98.9;
            OnCutsceneEnded();
        }
    }
}
