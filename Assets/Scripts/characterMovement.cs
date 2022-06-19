using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterMovement : MonoBehaviour
{
    public CharacterController controller;
    public Camera moveCamera;
    public float smooth = 10f;
    Animator animator;
    PlayerInput input;

    // variable to store optimized setter/getter parameter IDs
    int isWalkingForwardHash;
    int isRunningHash;

    // variables to store player input values
    bool movementForwardPressed;
    bool runPressed;
    Vector2 lookValue;

    // variables to store parameter values from animator
    bool isWalkingForward;
    bool isRunning;

    // Awake is called when the script instance is being loaded
    void Awake() {
        input = new PlayerInput();

        input.CharacterControls.Movement.performed += ctx => movementForwardPressed = ctx.ReadValueAsButton();
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
        isWalkingForwardHash = Animator.StringToHash("isWalkingForward");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    void Update()
    {
        // get parameter values from animator
        isRunning = animator.GetBool(isRunningHash);
        isWalkingForward = animator.GetBool(isWalkingForwardHash);

        handleMovement();
        handleRotation();
    }

    void handleRotation() {
        Vector3 direction = new Vector3(lookValue.x, 0f, lookValue.y).normalized;

        if (direction.magnitude >= 0.1f && (isWalkingForward || isRunning))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, moveCamera.transform.eulerAngles.y, 0), Time.deltaTime * smooth);
        }
    }

    void handleMovement() {
        // start walking if movement pressed is true and not already walking
        if (movementForwardPressed && !isWalkingForward) {
            animator.SetBool(isWalkingForwardHash, true);
        }

        // stop walking if movementPressed is false and currently walking
        if (!movementForwardPressed && isWalkingForward) {
            animator.SetBool(isWalkingForwardHash, false);
        }

        // start running if movement pressed and run pressed is true and not already running
        if ((movementForwardPressed && runPressed) && !isRunning) {
            animator.SetBool(isRunningHash, true);
        }

        // stop running if movement pressed and run pressed is false and currently running
        if ((!movementForwardPressed || !runPressed) && isRunning) {
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
