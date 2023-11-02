using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static NiceButtonWithState;

/// <summary>
/// 零件的编辑视图
/// </summary>
public class MainViewEdit : BaseView
{
    [Header("删除和创建")]
    [SerializeField]
    private Button _buttonDesignSucced;
    [SerializeField]
    private Button _buttonDelete;
    [Header("显示底部详细设置面板")]
    [SerializeField]
    private NiceButtonWithState _buttonDisplayMoreSettingsPanle;
    [Header("左侧铰接面板")]
    [SerializeField]
    private NiceButtonWithState _buttonDisplayConnectPanle;
    [Header("颜色选择按钮-原始")]
    [SerializeField]
    private Button _colorSelectOriginBtn;
    [Header("坐标输入框")]
    [SerializeField]
    private TMP_InputField _xInput;
    [SerializeField]
    private TMP_InputField _yInput;

    // --------------------
    // --- 公有成员
    // --------------------
    public ChildViewEdit_Functions FunctionsView => GetComponentInChildren<ChildViewEdit_Functions>();
    public ChildViewEdit_Connect ConnectView => GetComponentInChildren<ChildViewEdit_Connect>();

    // --------------------
    // --- 私有成员
    // --------------------
    private CircleSlider RotateCicle
    {
        get
        {
            if (_rotateCicle == null)
            {
                _rotateCicle = Instantiate(GameConfig.Instance.PartRoateCircle).GetComponent<CircleSlider>();
                _rotateCicle.gameObject.SetActive(false);
            }
            return _rotateCicle;
        }
    }
    private CircleSlider _rotateCicle;
    private ModelEdit Model => ModelEdit.Instance;
    // 拖拽结束的标识，没有拖拽时NULL，拖拽时Fasle，当前帧没有拖拽但是Flag有值且False则位
    private bool? finishedDragFlag = null;
    // --------------------
    // --- Unity消息
    // --------------------

    protected override void Awake()
    {
        base.Awake();
        _buttonDelete.onClick.AddListener(OnClick_Delete);
        _buttonDesignSucced.onClick.AddListener(OnClick_EditSucced);
        _buttonDisplayMoreSettingsPanle.onStateClick.AddListener(OnStateButtonClick_DisplayOrHideSettingsPanel);
        _buttonDisplayConnectPanle.onStateClick.AddListener(OnStateButtonClick_DisplayOrHideConnectionPanel);
		CreateColorSelectBtns();
        RotateCicle.OnValueChanged += OnValueChanged_Rotation;
		//_xInput.onEndEdit.AddListener(OnInputFiled_PositionX);
  //      _yInput.onEndEdit.AddListener(OnInputFiled_PositionY);

        //PartDragManager.Instance.OnStartDragPlayerPart.AddListener(delegate { Model.SetDirty(); });
        //PartDragManager.Instance.OnEndDragPlayerPart.AddListener(delegate { Model.SetDirty(); });
	}

    protected override void Start()
    {
        base.Start();
        ExitView();//???
    }

	private void Update()
	{
		if (Time.frameCount % 3 == 0)
		{
			if (ModelEdit.Instance.GetConnectMain != null)
			{
                UpdateEditingUI(ModelEdit.Instance.EditingPlayerPartCtrl as PlayerPartCtrl);
			}
		}
	}

	private void LateUpdate()
	{
        UpdatePartColor();
        UpdateBearingColor();
    }

	private void OnDrawGizmos()
	{
        Vector3 screenSize = GetComponent<RectTransform>().rect.size;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(screenSize/2, screenSize);
	}

	// --------------------
	// --- 私有方法
	// --------------------
	private void OnValueChanged_Rotation(float value)
    {
        ModelEdit.Instance.EditingPlayerPartCtrl.Rotation = Quaternion.Euler(0, 0, value);
	}

