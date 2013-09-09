using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ServerSettings
{
	private static ServerSettingsManager _manager;
	
	public static ServerSettingsManager manager
	{
		get
		{
			if (ServerSettings._manager != null)
				return ServerSettings._manager;
			
			GameObject obj = Flow.config;
			
			ServerSettingsManager manager = obj.GetComponent<ServerSettingsManager>();
			if (manager == null) manager = obj.AddComponent<ServerSettingsManager>();
			
			ServerSettings._manager = manager;
			
			return ServerSettings._manager;
		}
	}
	
	[System.Serializable]
	public class Serializable
	{
		public string value;
		public ServerSettings.Type type;
		
		public Serializable(string value, ServerSettings.Type type)
		{
			this.value = value;
			this.type = type;
		}
	}
	
	public enum Type
	{
		String, Int, Float, Double, Bool, Vector2, Vector3, Vector4, Quaternion, Color,
		ArrayString, ArrayInt, ArrayFloat, ArrayDouble, ArrayBool, ArrayVector2, ArrayVector3, 
		ArrayVector4, ArrayQuaternion, ArrayColor, Unknown
	};
	
	private static string pre = "__stng:";
	
	public static void Load()
	{
		manager.Load();
	}
	
	public static void Add(string key, Serializable setting)
	{
		if (!manager.settings.ContainsKey(key)) manager.settings.Add(key, setting);
		else manager.settings[key] = setting;
		
		manager.SaveKey(key);
	}
	
	public static string GetString(string key)
	{
		return Save.GetString(pre + key);
	}
	
	public static int GetInt(string key)
	{
		return Save.GetInt(pre + key);
	}
	
	public static float GetFloat(string key)
	{
		return Save.GetFloat(pre + key);
	}
	
	public static double GetDouble(string key)
	{
		return Save.GetDouble(pre + key);
	}
	
	public static bool GetBool(string key)
	{
		return Save.GetBool(pre + key);
	}
	
	public static Vector2 GetVector2(string key)
	{
		return Save.GetVector2(pre + key);
	}
	
	public static Vector3 GetVector3(string key)
	{
		return Save.GetVector3(pre + key);
	}
	
	public static Vector4 GetVector4(string key)
	{
		return Save.GetVector4(pre + key);
	}
	
	public static Quaternion GetQuaternion(string key)
	{
		return Save.GetQuaternion(pre + key);
	}
	
	public static string[] GetStringArray(string key)
	{
		return Save.GetStringArray(pre + key);
	}
	
	public static int[] GetIntArray(string key)
	{
		return Save.GetIntArray(pre + key);
	}
	
	public static float[] GetFloatArray(string key)
	{
		return Save.GetFloatArray(pre + key);
	}
	
	public static double[] GetDoubleArray(string key)
	{
		return Save.GetDoubleArray(pre + key);
	}
	
	public static bool[] GetBoolArray(string key)
	{
		return Save.GetBoolArray(pre + key);
	}
	
	public static Vector2[] GetVector2Array(string key)
	{
		return Save.GetVector2Array(pre + key);
	}
	
	public static Vector3[] GetVector3Array(string key)
	{
		return Save.GetVector3Array(pre + key);
	}
	
	public static Vector4[] GetVector4Array(string key)
	{
		return Save.GetVector4Array(pre + key);
	}
	
	public static Quaternion[] GetQuaternionArray(string key)
	{
		return Save.GetQuaternionArray(pre + key);
	}

}
