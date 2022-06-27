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

    // variables to store player input values
    Vector2 inputDirection;
    Vector3 movement;
    Vector2 lookValue;

    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;
    [SerializeField]
    private float smoothInputSpeed = .2f;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        input = new PlayerInput();

        input.CharacterControls.Movement.performed += ctx =>
        {
            inputDirection = ctx.ReadValue<Vector2>();
        };
        input.CharacterControls.Movement.canceled += _ =>
        {
            inputDirection = Vector2.zero;
        };
        input.CharacterControls.Rotate.performed += ctx => {
            lookValue = ctx.ReadValue<Vector2>();
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        // set the animator reference
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        handleRotation();
    }

    void handleRotation()
    {
        Vector3 direction = new Vector3(lookValue.x, 0f, lookValue.y).normalized;

        if (direction.magnitude >= 0.1f && movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, moveCamera.transform.eulerAngles.y, 0), Time.deltaTime * smooth);
        }
    }

    void handleMovement()
    {
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputDirection, ref smoothInputVelocity, smoothInputSpeed);
        movement = new Vector3(currentInputVector.x, currentInputVector.y, 0.0f);

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
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
