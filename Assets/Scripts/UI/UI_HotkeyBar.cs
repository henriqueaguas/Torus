using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HotkeyBar : MonoBehaviour
{
    [SerializeField] private Transform abilitySlotTemplate;
    public Sprite jumpSprite;
    public Sprite grabSprite;
    public Dictionary<Ability.AbilityType, Image> abilityImages;
    public Dictionary<Ability.AbilityType, Transform> abilitiesTransform;
    public Dictionary<Ability.AbilityType, Image> abilitiesBorder;

    private void Start()
    {
        abilityImages = new Dictionary<Ability.AbilityType, Image>();
        abilitiesTransform = new Dictionary<Ability.AbilityType, Transform>();
        abilitiesBorder = new Dictionary<Ability.AbilityType, Image>();
        if (GlobalManager.Instance != null)
            GlobalManager.Instance.OnHotkeyAbilityListChanged += UpdateVisual;
        UpdateVisual();
    }

    private void OnDestroy()
    {
        if (GlobalManager.Instance != null)
        {
            GlobalManager.Instance.OnHotkeyAbilityListChanged -= UpdateVisual;
        }
    }

    private void UpdateVisual()
    {
        if (GlobalManager.Instance == null) return;

        // Clear existing ability slots
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        abilityImages.Clear();

        // Create new ability slots
        List<Ability.AbilityType> hotkeyAbilityList = GlobalManager.Instance.playerAbilities;
        for (int i = 0; i < hotkeyAbilityList.Count; i++)
        {
            Ability.AbilityType hotkeyAbility = hotkeyAbilityList[i];
            Transform abilitySlotTransform = Instantiate(abilitySlotTemplate, transform);
            RectTransform abilitySlotRectTransform = abilitySlotTransform.GetComponent<RectTransform>();
            abilitySlotRectTransform.anchoredPosition = new Vector2(120f * i, 0f);
            abilitySlotTransform.Find("itemImage").GetComponent<Image>().sprite = HotBarManager.GetSprite(hotkeyAbility, jumpSprite, grabSprite);
            abilitySlotTransform.Find("numberText").GetComponent<TMPro.TextMeshProUGUI>().SetText((i + 1).ToString());

            abilitiesTransform[hotkeyAbility] = abilitySlotTransform;
            abilityImages[hotkeyAbility] = abilitySlotTransform.Find("cooldown").GetComponent<Image>();
            abilitiesBorder[hotkeyAbility] = abilitySlotTransform.Find("border").GetComponent<Image>();
        }
    }
}
