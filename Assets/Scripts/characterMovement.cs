using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterMovement : MonoBehaviour
{
    public CharacterController controller;
    public Camera moveCamera;
    public float smoothRotation = 10f;
    Animator animator;
    PlayerInput input;

    // variables to store player input values
    Vector2 inputDirection;
    Vector3 movement;
    Vector2 lookValue;

    // smooth dampening variables
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;
    [SerializeField]
    private float smoothInputSpeed = .2f;

    // variable to store optimized setter/getter parameter IDs
    int isRunningHash;
    int isCrouchingHash;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        input = new PlayerInput();

        input.CharacterControls.Movement.performed += ctx =>
        {
            // read WASD as Vector2
            inputDirection = ctx.ReadValue<Vector2>();
        };
        input.CharacterControls.Movement.canceled += _ =>
        {
            // no input => no movement
            inputDirection = Vector2.zero;
        };
        input.CharacterControls.Rotate.performed += ctx =>
        {
            // read Mouse Position as Vector2
            lookValue = ctx.ReadValue<Vector2>();
        };
        input.CharacterControls.Run.performed += ctx =>
        {
            // if crouching, uncrouch
            if (animator.GetBool(isCrouchingHash))
            {
                animator.SetBool(isCrouchingHash, false);
            }
            // start running
            animator.SetBool(isRunningHash, true);
        };
        input.CharacterControls.Run.canceled += ctx =>
        {
            // stop running
            animator.SetBool(isRunningHash, false);
        };
        input.CharacterControls.Crouch.performed += ctx =>
        {
            // if not running, crouch or uncrouch
            if (!animator.GetBool(isRunningHash))
            {
                animator.SetBool(isCrouchingHash, !animator.GetBool(isCrouchingHash));
            }
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        // set the animator reference
        animator = GetComponent<Animator>();

        // set the ID references
        isRunningHash = Animator.StringToHash("isRunning");
        isCrouchingHash = Animator.StringToHash("isCrouching");
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        handleRotation();
    }

    void handleMovement()
    {
        // direction vector smoothening
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputDirection, ref smoothInputVelocity, smoothInputSpeed);
        movement = new Vector3(currentInputVector.x, currentInputVector.y, 0.0f);

        // set parameter values in animator
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }

    void handleRotation()
    {
        Vector3 direction = new Vector3(lookValue.x, 0f, lookValue.y).normalized;

        // if player is moving, change direction using the mouse
        if (direction.magnitude >= 0.1f && movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, moveCamera.transform.eulerAngles.y, 0), Time.deltaTime * smoothRotation);
        }
    }

    void OnEnable()
    {
        // enable the character controls action map
        input.CharacterControls.Enable();
    }

    void OnDisable()
    {
        // disable the character controls action map
        input.CharacterControls.Disable();
    }
}
