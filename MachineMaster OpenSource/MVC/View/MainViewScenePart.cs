using UnityEngine;

public class MainViewScenePart: BaseView
{
	// ------------- //
	// -- 序列化
	// ------------- //
	[SerializeField]
	private NiceButton ExitButton;

	// ------------- //
	// -- 私有成员
	// ------------- //
	private ModelScenePart Model { get => ModelScenePart.Instance; }

	// ------------- //
	// -- 公有成员
	// ------------- //

	// ------------- //
	// -- Unity 消息
	// ------------- //
	protected override void Awake()
	{
		base.Awake();
		ExitButton.OnLeftClick.AddListener(OnClicked_ExitView);
	}

	private void Update()
	{
		if (IsDisplaying && Model.IsDirty)
			UpdateBearingColor();
	}
	// ------------- //
	// -- 公有方法
	// ------------- //
	public override void EnterView()
	{
		base.EnterView();
		UpdateBearingColor();
	}

	// ------------- //
	// -- 私有方法
	// ------------- //
	private void OnClicked_ExitView()
	{
		UIManager.Instance.OpenView<MainViewStart>();
	}

	private void UpdateBearingColor()
	{
		foreach (var itemConn in PartConnectionManager.Instance.AllConnection)
		{
			if (!itemConn.IsFixedJoint
				&& Model.EditingScenePart.OverlapPoint(itemConn.AnchorPosition)
				&& !itemConn.ConnectedParts.ContainsKey(Model.EditingScenePart)) //该轴承覆盖当前零件，而且没有连接到该零件
			{
				itemConn.Bearing.SetDisplay(true, true);
			}
			else
			{
				itemConn.Bearing.SetDisplay(true, false);
			}
		}
	}
}
