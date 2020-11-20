using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(AudioSource))]
public class PlayerCharacterController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;
    [Tooltip("You can't run and you can't hide...")]
    public Monster monster;
    [SerializeField]
    private TextMeshProUGUI interactText = null;
    [SerializeField]
    private TextMeshProUGUI keyCountText = null;
    public TextMeshProUGUI controls = null;
    public Animator deathAnim = null;
    public Animator winAnim = null;
    public TextMeshProUGUI winText = null;
    public Button winButton = null;
    public LayerMask raycastLayerMask;
    public int monsterLayer = 8;
    public int endGameZoneLayer = 15;

    [Header("General")]
    [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;
    [Tooltip("Time in seconds before reloading scene")]
    public float deathDelay = 1;
    [Tooltip("Time in seconds before ending")]
    public float winDelay = 1;

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
    [Range(0,1), Tooltip("Volume to play volume when talking a crouched step.")]
    public float crouchFootstepAudioVolume = 0.33f;
    [Range(0, 1), Tooltip("Volume to play volume when talking a normal step.")]
    public float normalFootstepAudioVolume = 0.66f;
    [Range(0, 1), Tooltip("Volume to play volume when talking a sprint step.")]
    public float sprintFootstepAudioVolume = 1f;
    [Tooltip("AI detection multiplier when talking a crouched step.")]
    public float crouchFootstepDetectionVolume = 1f;
    [Tooltip("AI detection multiplier when talking a normal step.")]
    public float normalFootstepDetectionVolume = 2f;
    [Tooltip("AI detection multiplier when talking a sprint step.")]
    public float sprintFootstepDetectionVolume = 4f;

    private Dictionary<Key.KeyType, int> keys = new Dictionary<Key.KeyType, int>();

    public UnityAction<bool> onStanceChanged;

    public Vector3 characterVelocity { get; set; }

    public bool isCrouching { get; private set; }

    public bool isDead { get; private set; }

    PlayerInputHandler m_InputHandler;
    CharacterController m_Controller;
    Vector3 m_CharacterVelocity;
    private PlayerFootstepEmitter playerFootstepEmitter = null;
    float m_CameraVerticalAngle = 0f;
    float m_footstepDistanceCounter;
    float m_TargetCharacterHeight;

    private Canvas canvas;

    void Start()
    {
        // Add key types to key dictionary
        foreach (Key.KeyType key in Enum.GetValues(typeof(Key.KeyType)))
        {
            keys.Add(key, 0);
        }

        // fetch components on the same gameObject
        m_Controller = GetComponent<CharacterController>();
        m_InputHandler = GetComponent<PlayerInputHandler>();
        playerFootstepEmitter = GetComponent<PlayerFootstepEmitter>();

        m_Controller.enableOverlapRecovery = true;

        // force the crouch state to false when starting
        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);

        // disable interaction prompt until we need it.
        interactText.text = "You aren't supposed to see this text!";
        interactText.gameObject.SetActive(false);

        keyCountText.text = "0";
        keyCountText.gameObject.SetActive(false);
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

        if (m_InputHandler.GetQuitPressed())
        {
            Application.Quit();
        }
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
            Vector3 noY = new Vector3(characterVelocity.x, 0, characterVelocity.z);
            m_footstepDistanceCounter += noY.magnitude * Time.deltaTime;

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
        float audioVolume = normalFootstepAudioVolume;
        float detectionVolume = normalFootstepDetectionVolume;
        if (isSprinting)
        {
            audioVolume = sprintFootstepAudioVolume;
            detectionVolume = sprintFootstepDetectionVolume;
        }
        else if (isCrouching)
        {
            audioVolume = crouchFootstepAudioVolume;
            detectionVolume = crouchFootstepDetectionVolume;
        }
        
        playerFootstepEmitter.PlayFootstep(audioVolume);

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
        interactText.gameObject.SetActive(false);

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactRange, raycastLayerMask))
        {
            Interactable subject = hit.collider.gameObject.GetComponent<Interactable>();
            if (subject != null && subject.IsInteractable())
            {
                interactText.gameObject.SetActive(true);
                interactText.text = subject.GetInteractionPrompt();

                if (m_InputHandler.GetInteractDown())
                {
                    subject.Interact(this);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == monsterLayer) 
        {
            StartCoroutine(Die());
        }
        else if (other.gameObject.layer == endGameZoneLayer)
        {
            StartCoroutine(Win());
        }
    }

    // fukken die
    public IEnumerator Die()
    {
        // Play death sound and fade out screen
        FindObjectOfType<GameAudioInterface>().PlayMIDISfx(GameAudioInterface.MIDISfx.PlayerDeath);
        deathAnim.speed = 1f / deathDelay;
        deathAnim.SetBool("FadeOut", true);

        isDead = true;

        yield return new WaitForSeconds(deathDelay);

        // Reload scene
        Restart();
    }

    public IEnumerator Win()
    {
        // Play fade out screen
        winAnim.speed = 1f / deathDelay;
        winAnim.SetBool("FadeOut", true);

        isDead = true;

        yield return new WaitForSeconds(winDelay);

        // Stop monster
        winText.gameObject.SetActive(true);
        winButton.gameObject.SetActive(true);
        winButton.onClick.AddListener(Restart);
        m_InputHandler.SetCursor(true);
        monster.EndGame();
        monster.enabled = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    internal void AddKey(Key.KeyType keyType)
    {
        keys[keyType] += 1;
        if (keys[Key.KeyType.Normal] > 0) {
            keyCountText.text = keys[Key.KeyType.Normal].ToString();
            keyCountText.gameObject.SetActive(true);
        }
    }

    internal bool HasKey(Key.KeyType keyType)
    {
        return keys[keyType] > 0;
    }

    internal void UseKey(Key.KeyType keyType)
    {
        if (keys[keyType] > 0)
        {
            keys[keyType]--;
            keyCountText.text = keys[Key.KeyType.Normal].ToString();
            controls.enabled = false;
        }
        if (keys[Key.KeyType.Normal] == 0)
        {
            keyCountText.gameObject.SetActive(false);
        }
    }
}
