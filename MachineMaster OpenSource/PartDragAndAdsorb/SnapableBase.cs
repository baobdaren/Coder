using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

/// <summary>
/// 吸附
/// </summary>
public class SnapableBase : SerializedMonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public static IPartSetShader SnapTouchedPart { private set; get; }
	/// <summary>
	/// 指定实际需要移动的目标，如为NULL，则使用当前Transform
	/// </summary>
	[SerializeField]
	[EnableIf("Dragable")]
	protected Transform _moveTarget;
	public Vector2 MovePosition { get => MoveTarget.transform.position; }
	// 吸附
	public virtual SnapBaseShapeTypes SnapBaseShapeType { get => SnapBaseShapeTypes.Point; }
	[HideIf("@!Snapable || SnapBaseShapeType != SnapBaseShapeTypes.Gear")]
	public CircleCollider2D GearMiddleCircle;
	[HideIf("@!Snapable || (SnapBaseShapeType != SnapBaseShapeTypes.Circle)")]
	public CircleCollider2D CircleBound;
	[HideIf("@!Snapable || SnapBaseShapeType != SnapBaseShapeTypes.Box")]
	public BoxCollider2D BoxBound;
	[HideIf("@!Snapable || SnapBaseShapeType != SnapBaseShapeTypes.Point")]
	public Transform Point;
	// 拖拽相关
	public virtual bool Snapable { get => false; }
	public bool IsDraging { private set; get; } = false;
	public Vector2 ConsoleReadOffset { get => dragScreenOffset; }
	public bool Dragable = true;
	public bool EnableSnapCenter;
	public bool EnableSnapBound;
	public class OnDragEvent : UnityEvent { }
	public class OnDragPosEvent : UnityEvent<Vector2> { }
	[HideInInspector]
	public OnDragPosEvent OnDraging = new OnDragPosEvent();
	[HideInInspector]
	public OnDragEvent OnDragStart = new OnDragEvent();
	[HideInInspector]
	public OnDragEvent OnDragEnd = new OnDragEvent();
	// ----------------//
	// --- 私有成员
	// ----------------//
	private static Vector2 dragScreenOffset = Vector2.zero;
	private Transform MoveTarget
	{
		get
		{
			if (_moveTarget != null)
				return _moveTarget;
			return transform;
		}
	}
	// ----------------//
	// --- Unity消息
	// ----------------//
	//private void OnEnable()
	//{
	//	if (TryGetComponent<IPartSetShader>(out IPartSetShader part))
	//	{
	//		SnapManager.Instance.RegistSnapTarget(this, part);
	//	}
	//}

	public void OnBeginDrag(PointerEventData eventData = null)
	{
		//Debug.LogError("Unity 拖拽消息 - " + name);
		//if (!enabled || !Dragable)
		//{
		//	Debug.LogWarning("Unity 拖拽消息 不能使用而结束 - " + name);
		//	return;
		//}
		DragProgress_Start(true);
	}

	/// <summary>
	/// 拖拽
	/// </summary>
	/// <param name="eventData"></param>
	public void OnDrag(PointerEventData eventData)
	{
		DragProgress_Draging();
	}

	public void OnEndDrag(PointerEventData eventData = null)
	{
		DragProgress_Finish();
	}

	private void OnDisable()
	{
		//SnapManager.Instance.UnRegistSnapTarget(this);
	}

	private void Reset()
	{
		if (_moveTarget == null)
		{
			_moveTarget = transform;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Snapable == false) return;
		Color oldColor = Gizmos.color;
		Gizmos.color = new Color(0.3f, 1f, 1f, (Time.time % 1));
		switch (SnapBaseShapeType)
		{
			case SnapBaseShapeTypes.Circle:
				Gizmos.DrawWireSphere((Vector2)transform.position + CircleBound.offset, CircleBound.radius);
				break;
			case SnapBaseShapeTypes.Box:
				Gizmos.DrawWireCube((Vector2)transform.position + BoxBound.offset, BoxBound.size);
				break;
			case SnapBaseShapeTypes.Point:
				Gizmos.DrawLine((Vector2)Point.position, (Vector2)Point.position+new Vector2(0.08f, -0.08f));
				Gizmos.DrawLine((Vector2)Point.position, (Vector2)Point.position+new Vector2(-0.08f, -0.08f));
				Gizmos.DrawLine((Vector2)Point.position, (Vector2)Point.position+new Vector2(0, 0.08f));
				break;
			case SnapBaseShapeTypes.Gear:
				Gizmos.DrawWireSphere((Vector2)transform.position + GearMiddleCircle.offset, GearMiddleCircle.radius);
				break;
		}
		Gizmos.color = oldColor;
	}

	private void OnDestroy()
	{
		SnapManager.Instance.UnRegistSnapTarget(this);
	}
	// ----------------//
	// --- 公有方法
	// ----------------//
	/// <summary>
	/// 强制拖拽对象，忽略了拖拽开始的过程，直接使得对象跟随正在按下的鼠标
	/// </summary>
	public void ForceDrag(bool musetKeepPress)
	{
		if (!musetKeepPress || (musetKeepPress && Mouse.current.leftButton.isPressed))
		{
			dragScreenOffset = Vector2.zero;
			StartCoroutine(CorForceDrag(musetKeepPress));
		}
	}

	// ----------------//
	// --- 私有方法
	// ----------------//
	protected virtual SnapVector SnapToCircle(SnapableCircle target) { return SnapVector.Empty; }
	protected virtual SnapVector SnapToBox(SnapableBox target) { return SnapVector.Empty; }
	protected virtual SnapVector SnapToPoint(SnapablePoint target) { return SnapVector.Empty; }
	protected virtual SnapVector SnapToGear(SnapableGear target) { return SnapVector.Empty; }


	/// <summary>
	/// 开始拖拽处理
	/// 对于手动拖拽游戏对象，需要偏移记录抓取的点
	/// 对于强制拖拽
	/// </summary>
	/// <param name="useOffset"></param>
	private void DragProgress_Start(bool useOffset)
	{
		//Debug.LogError("False Drag");
		IsDraging = true;
		OnDragStart?.Invoke();
		//Debug.LogError("Drag 进程 Start");
		if (useOffset)
		{
			dragScreenOffset = Mouse.current.position.ReadValue() - (Vector2)CameraActor.Instance.MainCamera.WorldToScreenPoint(MoveTarget.position);
		}
		else
		{
			dragScreenOffset = Vector2.zero;
		}
		//Debug.LogError("物体的屏幕坐标" + (Vector2)CameraActor.Instance.MainCamera.WorldToScreenPoint(MoveTransform.position));
	}

	/// <summary>
	/// 拖拽
	/// </summary>
	private void DragProgress_Draging()
	{
		if (!enabled || !Dragable)
		{
			return;
		}
		if (IsDraging == false)
		{
			return;
		}
		//Debug.LogError("snapdrag");
		Vector3 mouseScreenPos = Mouse.current.position.ReadValue() - dragScreenOffset;
		mouseScreenPos += (Vector3.forward * 10);
		MoveTarget.transform.position = CameraActor.Instance.MainCamera.ScreenToWorldPoint(mouseScreenPos);
		// 中心吸附和边界吸附的结果必须合并未一个向量
		// 最佳效果是：先吸附边界，如果可以中心吸附则进行完吸附中心后需再次吸附边界。？？？
		SnapVector minVecSnapCenter = SnapVector.Empty;
		foreach (var itemSnapTo in SnapManager.Instance.AllSnapableObjects)
		{
			if (itemSnapTo.Key == this) continue;
			SnapVector resultSnapCenter = SnapCenter(itemSnapTo.Key);
			minVecSnapCenter.TrySetMinValue(resultSnapCenter);
		}
		minVecSnapCenter.ClampCenterSnapVec(out bool snapX, out bool snapY);
		if (snapX || snapY)
		{
			MoveTarget.position += minVecSnapCenter.GetVector3;
			Vector2 snapLinePos = MoveTarget.position;
			if (snapX) SnapManager.Instance.DisplayX(snapLinePos); else SnapManager.Instance.HideX();
			if (snapY) SnapManager.Instance.DisplayY(snapLinePos); else SnapManager.Instance.HideY();
			//Debug.Log("中心可吸附");
		}
		else
		{
			//Debug.Log("中心可吸附 失败");
			SnapManager.Instance.HideX();
			SnapManager.Instance.HideY();
		}

		SnapVector minVecSnapBound = SnapVector.Empty;
		IPartSetShader bestSnapTouchedTarget = null;
		foreach (var itemSnapTo in SnapManager.Instance.AllSnapableObjects)
		{
			if (itemSnapTo.Key == this) continue;
			SnapVector resultSnapBound = SnapBound(itemSnapTo.Key);
			resultSnapBound.ClampValue(0.02f, 0.02f);
			if (minVecSnapBound.TrySetMinDisValue_Bind(resultSnapBound) || bestSnapTouchedTarget == null)
			{
				bestSnapTouchedTarget = itemSnapTo.Value;
			}
		}
		if (minVecSnapBound.HasAnyValue)
		{
			MoveTarget.position += minVecSnapBound.GetVector3;
			// 在这里使用接触点结果
			//SnapManager.Instance.DisplayCircle(bestSnapTouchedTarget);
			SnapTouchedPart = bestSnapTouchedTarget;
		}
		else
		{
			SnapTouchedPart = null;
			SnapManager.Instance.HideCircle();
		}
		OnDraging?.Invoke(MoveTarget.position);
	}

	private void DragProgress_Finish()
	{
		OnDragEnd?.Invoke();
		Cursor.visible = true;
		SnapManager.Instance.HideX();
		SnapManager.Instance.HideY();
		SnapManager.Instance.HideCircle();
		//Debug.LogError("finish Drag");
		IsDraging = false;
	}


	private IEnumerator CorForceDrag(bool mustKeepPress)
	{
		DragProgress_Start(false);
		//transform.position = CameraActor.Instance.MouseWorldPos;
		//Debug.LogError("开始 强制拖拽 必须保持按下？" + mustKeepPress);
		//while (Mouse.current.leftButton.isPressed)
		while (true)
		{
			if (mustKeepPress && !Mouse.current.leftButton.isPressed) break;
			if (Keyboard.current.escapeKey.isPressed) break;
			// 在不安下鼠标左键就可以拖拽的情况下，再按下鼠标左键是否结束拖拽？
			if (!mustKeepPress && Mouse.current.leftButton.isPressed) break;
			DragProgress_Draging();
			yield return 0;
		}
		//Debug.LogError("跳出强制拖拽");
		DragProgress_Finish();
	}

	protected SnapVector SnapBound(SnapableBase target)
	{
		if (EnableSnapBound == false) return SnapVector.Empty;
		if (target.EnableSnapBound == false) return SnapVector.Empty;
		SnapVector snapBound;
		switch (target.SnapBaseShapeType)
		{
			case SnapBaseShapeTypes.Circle:
				snapBound = SnapToCircle(target as SnapableCircle);
				break;
			case SnapBaseShapeTypes.Box:
				snapBound = SnapToBox(target as SnapableBox);
				break;
			case SnapBaseShapeTypes.Point:
				snapBound = SnapToPoint(target as SnapablePoint);
				break;
			case SnapBaseShapeTypes.Gear:
				if (SnapBaseShapeType == SnapBaseShapeTypes.Gear) snapBound = SnapToGear(target as SnapableGear);
				else snapBound = SnapVector.Empty;
				break;
			default:
				snapBound = SnapToPoint(target as SnapablePoint);
				break;
		}
		//Debug.Assert(snapBound.x.HasValue && snapBound.y.HasValue, "边界吸附的结果必须xy都有值");
		return snapBound;
	}

	protected SnapVector SnapCenter(SnapableBase target)
	{
		if (EnableSnapCenter && target.EnableSnapCenter)
		{
			return new SnapVector(target.MovePosition - MovePosition);
		}
		return SnapVector.Empty;
	}

	protected static Vector3 SnapCircleAndCircle(Vector2 masterPos, float masterRadius, Vector2 targetPos, float targetRadius)
	{
		Vector2 adsorbVec = targetPos - masterPos;
		return adsorbVec - adsorbVec.normalized * (masterRadius + targetRadius);
	}

	protected static SnapVector SnapBoxAndBox(BoxCollider2D masterBox, BoxCollider2D targetBox)
	{
		throw new Exception("没有实现盒子之间的吸附");
		//return SnapVector.Empty;
		//Vector2 moveDir; float moveDis;
		//float angle = Vector2.SignedAngle(masterBox.transform.right, masterBox.transform.position - targetBox.transform.position);
		//if (angle % 90 != 0)
		//{
		//	return null;
		//}
		//moveDis = Mathf.Abs(Vector2.Distance(targetBox.transform.position, masterBox.transform.position) * Mathf.Sin(Mathf.Abs(angle) * Mathf.Deg2Rad));
		//if (angle % 180 == 0)
		//{

		//}


		////moveDis -= masterBox.radius + targetBox.radius;
		//if (moveDis == 0)
		//{
		//	moveDir = Vector2.zero;
		//}
		//else
		//{
		//	Matrix4x4 rotateMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, (angle > 0 ? 1 : -1) * (90 - Mathf.Abs(angle))));
		//	moveDir = rotateMatrix.MultiplyVector(targetBox.transform.position - masterBox.transform.position).normalized;
		//}
		//if (!masterAdsorbTarget)
		//{
		//	moveDir *= -1;
		//}
		//return moveDir * moveDis;
	}

	protected static SnapVector SnapCircleAndBox(CircleCollider2D circle, BoxCollider2D box)
	{
		Vector2 moveDir; float moveDis;
		float angle = Vector2.SignedAngle(box.transform.right, box.transform.position - circle.transform.position);
		moveDis = Mathf.Abs(Vector2.Distance(box.transform.position, circle.transform.position) * Mathf.Sin(Mathf.Abs(angle) * Mathf.Deg2Rad));
		moveDis -= box.size.y / 2 + circle.radius;
		if (moveDis == 0)
		{
			moveDir = Vector2.zero;
		}
		else
		{
			Matrix4x4 rotateMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, (angle > 0 ? 1 : -1) * (90 - Mathf.Abs(angle))));
			moveDir = rotateMatrix.MultiplyVector(box.transform.position - circle.transform.position).normalized;
		}
		return new SnapVector(moveDir);
	}

	protected static SnapVector SnapCircleAndCircle(CircleCollider2D masterCircle, CircleCollider2D targetCircle)
	{
		Vector2 masterCenter = (Vector2)masterCircle.transform.position + masterCircle.offset;
		Vector2 targetCenter = (Vector2)targetCircle.transform.position + targetCircle.offset;
#if UNITY_EDITOR
		Debug.Assert(masterCircle.transform.lossyScale.x == masterCircle.transform.lossyScale.y, "圆的吸附必须保证xy缩放相同");
		Debug.Assert(targetCircle.transform.lossyScale.x == targetCircle.transform.lossyScale.y, "圆的吸附必须保证xy缩放相同");
#endif
		float masterRadius = masterCircle.radius * masterCircle.transform.lossyScale.x;
		float targetRadius = targetCircle.radius * targetCircle.transform.lossyScale.x;
		const float keepDis = 0.000f; // 吸附不能没有间距，导致齿轮无法转动
		SnapVector snapResult = new SnapVector((targetCenter - masterCenter).normalized * ((targetCenter - masterCenter).magnitude - masterRadius - targetRadius - keepDis));
		//snapResult - new SnapVector()
		return snapResult;
	}

	protected static SnapVector SnapPointAndCircle(Vector2 point, CircleCollider2D targetCircle)
	{
		Vector2 vecDis = (Vector2)targetCircle.transform.position + targetCircle.offset - point;
		return new SnapVector((vecDis.magnitude - targetCircle.radius) * vecDis.normalized);
	}

	protected static SnapVector SnapPointAndPoint(Vector2 main, Vector2 target, out Vector2 snapTouchPoint)
	{
		snapTouchPoint = target;
		return new SnapVector(target - main);
	}
	// ----------------//
	// --- 类型
	// ----------------//
	public enum SnapBaseShapeTypes
	{
		Circle, Box, Point, Gear
	}

	public struct SnapVector
	{
		public static readonly SnapVector Empty = new SnapVector(null, null);

		public float? x;
		public float? y;

		public bool HasAnyValue => x.HasValue || y.HasValue;

		public SnapVector(float? xValue = null, float? yValue = null) { x = xValue; y = yValue; }
		public SnapVector(Vector2 vec) { x = vec.x; y = vec.y; }

		public Vector3 GetVector3 => new Vector3(x.HasValue ? x.Value : 0, y.HasValue ? y.Value : 0, 0);

		public void ClampValue(float xRange, float yRange)
		{
			if (x.HasValue)
			{
				x = (x >= -xRange && x <= yRange) ? x : null;
			}
			if (y.HasValue)
			{
				y = (y >= -yRange && y <= yRange) ? y : null;
			}
		}

		public void ClampCenterSnapVec(out bool snapX, out bool snapY)
		{
			if (HasAnyValue == false)
			{
				x = null;
				y = null;
				snapX = false;
				snapY = false;
				return;
			}
			float xValue = x.Value;
			float yValue = y.Value;
			// 钳制x时，必须保证y轴的距离不能太远
			if (Mathf.Abs(xValue) > 0.03f || Mathf.Abs(yValue) > 1f)
			{
				snapX = false;
				x = null;
			}
			else
			{
				snapX = true;
			}
			if (Mathf.Abs(yValue) > 0.03f || Mathf.Abs(xValue) > 1f)
			{
				snapY = false;
				y = null;
			}
			else
			{
				snapY = true;
			}
		}

		public void TrySetMinValue(SnapVector other)
		{
			if (!x.HasValue)
			{
				x = other.x;
			}
			else if (other.x.HasValue && Mathf.Abs(other.x.Value) < Mathf.Abs(x.Value))
			{
				x = other.x;
			}
			if (!y.HasValue)
			{
				y = other.y;
			}
			else if (other.y.HasValue && Mathf.Abs(other.y.Value) < Mathf.Abs(y.Value))
			{
				y = other.y;
			}
		}

		/// <summary>
		/// 绑定设置最小值
		/// 必须同时设置xy
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool TrySetMinDisValue_Bind(SnapVector other)
		{
			if (x.HasValue && y.HasValue)
			{
				if (other.GetVector3.magnitude > GetVector3.normalized.magnitude)
				{
					//Debug.LogError("距离不小");
					return false;
				}
			}
			if (!other.x.HasValue || !other.y.HasValue)
			{
				//Debug.LogError("没有值");
				return false;
			}
			x = other.x;
			y = other.y;
			return true;
		}

		public bool TryGetSqrDistance(out float dis)
		{
			if (x.HasValue == false && x.HasValue == false)
			{
				dis = -1;
				return false;
			}
			else
			{
				dis = (x.HasValue ? (x * x) : 0).Value + (y.HasValue ? (y * y) : 0).Value;
				return true;
			}
		}

		public SnapVector Reverse()
		{
			if (this.x.HasValue)
			{
				this.x = -this.x.Value;
			}
			if (this.y.HasValue)
			{
				this.y = -this.y.Value;
			}
			return this;
		}
	}
}
