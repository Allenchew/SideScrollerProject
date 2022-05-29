using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Playercontrols controls;
    Movement characterMovement;
    private void Awake()
    {
        controls = new();
        characterMovement = new();
    }
    void Start()
    {
        
    }
   
    void Update()
    {
        controls.Player.Move.started += Move;
    }

    private void Move(InputAction.CallbackContext context)
    {
        var movementDirection = context.ReadValue<Vector2>();
        characterMovement.MoveHorizontal(gameObject, movementDirection);
    }
    public void OnEnable()
    {
        controls.Player.Enable();
    }

    public void OnDisable()
    {
        controls.Player.Disable();
    }

    private void FixedUpdate()
    {
        /*  TO BE REMOVE DUE TO THE USE OF INPUTACTION
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //initial jump,set upward speed
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            // while holding, upward speed decrease towards 0(decrease by gravity downward speed)
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            //upward speed = 0

        }*/
    }

    
}
