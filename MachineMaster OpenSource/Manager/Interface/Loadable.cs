using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArchiveManager;

public abstract class Loadable<T> where T : Loadable<T>, new()
{
	// ------------------ //    
	// --- 序列化    
	// ------------------ //


	// ------------------ //    
	// --- 公有成员    
	// ------------------ //
	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new T();
			}
			return _instance;
		}
	}


	// ------------------ //   
	// --- 私有成员    
	// ------------------ //
	private static T _instance;


	// ------------------ //    
	// --- Unity消息    
	// ------------------ //
	protected abstract void LoadArchive(Archive archive);
	protected abstract void SaveIntoArchive(Archive archive);

	// ------------------ //    
	// --- 公有方法   
	// ------------------ //
	public abstract void ResetData();
	public void LoadFromArchive(Archive archive)
	{
		ResetData();
		LoadArchive(archive);
	}
	public void SaveDataToArchive(Archive archive)
	{
		SaveIntoArchive(archive);
	}
	// ------------------ //   
	// --- 私有方法
	// ------------------ //
}
