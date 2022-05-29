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

    public void MoveHorizontal(GameObject gameObject,Vector2 direction)
    {
        gameObject.transform.localPosition += new Vector3(direction.x * defaultSpeed,0,0);
    }
    
   
    public void InitiateJump(GameObject gameObject)
    {

    }

    public void HoldJump(GameObject gameObject)
    {

    }

}

