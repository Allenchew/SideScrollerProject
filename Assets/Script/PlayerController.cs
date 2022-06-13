using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ControlData playerControlData;

    public Rigidbody2D rb;
    public LayerMask GroundMask;
    public LayerMask WallMask;
    public LayerMask CeilingMask;
    public float DefaultGravityScale = 3;
    public float ControllerDeadzone = 0.01f;

    private Playercontrols controls;
    private float targetSpeed;
    private float speedDif;
    private float accelerateRate;
    private float movement;
    private int coyote = 0;
    private int wallCoyote = 0;
    private int stickyCoyote = 0;
    private int wallJumpFrame = 0;
    private int dashFrameCount = 0;
    private int dashCDCount = 0;
    private bool isWallSlide = false;


    private void Awake()
    {
        controls = new();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = DefaultGravityScale;
        controls.Player.Dash.started += ApplyDash;
        controls.Player.Jump.started += CheckJumpStatus;
    }

    public void OnEnable()
    {
        controls.Player.Enable();
    }

    public void OnDisable()
    {
        controls.Player.Disable();
    }

    // TODO : Upward and downward movement
    // TODO : Tidy up and seperate Codes
    //-----------------------------------------------------------------

    private void FixedUpdate()
    {
        Vector2 playerInput = controls.Player.Move.ReadValue<Vector2>();

        CheckDashCD();
        CheckCoyote();
        CheckWallCoyote();
        CheckWallJumpFrame();
        CheckWallSlide(playerInput);

        if (!playerControlData.IsDashing && !playerControlData.IsWallJumping)
        {
            ApplyRun(playerInput);
        }
        else if(!IsWallJumpLocked() && IsMovementKeyPress() && !playerControlData.IsDashing)
        {
            ApplyRun(playerInput);
        }

        if (!playerControlData.IsDashing)
        {
            ApplyFriction();
        }

        if (playerControlData.IsJumping && IsOnGround() && !IsJumpKeyPress())
        {
            playerControlData.IsJumping = false;
        }

        if (playerControlData.IsJumping && !IsFalling() && !IsJumpKeyPress() && !IsAtWall())
        {
            ApplyJumpCut();
        }

        if (IsAtWall() && !IsJumpKeyPress())
        {
            wallJumpFrame = 0;
        }

        if ((playerControlData.IsJumping || playerControlData.IsWallJumping) && IsHitTop() && !IsFalling())
        {
            ZeroHorizontalVelocity();
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
            else if (isWallSlide)
            {
                if (stickyCoyote < playerControlData.StickyWallFrame)
                {
                    stickyCoyote++;
                    ZeroVerticalVelocity();
                }else
                    SetGravityScale(playerControlData.OnWallFallGravity);
            }
            else
            {
                SetGravityScale(playerControlData.FallMultiplier);
            }
        }
        else if (playerControlData.IsDashing)
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

    private void CheckJumpStatus(InputAction.CallbackContext context)
    {
        if ((IsOnGround() || IsUnderCoyote()))
        {
            if (!playerControlData.IsJumping && !playerControlData.IsDashing)
            {
                ApplyJump();
            }

        }
        if ((IsAtWall() || IsUnderWallCoyote()) && !playerControlData.IsWallJumping && !IsOnGround())
        {
            if (!playerControlData.IsDashing)
            {
                ApplyWallJump();
            }

        }
    }

    private void CheckCoyote()
    {
        if (!IsOnGround() && IsUnderCoyote() && !playerControlData.IsJumping)
        {
            coyote++;
        }
        else if (IsOnGround())
        {
            coyote = 0;
        }
    }
    private void CheckWallCoyote()
    {
        if (!IsAtWall() && IsUnderWallCoyote() && !IsOnGround())
        {
            wallCoyote++;
        }
        else if (IsAtWall() || IsOnGround())
        {
            wallCoyote = 0;
        }
    }

    private void CheckWallJumpFrame()
    {
        if (wallJumpFrame < playerControlData.FixedWallJumpFrame)
        {
            wallJumpFrame++;
        }
        else
        {
            playerControlData.IsWallJumping = false;
        }
    }

    private void CheckWallSlide(Vector2 playerInput)
    {
        if (isWallSlide && (!IsAtWall() || IsOnGround()))
        {
            isWallSlide = false;
        }
        else if((IsAtLeftWall() && playerInput.x < -ControllerDeadzone) 
            || (IsAtRightWall() && playerInput.x > ControllerDeadzone))
        {
            isWallSlide = true;
        }
    }

    private void CheckDashCD()
    {
        if(dashCDCount > 0)
        {
            dashCDCount--;
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
        movement = MathF.Pow(Math.Abs(speedDif) * accelerateRate, playerControlData.AccelPower) * Math.Sign(speedDif);
        if (!((IsAtLeftWall() && movement < 0) || (IsAtRightWall() && movement > 0)))
        {
            rb.AddForce(movement * Vector2.right);
        }
    }

    private void ApplyJump()
    {
        playerControlData.IsJumping = true;
        rb.AddForce(Vector2.up * playerControlData.JumpForce, ForceMode2D.Impulse);
    }

    private void ApplyWallJump()
    {
        playerControlData.IsWallJumping = true;
        wallCoyote = playerControlData.WallCoyoteByFrame;
        stickyCoyote = 0;
        Vector2 jumpDirection;
        if (IsAtWall())
        {
            jumpDirection = IsAtLeftWall() ? Vector2.right : Vector2.left;
        }
        else
        {
            jumpDirection = playerControlData.FaceRight ? Vector2.right : Vector2.left;
        }
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.AddForce((Vector2.up * playerControlData.UpJumpForce + jumpDirection * playerControlData.SideJumpForce)
            * playerControlData.JumpForce, ForceMode2D.Impulse);
    }

   
    private void ApplyJumpCut()
    {
        rb.AddForce(Vector2.down * rb.velocity.y * (1 - playerControlData.JumpCutMultiplier), ForceMode2D.Impulse);
    }

    private void ApplyDash(InputAction.CallbackContext context)
    {
        if (!playerControlData.IsDashing && IsDashCDFinish())
        {
            dashCDCount = playerControlData.DashCD;
            ZeroHorizontalVelocity();
            ZeroVerticalVelocity();
            playerControlData.IsDashing = true;
            Vector2 dashDirection;
            if (isWallSlide)
            {
                dashDirection = playerControlData.FaceRight ? Vector2.left : Vector2.right;
            }
            else
                dashDirection = playerControlData.FaceRight ? Vector2.right : Vector2.left;


            rb.AddForce(dashDirection * playerControlData.DashSpeed, ForceMode2D.Impulse);
        }
    }

    private void ApplyFriction()
    {
        if (IsOnGround() && Mathf.Abs(controls.Player.Move.ReadValue<Vector2>().x) < ControllerDeadzone)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(playerControlData.FrictionAmount));
            amount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    private void StopDash()
    {
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        dashFrameCount = 0;
    }

    private void SetGravityScale(float scaler)
    {
        rb.gravityScale = DefaultGravityScale * scaler;
    }

    private void ZeroHorizontalVelocity()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void ZeroVerticalVelocity()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    private bool IsDashCDFinish()
        => dashCDCount < 1;

    private bool IsOnGround()
        => Physics2D.OverlapBox(transform.localPosition - new Vector3(0, 0.5f, 0), new Vector2(0.9f, 0.1f), 0, GroundMask);

    private bool IsAtLeftWall()
        => Physics2D.OverlapBox(transform.localPosition - new Vector3(0.5f, -0.1f, 0), new Vector2(0.1f, 0.9f), 0, WallMask | GroundMask);

    private bool IsAtRightWall()
        => Physics2D.OverlapBox(transform.localPosition + new Vector3(0.5f, 0.1f, 0), new Vector2(0.1f, 0.9f), 0, WallMask | GroundMask);

    private bool IsHitTop()
        => Physics2D.OverlapBox(transform.localPosition + new Vector3(0, 0.5f, 0), new Vector2(0.9f, 0.1f), 0,CeilingMask);

    private bool IsAtWall()
        => IsAtLeftWall() || IsAtRightWall();

    private bool IsFalling() 
        => rb.velocity.y < -0.01f;

    private bool IsUnderCoyote()
        => coyote < playerControlData.CoyoteByFrame;

    private bool IsUnderWallCoyote()
        => wallCoyote < playerControlData.WallCoyoteByFrame;

    private bool IsWallJumpLocked()
        => wallJumpFrame < playerControlData.FixedWallJumpFrame / 2;

    private bool IsJumpKeyPress()
        => controls.Player.Jump.IsPressed();
    
    private bool IsDownKeyPress()
        => controls.Player.Move.ReadValue<Vector2>().y < -ControllerDeadzone;

    private bool IsMovementKeyPress()
        => Mathf.Abs(controls.Player.Move.ReadValue<Vector2>().x) > ControllerDeadzone;
}

