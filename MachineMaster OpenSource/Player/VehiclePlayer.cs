using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class VehiclePlayer : PlayerBase
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[SerializeField]
	private WheelJoint2D _leftWheelJoint;

	[SerializeField]
	private WheelJoint2D _rightWheelJoint;

	// ----------------//
	// --- 公有成员
	// ----------------//
	public override Vector2 GetPalyerPos => (Vector2)transform.position;

	// ----------------//
	// --- 私有成员
	// ----------------//
	private JointMotor2D _forwardMotor_left = new JointMotor2D() { maxMotorTorque = 4, motorSpeed = -1000 };
	private JointMotor2D _forwardMotor_right = new JointMotor2D() { maxMotorTorque = 0, motorSpeed = -1000 };
	private JointMotor2D _backMotor_left = new JointMotor2D() { maxMotorTorque = 0, motorSpeed = 800 };
	private JointMotor2D _backMotor_right = new JointMotor2D() { maxMotorTorque = 3, motorSpeed = 800 };
	private JointMotor2D _stopMotor = new JointMotor2D() { maxMotorTorque = 5, motorSpeed = 0 };
	private JointMotor2D _freeMotor = new JointMotor2D() { maxMotorTorque = 0.1f, motorSpeed = 0 };

	private enum PlayerRunState { Forward, Stop, Back, Free }
	private PlayerRunState _currentState = PlayerRunState.Stop;


	// ----------------//
	// --- Unity消息
	// ----------------//
	private void Awake()
	{
		_leftWheelJoint.useMotor = true;
		_rightWheelJoint.useMotor = true;
	}

	private void Update()
	{
		PlayerRunState state = GetInputState();
		if (state != _currentState)
		{
			_currentState = state;
			switch (_currentState)
			{
				case PlayerRunState.Forward:
					SetForward();
					break;
				case PlayerRunState.Stop:
					SetStop();
					break;
				case PlayerRunState.Back:
					SetBack();
					break;
				default:
					SetFree();
					break;
			}
		}
	}

	// ----------------//
	// --- 公有方法
	// ----------------//

	// ----------------//
	// --- 私有方法
	// ----------------//
	private void SetForward()
	{
		_leftWheelJoint.motor = _forwardMotor_left;
		_rightWheelJoint.motor = _forwardMotor_right;
	}

	private void SetBack()
	{
		_leftWheelJoint.motor = _backMotor_left;
		_rightWheelJoint.motor = _backMotor_right;
	}

	private void SetStop()
	{
		_leftWheelJoint.motor = _stopMotor;
		_rightWheelJoint.motor = _stopMotor;
	}

	private void SetFree()
	{
		_leftWheelJoint.motor = _freeMotor;
		_rightWheelJoint.motor = _freeMotor;
	}

	private PlayerRunState GetInputState()
	{
		if (Keyboard.current.spaceKey.isPressed)
		{
			return PlayerRunState.Stop;
		}
		if (Keyboard.current.aKey.isPressed)
		{
			return PlayerRunState.Forward;
		}
		if (Keyboard.current.zKey.isPressed)
		{
			return PlayerRunState.Back;
		}
		return PlayerRunState.Free;
	}
	// ----------------//
	// --- 类型
	// ----------------//
}
