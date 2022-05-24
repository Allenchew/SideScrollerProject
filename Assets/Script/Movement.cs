using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics;

public class Movement : Gravity
{
    private float defaultSpeed = 1.0f;
    
    public float MovementSpeed
    {
        get { return defaultSpeed; }
        set { defaultSpeed = value; }
    }

    public void MoveRight(GameObject gameObject)
    {
        gameObject.transform.localPosition += gameObject.transform.right * defaultSpeed;
    }
    
    public void MoveLeft(GameObject gameObject)
    {
        gameObject.transform.localPosition += gameObject.transform.right * -defaultSpeed;
    }
    
    public void InitiateJump(GameObject gameObject)
    {

    }

    public void HoldJump(GameObject gameObject)
    {

    }

}

