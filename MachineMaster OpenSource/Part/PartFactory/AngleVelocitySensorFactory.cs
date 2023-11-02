using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AngleVelocitySensorFactory : AbsPartFactory
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

	// ----------------//
	// --- 类型
	// ----------------//
	protected override void SetSize(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		//throw new System.NotImplementedException();
	}

	protected override void OnCreatedAsEdit(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.AppliyEditConfig(item);
		}
		accesstor.PartDragCmpnt.EnableSnapBound = true;
		accesstor.PartDragCmpnt.EnableSnapCenter = true;
	}

	protected override void OnCreatedAsPhysics(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.ApplySimulateConfig(item);
		}
	}
}
