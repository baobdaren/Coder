using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresserFactory : AbsPartFactory
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
		PresserAccessor presserAccesstor = accesstor as PresserAccessor;
		presserAccesstor.SliderJoint.enabled = false;
		presserAccesstor.SpringJoint.enabled = false;
		foreach (var item in presserAccesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.AppliyEditConfig(item);
		}
	}


	protected override void OnCreatedAsPhysics(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		PresserAccessor presserAccesstor = accesstor as PresserAccessor;
		presserAccesstor.SliderJoint.enabled = true;
		presserAccesstor.SpringJoint.enabled = true;
		presserAccesstor.SliderJoint.useLimits = true;
		presserAccesstor.SpringJoint.distance = PartConfig.Instance.SpringConfig.SpringDistanceMaxValue;
		presserAccesstor.SpringJoint.frequency = PartConfig.Instance.PresserConfig.Frequency;
		presserAccesstor.SliderJoint.limits = new JointTranslationLimits2D()
		{
			max = PartConfig.Instance.SpringConfig.SpringDistanceMaxValue,
			min = PartConfig.Instance.SpringConfig.SpringDistanceMinValue
		};
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.ApplySimulateConfig(item);
		}
	}



	protected override void SetSize(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		PresserAccessor presserAccesstor = accesstor as PresserAccessor;
		//float limitMax = PartConfig.Instance.PresserConfig.SliderMaxValue;
		//float limitMin = PartConfig.Instance.PresserConfig.SliderMinValue;
		//float dis = 1f * part.MyCtrlData.MainSizeIndex / 100 * (limitMax - limitMin) + limitMin;
		presserAccesstor.Bottom.transform.localPosition = Vector2.down * (partCtrlData.Size);
		presserAccesstor.SpringJoint.distance = partCtrlData.Size;
	}

	// ----------------//
	// --- 类型
	// ----------------//
}
