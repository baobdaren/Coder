using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 建模视图
/// 为可编程零件提供可视化编程
/// 提供传感器的使用
/// </summary>
public class MainViewModeling : BaseView
{   
    // ----------------- //
    // --  序列化
    // ----------------- //
    [Header("控制UI")]
    [SerializeField]
    private Button _buttonExit;
    [Header("符号创建按钮")]
    [SerializeField]
    private Button _buttonCreateCmdSympol;
    [SerializeField]
    private Button _buttonCreateFuncSympol;
    [SerializeField]
    private Button _buttonCreateOutputSympol;
    [SerializeField]
    private Button _buttonSensorOutputSympol;
    [Header("子视图")]
    [SerializeField]
    private ChildViewModeling_ExpressionList _ExpressionListView;
    [SerializeField]
    private RawImage _scannerCameraTexture;
    [Header("预览传感器")]
    [SerializeField]
    private GameObject _gameObjectPreviewWin;
    [SerializeField]
    private Button _buttonClosePreviewWin;
    [SerializeField]
    private Button _buttonConfirmSensor;
    [SerializeField]
    private Button _buttonNextSensor;
    [SerializeField]
    private Button _buttonLastSensor;
    [SerializeField]
    private TextMeshProUGUI _previewWinTipsText;
    [SerializeField]
    private RawImage _previewRenderTarget;


    // ----------------- //
    // -- 公有成员
    // ----------------- //
    public GameObject ModelingMapParent
    {
        get
        {
            if (_mapParent == null)
            {
                _mapParent = GameObject.Instantiate(GameConfig.Instance.ModelingBackGround);
                _mapParent.SetActive(true);
                _mapParent.transform.position = GameConfig.Instance.ModelingMapStartPosition;
            }
            return _mapParent;
        }
    }


    // ----------------- //
    // --  私有成员
    // ----------------- //
    private GameObject _mapParent;
    private Vector3 _editPosCameraRecord;
    private Vector3 _modelingCameraPosRecord
    {
        get 
        {
			if (_record == null)
			{
                _record = (Vector3)GameConfig.Instance.ModelingMapStartPosition + Vector3.back * 10;
            }
            return _record.Value;
        }
        set
        {
            _record = value;
        }
    }
    private Vector3? _record;
    private float _editCameraSizeRecord;
    private float _modelCameraSizeRecord;
    private ModelModeling Model => ModelModeling.Instance;
    private ControllerModeling Ctrl => ControllerModeling.Instance;


    // ----------------- //
    // --  Unity消息
    // ----------------- //
    protected override void Awake ( )
	{
        base.Awake();
        //GameCanvasMain.Instance.OpenView()
        UIManager.RenderTexCamera.targetTexture = _scannerCameraTexture.texture as RenderTexture;
        OnClicked_HidePreviewWin();
	}

	// Start is called before the first frame update
	new void Start()
    {
        _buttonExit.onClick.AddListener(OnClick_Exit);
        // 关于符号创建的按钮
        _buttonCreateOutputSympol.onClick.AddListener(()=>_ExpressionListView.DisplayExpressionListView(ExpressionBase.SorttingType.OUTPUT));
        _buttonCreateFuncSympol.onClick.AddListener(( ) => _ExpressionListView.DisplayExpressionListView(ExpressionBase.SorttingType.FUNC));
        _buttonCreateCmdSympol.onClick.AddListener(( ) => _ExpressionListView.DisplayExpressionListView(ExpressionBase.SorttingType.CMD));
        _buttonSensorOutputSympol.onClick.AddListener(( ) => _ExpressionListView.DisplayExpressionListView(ExpressionBase.SorttingType.SENSOR));
        _buttonClosePreviewWin.onClick.AddListener(OnClicked_HidePreviewWin);
        _buttonConfirmSensor.onClick.AddListener(OnClick_ConfirmSensorSelect);
        _buttonLastSensor.onClick.AddListener(( ) => { Ctrl.SwitchSelectSensor(false); });
        _buttonNextSensor.onClick.AddListener(( ) => { Ctrl.SwitchSelectSensor(true); });
        // 模式选择
        // 第一次激活模拟窗口时，准备所有的表达式单例并注册到ExpressionManager（调用一次就可以注册了）
        ExpressionManager.Instance.InitExpression();
    }


