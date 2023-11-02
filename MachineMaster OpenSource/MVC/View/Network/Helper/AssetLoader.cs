using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AssetLoader
{
	private static Dictionary<string, AssetBundle> LoadedBundle = new Dictionary<string, AssetBundle>();
	private static AssetBundleManifest BundleManifest
	{
		get
		{
			if(_manifest == null)
			{
				string path = Path.Combine(BundlePaths.PATH_BUNDLEROOT, "MechanicMaster");
				bool ok = File.Exists(path);
				var all = AssetBundle.LoadFromFile(path).LoadAllAssets();
				_manifest = all[0] as AssetBundleManifest;
			}
			return _manifest;
		}
	}
	private static AssetBundleManifest _manifest;

	public static AssetBundle LoadBundle ( string bundlePath, bool isRelativPath)
	{
		string path = isRelativPath ? (Path.Combine(BundlePaths.PATH_BUNDLEROOT, bundlePath)) : (bundlePath);
		path = path.Replace(@"\", "/");
		if(!Path.GetFileName(path).Contains(@"."))
		{
			path += ".unity3d";
		}
		try
		{
			if(!File.Exists(path))
			{
				return null;
			}
			if(!LoadedBundle.ContainsKey(path))
			{
				foreach(var item in AssetBundle.GetAllLoadedAssetBundles())
				{
					if(item.name == bundlePath)
					{
						Debug.LogError("违规加载的bundle-" + path);
						LoadedBundle.Add(path, item);
					}
				}
			}
			if(!LoadedBundle.ContainsKey(path))
			{
				AssetBundle loadedBundle = AssetBundle.LoadFromFile(path);
				if(loadedBundle == null)
				{
					Debug.LogError("Bundle加载失败-" + path);
				}
				else
				{
					LoadedBundle.Add(path, loadedBundle);
				}
			}
		}
		catch(System.Exception ex)
		{
			Debug.LogError(ex.Message + "\n使用资源 " + path);
		}
		return LoadedBundle[path];
	}


	#region 关卡配置
	private static List<string> ChapterBundlePaths
	{
		get
		{
			if(_chapterBundlePaths == null  || _chapterBundlePaths.Count == 0)
			{
				Debug.LogError("尚未实现压缩AssetBundle，测试阶段直接读取");
				_chapterBundlePaths = new List<string>();
				bool isEmpty = true;
				foreach(var chapterBundlePath in BundleManifest.GetAllAssetBundles())
				{
					// levels 文件夹下每一个bundle代表一个章节
					if(chapterBundlePath.StartsWith(GameConfig.Instance.ChapterBundlesFolderPath + "/"))
					{
						isEmpty = false;
						Debug.LogError("章节 -" + chapterBundlePath);
						_chapterBundlePaths.Add(chapterBundlePath);
					}
				}
				if (isEmpty)
				{
					Debug.LogError($"没有以{GameConfig.Instance.ChapterBundlesFolderPath}开头的章节资源");
				}
				else
				{
					Debug.LogError($"从Manifest找到章节 {_chapterBundlePaths.Count} 个");
				}
			}
			return _chapterBundlePaths;
		}
	}
	private static List<string> _chapterBundlePaths;
	/// <summary>
	/// 创建章节路径和章节主题纹理的字典
	/// 一个章节的所有场景在一个Bundle
	/// 所有章节的主题纹理都在一个文件夹下面
	/// </summary>
	/// <returns>返回一个字典（章节名称，章节界面纹理）</returns>
	public static Dictionary<string, Texture> LoadChapterTexs ( )
	{
#if LOAD_ASSETS_MODE_RES
		Dictionary<string, Texture> result = new Dictionary<string, Texture>();
		foreach (string chaptPath in Directory.EnumerateDirectories("Assets/Resources/Scenes/Chapters/"))
		{
			result.Add(chaptPath, Resources.Load<Texture>("Scenes/ChapterTexs/" + Path.GetFileName(chaptPath)));
			//foreach (var scenePath in Directory.EnumerateFiles(chaptPath))
			//{
			//	if (!scenePath.EndsWith(".meta"))
			//	{
			//		result.Add(scenePath, Resources.Load<Texture>("Scenes/ChapterTexs/" + Path.GetFileName(chaptPath)));
			//	}
			//}
		}
		return result;
#else
		AssetBundle chapterTexsBundle = LoadBundle(GameConfig.Instance.ChapterTexsFolderName, true);
		Dictionary<string, Texture> result = new Dictionary<string, Texture>();
		foreach (var item in chapterTexsBundle.GetAllAssetNames())
		{
			string fName = Path.GetFileNameWithoutExtension(item);
			result.Add(fName, chapterTexsBundle.LoadAsset<Texture>(fName));
		}
		return result;
#endif
	}
	#endregion
}