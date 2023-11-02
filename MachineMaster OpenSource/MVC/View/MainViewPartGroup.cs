using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;

/// <summary>
/// 通过零件层级，来控制显示
/// </summary>
public class MainViewPartGroup : BaseView
{
    // ------------- //
    // -- 序列化
    // ------------- //
    [SerializeField]
    GameObject toggleOrigin;
    [SerializeField]
    [ChildGameObjectsOnly]
    NiceButton ColorBtnOrigin;
    [SerializeField]
    [ChildGameObjectsOnly]
    RectTransform PartGroup;
    [SerializeField]
    [ChildGameObjectsOnly]
    GroupItem PartGroupItem;


	// ------------- //
	// --  私有属性
	// ------------- //
	private List<Toggle> _toggls = new List<Toggle>();


    // ------------- //
    // --  公有属性
    // ------------- //
	public event Action<float> OnSelectColor;

	// ------------- //
	// --  Unity消息
	// ------------- //

	void Start()
    {
  //      toggleOrigin.SetActive(true);
  //      toggleOrigin.GetComponent<Toggle>().isOn = true;
		//while (toggleOrigin.transform.parent.childCount < GameLayerManager.PartLayerCount)
		//{
  //          GameObject.Instantiate(toggleOrigin, toggleOrigin.transform.parent);
  //      }
  //      Debug.LogError("Start 层级过滤");
  //      for (int i = 0; i <  GameLayerManager.PartLayerCount; i++)
  //      {
  //          int layerIndex = i + GameLayerManager.DefaultPartLayer;
  //          GameObject toggle = toggleOrigin.transform.parent.GetChild(i).gameObject;
  //          //toggle.transform.Find("Background").GetComponent<Image>().color = GameLayerManager.Instance.GetPartColor(layerIndex);
  //          toggle.GetComponent<Toggle>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((bool isOn) => 
  //          { 
  //              OnToggleValueChanged_Layer(isOn, layerIndex); 
  //          }));
  //          // toggleGroup.RegisterToggle(toggleObject.GetComponent<Toggle>());
  //          _toggls.Add(toggle.GetComponent<Toggle>());
  //      }
    }


    // ------------- //
    // -- 私有方法
    // ------------- //
    private void OnToggleValueChanged_Layer(bool isOn, int layer)
    {
        foreach (PlayerPartCtrl item in PlayerPartManager.Instance.AllPlayerPartCtrls)
        {
			if (item.Layer == layer)
			{
                item.MyEditPartAccesstor.gameObject.SetActive(isOn);
			}
        }
		// 层级修改后刷新连接状态
		if (UIManager.Instance.PanelEdit.IsDisplaying)
		{
            UIManager.Instance.PanelEdit.ConnectView.ForceUpdate_ConnectCursor();
		}
    }

	// ------------- //
	// -- 公有方法
	// ------------- //
	public void TogglesReset ( )
    {
        foreach(var item in _toggls)
        {
            item.isOn = true;
        }
    }

	public override bool ExitCondition(BaseView willOpenView)
	{
		if (willOpenView.IsView<MainViewEdit>()|| willOpenView.IsView<MainViewStart>())
		{
			return false;
		}
		return true;
	}

	public override void ExitView()
	{
		base.ExitView();
		for (int i = 0; i < _toggls.Count; i++)
		{
            _toggls[i].isOn = true;
		}
	}
}
