using System;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Text;

public class PartConnectionManager:Loadable<PartConnectionManager>
{
	public PartConnectionManager() { }
	// --------------- //
	// --  私有成员
	// --------------- //
	private List<Joint2D> _jointList = new List<Joint2D>();

	// --------------- //
	// --  公有成员
	// --------------- //
	public List<BasePartConnection> AllConnection { private set; get; } = new List<BasePartConnection>();
	//public List<PartHingeConnection> AllHingeConnection { private set; get; } = new List<PartHingeConnection>();
	//public List<PartHingeConnection> AllFixedConnection { private set; get; } = new List<PartHingeConnection>();

	//public bool IsCursorOverlapedMainNow
	//{
	//	get
	//	{
	//		if (CurrentConnectCursorPos.HasValue == false) return false;
	//		return GetConnectMain.OverlapPoint(CurrentConnectCursorPos.Value);
	//	}
	//}
	//public bool IsCursorCanFixedConecctNow => IsCursorOverlapedMainNow && WillConnectTarget != null;

	// --------------- //
	// --  公有方法
	// --------------- //
	/// <summary>
	/// 尝试创建铰接对象
	/// 以交接主体和铰接对象的数据为准，尝试创建，创建失败则返回false
	/// </summary>
	/// <param name="isFixedOrHinge"></param>
	/// <returns></returns>
	public bool TryCreateFixedConnection(IConnectableCtrl part1, IConnectableCtrl part2, Vector2 anchorPos, StringBuilder sb)
	{
		BasePartConnection conn;
		//IConnectableCtrl part1 = Model.EditingPlayerPartCtrl;
		//Vector2 anchorPos = Model.CurrentConnectCursorPos.Value;
		if (!part1.TryGetOverlapPointEditColliderIndex(anchorPos, out int firstPartConnIndex))
		{
			sb.Append("-找不到“主物体”可铰接的碰撞器");
			return false;
		}
		//if (Model.WillConnectTarget == null)
		//{
		//	sb.Append("没有可铰接的目标零件");
		//	return false;
		//}
		//IConnectableCtrl part2 = Model.WillConnectTarget;
		if (!part2.TryGetOverlapPointEditColliderIndex(anchorPos, out int secondPartConnIndex))
		{
			sb.Append("-找不到“铰接对象”可铰接的碰撞器");
			return false;
		}
		conn = PartConnectionManager.Instance.CreateFixedConnection(part1, firstPartConnIndex, part2, secondPartConnIndex, anchorPos);
		//Model.IsDirty_BearingColor = true;
		if (conn != null)
		{
			return true;
		}
		else
		{
			Debug.LogError("铰接对象创建失败！！！");
			return false;
		}
	}

	public bool TryCreateHingeConnection(IConnectableCtrl part, Vector2 bearingPlacePos, StringBuilder sb)
	{
		//if (!Model.CurrentConnectCursorPos.HasValue)
		//{
		//	sb.Append("没有位置数据");
		//	return false;
		//}
		//Vector2 anchorPos = Model.CurrentConnectCursorPos.Value;
		BasePartConnection conn = PartConnectionManager.Instance.CreateHingeConection(bearingPlacePos);
		if (!part.TryGetOverlapPointEditColliderIndex(bearingPlacePos, out int overlapColliderIndex))
		{
			return false;
		}
		conn.ConnectedParts.Add(part, overlapColliderIndex);
		return conn != null;
	}

	/// <summary>
	/// 将主零件连接到轴承
	/// </summary>
	/// <param name="part"></param>
	/// <param name="bearingGO"></param>
	/// <returns></returns>
	public bool TryConnectPartToBearing(IConnectableCtrl part, EditingBearing bearingGO)
	{
		// 轴承图标所代表的连接
		BasePartConnection findResult = PartConnectionManager.Instance.AllConnection.Find(a => a.Bearing == bearingGO);
		if (findResult == null)
		{
			return false;
		}
		Debug.Assert(!findResult.IsFixedJoint);
		// 主零件必须覆盖轴承位置
		if (!part.TryGetOverlapPointEditColliderIndex(bearingGO.transform.position, out int overlapColliderIndex))
		{
			return false;
		}
		// 轴承没有主零件一说，可连接列表的零件均可删除。
		// 不过可以设计成属于主零件，默认第一个连接对象
		// 或者修改conn数据结构，增加一个字段为连接的主零件
		PartHingeConnection hingeConnection = findResult as PartHingeConnection;
		// 添加当前零件到该轴承连接
		hingeConnection.ConnectedParts.Add(part, overlapColliderIndex);
		//if (!hingeConnection.connectedParts.ContainsKey(Model.GetConnectMain))
		//{
		//	hingeConnection.connectedParts.Add(Model.GetConnectMain, -1);
		//}
		//hingeConnection.connectedParts[Model.GetConnectMain] = overlapColliderIndex;
		//Model.IsDirty_BearingColor = true;
		return true;
	}


