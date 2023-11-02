using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

public class CameraActor : MonoBehaviour
{
	[SerializeField]
	private CinemachineVirtualCamera _mainVirualCamera;

	[SerializeField]
	private GameObject FollowTargetObject;

	[SerializeField]
	private CinemachineConfiner2D _mainVirtualCameraConfiner;
	// ----------------//
	// --- 私有成员
	// ----------------//
	private static DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _changedOperation;
	private VirtualCameraTrigger _newVirtualCamera;
	[ReadOnly]
	private CameraWorkingStates CurrentWorkingStatue { get; set; }
	// 透视方式下使用z轴
	//private readonly float[] _editSize = new float[] { 40, 160 };
	//private readonly float[] _modelSize = new float[] { 10, 25 };
	// 正交方式下使用fileView大小
	private readonly float[] _sizeRange = new float[] { 0.5f, 5 };

	public int GetNum { get { return _num; } }
	private int _num;

	private RectTransform CanvasRect
	{
		get
		{
			if (_canvasRect == null)
			{
				_canvasRect = UIManager.Instance.GetComponentInChildren<RectTransform>(true);
			}
			return _canvasRect;
		}
	}
	private RectTransform _canvasRect;
	private VirualCameraCaheDatas MainVirualCameraCaheData;
	// ----------------//
	// --- 公有成员
	// ----------------//
	public static CameraActor Instance { private set; get; }
	public static event Action<float> OnCameraViewSizeChanged;
	public VirtualCameraTrigger CurrentVirtualCamera
	{
		get => _newVirtualCamera;
		set
		{
			if (_newVirtualCamera)
			{
				_newVirtualCamera.VirtualCamera.Follow = null;
			}
			_newVirtualCamera = value;
			if (_newVirtualCamera)
			{
				_newVirtualCamera.VirtualCamera.Follow = FollowTargetObject.transform;
			}
			//_mainVirualCamera.enabled = (_newVirtualCamera == null);
		}
	}
	public Camera MainCamera { get; private set; }

	public float CameraViewSize
	{
		// 透视
		get => CameraActor.Instance.MainCamera.orthographicSize;
		set => CameraActor.Instance.MainCamera.orthographicSize = value;
		// 正交
		//get => CameraActor.Instance.MainCamera.fieldOfView;
		//set => CameraActor.Instance.MainCamera.fieldOfView =value;
	}
	public float MinCameraViewSize { get => _sizeRange[0]; }
	public float MaxCameraViewSize { get => _sizeRange[1]; }
	public const float Duration = 0.5f;
	public Vector2? mouseDownPos = null;

	/// <summary>
	/// 记录当前帧鼠标点击的位置
	/// </summary>
	private int _mousePosUpdateTime = -1;
	private Vector3? _curFramMouseWorldPos;
	public Vector3 MouseWorldPos
	{
		get
		{
			if (_mousePosUpdateTime != Time.frameCount || !_curFramMouseWorldPos.HasValue)
			{
				_mousePosUpdateTime = Time.frameCount;
				_curFramMouseWorldPos = ScreenTo2DWorldPosition(Mouse.current.position.ReadValue());
			}
			return _curFramMouseWorldPos.Value;
		}
	}

	private bool? _mouseOverUI;
	private int _mouseOverUIUpdateTime = -1;
	public bool MouseOverUI
	{
		get
		{
			if (_mouseOverUIUpdateTime != Time.frameCount)
			{
				_mouseOverUIUpdateTime -= Time.frameCount;
				_mouseOverUI = EventSystem.current.IsPointerOverGameObject();
			}
			return _mouseOverUI.Value;
		}
	}

	public CinemachineBrain MainCameraCinemachinBrain
	{
		get
		{
			if (_cinemachineBrain == null)
			{
				_cinemachineBrain = GetComponent<CinemachineBrain>();
			}
			return _cinemachineBrain;
		}
	}
	private CinemachineBrain _cinemachineBrain;

