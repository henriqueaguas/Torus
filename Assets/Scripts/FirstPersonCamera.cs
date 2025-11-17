using MenteBacata.ScivoloCharacterControllerDemo;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstPersonCamera : MonoBehaviour
{
    public static Transform target;
    public static bool isInFirstPerson = true;
    [SerializeField] Transform playerModel;
    [SerializeField] SkinnedMeshRenderer eyes;
    [SerializeField] Vector3 offsetFromEye;
    [SerializeField] Transform thirdPersonCameraPosition;
    [SerializeField] Image uiCrosshair;
    private Transform firstPersonCameraPosition;

    private float mouseX;
    private float mouseY;
    [HideInInspector] public bool useTargetRotation = false;
    private Camera cameraComp;


    void Start()
    {
        firstPersonCameraPosition = eyes.bones[2];
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraComp = GetComponent<Camera>();

        if (isInFirstPerson)
            SwitchTo1stPerson();
        else
            SwitchTo3rdPerson();
    }

    public void SwitchTo3rdPerson()
    {
        target = thirdPersonCameraPosition;
        uiCrosshair.enabled = false;
        isInFirstPerson = false;
    }

    public void SwitchTo1stPerson()
    {
        target = firstPersonCameraPosition;
        uiCrosshair.enabled = true;
        isInFirstPerson = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (target == firstPersonCameraPosition)
                SwitchTo3rdPerson();
            else
                SwitchTo1stPerson();
        }

        if (SimpleCharacterController.allowMovement && Cursor.lockState == CursorLockMode.Locked)
        {
            // Define the limits for vertical rotation
            float minVerticalAngle = -85f;
            float maxVerticalAngle = 85f;

            Vector2 mouseInput = new(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            mouseX = mouseInput.x * -UI_InGameMenu.mouseLookSensitivity;
            mouseY = mouseInput.y * UI_InGameMenu.mouseLookSensitivity;

            Vector3 rotate = new(mouseY, mouseX, 0);
            Vector3 newEulerAngles = transform.eulerAngles - rotate;

            float clampedX = Mathf.Clamp(ClampAngle(newEulerAngles.x), minVerticalAngle, maxVerticalAngle);

            transform.eulerAngles = new(clampedX, newEulerAngles.y, newEulerAngles.z);
        }
        if (useTargetRotation)
        {
            transform.rotation = target.rotation;
            playerModel.transform.rotation = target.rotation;
        }

        cameraComp.fieldOfView = UI_InGameMenu.cameraFOV;
        transform.position = target.position + target.TransformDirection(offsetFromEye);
    }

    float ClampAngle(float angle)
    {
        if (angle > 180f)
            return angle - 360f;
        else if (angle < -180f)
            return angle + 360f;
        else
            return angle;
    }

    void FixedUpdate()
    {
        // Rotate the player smoothly (but not the camera nor movement)
        playerModel.transform.rotation = Quaternion.Lerp(playerModel.transform.rotation, Quaternion.Euler(0f, transform.eulerAngles.y, 0f), 10 * Time.deltaTime);
    }
}
