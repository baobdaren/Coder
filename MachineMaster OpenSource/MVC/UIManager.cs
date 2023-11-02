using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 关卡视图的管理
/// </summary>
public class UIManager : MonoSinglton<UIManager>
{

	// ---------- //
	// -- 序列化
	// ---------- //
	[SerializeField]
	private TextMeshProUGUI _mainTitleText;



	// ---------- //
	// -- 私有属性
	// ---------- //
	private Dictionary<string, BaseView> ViewDic;


	// ---------- //
	// -- 公有属性
	// ---------- //
	//public ViewDesignParts PanelCanvasSttings
	//{
	//	get
	//	{
	//		if(_panelCanvasSttings == null)
	//		{
	//			_panelCanvasSttings = Instantiate(AssetLoader.LoadView<ViewDesignParts>(), transform).GetComponent<ViewDesignParts>();
	//		}
	//		return _panelCanvasSttings;
	//	}
	//}
	public MainViewSimulate PanelSimulate { private set; get; }
	public MainViewCreate PanelCreate { private set; get; }
	public MainViewEdit PanelEdit { private set; get; }
	public MainViewStart PanelMain { private set; get; }
	public MainViewModeling PanelModeling{private set; get;}
	public MainViewScenePart PanelScenePart{private set; get; }
	//private ViewPartGroup _panelPartGroupList;
	//public ViewPartGroup PanelPartGroupList
	//{
	//	get
	//	{
	//		if (_panelPartGroupList == null)
	//		{
	//			_panelPartGroupList = Instantiate(GameConfig.Instance.View_LayerFilter, transform).GetComponent<ViewPartGroup>();
	//		}
	//		return _panelPartGroupList;
	//	}
	//}


	public BaseView PanelDisplaying
	{
		private set; get;
	}
	public BaseView PanelLastOpened
	{
		private set; get;
	}

	/// <summary>
	/// 用于预览传感器位置的相机
	/// </summary>
	private static Camera _renderTexCamera;
	public static Camera RenderTexCamera
	{
		get
		{
			if (_renderTexCamera == null)
			{
				GameObject obj = new GameObject("Camer_RenderSensorTexture");
				_renderTexCamera = obj.AddComponent<Camera>();
				_renderTexCamera.orthographic = false;
			}
			return _renderTexCamera;
		}
	}

	// ---------------- //
	// -- Unity消息
	// ---------------- //
	private void Awake()
	{
		PanelSimulate = Instantiate(GameConfig.Instance.View_Simulate, transform).GetComponentInChildren<MainViewSimulate>(true);
		PanelCreate = Instantiate(GameConfig.Instance.View_Create, transform).GetComponentInChildren<MainViewCreate>(true);
		PanelEdit = Instantiate(GameConfig.Instance.View_Edit, transform).GetComponentInChildren<MainViewEdit>(true);
		PanelMain = Instantiate(GameConfig.Instance.View_Main, transform).GetComponentInChildren<MainViewStart>(true);
		PanelModeling = Instantiate(GameConfig.Instance.View_Modeling, transform).GetComponentInChildren<MainViewModeling>(true);
		PanelScenePart = Instantiate(GameConfig.Instance.View_ScenePartEdit, transform).GetComponentInChildren<MainViewScenePart>(true);

		ViewDic = new Dictionary<string, BaseView>()
		{
			[PanelSimulate.GetType().ToString()] = PanelSimulate,
			[PanelCreate.GetType().ToString()] = PanelCreate,
			[PanelEdit.GetType().ToString()] = PanelEdit,
			[PanelMain.GetType().ToString()] = PanelMain,
			[PanelModeling.GetType().ToString()] = PanelModeling,
			[PanelScenePart.GetType().ToString()] = PanelScenePart,
		};
	}

	private void Start()
	{
		ModelSimulate.ResetData();
		ModelEdit.ResetData();
		ModelModeling.ResetData();
		ModelLevel.ResetData();
		foreach (var item in ViewDic)
		{
			item.Value.HideView();
		}

		GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
		//GetComponent<Canvas>().worldCamera = CameraActor.Instance.MainCamera;
		GetComponent<Canvas>().worldCamera = Camera.main;

		Debug.Log("初始化UI 显示");
		UIManager.Instance.Display();
	}

	private void Update()
	{
		PrintConsole();
	}

	// ---------- //
	// -- 私有方法
	// ---------- //
	private void PrintConsole()
	{
		if (Time.frameCount % 10 == 0 && PlayerPartManager.Instance.AllPlayerPartCtrls != null)
		{
			int colliderCount = PlayerPartManager.Instance.AllColliders.Count;
			_mainTitleText.text = $"当前刚体数量：{colliderCount}";
			_mainTitleText.text += Environment.NewLine + $"当前零件数量：{PlayerPartManager.Instance.AllPlayerPartCtrls.Count}";
			_mainTitleText.text += Environment.NewLine + $"当前连接数量：{PartConnectionManager.Instance.AllConnection.Count}";
			_mainTitleText.text += Environment.NewLine + $"当前场景零件数量：{LevelProgressBase.Instance.AllScenePartList.Count}";
			_mainTitleText.text += Environment.NewLine + $"当前场景吸附对象：{SnapManager.Instance.AllSnapableObjects.Count}";
			if (ModelEdit.Instance.IsEditingPlayerPart)
			{
				var dragCmpnt = ModelEdit.Instance.EditingPlayerPartCtrl.MyEditPartAccesstor.PartDragCmpnt;
				if (dragCmpnt.IsDraging)
				{
					_mainTitleText.text += Environment.NewLine + $"当前场景吸附偏移：{dragCmpnt.ConsoleReadOffset}";
				}
			}
		}
	}



	//-------------//
	//-- 公有方法
	//-------------//
	/// <summary>
	/// 切换视图
	/// </summary>
	/// <param name="view"></param>
	//public void OpenView(BaseView view = null)
	//{
	//	if (view == null)
	//	{
	//		view = PanelMain;
	//	}
	//	Debug.Log("切换视图为：" + view.GetType());
	//	PanelLastOpened = PanelDisplaying;
	//	PanelDisplaying = view;
	//	//PanelCanvasSttings.OnActiveViewChanged(to);
	//	foreach (var item in ViewDic)
	//	{
	//		item.Value.SwitchView(view);
	//	}
	//	//PanelPartGroupList.OnActiveViewChanged(to);
	//}

	/// <summary>
	/// 切换到视图
	/// </summary>
	public void OpenView<T>() where T : BaseView
	{
		var viewSwitchTo = ViewDic[typeof(T).ToString()];
		PanelDisplaying = viewSwitchTo;
		foreach (var item in ViewDic)
		{
			item.Value.SwitchView(viewSwitchTo);
		}
	}

	/// <summary>
	/// 隐藏当前的视图
	/// </summary>
	public void Hide()
	{
		if (PanelDisplaying)
		{
			PanelDisplaying.HideView();
		}
	}

	/// <summary>
	/// 显示当前的视图
	/// </summary>
	public void Display()
	{
		if (PanelLastOpened != null)
		{
			PanelDisplaying.DisplayView();
		}
		else
		{
			OpenView<MainViewStart>();
		}
	}


	public void OnSwipe_MoveCamera(Vector2 gs)
	{
		//if (Input.GetMouseButtonDown(2))
		if (Mouse.current.rightButton.isPressed)
		{
			CameraActor.Instance.MainCamera.transform.position += (Vector3)gs * CameraActor.Instance.MainCamera.orthographicSize * -1 / 400;
		}
	}
}
