using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


/// <summary>
/// 管理游戏进程，管理游戏数据，跨场景功能
/// </summary>
public class GameManager/*:SaveableManager<GameManager>*/
{
    public static readonly GameManager Instance = new GameManager();
    // ------------------ //    
    // --- 序列化    
    // ------------------ //

    // ------------------ //    
    // --- 公有成员    
    // ------------------ //
    /// <summary>
    /// 进入关卡场景事件
    /// </summary>
    public event Action<string, string> OnEnterLevel;
    public bool IsPlayingLevel { get; private set; }
    public string SelectedChapterName { get; set; }
    public string SelectedLevelName { get; set; }
    public GameObject PlayingLevelPrefabClone { get; private set; }
    
    public GameObject SelectLevelUI { get; private set; }
    public float MouseScrollSpeed { get; set; }
    public bool VerticalHold { get; set; }
    public float Volume 
    {
        get
        {
            return _volume;
        }
        set
        {
            _volume = value;
            AudioListener.volume = _volume;
        }
    }
    //public Vector2 RenterLevelPos { get => LevelProgressBase.Instance. }
    // ------------------ //   
    // --- 私有成员    
    // ------------------ //
    /// <summary>
    /// 音量保存-程序退出时音量始终为1，所以独立保存一下，用于保存设置时使用
    /// </summary>
    private float _volume;

    // ------------------ //    
    // --- Unity消息    
    // ------------------ //

    // ------------------ //    
    // --- 公有方法   
    // ------------------ //
    public GameManager()
    {
        Volume = GamePersistentData.Instance.VolumeData;
        MouseScrollSpeed = GamePersistentData.Instance.ScrollerSpeedData;
        VerticalHold = GamePersistentData.Instance.VerticalHoldData;

        Application.quitting += Application_quitting;
    }

    /// <summary>
    /// 进入关卡场景
    /// </summary>
    public void EnterLevel()
    {
        EnterLevelScene(SelectedLevelName);
        //MoveFirstAssetToActiveScene(SceneManager.GetActiveScene());
        //RecordSelectLevelSceneInfo();
    }

    public void ReplayLevel()
    {
        //EnterLevelScene(SelectedLevelName, RenterLevelPos);
    }


	public void On_LevelFinish(int finishScore)
	{
        IsPlayingLevel = false;
		//Debug.LogError($"本关卡结束 {finishScore} 分");
        ArchiveManager.Instance.SaveLevelArchive();
        GamePersistentData.Instance.SaveLevelScore(SelectedChapterName, SelectedLevelName, finishScore);
		if (PlayingLevelPrefabClone != null)
		{
			GameObject.Destroy(PlayingLevelPrefabClone);
		}
        // level完成后，恢复关卡选择界面时自动进入这个章节
        EnterLevelSelectScene(SelectedChapterName, SelectedLevelName);
	}


	public void EnterLevelSelectScene(string focusToChapter = null, string focusToLevel = null)
    {
        SelectedChapterName = focusToChapter;
        SelectedLevelName = focusToLevel;
        // 当前场景为开始游戏场景或者关卡场景
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("LevelSelect");

        GameObject.Instantiate(GameConfig.Instance.ES3Mgr);
        SceneManager.UnloadSceneAsync(currentScene);
    }


    // ------------------ //   
    // --- 私有方法
    // ------------------ //


	private void Application_quitting()
	{
        //Debug.LogError("清空存档");
        //ES3.DeleteDirectory(@"C:\Users\GODBO\AppData\LocalLow\xiaobaostudio\Mechanic Master");
	}

    /// <summary>
    /// 创建场景内容
    /// </summary>
    /// <param name="firstEntry"> 为false时不销毁当前场景只是重建内容 即重试关卡</param>
    private void EnterLevelScene(string sceneName, Vector2? enterPos = null)
    {
        //Scene selectScene = SceneManager.GetActiveScene();
#if LOAD_ASSETS_MODE_RES
        sceneName = (SelectedChapterName + "/" + sceneName + ".unity");
        Debug.Log(sceneName);
#endif
        AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        IsPlayingLevel = true;
        loadSceneOp.completed += (AsyncOperation op) =>
        {
            //GameObject.Instantiate(GameConfig.Instance.LevelUI);
            OnEnterLevel?.Invoke(SelectedChapterName, SelectedLevelName);
			if (enterPos.HasValue)
			{
                LevelProgressBase.Instance.SetStartPos(enterPos.Value);
			}
            LevelProgressBase.Instance.OnLevelFinish.AddListener(On_LevelFinish);
        };
    }
}
