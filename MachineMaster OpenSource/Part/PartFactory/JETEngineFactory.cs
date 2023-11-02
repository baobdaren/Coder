using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JETEngineFactory : AbsPartFactory
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
		(accesstor as JETEngineAccessor).FlameVFX.gameObject.SetActive(false);
	}

	protected override void OnCreatedAsPhysics(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.ApplySimulateConfig(item);
		}
		(accesstor as JETEngineAccessor).FlameVFX.gameObject.SetActive(true);
		accesstor.AllRigids[0].useAutoMass = false;
		accesstor.AllRigids[0].mass = 0.5f;
	}


	protected override void SetSize(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		
	}

	// ----------------//
	// --- 类型
	// ----------------//
}
