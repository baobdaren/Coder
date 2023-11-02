using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 编辑状态下的光标对象
/// </summary>
public class EditingBearing : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
	// ----------------//
	// --- 序列化
	// ----------------//
    [SerializeField]
    private GameObject IconDeleteBearing;
    [SerializeField]
    private GameObject IconConnectToBearing;
    [SerializeField]
    private SpriteRenderer DisplayIcon;
    // ----------------//
    // --- 公有成员
    // ----------------//
    private bool _connectable = false;
    private bool _deleteable = false;
	// ----------------//
	// --- 私有成员
	// ----------------//

	// ----------------//
	// --- Unity消息
	// ----------------//
	private void Awake()
	{
  //      GetComponent<SpriteRenderer>().sortingLayerID = RenderLayerManager.Instance.GetEditBearingIndex;
  //      int addOrder = 1;
		//foreach (var item in GetComponentsInChildren<SpriteRenderer>())
		//{
  //          item.sortingLayerID = RenderLayerManager.Instance.GetEditBearingIndex;
  //          item.sortingOrder = addOrder++;
		//}
    }

	void OnEnable()
    {
        IconDeleteBearing.SetActive(false);
        IconConnectToBearing.SetActive(false);
    }

    // ----------------//
    // --- 公有方法
    // ----------------//
    /// <summary>
    /// 轴承的显示状态
    /// 轴承的隐藏与否由父物体管理
    /// </summary>
    /// <param name="deleteable"></param>
    /// <param name="connectable"></param>
    public void SetDisplay(bool deleteable, bool connectable)
    {
        _connectable = connectable;
        _deleteable = deleteable;
        if (_connectable)
        {
            DisplayIcon.color = ColorConfig.Instance.BearingConnectableColor * 0.88f;
        }
        else if (_deleteable)
        {
            DisplayIcon.color = ColorConfig.Instance.BearingDeleteableColor * 0.88f;
        }
		else
		{
            DisplayIcon.color = ColorConfig.Instance.BearingDisableColor;
		}
        foreach (var item in GetComponentsInChildren<Rigidbody2D>())
        {
            item.simulated = true;
            item.bodyType = RigidbodyType2D.Static;
        }
        GetComponent<Collider2D>().isTrigger = true;
        // 暂时没有对编辑状态的轴承设置SortingLayer，暂时设置在所有零件之上显示
        // 但这会导致一个问题，对于下层和中层的轴承，会显示在上层零件之上
        // 以后可能需要设计高亮轴承所连接的两个零件
        //gameObject.GetComponent<SpriteRenderer>().sortingLayerID = GameLayerManager.Instance.EditBearingSortingLayer;
        //DeleteIcon.GetComponent<SpriteRenderer>().sortingLayerID = GameLayerManager.Instance.EditBearingSortingLayer;
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
        if (_connectable && _deleteable)
        {
            DisplayIcon.color = ColorConfig.Instance.BearingConnectableColor;
        }
        else if(_deleteable)
        {
            DisplayIcon.color = ColorConfig.Instance.BearingDeleteableColor;
        }
	    IconConnectToBearing.SetActive(_connectable);
		IconDeleteBearing.SetActive(_deleteable);
    }

	public void OnPointerExit(PointerEventData eventData)
	{
		IconDeleteBearing.SetActive(false);
		IconConnectToBearing.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
	{
        //Debug.Assert(ModelEdit.Instance.GetConnectMain != null);
        if (_connectable && eventData.button == PointerEventData.InputButton.Left) // 左键点击链接
        {
            var part = ModelScenePart.Instance.EditingScenePart ?? ModelEdit.Instance.GetConnectMain;
            Debug.Assert(part != null);
            bool result = PartConnectionManager.Instance.TryConnectPartToBearing(part, this);
			if (result)
			{
                IconConnectToBearing.SetActive(false);
			}
            if (result == false)
            {
                GameConsole.AddConsoleMessage("链接失败");
            }
        }
        else if (_deleteable && eventData.button == PointerEventData.InputButton.Right) // 右键点击删除
        { 
		    PartConnectionManager.Instance.DeleteConnection(this);
        }
    }

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
