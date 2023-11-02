using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 功能主界面管理
/// </summary>
public class ChildViewEdit_Functions : BaseChildView
{
	[Header("建模按钮")]
	[SerializeField]
	private Button _buttonModeling;
	// ------------- //
	// -- 私有成员
	// ------------- //

	// ------------- //
	// -- Unity 消息
	// ------------- //
	private void Awake()
	{
		_buttonModeling.onClick.AddListener(OnClick_Modeling);
	}

	public void DisplayModelingBtn(bool display)
	{
		_buttonModeling.gameObject.SetActive(display);
	}
	// ------------- //
	// -- 公有方法
	// ------------- //

	// ------------- //
	// -- 私有方法
	// ------------- //
	private void OnClick_Modeling()
	{
		UIManager.Instance.OpenView<MainViewModeling>();
	}
}
