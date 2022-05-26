using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class Gravity
    {
        private const float gravityPull = 0.98f;
        private float gravityMultiplier = 1.0f;
        
        public float GravityMultiplier { get { return gravityMultiplier; } set { gravityMultiplier = value; } }

        public float ApplyGravity()
        {
            return gravityPull * GravityMultiplier;
        }
        

        
    }
}