	/// <summary>
	/// 创建物理连接
	/// 连接物理碰撞器不依赖物理零件激活
	/// </summary>
	public void CreatePhysicsConnections(Transform bearingParent)
	{
		while (_jointList.Count > 0)
		{
			GameObject.DestroyImmediate(_jointList[0]);
			_jointList.RemoveAt(0);
		}
		//Transform parent = ModelSimulate.Instance.ParentOfClones.transform;
		List<AnchoredJoint2D> jointList = new List<AnchoredJoint2D>(AllConnection.Count*2); // 容量大概
		foreach (BasePartConnection itemConnection in AllConnection)
		{
			if (itemConnection.IsFixedJoint)
			{
				// 单个零件没有固定连接 ， 至少两个零件才能有固定连接
				Debug.Assert(itemConnection.ConnectedParts.Count == 2);
				IConnectableCtrl partA = null, partB = null;
				int connectIndexA = -1, connectIndexB = -1;
				bool firstConnection = true;
				foreach (KeyValuePair<IConnectableCtrl, int> item in itemConnection.ConnectedParts)
				{
					if (firstConnection)
					{
						firstConnection = false;
						connectIndexA = item.Value;
						partA = item.Key;
					}
					else
					{
						connectIndexB = item.Value;
						partB = item.Key;
					}
				}
				AnchoredJoint2D jiont = ConnectPhysicsCollider(partA.GetPhysicsCollider(connectIndexA), partB.GetPhysicsCollider(connectIndexB), itemConnection.AnchorPosition, true);
				Debug.Assert(jiont != null);
				SetIgnorePart(partA, itemConnection.ConnectedParts, null);
			}
			else
			{
				GameObject bearing = GameObject.Instantiate(PartConfig.Instance.PhysicsScrewrefab, bearingParent);
				bearing.transform.position = itemConnection.AnchorPosition;
				Collider2D bearingCollider = bearing.GetComponentInChildren<Collider2D>(true);
				foreach (KeyValuePair<IConnectableCtrl, int> itemConnectedPart in itemConnection.ConnectedParts)
				{
					//foreach (int itemConnectColliderIndex in itemConnectedPart.Value)
					//{
					//	ConnectPhysicsCollider(bearingCollider, itemConnectedPart.Key.GetPhysicsCollider(itemConnectColliderIndex), itemConnection.AnchorPosition, false);
					//	SetIgnorePart(itemConnectedPart.Key, itemConnection.connectedParts, bearingCollider);
					//}
					ConnectPhysicsCollider(bearingCollider, itemConnectedPart.Key.GetPhysicsCollider(itemConnectedPart.Value), itemConnection.AnchorPosition, false);
					SetIgnorePart(itemConnectedPart.Key, itemConnection.ConnectedParts, bearingCollider);
				}
			}
		}
	}


	public bool FindConnectableColliders ( Vector2 containPos,PlayerPartCtrl self, out List<Collider2D> findResult)
	{
		findResult = new List<Collider2D>();
		for (int i = 0; i < PlayerPartManager.Instance.AllColliders.Count; i++)
		{
			if (PlayerPartManager.Instance.ColliderToPartCtrl[PlayerPartManager.Instance.AllColliders[i]] != self)
			{
				if (PlayerPartManager.Instance.AllColliders[i].OverlapPoint(containPos))
				{
					findResult.Add(PlayerPartManager.Instance.AllColliders[i]);
				}
			}
		}

		foreach (PlayerPartCtrl item in PlayerPartManager.Instance.AllPlayerPartCtrls)
		{
			if (item != self)
			{
				if(item.MyEditPartAccesstor.GetConnectableCollider(containPos, out Collider2D connectableCollider))
				{
					findResult.Add(connectableCollider);
				}
			}
		}
		return findResult.Count > 0;
	}

