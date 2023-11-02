using System.Collections.Generic;
using UnityEngine;

public class ModelEdit : ResetAbleInstance<ModelEdit>
{

	// --------------- //
	// -- 私有属性
	// --------------- //
	private BasePartCtrl _editingPartCtrl;
	private IConnectableCtrl willConnectTarget;
	private PartTypes? connectablePartType;
	private bool isConnecting;

	// --------------- //
	// -- 公有属性
	// --------------- //
	public BasePartCtrl EditingPartCtrl
	{
		set
		{
			_editingPartCtrl = value;
			IsDirty_PartColor = true;
		}
	}

	public bool IsEditing
	{
		get{ return _editingPartCtrl != null;}
	}
	public bool IsEditingPlayerPart 
	{
		get 
		{ return _editingPartCtrl is PlayerPartCtrl; }
	}
	public PlayerPartCtrl EditingPlayerPartCtrl
	{
		get
		{return _editingPartCtrl as PlayerPartCtrl;}
	}

	public bool IsEditingScenePart
	{
		get { return _editingPartCtrl is ScenePartCtrl; }
	}
	public ScenePartCtrl EditingScenePartCtrl
	{
		get { return _editingPartCtrl as ScenePartCtrl; }
	}

	public IConnectableCtrl WillConnectTarget
	{
		private set
		{
			if (willConnectTarget != value)
			{
				IsDirty_PartColor = true;
				willConnectTarget = value;
			}
		}
		get { return willConnectTarget; }
	}

	public PartTypes? ConnectablePartType
	{
		get { return connectablePartType; }
		set
		{
			connectablePartType = value;
			IsDirty_PartColor = true;
		}
	}
	public IConnectableCtrl GetConnectMain => EditingPlayerPartCtrl;
	public Vector2? CurrentConnectCursorPos { get; private set; } = null;

	public bool IsDirty_PartColor
	{
		set
		{
			__partColorIsDirty = value;
			//if (value) Debug.LogWarning("零件 颜色 设置标志");
		}
		get
		{
			return __partColorIsDirty;
		}
	}
	private bool __partColorIsDirty;

	/// <summary>
	/// 轴承显示更新标志
	/// </summary>
	public bool IsDirty_BearingColor
	{
		set
		{
			__bearingColorIsDirty = value;
			//if (value) Debug.LogWarning("轴承颜色 设置标志");
		}
		get
		{
			return __bearingColorIsDirty;
		}
	}
	private bool __bearingColorIsDirty;

	/// <summary>
	/// 是否正在查找轴承
	/// 用于轴承图标更新
	/// </summary>
	public bool IsFindingBearing = false;


	public bool IsCursorOverlapedMainNow
	{
		get
		{
			if (CurrentConnectCursorPos.HasValue == false) return false;
			return GetConnectMain.OverlapPoint(CurrentConnectCursorPos.Value);
		}
	}
	public bool IsCursorCanFixedConecctNow => IsCursorOverlapedMainNow && WillConnectTarget != null;
	// --------------- //
	// --- 共有方法
	// --------------- //
	public void SetConnectData(Vector2? anchorPos, IConnectableCtrl connectTarget = null)
	{
		//if (anchorPos == null)
		//{
		//	Debug.Log("重置数据");
		//}
		//else
		//{ 
		//	Debug.Log($"设置连接数据 位置{(anchorPos.HasValue ? anchorPos.Value : null)} 铰接目标 {connectTarget}");
		//}
		CurrentConnectCursorPos = anchorPos;
		WillConnectTarget = connectTarget;
		IsDirty_BearingColor = true;
	}

	public void ResetConnectState()
	{
		SetConnectData(null, null);
	}

	// --------------- //
	// -- 公共方法
	// --------------- //
	public void ClearDirty()
	{
		Debug.Log("Clear Dirty");
		IsDirty_PartColor = false;
	}

	// --------------- //
	// -- 类型
	// --------------- //
}