    /// <summary>
    /// 设计完成
    /// </summary>
    private void OnClick_EditSucced()
    {
        //ControllerEdit.Instance.EditFinish_Succeed();
        ControllerEdit.Instance.EditFinish(true);
        UIManager.Instance.OpenView<MainViewCreate>();
    }
    /// <summary>
    /// 删除
    /// </summary>
    private void OnClick_Delete()
    {
        ControllerEdit.Instance.EditFinish(false);
        UIManager.Instance.OpenView<MainViewCreate>();
		if (PlayerPartManager.Instance.AllColliders.Find((a) => { return a == null; }))
		{
            Debug.LogError("删除零件导致空的碰撞器存在于列表");
        }
    }

    /// <summary>
    /// 设置尺寸等的底部界面
    /// </summary>
    private void OnStateButtonClick_DisplayOrHideSettingsPanel(NiceButtonWithState btn)
    {
        RectTransform rt = btn.GetComponent<RectTransform>();
        RectTransform movePanel = btn.Obj.GetComponent<RectTransform>();
        btn.ReverseState();
        btn.enabled = false;
        bool isOpening = btn.State == ButtonState.ON;
        movePanel.DOAnchorPosY(movePanel.anchoredPosition.y * -1, .1f).onComplete += () => 
        {
            btn.enabled = true;
            btn.GetComponent<RectTransform>().DOLocalRotate(btn.GetComponent<RectTransform>().localRotation.eulerAngles * -1, .8f);
			if (isOpening)
			{
                RotateCicle.Display(ModelEdit.Instance.EditingPlayerPartCtrl);
			}
			else
			{
                RotateCicle.Hide();
			}
        };
    }

    /// <summary>
    /// 左侧交接面板显现
    /// </summary>
    /// <param name=""></param>
    private void OnStateButtonClick_DisplayOrHideConnectionPanel(NiceButtonWithState btn)
    {
        RectTransform rt = btn.GetComponent<RectTransform>();
        RectTransform movePanel = btn.Obj.GetComponent<RectTransform>();
        btn.ReverseState();
        bool isOn = btn.State == ButtonState.ON;
        btn.enabled = false;
        movePanel.DOAnchorPosX(-1 * movePanel.anchoredPosition.x, .1f).onComplete+=()=>
        {
            btn.GetComponent<RectTransform>().DOLocalRotate(Vector3.forward * (isOn?0f:180f), .8f);
            btn.enabled = true;
        };
    }

    //private void OnInputFiled_PositionX(string text)
    //{
    //    //Vector3 pos = ModelEdit.Instance.ConnectMasterData.GetPlayerPart.Position;
    //    //ModelEdit.Instance.ConnectMasterData.GetPlayerPart.Position = new Vector3(float.Parse(text), pos.y, pos.z);
    //}

    //private void OnInputFiled_PositionY(string text)
    //{
    //    //Vector3 pos = ModelEdit.Instance.ConnectMasterData.GetPlayerPart.Position;
    //    //ModelEdit.Instance.ConnectMasterData.GetPlayerPart.Position = new Vector3(pos.x, float.Parse(text), pos.z);
    //}


    private void CreateColorSelectBtns()
    {
        int step = 2;
        Button[] cloneColorItems = new Button[360/step];
        cloneColorItems[0] = _colorSelectOriginBtn;
        for (int i = 1; i < cloneColorItems.Length; i++)
        {
            cloneColorItems[i] = Instantiate(_colorSelectOriginBtn, _colorSelectOriginBtn.transform.parent);
            cloneColorItems[i].name = i.ToString();
        }
        for (int i = 0; i < cloneColorItems.Length; i++)
        {
            int input = i*step;
            cloneColorItems[i].transform.GetChild(0).GetComponent<Graphic>().color = Color.HSVToRGB(input/360f, 1f, 1f);
            cloneColorItems[i].GetComponentInChildren<NiceButton>().onClick.AddListener(() => { ControllerEdit.Instance.SetMasterPartHueOffset(input); });
        }
    }

    private void DisplayRotateCircle(bool display = true)
    {
		if (display)
		{
            _rotateCicle.transform.position = ModelEdit.Instance.EditingPlayerPartCtrl.MyEditPartAccesstor.transform.position;
		}
        _rotateCicle.gameObject.SetActive(display);
    }

