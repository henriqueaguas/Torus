using UnityEngine;
using UnityEngine.SceneManagement;

public class HotBarManager : MonoBehaviour
{
    private AbilityManager abilityManager;

    void Start()
    {
        abilityManager = GetComponent<AbilityManager>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Village") return;

        if (GlobalManager.Instance == null) return;

        if (Input.GetKeyUp(KeyCode.Alpha1) && GlobalManager.Instance?.playerAbilities.Count > 0)
        {
            // Toggle Logic
            if (GlobalManager.Instance.currentAbility == GlobalManager.Instance.playerAbilities[0])
            {
                abilityManager.DeSelectJumpAbility();
                GlobalManager.Instance.SetAbility(Ability.AbilityType.None);
            }
            else
            {
                GlobalManager.Instance.SetAbility(GlobalManager.Instance.playerAbilities[0]);
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha2) && GlobalManager.Instance.playerAbilities.Count > 1)
        {
            // Toggle Logic
            if (GlobalManager.Instance.currentAbility == GlobalManager.Instance.playerAbilities[1])
            {
                GlobalManager.Instance.SetAbility(Ability.AbilityType.None);
            }
            else
            {
                GlobalManager.Instance.SetAbility(GlobalManager.Instance.playerAbilities[1]);
            }
        }
    }

    public static Sprite GetSprite(Ability.AbilityType abilityType, Sprite jumpSprite, Sprite grabSprite)
    {
        switch (abilityType)
        {
            default:
            case Ability.AbilityType.Jump: return jumpSprite;
            case Ability.AbilityType.Grab: return grabSprite;
        }
    }
}
