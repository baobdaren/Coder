using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 零件的创建采用抽象工厂模式（每种工厂都生产零件，但零件彼此不同却有共性；超级工厂提供了产品族的直接生产方式，无需直接调用工厂）
/// 这个超级工厂帮助直接调用各类工厂
/// </summary>
public class PartSuperFactory
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
	private PartSuperFactory() { }

	private static Dictionary<PartTypes, AbsPartFactory> _partTypeToFactory =
	new Dictionary<PartTypes, AbsPartFactory>()
	{
		[PartTypes.JETEngine] =  new JETEngineFactory(),
		[PartTypes.Engine] = new EngineFactory(),
		[PartTypes.Gear] = new GearFactory(),
		[PartTypes.Presser] = new PresserFactory(),
		[PartTypes.Rail] = new RailFactory(),
		[PartTypes.Rope] = new RopeFactory(),
		[PartTypes.Spring] = new SpringFactory(),
		[PartTypes.Steel] = new SteelFactory(),
		[PartTypes.Wheel] = new WheelFactory(),
		[PartTypes.AVSensor] = new AngleVelocitySensorFactory(),
		[PartTypes.DISSensor] = new DistanceSensorFactory(),
	};


	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	public static T GetFactor<T>() where T : AbsPartFactory
	{
		foreach (var item in _partTypeToFactory)
		{
			if (item.Value is T)
			{
				return item.Value as T;
			}
		}
		Debug.Assert(false, "没有找到工厂");
		return null;
	}

	/// <summary>
	/// 创建编辑状态零件控制和实例
	/// </summary>
	/// <param name="partType"></param>
	/// <returns></returns>
	public static PlayerPartCtrl CreateEditPart(PartTypes partType)
	{
		return CreateEditPart(new PartCtrlCoreData(partType));
	}

	/// <summary>
	/// 创建编辑状态零件实例
	/// </summary>
	/// <param name="partCtrl"></param>
	/// <returns></returns>
	public static PlayerPartCtrl CreateEditPart(PartCtrlCoreData partCtrlData)
	{
		PlayerPartCtrl partCtrl = new PlayerPartCtrl(partCtrlData);
		_partTypeToFactory[partCtrl.MyPartType].CreateEditPart(partCtrl);
		ModifyEditPart(partCtrl);
		PlayerPartManager.Instance.AddPart(partCtrl);
		PartDragManager.Instance.RegistPart(partCtrl);
		return partCtrl;
	}

	/// <summary>
	/// 创建编辑状态的零件
	/// 依据已有的数据来创建
	/// </summary>
	/// <param name="partCtrl"></param>
	/// <returns></returns>
	public static void ModifyEditPart(PlayerPartCtrl partCtrl)
	{
		_partTypeToFactory[partCtrl.MyPartType].ModifyPart(partCtrl);
	}

	public static void CreatePhysicsPart(PlayerPartCtrl partCtrl)
	{
		PlayerPartBase result = _partTypeToFactory[partCtrl.MyPartType].CreatePhysicsPart(partCtrl);
		if (result == null)
		{
			Debug.LogError("创建物理零件失败 - " + partCtrl.MyPartType);
		}
		partCtrl.MyPhysicsPart = result;
	}

	// ----------------//
	// --- 私有方法
	// ----------------//
	private static GameObject CreateEditPartGameObject(PlayerPartCtrl partCtrl)
	{
		return _partTypeToFactory[partCtrl.MyPartType].CreateEditPart(partCtrl);
	}

	// ----------------//
	// --- 类型
	// ----------------//
}
