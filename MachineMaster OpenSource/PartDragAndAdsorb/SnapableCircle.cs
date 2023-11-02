using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SnapableCircle : SnapableBase
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public override SnapBaseShapeTypes SnapBaseShapeType => SnapBaseShapeTypes.Circle;
	public override bool Snapable => true;

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

	protected override SnapVector SnapToBox(SnapableBox target)
	{
		throw new System.NotImplementedException();
	}

	protected override SnapVector SnapToCircle(SnapableCircle target)
	{
		return SnapCircleAndCircle(this.CircleBound, target.CircleBound);
	}

	protected override SnapVector SnapToPoint(SnapablePoint target)
	{
		return SnapPointAndCircle(target.Point.position, CircleBound).Reverse();
	}
}
