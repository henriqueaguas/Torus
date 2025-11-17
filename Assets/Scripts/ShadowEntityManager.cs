using UnityEngine;

public class ShadowEntityManager : MonoBehaviour
{
    [SerializeField] private float speed = 0.25f;
    [SerializeField] private float rotationSpeed = 1.5f;
    [SerializeField] private AudioSource defaultLoop1;
    [SerializeField] private AudioSource defaultLoop2;
    [SerializeField] private AudioSource growlSound;
    [SerializeField] private AudioSource eatPlayerSound;
    [SerializeField] public AudioSource hurtSound;
    private const float MIN_GROWL_INTERVAL_S = 15f;
    private const float MAX_GROWL_INTERVAL_S = 30f;
    private const float SHADOW_BACKOFF_DISTANCE = 25f;
    private const float SHADOW_BACKOFF_THRESHOLD = 10f;
    private const float SHADOW_ROAR_SOUND_TRIGGER_DISTANCE = 3f;
    private GameObject player;
    private Transform playerModel;
    private UI_InGame playerInGameUI;
    private CheckpointManager checkpointManager;
    private bool isFrozen = false;

    private float nextGrowlPlayTime;

    void Start()
    {
        GetPlayerIfNotExists();
    }


    // Update is called once per frame
    void Update()
    {
        GetPlayerIfNotExists();

        if (playerModel != null)
        {
            // Rotate to face the target
            // transform.LookAt(playerModel);
            // Same as LookAt but smoother
            Quaternion targetRotation = Quaternion.LookRotation(playerModel.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (!isFrozen)
            {
                // Move towards the target
                transform.position = Vector3.Lerp(transform.position, playerModel.transform.position, speed * Time.deltaTime);
            }
        }

        if (Time.time >= nextGrowlPlayTime)
        {
            growlSound.Play();
            nextGrowlPlayTime = Time.time + Random.Range(MIN_GROWL_INTERVAL_S, MAX_GROWL_INTERVAL_S);
        }

        if (!eatPlayerSound.isPlaying && Vector3.Distance(this.transform.position, playerModel.position) < SHADOW_ROAR_SOUND_TRIGGER_DISTANCE)
        {
            if (growlSound.isPlaying)
                growlSound.Stop();
            // Avoid growling when eating the player
            nextGrowlPlayTime = Time.time + Random.Range(MIN_GROWL_INTERVAL_S, MAX_GROWL_INTERVAL_S);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Can't kill player if stunned
        if (other.gameObject.CompareTag("Player") && GetSpeed() > .15f)
        {
            // KILL PLAYER
            GetPlayerIfNotExists();
            eatPlayerSound.Play();

            playerInGameUI.PlayerDeadFadeToBlack();

            if (Vector3.Distance(this.transform.position, checkpointManager.lastCheckpoint) < SHADOW_BACKOFF_THRESHOLD)
                transform.position = playerModel.position - playerModel.forward * SHADOW_BACKOFF_DISTANCE;

            checkpointManager.GoToLastCheckpoint();
        }
    }

    public void Flash()
    {
        Speed1();
        hurtSound.Play();
    }

    void GetPlayerIfNotExists()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning("Player not found");
            }
            else
            {
                playerModel = player.transform;

                if (!player.TryGetComponent<CheckpointManager>(out checkpointManager))
                    Debug.LogError("Start(): Checkpoint Manager not found!");

                if (!player.transform.parent.Find("UI_InGame").TryGetComponent<UI_InGame>(out playerInGameUI))
                    Debug.LogError("Start(): UI In Game not found!");
            }
        }
    }

    public float GetSpeed()
    {
        return speed;
    }
    public void Speed1()
    {
        speed = 0.15f;
    }

    public void Speed2()
    {
        speed = 0.25f;
    }

    public void Speed3()
    {
        speed = 0.35f;
    }

    public void Speed4()
    {
        speed = 0.45f;
    }

    public void UnfreezeShadow()
    {
        isFrozen = false;
    }

    public void FreezeShadow()
    {
        isFrozen = true;
    }

}
