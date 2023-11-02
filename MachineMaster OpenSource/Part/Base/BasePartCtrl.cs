using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class BasePartCtrl : IConnectableCtrl, ICollisionCtrl, IPartSetShader
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public abstract bool IsPlayerPart { get; }

	public abstract List<Collider2D> GetIgnorableColliders { get; }
	public abstract List<Collider2D> GetEditConnectableColliders { get; }
	public abstract List<Collider2D> GetPhysicsConnectablrColliders { get; }
	public abstract PartTypes MyPartType { get; }

	public abstract IEnumerable<Collider2D> GetPhysicsColliders { get; }
	// ----------------//
	// --- 私有成员
	// ----------------//
	protected abstract List<Renderer> EditRenders { get; }


	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//


	public abstract IEnumerable<Collider2D> GetColliders_CollisionTest(PartTypes partType);

	public void SetOutLine(Color outlineColor)
	{
		SetActiveOutLine(true);
		foreach (var item in EditRenders)
		{
			Debug.Assert(item.material.HasColor("OutlineColor"), "材质错误", item.gameObject);
			item.material.SetColor("OutlineColor", outlineColor);
		}
	}
	public void SetActiveOutLine(bool active)
	{
		foreach (var item in EditRenders)
		{
			Debug.Assert(item.material.HasInt("UseOutline"), "材质错误", item.gameObject);
			item.material.SetInt("UseOutline", active ? 1 : 0);
		}
	}
	public void SetActiveTex(bool active)
	{
		foreach (var item in EditRenders)
		{
			Debug.Assert(item.material.HasInt("UseTex"), "材质错误", item.gameObject);
			item.material.SetInt("UseTex", active ? 1 : 0);
		}
	}


	public bool OverlapOther(ICollisionCtrl other)
	{
		foreach (var itemOtherCollider in other.GetColliders_CollisionTest(MyPartType))
		{
			if (!itemOtherCollider.enabled || !itemOtherCollider.gameObject.activeSelf) continue;
			foreach (var itemSelfCollider in GetColliders_CollisionTest(other.MyPartType))
			{
				if (!itemSelfCollider.enabled || !itemSelfCollider.gameObject.activeSelf) continue;
				var dis = itemOtherCollider.Distance(itemSelfCollider);
				if (dis.isOverlapped && dis.distance < -0.002f)
				{
					//Debug.Log($"{itemSelfCollider.gameObject.name} - {itemOtherCollider.gameObject.name} = {dis.distance}");
					return true;
				}
			}
		}
		return false;
	}

	public void IgnoreCollision(ICollisionCtrl other)
	{
		foreach (var item in GetPhysicsColliders)
		{
			foreach (var itemOther in other.GetPhysicsColliders)
			{
				Physics2D.IgnoreCollision(item, itemOther);
			}
		}
	}
}
