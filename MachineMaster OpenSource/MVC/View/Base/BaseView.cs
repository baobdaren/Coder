using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseView: SerializedMonoBehaviour
{
	// ----------------//
	// --- 私有成员
	// ----------------//
	protected List<BaseChildView> AllChildViews
	{
		get
		{
			if (_AllChildViews == null)
			{
				_AllChildViews = new List<BaseChildView>(GetComponentsInChildren<BaseChildView>(true));
			}
			return _AllChildViews;
		}
	}
	private List<BaseChildView> _AllChildViews;

	// ----------------//
	// --- 公有成员
	// ----------------//
	public bool IsDisplaying
	{
		get => transform.GetChild(0).gameObject.activeSelf;
		private set
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			transform.GetChild(0).gameObject.SetActive(value);
		}
	}


	// ----------------//
	// --- Unity消息
	// ----------------//
	protected virtual void Awake() { }

	protected virtual void Start()
	{
		if (TryGetComponent<Canvas>(out Canvas canvas))
		{
			//canvas.worldCamera = CameraActor.Instance.MainCamera;
			canvas.worldCamera = Camera.main;
		}
	}
	// ----------------//
	// --- 私有方法
	// ----------------//


	// ----------------//
	// --- 公有方法
	// ----------------//
	public bool IsView<T>() where T : BaseView
	{
		return this is T;
	}

	public void SwitchView(BaseView openView)
	{
		if (openView  != this && this.IsDisplaying == true)
		{
			if (ExitCondition(openView))
			{
				ExitView();
			}
			else
			{
				HideView();
			}
			foreach (var item in AllChildViews)
			{
				item.OnExitParentView();
			}
		}
		if (openView == this/* && this.IsDisplay == false*/)
		{
			if (EnterCondition(openView))
			{
				EnterView();
			}
			else
			{
				DisplayView();
			}
			foreach (var item in AllChildViews)
			{
				item.OnEnterParentView();
			}
		}
	}

	public virtual bool EnterCondition(BaseView baseView)
	{
		return true ;
	}

	public virtual bool ExitCondition(BaseView willOpenView)
	{
		return true;
	}

	public virtual void EnterView() 
	{
		IsDisplaying = true;
	}
	public virtual void ExitView() 
	{
		IsDisplaying = false;
	}

	/// <summary>
	/// 暂时挂起
	/// </summary>
	public virtual void HideView()
	{
		IsDisplaying = false;
	}
	public virtual void DisplayView()
	{
		IsDisplaying = true;
	}
	// ----------------//
	// --- 类型
	// ----------------//
	public enum SwitchState
	{
		Suspend, Exit, Enter
	}

	public enum MainViewTypes
	{ 
		Create, Edit, Modeling, Simulate, Start
	}
}
