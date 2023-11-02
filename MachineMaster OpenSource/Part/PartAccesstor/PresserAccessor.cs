using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresserAccessor : AbsPartAccessorBase
{
	// ----------------//
	// --- 序列化
	// ----------------//


	// ----------------//
	// --- 公有成员
	// ----------------//
	public GameObject Top;
	public GameObject Bottom;
	public SpringJoint2D SpringJoint;
	public SliderJoint2D SliderJoint;

	// ----------------//
	// --- 私有成员
	// ----------------//


	// ----------------//
	// --- Unity消息
	// ----------------//
	private void Awake()
	{
		SliderJoint.limits = new JointTranslationLimits2D()
		{
			min = PartConfig.Instance.PresserConfig.SliderMinValue,
			max = PartConfig.Instance.PresserConfig.SliderMaxValue
		};
	}

	// ----------------//
	// --- 公有方法
	// ----------------//

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//

}