	public bool IsOverlapPart ( Vector2 containPos, PlayerPartCtrl ctrlData, out Collider2D result)
	{
		var partColliderList = ctrlData.MyEditPartAccesstor.AllConectableColliders;
		for (int i = 0; i < partColliderList.Count; i++)
		{
			if (partColliderList[i].OverlapPoint(containPos))
			{
				result = partColliderList[i];
				return true;
			}
		}
		result = null;
		return false;
	}

	/// <summary>
	/// 创建一个连接
	/// </summary>
	/// <param name="master"></param>
	/// <param name="masterColliderIndex"></param>
	/// <param name="target"></param>
	/// <param name="targetColliderIndex"></param>
	/// <param name="isFixed"></param>
	/// <param name="connectWorldPos"></param>
	/// <returns></returns>
	public PartFixedConnection CreateFixedConnection(IConnectableCtrl master, int masterColliderIndex, IConnectableCtrl target, int targetColliderIndex, Vector2 connectWorldPos)
	{
		PartFixedConnection partConnection = new PartFixedConnection(connectWorldPos, master, masterColliderIndex, target, targetColliderIndex);
		AllConnection.Add(partConnection);
		return partConnection;
	}

	/// <summary>
	/// 创建一个轴承连接
	/// 这里和固定焊接不同，只需
	/// </summary>
	/// <param name="anchorPos"></param>
	/// <returns></returns>
	public PartHingeConnection CreateHingeConection(Vector2 anchorPos)
	{ 
		PartHingeConnection partConnection = new PartHingeConnection(anchorPos);
		AllConnection.Add(partConnection);
		return partConnection;
	}

	/// <summary>
	/// 连接这个零件到轴承
	/// </summary>
	/// <param name="bearingObject"></param>
	/// <param name="part"></param>
	/// <param name="connectPartColliderIndex"></param>
	public void AddPartToHingeJoint(GameObject bearingObject, IConnectableCtrl part, int connectPartColliderIndex)
	{
		PartHingeConnection hingeConnection = AllConnection.Find(conn => conn.Bearing == bearingObject) as PartHingeConnection;
		Debug.Assert(hingeConnection != null);
		if (hingeConnection.ConnectedParts.ContainsKey(part))
		{
			Debug.Log("该零件已经连接到轴，这里删除了旧的连接");
			hingeConnection.ConnectedParts.Remove(part);
		}
		if (!hingeConnection.ConnectedParts.ContainsKey(part))
		{
			hingeConnection.ConnectedParts.Add(part, connectPartColliderIndex);
		}
		else
		{
			hingeConnection.ConnectedParts[part]=connectPartColliderIndex;
		}
	}

	/// <summary>
	/// 删除该连接
	/// 删除连接在这个轴承上的所有
	/// </summary>
	/// <param name="scrw"></param>
	public void DeleteConnection(BasePartConnection deleteTarget)
	{
		DeleteConnection(deleteTarget.Bearing);
	}

	public void DeleteConnection(EditingBearing scrw)
	{
		//PartConnection findResult = AllConnection.Find((PartConnection item) => { return item.BearingEditIcon == scrw; });
		//AllConnection.Remove(findResult);
		BasePartConnection findResult = AllConnection.Find(itemConn => itemConn.Bearing == scrw);
		findResult.ConnectedParts.Clear();
		AllConnection.Remove(findResult);
		scrw.gameObject.SetActive(false);
		GameObject.Destroy(scrw.gameObject);
	}

	/// <summary>
	/// 清除所有和该零件有关的数据
	/// 用于零件删除或清空该零件连接
	/// </summary>
	/// <param name="part"></param>
	public void ClearPartConnection(BasePartCtrl part)
	{
		//List<BasePartConnection> removeList = AllConnection.FindAll(item => { return item.MasterPart == part || item.TargetPart == part; });
		foreach (BasePartConnection itemConntion in AllConnection)
		{
			itemConntion.ConnectedParts.Remove(part);
		}
		// 删除没有连接目标的轴承
		List<BasePartConnection> deleteTargets = AllConnection.FindAll(itemConn => itemConn.ConnectedParts.Count <= 1);
		while (deleteTargets.Count > 0)
		{
			DeleteConnection(deleteTargets[0]);
			deleteTargets.RemoveAt(0);
		}
		// 这里可能会留下一些连接的数据，这些连接只连接到一个零件
	}

