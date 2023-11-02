using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleKeyDict<TKey1, TKey2, TValue>
{
	// ----------------//
	// --- 序列化
	// ----------------//


	// ----------------//
	// --- 公有成员
	// ----------------//
	public TValue this[TKey1 key] 
	{ 
		get => key2ToValue[key1ToKey2[key]];
	}
	public TValue this[TKey2 key] 
	{ 
		get => key2ToValue[key]; 
	}
	public TValue this[TKey1 key1, TKey2 key2]
	{
		set 
		{ 
			key1ToKey2.Add(key1, key2);
			key2ToKey1.Add(key2, key1);
			key2ToValue.Add(key2, value);
		}
	}

	// ----------------//
	// --- 私有成员
	// ----------------//
	private Dictionary<TKey1, TKey2> key1ToKey2 = new Dictionary<TKey1, TKey2>();
	private Dictionary<TKey2, TKey1> key2ToKey1 = new Dictionary<TKey2, TKey1>();
	private Dictionary<TKey2, TValue> key2ToValue = new Dictionary<TKey2, TValue>();


	// ----------------//
	// --- 公有方法
	// ----------------//
	public void Clear()
	{
		key1ToKey2.Clear();
		key2ToValue.Clear();
		key2ToKey1.Clear();
	}
	public void Remove(TKey1 key)
	{
		key2ToValue.Remove(key1ToKey2[key]);
		key2ToKey1.Remove(key1ToKey2[key]);
		key1ToKey2.Remove(key);
	}
	public void Remove(TKey2 key)
	{
		Remove(key2ToKey1[key]);
	}

	// ----------------//
	// --- 私有方法
	// ----------------//

	// ----------------//
	// --- 类型
	// ----------------//

}
