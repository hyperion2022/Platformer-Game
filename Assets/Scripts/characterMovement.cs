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
    [SerializeField] private GameObject StaminaBar;

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

    // stats scripts references
    CurrencyCount currencyScript;
    HealthBar healthScript;
    StaminaBar staminaScript;
    private bool consumesStamina;

    // variable to store stat timing
    private float timeSinceLastDangerousCollision;

    // Awake is called when the script instance is being loaded
    private void Awake()
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
            // start running
            consumesStamina = true;
            
            // if crouching, uncrouch
            if (animator.GetBool(isCrouchingHash))
            {
                animator.SetBool(isCrouchingHash, false);
            }

            // start running
            animator.SetBool(isRunningHash, true);

            staminaScript.showFatigueMessage();
        };
        input.CharacterControls.Run.canceled += ctx =>
        {
            // stop running
            consumesStamina = false;
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
    private void Start()
    {
        // set the animator reference
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // set the ID references
        isRunningHash = Animator.StringToHash("isRunning");
        isCrouchingHash = Animator.StringToHash("isCrouching");

        // set the stats references
        currencyScript = CurrencyText.GetComponent<CurrencyCount>();
        healthScript = HealthBar.GetComponent<HealthBar>();
        staminaScript = StaminaBar.GetComponent<StaminaBar>();
        timeSinceLastDangerousCollision = Time.time;
        consumesStamina = false;
    }

    // Update is called once per frame
    private void Update()
    {
        handleMovement();
        handleRotation();
        handleStamina();
    }

    private void handleMovement()
    {
        // direction vector smoothening
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputDirection, ref smoothInputVelocity, smoothInputSpeed);
        movement = new Vector3(currentInputVector.x, currentInputVector.y, 0.0f);

        // set parameter values in animator
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }

    private void handleRotation()
    {
        Vector3 direction = new Vector3(lookValue.x, 0f, lookValue.y).normalized;

        // if player is moving, change direction using the mouse
        if (direction.magnitude >= 0.1f && movement.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, activeCamera.transform.eulerAngles.y, 0), Time.deltaTime * smoothRotation);
        }
    }

    private void handleStamina()
    {
        if (consumesStamina && staminaScript.getStamina() > 0f)
        {
            // update the stamina to decrease by a factor of 5
            // we subtract the time passed since the last run started * 5
            staminaScript.setStamina(staminaScript.getStamina() - Time.deltaTime * 5f);

            if (staminaScript.getStamina() < 1f)
            {
                // if stamina is < 1,
                // show the fatigue text on screen
                staminaScript.setStamina(0f);
            }
        }
        else
        {
            // if we don't consume stamina or we have none to consume,
            // we stop consuming it and the running animation
            consumesStamina = false;
            animator.SetBool(isRunningHash, false);
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


    // for kinematic objects
    private void OnTriggerEnter(Collider other)
    {
        // If we collide with a green gem, we add 1 to currency
        if (other.gameObject.CompareTag("Green Gem"))
        {
            other.gameObject.SetActive(false);
            currencyScript.setCount(currencyScript.getCount() + 1);
        }
        // If we collide with a silver gem, we add 10 to currency
        else if (other.gameObject.CompareTag("Silver Gem"))
        {
            other.gameObject.SetActive(false);
            currencyScript.setCount(currencyScript.getCount() + 10);
        }
        // If we collide with a gold gem, we add 50 to currency
        else if (other.gameObject.CompareTag("Gold Gem"))
        {
            other.gameObject.SetActive(false);
            currencyScript.setCount(currencyScript.getCount() + 50);
        }
        // If we collide with a bandage roll, we add 20 to health
        else if (other.gameObject.CompareTag("Bandages"))
        {
            other.gameObject.SetActive(false);
            healthScript.setHealth(healthScript.getHealth() + 20);
        }
        // If we collide with a medkit, we add 100 to health
        else if (other.gameObject.CompareTag("Medkit"))
        {
            other.gameObject.SetActive(false);
            healthScript.setHealth(healthScript.getHealth() + 100);
        }
        // If we collide with an energy drinkl, we add 10 to stamina
        else if (other.gameObject.CompareTag("Energy Drink"))
        {
            other.gameObject.SetActive(false);
            staminaScript.setStamina(staminaScript.getStamina() + 10);
        }
        // If we collide with a syringe, we add 50 to stamina
        else if (other.gameObject.CompareTag("Syringe"))
        {
            other.gameObject.SetActive(false);
            staminaScript.setStamina(staminaScript.getStamina() + 50);
        }
    }

    // for non-kinematic objects
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If we collide with a cactus and 0.5s passed since out last dangerous collision,
        // we update the time and subtract 10 from health
        if (hit.gameObject.CompareTag("Cactus") && (Time.time - timeSinceLastDangerousCollision) > 0.5f)
        {
            timeSinceLastDangerousCollision = Time.time;
            healthScript.setHealth(healthScript.getHealth() - 10);
        }
    }
}
