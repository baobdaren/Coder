using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeFactory : AbsPartFactory
{
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


	// ----------------//
	// --- 私有方法
	// ----------------//
	protected override void OnCreatedAsEdit(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		RopeAccessor ropeAccesstor = accesstor as RopeAccessor;
		foreach (var item in accesstor.AllRigids)
		{
			PartConfig.Instance.PartRigidConfig.ApplyHighDragConfig(item);
		}
		// 多节零件需要再拖拽时禁止物理模拟
		accesstor.PartDragCmpnt.OnDragStart.AddListener(() =>
		{
			for (int i = 0; i < accesstor.AllRigids.Count; i++)
			{
				accesstor.AllRigids[i].simulated = false;
			}
		});
		accesstor.PartDragCmpnt.OnDragEnd.AddListener(() =>
		{
			for (int i = 0; i < accesstor.AllRigids.Count; i++)
			{
				accesstor.AllRigids[i].simulated = true;
			}
		});
	}


	protected override void OnCreatedAsPhysics(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		foreach (var item in accesstor.AllRigids)
		{
			item.simulated = true;
			PartConfig.Instance.PartRigidConfig.ApplySimulateConfig(item);
		}
	}


	protected override void SetSize(PlayerPartCtrl partCtrlData, AbsPartAccessorBase accesstor)
	{
		RopeAccessor ropeAccesstor = accesstor as RopeAccessor;
		PlaceSection(ropeAccesstor, (int)partCtrlData.Size);
		ConnectAllSection(ropeAccesstor, (int)partCtrlData.Size);
	}


	protected override void SetPositionAndRotation(PlayerPartCtrl partCtrl, AbsPartAccessorBase accesstor)
	{
		base.SetPositionAndRotation(partCtrl, accesstor);
		//if (partCtrl.core != null) // 新创建的零件该参数为空
		//{
		//	for (int i = 0; i < accesstor.SectionList.Count; i++)
		//	{
		//		if (i >= partCtrl.SectionPositionList.Count)
		//		{
		//			break;
		//		}
		//		accesstor.SectionList[i].transform.SetPositionAndRotation(partCtrl.SectionPositionList[i], partCtrl.SectionRotatioList[i]);
		//	}
		//}
	}
	//private GameObject CreateSetcion()
	//{
	//	var cloneSectionRigid = GameObject.Instantiate(_originNode, _originNode.transform.parent).GetComponent<Rigidbody2D>();
	//	cloneSectionRigid.name = _originNode.name;
	//	cloneSectionRigid.gameObject.SetActive(false);
	//	PartConfig.Instance.RopeConfig.Apply(cloneSectionRigid, 0.1f);
	//	return cloneSectionRigid.gameObject;
	//}

	private void ConnectAllSection(RopeAccessor rope, int useAmount)
	{
		for (int i = useAmount - 1; i > 0; i--)
		{
			GameObject sectionA = rope.SectionList[i - 1].gameObject;
			GameObject sectionConnectTo = rope.SectionList[i].gameObject;
			// 从后往前连接可以利用自动铰接属性
			if (!sectionConnectTo.TryGetComponent(out HingeJoint2D joint))
			{
				joint = sectionConnectTo.AddComponent<HingeJoint2D>();
			}
			joint.enabled = i != 0;
			joint.connectedBody = sectionA.GetComponent<Rigidbody2D>();
			joint.autoConfigureConnectedAnchor = true;
			joint.anchor = Vector2.zero;
			joint.autoConfigureConnectedAnchor = false;
			joint.useMotor = false;
		}
		if (rope.SectionList[0].TryGetComponent<HingeJoint2D>(out var joint0))
		{
			joint0.enabled = false;
		}
	}

	private void PlaceSection(RopeAccessor accesstor, int useAmount)
	{
		//Assert.IsTrue(_sectionList.Count >= placeAmount, "放置数量必须小于总数");
		for (int i = 0; i < accesstor.SectionList.Count; i++)
		{
			accesstor.SectionList[i].gameObject.SetActive(i < useAmount);
			if (i == 0)
			{
				accesstor.SectionList[i].transform.SetPositionAndRotation(accesstor.transform.position, accesstor.transform.rotation);
			}
			else if (i < useAmount)
			{
				accesstor.SectionList[i].transform.SetPositionAndRotation(accesstor.SectionList[i - 1].transform.TransformPoint(Vector3.right * /*PartConfig.DISTANCE_SECTION*/1), accesstor.SectionList[i - 1].transform.rotation);
			}
		}
	}
	// ----------------//
	// --- 类型
	// ----------------//
}
