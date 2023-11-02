using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Collider2D))]
public class TargetTrigger : MonoBehaviour
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[SerializeField]
	private bool _triggeOnce = true;

	[SerializeField]
	private GameObject TriggerTarget;
	// ----------------//
	// --- 公有成员
	// ----------------//
	[HideInInspector]
	public readonly UnityEvent OnPlayerEnter = new UnityEvent();
	[HideInInspector]
	public readonly UnityEvent OnPlayerExit = new UnityEvent();

	public bool TargetStayIn { get; private set; } = false;
	// ----------------//
	// --- 私有成员
	// ----------------//
	[ReadOnly]
	bool Trigged = false;
	[ReadOnly]
	// ----------------//
	// --- Unity消息
	// ----------------//
	private void Awake()
	{
		Debug.Assert(TriggerTarget != null, transform.name + "断言失败", gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Debug.Log("进入 对象" + collision.gameObject.name);
		if (Trigged && _triggeOnce) return;
		if (collision.gameObject != TriggerTarget) return;
		TargetStayIn = true;
		OnPlayerEnter?.Invoke();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		//Debug.Log("退出 对象" + collision.gameObject.name);
		if (Trigged && _triggeOnce) return;
		if (collision.gameObject != TriggerTarget) return;
		OnPlayerExit?.Invoke();
		TargetStayIn = false;
		Trigged = true;
	}

	// ----------------//
	// --- 公有方法
	// ----------------//
	public void AddListener(UnityAction enterFunc, UnityAction exitFunc)
	{
		OnPlayerEnter.AddListener(enterFunc);
		OnPlayerExit.AddListener(exitFunc);
	}

	private void OnDestroy()
	{
		OnPlayerEnter.RemoveAllListeners();
		OnPlayerExit.RemoveAllListeners();
	}

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//
}
