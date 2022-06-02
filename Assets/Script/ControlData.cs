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
	public float FrictionAmount;
	[Range(0, 1)] public float AccelInAir;
	[Range(0, 1)] public float DeccelInAir;
	[Space(5)]
	[Range(.5f, 2f)] public float AccelPower;
	[Range(.5f, 2f)] public float StopPower;
	[Range(.5f, 2f)] public float TurnPower;

	[Header("Jump")]
	public int CoyoteByFrame;
	public float JumpForce;
	public float FallMultiplier;
	public float FastFallMultipler;
	public bool IsJumping;
	[Range(0, 1)] public float JumpCutMultiplier;

// TODO: Add custom gravity to override physics gravity
// TODO: Add Double Jump Parameter
// TODO: Add Drag
// TODO: Add Wall Jump Parameter
// TODO: Add Dash Parameter
}

