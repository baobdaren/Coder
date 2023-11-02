using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SnapableGear : SnapableBase
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
	public override bool Snapable => true;
	public override SnapBaseShapeTypes SnapBaseShapeType => SnapBaseShapeTypes.Gear;

	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	protected override SnapVector SnapToGear(SnapableGear target)
	{
		return SnapCircleAndCircle(GearMiddleCircle, target.GearMiddleCircle);
	}

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
