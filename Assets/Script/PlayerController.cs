using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Playercontrols.IPlayerActions
{
    private Movement characterMovement = new Movement();
    
    Playercontrols controls;
    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new Playercontrols();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }

    public void OnDisable()
    {
        controls.Player.Disable();
    }
    
    public void OnWASD(InputAction.CallbackContext context)
    {
        if (context.control.IsPressed())
        {
            Debug.Log("pressed");
        }
        if(context.control.displayName == "D")
        {
            characterMovement.MoveRight(gameObject);
        }
        else if(context.control.displayName == "A")
        {
            characterMovement.MoveLeft(gameObject);
        }
        //throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        
        //throw new System.NotImplementedException();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

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
