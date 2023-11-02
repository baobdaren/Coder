using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// 运行关卡的界面
/// 主要负责运行零件
/// </summary>
public class MainViewSimulate : BaseView
{
	// ---------- //
	// --- 序列化
	// ---------- //
	[SerializeField]
	Button buttonExit;

	[SerializeField]
	Button buttonRun;

	[SerializeField]
	Button buttonReset;

	// ---------- //
	// --- 私有属性
	// ---------- //
	private readonly List<GameObject> _errorLineList = new List<GameObject>();
	private Transform _conflictLineParent
	{
		get
		{
			if (_conflictParentGameObject == null)
			{
				_conflictParentGameObject = new GameObject("Parent Of Conflict Line").transform;
			}
			return _conflictParentGameObject;
		}
	}
	private Transform _conflictParentGameObject;

	// ---------- //
	// --- 公有属性
	// ---------- //

	public static bool IsRunInPhysicsModel
	{
		get => ParentsManager.Instance.ParentOfPhysicsParts && ParentsManager.Instance.ParentOfPhysicsParts.activeSelf;
	}

	// ---------- //
	// --- Unity 消息
	// ---------- //

	protected override void Awake()
	{
		base.Awake();
		buttonExit.onClick.AddListener(OnClick_Exit);
		buttonRun.onClick.AddListener(OnClicked_RunGame);
		buttonReset.onClick.AddListener(OnClicked_Reset);
	}

	//private void Update()
	//{
	//	if (!ModelSimulate.Instance.Simulating/* && Time.frameCount % 4 == 0*/)
	//	{
	//		ControllerSimulate.Instance.TestPartConflict(out var _, out var conflictPosList);
	//		DisplayErrorLine(conflictPosList, ControllerSimulate.Instance.TestBeyongBound().Item2);
	//	}
	//}

	// ---------- //
	// --- 私有方法
	// ---------- //
	public override void EnterView()
	{
		base.EnterView();
		ControllerSimulate.Instance.CreateCommanderUI();
	}


	private void OnClick_Exit()
	{
		OnClicked_Reset();
		UIManager.Instance.OpenView<MainViewStart>();
	}


	//private void DisplayErrorLine(List<List<Vector3>> conflictList, List<Vector3[]> beyoundBoundPos)
	//{
	//	while (_errorLineList.Count < conflictList.Count + beyoundBoundPos.Count)
	//	{
	//		_errorLineList.Add(CreatePartErrorLine());
	//	}
	//	for (int i = 0; i < _errorLineList.Count; i++)
	//	{
	//		_errorLineList[i].SetActive(i < conflictList.Count + beyoundBoundPos.Count);
	//	}
	//	for (int i = 0; i < conflictList.Count; i++)	
	//	{
	//		if (i < conflictList.Count)
	//		{
	//			_errorLineList[i].gameObject.SetActive(true);
	//			conflictList[i].Sort((a, b) => { return a.x < b.x ? -1 : 1; });
	//			_errorLineList[i].GetComponent<LineRenderer>().SetPositions(conflictList[i].ToArray());
	//		}
	//	}
	//	for (int i = conflictList.Count; i < conflictList.Count + beyoundBoundPos.Count; i++)
	//	{
	//		int beyoundBoundPosIndex = i - conflictList.Count;
	//		Array.Sort(beyoundBoundPos[beyoundBoundPosIndex], (a, b) => { return a.x < b.x ? -1 : 1; });
	//		_errorLineList[i].GetComponent<LineRenderer>().SetPositions(beyoundBoundPos[beyoundBoundPosIndex]);
	//	}
	//}


	private GameObject CreatePartErrorLine()
	{
		GameObject conflictLine = new GameObject("Conflict Line" + _errorLineList.Count);
		conflictLine.transform.parent = _conflictLineParent;
		LineRenderer lineCmpnt = conflictLine.AddComponent<LineRenderer>();
		lineCmpnt.startWidth = 0.5f;
		lineCmpnt.positionCount = 4;
		lineCmpnt.numCornerVertices = 6;
		lineCmpnt.material = MaterialConfig.Instance.RedErrorLine;
		lineCmpnt.sortingLayerID = SortingLayer.NameToID("ConnectCursor");
		return conflictLine;
	}

	// ---------- //
	// --- 公有方法
	// ---------- //

	/// <summary>
	/// 运行
	/// </summary>
	public void OnClicked_RunGame()
	{
		StartCoroutine(ControllerSimulate.Instance.StartPhysicsSimulate());
	}

	public void OnClicked_Reset()
	{
		ModelSimulate.Instance.IsSimulating = false;
		StartCoroutine(ControllerSimulate.Instance.Cor_DeletePhysicsClones());
		ParentsManager.Instance.ParentOfEditBearing.SetActive(true);
		ParentsManager.Instance.ParentOfEditParts.SetActive(true);
		GameManager.Instance.ReplayLevel();
	}
}
