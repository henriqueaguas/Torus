//#define MB_DEBUG

using MenteBacata.ScivoloCharacterController;
using UnityEngine;
using UnityEngine.UI;

namespace MenteBacata.ScivoloCharacterControllerDemo
{
    public class SimpleCharacterController : MonoBehaviour
    {
        private const float STAMINA_DECAY_RATE = .1f;
        private const float STAMINA_REGEN_RATE = .2f;

        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;

        [SerializeField] private float jumpSpeed = 8f;

        [SerializeField] private float reachKneeSpeed = 7;
        [SerializeField] private float reachArmSpeed = 4;

        [SerializeField] private float rotationSpeed = 720f;

        [SerializeField] private float gravity;

        [SerializeField] private CharacterCapsule capsule;
        [SerializeField] private CharacterMover mover;

        [SerializeField] private GroundDetector groundDetector;

        private const float minVerticalSpeed = -12f;

        // Allowed time before the character is set to ungrounded from the last time he was safely grounded.
        private const float timeBeforeUngrounded = 0.02f;

        // Speed along the character local up direction.
        private float verticalSpeed = 0f;

        // Time after which the character should be considered ungrounded.
        private float nextUngroundedTime = -1f;

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private LayerMask jumpLayer;

        private Collider[] overlaps = new Collider[5];

        private int overlapCount;

        private MoveContact[] moveContacts = CharacterMover.NewMoveContactArray;

        private int contactCount;

        private bool isOnMovingPlatform = false;

        private MovingPlatform movingPlatform;
        private Animator anim;
        [HideInInspector] static public bool allowMovement = true;
        [SerializeField] private AudioSource walkRunAS;
        [SerializeField] private AudioSource jumpAS;
        [SerializeField] private AudioSource landAS;
        [SerializeField] private Slider uiStaminaSlider;
        [SerializeField] private Image uiStaminaColor;
        [SerializeField] float bunnyHoppingSpeedBoost;

        private float lastJumpTime;
        private bool isReaching;
        private float stamina = 1;
        [HideInInspector] public bool isGrounded;
        private const float STEP_HEIGHT = .8f;
        private const float ARMS_HEIGHT = 1.5f;
        private float defaultJumpSpeed;
        private AbilityManager abilityManager;

        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public bool isRunning;

        private void Start()
        {
            mover.canClimbSteepSlope = true;
            anim = GetComponent<Animator>();
            defaultJumpSpeed = jumpSpeed;
            abilityManager = GetComponent<AbilityManager>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            Vector3 movementInput = allowMovement ? GetMovementInput() : Vector3.zero;
            isRunning = velocity.magnitude > .1f && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

            if (isRunning)
                stamina -= STAMINA_DECAY_RATE * Time.deltaTime;
            else
                stamina += STAMINA_REGEN_RATE * Time.deltaTime;

            stamina = Mathf.Clamp(stamina, 0f, 1f);
            uiStaminaSlider.value = stamina;
            uiStaminaColor.enabled = stamina > 0;

            // Note: Uncomment the following to make the Stamina system work
            // if (isRunning && stamina == 0)
            //     isRunning = false;

            velocity = (isRunning ? runSpeed : walkSpeed) * movementInput;

            HandleOverlaps();

            bool wasGrounded = isGrounded;
            bool groundDetected = DetectGroundAndCheckIfGrounded(out isGrounded, out GroundInfo groundInfo);

            if (isGrounded && !wasGrounded)
                landAS.Play();

            // SetGroundedIndicatorColor(isGrounded);

            isOnMovingPlatform = false;

            if (isGrounded && allowMovement && Input.GetButton("Jump"))
            {
                verticalSpeed = jumpSpeed;
                nextUngroundedTime = -1f;
                isGrounded = false;
                lastJumpTime = Time.time;
                jumpAS.Play();
                velocity *= bunnyHoppingSpeedBoost;

                if (abilityManager.usingJumpAbility)
                {
                    if (GlobalManager.Instance != null) GlobalManager.Instance.instrJumpAbilityUsed++;
                    abilityManager.TurnOffJumpAbility();
                }
            }

            if (isGrounded)
            {
                mover.mode = CharacterMover.Mode.Walk;
                verticalSpeed = 0f;

                if (groundDetected)
                    isOnMovingPlatform = groundInfo.collider.TryGetComponent(out movingPlatform);
            }
            else
            {
                mover.mode = CharacterMover.Mode.SimpleSlide;

                BounceDownIfTouchedCeiling();

                verticalSpeed += gravity * deltaTime;

                if (verticalSpeed < minVerticalSpeed)
                    verticalSpeed = minVerticalSpeed;

                velocity += verticalSpeed * transform.up;
            }

            HandleReach();
            HandlePlayerAnimations();
            HandlePlayerAudio();

            mover.Move(velocity * deltaTime, groundDetected, groundInfo, overlapCount, overlaps, moveContacts, out contactCount);
        }

