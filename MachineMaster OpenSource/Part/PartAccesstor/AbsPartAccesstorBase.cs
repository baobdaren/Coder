using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static MaterialSetter;

/// <summary>
/// 零件的组件访问器
/// 基类简单实现了所有的访问属性，并给定了必须给出几个字段
/// 子类通过虚属性实现动态添加组件。
/// </summary>
public abstract class AbsPartAccessorBase : SerializedMonoBehaviour
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[HideInInspector]
	public PlayerPartCtrl MyPartCtrl;
	// 可连接刚体不可动态添加，这一位置，多节物体只能指定初始的值，或者不可连接
	public List<Collider2D> AllConectableColliders { get => _allConectableColliders; }
	
	public List<Renderer> AllRenders { get { InitAccesstor(); return _allRenders; } }
	public List<Collider2D> AllColliders { get { InitAccesstor(); return _allColliders; } }
	public List<Rigidbody2D> AllRigids { get { InitAccesstor(); return _allRigids; } }

	[HideInInspector]
	public SnapableBase PartDragCmpnt 
	{ 
		get 
		{ 
			if (_dragCmpnt == null) _dragCmpnt = GetComponentInChildren<SnapableBase>(true); 
			return _dragCmpnt; 
		} 
	}
	private SnapableBase _dragCmpnt;
	[HideInInspector]
	public MouseDoubleClick DoubleClickCmpnt;


	// ----------------//
	// --- 公有成员
	// ----------------//

	// ----------------//
	// --- 私有成员
	// ----------------//
	[SerializeField][ChildGameObjectsOnly][Header("可焊接/铰接的碰撞器")]
	private List<Collider2D> _allConectableColliders;
	[HideInInspector]
	private List<Renderer> _allRenders;
	private List<Collider2D> _allColliders;
	private List<Rigidbody2D> _allRigids;
	private bool Initted = false;

	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	public bool GetConnectableCollider(Vector2 worldPos, out Collider2D result)
	{
		for (int i = 0; i < AllConectableColliders.Count; i++)
		{
			if (AllConectableColliders[i].OverlapPoint(worldPos))
			{
				result = AllConectableColliders[i];
				return true;
			}
		}
		result = null;
		return false;
		//throw new System.Exception($"{transform.parent.name} - {name} 没有覆盖这个连接点 位置={worldPos}");
	}

	/// <summary>
	/// 在从预制体复制后，立即调用以初始化一些快速访问的属性
	/// 时机必须尽早，否则可能导致后期添加的子物体被错误添加
	/// </summary>
	public virtual void InitAccesstor()
	{
		if (!Initted)
		{
			Debug.Log($"初始化所{name} Accesstor 有子物体");
			OnBeforeInitAccssor();
			_allColliders = new List<Collider2D>(GetComponentsInChildren<Collider2D>(true));
			_allRenders = new List<Renderer>(GetComponentsInChildren<Renderer>(true));
			_allRigids = new List<Rigidbody2D>(GetComponentsInChildren<Rigidbody2D>(true));
			Initted = true;
		}
		//if (PartType.Steel == this.MyPartCtrl.MyPartType)
		//{
		//	Debug.Log("Steel 初始化后 有碰撞器" + _allColliders.Count);
		//}
	}

	public List<Collider2D> GetCollidersForConflictTest(PartTypes targetPartType)
	{
		InitAccesstor();
		return GetCollidersForConflict(targetPartType);
	}
	// ----------------//
	// --- 私有方法
	// ----------------//
	/// <summary>
	/// 创建可能用到的所有子物体，以创建快速访问的属性
	/// </summary>
	protected virtual void OnBeforeInitAccssor() { }
	protected virtual List<Collider2D> GetCollidersForConflict(PartTypes targetPartType) { return _allColliders; }

	// ----------------//
	// --- 类型
	// ----------------//
	public enum InitType
	{
		Edit, Simulate
	}

}
