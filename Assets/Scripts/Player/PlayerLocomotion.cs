using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerCharacter playerCharacter;
    InputManager inputManager;
    [Header("Autio")]
    [SerializeField] private AudioManager audioManager;
    [Space]
    [Header("Movement")]
    [SerializeField] private PlayerMoveData movementData;
    [SerializeField] private Vector2 moveDirection;
    private Rigidbody2D playerRigidBody;
    [Space]
    [Header("Animation")]
    [SerializeField] private Animator animator;


    [Header("Timers")]
    [SerializeField] private float lastOnGroundTime;
    [SerializeField] private float lastOnWallTime;
    [SerializeField] private float lastOnWallRightTime;
    [SerializeField] private float lastOnWallLeftTime;

    public float LastPressedJumpTime { get; private set; }


    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    [Header("Checks")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isFacingRight;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isJumpCut;
    [SerializeField] private bool isJumpFalling;




    [Header("Collision Checks")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);




    private void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        inputManager = GetComponent<InputManager>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
    }
    private void Start()
    {
        SetGravityScale(movementData.gravityScale);
        isFacingRight = true;
    }
    private void Update()
    {
        HandleTimers();
        if (inputManager.horizontalInput != 0)
            CheckDirectionToFace(inputManager.horizontalInput > 0);

        HandleCollisionChecks();
        HandleJumpChecks();

    }
    private void FixedUpdate()
    {

        HandleAllMovement();
        HandleFallingAndLanding();
    }
    public void HandleAllMovement()
    {


        if (playerCharacter.isInteracting)
        {
            return;
        }
        //Handle Run
        HandleRun(1);


    }

    private void HandleTimers()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;
        lastOnWallRightTime -= Time.deltaTime;
        lastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;

    }
    //MOVEMENT METHODS
    private void HandleRun(float lerpAmount)
    {
        if (!playerCharacter.isInteracting)
        {
            animator.SetFloat("Speed",Mathf.Abs(inputManager.horizontalInput));
            float targetSpeed = inputManager.horizontalInput * movementData.runMaxSpeed;
            targetSpeed = Mathf.Lerp(playerRigidBody.velocity.x, targetSpeed, lerpAmount);
            float accelerationRate;
            if (lastOnGroundTime > 0)
                accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? movementData.runAccelAmount : movementData.runDeccelAmount;
            else
                accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? movementData.runAccelAmount * movementData.accelInAir : movementData.runDeccelAmount * movementData.deccelInAir;

            if (movementData.doConserveMomentum && Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(playerRigidBody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
            {
                accelerationRate = 0;
            }

            float speedDif = targetSpeed - playerRigidBody.velocity.x;


            float movement = speedDif * accelerationRate;

            playerRigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);

        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }
    private void HandleJumpChecks()
    {
        if (isJumping && playerRigidBody.velocity.y < 0)
        {
            isJumping = false;

            isJumpFalling = true;
        }



        if (lastOnGroundTime > 0 && !isJumping)
        {
            isJumpCut = false;

            isJumpFalling = false;
        }
        //Jump
        if (CanJump() && LastPressedJumpTime > 0)
        {
            isJumping = true;

            isJumpCut = false;
            isJumpFalling = false;
            HandleJumping();
            animator.SetTrigger("Jump");
            audioManager.PlayOneShot("Jump");

        }
    }

    private void HandleFallingAndLanding()
    {


 
        if (isJumpCut)
        {

            SetGravityScale(movementData.gravityScale * movementData.jumpCutGravityMult);
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, Mathf.Max(playerRigidBody.velocity.y, -movementData.maxFallSpeed));
        }
        else if ((isJumping || isJumpFalling) && Mathf.Abs(playerRigidBody.velocity.y) < movementData.jumpHangTimeThreshold)
        {
            SetGravityScale(movementData.gravityScale * movementData.jumpHangGravityMult);
        }
        else if (playerRigidBody.velocity.y < 0)
        {
           
            SetGravityScale(movementData.gravityScale * movementData.fallGravityMult);
            
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, Mathf.Max(playerRigidBody.velocity.y, -movementData.maxFallSpeed));
        }
        else
        {

            SetGravityScale(movementData.gravityScale);
        }

        if (!isGrounded && !isJumpFalling)
        {
            animator.ResetTrigger("JumpFalling");
            animator.SetTrigger("Falling");
        }
        else if (!isGrounded && isJumpFalling)
        {
            animator.ResetTrigger("Falling");
            animator.SetTrigger("JumpFalling");
        }
        else
        {
            animator.ResetTrigger("Falling");
            animator.ResetTrigger("JumpFalling");
        }


    }
    
    public void HandleJumping()
    {

     
        LastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        
        float force = movementData.jumpForce;
        if (playerRigidBody.velocity.y < 0)
            force -= playerRigidBody.velocity.y;

        playerRigidBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);

    }



    #region INPUT CALLBACKS
    //Methods which whandle input detected in Update()
    public void OnJumpInput()
    {
        LastPressedJumpTime = movementData.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut())
            isJumpCut = true;
    }


    #endregion

    #region GENERAL METHODS
    //GENERAL METHODS
    private void Turn()
    {
     
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    public void SetGravityScale(float scale)
    {
        playerRigidBody.gravityScale = scale;
    }





    #endregion

    #region CHECK METHODS
    //CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping && isGrounded;
    }
    private bool CanJumpCut()
    {
        return isJumping && playerRigidBody.velocity.y > 0;
    }


    #endregion


    //COLLISION CHECKS
    private void HandleCollisionChecks()
    {
        if (!isJumping)
        {

            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, groundLayer)) 
            {

                
                if (lastOnGroundTime < -0.1f && !isGrounded)
                {
                    animator.ResetTrigger("JumpFalling");
                    animator.ResetTrigger("Falling");
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("Landed");
                    isGrounded = true;

                }
                else if (lastOnGroundTime < -0.1f && lastOnWallTime < -0.1f && !isGrounded)
                {
                    animator.ResetTrigger("JumpFalling");
                    animator.ResetTrigger("Falling");
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("Landed");
                    isGrounded = true;
                }
                else if (lastOnGroundTime < -0.1f && lastOnWallTime < -0.1f && isGrounded)
                {
                    animator.ResetTrigger("JumpFalling");
                    animator.ResetTrigger("Falling");
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("Landed");
                    isGrounded = true;
                }
                else if (lastOnGroundTime <= 0.1f && lastOnWallTime < -0.1f && !isGrounded)
                {
                    animator.ResetTrigger("JumpFalling");
                    animator.ResetTrigger("Falling");
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("Landed");
                    isGrounded = true;
                }
                else if (lastOnGroundTime <= 0.1f && lastOnWallTime <= 0.1f && !isGrounded)
                {
                    animator.ResetTrigger("JumpFalling");
                    animator.ResetTrigger("Falling");
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("Landed");
                    isGrounded = true;
                }
                else if (lastOnGroundTime < -0.1f && lastOnWallTime < -0.1f && isJumpCut && isJumping && isGrounded)
                {
                    animator.ResetTrigger("JumpFalling");
                    animator.ResetTrigger("Falling");
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("Landed");
                    isGrounded = true;
                }

                lastOnGroundTime = movementData.coyoteTime;
            }
            else
            {
                isGrounded = false;
            }


            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && isFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && !isFacingRight)))
                lastOnWallRightTime = movementData.coyoteTime;

           
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && !isFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && isFacingRight)))
                lastOnWallLeftTime = movementData.coyoteTime;

          
            lastOnWallTime = Mathf.Max(lastOnWallLeftTime, lastOnWallRightTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }

}
