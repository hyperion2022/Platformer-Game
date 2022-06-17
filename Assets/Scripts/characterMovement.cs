using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterMovement : MonoBehaviour
{
    // variable to store character animator component
    Animator animator;

    // variable to store optimized setter/getter parameter IDs
    int isWalkingHash;
    int isRunningHash;

    public Camera moveCamera;
    public float mouseSensitivity = .001f;

    // variable to store the instance of the PlayerInput
    PlayerInput input;

    // variables to store player input values
    bool movementPressed;
    bool runPressed;
    Vector2 lookValue;
    float xLook = 0;

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
        handleMovement();
        handleRotation();
    }

    void handleRotation() {
        // xLook is a float for the up/down angle of the moveCamera, so probably 0
        // Mouse sensitivity is also a float, but is between 0 and 1

        // turn whole body left and right
        transform.Rotate(Vector3.up * lookValue.x * mouseSensitivity);
        // turn just moveCamera up and down, negative value to un-invert it.
        xLook -= lookValue.y * mouseSensitivity;
        // clamp to look straight up or down, rather than behind the player
        xLook = Mathf.Clamp(xLook, -90, 90);
        // set rotation
        moveCamera.transform.localRotation = Quaternion.Euler(xLook, 0f, 0f);
    }

    void handleMovement() {
        // get parameter values from animator
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);

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
