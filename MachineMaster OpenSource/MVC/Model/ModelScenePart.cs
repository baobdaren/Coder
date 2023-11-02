using UnityEngine;

public class ModelScenePart: ResetAbleInstance<ModelScenePart>
{
	// ------------- //
	// -- 序列化
	// ------------- //

	// ------------- //
	// -- 私有成员
	// ------------- //
	private IConnectableCtrl _editingScenePart;
	// ------------- //
	// -- 公有成员
	// ------------- //
	public IConnectableCtrl EditingScenePart
	{
		set
		{ 
			_editingScenePart = value;
			IsDirty = true;
		}
		get { return _editingScenePart; }
	}

	public bool IsDirty { set; get; }
	// ------------- //
	// -- Unity 消息
	// ------------- //

	// ------------- //
	// -- 公有方法
	// ------------- //


	// ------------- //
	// -- 私有方法
	// ------------- //

}
