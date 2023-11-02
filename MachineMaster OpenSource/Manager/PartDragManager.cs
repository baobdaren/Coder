using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;

public class PartDragManager
{
	// ----------------//
	// --- 序列化
	// ----------------//
	public static PartDragManager Instance = new PartDragManager();

	// ----------------//
	// --- 公有成员
	// ----------------//
	public bool IsDraging => DragingPart != null;
	public PlayerPartCtrl DragingPart { private set; get; } = null;
	//public UnityEvent<PlayerPartCtrl> OnStartDragPlayerPart = new UnityEvent<PlayerPartCtrl>();
	//public UnityEvent<PlayerPartCtrl> OnEndDragPlayerPart = new UnityEvent<PlayerPartCtrl>();
	// ----------------//
	// --- 私有成员
	// ----------------//

	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	public void RegistPart(PlayerPartCtrl playerPart)
	{
		//Debug.LogWarning("注册零件的拖拽监听" + playerPart.MyPartType);
		playerPart.MyEditPartAccesstor.PartDragCmpnt.OnDragStart.AddListener((UnityAction)(() => {
			//Debug.LogWarning((object)("开始拖拽零件" + playerPart.MyPartType));
			DragingPart = playerPart;
			//OnStartDragPlayerPart?.Invoke(playerPart); 
		}));
		playerPart.MyEditPartAccesstor.PartDragCmpnt.OnDragEnd.AddListener(() => { 
			DragingPart = null;
			//OnEndDragPlayerPart?.Invoke(playerPart); 
		});
	}

	public void ResetData()
	{
		//OnStartDragPlayerPart.RemoveAllListeners();
		//OnEndDragPlayerPart.RemoveAllListeners();
		DragingPart = null;
	}

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
