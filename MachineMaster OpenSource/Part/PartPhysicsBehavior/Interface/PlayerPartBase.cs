using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 抽象基类
/// 零件在物理模拟状态下的控制逻辑
/// </summary>
public abstract class PlayerPartBase:MonoBehaviour
{
    // ----------------- //
    // --  序列化
    // ----------------- //

    // ----------------- //
    // --  私有成员
    // ----------------- //
    private int _layer;
    public AbsPartAccessorBase Accessor { set; get; }

    // ----------------- //
    // --  公有成员
    // ----------------- //
    public PlayerPartCtrl MyCtrlData { set; get; } 

    public int Layer
    {
        set 
        {
            _layer = value;
			foreach (var item in gameObject.GetComponentsInChildren<Transform>())
			{
                item.gameObject.layer = _layer;
			}
        }
        get { return _layer; }
    }

	// ----------- //
	// -- Unity消息
	// ----------- //

    // ----------- //
    // -- 私有方法
    // ----------- //
    //private GameObject CreateScrewGameObject(Vector3 worldAnchor)
    //{
    //    GameObject screw = GameObject.Instantiate(PartConfig.Instance.PhysicsScrewrefab);
    //    if (screw)
    //    {
    //        screw.transform.SetParent(ObjParentsManager.Instance.ParentOfCreatedPart.transform);
    //        screw.transform.position = worldAnchor;
    //        screw.SetActive(false);
    //    }
    //    return screw;
    //}


    // ----------- //
    // -- 公有方法 
    // ----------- //


    // ----------- //
    // -- 类型
    // ----------- //
    //public struct PartConnectionObject
    //{
    //    public PartConnectionObject(GameObject screwObject, PartBase jointAttatch, PartBase jointConnect)
    //    {
    //        ScrewObject = screwObject;
    //        ConnectBaseJointAttacthed = jointAttatch;
    //        ConnectBaseJointConnectTo = jointConnect;
    //    }
    //    public GameObject ScrewObject;
    //    public PartBase ConnectBaseJointAttacthed;
    //    public PartBase ConnectBaseJointConnectTo;
    //}
}

public enum PartWorkState
{
    /// <summary>
    /// Main界面时正常显示状态
    /// </summary>
    Normal, // 基本状态
    /// <summary>
    /// 编辑状态
    /// </summary>
    TmpEditMaster,
    /// <summary>
    /// 链接时作为主物体状态
    /// </summary>
    TmpConnectingMaster,
    /// <summary>
    /// 铰接时可以作为铰接对象的状态
    /// </summary>
    TmpConnecting_SelectedTarget,
    /// <summary>
    /// 铰接时尚未作为铰接对象的状态
    /// </summary>
    TmpConnectingTarget_Connectable, 
    /// <summary>
    /// 铰接时被过滤的零件状态
    /// </summary>
    TmpConnectingTarget_UnConnectable,
    /// <summary>
    /// 弹出，即当前状态弹出
    /// </summary>
    Cancle,
    /// <summary>
    /// 空，特殊用途
    /// </summary>
    None // 用于不切换时传递
}   