using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
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
            controls.gameplay.SetCallbacks(this);
        }
        controls.gameplay.Enable();
    }

    public void OnDisable()
    {
        controls.gameplay.Disable();
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        // 'Use' code here.
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // 'Move' code here.
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
        if (Input.GetKey(KeyCode.D))
        {
            characterMovement.MoveRight(gameObject);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            characterMovement.MoveLeft(gameObject);
        }

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
