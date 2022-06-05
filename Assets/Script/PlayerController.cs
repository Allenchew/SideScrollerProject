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
    public float DefaultGravityScale = 3;
    
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

    // TODO Apply wall detection
    // TODO prevent player to stick to wall with direction key
    // TODO Add Wall Jump
    // TODO Coyote for wall jump
    // TODO Add Dash
    // TODO Tidy up and seperate Codes
    private void FixedUpdate()
    {
        Vector2 playerInput = controls.Player.Move.ReadValue<Vector2>();

        if(Mathf.Abs(playerInput.x) > 0 && !playerControlData.IsDashing)
            ApplyRun(playerInput);

        ApplyFriction();

        if (!playerControlData.IsJumping)
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
        if (IsFalling() && !playerControlData.IsDashing)
        {
            if (IsDownKeyPress())
                SetGravityScale(playerControlData.FastFallMultipler);
            else
                SetGravityScale(playerControlData.FallMultiplier);
        }else if (playerControlData.IsDashing)
        {
            SetGravityScale(0);
        }
        else
        {
            SetGravityScale(1.0f);
        }

        if (IsDashKeyPress() && !playerControlData.IsDashing)
        {
            playerControlData.IsDashing = true;
            ApplyDash();
        }

        if(playerControlData.IsDashing && dashFrameCount < playerControlData.DashFrame)
        {
            dashFrameCount++;
        }
        else
        {
            playerControlData.IsDashing = false;
            StopDash();
        }
    }

    private void ApplyRun(Vector2 controlInput)
    {
        targetSpeed = controlInput.x * playerControlData.RunMaxSpeed;
        speedDif = targetSpeed - rb.velocity.x;
        SetFacing();
        if(IsOnGround())
            accelerateRate = (Math.Abs(targetSpeed) > 0.01f) ? playerControlData.RunAccel : playerControlData.RunDeccel;
        else
            accelerateRate = (Math.Abs(targetSpeed) > 0.01f) ? playerControlData.RunAccel * playerControlData.AccelInAir : playerControlData.RunDeccel  * playerControlData.DeccelInAir;
        movement = MathF.Pow(Math.Abs(speedDif) * accelerateRate,playerControlData.AccelPower) * Math.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
    }

    private void SetFacing()
    {
        if (targetSpeed > 0.1f)
            playerControlData.FaceRight = true;
        else if (targetSpeed < -0.1f)
            playerControlData.FaceRight = false;
    }
    private void ApplyJump()
    {
        playerControlData.IsJumping = true;
        rb.AddForce(Vector2.up * playerControlData.JumpForce,ForceMode2D.Impulse);
    }


    private void ApplyDash()
    {
        Vector2 dashDirection = playerControlData.FaceRight ? Vector2.right : Vector2.left;
        rb.AddForce(dashDirection * playerControlData.DashSpeed, ForceMode2D.Impulse);
        
    }
    private void StopDash()
    {
        rb.AddForce(rb.velocity.x * Vector2.right);
        dashFrameCount = 0;
    }
    private void CheckCoyote()
    {
        if (!IsOnGround() && coyote < playerControlData.CoyoteByFrame)
        {
            coyote++;
        }
        else if(IsOnGround())
        {
            coyote = 0;
        }
    }
    private void ApplyJumpCut()
    {
        rb.AddForce(Vector2.down * rb.velocity.y * (1 - playerControlData.JumpCutMultiplier), ForceMode2D.Impulse);
    }

    private void ApplyFriction()
    {
        if(IsOnGround() && Mathf.Abs(controls.Player.Move.ReadValue<Vector2>().x) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(playerControlData.FrictionAmount));

            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    private void SetGravityScale(float scaler)
    {
        rb.gravityScale = DefaultGravityScale * scaler;
    }
    private bool IsOnGround()
    {
        return Physics2D.OverlapBox(transform.position - new Vector3(0, 0.5f, 0), new Vector2(1f, 0.1f), 0, GroundMask);
    }
    private bool IsFalling()
    {
        return rb.velocity.y < 0;
    }

    private bool IsJumpKeyPress()
    {
        return controls.Player.Jump.ReadValue<float>() > 0;
    }
    private bool IsDownKeyPress()
    {
        return controls.Player.Move.ReadValue<Vector2>().y < 0;
    }
    private bool IsDashKeyPress()
    {
        return controls.Player.Dash.ReadValue<float>() > 0;
    }
}



public class DebugTools
{

}
