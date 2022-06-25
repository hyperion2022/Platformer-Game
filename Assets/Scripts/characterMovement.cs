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

    // Awake is called when the script instance is being loaded
    void Awake() {
        input = new PlayerInput();

        input.CharacterControls.Movement.performed += ctx =>
        {
            inputDirection = ctx.ReadValue<Vector2>();
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

    void handleRotation() {
        Vector3 direction = new Vector3(lookValue.x, 0f, lookValue.y).normalized;

        if (direction.magnitude >= 0.1f && movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, moveCamera.transform.eulerAngles.y, 0), Time.deltaTime * smooth);
        }
    }

    void handleMovement() {
        movement = new Vector3(inputDirection.x, inputDirection.y, 0.0f);

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);
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
