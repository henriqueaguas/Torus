using UnityEngine;

public class Ability : MonoBehaviour {

    [SerializeField] private float pickUpTime = 2f;
    [SerializeField] private AbilityType type;
    public enum AbilityType
    {
        None,
        Jump,
        Grab,
    }

    public void PickUp()
    {
        GlobalManager.Instance.CaptureAbility(gameObject);
        transform.gameObject.SetActive(false);
    }

    public AbilityType GetAbilityType()
    {
        return type;
    }

    public float GetPickUpTime()
    {
        return pickUpTime;
    }
}