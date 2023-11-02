using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class GearFactory : AbsPartFactory
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
		accesstor.PartDragCmpnt.EnableSnapCenter = true;
	}

	protected override void OnCreatedAsPhysics(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.ApplySimulateConfig(item);
		}
	}

	protected override void SetSize(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		GearAccessor gearAccesstor = accesstor as GearAccessor;
		List<GameObject> gearHeads = gearAccesstor.OriginGeadHeads;
		int HeadAmount = (int)partCtrlData.Size * 2 + 10;
		// 齿头半径任意，根据视觉比例调整即可
		float gearMiddleRadius = 0.1979f + (float)partCtrlData.Size * (0.4744f - 0.1979f)/7f;
		//float GearMiddleRadius = 0.158f + (float)partCtrlData.Size * (0.421f - 0.158f)/7f + gearAccesstor.ToothRadius;
		float gearMiddleRadius2 = (0.1979f + gearAccesstor.ToothRadius) + (float)partCtrlData.Size * (0.421f - (0.1949f + gearAccesstor.ToothRadius)) /7f;
		float gearTopRadius = (0.1979f + gearAccesstor.ToothRadius*2f) + (float)partCtrlData.Size * (0.421f - (0.1949f + gearAccesstor.ToothRadius*2f)) /7f;
		//Debug.Log($"齿中圆半径 {GearMiddleRadius}");
		for (int i = 0; i < gearHeads.Count; i++)
		{
			gearHeads[i].gameObject.SetActive(i < HeadAmount);
			if (i >= HeadAmount) // 多余的节点禁用
			{
				continue;
			}
			float ag = (1f * i / HeadAmount * 360 - 90f) * Mathf.Deg2Rad;
			gearHeads[i].transform.localPosition =
				new Vector3(gearMiddleRadius * Mathf.Cos(ag), gearMiddleRadius * Mathf.Sin(ag), 0);
			gearHeads[i].transform.localEulerAngles = new Vector3(0, 0, ag*Mathf.Rad2Deg-90);
			gearHeads[i].transform.name = i.ToString();
		}
		// 齿身原始大小
		//float n = gearAccesstor.GearBody.GetComponent<SpriteRenderer>().sprite.rect.width / gearAccesstor.GearBody.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
		gearAccesstor.MiddleCircle.radius = gearMiddleRadius;
		//gearAccesstor.GearBody.transform.localScale = (GearMiddleRadius - gearAccesstor.ToothRadius) * 2 * Vector3.one / n;
		gearAccesstor.GearBodyCircle.radius = gearMiddleRadius - gearAccesstor.ToothRadius;
		gearAccesstor.GearTopRadius.radius = gearMiddleRadius + gearAccesstor.ToothRadius;
		gearAccesstor.SetGearDisplay((int)partCtrlData.Size);
		return;
	}


	// ----------------//  
	// --- 类型
	// ----------------//
}