	public PlayableDirector MainCameraDirector
	{
		get
		{
			if (_mainCameraDirector == null)
			{
				_mainCameraDirector = GetComponent<PlayableDirector>();
			}
			return _mainCameraDirector;
		}
	}
	private PlayableDirector _mainCameraDirector;

	public CinemachineVirtualCamera MainVirtualCamera { get => _mainVirualCamera; }
	public CinemachineConfiner2D mainVirtualCameraConfiner { get => _mainVirtualCameraConfiner; }
	// ----------------//
	// --- Unity 消息
	// ----------------//
	private void Awake()
	{
		Instance = this;
		Debug.Assert(FollowTargetObject, "Awake时应该存在跟踪对象");
		MainCamera = GetComponentInChildren<Camera>();
		MainVirtualCamera.Follow = FollowTargetObject.transform;
		MainVirualCameraCaheData = new VirualCameraCaheDatas(MainVirtualCamera, mainVirtualCameraConfiner);
		ResetCamera();
	}

	private void Update()
	{
#if UNITY_EDITOR
		if (Keyboard.current.sKey.isPressed && Keyboard.current.spaceKey.isPressed && !EditorApplication.isPaused)
		{
			EditorApplication.isPaused = true;
		}
#endif
	}

	void LateUpdate()
	{
		//改为移动虚拟相机，而不是相机
		UpdateCamera();
	}


	// ----------------//
	// --- 公有方法
	// ----------------//
	public void SetMoveTo(Vector2 targetPos, float dur = Duration, Action completeCallBack = null)
	{
		CameraWorkingStates lastWorkingType = CurrentWorkingStatue;
		// 暂时锁定相机控制
		CurrentWorkingStatue = CameraWorkingStates.FreeMove;
		var op = MainVirtualCamera.transform.DOMove(new Vector3(targetPos.x, targetPos.y, MainVirtualCamera.transform.position.z), dur);
		// 移动完成后恢复设置工作模式
		op.onComplete = new TweenCallback(() =>
		{
			SetWorkingModel(lastWorkingType);
			completeCallBack?.Invoke();
		});
	}

	public void SetFocusTo(float to, float dur = Duration)
	{
		//if (CameraActor.Instance.MainCamera.orthographic)
		//{
		//	_changedOperation = DOTween.To(
		//		()=>CameraActor.Instance.MainCamera.orthographicSize,	
		//		(float v)=>CameraActor.Instance.MainCamera.orthographicSize = v,
		//		Mathf.Clamp(to, MinCameraSize, MaxCameraSize),
		//		dur
		//	);
		//}
		//else 
		//{
		//Debug.LogError($"{GetHashCode()} 设置相机视口大小 {to} -> {tar} 钳制区间 {MinCameraSize} {MaxCameraSize}");
		_changedOperation = DOTween.To(
			() => (float)CameraActor.Instance.CameraViewSize,
			(float v) => CameraActor.Instance.CameraViewSize = v,
			Mathf.Clamp(to, MinCameraViewSize, MaxCameraViewSize),
			dur
		);
		//}

		//if (tar != CameraActor.Instance.MainCamera.orthographicSize && CameraCanChange)
		//{
		//	//_changedOperation = DOTween.To(
		//	//	() => CameraActor.Instance.MainCamera.orthographicSize,
		//	//	setValue => CameraActor.Instance.MainCamera.orthographicSize = setValue,
		//	//	tar,
		//	//	dur
		//	//);
		//}
	}

	public void SetCameraWorkeState(CameraWorkingStates switchTo)
	{
		switch (switchTo)
		{
			case CameraWorkingStates.FreeMove:
				//mainVirtualCameraConfiner.m_BoundingShape2D = LevelProgressBase.Instance.CurrentEditZone.EditArea.PolygonTrigger;
				MainVirtualCamera.Priority = int.MaxValue;
				MainVirtualCamera.Follow = null;
				break;
			case CameraWorkingStates.Follow:
				MainVirtualCamera.Priority = MainVirualCameraCaheData.priorityCache;
				MainVirtualCamera.Follow = MainVirualCameraCaheData.followCache;
				//mainVirtualCameraConfiner.m_BoundingShape2D = MainVirualCameraCaheData.confinerCache;
				break;
		}
	}



