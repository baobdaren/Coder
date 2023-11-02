using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class AircraftPlayer : SerializedMonoBehaviour
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[SerializeField]
	[Range(0, 100)]
	private float MoveForce = 1;

	[SerializeField]
	private float TorqueForce = 1;

	[ReadOnly]
	[SerializeField]
	private float CurentTorque;
	// ----------------//
	// --- 公有成员
	// ----------------//
	public bool Working
	{
		private set
		{
			m_working = value;
			m_rigidbody.gravityScale = m_working ? 0 : 1;
			m_rigidbody.drag = m_working ? 1 : 0.1f;
			m_rigidbody.angularDrag = m_working ? 5 : 0.1f;
		}
		get => m_working;
	}
	private bool m_working = false;

	// ----------------//
	// --- 私有成员
	// ----------------//
	private Key RightMoveKey = Key.D;
	private Key LeftMoveKey = Key.A;
	private Key UpMoveKey = Key.W;
	private Key DownMoveKey = Key.S;

	private Rigidbody2D m_rigidbody { get { if (mm_rigidbody == null) mm_rigidbody = GetComponent<Rigidbody2D>(); return mm_rigidbody; } }
	private Rigidbody2D mm_rigidbody;

	private float m_moveTime = 0;
	private float m_accelerationTime = 0.3f; // 加速度从0到满的时间
	private Vector2 m_TargetUpDir = Vector2.up;
	private readonly Vector2 m_RightMoveStateDir = new Vector2(1, 2);
	private readonly Vector2 m_LeftMoveStateDir = new Vector2(-1, 2);

	// ----------------//
	// --- Unity消息
	// ----------------//
	private void Update()
	{
		if (Keyboard.current.spaceKey.wasPressedThisFrame) { Working = !Working; }
	}

	private void FixedUpdate()
	{
		if (!Working) return;
		// 飞行控制
		Vector2 moveDir = Vector2.zero;
		if (Keyboard.current[RightMoveKey].isPressed) moveDir += Vector2.right;
		if (Keyboard.current[LeftMoveKey].isPressed) moveDir += Vector2.left;
		if (Keyboard.current[UpMoveKey].isPressed) moveDir += Vector2.up;
		if (Keyboard.current[DownMoveKey].isPressed) moveDir += Vector2.down;
		Vector2 moveVec = moveDir.normalized * MoveForce;
		if (moveVec == Vector2.zero)
		{
			m_moveTime = 0;
		}
		else
		{
			m_moveTime += Time.fixedDeltaTime;
			m_rigidbody.AddForce(moveVec * ((m_moveTime < m_accelerationTime) ? (m_moveTime / m_accelerationTime) : 1));
		}
		// 旋转恢复 ()
		if (moveDir.x == 0)
		{
			m_TargetUpDir = Vector2.up;
		}
		else
		{ 
			m_TargetUpDir = moveDir.x > 0 ? m_RightMoveStateDir : m_LeftMoveStateDir;
		}
		float curUpAngle = Vector2.SignedAngle(m_TargetUpDir, transform.up);
		m_rigidbody.AddTorque(-curUpAngle / 60f * TorqueForce);
		// 重力调整(倾斜角过大时，模拟升力减小)
		float absCurUpDir = Mathf.Abs(curUpAngle);
		if (absCurUpDir > 60)
		{
			m_rigidbody.gravityScale = (absCurUpDir - 60) / 30;
		}
		else
		{
			m_rigidbody.gravityScale = 0;
		}
	}
	// ----------------//
	// --- 公有方法
	// ----------------//

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
