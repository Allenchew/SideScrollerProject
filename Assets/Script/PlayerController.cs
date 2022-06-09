﻿using System;
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
    private int wallCoyote = 0;
    private int stickyCoyote = 0;
    private int dashFrameCount = 0;
    private bool jumpKeyUp = true;

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

    // TODO : Adjust Wall Jump     **need to test and get feed back first
    // TODO : prevent wall jump while at ground
    // TODO : add interval for dash and reset dash while on ground or on wall
    // TODO : Tidy up and seperate Codes
    //-----------------------------------------------------------------

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
        CheckCoyote();
        CheckWallCoyote();

        if (playerControlData.IsJumping && IsOnGround() && !IsJumpKeyPress())
        {
            playerControlData.IsJumping = false;
        }
        if (playerControlData.IsWallJumping && IsAtWall() && !IsJumpKeyPress())
        {
            playerControlData.IsWallJumping = false;
        }

        if (playerControlData.IsJumping && !IsFalling() && !IsJumpKeyPress() && !IsAtWall())
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
                if (stickyCoyote < playerControlData.StickyWallFrame)
                {
                    stickyCoyote++;
                    SetGravityScale(0);
                } else
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

    private void CheckJumpStatus(InputAction.CallbackContext context)
    {
        if ((IsOnGround() || IsUnderCoyote()))
        {
            if (!playerControlData.IsJumping && !playerControlData.IsDashing)
            {
                ApplyJump();
            }

        }
        if (IsAtWall() || IsUnderWallCoyote())
        {
            if (!playerControlData.IsDashing && !playerControlData.IsWallJumping)
            {
                ApplyWallJump();
            }

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
        wallCoyote = 0;
        Vector2 jumpDirection = IsAtLeftWall() ? Vector2.right : Vector2.left;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.AddForce((Vector2.up + (jumpDirection * 0.5f)) * playerControlData.JumpForce, ForceMode2D.Impulse);
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
        if (!IsAtWall() && IsUnderWallCoyote() && !IsOnGround() && !playerControlData.IsWallJumping)
        {
            wallCoyote++;
        } else if (IsAtWall() || IsOnGround())
        {
            wallCoyote = 0;
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
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        dashFrameCount = 0;
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

    private void SetGravityScale(float scaler)
    {
        rb.gravityScale = DefaultGravityScale * scaler;
    }

    private bool IsOnGround()
        => Physics2D.OverlapBox(transform.position - new Vector3(0, 0.5f, 0), new Vector2(0.9f, 0.1f), 0, GroundMask);

    private bool IsAtLeftWall()
        => Physics2D.OverlapBox(transform.position - new Vector3(0.5f, -0.1f, 0), new Vector2(0.1f, 0.9f), 0, WallMask | GroundMask);

    private bool IsAtRightWall()
        => Physics2D.OverlapBox(transform.position + new Vector3(0.5f, 0.1f, 0), new Vector2(0.1f, 0.9f), 0, WallMask | GroundMask);
    
    private bool IsAtWall()
        => IsAtLeftWall() || IsAtRightWall();
    private bool IsFalling() 
        => rb.velocity.y < -0.01f;

    private bool IsUnderCoyote()
        => coyote < playerControlData.CoyoteByFrame;

    private bool IsUnderWallCoyote()
        => wallCoyote < playerControlData.WallCoyoteByFrame;

    private bool IsJumpKeyPress()
        => controls.Player.Jump.IsPressed();
    
    private bool IsDownKeyPress()
        => controls.Player.Move.ReadValue<Vector2>().y < -ControllerDeadzone;
    
    private void JumpKeyReleased()
    {
        jumpKeyUp = true;
    }
}

