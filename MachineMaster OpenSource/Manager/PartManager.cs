using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PartManager
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public static PartManager Instance = new PartManager();
	/// <summary>
	/// 玩家零件和场景零件总数
	/// </summary>
	public int AllPartCount => PlayerPartManager.Instance.AllPlayerPartCtrls.Count + 
		(LevelProgressBase.Instance.CurrentEditZone != null ? LevelProgressBase.Instance.CurrentEditZone.AllSceneParts.Count : 0);

	// ----------------//
	// --- 私有成员
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	/// <summary>
	/// 获取零件，包括场景零件
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public BasePartCtrl GetPart(int index)
	{
		if (index < PlayerPartManager.Instance.AllPlayerPartCtrls.Count)
		{
			return PlayerPartManager.Instance.AllPlayerPartCtrls[index];
		}
		else
		{
			return LevelProgressBase.Instance.CurrentEditZone.AllSceneParts[index - PlayerPartManager.Instance.AllPlayerPartCtrls.Count];
		}
	}

	/// <summary>
	/// 测试零件之间是否有冲突的碰撞器
	/// </summary>
	/// <param name="conflictPart"></param>
	/// <returns></returns>
	public bool GetConflictPart(out List<(BasePartCtrl, BasePartCtrl)> ConflictParts)
	{
		bool recordActiveState = ParentsManager.Instance.ParentOfEditParts.gameObject.activeSelf;
		ParentsManager.Instance.ParentOfEditParts.gameObject.SetActive(true);
		ConflictParts = new List<(BasePartCtrl, BasePartCtrl)>();
		for (int partIndex = 0; partIndex < AllPartCount; partIndex++)
		{
			for (int partIndex2 = partIndex + 1; partIndex2 < AllPartCount; partIndex2++)
			{
				BasePartCtrl partA = GetPart(partIndex);
				BasePartCtrl partB = GetPart(partIndex2);
				if (partA == partB) continue;
				if (partA.OverlapOther(partB))
				{
					ConflictParts.Add((partA, partB));
				}
			}
		}
		ParentsManager.Instance.ParentOfEditParts.gameObject.SetActive(recordActiveState);
		return ConflictParts.Count > 0;
	}
	// ----------------//
	// --- 私有方法
	// ----------------//'

	// ----------------//
	// --- 类型
	// ----------------//
}
