using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public interface IConnectableCtrl
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public List<Collider2D> GetIgnorableColliders { get; }

	// ----------------//
	// --- 私有成员
	// ----------------//
	public List<Collider2D> GetEditConnectableColliders { get; }
	public List<Collider2D> GetPhysicsConnectablrColliders { get; }

	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	public bool OverlapPoint(Vector2 pos)
	{
		if (GetEditConnectableColliders != null)
		{
			foreach (var item in GetEditConnectableColliders)
			{
				if (item.OverlapPoint(pos))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool TryGetOverlapPointEditColliderIndex(Vector2 point, out int index)
	{
		for (int i = 0; i < GetEditConnectableColliders.Count; i++)
		{
			if (GetEditConnectableColliders[i].OverlapPoint(point))
			{
				index = i;
				return true;
			}
		}
		index = -1;
		return false;
	}

	public Collider2D GetPhysicsCollider(int editIndex)
	{ 
		return GetPhysicsConnectablrColliders[editIndex];
	}

	public Collider2D GetPhysicsCollider(Collider2D editCollider)
	{
		return GetPhysicsCollider(GetEditConnectableColliders.IndexOf(editCollider));
	}
	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
