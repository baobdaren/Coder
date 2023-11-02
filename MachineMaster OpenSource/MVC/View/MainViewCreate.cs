using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


/// <summary>
/// 零件的创建视图
/// </summary>
public class MainViewCreate : BaseView
{
    // ----------------- //
    // -- 序列化
    // ----------------- //
    [SerializeField]
    private NiceButton CancleButton;

	// ----------------- //
	// -- Unity 消息
	// ----------------- //
	protected override void Awake()
	{
		base.Awake();
		CancleButton.OnLeftClick.AddListener(OnClicked_Cancle);
	}
	// ----------------- //
	// -- 公有成员
	// ----------------- //

	// ----------------- //
	// -- 公有方法
	// ----------------- //
	/// <summary>
	/// 创建零件名称，再面板中调用
	/// </summary>
	/// <param name="prefabName"></param>
	public void OnClick_CreatePart(PartTypes partType)
    {
        PlayerPartCtrl ctrlData = PartSuperFactory.CreateEditPart(partType);
        ctrlData.MyEditPartAccesstor.transform.SetParent(ParentsManager.Instance.ParentOfEditParts.transform);
        ctrlData.MyEditPartAccesstor.enabled = true;
        ctrlData.MyEditPartAccesstor.PartDragCmpnt.ForceDrag(true);
        ControllerEdit.Instance.SetEditMainPart(ctrlData);
		UIManager.Instance.OpenView<MainViewEdit>();
    }
    // ----------------- //
    // -- 私有方法
    // ----------------- //
    private void OnClicked_Cancle()
    {
        UIManager.Instance.OpenView<MainViewStart>();
    }
}