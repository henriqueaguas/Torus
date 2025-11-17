using UnityEngine;

public class TutorialCheckpointTrigger : MonoBehaviour
{
    const string textToDisplay = "This is a Checkpoint. Your light recharges here.\nBeware, venturing without light is dangerous!";
    [SerializeField] TutorialManager tutorialManager;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.DisplayText(textToDisplay);
            tutorialManager.ReachedCheckpoint();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.ExitedCheckpoint();
        }
    }
}
