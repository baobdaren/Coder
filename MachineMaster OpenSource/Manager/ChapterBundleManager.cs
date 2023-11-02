using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChapterBundleManager
{
	// ------------------ //
	// --- 序列化
	// ------------------ //


	// ------------------ //
	// --- 公有成员
	// ------------------ //
	public static ChapterBundleManager Instance = new ChapterBundleManager();


	// ------------------ //
	// --- 私有成员
	// ------------------ //
	private Dictionary<string, Texture> _allChapters;


	// ------------------ //
	// --- Unity消息
	// ------------------ //


	// ------------------ //
	// --- 公有方法
	// ------------------ //
	public Dictionary<string, Texture> GetAllChapters()
	{
		return _allChapters;
	}

	/// <summary>
	/// 返回所给地址bundle里所有的关卡预制体名称
	/// </summary>
	/// <param name="chapterBundleName"></param>
	/// <returns></returns>
	public List<string> GetChapterLevelNames(string chapterBundleName)
	{
#if LOAD_ASSETS_MODE_RES
		Object[] sceneAssets = Resources.LoadAll<Object>(chapterBundleName.Replace("Assets/Resources/", ""));
		List<string> levelNames = new List<string>(sceneAssets.Length);
		foreach (var item in sceneAssets)
		{
			levelNames.Add(chapterBundleName + "/" + item.name);
		}
		return levelNames;
#else
		var chapterBundle = AssetLoader.LoadBundle(Path.Combine(GameConfig.Instance.ChapterBundlesFolderPath, chapterBundleName), true);
		List<string> result = new List<string>(chapterBundle.GetAllAssetNames().Length);
		foreach (var item in chapterBundle.GetAllScenePaths())
		{
			string fileName = Path.GetFileName(item);
			result.Add(fileName);
		}
		return result;
#endif
	}

	//public GameObject GetChapterLevelPrefab(string chapterName, string levelName)
	//{
	//	try
	//	{
	//		return AssetLoader.LoadBundle(chapterName, true).LoadAsset<GameObject>(levelName);
	//	}
	//	catch (System.Exception ex)
	//	{
	//		Debug.LogError(ex.Message);
	//		return null;
	//	}
	//}

	/// <summary>
	/// 有的关卡具有自己独特的零件预设
	/// </summary>
	/// <param name="chapterName"></param>
	/// <returns></returns>
	public PartPrefabConfig LoadPartPrefabConfig(string chapterName)
	{
		string configBundleName = "PartPrefabConfig";
		//return AssetLoader.LoadBundle(chapterName).LoadAsset<PartPrefabConfig>("PartPrefabConfig");
		var bundle = AssetLoader.LoadBundle(chapterName, true);
		if (bundle == null)
		{
			return null;
		}
		string[] bundleNames = bundle.GetAllAssetNames();
		for (int i = 0; i < bundleNames.Length; i++)
		{
			if (bundleNames[i] == configBundleName)
			{
				return bundle.LoadAsset<PartPrefabConfig>(configBundleName);
			}
		}
		return null;
	}

	// ------------------ //
	// --- 私有方法
	// ------------------ //
	private ChapterBundleManager()
	{
		if (_allChapters == null)
		{
			_allChapters = AssetLoader.LoadChapterTexs();
		}
	}
}
