using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 1.0f;
    public float AccelerateSpeed = 1.0f;
    public float DecelerateSpeed = -1.0f;
    public float VelPower = 1.0f;
    public float jumpForce = 1.0f;
    public Rigidbody2D rb;
    
    private Playercontrols controls;
    private float targetSpeed;
    private float speedDif;
    private float accelerateRate;
    private float movement;

    private float lastGroundedTime = 0;
    private float lastJumpTime = 0;
    private bool isJumping;
    private bool jumpInputReleased;

    private void Awake()
    {
        controls = new();

    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
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

    private void FixedUpdate()
    {
        Vector2 playerInput = controls.Player.Move.ReadValue<Vector2>();
        ApplyRun(playerInput);
        if (Physics2D.OverlapBox(new Vector2(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y), new Vector2(1, 1), 0))
        {
            controls.Player.Jump.started += ApplyJump;
        }
        controls.Player.Jump.canceled += JumpCut;
    }

    private void ApplyRun(Vector2 controlInput)
    {
        targetSpeed = controlInput.x * MoveSpeed;
        speedDif = targetSpeed - rb.velocity.x;
        accelerateRate = (Math.Abs(targetSpeed) > 0.01f) ? AccelerateSpeed : DecelerateSpeed;
        movement = MathF.Pow(Math.Abs(speedDif) * accelerateRate, VelPower) * Math.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
    }

    private void ApplyJump(InputAction.CallbackContext context)
    {
        Debug.Log("jumped");
        rb.AddForce(Vector2.up * jumpForce,ForceMode2D.Impulse);
        lastGroundedTime = 0;
        lastJumpTime = 0;
        isJumping = true;
        jumpInputReleased = false;
    }

    private void JumpCut(InputAction.CallbackContext context)
    {

    }
}