    /// <summary>
    /// 重新设置所有零件的shader
    /// </summary>
    private void UpdatePartColor()
    {
		if (PartDragManager.Instance.IsDraging)
		{
            finishedDragFlag = false;
		}
		if (finishedDragFlag == false && PartDragManager.Instance.IsDraging == false) // 上一帧正在拖拽，这一帧没有拖拽
		{
            finishedDragFlag = true;
		}
        bool cleanModelDirty = false;
        // 遍历场景零件和玩家零件
		for (int i = 0; i < PartManager.Instance.AllPartCount; i++)
        {
            IPartSetShader itemSetShaderPart = PartManager.Instance.GetPart(i);
            if (Model.IsDirty_PartColor && Model.CurrentConnectCursorPos.HasValue) // 正在连接
            {
                if (itemSetShaderPart == Model.GetConnectMain)
                {
                    PartColorManager.Instance.SetMaterial_Connect_MainPart(itemSetShaderPart);
                    continue;
                }
                else if (itemSetShaderPart == Model.WillConnectTarget)
                {
                    PartColorManager.Instance.Setmaterial_Connect_TargetPart(itemSetShaderPart);
                    continue;
                }
                if (Model.ConnectablePartType.HasValue && (itemSetShaderPart is PlayerPartCtrl) && (itemSetShaderPart as PlayerPartCtrl).MyPartType != Model.ConnectablePartType) // 过滤是否涉及场景零件呢？
                {
                    PartColorManager.Instance.SetMaterial_Connect_UnconnectablePart(itemSetShaderPart);
                }
                else
                {
                    PartColorManager.Instance.SetMaterial_Connect_UnconnectablePart(itemSetShaderPart);
                }
                cleanModelDirty = true;
            }
            else if (PartDragManager.Instance.IsDraging) // 正在拖拽
            {
                if (itemSetShaderPart == PartDragManager.Instance.DragingPart)
                {
                    PartColorManager.Instance.SetMaterial_Drag_DragingPart(itemSetShaderPart);
                }
                else if (SnapableBase.SnapTouchedPart != null && SnapableBase.SnapTouchedPart == itemSetShaderPart)
                {
                    PartColorManager.Instance.SetMaterial_Drag_Touched(itemSetShaderPart);
                }
                else
                {
                    // 区分此时会被忽略碰撞和正常碰撞的其他零件
                    if ((itemSetShaderPart as BasePartCtrl).OverlapOther(PartDragManager.Instance.DragingPart))
                    {
                        PartColorManager.Instance.SetMaterial_Drag_Overlaped(itemSetShaderPart);
                    }
                    else
                    {
                        PartColorManager.Instance.SetMaterial_Drag_Unoverlaped(itemSetShaderPart);
                    }
                }
            }
            else if(Model.IsDirty_PartColor || finishedDragFlag == true)
            {
                if (itemSetShaderPart == Model.EditingPlayerPartCtrl)
                {
                    //Debug.Log("Edit Main 不为空");
                    PartColorManager.Instance.SetMaterial_EditMainPart(itemSetShaderPart);
                }
                else
                {
                    PartColorManager.Instance.SetMaterial_AsNormal(itemSetShaderPart);
                }
                cleanModelDirty = true;
            }
        }
        if(cleanModelDirty) Model.ClearDirty();
		if (finishedDragFlag.HasValue && finishedDragFlag.Value == true)
		{
            finishedDragFlag = null;
		}
    }

    private void UpdateBearingColor()
    {
        if (Model.IsDirty_BearingColor == false) return;
        // 查找轴承
        if(Model.IsFindingBearing == false) Model.IsDirty_BearingColor = false;

		foreach (var itemConn in PartConnectionManager.Instance.AllConnection)
		{
			if (IsDisplaying == false)// 处于编辑页面
			{
				itemConn.Bearing.SetDisplay(false, false);
			}
			else if (Model.GetConnectMain.OverlapPoint(itemConn.AnchorPosition)
				&& !itemConn.ConnectedParts.ContainsKey(Model.GetConnectMain)) //该轴承覆盖当前零件，而且没有连接到该零件
			{
				itemConn.Bearing.SetDisplay(true, true);
			}
			else
			{
				itemConn.Bearing.SetDisplay(true, false);
			}
		}
	}
    // --------------------
    // --- 公有方法
    // --------------------
    /// <summary>
    /// 点击清除连接
    /// </summary>
    public void OnClick_ClearConnects()
    {
        ControllerEdit.Instance.DeleteMasterConnection();
    }

