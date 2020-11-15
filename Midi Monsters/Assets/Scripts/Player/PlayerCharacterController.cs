using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(AudioSource))]
public class PlayerCharacterController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;
    [Tooltip("Audio source for footsteps, etc...")]
    public AudioSource audioSource;
    [Tooltip("You can't run and you can't hide...")]
    public Monster monster;
    [SerializeField]
    private TextMeshProUGUI interactText = null;
    public Animator fadeOutAnim = null;

    [Header("General")]
    [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;
    [Tooltip("Time in seconds before reloading scene")]
    public float deathDelay = 1;

    [Header("Movement")]
    [Tooltip("Max movement speed when not sprinting")]
    public float maxSpeed = 10f;
    [Tooltip("Sharpness for the movement, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float movementSharpness = 15;
    [Tooltip("Max movement speed when crouching")]
    [Range(0, 1)]
    public float maxSpeedCrouchedRatio = 0.5f;
    [Tooltip("Multiplicator for the sprint speed")]
    public float sprintSpeedModifier = 2f;
    [Tooltip("When the monster predicts our future position based on our current movement, how many seconds ahead?")]
    public float predictedPositionSeconds = 0.5f;

    [Header("Interaction Settings")]
    [Tooltip("How far away we can interact with an object.")]
    public float interactRange = 4f;

    [Header("Stance")]
    [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
    public float cameraHeightRatio = 0.9f;
    [Tooltip("Height of character when standing")]
    public float capsuleHeightStanding = 1.8f;
    [Tooltip("Height of character when crouching")]
    public float capsuleHeightCrouching = 0.9f;
    [Tooltip("Speed of crouching transitions")]
    public float crouchingSharpness = 10f;

    [Header("Audio")]
    [Tooltip("Amount of footstep sounds played when moving one meter")]
    public float footstepSFXFrequency = 1f;
    [Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
    public float footstepSFXFrequencyWhileSprinting = 1f;
    [Tooltip("Volume when talking a crouched step for the purposes of AI detection.")]
    public float crouchFootstepDetectionVolume = 1f;
    [Tooltip("Volume when talking a normal step for the purposes of AI detection.")]
    public float normalFootstepDetectionVolume = 2f;
    [Tooltip("Volume when talking a sprint step for the purposes of AI detection.")]
    public float sprintFootstepDetectionVolume = 4f;

    public UnityAction<bool> onStanceChanged;

    public Vector3 characterVelocity { get; set; }
    public bool isCrouching { get; private set; }
    public bool isDead { get; private set; }

    PlayerInputHandler m_InputHandler;
    CharacterController m_Controller;
    Vector3 m_CharacterVelocity;
    float m_CameraVerticalAngle = 0f;
    float m_footstepDistanceCounter;
    float m_TargetCharacterHeight;

    void Start()
    {
        // fetch components on the same gameObject
        m_Controller = GetComponent<CharacterController>();
        m_InputHandler = GetComponent<PlayerInputHandler>();

        m_Controller.enableOverlapRecovery = true;

        // force the crouch state to false when starting
        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);

        // disable interaction prompt until we need it.
        interactText.text = "press F to interact";
        interactText.enabled = false;
    }

    void Update()
    {
        // crouching
        if (m_InputHandler.GetCrouchInputDown())
        {
            SetCrouchingState(!isCrouching, false);
        }

        HandleCameraMovement();

        CheckInteraction();
    }

    private void FixedUpdate()
    {
        UpdateCharacterHeight(false);

        HandleCharacterMovement();
    }

    void HandleCameraMovement()
    {
        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal()), 0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical();

            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
        }
    }

    void HandleCharacterMovement()
    {
        // character movement handling
        bool isSprinting = m_InputHandler.GetSprintInputHeld();
        {
            if (isSprinting)
            {
                isSprinting = SetCrouchingState(false, false);
            }

            float speedModifier = isSprinting ? sprintSpeedModifier : 1f;

            // converts move input to a worldspace vector based on our character's transform orientation
            Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

            // handle movement
            // calculate the desired velocity from inputs, max speed, and current slope
            Vector3 targetVelocity = worldspaceMoveInput * maxSpeed * speedModifier;
            // reduce speed if crouching by crouch speed ratio
            if (isCrouching)
                targetVelocity *= maxSpeedCrouchedRatio;
            targetVelocity = targetVelocity.normalized * targetVelocity.magnitude;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpness * Time.deltaTime);

            // footsteps sound
            float chosenFootstepSFXFrequency = (isSprinting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency);
            if (m_footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency)
            {
                m_footstepDistanceCounter = 0f;

                PlayFootstep(isSprinting);
            }

            // keep track of distance traveled for footsteps sound
            m_footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;

            // apply the gravity to the velocity
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }

        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);
        m_Controller.Move(characterVelocity * Time.deltaTime);

        // detect obstructions to adjust velocity accordingly
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius, characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            characterVelocity = Vector3.ProjectOnPlane(characterVelocity, hit.normal);
        }
    }

    private void PlayFootstep(bool isSprinting)
    {
        float detectionVolume = normalFootstepDetectionVolume;
        if (isSprinting)
        {
            detectionVolume = sprintFootstepDetectionVolume;
        }
        else if (isCrouching)
        {
            detectionVolume = crouchFootstepDetectionVolume;
        }

        // send sound event to monster
        Vector3 predictedPosition = transform.position + (characterVelocity * predictedPositionSeconds); // Where are we estimated to be predictedPositionSeconds later?
        monster.DetectSound(new Monster.DetectedSound(transform.position, predictedPosition, detectionVolume));
    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule    
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    void UpdateCharacterHeight(bool force)
    {
        // Update height instantly
        if (force)
        {
            m_Controller.height = m_TargetCharacterHeight;
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * cameraHeightRatio;
        }
        // Update smooth height
        else if (m_Controller.height != m_TargetCharacterHeight)
        {
            // resize the capsule and adjust camera position
            m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight, crouchingSharpness * Time.deltaTime);
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * m_TargetCharacterHeight * cameraHeightRatio, crouchingSharpness * Time.deltaTime);
        }
    }

    // returns false if there was an obstruction
    bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {
        // set appropriate heights
        if (crouched)
        {
            m_TargetCharacterHeight = capsuleHeightCrouching;
        }
        else
        {
            // Detect obstructions
            if (!ignoreObstructions)
            {
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere(capsuleHeightStanding),
                    m_Controller.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);
                foreach (Collider c in standingOverlaps)
                {
                    if (c != m_Controller)
                    {
                        return false;
                    }
                }
            }

            m_TargetCharacterHeight = capsuleHeightStanding;
        }

        if (onStanceChanged != null)
        {
            onStanceChanged.Invoke(crouched);
        }

        isCrouching = crouched;
        return true;
    }

    private void CheckInteraction()
    {
        interactText.enabled = false;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactRange))
        {
            Interactable subject = hit.collider.gameObject.GetComponent<Interactable>();
            if (subject != null && subject.IsInteractable())
            {
                interactText.text = subject.InteractionPrompt;
                interactText.enabled = true;

                if (m_InputHandler.GetInteractDown())
                {
                    subject.Interact();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) // magic number :(
        {
            StartCoroutine(Die());
        }
    }

    // fukken die
    public IEnumerator Die()
    {
        // Play death sound and fade out screen
        FindObjectOfType<GameAudioInterface>().PlayMIDISfx(GameAudioInterface.MIDISfx.PlayerDeath);
        fadeOutAnim.speed = 1f / deathDelay;
        fadeOutAnim.SetBool("FadeOut", true);

        isDead = true;

        yield return new WaitForSeconds(deathDelay);

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
