using UnityEngine;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBrakeForce;
    private float brakeTreshold = .5f;

    // Settings
    [SerializeField] private float motorForce, brakeForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider_1, rearRightWheelCollider_1;


    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform_1, rearRightWheelTransform_1;
    [SerializeField] private Transform steeringWheel;

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        HandleSteeringAngle();
    }

    private void GetInput()
    {
        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        // Acceleration Input
        verticalInput = Input.GetAxis("Vertical");
    }

    public bool DifferentSign(float num1, float num2)
    {
        return (num1 < 0 && num2 > 0) || (num1 > 0 && num2 < 0);
    }


    private void HandleMotor()
    {
        var rb = GetComponent<Rigidbody>();

        float forwardVelocity = Vector3.Dot(rb.velocity, transform.forward);
        float velocity = rb.velocity.magnitude;

        if (verticalInput != 0f)
        {
            if (DifferentSign(forwardVelocity, verticalInput))
            {
                if (velocity > brakeTreshold)
                {
                    currentBrakeForce = brakeForce;
                }
                else
                {
                    currentBrakeForce = 0f;
                    frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
                    frontRightWheelCollider.motorTorque = verticalInput * motorForce;
                }
            }
            else
            {
                currentBrakeForce = 0f;
                frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
                frontRightWheelCollider.motorTorque = verticalInput * motorForce;
            }
        }
        else
        {
            // No vertical input, set motor torque to 0 and apply breakforce
            frontLeftWheelCollider.motorTorque = 0f;
            frontRightWheelCollider.motorTorque = 0f;
            currentBrakeForce = brakeForce * 0.3f;
        }
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider_1.brakeTorque = currentBrakeForce;
        rearRightWheelCollider_1.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput * .5f;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider_1, rearRightWheelTransform_1);
        UpdateSingleWheel(rearLeftWheelCollider_1, rearLeftWheelTransform_1);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.SetPositionAndRotation(pos, rot);
    }

    private void HandleSteeringAngle()
    {
        float amplifiedSteerAngle = frontLeftWheelCollider.steerAngle * -3f;
        Quaternion targetRotation = Quaternion.Euler(steeringWheel.localRotation.eulerAngles.x, steeringWheel.localRotation.eulerAngles.y, -amplifiedSteerAngle);
        steeringWheel.localRotation = Quaternion.Lerp(steeringWheel.localRotation, targetRotation, 3f * Time.deltaTime);
    }

    public bool IsGrounded()
    {
        return frontLeftWheelCollider.isGrounded && frontRightWheelCollider.isGrounded && rearRightWheelCollider.isGrounded &&
            rearLeftWheelCollider.isGrounded && rearRightWheelCollider_1.isGrounded && rearLeftWheelCollider_1.isGrounded;
    }
}
