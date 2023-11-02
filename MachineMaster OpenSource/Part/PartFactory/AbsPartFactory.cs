using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MaterialSetter;

public abstract class AbsPartFactory
{
	// ----------------//
	// --- 序列化 s
	// ----------------//


	// ----------------//
	// --- 公有成员
	// ----------------//

	/// !!!不需要再次实例化单例，访问超级工厂即可，超级工厂调用其他工厂实例
	//public static AbsPartFactory Intsance { get => SuperFactory.GetFactor<> }


	// ----------------//
	// --- 私有成员
	// ----------------//
	protected Stack<PartMaterialType> MaterialStack = new Stack<PartMaterialType>() {  };

	// ----------------//
	// --- Unity消息
	// ----------------//
		

	// ----------------// 
	// --- 公有方法
	// ----------------//
	/// <summary>
	/// 编辑模式零件只有自带的Accesstor
	/// </summary>
	/// <param name="partCtrl"></param>
	/// <returns></returns>
	public GameObject CreateEditPart(PlayerPartCtrl partCtrl)
	{
		Debug.Log("创建Edit零件 " + partCtrl.MyPartType);
		// 实例化预制体
		GameObject editPart = PartConfig.Instance.InstantiatePart(partCtrl.MyPartType, ParentsManager.Instance.ParentOfEditParts);
		foreach (Transform item in editPart.GetComponentsInChildren<Transform>())
		{
			item.gameObject.layer = GameObjectLayerManager.EditPart;
		}
		// 获取组件访问器
		partCtrl.MyEditPartAccesstor = editPart.GetComponentInChildren<AbsPartAccessorBase>(true);
		partCtrl.MyEditPartAccesstor.MyPartCtrl = partCtrl;
		CreateAsEdit(partCtrl, partCtrl.MyEditPartAccesstor);
		return partCtrl.MyEditPartAccesstor.gameObject;
	}

	/// <summary>
	/// 创建物理模拟模式的零件，物理模式零件需要实例化后添加PartBase组件
	/// </summary>
	/// <param name="part"></param>
	/// <returns></returns>
	public PlayerPartBase CreatePhysicsPart(PlayerPartCtrl partCtrl)
	{
		Debug.Log($"创建Physics零件 {partCtrl.MyPartType} 尺寸 {partCtrl.Size}");
		// 实例化
		GameObject physicsPart = PartConfig.Instance.InstantiatePart(partCtrl.MyPartType, ParentsManager.Instance.ParentOfPhysicsParts);
		// 物理零件需要PartBase
		partCtrl.MyPhysicsPart = PartConfig.Instance.AddPartBehaviorCmpnt(partCtrl.MyPartType, physicsPart, partCtrl);
		// 组件访问器
		partCtrl.MyPhysicsPart.Accessor = partCtrl.MyPhysicsPartAccesstor;
		partCtrl.MyPhysicsPart.Accessor.MyPartCtrl = partCtrl;
		CreateAsPhysics(partCtrl, partCtrl.MyPhysicsPart.Accessor);
		//ModifyPart(partCtrl, false);
		return partCtrl.MyPhysicsPart;
	}


	/// <summary>
	/// 设置编辑零件的一些属性
	/// </summary>
	/// <param name="accestor"></param>
	/// <param name="asEdit"></param> 
	/// <returns></returns>
	public void ModifyPart(PlayerPartCtrl partCtrl)
	{
		SetPart(partCtrl, true);
	}
	// ----------------//   
	// --- 私有方法
	// ----------------//
	protected abstract void SetSize(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor);
	protected abstract void OnCreatedAsEdit(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor);
	protected abstract void OnCreatedAsPhysics(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor);

	private void CreateAsEdit(PlayerPartCtrl partCtrl, AbsPartAccessorBase part)
	{
		part.transform.name = partCtrl.MyPartType.ToString();
		if (part.DoubleClickCmpnt == null)
		{
			part.DoubleClickCmpnt = part.gameObject.AddComponent<MouseDoubleClick>();
		}
		part.PartDragCmpnt.enabled = true;
		part.DoubleClickCmpnt.enabled = true;
		part.DoubleClickCmpnt.OnDoubleClick.AddListener(() =>
		{
			//PartColorManager.Instance.SetMaterial_MainPart(partCtrl);
			ControllerEdit.Instance.SetEditMainPart(partCtrl);
			UIManager.Instance.OpenView<MainViewEdit>();
		});
		partCtrl.MyEditPartAccesstor.PartDragCmpnt.OnDraging.AddListener(
		(Vector2 newPos) => 
		{ 
			partCtrl.Position = newPos; 
		});
		// 这里注册  在哪取消注册？？？
		SnapManager.Instance.RegistSnapTarget(partCtrl.MyEditPartAccesstor.PartDragCmpnt, partCtrl);
		//partCtrl.MyEditPartAccesstor.PartDragCmpnt.OnDragStart.AddListener(() => { PartConnectionManager.Instance.ClearConnection(partCtrl); });
		MaterialPropertyBlock block = new MaterialPropertyBlock();
		foreach (var item in part.AllRenders) //测试
		{
			item.material = MaterialConfig.Instance.Part;
		}
		//part.SwitchAccesstorDisplayState(PartWorkState.Normal);
		//PartColorManager.Instance.SetMaterial_EditMainPart(partCtrl);
		OnCreatedAsEdit(partCtrl, part);
		SetPart(partCtrl, true);
	}

	private void CreateAsPhysics(PlayerPartCtrl partCtrl, AbsPartAccessorBase part)
	{
		foreach (var item in part.AllRenders)
		{
			item.material = MaterialConfig.Instance.Part_Physics;
			MaterialSetter.SetPart_Physics(item, partCtrl.ColorHue);
		}
		foreach (var item in part.GetComponentsInChildren<PartLed>())
		{
			item.TurnOn(partCtrl);
		}
		GameObject.Destroy(part.PartDragCmpnt);
		OnCreatedAsPhysics(partCtrl,part);
		SetPart(partCtrl, false);
	}

	/// <summary>
	/// 设置零件属性
	/// 该方法也可以修改物理零件属性，修改物理零件属性外部不调用
	/// </summary>
	/// <param name="ctrlData"></param>
	/// <param name="asEdit"></param>
	private void SetPart(PlayerPartCtrl ctrlData, bool asEdit)
	{
		AbsPartAccessorBase editAccestor = (asEdit ? ctrlData.MyEditPartAccesstor : ctrlData.MyPhysicsPartAccesstor);
		SetSize(ctrlData, editAccestor);
		SetPositionAndRotation(ctrlData, editAccestor);
		SetColor(ctrlData, editAccestor);
		SetRenderLayer(ctrlData, editAccestor);
	}

	protected virtual void SetRenderLayer(PlayerPartCtrl partCtrl, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRenders)
		{
			item.sortingLayerID = RenderLayerManager.Instance.GetPartSortingLayer(partCtrl);
		}
	}

	protected virtual void SetColor(PlayerPartCtrl partCtrl, AbsPartAccessorBase editAccesstor)
	{

	}

	protected virtual void SetPositionAndRotation(PlayerPartCtrl partCtrl, AbsPartAccessorBase accesstor)
	{
		accesstor.transform.SetPositionAndRotation(partCtrl.Position, partCtrl.Rotation);
	}

	// ----------------//
	// --- 类型
	// ----------------//
	public enum PartEdittingType { NORMAL, SELECT, DRAG }
}
