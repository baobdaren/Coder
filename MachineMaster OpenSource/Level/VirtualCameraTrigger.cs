using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;

[RequireComponent(typeof(TargetTrigger))]
public class VirtualCameraTrigger : SerializedMonoBehaviour
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[SerializeField]
	private GameObject TriggerObject;	
	// ----------------//
	// --- 公有成员
	// ----------------//
	public CinemachineVirtualCamera VirtualCamera { get => _virtualCamera; }

	// ----------------//
	// --- 私有成员
	// ----------------//
	private CinemachineVirtualCamera _virtualCamera;
	private CinemachineConfiner2D _virtualCameraConfiner;
	// ----------------//
	// --- Unity消息
	// ----------------//
	private void Awake()
	{
		Debug.Assert(TriggerObject != null);
		_virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
		_virtualCameraConfiner = GetComponentInChildren<CinemachineConfiner2D>();
		GetComponentInChildren<TargetTrigger>().AddListener(OnPlayerEnter, OnPlayerExit);
		VirtualCamera.enabled = false;
	}

	private void OnPlayerEnter()
	{
		//Debug.Log("进入 对象" + collision.gameObject.name);
		//if (Trigged && TriggerType == TriggerTypes.Once) return;
		//if (collision.gameObject != TriggerObject) return;

		VirtualCamera.enabled = true;
		CameraActor.Instance.CurrentVirtualCamera = this;
		_virtualCameraConfiner.m_BoundingShape2D = CameraActor.Instance.mainVirtualCameraConfiner.m_BoundingShape2D;
		Debug.Log("进入 虚拟相机范围");
	}

	private void OnPlayerExit()
	{
		//Debug.Log("退出 对象" + collision.gameObject.name);
		//if (Trigged && TriggerType == TriggerTypes.Once) return;
		//if (collision.gameObject != TriggerObject)
		//{
		//	return;
		//}

		VirtualCamera.enabled = false;
		if (CameraActor.Instance.CurrentVirtualCamera == this)
		{
			CameraActor.Instance.CurrentVirtualCamera = null;
		}
		Debug.Log("退出 虚拟相机范围");
	}


	// ----------------//
	// --- 公有方法
	// ----------------//

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
