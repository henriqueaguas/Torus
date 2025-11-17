using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FlashLightManager : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] float flashDistance = 10;
    [SerializeField] float flashDuration = 0.5f;
    [SerializeField] private Light flashLight;
    [SerializeField] private ParticleSystem flashParticle;
    private Animator animator;
    private LayerMask flashLayerMask;
    private LightManager playerLightManager;
    private UI_InGame uI_InGame;
    private Coroutine flashCoroutine;
    private int defaultLayer;
    private bool canFlash = true;
    [SerializeField] private string hiddenLayer = "HideFromCamera";
    [SerializeField] private float flashEntityDelayS = .7f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerLightManager = transform.GetComponent<LightManager>();
        uI_InGame = transform.parent.Find("UI_InGame").GetComponent<UI_InGame>();
        flashLayerMask = LayerMask.GetMask("ShadowEntity");

        defaultLayer = flashParticle.gameObject.layer;
        flashParticle.gameObject.layer = LayerMask.NameToLayer(hiddenLayer);
        StartCoroutine(FlashParticles());
    }

    // Update is called once per frame
    void Update()
    {
        if (
            Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHitInfo, flashDistance, flashLayerMask) &&
            raycastHitInfo.transform.TryGetComponent<ShadowEntityFlash>(out ShadowEntityFlash objectFlashable) &&
            playerLightManager.CanFlash() && objectFlashable.CanFlash() && canFlash
        )
        {
            uI_InGame.UpdateText("[Left Click] Flash Shadow Entity");
            if (Input.GetMouseButtonDown(0))
            {
                canFlash = false;

                if (GlobalManager.Instance != null)
                    GlobalManager.Instance.instrFlashedEntity++;

                animator.CrossFade("FlashEntity", .05f);
                IEnumerator FlashEntity()
                {
                    yield return new WaitForSeconds(flashEntityDelayS);
                    playerLightManager.UseFlash();
                    objectFlashable.Flash();

                    if (flashCoroutine != null)
                        StopCoroutine(flashCoroutine);

                    flashCoroutine = StartCoroutine(FlashShadowEntity());
                }

                StartCoroutine(FlashEntity());
            }
        }
    }

    IEnumerator FlashShadowEntity()
    {
        // Activate the flash effect
        flashLight.enabled = true;

        // Activate flash particles
        flashParticle.gameObject.layer = defaultLayer;

        // Wait for some duration
        yield return new WaitForSeconds(flashDuration);

        // Deactivate the flash effect
        flashLight.enabled = false;

        // Desactivate flash particles
        flashParticle.gameObject.layer = LayerMask.NameToLayer(hiddenLayer);

        // Reset coroutine reference
        flashCoroutine = null;

        canFlash = true;
    }

    IEnumerator FlashParticles()
    {
        yield return new WaitForSeconds(0.5f);
        var main = flashParticle.main;
        main.simulationSpeed = 0f;
    }
}
