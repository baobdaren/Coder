using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 零件组列表的Item
/// </summary>
public class GroupItem : SerializedMonoBehaviour
{
	// ----------------//
	// --- 序列化
	// ----------------//
	/// <summary>
	/// 颜色显示/修改按钮
	/// </summary>
	[ChildGameObjectsOnly]
	public NiceButton ColorSelectBtn;
	/// <summary>
	/// 增加零件到组的按钮
	/// </summary>
	[ChildGameObjectsOnly]
	public NiceButton AddToGroupBtn;
	/// <summary>
	/// 当前组的零件数量
	/// </summary>
	[ChildGameObjectsOnly]
	public TextMeshProUGUI PartGroupAmountText;
	// ----------------//
	// --- 公有成员
	// ----------------//

	// ----------------//
	// --- 私有成员
	// ----------------//

	// ----------------//
	// --- Unity消息
	// ----------------//

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
