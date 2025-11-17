using UnityEngine;

public class TutorialSectionTrigger : MonoBehaviour
{
    [SerializeField] string textToDisplay;
    TutorialManager tutorialManager;


    void Start()
    {
        tutorialManager = transform.parent.GetComponent<TutorialManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.DisplayText(textToDisplay);
        }
    }
}
