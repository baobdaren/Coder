using Sirenix.OdinInspector;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ConnectCursor : SerializedMonoBehaviour
{
	// ------------------ //    
	// --- 序列化    
	// ------------------ //
	private static ConnectCursor _instance;
	// ------------------ //    
	// --- 公有成员    
	// ------------------ //
	public static ConnectCursor Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (GameObject.Instantiate(GameConfig.Instance.ConnectCursor) as GameObject).GetComponentInChildren<ConnectCursor>();
			}
			return _instance;
		}
	}

	//public class NonArgEvent : UnityEvent{ }
	public class CursorAct : UnityEvent<ConnectCursorStates> { }
	[HideInInspector]
	public CursorAct OnMoveStart = new CursorAct();
	[HideInInspector]
	public CursorAct OnMoving = new CursorAct();
	[HideInInspector]
	public CursorAct OnMoveEnd = new CursorAct();
	[HideInInspector]
	public CursorAct OnClick_NotClickedUI = new CursorAct();
	[HideInInspector]
	public CursorAct OnCLick_UI = new CursorAct();

	public ConnectCursorStates ConnectingState { private set; get; }

	// ------------------ //   
	// --- 私有成员    
	// ------------------ //
	private readonly StringBuilder stringBuilder = new StringBuilder(32);
	private bool _moveing = false;
	// ------------------ //    
	// --- Unity消息    
	// ------------------ //
	private void Update()
	{
		//if (!_moveing)
		//{
		//	return;
		//}
		stringBuilder.Clear();
		if (Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			Hide();
			return;
		}
		//if (FirstAsset.Instance.CameraAction.MouseWorldPos == transform.position)
		//{
		//	return;
		//}
		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			Debug.Log("按下鼠标左键");
		}
		// 屏幕外时不跟随
		// snap具有拖拽性质
		//Vector2 mousePos = Mouse.current.position.ReadValue();
		//mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
		//mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
		//transform.position = CameraActor.Instance.MouseWorldPos;

		//bool adsorbed = AbsAdsorbableGameObject.Adsorb(transform, 0.2f);
		//Debug.Log(adsorbed?"可吸附":"不可吸附");
		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			if (!UIHelper.ClickedUI(Mouse.current.position.ReadValue(), out string castname))
			{
				OnClick_NotClickedUI?.Invoke(ConnectingState);
				Hide();
				SnapManager.Instance.HideX();
				return;
			}
			else
			{
				Debug.LogError("点击到UI不算" + castname);
			}
		}
		else
		{ 
			OnMoving?.Invoke(ConnectingState);
		}
	}

	// ------------------ //    
	// --- 公有方法   
	// ------------------ //
	public void SetColor(int num)
	{
		Color color = Color.red;
		if (num < 0)
		{
			color = Color.red;
		}
		if (num == 0)
		{
			color = Color.yellow;
		}
		if (num > 0)
		{
			color = Color.green;
		}
		foreach (var childRenders in GetComponentsInChildren<SpriteRenderer>())
		{
			childRenders.color = color;
		}
	}

	public void Display(ConnectCursorStates state)
	{
		ConnectingState = state;
		gameObject.SetActive(true);
		if (!_moveing)
		{
			OnMoveStart?.Invoke(ConnectingState);
			_moveing = true;
		}
		GetComponent<SnapableBase>().ForceDrag(false);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		if (_moveing)
		{
			OnMoveEnd?.Invoke(ConnectingState);
			_moveing = false;
		}
	}
	// ------------------ //   
	// --- 私有方法
	// ------------------ //

	// ------------------ //   
	// --- 私有方法
	// ------------------ //
	public enum ConnectCursorStates
	{
		Fixed, Bearing, FindBearing
	}
}
