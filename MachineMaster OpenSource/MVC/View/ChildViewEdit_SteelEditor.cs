using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.U2D;

/// <summary>
/// 对于框架(钢材组成的结构)的编辑，采用单独的设计界面，钢材是一个特殊又基础的零件
/// </summary>
public class ChildViewEdit_SteelEditor : BaseChildView
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[SerializeField]
	private NiceButton BeginEditSteelBtn;
	[SerializeField]
	private NiceButton DeleteIconBtn;
	/// <summary>
	/// 插入点按钮，位于节点中间
	/// </summary>
	[SerializeField]
	private NiceButton InsertIconBtn;
	[SerializeField]
	private bool TestFlag = true;
	// ----------------//
	// --- 公有成员
	// ----------------//

	// ----------------//
	// --- 私有成员
	// ----------------//
	/// <summary>
	/// 点击开始编辑后为True
	/// </summary>
	public bool IsEditing { get; private set; } = false;
	public bool IsInserting { get; private set; } = false;
	[ReadOnly]
	public bool IsMoveingCorner = false;
	/// <summary>
	/// 左击拐角图标后，以该图标为插入点
	/// 删除任意一个拐点后清空
	/// </summary>
	private SteelAccessor CurSteelEditAccesstor 
	{
		get
		{
			if (CurSteelCtrl == null) return null;
			return CurSteelCtrl.MyEditPartAccesstor as SteelAccessor;
		}
	}
	private PlayerPartCtrl CurSteelCtrl 
	{
		get
		{
			if (!ModelEdit.Instance.IsEditing || (ModelEdit.Instance.GetConnectMain is ScenePartCtrl) || ModelEdit.Instance.EditingPlayerPartCtrl.MyPartType != PartTypes.Steel) return null;
			return ModelEdit.Instance.EditingPlayerPartCtrl as PlayerPartCtrl;
		}
	}
	private List<(Vector3, Quaternion)> _curSteelCorners = new List<(Vector3, Quaternion)>(11);
	private int _curSteelCornerUpdatedFrame = -1;
	private List<(Vector3, Quaternion)> CurSteelCorners
	{
		get
		{
			if (_curSteelCorners.Count == 0 || _curSteelCornerUpdatedFrame != Time.frameCount)
			{
				PartSuperFactory.GetFactor<SteelFactory>().GetWorldSpaceCroners_NonAlloc(CurSteelCtrl, CurSteelEditAccesstor, ref _curSteelCorners);
			}
			return _curSteelCorners;
		}
	}

	private List<NiceButton> _btnDeleteNode;
	private List<NiceDrag> _dragMoveNode;
	//private List<NiceButton> _btnInsertNode;
	//private int? InsertAtIndex { set; get; }
	// ----------------//
	// --- Unity消息
	// ----------------//
	private void Awake()
	{
		_btnDeleteNode = new List<NiceButton>(10);
		_dragMoveNode = new List<NiceDrag>(10);
		// 添加删除按钮和拖拽按钮
		for (int i = 0; i < _btnDeleteNode.Capacity; i++)
		{
			if (i == 0) _btnDeleteNode.Add(DeleteIconBtn);
			else _btnDeleteNode.Add(Instantiate(DeleteIconBtn.gameObject, DeleteIconBtn.transform.parent).GetComponent<NiceButton>());
			_btnDeleteNode[i].gameObject.SetActive(false);
			NiceButton curBtn = _btnDeleteNode[i];
			_dragMoveNode.Add(curBtn.GetComponent<NiceDrag>());
			_btnDeleteNode[i].OnRightClick.AddListener(() => 
			{
				OnRightClickNode_DeleteNode(curBtn);
			});
		}
		foreach (NiceDrag item in _dragMoveNode)
		{
			NiceDrag curDrag = item;
			item.Drag.AddListener(() => { OnDragCorners(curDrag); });
		}
		InsertIconBtn.OnLeftClick.AddListener(OnLeftClickInsertNode);
		//_btnInsertNode = new List<NiceButton>(9);
		//for (int i = 0; i < _btnInsertNode.Capacity; i++)
		//{
		//	if (i == 0) _btnInsertNode.Add(InsertIconBtn);
		//	else _btnInsertNode.Add(Instantiate(InsertIconBtn.gameObject, InsertIconBtn.transform.parent).GetComponent<NiceButton>());
		//	_btnInsertNode[i].gameObject.SetActive(false);
		//	_btnInsertNode[i].OnRightClick.AddListener(() => OnClick_InsertNode());
		//}

	}

	/// <summary>
	/// 显示“可插入点”状态
	/// 在空白地方点击后尝试插入节点
	/// 
	/// </summary>
	private void Update()
	{
		if (IsEditing == false) return;
		//bool rightClickBlank = Mouse.current.rightButton.wasPressedThisFrame;
		//rightClickBlank &= !CameraActor.Instance.MouseOverUI;
		//if (rightClickBlank)
		//{
		//	if (InsertAtIndex.HasValue)
		//	{
		//		PartSuperFactory.GetFactor<SteelFactory>().RemoveCorner(CurSteelCtrl, InsertAtIndex.Value, CurSteelEditAccesstor);
		//	}
		//	FinishEditSteel();
		//	return;
		//}
		//if (!ModelEdit.Instance.IsEditing || ModelEdit.Instance.EditingPartCtrl.MyPartType != PartTypes.Steel)
		//{
		//	return;
		//}
		//if (Mouse.current.leftButton.wasPressedThisFrame && !CameraActor.Instance.MouseOverUI)
		//{
		//	int clickInsertableIndex = TryGetInsertableIndex(CameraActor.Instance.MouseWorldPos);
		//	if (clickInsertableIndex != -1)
		//	{
		//		PartSuperFactory.GetFactor<SteelFactory>().InsertCorner(CurSteelCtrl, CameraActor.Instance.MouseWorldPos, clickInsertableIndex);
		//		Debug.Log($"插入坐标成功");
		//	}
		//	else
		//	{
		//		Debug.Log($"插入坐标失败");
		//	}

		//	//// 点击距离太远时
		//	//if ((CurSteelEditAccesstor.transform.position - (Vector3)clickedWolrdPos).sqrMagnitude > 4)
		//	//{
		//	//	Debug.LogError(clickedWolrdPos);
		//	//	Debug.LogError(CurSteelEditAccesstor.transform.position);
		//	//	GameMessage.Instance.PrintMessageAtScreenCenter("没有点击在范围内" + (CurSteelEditAccesstor.transform.position - (Vector3)clickedWolrdPos).sqrMagnitude);
		//	//	return;
		//	//}
		//	//InsertAtIndex++;
		//}

		if (Time.frameCount%1==0)
		{
			if (TryGetInsertableIndex(CameraActor.Instance.MouseWorldPos) != -1)
			{ 
				InsertIconBtn.transform.position = CameraActor.Instance.MouseWorldPos;
				InsertIconBtn.gameObject.SetActive(true);
			}
			else
			{
				InsertIconBtn.gameObject.SetActive(false);
			}
		}
	}

	public override void OnEnterParentView()
	{
		// 只有编辑Steel时才显示
		if (ModelEdit.Instance.IsEditingPlayerPart && ModelEdit.Instance.EditingPlayerPartCtrl.MyPartType != PartTypes.Steel)
		{
			FinishEditSteel();
			return;
		}
		base.OnEnterParentView();
		BeginEditSteelBtn.gameObject.SetActive(true);
		BeginEditSteelBtn.onClick.RemoveAllListeners();
		BeginEditSteelBtn.onClick.AddListener(
		() =>
		{
			if (IsEditing)
			{
				FinishEditSteel();
			}
			else
			{
				BeginEditSteel();
			}
		});
		ResetButtons();
	}

	public override void OnExitParentView()
	{
		base.OnExitParentView();
		FinishEditSteel();
	}
	// ----------------//
	// --- 公有方法
	// ----------------//
	         

	public void BeginEditSteel()
	{
		if (CurSteelCtrl == null)
		{
			BeginEditSteelBtn.gameObject.SetActive(false);
			return;
		}
		IsEditing = true;
		BeginEditSteelBtn.Text = "结束编辑";
		StartCoroutine(Cor_UpdateIcons());
		ResetButtons();
	}

	public void FinishEditSteel()
	{
		BeginEditSteelBtn.gameObject.SetActive(false);
		IsEditing = false;
		BeginEditSteelBtn.Text = "开始编辑";
		ResetButtons();
		InsertIconBtn.gameObject.SetActive(false);
		StopAllCoroutines();
	}

	// ----------------//
	// --- 私有方法
	// ----------------//
	/// <summary>
	/// 左击
	/// </summary>
	/// <param name="clickedBtn"></param>
	private void OnRightClickNode_DeleteNode(NiceButton clickedBtn)
	{
		GameMessage.Instance.PrintMessageAtMousePos("删除点" + _btnDeleteNode.IndexOf(clickedBtn));
		PartSuperFactory.GetFactor<SteelFactory>().RemoveCorner(CurSteelCtrl, _btnDeleteNode.IndexOf(clickedBtn), CurSteelEditAccesstor);
	}

	private void OnDragCorners(NiceDrag dragedCmpnt)
	{ 
		int dragCornerIndex = _dragMoveNode.IndexOf(dragedCmpnt);
		var mouseWorldPos = CurSteelEditAccesstor.transform.InverseTransformPoint(CameraActor.Instance.MouseWorldPos);
		PartSuperFactory.GetFactor<SteelFactory>().UpdateCorner(CurSteelCtrl, dragCornerIndex, mouseWorldPos);
		//_curSteelEditAccesstor.InsertCorners(InsertAtIndex.Value, mouseWorldPos);
		//SteelFactory.Intsance.InsertCorner(_curSteelCtrl, mouseWorldPos, Quaternion.identity, _curSteelEditAccesstor, InsertAtIndex.Value);
	}

	private void OnLeftClickInsertNode()
	{
		int insertIndex = TryGetInsertableIndex(CameraActor.Instance.MouseWorldPos);
		if (insertIndex == -1) return;
		PartSuperFactory.GetFactor<SteelFactory>().InsertCorner(CurSteelCtrl, CameraActor.Instance.MouseWorldPos, insertIndex);
	}

	private void ResetButtons()
	{
		foreach (var item in _btnDeleteNode)
		{
			item.gameObject.SetActive(false);
		}
	}

	private IEnumerator Cor_UpdateIcons()
	{
		RectTransform rect = UIManager.Instance.GetComponent<RectTransform>();
		while (enabled && CurSteelEditAccesstor)
		{
			//worldSpaceCornerPoss = SteelFactory.Intsance.GetCorners_WorldSpace(_curSteelCtrl, _curSteelEditAccesstor);
			// 显示拐点按钮
			for (int i = 0; i < _btnDeleteNode.Count; i++)
			{
				if (i > CurSteelCorners.Count)
				{
					_btnDeleteNode[i].gameObject.SetActive(false);
					continue;
				}
				_btnDeleteNode[i].gameObject.SetActive(true);
				Vector2 screenPos;

				if (i == CurSteelCorners.Count)
				{
					screenPos = _btnDeleteNode[i].transform.position + (_btnDeleteNode[i].transform.localPosition - _btnDeleteNode[i - 1].transform.localPosition).normalized * 0.1f;
				}
				else
				{
					screenPos = (Vector2)CameraActor.Instance.MainCamera.WorldToScreenPoint(CurSteelCorners[i].Item1);
				}
				RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, CameraActor.Instance.MainCamera, out Vector2 loaclPoint);
				_btnDeleteNode[i].transform.localPosition = loaclPoint;
			}
			yield return null;
		}
		Debug.Log("退出");
	}

	private int TryGetInsertableIndex(Vector3 inputPos)
	{
		const float minDisToEndPoint = 0.03f;
		float? minInsertDis = null;
		//insertPos = Vector3.zero;
		int? minInsertPointIndex = null;
		for (int i = 1; i < CurSteelCorners.Count; i++)
		{
			Vector3 p2ToP1 = CurSteelCorners[i - 1].Item1 - CurSteelCorners[i].Item1;
			Vector3 p2ToInput = inputPos - CurSteelCorners[i].Item1;
			Vector3 p1ToInput = inputPos - CurSteelCorners[i-1].Item1;
			if (Vector3.Dot(p2ToInput, p2ToP1) <= 0 || Vector3.Dot(p1ToInput, -p2ToP1) <= 0) continue; // 判断如果处在端点两边则返回
			if (p2ToInput.sqrMagnitude < minDisToEndPoint || p1ToInput.sqrMagnitude < minDisToEndPoint) continue;
			Vector3 helpVec = Vector3.Cross(p2ToP1, Vector3.Cross(p2ToP1, p2ToInput)).normalized;
			float dis = Mathf.Abs(Vector3.Dot(p2ToInput, helpVec));
			if (!minInsertDis.HasValue || minInsertDis.Value > dis)
			{
				minInsertDis = dis;
				minInsertPointIndex = i-1;
				//insertPos = inputPos + -helpVec * dis;
			}
		}
		if (!minInsertDis.HasValue || minInsertDis.Value > 0.05f)
		{
			return -1;
		}
		//Debug.DrawLine(inputPos, insertPos);
		return minInsertPointIndex.HasValue ? minInsertPointIndex.Value : -1;
	}
	// ----------------//
	// --- 类型
	// ----------------//
}
