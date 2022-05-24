using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class Gravity
    {
        private const float gravityPull = 9.8f;
        private float gravityMultiplier;
        
        public float GravityMultiplier { get { return gravityMultiplier; } set { gravityMultiplier = value; } }

        public void ApplyGravity(GameObject gameObject)
        {
            //gameObject.transform.localPosition +=  
        }
        

        
    }
}

