﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Movement characterMovement = new Movement();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
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
           
        }else if (Input.GetKey(KeyCode.Space))
        {
            
        }else if (Input.GetKeyUp(KeyCode.Space))
        {

        }
    }
}
