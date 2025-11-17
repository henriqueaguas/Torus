using MenteBacata.ScivoloCharacterControllerDemo;
using UnityEngine;

public class Player_VehicleInteraction : MonoBehaviour
{
    [SerializeField] Transform rayTransformPivot;
    [SerializeField] GameObject playerCharacter;
    private RaycastHit hit;
    private float detectRadius = 0.7f;
    private float detectRange = 7f;
    Transform vehicleTransform;
    private bool insideVehicle = false;

    void Update()
    {
        ManageVehicleInteraction();
    }

    void ManageVehicleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Player inside vehicle, leave
            if (insideVehicle)
            {
                TogglePlayerVisibility(true);

                //Desactivate CarController script
                vehicleTransform.GetComponent<CarController>().enabled = false;

                insideVehicle = false;
            }
            // Player not inside vehicle, enter 
            else
            {
                if (Physics.SphereCast(rayTransformPivot.position, detectRadius, rayTransformPivot.forward, out hit, detectRange))
                {
                    if (hit.transform.tag == "Vehicle")
                    {
                        vehicleTransform = hit.transform;
                        TogglePlayerVisibility(false);

                        //Activate CarController script
                        vehicleTransform.GetComponent<CarController>().enabled = true;

                        insideVehicle = true;
                    }
                }
            }
        }
    }

    void TogglePlayerVisibility(bool enable)
    {
        if (enable)
        {
            ChangeColliders(true);
            ChangePlayerPosition(true);
            TogglePlayerGameObjects(true);
        }
        else
        {
            ChangeColliders(false);
            ChangePlayerPosition(false);
            TogglePlayerGameObjects(false);
        }
        SwitchCameras(vehicleTransform.Find("CameraPosition").transform);
    }

    void ChangeColliders(bool enabled)
    {
        if (this.transform.TryGetComponent<CapsuleCollider>(out var c1))
        {
            c1.enabled = enabled;
        }
        else
        {
            Debug.LogError("Capsule Collider not found");
        }
        if (this.transform.TryGetComponent<SphereCollider>(out var c2))
        {
            c2.enabled = enabled;
        }
        else
        {
            Debug.LogError("Sphere Collider not found");
        }
    }

    void TogglePlayerGameObjects(bool enable)
    {
        this.transform.parent.transform.Find("Fog around player").gameObject.SetActive(enable);
        this.transform.Find("Floating Dust").gameObject.SetActive(enable);
        this.transform.Find("Player Light").gameObject.SetActive(enable);
        playerCharacter.SetActive(enable);
    }

    void ChangePlayerPosition(bool enable)
    {
        if (enable)
        {
            //Exit Car
            // playerRB.isKinematic = false;
            ExitVehicle();
            // Enable Player Controllers 
            this.transform.GetComponent<SimpleCharacterController>().EnableMovement();
        }
        else
        {
            // Enter Car
            // playerRB.isKinematic = true;

            // Disable Player Controllers 
            this.transform.GetComponent<SimpleCharacterController>().DisableMovement();
        }
    }

    void SwitchCameras(Transform CarCamPosition)
    {
        FirstPersonCamera firstPersonCameraScript = GameObject.FindWithTag("MainCamera").GetComponent<FirstPersonCamera>();
        // Toggle between camera Position
        if (insideVehicle)
        {
            // Change Target Position o main camera to player Camera Target
            firstPersonCameraScript.SwitchTo1stPerson();
        }
        else
        {
            // Change Target Position o main camera to CarCamPosition
            FirstPersonCamera.target = CarCamPosition;
        }
    }

    void ExitVehicle()
    {
        if (!vehicleTransform.GetComponent<CarController>().IsGrounded())
        {
            this.transform.position = new Vector3(vehicleTransform.position.x, vehicleTransform.position.y + 5, vehicleTransform.position.z);
        }
        else
        {
            this.transform.position = vehicleTransform.Find("ExitPosition").transform.position;
        }
    }
}
