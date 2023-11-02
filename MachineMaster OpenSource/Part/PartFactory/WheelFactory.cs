using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelFactory : AbsPartFactory
{
	// ----------------//
	// --- 序列化
	// ----------------//


	// ----------------//
	// --- 公有成员
	// ----------------//


	// ----------------//
	// --- 私有成员
	// ----------------//


	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//


	// ----------------//
	// --- 私有方法
	// ----------------//

	protected override void OnCreatedAsEdit(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.AppliyEditConfig(item);
		}
		accesstor.PartDragCmpnt.EnableSnapBound = true;
	}


	protected override void OnCreatedAsPhysics(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.ApplySimulateConfig(item);
		}
	}

	//protected override void SetLayer(Wheel part, int layerIndex)
	//{
	//	foreach (var item in part.GetComponentsInChildren<Transform>())
	//	{
	//		item.gameObject.layer = layerIndex;
	//	}
	//	foreach (var item in part.GetComponentsInChildren<SpriteRenderer>())
	//	{
	//		item.sortingLayerID = GameLayerManager.PartLayerToSortingLayerID(layerIndex);
	//	}
	//}


	protected override void SetSize(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		accesstor.transform.localScale = Vector2.one * partCtrlData.Size;
	}


	// ----------------//
	// --- 类型
	// ----------------//
}