	public void ResetCamera()
	{
		CameraActor.Instance.CurrentWorkingStatue = CameraWorkingStates.FreeMove;
		CameraActor.Instance.MainCamera.transform.position = new Vector3(0, 0, -10);
		CameraActor.Instance.MainCamera.orthographic = true;
		CameraViewSize = MinCameraViewSize;
		//CameraActor.Instance.MainCamera.orthographicSize = MaxCameraSize;
	}

	/// <summary>
	/// 将屏幕坐标转换为世界坐标中z轴为0的平面上
	/// </summary>
	/// <param name="mousePos"></param>
	/// <returns></returns>
	public static Vector3 ScreenTo2DWorldPosition(Vector2 mousePos)
	{
		float distance = Mathf.Abs(CameraActor.Instance.MainCamera.transform.position.z);
		return CameraActor.Instance.MainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distance));
	}

	public bool MouseClicked(MouseBtn mouseBtn, bool falseWhenOverUI = true)
	{
		if (falseWhenOverUI && MouseOverUI)
		{
			return false;
		}
		return MouseBtnClicked(mouseBtn);
	}

	public bool MouseClickedWolrdPos(CameraActor.MouseBtn btn, out Vector2 pos,  bool ignoreWhenClickUI )
	{
		if (ignoreWhenClickUI && MouseOverUI)
		{
			pos = Vector2.zero;
			return false;
		}
		pos = ScreenTo2DWorldPosition(Mouse.current.position.ReadValue());
		return true;
	}

	public Vector2 WorldToScreenPos(Vector3 worldPos)
	{
		Vector3 screenPos = CameraActor.Instance.MainCamera.WorldToScreenPoint(worldPos);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, screenPos, CameraActor.Instance.MainCamera, out Vector2 loaclPoint);
		return loaclPoint;
	}

	// ----------------//
	// --- 私有方法
	// ----------------//
	private void SetWorkingModel(CameraWorkingStates workingType)
	{
		CurrentWorkingStatue = workingType;
		if (workingType == CameraWorkingStates.FreeMove)
		{
			SetCameraSize(true);
		}
	}
	private void UpdateCamera()
	{
		if (CurrentWorkingStatue == CameraWorkingStates.FreeMove)
		{
			UpdateFreeMove();
			UpdateFreeScale();
		}
		//switch (CurrentWorkingStatue)
		//{
		//	case CameraWorkingStates.FreeMove: // 编辑状态下自由移动相机
		//								   //UpdateCameraSize(); // 做好不改变尺寸
		//								   // 边缘移动相机
		//		UpdateCameraPosition();
		//		break;
		//	case CameraWorkingStates.Follow:
		//		if (MainVirtualCamera.Follow == null)
		//		{
		//			MainVirtualCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
		//		}
		//		break;
		//}
	}

	/// <summary>
	/// 鼠标中键滚动缩放相机
	/// </summary>
	/// <param name="force"></param>
	private void SetCameraSize(bool force = false)
	{
		// 除以120后分为1，2，3档位（速度渐快）
		// 负号变换：前推为拉近
		float wheel = Mouse.current.scroll.ReadValue().y / -120;
		bool anykeyDown = Keyboard.current.anyKey.isPressed;
		if (!anykeyDown && (wheel != 0 || force))
		{
			//wheel *= 1 + (FirstAsset.Instance.CameraAction.CameraViewSize - MinCameraViewSize) / (MaxCameraViewSize - MinCameraViewSize);
			// 滚轮滚动10次为一次完整的缩放
			SetFocusTo(CameraActor.Instance.CameraViewSize + wheel * ((MaxCameraViewSize - MinCameraViewSize) / 9), 0.2f);
		}
	}
	private void UpdateFreeScale()
	{
		float maxSize = 7;
		float minSize = 3;
		float middleBtnScroller = Mouse.current.scroll.ReadValue().y;
		if (middleBtnScroller != 0)
		{ 
			float scale = (maxSize - MainVirtualCamera.m_Lens.OrthographicSize) * 0.1f;
			scale = Mathf.Clamp(scale, (maxSize - minSize) * 0.05f, (maxSize - minSize) * 0.1f) * (middleBtnScroller > 0 ? 1 : -1);
			MainVirtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(MainVirtualCamera.m_Lens.OrthographicSize + scale, minSize, maxSize);
		}
	}

	private void UpdateFreeMove()
	{
		#region 靠边移动相机方案
		//Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
		//int dectedRange = 4;
		//Vector2 moveDir = Vector2.zero;
		//if (mouseScreenPos.x < dectedRange)
		//{
		//	moveDir += Vector2.left;
		//}
		//else if (mouseScreenPos.x > Screen.width - dectedRange)
		//{
		//	moveDir += Vector2.right;
		//}

		//if (mouseScreenPos.y < dectedRange)
		//{
		//	moveDir += Vector2.up;
		//}
		//else if (mouseScreenPos.y > Screen.height - dectedRange)
		//{
		//	moveDir += Vector2.down;
		//}
		//if (moveDir == Vector2.zero) return;
		//float ratio = CameraActor.Instance.MainCamera.orthographicSize * GameManager.Instance.MouseScrollSpeed * Time.deltaTime;
		//CameraActor.Instance.MainCamera.transform.position += (Vector3)moveDir * ratio;
		#endregion

		#region 鼠标右键移动相机方案
		if (Mouse.current.rightButton.wasPressedThisFrame)
		{
			mouseDownPos = Mouse.current.position.ReadValue();
		}
		else if (Mouse.current.rightButton.wasReleasedThisFrame)
		{
			mouseDownPos = Vector2.down;
		}
		if (mouseDownPos != Vector2.down && Mouse.current.rightButton.isPressed)
		{
			float ratio = GameManager.Instance.MouseScrollSpeed * Time.deltaTime;
			Vector3 moveDir = (Mouse.current.position.ReadValue() - mouseDownPos.Value);
			MainVirtualCamera.transform.position += moveDir * ratio;
			// 取消了范围限制
			//if (!mainVirtualCameraConfiner.m_BoundingShape2D.OverlapPoint(MainVirtualCamera.transform.position))
			//{
			//	MainVirtualCamera.transform.position = mainVirtualCameraConfiner.m_BoundingShape2D.ClosestPoint(MainVirtualCamera.transform.position);
			//	MainVirtualCamera.transform.position += new Vector3(0, 0, -10);
			//}
		}

		float middleBtnOffset = Mouse.current.middleButton.ReadValue();
		if (middleBtnOffset != 0)
		{
			MainVirtualCamera.m_Lens.OrthographicSize += middleBtnOffset;
		}
		#endregion

		#region 鼠标中键拖拽移动 

		#endregion
	}

	private bool MouseBtnClicked(MouseBtn mouseBtn)
	{
		if (mouseBtn == MouseBtn.Right && Mouse.current.rightButton.wasPressedThisFrame)
		{
			return true;
		}
		if (mouseBtn == MouseBtn.Middle && Mouse.current.middleButton.wasPressedThisFrame)
		{
			return true;
		}
		if (mouseBtn == MouseBtn.Left && Mouse.current.leftButton.wasPressedThisFrame)
		{
			return true;
		}
		return false;
	}
	//----------------//
	// --- 类型
	// ----------------//
	public enum CameraWorkingStates
	{
		FreeMove, Follow
	}

	public enum MouseBtn
	{
		Left, Middle, Right
	}

	private class VirualCameraCaheDatas
	{
		//public Collider2D confinerCache;
		public Transform followCache;
		public int priorityCache;
		public VirualCameraCaheDatas(Cinemachine.CinemachineVirtualCameraBase vCam, Cinemachine.CinemachineConfiner2D confiner) 
		{
			followCache = vCam.Follow;
			priorityCache = vCam.Priority;
			//confinerCache = confiner.m_BoundingShape2D;
		}
	}


}
