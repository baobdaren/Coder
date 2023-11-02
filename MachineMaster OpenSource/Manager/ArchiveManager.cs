using System;
using System.Collections.Generic;
using UnityEngine;

public class ArchiveManager
{
	// ------------------ //    
	// --- 序列化    
	// ------------------ //

	// ------------------ //    
	// --- 公有成员    
	// ------------------ //
	public static ArchiveManager Instance = new ArchiveManager();
	//public Archive CurrentArchive { get; private set; }
	private ArchiveManager() { GameManager.Instance.OnEnterLevel += OnEnterLevel_LoadArchive; }

	// ------------------ //   
	// --- 私有成员    
	// ------------------ //
	private const string ArchiveName = "Ach";

	// ------------------ //    
	// --- Unity消息    
	// ------------------ //

	// ------------------ //    
	// --- 公有方法   
	// ------------------ //
	public void SaveLevelArchive()
	{
		//foreach (var item in PartManager.Instance.AllPartCtrls)
		//{
		//	item.UpdateDataFromAccesstor();
		//}
		Debug.Log($"正在存档 - 存档零件 {PlayerPartManager.Instance.AllPlayerPartCtrls.Count} 个");
		Archive archive = new Archive();
		PlayerPartManager.Instance.SaveDataToArchive(archive);
		SensorManager.Instance.SaveDataToArchive(archive);
		PartConnectionManager.Instance.SaveDataToArchive(archive);
		ModelMapManager.Instance.SaveDataToArchive(archive);
		//archive.playerPos = GameManager.Instance.pla
		ES3.Save(CombinArchiveName(ArchiveName), archive);
		return;
    }

	public static string CombinArchiveName(string chapterName, string levelName, string name)
	{
		return $"[{chapterName}][{levelName}][{name}]";
	}

	public static bool HasArchive(string chapterName, string levelName)
	{
		return ES3.KeyExists(CombinArchiveName(chapterName, levelName, ArchiveName));
	}


	// ------------------ //   
	// --- 私有方法
	// ------------------ //
	private static string CombinArchiveName(string name)
	{
		return CombinArchiveName(GameManager.Instance.SelectedChapterName, GameManager.Instance.SelectedLevelName, name);
	} 

	private void OnEnterLevel_LoadArchive(string chapterName, string levelName)
	{
		PlayerPartManager.Instance.ResetData();
		SensorManager.Instance.ResetData();
		PartConnectionManager.Instance.ResetData();
		if (HasArchive(chapterName, levelName))
		{
			Debug.Log($"正在加载{chapterName + levelName}的存档");
			ApplyArchive(ES3.Load<Archive>(CombinArchiveName(chapterName, levelName, ArchiveName)));
			
			//foreach (var item in PartManager.Instance.AllPartCtrls)
			//{
			//	Debug.LogError($"ID {item.GetHashCode()}");
			//}
			//foreach (var item in PartConnectionManager.Instance.AllConnection)
			//{
			//	Debug.LogError($"conn {item.MasterPart.GetHashCode()} - {item.TargetPart.GetHashCode()}");
			//}
		}
	}

	private void ApplyArchive(Archive archive)
	{
		// 应用存档
		PlayerPartManager.Instance.LoadFromArchive(archive);
		SensorManager.Instance.LoadFromArchive(archive);
		PartConnectionManager.Instance.LoadFromArchive(archive);
		ModelMapManager.Instance.LoadFromArchive(archive);
	}

	// ------------------ //   
	// --- 类型
	// ------------------ //
	[Serializable]
	public class Archive
	{
		public List<PartCtrlCoreData> partList;
		public List<PartConnectionCoreData> partConnectionsArchive;

		public Dictionary<int, (List<Node>, List<SymbolConnectArchiveData>)> partModelingMapsArchive;

		public List<(int, int, bool)> partCollisionArchive;

		public SensorManager sensorMgrArchive;
		public SymbolConnector symbolConnectMgrArchive;
		public float[] playerPos;

		public Archive() { }
	}

	public enum E_GOArchive
	{
		PartParent, ModelParent
	}

	/// <summary>
	/// 由于引用关系在ES3里不能保存
	/// 使用特殊的数据结构来保存连接
	/// 这时一个node到另外一个node的一个连接
	/// </summary>
	[Serializable]
	public struct SymbolConnectArchiveData
	{
		public int outputIndex;
		public int outputIO;
		public int inputIndex;
		public int inputIO;
	}
}
