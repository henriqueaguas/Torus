using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        if (GameObject.FindWithTag("Player") == null)
        {
            GameObject playerPrefab = Resources.Load<GameObject>("Player");
            GameObject playerGO = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            Transform playerModel = playerGO.transform.Find("PlayerModel");
            playerModel.GetComponent<CheckpointManager>().levelSpawnPoint = this.transform.position;
            playerModel.GetComponent<CheckpointManager>().levelSpawnPointRot = this.transform.rotation;
            playerModel.transform.position = this.transform.position;
            GameObject.FindWithTag("MainCamera").transform.rotation = this.transform.rotation;
        }
    }
}
