using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : SerializedMonoBehaviour
{
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
	// --- Unity消息
	// ----------------//
	private void Awake()
	{
		GetComponentInChildren<Camera>(true).gameObject.SetActive(false);
	}

	private void OnBecameVisible()
	{
		Debug.Log(name + "可见");
		GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
		GetComponentInChildren<SpriteRenderer>(true).enabled = true;
	}

	private void OnBecameInvisible()
	{
		Debug.Log(name + "不可见");
		GetComponentInChildren<Camera>(true).gameObject.SetActive(false);
		GetComponentInChildren<SpriteRenderer>(true).enabled = false;
	}
	// ----------------//
	// --- 公有方法
	// ----------------//


	// ----------------//
	// --- 私有方法
	// ----------------//
}
