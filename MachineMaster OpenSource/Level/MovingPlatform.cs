using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class MovingPlatform : SerializedMonoBehaviour
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[SerializeField]
	private float Speed = 0.1f;
	[SerializeField]
	private Vector3 _targetPoint;
	[SerializeField]
	private Sprite GreenSprite;
	[SerializeField]
	private Sprite RedSprite;
	[SerializeField]
	private Sprite BlueSprite;
	// ----------------//
	// --- 公有成员
	// ----------------//

	// ----------------//
	// --- 私有成员
	// ----------------//
	private bool _playerInside = false;
	private Rigidbody2D _rigidbody;
	private bool? _isMovingToEndPoint = null;

	private Vector3 _startPoint;
	private Vector3 _moveVec;

	// ----------------//
	// --- Unity消息
	// ----------------//
	private void Awake()
	{
		_startPoint = transform.position;
		_moveVec = _targetPoint - _startPoint;
	}

	private void Start()
	{
		GetComponentInChildren<TargetTrigger>().AddListener(PlayerEnter, PlayerExit);
		_rigidbody = GetComponentInChildren<Rigidbody2D>();
	}

	private void Update()
	{
		if (_playerInside == false) return;
		if (Keyboard.current.fKey.wasPressedThisFrame)
		{
			if (_isMovingToEndPoint.HasValue == false) 
				_isMovingToEndPoint = true;
			else 
				_isMovingToEndPoint = !_isMovingToEndPoint;
		}
	}

	private void FixedUpdate()
	{
		if (_isMovingToEndPoint.HasValue)
		{
			if (Vector2.SqrMagnitude(transform.position - (_isMovingToEndPoint.Value ? _targetPoint : _startPoint)) < (_moveVec * Time.deltaTime * Speed).sqrMagnitude)
				_rigidbody.MovePosition(_isMovingToEndPoint.Value ? _targetPoint : _startPoint);
			else
				_rigidbody.MovePosition(transform.position + (_isMovingToEndPoint.Value ? 1 : -1) * _moveVec * Time.deltaTime * Speed);
		}
	}
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, _targetPoint);
	}
#endif
	// ----------------//
	// --- 公有方法
	// ----------------//

	// ----------------//
	// --- 私有方法
	// ----------------//
	private void PlayerEnter()
	{
		_playerInside = true;
	}

	private void PlayerExit()
	{ 
		_playerInside = false;
	}

	[Button("恢复到局部原点")]
	private void ResetValue()
	{
		_targetPoint = transform.position;
	}
	// ----------------//
	// --- 类型
	// ----------------//
}
