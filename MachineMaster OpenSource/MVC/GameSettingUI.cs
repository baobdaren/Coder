using UnityEngine;
using UnityEngine.UI;

public class GameSettingUI : MonoBehaviour
{
	// ------------------ //    
	// --- 序列化    
	// ------------------ //
	[SerializeField]
	GameObject DisplayWindow;
	[SerializeField]
	Button IconButton;

    [SerializeField]
    Slider VolumeSlider;

    [SerializeField]
    Slider ScrollSpeedSlider;

	[SerializeField]
	Button ExitSettingButton;

	//[SerializeField]
	//Toggle VerticalHoldToggle;

	[SerializeField]
	Button ExitGameButton;
	// ------------------ //    
	// --- 公有成员    
	// ------------------ //

	// ------------------ //   
	// --- 私有成员    
	// ------------------ //

	// ------------------ //    
	// --- Unity消息    
	// ------------------ //
	protected void Awake()
	{
		DisplayWindow.SetActive(false);

		float tmpValue = GameManager.Instance.Volume;
		VolumeSlider.maxValue = 1; 
		VolumeSlider.minValue = 0;
		VolumeSlider.value = tmpValue;

		tmpValue = GameManager.Instance.MouseScrollSpeed;
		ScrollSpeedSlider.minValue = 0.01f;
		ScrollSpeedSlider.maxValue = 0.31f;
		ScrollSpeedSlider.value = tmpValue;

		//VerticalHoldToggle.isOn = GameManager.Instance.VerticalHold;
	}

	private void Start()
	{
		GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
		GetComponent<Canvas>().worldCamera = CameraActor.Instance.MainCamera;
		GetComponent<Canvas>().sortingLayerID = SortingLayer.NameToID("UI2");
		ExitGameButton.onClick.AddListener(On_ExitGameButtonClick);
	}

	private void OnEnable()
	{
		VolumeSlider.onValueChanged.AddListener(On_VolumeSlider);
		ScrollSpeedSlider.onValueChanged.AddListener(On_ScrollSpeed);
		ExitSettingButton.onClick.AddListener(On_HideSettingsCanvas);
		IconButton.onClick.AddListener(On_ClickIconButton);
		//VerticalHoldToggle.onValueChanged.AddListener(On_VerticalHoldToggleValueChanged);
	}

	private void OnDisable()
	{
		VolumeSlider.onValueChanged.RemoveAllListeners();
		ScrollSpeedSlider.onValueChanged.RemoveAllListeners();
		ExitSettingButton.onClick.RemoveAllListeners();
		IconButton.onClick.RemoveAllListeners();
	}

	// ------------------ //    
	// --- 公有方法   
	// ------------------ //    


	// ------------------ //   
	// --- 私有方法
	// ------------------ //

	private void On_VolumeSlider(float value)
    {
		GameManager.Instance.Volume = value;
	}

	private void On_ScrollSpeed(float value)
	{
		GameManager.Instance.MouseScrollSpeed = value;
	}

	private void On_HideSettingsCanvas()
	{
		DisplayWindow.SetActive(false);
	}

	private void On_ClickIconButton()
	{
		DisplayWindow.SetActive(!DisplayWindow.activeSelf);
		//(DisplayWindow.transform as RectTransform).localPosition = new Vector2(Screen.width / 2, Screen.height / 2);
	}

	//private void On_VerticalHoldToggleValueChanged(bool ison)
	//{
	//	Debug.Log("VScount" + QualitySettings.vSyncCount);
	//	GamePersistentData.Instance.VerticalHoldData = ison;
	//	Application.targetFrameRate = 120;
	//	QualitySettings.vSyncCount = (GamePersistentData.Instance.VerticalHoldData) ? 1 : 0;
	//}

	private void On_ExitGameButtonClick()
	{
		if (GameManager.Instance.IsPlayingLevel)
		{
			GameManager.Instance.On_LevelFinish(0);
		}
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			UnityEditor.EditorApplication.isPlaying = false;
		}
# else
		Application.Quit();
#endif
	}
}
