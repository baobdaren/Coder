using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class NiceButton : Button
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public ButtonClickedEvent OnRightClick = new ButtonClickedEvent();
	public ButtonClickedEvent OnLeftClick = new ButtonClickedEvent();

	// ----------------//
	// --- 私有成员
	// ----------------//
	private TextMeshProUGUI _tex { get => GetComponentInChildren<TextMeshProUGUI>(true); }
	public string Text
	{
		get => GetComponentInChildren<TextMeshProUGUI>(true).text;
		set => GetComponentInChildren<TextMeshProUGUI>(true).text = value;
	}
	[OnValueChanged("Editor_TexColorChanged")]
	public Color _texNormalColor;
	public Color _texHighColor;
	public Color _texPressColor;

	// ----------------//
	// --- Unity消息
	// ----------------//

	[ContextMenu("使用粘贴板颜色")]
	private void UsePastPanleColor()
	{
		string copyColor = GUIUtility.systemCopyBuffer;
		if (copyColor == null || copyColor == string.Empty)
		{
			Debug.LogError("粘贴板为空");
			return;
		}
		try
		{
			float r = int.Parse(copyColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
			float g = int.Parse(copyColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
			float b = int.Parse(copyColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
			Color col = new Color(r, g, b);
			Color.RGBToHSV(col, out float h, out float s, out float v);
			this.colors = new ColorBlock()
			{
				normalColor = new Color(r, g, b, 1),
				highlightedColor = Color.HSVToRGB(h, 1, v),
				pressedColor = Color.HSVToRGB(h, (1 + s) / 2, v),
				selectedColor = new Color(0, 0, 0, 0),
				colorMultiplier = 1
			};
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
		}
		catch (System.Exception ex)
		{
			Debug.LogError(copyColor + ":" + ex);
			throw;
		}
	}

	protected override void Start()
	{
		SetTexColor(TexState.Normal);
	}

	// ----------------//
	// --- 公有方法
	// ----------------//
	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			OnRightClick?.Invoke();
		}
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			OnLeftClick?.Invoke();
		}
		base.OnDeselect(eventData);
		SetTexColor( TexState.Normal);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		base.OnDeselect(eventData);
		SetTexColor( TexState.Normal);
	}

	//public override void OnMove(AxisEventData eventData)
	//{
	//	base.OnMove(eventData);
	//       Debug.LogError("OnMove");
	//}
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		SetTexColor(TexState.High);
	}
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		//SetTexColor(colors.pressedColor);
		SetTexColor(TexState.Press);
	}

	//public override void OnDeselect(BaseEventData eventData)
	//{
	//	base.OnDeselect(eventData);
	//       Debug.LogError("OnDeselect");
	//   }

	//public override void OnSelect(BaseEventData eventData)
	//{
	//	base.OnSelect(eventData);
	//       Debug.LogError("OnSelect");

	//   }

	private void SetTexColor(TexState state)
	{
		if (_tex == null)
		{
			return;
		}
		switch (state)
		{
			case TexState.Normal:
				_tex.color = _texNormalColor;
				break;
			case TexState.High:
				_tex.color = _texHighColor;
				break;
			case TexState.Press:
				_tex.color = _texPressColor;
				break;
		}
	}

	private enum TexState
	{
		Normal, High, Press
	}

	// ----------------//
	// --- 私有方法
	// ----------------//
	public void Editor_TexColorChanged()
	{
		if (GetComponentInChildren<TextMeshProUGUI>())
		{
			GetComponentInChildren<TextMeshProUGUI>().color = _texNormalColor;
		}
	}
}
