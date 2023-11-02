using System.Collections.Generic;
using UnityEngine;

public class ModelSimulate:ResetAbleInstance<ModelSimulate>
{
    // ----------- //
    // --- 私有属性
    // ----------- //
    // 克隆后用来模拟物理的物体和设计所用的物体互相查找
    private readonly Dictionary<PlayerPartBase, PlayerPartBase> CloneToCreated = new Dictionary<PlayerPartBase, PlayerPartBase>();
    private readonly Dictionary<PlayerPartBase, PlayerPartBase> CreatedToClone = new Dictionary<PlayerPartBase, PlayerPartBase>();
    /// <summary>
    /// key1=modelMap
    /// key2=editpart
    /// value=clonepart
    /// </summary>
    private readonly DoubleKeyDict<PartModelingNodeMap, PlayerPartBase, PlayerPartBase> CloneDict = new DoubleKeyDict<PartModelingNodeMap, PlayerPartBase, PlayerPartBase>();
    //private GameObject _parentOfClones;
    //private GameObject _parentOfConnections;

    // ----------- //
    // --- 公有属性
    // ----------- //
    public CommandDataCache CommandDataCache = new CommandDataCache();
   // public GameObject ParentOfPhysicsParts
   // {
   //     get
   //     {
			//if(_parentOfClones == null)
			//{
   //             _parentOfClones = new GameObject(" Physics Parts *");
   //             _parentOfClones.SetActive(false);
			//}
   //         return _parentOfClones;
   //     }
   // }

   // public GameObject ParentOfPhysicsConnection
   // {
   //     get
   //     {
   //         if (_parentOfConnections == null)
   //         {
   //             _parentOfConnections = new GameObject("-- Physics Connections --");
   //             _parentOfConnections.SetActive(false);
   //         }
   //         return _parentOfConnections;
   //     }
   // }

    /// <summary>
    /// 物理零件父物体存在且激活
    /// </summary>
    public bool IsSimulating { get; set; }

    // ----------- //
    // --- 私有方法
    // ----------- //

    
    
    // ----------- //
    // --- 公有方法
    // ----------- //
    public ModelSimulate () {  }

    public void ClearCloneDic ( )
    {
        CloneToCreated.Clear();
        CreatedToClone.Clear();
    }

    public void RegistClone ( PlayerPartBase clone, PlayerPartBase created )
    {
        CloneToCreated.Add(clone, created);
        CreatedToClone.Add(created, clone);
    }

    public T GetClonePart<T> ( PlayerPartBase created ) where T :PlayerPartBase
    {
        return CreatedToClone[created] as T;
    }
    public T GetCreatedPart<T> ( PlayerPartBase clone ) where T:PlayerPartBase
    {
        return CloneToCreated[clone] as T;
    }
}

public class CommandDataCache
{
    public void Clear() 
    {
		foreach (var item in GO2Num)
		{
            GameObject.DestroyImmediate(item.Key);
		}
        GO2Num.Clear(); 
        Node2GO.Clear(); 
    }
    private Dictionary<GameObject, float> GO2Num = new Dictionary<GameObject, float>();
    private Dictionary<Node, GameObject> Node2GO = new Dictionary<Node, GameObject>();

    public void Add(GameObject goKey, Node nodeKey) 
    {
        GO2Num.Add(goKey, 0);
        Node2GO.Add(nodeKey, goKey);
    }
    public float this[GameObject goKey]
    { set => GO2Num[goKey] = value; }
    public float this[Node node]
    { get => GO2Num[Node2GO[node]];}
}