	public void DispalyBearing(bool display)
	{
		foreach (var item in AllConnection)
		{
			item.Bearing.gameObject.SetActive(display);
		}
	}

    public override void ResetData()
    {
        AllConnection.Clear();
    }

	// --------------- //
	// --  私有方法
	// --------------- //
	// 加载时场景对象有交接出错
	// 场景零件的item2为-1
	protected override void LoadArchive(ArchiveManager.Archive archive)
	{
        ResetData();
        foreach (PartConnectionCoreData itemConnectionData in archive.partConnectionsArchive)
		{
			BasePartConnection connection;
			if (itemConnectionData.isFixed)
			{
				IConnectableCtrl part1 = GetPartCtrlByStrID(itemConnectionData.connectedPartIDs[0]);
				IConnectableCtrl part2 = GetPartCtrlByStrID(itemConnectionData.connectedPartIDs[1]);
				int part1ConnectIndex = itemConnectionData.connectedPartCollidersIndex[0];
				int part2ConnectIndex = itemConnectionData.connectedPartCollidersIndex[1];
				connection = new PartFixedConnection(itemConnectionData.anchorPosition, part1, part1ConnectIndex, part2, part2ConnectIndex);
			}
			else
			{
				connection = new PartHingeConnection(itemConnectionData.anchorPosition);
				for (int i = 0; i < itemConnectionData.connectedPartIDs.Length; i++)
				{
					string itemConnectedPartIDs = itemConnectionData.connectedPartIDs[i];
					IConnectableCtrl part = GetPartCtrlByStrID(itemConnectedPartIDs);
					if (!connection.ConnectedParts.ContainsKey(part))
					{
						connection.ConnectedParts.Add(part, itemConnectionData.connectedPartCollidersIndex[i]);
					}
					connection.ConnectedParts[part] = itemConnectionData.connectedPartCollidersIndex[i];
				}
			}
			AllConnection.Add(connection);
			//PlayerPartCtrl part1;
			//IConnectableCtrl part2;
			//part1 = PlayerPartManager.Instance.AllPlayerPartCtrls[int.Parse(itemConnectionData.part1ID)];
			//if (int.TryParse(itemConnectionData.part2ID, System.Globalization.NumberStyles.Integer, null, out int part1IDNum))
			//{
			//	part2 = PlayerPartManager.Instance.AllPlayerPartCtrls[part1IDNum];
			//}
			//else
			//{
			//	part2 = new ScenePartCtrl(LevelProgressBase.Instance.GetScenepart(Hash128.Parse(itemConnectionData.part2ID))) as IConnectableCtrl;
			//}
			//CreateConnection(part1, itemConnectionData.part1ColliderIndex, part2, itemConnectionData.part2ColliderIndex, itemConnectionData.isFixedJoint, itemConnectionData.anchorPos);
			//AllConnection.Add(conn);
		}
		ModelEdit.Instance.IsDirty_BearingColor = true;
		//foreach (BasePartConnection item in AllConnection)
		//{
		//	item.Bearing.GetComponent<EditingBearing>().SetDisplay(false, false);
  //      }
	}

	protected override void SaveIntoArchive(ArchiveManager.Archive archive)
	{
		archive.partConnectionsArchive = new (AllConnection.Count);
		foreach (BasePartConnection itemConnection in AllConnection)
		{
			PartConnectionCoreData connData = new PartConnectionCoreData()
			{
				anchorPosition = itemConnection.AnchorPosition,
				isFixed = itemConnection.IsFixedJoint,
				connectedPartIDs = new string[itemConnection.ConnectedParts.Count],
				connectedPartCollidersIndex = new int[itemConnection.ConnectedParts.Count],
			};
			int indexConnectedPart = 0;
			foreach (KeyValuePair<IConnectableCtrl, int> itemConnectedPart in itemConnection.ConnectedParts)
			{
				connData.connectedPartIDs[indexConnectedPart] = GetStrIDByPartCtrl(itemConnectedPart.Key);
				//foreach (int itemConnectColliderIndex in itemConnectedPart.Value)
				//{
				//	connData.connectedPartCollidersIndex[indexConnectedPart] = itemConnectColliderIndex;
				//} ???
				connData.connectedPartCollidersIndex[indexConnectedPart] = itemConnectedPart.Value;
				indexConnectedPart++;
			}
			archive.partConnectionsArchive.Add(connData);
		}
	}

