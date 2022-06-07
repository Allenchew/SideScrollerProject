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
	public bool FaceRight;
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
	public bool IsWallJumping;
	[Range(0, 1)] public float JumpCutMultiplier;


	[Header("Dash")]
	public float DashSpeed;
	public float StopSpeed;
	public bool IsDashing;
	public int DashFrame;

	[Header("Gravity")]
	public float FallMultiplier;
	public float FastFallMultipler;
	public float OnWallFallGravity;

	// TODO: Add Double Jump Parameter
	// TODO: Add Drag
	// TODO: Add Wall Jump Parameter
}

