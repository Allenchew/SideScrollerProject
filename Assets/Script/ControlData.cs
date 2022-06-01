using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Player/PlayerMovementData")]
public class ControlData : ScriptableObject
{

    [Header("Run")]
	public float RunMaxSpeed;
	public float RunAccel;
	public float RunDeccel;
	[Range(0, 1)] public float AccelInAir;
	[Range(0, 1)] public float DeccelInAir;
	[Space(5)]
	[Range(.5f, 2f)] public float AccelPower;
	[Range(.5f, 2f)] public float StopPower;
	[Range(.5f, 2f)] public float TurnPower;

	[Header("Jump")]
	public int CoyoteByFrame;
	public float JumpForce;
	public bool IsJumping;

// TODO: Add Coyote
// TODO: Add custom gravity to override physics gravity
// TODO: Add Jump and Double Jump Parameter
// TODO: Add Drag
// TODO: Add Wall Jump Parameter
// TODO: Add Dash Parameter
}