	/// <summary>
	/// 对两个碰撞器增加铰接组件
	/// </summary>
	/// <param name="colliderA"></param>
	/// <param name="colliderB"></param>
	/// <param name="anchorPos"></param>
	/// <param name="fixedJoint"></param>
	/// <returns></returns>
	private AnchoredJoint2D ConnectPhysicsCollider(Collider2D colliderA, Collider2D colliderB, Vector2 anchorPos, bool fixedJoint)
	{
		Rigidbody2D rigidbodyA = colliderA.attachedRigidbody;
		Rigidbody2D rigidbodyB = colliderB.attachedRigidbody;
		AnchoredJoint2D jointCmpnt = (fixedJoint ? colliderA.gameObject.AddComponent<FixedJoint2D>() : colliderA.gameObject.AddComponent<HingeJoint2D>());
		Vector2 anchorLocal = rigidbodyA.transform.InverseTransformPoint(anchorPos);
		Vector2 anchorTargeLocal = colliderB.transform.InverseTransformPoint(anchorPos);
		jointCmpnt.enabled = false;
		jointCmpnt.connectedBody = rigidbodyB.GetComponent<Rigidbody2D>();
		jointCmpnt.anchor = anchorLocal;
		jointCmpnt.connectedAnchor = anchorTargeLocal;
		jointCmpnt.enableCollision = true;
		jointCmpnt.autoConfigureConnectedAnchor = false;
		jointCmpnt.enabled = true;
		return jointCmpnt;
	}

	/// <summary>
	/// 获取零件的ID
	/// 玩家零件为序号
	/// 场景零件为生成的随机值
	/// </summary>
	/// <param name="partCtrl"></param>
	/// <returns></returns>
	private string GetStrIDByPartCtrl(IConnectableCtrl partCtrl)
	{
		if (partCtrl is PlayerPartCtrl)
		{
			return PlayerPartManager.Instance.AllPlayerPartCtrls.IndexOf(partCtrl as PlayerPartCtrl).ToString();
		}
		else
		{
			return (partCtrl as ScenePartCtrl).GetID.ToString();
		}
	}

	private IConnectableCtrl GetPartCtrlByStrID(string strID)
	{
		if (int.TryParse(strID, System.Globalization.NumberStyles.Integer, null, out int partIndex))
		{
			return PlayerPartManager.Instance.AllPlayerPartCtrls[partIndex];
		}
		else
		{
			return LevelProgressBase.Instance.GetScenepart(Hash128.Parse(strID)).MyCtrl as IConnectableCtrl;
		}
	}

	/// <summary>
	/// 对被连接的零件设置碰撞器忽略
	/// </summary>
	/// <param name="partMain"></param>
	/// <param name="connectedParts"></param>
	/// <param name="bearing"></param>
	private void SetIgnorePart(IConnectableCtrl partMain, Dictionary<IConnectableCtrl, int> connectedParts, Collider2D bearing = null)
	{
		// 所有连接在这个轴承上的零件都要忽略碰撞
		foreach (var itemConnectedCollider in partMain.GetIgnorableColliders)
		{
			// 忽略和轴承的碰撞
			if (bearing)
			{
				Physics2D.IgnoreCollision(bearing, itemConnectedCollider);
			}
			foreach (var itemConnectedOtherPart in connectedParts)
			{
				if (partMain == itemConnectedOtherPart.Key)
				{
					continue;
				}
				foreach (var itemConectedOtherCollider in itemConnectedOtherPart.Key.GetIgnorableColliders)
				{
					Physics2D.IgnoreCollision(itemConectedOtherCollider, itemConnectedCollider);
				}
			}
		}
	}
	// --------------- //
	// --  类型
	// --------------- //

	// --------------- //
	// --  私有方法
	// --------------- //

	// --------------- //
	// --  类型
	// --------------- //
}