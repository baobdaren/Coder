using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MaterialSetter
{
	public const string Part_HueKey = "HueOffset";
	public const string Part_UseOutlineKey = "UseOutline";
	public const string Part_UseTexKey = "UseTex";
	public const string Part_UseLitKey = "UseLit";
	public const string Part_OutlineColor = "OutlineColor";
	// ----------------//
	// --- 序列化
	// ----------------//


	// ----------------//
	// --- 公有成员
	// ----------------//

	// ----------------//
	// --- 私有成员
	// ----------------//

	// ----------------//
	// --- 公有方法
	// ----------------//
	public static void SetPart(Renderer sp, bool enableOutLine, bool enableTex, Color? outlineColor = null)
	{
		SetPartSettings(sp, enableOutLine, enableTex, outlineColor);
	}

	public static void SetPartHue(Renderer sp, float hue)
	{
		SetPartSettings(sp, hue: hue);
	}

	public static void SetPart_Physics(Renderer renderer, float hue)
	{
		SetPartSettings(renderer, false, true, hue: hue);
	}
	// ----------------//
	// --- 私有方法
	// ----------------//
	private MaterialSetter() { }

	/// <summary>
	/// 设置渲染该Render的方式
	/// </summary>
	/// <param name="render"></param>
	/// <param name="enableOutLine"></param>
	/// <param name="enableTex"></param>
	/// <param name="outlineColor"></param>
	/// <param name="hue"></param>
	public static void SetPartSettings(Renderer render, bool? enableOutLine = null, bool? enableTex = null, Color? outlineColor = null, float? hue = null)
	{
		// 双击零件没有调用shader修改
		//Debug.Log($"设置零件 Material 描边颜色{outlineColor} 描边{enableOutLine} 纹理{enableTex}");
		MaterialPropertyBlock _block = new MaterialPropertyBlock();
		if (render.HasPropertyBlock())
		{
			render.GetPropertyBlock(_block);
		}
		if (outlineColor.HasValue)
		{
			_block.SetColor(Part_OutlineColor, outlineColor.Value);
		}
		if (enableOutLine.HasValue)
		{
			_block.SetFloat(Part_UseOutlineKey, enableOutLine.Value ? 1 : 0);
		}
		if (enableTex.HasValue)
		{
			_block.SetFloat(Part_UseTexKey, enableTex.Value ? 1 : 0);
		}

		if (hue.HasValue)
		{
			_block.SetFloat(Part_HueKey, hue.HasValue ? 1 : 0);
		}
		render.SetPropertyBlock(_block);
		//sp.gameObject.SetActive(false);
		//sp.gameObject.SetActive(true);
	}

	// ----------------//
	// --- 类型
	// ----------------//
	public enum PartMaterialType
	{
		TEX,
		OUTLINE_TEX,
		OUTLINE_GREEN,
		OUTLINE_BLUE,
		OUTLINE_TEX_BLUE,
		OUTLINE_RED,
	}
}
