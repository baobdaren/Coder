using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DistanceSensorFactory : AbsPartFactory
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
	protected override void SetSize(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accessor)
	{
		//throw new System.NotImplementedException();
	}

	protected override void OnCreatedAsEdit(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accessor)
	{
		foreach (var item in accessor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.AppliyEditConfig(item);
		}
		accessor.PartDragCmpnt.EnableSnapBound = true;
		(accessor as DistanceSensorAccessor)._lineR.enabled = false;
	}

	protected override void OnCreatedAsPhysics(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accessor)
	{
		foreach (var item in accessor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.ApplySimulateConfig(item);
		}
		(accessor as DistanceSensorAccessor)._lineR.enabled = true;
	}
}
