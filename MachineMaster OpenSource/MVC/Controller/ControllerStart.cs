using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ControllerStart
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public static ControllerStart Instance { get; private set; } = new ControllerStart();
	public MainViewStart View
	{
		get
		{
			if (_view == null)
			{
				_view = GameObject.FindObjectOfType<MainViewStart>();
			}
			return _view;
		}
	}

	// ----------------//
	// --- 私有成员
	// ----------------//
	private MainViewStart _view;

	// ----------------//
	// --- 公有方法
	// ----------------//
	public void SetCameraFreeMove()
	{
		CameraActor.Instance.SetCameraWorkeState(CameraActor.CameraWorkingStates.FreeMove);
	}

	public void FinishiLevel()
	{
		GameManager.Instance.On_LevelFinish(-1);
	}
	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
