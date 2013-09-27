using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public static class ConfigManagerServerSettingsListExtension
{
	
	public static void Draw(ConfigManager config)
	{
		foreach (ConfigManager.CachedServerSettings setting in config.serverSettings)
		{
			if (setting.setting.type == ServerSettings.Type.String)
			{
				setting.setting.value = EditorGUILayout.TextField(setting.key + " (string)", setting.setting.value);
			}
			else if (setting.setting.type == ServerSettings.Type.Int)
			{
				setting.setting.value = EditorGUILayout.IntField(setting.key + " (int)", setting.setting.value.ToInt32()).ToString();
			}
			else if (setting.setting.type == ServerSettings.Type.Float || setting.setting.type == ServerSettings.Type.Double)
			{
				setting.setting.value = EditorGUILayout.FloatField(setting.key + " (float)", setting.setting.value.ToFloat()).ToString();
			}
			else if (setting.setting.type == ServerSettings.Type.Bool)
			{
				setting.setting.value = EditorGUILayout.Toggle(setting.key + " (bool)", setting.setting.value.ToBool()).ToString();
			}
			else if (setting.setting.type == ServerSettings.Type.Vector2)
			{
				setting.setting.value = EditorGUILayout.Vector2Field(setting.key + " (Vector2)", setting.setting.value.ToVector2()).ToString();
			}
			else if (setting.setting.type == ServerSettings.Type.Vector3)
			{
				setting.setting.value = EditorGUILayout.Vector3Field(setting.key + " (Vector3)", setting.setting.value.ToVector3()).ToString();
			}
			else if (setting.setting.type == ServerSettings.Type.Vector4 || setting.setting.type == ServerSettings.Type.Quaternion)
			{
				setting.setting.value = EditorGUILayout.Vector4Field(setting.key + " (" + ((setting.setting.type == ServerSettings.Type.Quaternion) ? "Quaternion" : "Vector4") + ")", setting.setting.value.ToVector4()).ToString();
			}
			else if (setting.setting.type == ServerSettings.Type.Color)
			{
				setting.setting.value = EditorGUILayout.ColorField(setting.key + " (Color)", setting.setting.value.ToColor()).ToHex();
			}
			else if (setting.setting.type == ServerSettings.Type.ArrayString)
			{
				EditorGUILayout.LabelField(setting.key + " (string[])");
				
				string[] array = setting.setting.value.ToArrayString();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = EditorGUILayout.TextField(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
				}
				
				if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array)) array[array.Length - 1] = " ";
				
				ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
				ConfigManagerServerSettingsUIExtension.EndChanges(array, setting.setting);
			}
			else if (setting.setting.type == ServerSettings.Type.ArrayInt)
			{
				EditorGUILayout.LabelField(setting.key + " (int[])");
				
				int[] array = setting.setting.value.ToArrayInt32();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = EditorGUILayout.IntField(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
				}
				
				if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
				{
					array[array.Length - 1] = 0;
				}
				
				ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
				ConfigManagerServerSettingsUIExtension.EndChanges(array, setting.setting);
			}
			else if (setting.setting.type == ServerSettings.Type.ArrayFloat || setting.setting.type == ServerSettings.Type.ArrayDouble)
			{
				EditorGUILayout.LabelField(setting.key + " (" + ((setting.setting.type == ServerSettings.Type.ArrayDouble) ? "double" : "float") + "[])");
				
				float[] array = setting.setting.value.ToArrayFloat();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = EditorGUILayout.FloatField(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
				}
				
				if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array)) array[array.Length - 1] = 1f;
				
				ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
				ConfigManagerServerSettingsUIExtension.EndChanges(array, setting.setting);
			}
			else if (setting.setting.type == ServerSettings.Type.ArrayBool)
			{
				EditorGUILayout.LabelField(setting.key + " (bool[])");
				
				bool[] array = setting.setting.value.ToArrayBool();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = EditorGUILayout.Toggle(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
				}
				if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
					array[array.Length - 1] = false;
				ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
				ConfigManagerServerSettingsUIExtension.EndChanges(array, setting.setting);
			}
			else if (setting.setting.type == ServerSettings.Type.ArrayVector2)
			{
				EditorGUILayout.LabelField(setting.key + " (Vector2[])");
				
				Vector2[] array = setting.setting.value.ToArrayVector2();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = EditorGUILayout.Vector2Field(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
				}
				
				if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array)) array[array.Length - 1] = Vector2.zero;
				
				ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
				ConfigManagerServerSettingsUIExtension.EndChanges(array, setting.setting);
			}
			else if (setting.setting.type == ServerSettings.Type.ArrayVector3)
			{
				EditorGUILayout.LabelField(setting.key + " (Vector3[])");
				
				Vector3[] array = setting.setting.value.ToArrayVector3();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = EditorGUILayout.Vector3Field(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
				}
				
				if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array)) array[array.Length - 1] = Vector3.zero;
				
				ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
				ConfigManagerServerSettingsUIExtension.EndChanges(array, setting.setting);
			}
			else if (setting.setting.type == ServerSettings.Type.ArrayVector4 || setting.setting.type == ServerSettings.Type.ArrayQuaternion)
			{
				EditorGUILayout.LabelField(setting.key + " (" + ((setting.setting.type == ServerSettings.Type.ArrayQuaternion) ? "Quaternion" : "Vector4") + "[])");
				
				Vector4[] array = setting.setting.value.ToArrayVector4();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = EditorGUILayout.Vector4Field(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
				}
				
				if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array)) array[array.Length - 1] = Vector4.zero;
				
				ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
				ConfigManagerServerSettingsUIExtension.EndChanges(array, setting.setting);
			}
			else if (setting.setting.type == ServerSettings.Type.ArrayColor)
			{
				EditorGUILayout.LabelField(setting.key + " (Color[])");
				
				Color[] array = setting.setting.value.ToArrayColor();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = EditorGUILayout.ColorField(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
				}
				
				if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array)) array[array.Length - 1] = Color.black;
				
				ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
				ConfigManagerServerSettingsUIExtension.EndChanges(array, setting.setting);
			}
		}
		
		ConfigManagerServerSettingsUIExtension.SetSpace();
	}

}
