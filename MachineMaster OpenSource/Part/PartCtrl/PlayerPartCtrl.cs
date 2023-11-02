using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 提供零件的控制数据访问和修改，修改数据自动刷新编辑状态的零件
/// 数据修改必须全部经过该类，该类则回使用工厂以最新的数据来修改编辑状态的零件
/// </summary>
public class PlayerPartCtrl:BasePartCtrl
{
	// ----------------//
	// --- 公有成员
	// ----------------//
	public PartCtrlCoreData CoreData { get; private set; }

	public override PartTypes MyPartType  => CoreData.MyPartType; 
	public PartModelingNodeMap ModelMap
	{
		get
		{
			return ModelMapManager.Instance.CreateMap(this);
		}
	}
	public Vector2 Position
	{
		get => CoreData.Position;
		set { CoreData.Position = value; PartSuperFactory.ModifyEditPart(this); }
	}
	public Quaternion Rotation
	{
		get => CoreData.Rotation;
		set { CoreData.Rotation = value; PartSuperFactory.ModifyEditPart(this); }
	}

	public float Size
	{
		get => CoreData.Size;
		set { CoreData.Size = value; PartSuperFactory.ModifyEditPart(this); }
	}
	public int Layer
	{
		get;set;
	}

	public int ColorHue
	{
		get => CoreData.ColorOffset;
		set
		{
			CoreData.ColorOffset = value;
			PartSuperFactory.ModifyEditPart(this);
		}
	}

	public bool IsProgrammablePart
	{
		get => MyPartType == PartTypes.Engine || MyPartType == PartTypes.JETEngine || MyPartType == PartTypes.Presser;
	}
	public bool IsSectionPart
	{
		get => MyPartType == PartTypes.Rail || MyPartType == PartTypes.Rope;
	}

	public AbsPartAccessorBase MyEditPartAccesstor;
	public PlayerPartBase MyPhysicsPart;
	public AbsPartAccessorBase MyPhysicsPartAccesstor
	{
		get
		{
			if (_physicsAccesstor == null)
			{
				_physicsAccesstor = MyPhysicsPart.GetComponentInChildren<AbsPartAccessorBase>();
				_physicsAccesstor.gameObject.layer = GameObjectLayerManager.DefaultPart;
				_physicsAccesstor.MyPartCtrl = this;
			}
			return _physicsAccesstor;
		}
	}

	public override List<Collider2D> GetEditConnectableColliders => MyEditPartAccesstor.AllConectableColliders;

	public override List<Collider2D> GetPhysicsConnectablrColliders => MyPhysicsPartAccesstor.AllConectableColliders;

	public override List<Collider2D> GetIgnorableColliders => MyPhysicsPartAccesstor.AllColliders;

	protected override List<Renderer> EditRenders => MyEditPartAccesstor.AllRenders;

	public override bool IsPlayerPart => true;

	public override IEnumerable<Collider2D> GetPhysicsColliders => MyPhysicsPartAccesstor.AllColliders;

	// ----------------//
	// --- 私有成员
	// ----------------//
	private AbsPartAccessorBase _physicsAccesstor;

	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	public PlayerPartCtrl(PartTypes t)
	{
		CoreData = new PartCtrlCoreData(t);
	}
	/// <summary>
	/// 加载存档时需要直接从数据创建
	/// </summary>
	/// <param name="partCtrlData"></param>
	public PlayerPartCtrl(PartCtrlCoreData partCtrlData)
	{
		CoreData = partCtrlData;
	}

	/// <summary>
	/// 主动更新数据
	/// 多节组合的零件（绳子和铰链），其每一节都需要保存位置
	/// 更新主体位置和旋转以及层级
	/// </summary>
	public void UpdateDataFromAccesstor()
	{
		if (IsSectionPart)
		{
			List<GameObject> sections = (MyEditPartAccesstor as AbsSectionPartAccesor).SectionList;
			CoreData.SectionDataList = new List<(Vector3, Quaternion)>(sections.Count);
			for (int i = 0; i < sections.Count; i++)
			{
				CoreData.SectionDataList.Add((sections[i].transform.position, sections[i].transform.rotation));
			}
		}
		//MainPosition = MyEditPartAccesstor.transform.position;
		//MainRotation = MyEditPartAccesstor.transform.rotation;
		//LayerIndex = MyEditPartAccesstor.gameObject.layer;
	}

	public void ApplyDataToAccesstor()
	{
		if (IsSectionPart)
		{
			List<GameObject> sections = (MyEditPartAccesstor as AbsSectionPartAccesor).SectionList;
			for (int i = 0; i < sections.Count; i++)
			{
				sections[i].transform.SetPositionAndRotation(CoreData.SectionDataList[i].Item1, CoreData.SectionDataList[i].Item2);
			}
		}
		MyEditPartAccesstor.transform.SetPositionAndRotation(Position, Rotation);
		MyEditPartAccesstor.gameObject.layer = Layer;
	}

	public override IEnumerable<Collider2D> GetColliders_CollisionTest(PartTypes partType)
	{
		return MyEditPartAccesstor.GetCollidersForConflictTest(partType);
	}

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
