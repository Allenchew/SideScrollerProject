using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ControlData playerControlData;

    public Rigidbody2D rb;
    public LayerMask GroundMask;
    public LayerMask WallMask;
    public float DefaultGravityScale = 3;
    public float ControllerDeadzone = 0.01f;
    
    private Playercontrols controls;
    private float targetSpeed;
    private float speedDif;
    private float accelerateRate;
    private float movement;
    private int coyote = 0;
    private int dashFrameCount = 0;

    private void Awake()
    {
        controls = new();

    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = DefaultGravityScale;
        controls.Player.Dash.started += ApplyDash;
    }
    public void OnEnable()
    {
        controls.Player.Enable();
    }

    public void OnDisable()
    {
        controls.Player.Disable();
    }
    void Update()
    {
        
    }

    // TODO : Apply wall detection
    // TODO : Add Wall Jump
    // TODO : Coyote for wall jump
    // TODO : Tidy up and seperate Codes
    private void FixedUpdate()
    {
        Vector2 playerInput = controls.Player.Move.ReadValue<Vector2>();

        if (Mathf.Abs(playerInput.x) > 0 && !playerControlData.IsDashing)
        {
            ApplyRun(playerInput);
        }

        if (!playerControlData.IsDashing)
        {
            ApplyFriction();

        }

        if (!playerControlData.IsJumping && !playerControlData.IsDashing)
        {
            if (IsJumpKeyPress() && coyote < playerControlData.CoyoteByFrame) 
            {
                ApplyJump();
            }
            CheckCoyote();
        }

        if(playerControlData.IsJumping && IsOnGround() && !IsJumpKeyPress())
        {
            playerControlData.IsJumping = false;
        }

        if(playerControlData.IsJumping && !IsFalling() && !IsJumpKeyPress())
        {
            ApplyJumpCut();
        }

        if (playerControlData.IsDashing && dashFrameCount < playerControlData.DashFrame)
        {
            dashFrameCount++;
        }

        else if (playerControlData.IsDashing && dashFrameCount >= playerControlData.DashFrame)
        {
            playerControlData.IsDashing = false;
            StopDash();
        }

        if (IsFalling() && !playerControlData.IsDashing)
        {
            if (IsDownKeyPress())
            {
                SetGravityScale(playerControlData.FastFallMultipler);
            }
            else if ((IsAtLeftWall() && playerInput.x < 0) || (IsAtRightWall() && playerInput.x > 0))
            {
                SetGravityScale(playerControlData.OnWallFallGravity);
            }
            else
            {
                SetGravityScale(playerControlData.FallMultiplier);
            }

        }else if (playerControlData.IsDashing)
        {
            SetGravityScale(0);
        }
        else
        {
            SetGravityScale(1.0f);
        }
    }
    private void SetFacing()
    {
        if (targetSpeed > 0.1f)
        {
            playerControlData.FaceRight = true;
        }
        else if (targetSpeed < -0.1f)
        {
            playerControlData.FaceRight = false;
        }
    }
    private void ApplyRun(Vector2 controlInput)
    {
        targetSpeed = controlInput.x * playerControlData.RunMaxSpeed;
        speedDif = targetSpeed - rb.velocity.x;
        SetFacing();

        if (IsOnGround())
        {
            accelerateRate = (Math.Abs(targetSpeed) > ControllerDeadzone) 
                ? playerControlData.RunAccel 
                : playerControlData.RunDeccel;
        }
        else
        {
            accelerateRate = (Math.Abs(targetSpeed) > ControllerDeadzone) 
                ? playerControlData.RunAccel * playerControlData.AccelInAir 
                : playerControlData.RunDeccel * playerControlData.DeccelInAir;
        }
        movement = MathF.Pow(Math.Abs(speedDif) * accelerateRate,playerControlData.AccelPower) * Math.Sign(speedDif);
        if (!((IsAtLeftWall() && movement < 0) || (IsAtRightWall() && movement > 0)))
        {
            rb.AddForce(movement * Vector2.right);
        }
    }

    private void ApplyJump()
    {
        playerControlData.IsJumping = true;
        rb.AddForce(Vector2.up * playerControlData.JumpForce,ForceMode2D.Impulse);
    }

    private void CheckCoyote()
    {
        if (!IsOnGround() && coyote < playerControlData.CoyoteByFrame)
        {
            coyote++;
        }
        else if (IsOnGround())
        {
            coyote = 0;
        }
    }
    private void ApplyJumpCut()
    {
        rb.AddForce(Vector2.down * rb.velocity.y * (1 - playerControlData.JumpCutMultiplier), ForceMode2D.Impulse);
    }
    private void ApplyDash(InputAction.CallbackContext context)
    {
        if (!playerControlData.IsDashing)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            playerControlData.IsDashing = true;
            Vector2 dashDirection = playerControlData.FaceRight ? Vector2.right : Vector2.left;
            rb.AddForce(dashDirection * playerControlData.DashSpeed, ForceMode2D.Impulse);
        }
    }
    private void StopDash()
    {
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            Vector2 dashDirection = playerControlData.FaceRight ? Vector2.left : Vector2.right;
            rb.AddForce(dashDirection * playerControlData.StopSpeed, ForceMode2D.Impulse);
        }
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        dashFrameCount = 0;
    }
    
    private void ApplyFriction()
    {
        if(IsOnGround() && Mathf.Abs(controls.Player.Move.ReadValue<Vector2>().x) < ControllerDeadzone)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(playerControlData.FrictionAmount));
            amount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    private void ApplyWallFriction()
    {

    }

    private void SetGravityScale(float scaler)
    {
        rb.gravityScale = DefaultGravityScale * scaler;
    }

    private bool IsOnGround()
        => Physics2D.OverlapBox(transform.position - new Vector3(0, 0.5f, 0), new Vector2(1f, 0.1f), 0, GroundMask);
    
    private bool IsAtLeftWall() 
        => Physics2D.OverlapBox(transform.position - new Vector3(0.5f, 0, 0), new Vector2(0.1f, 1f), 0, WallMask);
    
    private bool IsAtRightWall()
        => Physics2D.OverlapBox(transform.position + new Vector3(0.5f, 0, 0), new Vector2(0.1f, 1f), 0, WallMask);
    
    private bool IsFalling() 
        => rb.velocity.y < 0;

    private bool IsJumpKeyPress()
        => controls.Player.Jump.ReadValue<float>() > 0;
    
    private bool IsDownKeyPress()
        => controls.Player.Move.ReadValue<Vector2>().y < 0;
    
}

