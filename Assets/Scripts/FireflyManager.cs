using UnityEngine;
using UnityEngine.SceneManagement;

public class FireflyManager : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] public float playerOffset = 0.1f;
    [SerializeField] private float playerOffset_Y = 1f;
    private GameObject player;
    private MeshRenderer meshRenderer;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = false;
        if (SceneManager.GetActiveScene().name == "Village")
        {
            DeactivateUselessAbilities();
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Village" && GlobalManager.Instance?.nextTarget != null)
        {
            SetTarget(GlobalManager.Instance?.nextTarget);
        }
        else
        {
            if (SceneManager.GetActiveScene().name.Contains("-"))
            {
                // Final Portal
                SetTarget(GameObject.Find("end").transform);
            }
        }

        if (target != null)
        {
            transform.position = (target.position - player.transform.position).normalized * playerOffset + player.transform.position + Vector3.up * playerOffset_Y;
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    private void DeactivateUselessAbilities()
    {
        string nextTargetAbility = GlobalManager.Instance?.nextTarget;
        foreach (var entry in GlobalManager.Instance?.abilities)
        {
            string abilityTag = entry.Value;

            // If the tag is different from the next target ability
            if (abilityTag != nextTargetAbility)
            {
                // Find the GameObject with the tag
                GameObject obj = GameObject.FindWithTag(abilityTag);

                // If the GameObject is found, set its visibility to false
                if (obj != null)
                {
                    obj.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("GameObject with tag " + abilityTag + " not found.");
                }
            }
        }
    }

    public void SetTarget(string newTarget)
    {
        target = GameObject.FindWithTag(newTarget).transform;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
