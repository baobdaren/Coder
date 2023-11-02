using Sirenix.OdinInspector;
using UnityEngine;

public class LevelCreateHelper: MonoBehaviour
{
	// ------------- //
	// -- 序列化
	// ------------- //
	[ReadOnly]
	[SerializeField]
	[GUIColor("HasEditArea?Color.Green:Color.Red")]
	private bool HasEditArea;


	// ------------- //
	// -- 私有成员
	// ------------- //

	// ------------- //
	// -- 公有成员
	// ------------- //

	// ------------- //
	// -- Unity 消息
	// ------------- //
	[Button(name: "检测")]
	private void Check()
	{ 
		
	}

	// ------------- //
	// -- 公有方法
	// ------------- //

	// ------------- //
	// -- 私有方法
	// ------------- //

}
