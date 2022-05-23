using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
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
        gameObject.transform.localPosition -= gameObject.transform.right * defaultSpeed;
    }
    public void Jump(GameObject gameObject)
    {

    }
}

