using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static MaterialConfig;
using static PartConnectionManager;

public class ControllerEdit
{
	public static ControllerEdit Instance { get; private set; } = new ControllerEdit();


	// ----------------- //
	// --  公有成员
	// ----------------- //
	[SerializeField] public class NonArgAction : UnityEvent { }
	public NonArgAction OnStartEditPlayerPart = new NonArgAction();
	public NonArgAction OnFinishEditPart = new NonArgAction();

	public bool IsCreating => Model.EditingPlayerPartCtrl != null;

	// ----------------- //
	// --  私有成员
	// ----------------- //
	private ModelEdit Model { get => ModelEdit.Instance; }
	private ControllerEdit()
	{
		ConnectCursor.Instance.OnMoveStart.AddListener(EnterConnectingState);
		//ConnectCursor.Instance.OnMoving.AddListener(UpdateConnectingState);
		ConnectCursor.Instance.OnMoveEnd.AddListener(FinishConnect);

	}

	//-------------------//
	// --- 私有方法
	//-------------------//
	/// <summary>
	/// 进入链接状态，链接光标开始工作时回调
	/// </summary>
	private void EnterConnectingState(ConnectCursor.ConnectCursorStates state)
	{
		//Model.IsConnecting = true;
		//Model.OverlapedMain = false;
	}

	/// <summary>
	/// 结束链接状态，链接光标结束工作时回调
	/// </summary>
	private void FinishConnect(ConnectCursor.ConnectCursorStates state)
	{
		Model.ResetConnectState();
		return;
		if (state == ConnectCursor.ConnectCursorStates.Bearing||
			state == ConnectCursor.ConnectCursorStates.Fixed)
		{
			Model.ResetConnectState();
		}
	}


	/// <summary>
	/// 创建界面完成
	/// </summary>
	private void EditFinish_Succeed()
	{
		if (Model.EditingPlayerPartCtrl != null)
		{
			if ((Model.EditingPlayerPartCtrl as PlayerPartCtrl).IsSectionPart)
			{
				(Model.EditingPlayerPartCtrl as PlayerPartCtrl).UpdateDataFromAccesstor();
			}
			OnFinishEditPart?.Invoke();
			Model.EditingPartCtrl = null;
		}
		Model.ResetConnectState();
	}

	/// <summary>
	/// 创建界面删除
	/// </summary>
	private void EditFinish_Delete()
	{
		PlayerPartCtrl deletePart = Model.EditingPlayerPartCtrl;
		SnapManager.Instance.UnRegistSnapTarget(deletePart.MyEditPartAccesstor.PartDragCmpnt);
		PartConnectionManager.Instance.ClearPartConnection(deletePart);
		PlayerPartManager.Instance.DeletePart(deletePart);
		Model.EditingPartCtrl = null;
		OnFinishEditPart?.Invoke();
		Model.ResetConnectState();
	}

	//-------------------//
	// --- 公有方法
	//-------------------//
	/// <summary>
	/// 设置这个PART为编辑对象
	/// </summary>
	/// <param name="part"></param>
	public void SetEditMainPart(BasePartCtrl part)
	{
		Model.EditingPartCtrl = part;
		if (part.IsPlayerPart)
		{
			if (Model.EditingPlayerPartCtrl != part)
			{
				PartColorManager.Instance.SetMaterial_MainPart(part);
			}
			(part as PlayerPartCtrl).MyEditPartAccesstor.gameObject.SetActive(true);
			// 处理 连接图标显示
			OnStartEditPlayerPart?.Invoke(/* part.MyCtrlData.MainSizeIndex)*/);
		}
	}

	/////// <summary>
	/////// 设置铰接主题的数据
	/////// </summary>
	/////// <param name="connectPos"></param>
	//public void SetPhysicsPartsDisplay(bool active)
	//{
	//	ObjParentsManager.Instance.ParentOfEditParts.SetActive(active);
	//}

	///// <summary>
	///// 设置铰接目标数据
	///// </summary>
	///// <param name="targetCtrlData"></param>
	///// <param name="anchorPosition"></param>
	//public void SetConnectTarget( IConnectableCtrl targetCtrlData)
	//{
	//	if (targetCtrlData != Model.WillConnectTarget)
	//	{
	//		Debug.Log("更换铰接对象目标为" + targetCtrlData);
	//		Model.WillConnectTarget = targetCtrlData;
	//	}
	//	else
	//	{
	//		Debug.Log("设置铰接对象目标为" + targetCtrlData);
	//	}
	//}

	///// <summary>
	///// 重置铰接目标数据
	///// </summary>
	//public void ResetTargetConnectionState()
	//{
	//	Model.WillConnectTarget = null;
	//}

	/// <summary>
	/// 清除铰接目标和铰接坐标的数据
	/// </summary>
	//public void ResetConnectData()
	//{
	//	Debug.Log("清空连接数据");
	//	Model.WillConnectTarget = null;
	//	Model.AnchorPos = null;
	//	Model.OverlapedMain = null;
	//}

	/// <summary>
	/// 重置铰接主体数据
	/// </summary>
	public void DeleteMasterConnection()
	{
		PartConnectionManager.Instance.ClearPartConnection(Model.EditingPlayerPartCtrl as PlayerPartCtrl);
		//Model.MainPartData.part.ClearConnects();
	}

	public void EditFinish(bool createSucceed)
	{
		if (createSucceed)
		{
			EditFinish_Succeed();
		}
		else
		{
			EditFinish_Delete();
		}
	}

	public void SetMasterPartHueOffset(int offset)
	{
		Debug.LogError("设置颜色 - " + offset);
		if (Model.EditingPlayerPartCtrl != null && Model.EditingPlayerPartCtrl is PlayerPartCtrl)
		{
			(Model.EditingPlayerPartCtrl as PlayerPartCtrl).ColorHue = offset;
		}
	}
}
