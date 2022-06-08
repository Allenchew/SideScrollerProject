using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PlayerObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Vector3 positionDifference = PlayerObject.transform.localPosition - transform.localPosition;
        
        if(Mathf.Abs(positionDifference.x) > 3 || Mathf.Abs(positionDifference.y) > 3)
        {
            Vector3 targetPosition = new Vector3(PlayerObject.transform.localPosition.x,PlayerObject.transform.localPosition.y,transform.localPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition,targetPosition, Time.deltaTime);
        }
       
    }
}
