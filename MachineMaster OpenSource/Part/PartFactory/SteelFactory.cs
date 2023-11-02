using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelFactory : AbsPartFactory
{
	public static object Intsance { get; internal set; }

	// ----------------//
	// --- 序列化
	// ----------------//

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

	/// <summary>
	/// 初始化钢材的节点
	/// </summary>
	/// <param name="partCtrl"></param>
	/// <param name="newNodeWorldSpacePossPairs"></param>
	/// <param name="accesstor"></param>
	public void InitCorners(PlayerPartCtrl partCtrl)
	{
		Debug.Assert(partCtrl.CoreData.SectionDataList == null || partCtrl.CoreData.SectionDataList.Count == 0);
		var newNodeWorldSpacePossPairs = new List<(Vector3, Quaternion)> { (new Vector3(-0.4f, 0, 0), Quaternion.identity), (new Vector3(0.4f, 0, 0), Quaternion.identity) };
		SteelAccessor accessor = partCtrl.MyEditPartAccesstor as SteelAccessor;
		foreach (var item in newNodeWorldSpacePossPairs)
		{
			partCtrl.CoreData.SectionDataList.Add((accessor.transform.InverseTransformPoint(item.Item1), item.Item2));
		}
		ModifyCorners(partCtrl, accessor);
	}

	public void UpdateCorner(PlayerPartCtrl partCtrl, int updateIndex, Vector3 newNodeWorldSpacePos)
	{
		partCtrl.CoreData.SectionDataList[updateIndex] = (newNodeWorldSpacePos, Quaternion.identity);
		ModifyPart(partCtrl);
	}

	/// <summary>
	/// 设置虚拟显示的段的索引，编辑模式下的编辑状态！！！
	/// </summary>
	/// <param name="accesstor"></param>
	/// <param name="posIndex"></param>
	public void SetEditCornerIndex(SteelAccessor accesstor, int? posIndex = null)
	{
		accesstor.SetHighLightIndex(posIndex);
	}

	public void InsertCorner(PlayerPartCtrl partCtrl, Vector3 newWorldSpaceCornerPos, int insertIndex)
	{
		Debug.Assert(partCtrl.MyPartType == PartTypes.Steel);
		SteelAccessor accesstor = partCtrl.MyEditPartAccesstor as SteelAccessor;
		newWorldSpaceCornerPos = accesstor.transform.InverseTransformPoint(newWorldSpaceCornerPos);
		if (partCtrl.CoreData.SectionDataList.Count - 1 >= accesstor.ColliderSections.Length)
		{
			return;
		}
		partCtrl.CoreData.SectionDataList.Insert(insertIndex + 1, (newWorldSpaceCornerPos, Quaternion.identity));
		ModifyCorners(partCtrl, accesstor);
	}

	public bool RemoveCorner(PlayerPartCtrl partCtrl, int removeIndex, SteelAccessor accesstor)
	{
		if (partCtrl.CoreData.SectionDataList.Count <= 2)
		{
			return false;
		}
		partCtrl.CoreData.SectionDataList.RemoveAt(removeIndex);
		ModifyCorners(partCtrl, accesstor);
		return true;
	}

	/// <summary>
	/// 获取所有拐点再世界坐标下的信息
	/// </summary>
	/// <param name="partCtrl"></param>
	/// <param name="steelAccesstor"></param>
	/// <returns></returns>
	public List<(Vector3, Quaternion)> GetCorners_WorldSpace(PlayerPartCtrl partCtrl, SteelAccessor accessor)
	{
		List<(Vector3, Quaternion)> corners = new List<(Vector3, Quaternion)>(10);
		GetWorldSpaceCroners_NonAlloc(partCtrl, accessor, ref corners);
		return corners;
	}
	/// <summary>
	/// 后期需要删除传入参数accesster
	/// 从partCtrl中获取位置和旋转计算拐点世界坐标
	/// </summary>
	/// <param name="partCtrl"></param>
	/// <param name="steelAccesstor"></param>
	/// <param name="result"></param>
	/// <returns></returns>
	public void GetWorldSpaceCroners_NonAlloc(PlayerPartCtrl partCtrl, SteelAccessor accessor, ref List<(Vector3, Quaternion)> result)
	{
		result.Clear();
		int count = partCtrl.CoreData.SectionDataList.Count;
		if (result.Capacity < 10)
		{
			result = new List<(Vector3, Quaternion)>(10);
		}
		Debug.Assert(result.Capacity >= 10 && count <= 10);
		for (int i = 0; i < count; i++)
		{
			result.Add((accessor.transform.TransformPoint(partCtrl.CoreData.SectionDataList[i].Item1), Quaternion.identity));
		}
	}


	// ----------------//
	// --- 私有方法
	// ----------------//
	protected override void OnCreatedAsEdit(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.AppliyEditConfig(item);
		}
		if (partCtrlData.CoreData.SectionDataList.Count == 0)
		{
			InitCorners(partCtrlData);
		}
		accesstor.PartDragCmpnt.EnableSnapBound = true;
	}

	protected override void OnCreatedAsPhysics(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.ApplySimulateConfig(item);
		}
		accesstor.PartDragCmpnt.EnableSnapBound = true;
		//GameObject.DestroyImmediate(accesstor.GetComponent<AbsAdsorbableTarget>());
	}

	/// <summary>
	/// 对于编辑状态的Steel来说，不存在UI界面拖拽修改尺寸的方式
	/// 物理状态的Steel需要修改
	/// </summary>
	/// <param name="partCtrlData"></param>
	/// <param name="accesstor"></param>
	protected override void SetSize(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		//Vector2 originSize = PartConfig.Instance.SteelConfig.SteelOrigiSize;
		//accesstor.GetComponent<BoxCollider2D>().autoTiling = true;
		//accesstor.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
		//accesstor.GetComponent<SpriteRenderer>().size = Vector2.right * (partCtrlData.Size + 1) * originSize.x + Vector2.up * originSize.y;
		ModifyCorners(partCtrlData, accesstor as SteelAccessor);
	}

	/// <summary>
	/// 修改拐角表现
	/// </summary>
	/// <param name="partCtrlData"></param>
	/// <param name="accesstor"></param>
	private void ModifyCorners(PlayerPartCtrl partCtrlData, SteelAccessor accesstor)
	{
		if(!CheckStellLength(partCtrlData.CoreData.SectionDataList, 5)) { return; }
		bool isEdit = partCtrlData.MyEditPartAccesstor == accesstor;
		//List<(Vector3, Quaternion)> sectionPoss = partCtrlData.CoreData.SectionDataList;
		CapsuleCollider2D col = accesstor.ColliderSections[0].GetComponentInChildren<CapsuleCollider2D>();
		float capsuleHeight = col.size.y;
		List<(Vector3, Quaternion)> worldSpaceCorners = GetCorners_WorldSpace(partCtrlData, accesstor);
		if (worldSpaceCorners.Count < 2) return;
		for (int i = 1; i < worldSpaceCorners.Count; i++)
		{
			// 处理每一段的位置和旋转
			Quaternion quaternion = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, worldSpaceCorners[i].Item1 - worldSpaceCorners[i - 1].Item1));
			Vector2 pos = (worldSpaceCorners[i].Item1 + worldSpaceCorners[i - 1].Item1) / 2;
			accesstor.ColliderSections[i-1].gameObject.SetActive(true);
			accesstor.ColliderSections[i-1].transform.SetPositionAndRotation(pos, quaternion);
			// 区分编辑模式和物理模式下的碰撞器大小
			accesstor.ColliderSections[i-1].size = new Vector2(Vector2.Distance(worldSpaceCorners[i].Item1, worldSpaceCorners[i - 1].Item1) + (isEdit ? -capsuleHeight : capsuleHeight), capsuleHeight);
		}
		// 处理SpriteShapeRender的设置点
		for (int i = worldSpaceCorners.Count; i < accesstor.ColliderSections.Length; i++)
		{
			accesstor.ColliderSections[i].gameObject.SetActive(false);
		}
		// 更新linerender的拐点
		accesstor.UpdateCorners(partCtrlData.CoreData.SectionDataList);
		// 设定表现
	}

	private bool CheckStellLength(List<(Vector3, Quaternion)> poss, float maxLen)
	{
		float len = 0;
		for (int i = 1; i < poss.Count; i++)
		{
			len += Vector3.Distance(poss[i].Item1, poss[i - 1].Item1);
		}
		return len <= maxLen;
	}
	// ----------------//
	// --- 类型
	// ----------------//
}
