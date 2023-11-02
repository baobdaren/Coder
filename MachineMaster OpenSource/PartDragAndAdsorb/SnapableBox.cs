using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SnapableBox : SnapableBase
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public override SnapBaseShapeTypes SnapBaseShapeType => SnapBaseShapeTypes.Box;
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
	protected override SnapVector SnapToBox(SnapableBox target)
	{
		throw new System.NotImplementedException();
	}

	protected override SnapVector SnapToCircle(SnapableCircle target)
	{
		throw new System.NotImplementedException();
	}

	protected override SnapVector SnapToPoint(SnapablePoint target)
	{
		throw new System.NotImplementedException();
	}

	// ----------------//
	// --- 类型
	// ----------------//

}
