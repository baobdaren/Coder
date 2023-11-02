using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SnapablePoint : SnapableBase
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
	public override SnapBaseShapeTypes SnapBaseShapeType => SnapBaseShapeTypes.Point;
	public override bool Snapable => true;

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
		return SnapPointAndCircle(this.Point.transform.position, target.CircleBound);
	}

	protected override SnapVector SnapToPoint(SnapablePoint target)
	{
		return new SnapVector(target.Point.position - Point.position);
	}
}
