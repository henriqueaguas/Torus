using System.Collections;
using MenteBacata.ScivoloCharacterControllerDemo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    private const int DYING_TIME_PENALIZATION_S = 30;
    [SerializeField] private TextMeshProUGUI uiTimePenalizationText;
    [HideInInspector] public Vector3 lastCheckpoint;
    [HideInInspector] public Quaternion lastCheckpointRot;
    Transform playerCamera;
    LightManager lightManager;
    [HideInInspector] public Vector3 levelSpawnPoint;
    [HideInInspector] public Quaternion levelSpawnPointRot;
    ShadowEntityManager shadowEntityManager;
    private AbilityManager abilityManager;
    private SimpleCharacterController controller;
    private float? lastDeath = null;
    private Coroutine uiTimePenalizationHideCoroutine;
    int offset = 10;

    void Start()
    {
        if (!SceneManager.GetActiveScene().name.Contains("-"))
        {
            this.enabled = false;
        }

        playerCamera = GameObject.FindWithTag("MainCamera").transform;

        if (playerCamera == null)
        {
            Debug.LogError("Player Camera Transform not found");
        }

        if (lightManager == null)
        {
            lightManager = GetComponent<LightManager>();
        }
        if (lightManager == null)
        {
            Debug.LogError("Light Manager not found");
        }

        controller = GetComponent<SimpleCharacterController>();
        if (controller == null)
        {
            Debug.LogError("Simple Character Controller not found");
        }

        if (!transform.TryGetComponent<AbilityManager>(out abilityManager))
            Debug.LogError("Start(): Ability Manager not found!");

        lastCheckpoint = levelSpawnPoint;
        lastCheckpointRot = levelSpawnPointRot;
    }

    void Update()
    {
        if (this.transform.position.y < levelSpawnPoint.y - 10)
        {
            GetShadowEntityManagerIfNotExists();

            if (shadowEntityManager != null && (lastCheckpoint.z - shadowEntityManager.transform.localPosition.z) < offset)
            {
                shadowEntityManager.transform.localPosition = new Vector3(shadowEntityManager.transform.localPosition.x, shadowEntityManager.transform.localPosition.y, lastCheckpoint.z + offset);
            }
            GoToLastCheckpoint();
        }
    }

    public void UpdateCheckpoint(Vector3 position, Quaternion rot)
    {
        if (!lastCheckpoint.Equals(position))
        {
            // Never been in this checkpoint before
            if (GlobalManager.Instance != null)
                GlobalManager.Instance.instrCheckpoints++;
        }

        lastCheckpoint = position;
        lastCheckpointRot = rot;
    }

    // Called when player falls or dies to shadow entity
    public void GoToLastCheckpoint()
    {
        if (lastCheckpoint != null)
        {
            abilityManager.DropObject(0);
            if (lastDeath == null || (Time.time - lastDeath > .7f && GlobalManager.Instance != null))
            {
                GlobalManager.Instance.instrDeaths++;
                lastDeath = Time.time;

                GlobalManager.Instance.currentHouseTimerMs += DYING_TIME_PENALIZATION_S * 1000;
                uiTimePenalizationText.text = "+" + DYING_TIME_PENALIZATION_S + "s";
                uiTimePenalizationText.enabled = true;
                if (uiTimePenalizationHideCoroutine != null) StopCoroutine(uiTimePenalizationHideCoroutine);
                uiTimePenalizationHideCoroutine = StartCoroutine(DisableTimePenalizationUI());
            }

            controller.transform.position = lastCheckpoint;
            playerCamera.rotation = lastCheckpointRot;
        }

        // Recharge Light
        lightManager.RechargeLight();
    }

    private IEnumerator DisableTimePenalizationUI()
    {
        yield return new WaitForSeconds(7);
        uiTimePenalizationText.enabled = false;
        uiTimePenalizationHideCoroutine = null;
    }

    void GetShadowEntityManagerIfNotExists()
    {
        if (shadowEntityManager == null)
        {
            var go = GameObject.FindWithTag("ShadowEntity");

            if (go != null && !go.TryGetComponent<ShadowEntityManager>(out shadowEntityManager))
            {
                Debug.LogWarning("ShadowEntityManager not found");
            }
        }
    }
}
