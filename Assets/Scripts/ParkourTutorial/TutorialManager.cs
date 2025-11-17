using MenteBacata.ScivoloCharacterControllerDemo;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum TUTORIAL_STATE
    {
        BEFORE_CHECKPOINT,
        INSIDE_CHECKPOINT_BEFORE_LOCK,
        INSIDE_CHECKPOINT_LOCKED,
        INSIDE_CHECKPOINT_UNLOCKED,
        AFTER_CHECKPOINT
    }
    public static bool isInTutorial = false;
    private TUTORIAL_STATE state = TUTORIAL_STATE.BEFORE_CHECKPOINT;

    private ShadowEntityManager shadowEntityManager;
    private Transform playerModel;
    private FirstPersonCamera playerCameraControl;
    private UI_FirstLevelTutorial firstLevelTutorialUI;

    private const float TIME_TO_LOCK_CHECKPOINT = 0.3f;
    private const float TIME_TO_SECOND_TEXT = TIME_TO_LOCK_CHECKPOINT + 8f;
    private const float TIME_STUCK_IN_CHECKPOINT = TIME_TO_LOCK_CHECKPOINT + TIME_TO_SECOND_TEXT + 3.0f;
    private float timeArrivedAtCheckpoint;
    private SimpleCharacterController controller;
    private Animator animator;

    public void Disable()
    {
        TryGetPlayer();
        firstLevelTutorialUI.Close();
    }

    public void DisplayText(string text)
    {
        TryGetPlayer();
        firstLevelTutorialUI.DisplayText(text);
    }

    public void ReachedCheckpoint()
    {
        if (state.Equals(TUTORIAL_STATE.BEFORE_CHECKPOINT))
        {
            state = TUTORIAL_STATE.INSIDE_CHECKPOINT_BEFORE_LOCK;
            timeArrivedAtCheckpoint = Time.time;
        }
    }

    public void ExitedCheckpoint()
    {
        if (state.Equals(TUTORIAL_STATE.INSIDE_CHECKPOINT_UNLOCKED))
        {
            state = TUTORIAL_STATE.AFTER_CHECKPOINT;
        }
    }

    void Update()
    {
        TryGetPlayer();

        // Freeze Shadow in the beggining
        if (shadowEntityManager == null)
        {
            shadowEntityManager = GameObject.FindWithTag("ShadowEntity")?.GetComponent<ShadowEntityManager>();
            if (shadowEntityManager != null)
            {
                shadowEntityManager.FreezeShadow();
            }
        }

        // Lock player controls after {TIME_TO_LOCK_CHECKPOINT} seconds
        if (state.Equals(TUTORIAL_STATE.INSIDE_CHECKPOINT_BEFORE_LOCK) && Time.time - timeArrivedAtCheckpoint > TIME_TO_LOCK_CHECKPOINT)
        {
            // Lock checkpoint for {TIME_STUCK_IN_CHECKPOINT - TIME_TO_LOCK_CHECKPOINT} seconds
            controller.DisableMovement();
            isInTutorial = true;

            firstLevelTutorialUI.ShowBlackBars();

            // Lock camera and look at shadow entity for some time
            animator.Play("Idle");
            playerModel.LookAt(shadowEntityManager.transform);
            playerCameraControl.transform.LookAt(shadowEntityManager.transform);

            state = TUTORIAL_STATE.INSIDE_CHECKPOINT_LOCKED;
        }

        if (state.Equals(TUTORIAL_STATE.INSIDE_CHECKPOINT_LOCKED) && Time.time - timeArrivedAtCheckpoint > TIME_TO_SECOND_TEXT)
        {
            // Lock checkpoint for {TIME_STUCK_IN_CHECKPOINT - TIME_TO_LOCK_CHECKPOINT} seconds
            shadowEntityManager.UnfreezeShadow();
            DisplayText("The Shadow is coming! You must escape!\nYou can spend some of your light to stun it!");
        }

        // Unlock player controls + Unfreeze shadow entity
        if (state.Equals(TUTORIAL_STATE.INSIDE_CHECKPOINT_LOCKED) && Time.time - timeArrivedAtCheckpoint > TIME_STUCK_IN_CHECKPOINT)
        {
            controller.EnableMovement();
            firstLevelTutorialUI.HideBlackBars();
            state = TUTORIAL_STATE.INSIDE_CHECKPOINT_UNLOCKED;
            isInTutorial = false;
        }
    }

    void TryGetPlayer()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerModel = player.transform;
            firstLevelTutorialUI = playerModel.parent.transform.Find("UI_FirstLevelTutorial").GetComponent<UI_FirstLevelTutorial>();
            controller = playerModel.GetComponent<SimpleCharacterController>();
            animator = playerModel.GetComponent<Animator>();
            playerCameraControl = GameObject.FindWithTag("MainCamera").GetComponent<FirstPersonCamera>();
        }
    }
}
