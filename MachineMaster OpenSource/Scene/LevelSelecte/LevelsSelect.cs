using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 关卡选择界面
/// </summary>
public class LevelsSelect : MonoBehaviour
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[SerializeField]
	GameObject _chapterSelectView;
	[SerializeField]
	GameObject _levelsSelectView;
	[SerializeField]
	Button _exitButton;

	[SerializeField]
	GameObject _originChapterObject;
	[SerializeField]
	GameObject _originLevelObject;
	[SerializeField]
	RawImage _selectedChapterImage;

	// ----------------//
	// --- 公有成员
	// ----------------//
	public static string SelectedChapterName { private set; get; }


	// ----------------//
	// --- 私有成员
	// ----------------//
	private Dictionary<GameObject, string[]> _createdLevelsItemToArchiveID = new Dictionary<GameObject, string[]>();

	// ----------------//
	// --- Unity消息
	// ----------------//
	protected void Awake()
	{
		Application.quitting += () =>
		{
			AssetBundle.UnloadAllAssetBundles(true);
		};
		_exitButton.onClick.AddListener(OnClick_ExitButton);
		_originChapterObject.gameObject.SetActive(false);
		_originLevelObject.gameObject.SetActive(false);
	}

	private void Start()
	{
		CreateChaptersMenu();
		if (!string.IsNullOrEmpty( GameManager.Instance.SelectedChapterName))
		{
			OnClick_ChapterItem(GameManager.Instance.SelectedChapterName);
		}
		if (!string.IsNullOrEmpty(GameManager.Instance.SelectedLevelName))
		{
			Debug.Log("这里应当自动聚焦到" + GameManager.Instance.SelectedLevelName);
		}
	}

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(SelectedChapterName))
		{
			UpdateLevelArchiveStatus();
		}
	}


	// ----------------//
	// --- 公有方法
	// ----------------//


	// ----------------//
	// --- 私有方法
	// ----------------//
	private int CreateChaptersMenu()
	{
		SetDisplay(true);
		int chapterAmount = 0;
		foreach (var item in ChapterBundleManager.Instance.GetAllChapters())
		{
			chapterAmount++;
			GameObject clone = GameObject.Instantiate(_originChapterObject, _originChapterObject.transform.parent);
			clone.GetComponentInChildren<RawImage>().texture = item.Value;
			clone.transform.GetComponentInChildren<Button>().onClick.AddListener(() =>
			{
				OnClick_ChapterItem(item.Key);
			});
			clone.SetActive(true);
		}
		return 0;
	}

	private void CreateLevelsMenu(string chapterName, List<string> levelNamesWithExtension)
	{
		SetDisplay(false);
		while (_createdLevelsItemToArchiveID.Count < levelNamesWithExtension.Count)
		{
			GameObject levelItemClone = Instantiate(_originLevelObject, _originLevelObject.transform.parent);
			_createdLevelsItemToArchiveID.Add(levelItemClone, new string[2]);
		}
		int levelNameIndex = 0;
		foreach (KeyValuePair<GameObject, string[]> item in _createdLevelsItemToArchiveID)
		{
			item.Key.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			if (levelNameIndex < levelNamesWithExtension.Count)
			{
				string levelName = Path.GetFileNameWithoutExtension(levelNamesWithExtension[levelNameIndex]);
				item.Key.SetActive(true);
				item.Key.GetComponentInChildren<TextMeshProUGUI>().SetText(levelName);
				_createdLevelsItemToArchiveID[item.Key][0] = chapterName;
				_createdLevelsItemToArchiveID[item.Key][1] = levelName;
				item.Key.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
				item.Key.GetComponentInChildren<Button>().onClick.AddListener(
				() =>
				{
					//Debug.LogError("点击触发" + GetInstanceID());
					OnClick_LevelItem(Path.GetFileNameWithoutExtension(levelName));
				});
				levelNameIndex++;
			}
			else
			{
				item.Key.SetActive(false);
			}
		}
	}

	/// <summary>
	/// 点击了某一个章节
	/// </summary>
	/// <param name="chapterBundlePath"></param>
	private void OnClick_ChapterItem(string chapterBundlePath)
	{
		GameManager.Instance.SelectedChapterName = chapterBundlePath;
		// 进入章节后加载章节Bundle 存档问题？
		SelectedChapterName = chapterBundlePath;
		_selectedChapterImage.GetComponent<RawImage>().texture = ChapterBundleManager.Instance.GetAllChapters()[chapterBundlePath];
		_selectedChapterImage.gameObject.SetActive(true);
		var levelsName = ChapterBundleManager.Instance.GetChapterLevelNames(chapterBundlePath);
		CreateLevelsMenu(SelectedChapterName, levelsName);
		UpdateLevelArchiveStatus();
	}

	private void OnClick_LevelItem(string levelName)
	{
		GameManager.Instance.SelectedLevelName = levelName;
		if (GameManager.Instance.IsPlayingLevel)
		{
			Debug.LogError("因IsPlaying返回呢");
			return;
		}
		GameManager.Instance.EnterLevel();
	}

	private void OnClick_ExitButton()
	{
		if (_levelsSelectView.gameObject.activeSelf)
		{
			_selectedChapterImage.GetComponent<RawImage>().texture = null;
			SetDisplay(true);
		}
		else
		{
			SceneManager.LoadScene(0, LoadSceneMode.Additive);
		}
		SelectedChapterName = null;
	}

	private void UpdateLevelArchiveStatus()
	{
		bool hasArchive;
		foreach (var item in _createdLevelsItemToArchiveID)
		{
			hasArchive = ArchiveManager.HasArchive(item.Value[0], item.Value[1]);
			item.Key.transform.Find("-OK").gameObject.SetActive(hasArchive);
			item.Key.transform.Find("-NO").gameObject.SetActive(!hasArchive);
		}
	}

	private void SetDisplay(bool displayChapterView)
	{
		_chapterSelectView.gameObject.SetActive(displayChapterView);
		_levelsSelectView.gameObject.SetActive(!displayChapterView);
	}
}
