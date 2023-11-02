using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

/// <summary>
/// 零件的核心数据
/// </summary>
[ES3Serializable]
public class PartCtrlCoreData
{
	public PartCtrlCoreData(PartTypes partType) 
	{
		MyPartType = partType; 
		Size = PartConfig.Instance.GetPartSizeSliderSetting(MyPartType).Item1;
		SectionDataList = new List<(Vector3, Quaternion)>();
	}
	/// <summary>
	/// 持久化使用的无参构造函数
	/// </summary>
	public PartCtrlCoreData() { }

	public PartTypes MyPartType;
	public float Size;
	public int ColorOffset;
	public Vector3 Position;
	public Quaternion Rotation;
	public List<(Vector3, Quaternion)> SectionDataList;
}

[Serializable]
public enum PartTypes
{
	JETEngine, Engine, Gear, Presser, Rail, Rope, Spring, Steel, Wheel, AVSensor, DISSensor, ScecneCustom
}