using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

/// <summary>
/// 零件高亮的逻辑放置在这里
/// 
/// </summary>
public class PartColorManager
{
	// ----------------//
	// --- 序列化
	// ----------------//

	// ----------------//
	// --- 公有成员
	// ----------------//
	public static PartColorManager Instance = new PartColorManager();

	// ----------------//
	// --- 私有成员
	// ----------------//
	private readonly Color EditMainColor = new Color(0.286f, 0.636f, 1f);
	private readonly Color ConnectableColor = new Color(0.2f, 0.8f, 0.2f);
	private readonly Color UnconnectColor = new Color(0.1f, 0.1f, 0.9f);
	private readonly Color ConnectingTargetColor = new Color(1, 1, 1);

	private readonly Color Draging_DragingPart = new Color(0.1f, 0.1f, 0.9f);
	private readonly Color Draging_TouchedPart = new Color(0.88f, 0.90f, 0.03f);
	private readonly Color Draging_OverlapedColor = new Color(0.89f, 0.1f, 0.1f);
	private readonly Color Draging_UnoverlapedColor = new Color(0.1f, 0.88f, 0.1f);

	// ----------------//
	// --- Unity消息
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	public void SetMaterial_EditMainPart(IPartSetShader partShaderCtrl)
	{
		partShaderCtrl.SetOutLine(EditMainColor);
		partShaderCtrl.SetActiveTex(true);
	}

	public void SetMaterial_MainPart(IPartSetShader partShaderCtrl)
	{
		partShaderCtrl.SetOutLine(EditMainColor);
		partShaderCtrl.SetActiveTex(true);
	}

	public void SetMaterial_Drag_DragedTargetPart(IPartSetShader partSetShader)
	{
		partSetShader.SetOutLine(EditMainColor);
		partSetShader.SetActiveTex(false);
	}

	public void SetMaterial_Drag_DragingPart(IPartSetShader partSetShader)
	{
		partSetShader.SetOutLine(Draging_DragingPart);
		partSetShader.SetActiveTex(false);
	}

	public void SetMaterial_Drag_Unoverlaped(IPartSetShader partSetShader)
	{
		partSetShader.SetOutLine(Draging_UnoverlapedColor);
		partSetShader.SetActiveTex(false);
	}
	public void SetMaterial_Drag_Touched(IPartSetShader partSetShader)
	{
		partSetShader.SetOutLine(Draging_TouchedPart);
		partSetShader.SetActiveTex(false);
	}

	public void SetMaterial_Drag_Overlaped(IPartSetShader partSetShader)
	{
		partSetShader.SetOutLine(Draging_OverlapedColor);
		partSetShader.SetActiveTex(false);
	}

	public void SetMaterial_Connect_MainPart(IPartSetShader partSetShader)
	{ 
		partSetShader.SetOutLine(EditMainColor);
		partSetShader.SetActiveTex(false);
	}

	public void Setmaterial_Connect_TargetPart(IPartSetShader partShaderCtrl)
	{
		partShaderCtrl.SetOutLine(ConnectableColor);
		partShaderCtrl.SetActiveTex(false);
	}

	public void SetMaterial_Connect_ConnectingPart(IPartSetShader partShaderCtrl)
	{
		partShaderCtrl.SetOutLine(ConnectingTargetColor);
		partShaderCtrl.SetActiveTex(true);
	}

	public void SetMaterial_Connect_UnconnectablePart(IPartSetShader partShaderCtrl)
	{
		partShaderCtrl.SetOutLine(UnconnectColor);
		partShaderCtrl.SetActiveTex(false);
	}

	public void SetMaterial_AsNormal(IPartSetShader partShaderCtrl)
	{
		partShaderCtrl.SetActiveOutLine(false);
		partShaderCtrl.SetActiveTex(true);
	}
	
	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//

}
