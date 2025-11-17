using UnityEngine;

public class TutorialLastSectionTrigger : MonoBehaviour
{
    TutorialManager tutorialManager;

    void Start()
    {
        tutorialManager = transform.parent.GetComponent<TutorialManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.Disable();
        }
    }
}
