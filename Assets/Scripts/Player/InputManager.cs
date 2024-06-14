using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Managers")]
    public PlayerCharacter playerCharacter;
    public PlayerControls playerControls;
    public PlayerLocomotion playerLocomotion;

    [Space]
    [Header("Movement")]
    public Vector2 movementInput;
    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    [Space]
    [Header("Boolean Checks")]
    public bool jumpInput;
    public bool jumpUpInput;
    public bool interactInput;


    private void Awake()
    {

        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerCharacter = GetComponent<PlayerCharacter>();  

    }

    private void OnEnable()
    {

        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }

        playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        playerControls.PlayerMovement.Movement.canceled += i => movementInput = new Vector2(0, 0);
        playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
        playerControls.PlayerActions.Jump.canceled += i => jumpUpInput = true;
        playerControls.PlayerActions.Interact.performed += i => interactInput = true;
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleJumpInput();
        HandleInteractaInput();

    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
    }

    private void HandleJumpInput()
    {

        if (jumpInput)
        {
            playerLocomotion.OnJumpInput();
            jumpInput = false;

        }
        if (jumpUpInput)
        {
            playerLocomotion.OnJumpUpInput();
            jumpUpInput = false;

        }
    }

    private void HandleInteractaInput()
    {
        if (interactInput)
        {
            playerCharacter.TryInteract();
            interactInput = false;
        }
        
    }
}
