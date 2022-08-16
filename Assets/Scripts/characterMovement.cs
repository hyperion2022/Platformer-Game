using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterMovement : MonoBehaviour
{
    [SerializeField] private Camera activeCamera;
    [SerializeField] private GameObject[] _vCams = new GameObject[2];
    [SerializeField] private float smoothRotation = 10f;
    [SerializeField] private GameObject CurrencyText;
    [SerializeField] private GameObject HealthBar;

    private Animator animator;
    private CharacterController characterController;
    private PlayerInput input;

    // variables to store player input values
    private Vector2 inputDirection;
    private Vector3 movement;
    private Vector2 lookValue;

    // smooth dampening variables
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;
    [SerializeField] private float smoothInputSpeed = .2f;

    // variable to store optimized setter/getter parameter IDs
    private int isRunningHash;
    private int isCrouchingHash;

    CurrencyCount currencyScript;
    HealthBar healthScript;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        input = new PlayerInput();

        input.CharacterControls.Movement.started += ctx =>
        {
            // read WASD as Vector2
            inputDirection = ctx.ReadValue<Vector2>();
        };
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
        input.CharacterControls.Rotate.started += ctx =>
        {
            // read Mouse Position as Vector2
            lookValue = ctx.ReadValue<Vector2>();
        };
        input.CharacterControls.Run.started += ctx =>
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
        input.CharacterControls.Crouch.started += ctx =>
        {
            // if not running, crouch or uncrouch
            if (!animator.GetBool(isRunningHash))
            {
                animator.SetBool(isCrouchingHash, !animator.GetBool(isCrouchingHash));
            }
        };
        input.CharacterControls.Look.started += ctx =>
        {
            // deactivate main camera
            // _vCams[0] -> move camera, _vCams[1] -> look camera
            _vCams[0].SetActive(false);
            _vCams[1].SetActive(true);

            input.CharacterControls.Disable();
            input.LookCameraControls.Enable();
        };
        input.LookCameraControls.Exit.started += ctx =>
        {
            // activate main camera
            // _vCams[0] -> move camera, _vCams[1] -> look camera
            _vCams[1].SetActive(false);
            _vCams[0].SetActive(true);

            input.LookCameraControls.Disable();
            input.CharacterControls.Enable();
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        // set the animator reference
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // set the ID references
        isRunningHash = Animator.StringToHash("isRunning");
        isCrouchingHash = Animator.StringToHash("isCrouching");

        currencyScript = CurrencyText.GetComponent<CurrencyCount>();
        healthScript = HealthBar.GetComponent<HealthBar>();
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
        if (direction.magnitude >= 0.1f && movement.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, activeCamera.transform.eulerAngles.y, 0), Time.deltaTime * smoothRotation);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Green Gem"))
        {
            other.gameObject.SetActive(false);
            currencyScript.setCount(currencyScript.getCount() + 1);
        }
        else if (other.gameObject.CompareTag("Silver Gem"))
        {
            other.gameObject.SetActive(false);
            currencyScript.setCount(currencyScript.getCount() + 10);
        }
        else if (other.gameObject.CompareTag("Gold Gem"))
        {
            other.gameObject.SetActive(false);
            currencyScript.setCount(currencyScript.getCount() + 50);
        }
        else if (other.gameObject.CompareTag("Bandages"))
        {
            other.gameObject.SetActive(false);
            healthScript.setHealth(healthScript.getHealth() + 20);
        }
        else if (other.gameObject.CompareTag("Medkit"))
        {
            other.gameObject.SetActive(false);
            healthScript.setHealth(healthScript.getHealth() + 100);
        }
    }
}