        private void LateUpdate()
        {
            if (isOnMovingPlatform)
                ApplyPlatformMovement(movingPlatform);
        }

        private void HandlePlayerAnimations()
        {
            anim.SetFloat("Velocity", velocity.magnitude);
            anim.SetBool("isRunning", isRunning);
            anim.SetBool("onAir", !isGrounded);
        }

        private void HandlePlayerAudio()
        {
            if (!allowMovement || !isGrounded || velocity == Vector3.zero)
            {
                if (walkRunAS.isPlaying) walkRunAS.Stop();
                return;
            }

            // It is grounded!
            if (!isRunning)
            {
                walkRunAS.pitch = .8f;
                walkRunAS.volume = .55f;
                if (!walkRunAS.isPlaying) walkRunAS.Play();
            }
            else
            {
                walkRunAS.pitch = 1.05f;
                walkRunAS.volume = .67f;
                if (!walkRunAS.isPlaying) walkRunAS.Play();
            }
        }

        private Vector3 GetMovementInput()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized;
            Vector3 right = Vector3.Cross(transform.up, forward);

            return x * right + y * forward;
        }

        private void HandleOverlaps()
        {
            if (capsule.TryResolveOverlap())
            {
                overlapCount = 0;
            }
            else
            {
                overlapCount = capsule.CollectOverlaps(overlaps);
            }
        }

        private bool DetectGroundAndCheckIfGrounded(out bool isGrounded, out GroundInfo groundInfo)
        {
            bool groundDetected = groundDetector.DetectGround(out groundInfo);

            if (groundDetected)
            {
                if (groundInfo.isOnFloor && verticalSpeed < 0.1f)
                    nextUngroundedTime = Time.time + timeBeforeUngrounded;
            }
            else
                nextUngroundedTime = -1f;

            isGrounded = Time.time < nextUngroundedTime;
            return groundDetected;
        }

        private void ApplyPlatformMovement(MovingPlatform movingPlatform)
        {
            GetMovementFromMovingPlatform(movingPlatform, out Vector3 movement, out float upRotation);

            transform.Translate(movement, Space.World);
            transform.Rotate(0f, upRotation, 0f, Space.Self);
        }

        private void GetMovementFromMovingPlatform(MovingPlatform movingPlatform, out Vector3 movement, out float deltaAngleUp)
        {
            movingPlatform.GetDeltaPositionAndRotation(out Vector3 platformDeltaPosition, out Quaternion platformDeltaRotation);
            Vector3 localPosition = transform.position - movingPlatform.transform.position;
            movement = platformDeltaPosition + platformDeltaRotation * localPosition - localPosition;

            platformDeltaRotation.ToAngleAxis(out float platformDeltaAngle, out Vector3 axis);
            float axisDotUp = Vector3.Dot(axis, transform.up);

            if (-0.1f < axisDotUp && axisDotUp < 0.1f)
                deltaAngleUp = 0f;
            else
                deltaAngleUp = platformDeltaAngle * Mathf.Sign(axisDotUp);
        }

        private void BounceDownIfTouchedCeiling()
        {
            for (int i = 0; i < contactCount; i++)
            {
                if (Vector3.Dot(moveContacts[i].normal, transform.up) < -0.7f)
                {
                    verticalSpeed = -0.25f * verticalSpeed;
                    break;
                }
            }
        }

        private void HandleReach()
        {
            // Reset flag when back to ground
            if (isReaching && isGrounded)
                isReaching = false;

            Vector3 kneeOrigin = transform.position + Vector3.up * STEP_HEIGHT;
            Vector3 armOrigin = transform.position + Vector3.up * ARMS_HEIGHT;

            // Debug.DrawLine(kneeOrigin, kneeOrigin + transform.forward * 1, Color.red);
            // Debug.DrawLine(armOrigin, armOrigin + transform.forward * 1, Color.blue);

            if (Time.time - lastJumpTime > .3f && Input.GetButton("Jump") && !isGrounded && !isReaching)
            {
                if (Physics.Raycast(kneeOrigin, transform.forward, out RaycastHit hit1, 1, jumpLayer))
                {
                    verticalSpeed = reachKneeSpeed;
                    nextUngroundedTime = -1f;
                    lastJumpTime = Time.time;
                    isReaching = true;
                    jumpAS.Play();
                }
                else if (Physics.Raycast(armOrigin, transform.forward, out RaycastHit hit2, 1, jumpLayer))
                {
                    verticalSpeed = reachArmSpeed;
                    nextUngroundedTime = -1f;
                    lastJumpTime = Time.time;
                    isReaching = true;
                    jumpAS.Play();
                }
            }
        }

        public void DisableMovement()
        {
            allowMovement = false;
        }

        public void EnableMovement()
        {
            allowMovement = true;
        }

        public void UpdateJumpSpeed(float speed)
        {
            jumpSpeed = speed;
        }

        public float GetDefaultJumpSpeed()
        {
            return defaultJumpSpeed;
        }
    }
}
