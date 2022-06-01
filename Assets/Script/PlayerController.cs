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
    
    private Playercontrols controls;
    private float targetSpeed;
    private float speedDif;
    private float accelerateRate;
    private float movement;
    private int coyote = 0;

    private void Awake()
    {
        controls = new();

    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
      //  controls.Player.Jump.started += ApplyJump;
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
        if (!playerControlData.IsJumping)
        {
            if (controls.Player.Jump.ReadValue<float>() > 0 && coyote < playerControlData.CoyoteByFrame) 
            {
                ApplyJump();
            }
            checkCoyote();
            //else if(coyote < playerControlData.CoyoteByFrame)
            //{
            //    coyote++;
            //}
            //else if(!Physics2D.OverlapBox(transform.position, new Vector2(1, 1), 0, GroundMask))
            //{
            //    coyote = 0;
            //}

        }
        //controls.Player.Jump.canceled += JumpCut;

        if(playerControlData.IsJumping && rb.velocity.y < 0)
        {
            playerControlData.IsJumping = false;
        }
    }

    private void ApplyRun(Vector2 controlInput)
    {
        targetSpeed = controlInput.x * playerControlData.RunMaxSpeed;
        speedDif = targetSpeed - rb.velocity.x;
        accelerateRate = (Math.Abs(targetSpeed) > 0.01f) ? playerControlData.RunAccel : playerControlData.RunDeccel;
        movement = MathF.Pow(Math.Abs(speedDif) * accelerateRate,playerControlData.AccelPower) * Math.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
    }

    private void ApplyJump()
    {
        playerControlData.IsJumping = true;
        rb.AddForce(Vector2.up * playerControlData.JumpForce,ForceMode2D.Impulse);
        
    }

    private void checkCoyote()
    {
        if (!Physics2D.OverlapBox(transform.position-new Vector3(0,0.5f,0), new Vector2(1, 0.5f), 0, GroundMask) && coyote < playerControlData.CoyoteByFrame){
            coyote++;
        }
        else if(Physics2D.OverlapBox(transform.position - new Vector3(0, 0.5f, 0), new Vector2(1, 0.5f), 0, GroundMask))
        {
            coyote = 0;
        }
    }
    private void JumpCut(InputAction.CallbackContext context)
    {

    }
}
