using System.Collections;
using MenteBacata.ScivoloCharacterControllerDemo;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    const float GRABBED_OBJECT_SCROLL_SPEED = 800;
    [SerializeField] Transform playerCameraTransform;
    [SerializeField] float pickUpDistance;
    [SerializeField] float jumpAbilitySpeed;
    private LayerMask pickUpLayerMask;
    private LayerMask abilityLayerMask;
    private Transform objectGrabbableTransform;
    private Transform objectGrabbableTransformParentBackup;
    private Ability loadingAbility;
    private Image loadBar;
    private UI_InGame uI_InGame;
    private UI_HotkeyBar ui_HotkeyBar;
    private float currentTime = 0f;
    private bool isPickingUp = false;
    private bool canUseGrabAbility = true;
    private bool canUseJumpAbility = true;
    private float grabAbilityCooldown = 10f;
    private float jumpAbilityCooldown = 10f;
    [HideInInspector] public bool usingJumpAbility = false;
    private Ability.AbilityType previousAbility = Ability.AbilityType.None;
    [SerializeField] private Color originalSlotColor;
    private SimpleCharacterController controller;

    void Start()
    {
        pickUpLayerMask = LayerMask.GetMask("Pickable");
        abilityLayerMask = LayerMask.GetMask("Ability");
        loadBar = transform.parent.Find("UI_InGame").Find("LoadBar").GetComponent<Image>();
        uI_InGame = transform.parent.Find("UI_InGame").GetComponent<UI_InGame>();
        controller = GetComponent<SimpleCharacterController>();

        ui_HotkeyBar = GameObject.Find("UI_HotkeyBar").GetComponent<UI_HotkeyBar>();
    }

    void Update()
    {
        HandleAbilityPickUp();
        HandleGrabAbility();
        HandleJumpAbility();

        if (GlobalManager.Instance != null && GlobalManager.Instance.currentAbility != previousAbility)
        {
            if (previousAbility != Ability.AbilityType.None)
            {
                ui_HotkeyBar.abilitiesTransform[previousAbility].position += Vector3.down * 10f;
                ui_HotkeyBar.abilitiesBorder[previousAbility].color = originalSlotColor;
            }
            previousAbility = GlobalManager.Instance.currentAbility;

            if (previousAbility != Ability.AbilityType.None)
            {
                ui_HotkeyBar.abilitiesTransform[previousAbility].position += Vector3.up * 10f;
                ui_HotkeyBar.abilitiesBorder[previousAbility].color = Color.green;
            }
        }
    }

    private void HandleGrabAbility()
    {
        if (GlobalManager.Instance != null && (GlobalManager.Instance.currentAbility == Ability.AbilityType.Grab || objectGrabbableTransform != null) && SceneManager.GetActiveScene().name != "Village")
        {
            // Revert the other ability (if it was activated or not)
            controller.UpdateJumpSpeed(controller.GetDefaultJumpSpeed());
            usingJumpAbility = false;

            if (objectGrabbableTransform != null)
            {
                HandleObjectDrop();
            }
            else
            {
                HandleObjectPickUp();
            }
            HandleObjectScroll();
        }
    }

    private void HandleObjectDrop()
    {
        uI_InGame.UpdateText("[Left Click] Release Object");
        if (Input.GetMouseButtonUp(0))
        {
            DropObject(grabAbilityCooldown);
        }
    }

    public void DropObject(float cooldown)
    {
        if (objectGrabbableTransform == null) return;

        objectGrabbableTransform.parent = null;
        objectGrabbableTransform.parent = objectGrabbableTransformParentBackup;
        ChangeColliders(objectGrabbableTransform, true);
        objectGrabbableTransform = null;
        objectGrabbableTransformParentBackup = null;
        canUseGrabAbility = false;
        GlobalManager.Instance.currentAbility = Ability.AbilityType.None;
        StartCoroutine(CooldownRoutine(cooldown, () => canUseGrabAbility = true, Ability.AbilityType.Grab));
    }

    private void HandleObjectPickUp()
    {
        if (canUseGrabAbility && Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHitGrab, pickUpDistance, pickUpLayerMask))
        {
            uI_InGame.UpdateText("[Left Click] Pick Up Object");
            if (Input.GetMouseButtonUp(0))
            {
                if (HasColliders(raycastHitGrab.transform))
                {
                    GetPlayerCameraIfNotExists();
                    objectGrabbableTransform = raycastHitGrab.transform;
                    objectGrabbableTransformParentBackup = objectGrabbableTransform.parent;
                    objectGrabbableTransform.parent = playerCameraTransform;
                    ChangeColliders(objectGrabbableTransform, false);

                    if (GlobalManager.Instance != null)
                        GlobalManager.Instance.instrGrabAbilityUsed++;
                }
            }
        }
    }

    private void HandleObjectScroll()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0 && objectGrabbableTransform != null)
        {
            Vector3 movement = GRABBED_OBJECT_SCROLL_SPEED * scrollInput * Time.deltaTime * playerCameraTransform.forward;
            objectGrabbableTransform.position += movement;
        }
    }

    private void HandleJumpAbility()
    {
        if (GlobalManager.Instance != null && GlobalManager.Instance.currentAbility == Ability.AbilityType.Jump && canUseJumpAbility && SceneManager.GetActiveScene().name != "Village")
        {
            controller.UpdateJumpSpeed(jumpAbilitySpeed);
            usingJumpAbility = true;
        }
    }

    public void TurnOffJumpAbility()
    {
        canUseJumpAbility = false;
        GlobalManager.Instance.currentAbility = Ability.AbilityType.None;
        StartCoroutine(CooldownRoutine(jumpAbilityCooldown, () => canUseJumpAbility = true, Ability.AbilityType.Jump));
        controller.UpdateJumpSpeed(controller.GetDefaultJumpSpeed());
        usingJumpAbility = false;
    }

    public void DeSelectJumpAbility()
    {
        // canUseJumpAbility = false;
        GlobalManager.Instance.currentAbility = Ability.AbilityType.None;
        // StartCoroutine(CooldownRoutine(jumpAbilityCooldown, () => canUseJumpAbility = true, Ability.AbilityType.Jump));
        controller.UpdateJumpSpeed(controller.GetDefaultJumpSpeed());
        usingJumpAbility = false;
    }

    private void HandleAbilityPickUp()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, abilityLayerMask))
        {
            if (raycastHit.transform.TryGetComponent<Ability>(out loadingAbility))
            {
                uI_InGame.UpdateText("[Hold E] Pick Up Ability");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    isPickingUp = true;
                    currentTime = 0f;
                    loadBar.fillAmount = 0f;
                }
            }
        }

        if (isPickingUp)
        {
            controller.DisableMovement();
            if (Input.GetKeyUp(KeyCode.E))
            {
                controller.EnableMovement();
                isPickingUp = false;
                loadBar.fillAmount = 0f;
                currentTime = 0;
            }
            else
            {
                currentTime += Time.deltaTime;
                float progress = currentTime / loadingAbility.GetPickUpTime();
                loadBar.fillAmount = Mathf.Lerp(0f, 1f, progress);

                if (currentTime >= loadingAbility.GetPickUpTime())
                {
                    controller.EnableMovement();
                    isPickingUp = false;
                    loadBar.fillAmount = 0f;
                    loadingAbility.PickUp();
                }
            }
        }
    }

    private IEnumerator CooldownRoutine(float cooldownTime, System.Action onCooldownComplete, Ability.AbilityType abilityType)
    {
        float startTime = Time.time;
        while (Time.time - startTime < cooldownTime)
        {
            float elapsedTime = Time.time - startTime;
            ui_HotkeyBar.abilityImages[abilityType].fillAmount = Mathf.Lerp(1f, 0f, elapsedTime / cooldownTime);
            yield return null;
        }
        ui_HotkeyBar.abilityImages[abilityType].fillAmount = 0f;
        onCooldownComplete.Invoke();
    }

    void GetPlayerCameraIfNotExists()
    {
        if (playerCameraTransform == null)
        {
            playerCameraTransform = GameObject.FindWithTag("MainCamera").transform;
        }
        if (playerCameraTransform == null)
        {
            Debug.LogError("Main Camera Transform not found");
        }
    }

    bool HasColliders(Transform t)
    {
        return t.GetComponents<Collider>().Length > 0;
    }

    void ChangeColliders(Transform t, bool enabled)
    {
        var colliders = t.GetComponents<Collider>();
        if (colliders.Length == 0)
        {
            Debug.LogError("No colliders found on the object: " + t.name);
            return;
        }

        foreach (var collider in colliders)
        {
            collider.enabled = enabled;
        }
    }
}
