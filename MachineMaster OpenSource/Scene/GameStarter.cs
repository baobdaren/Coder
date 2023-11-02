using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[SerializeField]
	Image _bgImage;
	[SerializeField]
	Button _buttonStartGame;
	[SerializeField]
	Button _buttonExitGame;
	[SerializeField]
	Slider _loadProgressSlider;
	[SerializeField]
	TextMeshProUGUI _progressText;
	[SerializeField]
	TextMeshProUGUI _debugText;

	// ----------------//
	// --- 公有成员
	// ----------------//

	// ----------------//
	// --- 私有成员
	// ----------------//


	// ----------------//
	// --- Unity消息
	// ----------------//
	private void Awake()
	{
		float startTime = Time.realtimeSinceStartup;
		var loadedObject = Resources.LoadAll("DataFile/SingleConfig/");
		foreach (var item in loadedObject)
		{
			(item as UniqueConfigBase).InitInstance();
			Debug.LogWarning("加载SO文件 - " +  item.GetType().ToString());
		}
		Debug.LogWarning("结束加载SO文件" + (Time.realtimeSinceStartup - startTime)/1000 + "ms");
		// 创建相机和事件系统
		//GameManager.Instance.CreateFirstGameObject();
		_loadProgressSlider.gameObject.SetActive(false);
		Screen.sleepTimeout = 0;
		_progressText.gameObject.SetActive(false);
		_bgImage?.gameObject.SetActive(true);
		//GetComponent<Canvas>().worldCamera = CameraActor.Instance.MainCamera;
		Application.targetFrameRate = 60;
	}
	private void Start()
	{
		_buttonStartGame.onClick.AddListener(On_ClickStartGame);
		_loadProgressSlider.handleRect.gameObject.GetComponent<MaskableGraphic>().raycastTarget = false;
		_buttonExitGame.onClick.AddListener(() =>
		{
			Application.Quit();
		}); 
	}

	//private void Update()
	//{
	//	//_debugText.SetText(Time.time.ToString());
	//	if (Input.GetKeyDown(KeyCode.F))
	//	{
	//		Debug.LogError("Resources Load Game");
	//		Resources.Load("Game");
	//		//GameManager.Instance.CreateFirstGameObject();
	//		GetComponent<Canvas>().worldCamera = CameraActor.Instance.MainCamera;
	//	}
	//}

	// ----------------//
	// --- 公有方法
	// ----------------//

	// ----------------//
	// --- 私有方法
	// ----------------//
	private void On_ClickStartGame()
	{
		_buttonStartGame.interactable = false;
		//AsyncOperation operation = SceneManager.LoadSceneAsync(MainSceneManager.Instance.GetMainSceneIndex(MainSceneManager.ENUM_SCENE.SELECT), LoadSceneMode.Single);
		//StartCoroutine(LoadSceneProgressUpdate(operation));
		GameManager.Instance.EnterLevelSelectScene();
	}

	private IEnumerator LoadSceneProgressUpdate(AsyncOperation op)
	{
		_loadProgressSlider.gameObject.SetActive(true);
		_progressText.gameObject.SetActive(true);
		_loadProgressSlider.value = 0;
		op.allowSceneActivation = false;
		float displayValue = 0;
		while (displayValue < 1)
		{
			SetBitProgress(displayValue);
			yield return new WaitForEndOfFrame();
			displayValue += 0.01f;
		}
		yield return new WaitForSeconds(0.5f);
		SetBitProgress(1);
		yield return new WaitForEndOfFrame();
		op.allowSceneActivation = true;
		_loadProgressSlider.gameObject.SetActive(false);
		_progressText.gameObject.SetActive(false);
	}
	private void SetBitProgress(float value)
	{
		const int max = 255;
		const float pauseValue = 0.9f;
		string maxStr = "/" + Convert.ToString(max, 2);
		if (value == 1)
		{
			_loadProgressSlider.value = 1;
		}
		else
		{
			_loadProgressSlider.value = Mathf.Clamp(value / pauseValue, 0, 0.999999f);
		}
		//_progressText.text = _loadProgressSlider.value.ToString();
		_progressText.text = Convert.ToString((int)(_loadProgressSlider.value * max), 2) + maxStr;
	}

}
