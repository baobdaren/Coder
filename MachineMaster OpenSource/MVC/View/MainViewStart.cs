using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 关卡主界面，默认视图
/// </summary>
public class MainViewStart : BaseView
{
    // --------------------
    // --- 序列化
    // --------------------
    [ChildGameObjectsOnly][SerializeField]
    Button _buttonExit;
    [ChildGameObjectsOnly]
    [SerializeField]
    Button _buttonDesign;
    [SerializeField]
    [ChildGameObjectsOnly]
    Button _buttonSimulate;
    [SerializeField]
    [ChildGameObjectsOnly]
    Button _buttonReplay;

    // --------------------
    // --- Unity消息
    // --------------------

    protected override void Awake()
    {
        base.Awake();
        _buttonDesign.onClick.AddListener(OnClick_BeginEdit);
        _buttonSimulate.onClick.AddListener(() => { UIManager.Instance.OpenView<MainViewSimulate>(); });
        _buttonExit.onClick.AddListener(() => ControllerStart.Instance.FinishiLevel());
        SetEditState(false);
    }

	protected override void Start()
	{
        base.Start();
        LevelProgressBase.Instance.OnEnableEdit.AddListener(()=>SetEditState(true));
        LevelProgressBase.Instance.OnDisableEdit.AddListener(()=> SetEditState(false));
	}

	// --------------------
	// --- 私有方法
	// --------------------
	public override void EnterView()
	{
		base.EnterView();
        CameraActor.Instance.SetCameraWorkeState(CameraActor.CameraWorkingStates.Follow);
    }

	public override void ExitView()
	{
		base.ExitView();
        CameraActor.Instance.SetCameraWorkeState(CameraActor.CameraWorkingStates.FreeMove);
    }

    private void SetEditState(bool editable)
    {
        _buttonDesign.gameObject.SetActive(editable);
        _buttonSimulate.gameObject.SetActive(editable);
    }

    private void OnClick_BeginEdit()
    {
        UIManager.Instance.OpenView<MainViewCreate>();
        ControllerStart.Instance.SetCameraFreeMove();
    }

    // --------------------
    // --- 公共方法
    // --------------------
}