	// ----------------- //
	// 私有方法
	// ----------------- //
	private void OnClick_Exit()
    {
        //UIManager.Instance.OpenView(UIManager.Instance.PanelEdit);
        UIManager.Instance.OpenView<MainViewEdit>();
    }

    /// <summary>
    /// 退出建模界面的相机移动完成的回调
    /// </summary>
    private void OnMoveCameraBackToEdit()
    {
        Vector3 targetPos;
        if (ControllerEdit.Instance.IsCreating)
        {
            Vector2 editPartPos = ModelEdit.Instance.EditingPlayerPartCtrl.MyEditPartAccesstor.transform.position;
            targetPos = new Vector3(editPartPos.x, editPartPos.y, CameraActor.Instance.MainCamera.transform.position.z);
        }
        else
        {
            targetPos = new Vector3(0, 0, CameraActor.Instance.MainCamera.transform.position.z);
        }
        HideView();
        CameraActor.Instance.SetMoveTo(targetPos);
    }

    /// <summary>
    /// 进入建模界面
    /// </summary>
    private void OnCameraMovedToModeling ( ) 
    {
        CameraActor.Instance.SetFocusTo(_modelCameraSizeRecord);
        if (!(ModelEdit.Instance.EditingPlayerPartCtrl.IsProgrammablePart))
		{
            return;
		}
        DisplayView();
        //HideView();
        ModelingMapParent.SetActive(true);
        Ctrl.DisplayModelMap(ModelEdit.Instance.EditingPlayerPartCtrl);
    }
    /// <summary>
    /// 隐藏预览窗口
    /// </summary>
    private void OnClicked_HidePreviewWin ( )
    {
        _gameObjectPreviewWin.SetActive(false);
		if(Model.CreatingSensorSymbol)
		{
            Model.CreatingSensorSymbol.gameObject.SetActive(false);
            Destroy(Model.CreatingSensorSymbol);
            Model.CreatingSensorSymbol = null;
		}
    }
    /// <summary>
    /// 确认传感器选择
    /// </summary>
    private void OnClick_ConfirmSensorSelect ( )
    {
        _gameObjectPreviewWin.SetActive(false);
		if(ModelModeling.Instance.CurPreviewSensor == null)
		{
            OnClicked_HidePreviewWin();
		}
		else
		{
            Ctrl.SetSymbolSensor();
            Model.CreatingSensorSymbol = null;
		}
    }
    // ----------------- //
    // --  公有方法
    // ----------------- //
    public void ShowSensorPreviewWin ( Symbol sensorSymbol )
    {
        Debug.LogError("打开预览窗口");
        _gameObjectPreviewWin.SetActive(true);
        Model.CreatingSensorSymbol = sensorSymbol;
        Ctrl.SwitchSelectSensor(true);
    }
    public void ShowErrorTips ( bool show )
    {
        //_previewRenderTarget.enabled = !show;
        _previewWinTipsText.gameObject.SetActive(show);
    }
    /// <summary>
    /// 进入视图
    /// </summary>
    public override void EnterView()
    {
        base.EnterView();
        SymbolSettingView.Instance.Hide();
        _editPosCameraRecord = CameraActor.Instance.MainCamera.transform.position;
        _editCameraSizeRecord = CameraActor.Instance.MainCamera.orthographicSize;
        CameraActor.Instance.SetMoveTo(_modelingCameraPosRecord, dur:1, completeCallBack: OnCameraMovedToModeling);
	}

    public override void ExitView()
	{
        base.ExitView();
        SymbolSettingView.Instance.Hide();
        _modelingCameraPosRecord = CameraActor.Instance.MainCamera.transform.position;
        _modelCameraSizeRecord = CameraActor.Instance.MainCamera.orthographicSize;
        CameraActor.Instance.SetFocusTo(_editCameraSizeRecord);
        ModelingMapParent.SetActive(false);
        CameraActor.Instance.SetMoveTo(_editPosCameraRecord, dur:1, completeCallBack:OnMoveCameraBackToEdit);
    }
}