    public override void EnterView()
    {
        base.EnterView();
        // 如果编辑了一个场景零件，则只需显示编辑完成按钮
        //bool hideOtherBtn = !Model.IsEditingPlayerPart;
        //_buttonDesignSucced.gameObject.SetActive(hideOtherBtn == false);
        //_buttonDelete.gameObject.SetActive(hideOtherBtn);
        //_buttonDisplayMoreSettingsPanle.gameObject.SetActive(hideOtherBtn);
        //_buttonDisplayConnectPanle.gameObject.SetActive(hideOtherBtn);
        //_colorSelectOriginBtn.gameObject.SetActive(hideOtherBtn);
        //_xInput.gameObject.SetActive(hideOtherBtn);
        //_yInput.gameObject.SetActive(hideOtherBtn);
        UpdateEditingUI(ModelEdit.Instance.EditingPlayerPartCtrl);
        GetComponent<ChildViewEdit_Functions>().DisplayModelingBtn(ModelEdit.Instance.EditingPlayerPartCtrl.IsProgrammablePart);
        GetComponent<ChildViewEdit_SteelEditor>().enabled = ModelEdit.Instance.EditingPlayerPartCtrl.MyPartType == PartTypes.Steel;
        Model.IsDirty_BearingColor = true;
    }

    public override void ExitView()
	{
        base.ExitView();
        DisplayRotateCircle(false);
		if (_buttonDisplayConnectPanle.State == ButtonState.ON)
		{
            OnStateButtonClick_DisplayOrHideConnectionPanel(_buttonDisplayConnectPanle);
		}
		if (_buttonDisplayMoreSettingsPanle.State == ButtonState.ON)
		{
            OnStateButtonClick_DisplayOrHideSettingsPanel(_buttonDisplayMoreSettingsPanle);
		}
        if (ModelEdit.Instance.IsEditing)
        {
            //ControllerEdit.Instance.EditFinish_Succeed();
            ControllerEdit.Instance.EditFinish(true);
        }
        ConnectCursor.Instance.Hide();
        foreach (PlayerPartCtrl item in PlayerPartManager.Instance.AllPlayerPartCtrls)
        {
            if (item == null)
            {
                Debug.LogError("已创建列表中有一个对象为空！");
                continue;
            }
            //item.MyEditPartAccesstor.DisplayLayerChanged(item.MyEditPartAccesstor.Layer, true);
        }
        PartConnectionManager.Instance.DispalyBearing(true);
        Model.IsDirty_BearingColor = true;
        Model.IsFindingBearing = false;
        UpdateBearingColor();
    }

    public override bool ExitCondition(BaseView willOpenView)
	{
		if (willOpenView.IsView<MainViewModeling>())
		{
            return false;
		}
        return true;
	}

	//public override void OnActiveViewChanged(BaseView openView)
	//{
	//       if (openView  != this && this.IsDisplaying == true)
	//       {
	//		if (openView == UIManager.Instance.PanelModeling)
	//		{
	//               ExitView(true);
	//		}
	//		else
	//		{
	//               ExitView();
	//		}
	//       }
	//       if (openView  == this )
	//       {
	//           EnterView();
	//       }
	//   }

	/// <summary>
	/// 设置旋转信息的滑块和文本
	/// </summary>
	public void UpdateEditingUI(PlayerPartCtrl data)
    {
        _xInput.SetTextWithoutNotify(data.Position.x.ToString());
        _yInput.SetTextWithoutNotify(data.Position.y.ToString());
        RotateCicle.SetValue(data.Rotation.eulerAngles.z, false);
    }
}
