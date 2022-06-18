using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float smooth = 10f;
    public Camera moveCamera;

    // variable to store character animator component
    Animator animator;

    // variable to store optimized setter/getter parameter IDs
    int isWalkingHash;
    int isRunningHash;

    // variable to store the instance of the PlayerInput
    PlayerInput input;

    // variables to store player input values
    bool movementPressed;
    bool runPressed;
    bool isWalking;
    bool isRunning;
    Vector2 lookValue;

    // Awake is called when the script instance is being loaded
    void Awake() {
        input = new PlayerInput();

        input.CharacterControls.Movement.performed += ctx => movementPressed = ctx.ReadValueAsButton();
        input.CharacterControls.Run.performed += ctx => runPressed = ctx.ReadValueAsButton();
        input.CharacterControls.Rotate.performed += ctx => {
            lookValue = ctx.ReadValue<Vector2>();
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        // set the animator reference
        animator = GetComponent<Animator>();

        // set the ID references
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    void Update()
    {
        // get parameter values from animator
        isRunning = animator.GetBool(isRunningHash);
        isWalking = animator.GetBool(isWalkingHash);

        handleMovement();
        handleRotation();
    }

    void handleRotation() {
        Vector3 direction = new Vector3(lookValue.x, 0f, lookValue.y).normalized;

        if (direction.magnitude >= 0.1f && (isWalking || isRunning))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, moveCamera.transform.eulerAngles.y, 0), Time.deltaTime * smooth);
        }
    }

    void handleMovement() {
        // start walking if movement pressed is true and not already walking
        if (movementPressed && !isWalking) {
            animator.SetBool(isWalkingHash, true);
        }

        // stop walking if movementPressed is false and currently walking
        if (!movementPressed && isWalking) {
            animator.SetBool(isWalkingHash, false);
        }

        // start running if movement pressed and run pressed is true and not already running
        if ((movementPressed && runPressed) && !isRunning) {
            animator.SetBool(isRunningHash, true);
        }

        // stop running if movement pressed and run pressed is false and currently running
        if ((!movementPressed || !runPressed) && isRunning) {
            animator.SetBool(isRunningHash, false);
        }
    }

    void OnEnable() {
        // enable the character controls action map
        input.CharacterControls.Enable();
    }

    void OnDisable() {
        // disable the character controls action map
        input.CharacterControls.Disable();
    }
}
