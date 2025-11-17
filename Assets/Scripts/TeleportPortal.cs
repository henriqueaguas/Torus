using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPortal : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetPortalName;
    public static float playerPortalSpawnOffsetY = 4;
    public static float shadowEntityPortalSpawnOffsetY = -15;
    private Material portalMaterial;
    // Only != -1 on portals that are house entrypoints
    private int portalHouseId = -1;

    void Start()
    {
        // Clone material so that it's unique among different portals
        var renderer = GetComponent<Renderer>();
        renderer.material = new Material(renderer.material);
        portalMaterial = renderer.material;
        if (this.name.StartsWith("house"))
            portalHouseId = int.Parse(this.name.Split(" ")[1]);

        if (this.name == "start" && GameObject.FindWithTag("Player") == null)
        {
            GameObject playerPrefab = Resources.Load<GameObject>("Player");
            Vector3 targetPosition = transform.position + transform.up * playerPortalSpawnOffsetY;
            GameObject playerGO = Instantiate(playerPrefab, targetPosition, Quaternion.identity);
            Transform playerModel = playerGO.transform.Find("PlayerModel");
            playerModel.GetComponent<CheckpointManager>().levelSpawnPoint = targetPosition;
            playerModel.GetComponent<CheckpointManager>().levelSpawnPointRot = Quaternion.AngleAxis(-90f, this.transform.forward);
            playerModel.transform.position = targetPosition;
            GameObject.FindWithTag("MainCamera").transform.rotation = Quaternion.AngleAxis(-90f, transform.forward);

            GameObject shadowPrefab = Resources.Load<GameObject>("ShadowEntity");
            Vector3 targetEntityPosition = transform.position + transform.up * shadowEntityPortalSpawnOffsetY;
            Instantiate(shadowPrefab, targetEntityPosition, Quaternion.identity);
        }
    }

    void Update()
    {
        if (GlobalManager.Instance != null && portalHouseId != -1 && GlobalManager.Instance.playerAbilities.Count < portalHouseId)
            portalMaterial.DisableKeyword("_EMISSION");
        else
            portalMaterial.EnableKeyword("_EMISSION");
    }

    // Note: The portal has isTrigger=False but the player has a capsule collider with isTrigger=True
    void OnTriggerEnter(Collider other)
    {
        if (this.enabled && other.CompareTag("Player") && targetSceneName != "")
        {
            if (targetSceneName != null && targetSceneName != SceneManager.GetActiveScene().name)
            {
                // ENTER HOUSE
                if (SceneManager.GetActiveScene().name == "Village")
                {
                    if (GlobalManager.Instance != null && GlobalManager.Instance.TryEnterHouse(this.name))
                    {
                        SceneManager.LoadScene(targetSceneName);
                        Debug.Log("Enter house");
                    }
                }
                else
                {
                    // COMPLETE LEVEL
                    if (targetSceneName == "Village")
                    {
                        if (this.name == "end")
                        {
                            // First go to the Heart cutscene. Then go back to the village
                            GlobalManager.Instance?.CompleteHouse(targetPortalName);
                            if (!HeartController.IsHeartFinished)
                            {
                                HeartController.Shrink(() =>
                                {
                                    SceneManager.LoadScene("Village");
                                    SceneManager.sceneLoaded += OnVillageSceneLoaded;
                                });
                            }
                            else
                            {
                                SceneManager.LoadScene("Village");
                                SceneManager.sceneLoaded += OnVillageSceneLoaded;
                            }
                            Debug.Log("Completed House");
                        }
                        else
                        {
                            SceneManager.LoadScene("Village");
                            SceneManager.sceneLoaded += OnVillageSceneLoaded;
                            Debug.Log("Gave Up House");
                        }
                    }
                    // GOING THROUGH LEVEL PORTALS
                    else
                    {
                        SceneManager.LoadScene(targetSceneName);
                        Debug.Log("Next level");
                    }
                }
            }
            else
            {
                // Local Teleport
                var targetPortal = GameObject.Find(targetPortalName.ToString());
                Vector3 targetPosition = targetPortal.transform.position + targetPortal.transform.up * playerPortalSpawnOffsetY;
                other.transform.position = targetPosition;
                GameObject.FindWithTag("MainCamera").transform.rotation = Quaternion.AngleAxis(-90f, targetPortal.transform.forward);
                // StartCoroutine(EnableAfterDelay());
            }
        }
    }

    private IEnumerator EnableAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        this.enabled = true;
    }

    private void OnVillageSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cross-Scene Teleport
        var housePortal = GameObject.Find(targetPortalName.ToString());
        Vector3 targetPlayerPosition = housePortal.transform.position + housePortal.transform.up * playerPortalSpawnOffsetY;

        // Don't spawn player if the final cutscene should be played
        if (!GlobalManager.Instance.HasCompletedAllHouses() || FinalCutsceneManager.hasPlayed)
        {
            GameObject playerPrefab = Resources.Load<GameObject>("Player");
            GameObject playerGO = Instantiate(playerPrefab, targetPlayerPosition, Quaternion.identity);
            Transform playerModel = playerGO.transform.Find("PlayerModel");
            playerModel.transform.position = targetPlayerPosition;
            playerModel.GetComponent<CheckpointManager>().levelSpawnPoint = targetPlayerPosition;
            playerModel.GetComponent<CheckpointManager>().levelSpawnPointRot = Quaternion.AngleAxis(-90f, housePortal.transform.forward);
            GameObject.FindWithTag("MainCamera").transform.rotation = Quaternion.AngleAxis(-90f, housePortal.transform.forward);
        }

        SceneManager.sceneLoaded -= OnVillageSceneLoaded;
    }
}
