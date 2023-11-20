# A*寻路算法

# A*寻路算法了理论
启发式搜索。

# 基本
* 加入节点到开放列表时处理
```c#
G值=G.parent.G + GetG(G, center)
// G值为从起点到该点的代价
H值=预测值(startPos, endPos)
// 从此点到终点的预测代价，这是启发搜索的核心
F=G+H
// F值=G值+H值
```
#Q&A
Q：加入周围节点到开放列表后，此时设置被加入节点的父节点为中心节点吗？
A：被加入节点的父节点设置，取决于G值，总是设置G只最小路径中的父物体！在加入周围节点后，对于已经存在的节点，如果已有G值大于中心节点为父物体的G值，则修改这个已经从在节点的G值到中心节点。
Q：每次选取下一个预测节点依据时候什么？
A：F！

# 代码
```C#
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.X86.Avx;

public class AStart: SerializedMonoBehaviour
{
	// ------------- //
	// -- 序列化
	// ------------- //
	[SerializeField]
	private bool StepByFKey = false;
	[SerializeField]	
	GameObject Wall;
	[SerializeField]
	GameObject Ground;
	[SerializeField]
	GameObject Path;
	[SerializeField]
	GameObject DirVector;
	[SerializeField]
	GameObject ClosedTile;
	[SerializeField]
	TextMeshProUGUI TitleOpensCount;

	//[SerializeField]
	//Vector2Int TargetPos;

	[SerializeField]
	GameObject Flag;

	[SerializeField]
	Vector2Int StartPos;
	[SerializeField]
	Vector2Int EndPos;

	[SerializeField]
	GameObject _title;

	public static GameObject SetTxt;
	// ------------- //
	// -- 私有成员
	// ------------- //
	[SerializeField]
	private bool[,] GameMap = new bool[16, 16];
	[Button("清空地图")]
	public void ClearMap()
	{
		for (int i = 0; i <= GameMap.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= GameMap.GetUpperBound(1); j++)
			{
				GameMap[i,j] = false;
			}
		}
	}

	private Dictionary<Vector2Int, APos> opens = new Dictionary<Vector2Int, APos>();
	private Dictionary<Vector2Int, APos> closes = new Dictionary<Vector2Int, APos>();
	//private HashSet<Vector2Int> closes = new HashSet<Vector2Int>();

	Dictionary<Vector2Int, GameObject> MapTileCostUI = new Dictionary<Vector2Int, GameObject>();
	List<GameObject> MapDirector = new();
	List<GameObject> MapClosed = new List<GameObject>();
	// ------------- //
	// -- 公有成员
	// ------------- //

	// ------------- //
	// -- Unity 消息
	// ------------- //
	private void Awake()
	{
		AStart.SetTxt = _title;
		Wall.SetActive(false);
		Ground.SetActive(false);
		Path.SetActive(false);
		Flag.SetActive(false);
		DirVector.SetActive(false);
		ClosedTile.SetActive(false);

		for (int i = 0; i < GameMap.Length; i++)
		{
			MapDirector.Add(GameObject.Instantiate(DirVector, Vector2.zero, Quaternion.identity));
			MapDirector[i].SetActive(false);
		}
		for (int i = 0; i < GameMap.Length; i++)
		{
			MapClosed.Add(GameObject.Instantiate(ClosedTile, Vector2.zero, Quaternion.identity));
			MapClosed[i].SetActive(false);
		}
	}

	private void Start()
	{
		DrawMap();

		var start = new APos(StartPos.x, StartPos.y);
		var end = new APos(EndPos.x, EndPos.y);
		start.CostToEnd = DistanceTo(start.Pos, end.Pos);
		end.CostToEnd = 0;

		StartCoroutine(GetPath(start, end));
	}

	private void Update()
	{
		TitleOpensCount.text = "";
		foreach (var item in opens)
		{
			TitleOpensCount.text += item.ToString() + "G=" + item.Value.CostFromStart + "\n";
		}
		int useDirIndex = 0;
		Dictionary < Vector2Int, APos > tmpSet = new Dictionary<Vector2Int, APos >(opens);
		tmpSet.AddRange(closes);
		foreach (KeyValuePair<Vector2Int, APos> item in tmpSet)
		{
			if (item.Value.Parent == null) continue;
			Vector3 v3CurrentPos = new Vector3(item.Key.x, item.Key.y, 0);
			Vector2Int vDir = (item.Value.Parent.Pos - item.Value.Pos);
			Vector3 v3Dir = new Vector3(vDir.x, vDir.y, 0);
			Quaternion q = Quaternion.FromToRotation(Vector3.down, v3Dir);
			MapDirector[useDirIndex].transform.SetPositionAndRotation(v3CurrentPos, q);
			MapDirector[useDirIndex].SetActive(true);
			useDirIndex++;
		}
		for (; useDirIndex < MapDirector.Count; useDirIndex++)
		{
			MapDirector[useDirIndex].SetActive(false);
		}

		int indexClosed = 0;
		foreach (var item in closes)
		{
			MapClosed[indexClosed].SetActive(true);
			MapClosed[indexClosed++].transform.position = new Vector3(item.Key.x, item.Key.y, 0);
		}
		for(;indexClosed < MapDirector.Count; indexClosed++)
		{
			MapClosed[indexClosed].SetActive(false);
		}


		//foreach (var item in MapTileCostUI)
		//{

		//}
		foreach (KeyValuePair<Vector2Int, APos> item in opens)
		{
			MapTileCostUI[item.Key].transform.Find("G").GetComponent<TextMeshProUGUI>().text 
				= item.Value.CostFromStart.ToString();
			MapTileCostUI[item.Key].transform.Find("H").GetComponent<TextMeshProUGUI>().text
				= item.Value.CostToEnd.ToString();
			MapTileCostUI[item.Key].transform.Find("F").GetComponent<TextMeshProUGUI>().text
				= item.Value.SumCost.ToString();
			MapTileCostUI[item.Key].gameObject.SetActive(true);
		}

		foreach (var item in closes)
		{
			MapTileCostUI[item.Key].transform.Find("G").GetComponent<TextMeshProUGUI>().text
	= item.Value.CostFromStart.ToString();
			MapTileCostUI[item.Key].transform.Find("H").GetComponent<TextMeshProUGUI>().text
				= item.Value.CostToEnd.ToString();
			MapTileCostUI[item.Key].transform.Find("F").GetComponent<TextMeshProUGUI>().text
				= item.Value.SumCost.ToString();
			MapTileCostUI[item.Key].gameObject.SetActive(true);
		}
	}
	// ------------- //
	// -- 公有方法
	// ------------- //

	// ------------- //
	// -- 私有方法
	// ------------- //
	private void DrawMap()
	{
		for (int i = 0; i <= GameMap.GetUpperBound(0) ; i++)
		{
			for (int j = GameMap.GetUpperBound(1); j >= 0; j--)
			{
				GameObject g = null;
				GameMap[i, j] = !GameMap[i, j];
				if (GameMap[i, j] == false)
				{
					g = Instantiate(Wall, new Vector3(i, j, 0), Quaternion.identity);
				}
				else
				{
					g = Instantiate(Ground, new Vector3(i, j, 0), Quaternion.identity);
				}
				g.SetActive(true);
				g.transform.name = $"{i},{j}";
				_title.SetActive(false);
				GameObject tileInfoTex = Instantiate(_title, new Vector3(i, j, 0), Quaternion.identity);
				tileInfoTex.name = $"info {i},{j} ";
				MapTileCostUI.Add(new Vector2Int(i,j), tileInfoTex);
			}
		}
		Instantiate(Flag, new Vector3(StartPos.x, StartPos.y, 0), Quaternion.identity).SetActive(true);
		Instantiate(Flag, new Vector3(EndPos.x, EndPos.y, 0), Quaternion.identity).SetActive(true);
	}

	/// <summary>
	/// 寻路主方法
	/// </summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <returns></returns>
	private IEnumerator  GetPath(APos start, APos end)
	{
		List<APos> r = new List<APos>();
		Debug.LogError($"开始查找路径，起点为{start}，终点为{end}");
		opens.Add(start.Pos, start);
		APos current = start;
		//while (opens.ContainsValue(start) == false || opens.Count == 0)

		int findTimes = 0;
		while (current != end)
		{
			if(current == null)  break;
			current = GetNextBestPos();
			if (opens.Count == 0)  break;
			if(current.Pos == end.Pos) { break; }
            if (current == null)  break ;

			//closes.Add(current.Pos ,current);
			//opens.Remove(current.Pos);

			if(StepByFKey)
			{
				while (Keyboard.current.fKey.wasPressedThisFrame == false)
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.05f);
			}
            AddAroundToOpens(current, end);
			findTimes++;
			// 从开放列表移除？那么后面可能需要比较节点花费？？
		}
		Debug.Log("查找次数：" + findTimes);
		while (current != null && current.Parent != null)
		{
			r.Add(current);
			current = current.Parent;
		}

		Debug.Log($"路径点数{((r == null) ? (0) : (r.Count))}");
		foreach (var a in r)
		{
			GameObject.Instantiate(Path, new Vector3(a.Pos.x, a.Pos.y), Quaternion.identity).gameObject.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
	}

	/// <summary>
	/// 增加周围节点到开放列表中
	/// </summary>
	/// <param name="center"></param>
	/// <param name="end"></param>
	private void AddAroundToOpens(APos center, APos end)
	{
		int addCount = 0;
		int resetCount = 0;
		//closes.Add(center.Pos, center);
		for(int indexH = -1; indexH <  2; indexH++)
		{
			for(int indexV = -1; indexV < 2; indexV++)
			{
				if (indexH == 0 && indexV == 0) continue;
				int x = center.Pos.x + indexH;
				if (x < 0 || x > GameMap.GetUpperBound(0)) // 
					continue;
				int y = center.Pos.y + indexV;
				if (y < 0 || y > GameMap.GetUpperBound(1))
					continue;
				if (GameMap[x, y] == false)
					continue;
				if (closes.ContainsKey(new Vector2Int(x,y))) continue;
				// 对周围节点重新计算需要处理一两件事
				// 创建一个以选择中心为父物体的节点
				// 1：如果开放列表没有位置，则直接加入该点
				// 2：如果开放列表已有此位置，则比较列表中点与新创建节点的来路代价，选择更小的存放
				// 这是一个临时节点
				APos tmp = new APos(x, y);
				tmp.Parent = center;
				//tmp.CostFromStart = tmp.Parent.CostFromStart + ((tmp.Pos.x == tmp.Parent.Pos.x || tmp.Pos.y == tmp.Parent.Pos.y) ? 10 : 14);
				tmp.CostToEnd = DistanceTo(tmp.Pos, end.Pos);
				// 新增的开放节点
				if (opens.ContainsKey(new Vector2Int(x,y)) == false)
				{
					opens.Add(tmp.Pos, tmp);
					addCount++;
				}
				else // 这个位置的节点已经加入开放列表，但我们需要比较来路的代价，选择更小的，从而更新父节点
				{
					APos r = opens[new Vector2Int(x, y)];
					// 已存在节点，但是更加接近终点 ！！！
					if(r.CostFromStart > tmp.CostFromStart)
					{
						//opens.Remove(r.Pos);
						//opens.Add(tmp.Pos, tmp);
						opens[new Vector2Int(x,y)].Parent = center;
						resetCount++;
					}
				}
			}
		}
		// 加入周围节点后，把这个点从开放列表移入关闭列表
		opens.Remove(center.Pos);
		closes.Add(center.Pos, center);
		Debug.LogError($"增加节点，中心为{center.Pos.x}:{center.Pos.y} 此次新增节点{addCount}个，修改花费更少的节点{resetCount}个");
	}

	private int DistanceTo(Vector2Int start, Vector2Int end)
	{
		Vector2Int disVec = new Vector2Int(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
		int cmpDis = Mathf.Abs(disVec.x - disVec.y);
		return (cmpDis * 10) + (Mathf.Max(disVec.x, disVec.y) - cmpDis) * 14;
	}

	/// <summary>
	/// 选择最好的节点，好坏不影响结果，但是越好越有利于最终结果
	/// </summary>
	/// <returns></returns>
	private APos GetNextBestPos()
	{
		APos minPos = null;
		foreach (KeyValuePair<Vector2Int, APos> item in opens)
		{
			//if(minPos == null || minPos.CostToEnd > item.Value.CostToEnd)  // 30
			if(minPos == null || minPos.SumCost > item.Value.SumCost)  // 66
			//if(minPos == null || minPos.CostFromStart > item.Value.CostFromStart)  // 145
			{
				minPos = item.Value;
			}
		}
		return minPos;
	}
	// ------------- //
	// -- 类型
	// ------------- //

	public class APos:object
	{
		public APos(int x, int y)
		{
			Pos = new Vector2Int(x, y);
		}
		public APos Parent 
		{
			get => _parent;
			set
			{
				_parent = value;
				CostFromStart = _parent.CostFromStart + ((_parent.Pos.x == Pos.x || _parent.Pos.y == Pos.y) ? 10 : 14);
			}
		}
		private APos _parent;
		public Vector2Int Pos { get; private set; }
		// 从起点开始 追踪路径需要的代价
		public int CostFromStart { get; set; }
		// 到达目的地 忽略碰撞时需要的代价
		public int CostToEnd { get; set; }
		public int SumCost { get => CostFromStart + CostToEnd; }

		public override string ToString()
		{
			return $"{Pos.x},{Pos.y}";
		}
	}
}


```
