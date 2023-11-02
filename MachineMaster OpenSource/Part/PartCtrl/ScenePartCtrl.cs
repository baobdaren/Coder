using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ScenePartCtrl : BasePartCtrl
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public Hash128 GetID { get => _scenePart.GetPartHashID; }
	/// <summary>
	/// 暂时设定场景零件没有编辑状态和物理状态区分
	/// </summary>
	public ScenePart PhysicsPart
	{
		get { return _scenePart; }
	}

	public override List<Collider2D> GetEditConnectableColliders => _scenePart.ConnectableColliders;
	public override List<Collider2D> GetPhysicsConnectablrColliders => _scenePart.ConnectableColliders;
	public override List<Collider2D> GetIgnorableColliders => PhysicsPart.AllColliders;
	protected override List<Renderer> EditRenders => _scenePart.AllRenders;

	public override PartTypes MyPartType => _scenePart.IsGear ? PartTypes.Gear : PartTypes.ScecneCustom;

	public override bool IsPlayerPart => false;

	public override IEnumerable<Collider2D> GetPhysicsColliders => PhysicsPart.AllColliders;


	// ----------------//
	// --- 私有成员
	// ----------------//
	private ScenePart _scenePart;

	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	public ScenePartCtrl(ScenePart scenePart)
	{ 
		_scenePart = scenePart;
	}

	public override IEnumerable<Collider2D> GetColliders_CollisionTest(PartTypes partType)
	{
		if (partType == PartTypes.Gear && _scenePart.IsGear)
		{
			return new List<Collider2D>() { _scenePart.GearMiddleCircle };
		}
		else
		{
			return _scenePart.AllColliders;
		}
	}

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
