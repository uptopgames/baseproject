using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public static class ConfigManagerServerSettingsUIExtension
{
	public static bool MoreButton<T>(ref T[] array)
	{
		if (GUILayout.Button("+"))
		{
			Array.Resize(ref array, array.Length + 1);
			return true;
		}
		return false;
	}
	
	public static void MinusButton<T>(ref T[] array)
	{
		if (GUILayout.Button("-") && array.Length > 1)
			Array.Resize(ref array, array.Length - 1);
	}
	
	public static void SetSpace()
	{
		EditorGUILayout.Space();
	}
	
	public static void EndChanges(string[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
	
	public static void EndChanges(int[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
	
	public static void EndChanges(float[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
	
	public static void EndChanges(double[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
	
	public static void EndChanges(bool[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
	
	public static void EndChanges(Vector2[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
	
	public static void EndChanges(Vector3[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
	
	public static void EndChanges(Vector4[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
	
	public static void EndChanges(Quaternion[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
	
	public static void EndChanges(Color[] array, ServerSettings.Serializable setting, bool space = true)
	{
		setting.value = array.ToStringArray();
		if (space) SetSpace();
	}
}
