using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public static class ConfigManagerServerSettingsCreateExtension
{
	
	public static void Draw(ConfigManager config)
	{
		EditorGUILayout.LabelField("- Create Server Setting");
		config.serverSettingKey = EditorGUILayout.TextField("Key", config.serverSettingKey);
	
		string valueText = "Value";
		
		if (config.newSetting.type != config.oldSetting)
		{
			ConfigManagerServerSettingsExtension.FixEmptySetting(config.newSetting);
			config.oldSetting = config.newSetting.type;
		}
		
		if (config.newSetting.type == ServerSettings.Type.String)
			config.newSetting.value = EditorGUILayout.TextField(valueText, config.newSetting.value);
		else if (config.newSetting.type == ServerSettings.Type.Int)
			config.newSetting.value = EditorGUILayout.IntField(valueText, config.newSetting.value.ToInt32()).ToString();
		else if (config.newSetting.type == ServerSettings.Type.Float || config.newSetting.type == ServerSettings.Type.Double)	
			config.newSetting.value = EditorGUILayout.FloatField(valueText, config.newSetting.value.ToFloat()).ToString();
		else if (config.newSetting.type == ServerSettings.Type.Bool)
			config.newSetting.value = EditorGUILayout.Toggle(valueText, config.newSetting.value.ToBool()).ToString();
		else if (config.newSetting.type == ServerSettings.Type.Vector2)
			config.newSetting.value = EditorGUILayout.Vector2Field(valueText, config.newSetting.value.ToVector2()).ToString();
		else if (config.newSetting.type == ServerSettings.Type.Vector3)
			config.newSetting.value = EditorGUILayout.Vector3Field(valueText, config.newSetting.value.ToVector3()).ToString();
		else if (config.newSetting.type == ServerSettings.Type.Vector4 || config.newSetting.type == ServerSettings.Type.Quaternion)
			config.newSetting.value = EditorGUILayout.Vector4Field(valueText, config.newSetting.value.ToVector4()).ToString();
		else if (config.newSetting.type == ServerSettings.Type.Color)
			config.newSetting.value = EditorGUILayout.ColorField(valueText, config.newSetting.value.ToColor()).ToHex();
		
		else if (config.newSetting.type == ServerSettings.Type.ArrayString)
		{
			EditorGUILayout.LabelField(valueText);
				
			string[] array = config.newSetting.value.ToArrayString();
			for (int i = 0; i < array.Length; i++)
				array[i] = EditorGUILayout.TextField(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
			

			if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
				array[array.Length - 1] = " ";
			ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
			ConfigManagerServerSettingsUIExtension.EndChanges(array, config.newSetting, false);
		}
		else if (config.newSetting.type == ServerSettings.Type.ArrayInt)
		{
			EditorGUILayout.LabelField(valueText);
				
			int[] array = config.newSetting.value.ToArrayInt32();
			for (int i = 0; i < array.Length; i++)
				array[i] = EditorGUILayout.IntField(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
			
			if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
				array[array.Length - 1] = 0;
			ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
			ConfigManagerServerSettingsUIExtension.EndChanges(array, config.newSetting, false);
		}
		else if (config.newSetting.type == ServerSettings.Type.ArrayFloat || config.newSetting.type == ServerSettings.Type.ArrayDouble)
		{
			EditorGUILayout.LabelField(valueText);
				
			float[] array = config.newSetting.value.ToArrayFloat();
			for (int i = 0; i < array.Length; i++)
				array[i] = EditorGUILayout.FloatField(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
			
			if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
				array[array.Length - 1] = 0f;
			ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
			ConfigManagerServerSettingsUIExtension.EndChanges(array, config.newSetting, false);
		}
		else if (config.newSetting.type == ServerSettings.Type.ArrayBool)
		{
			EditorGUILayout.LabelField(valueText);
				
			bool[] array = config.newSetting.value.ToArrayBool();
			for (int i = 0; i < array.Length; i++)
				array[i] = EditorGUILayout.Toggle(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
			
			if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
				array[array.Length - 1] = false;
			ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
			ConfigManagerServerSettingsUIExtension.EndChanges(array, config.newSetting, false);
		}
		else if (config.newSetting.type == ServerSettings.Type.ArrayVector2)
		{
			EditorGUILayout.LabelField(valueText);
				
			Vector2[] array = config.newSetting.value.ToArrayVector2();
			for (int i = 0; i < array.Length; i++)
				array[i] = EditorGUILayout.Vector2Field(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
			
			if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
				array[array.Length - 1] = Vector2.zero;
			ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
			ConfigManagerServerSettingsUIExtension.EndChanges(array, config.newSetting, false);
		}
		else if (config.newSetting.type == ServerSettings.Type.ArrayVector3)
		{
			EditorGUILayout.LabelField(valueText);
				
			Vector3[] array = config.newSetting.value.ToArrayVector3();
			for (int i = 0; i < array.Length; i++)
				array[i] = EditorGUILayout.Vector3Field(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
			
			if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
				array[array.Length - 1] = Vector3.zero;
			ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
			ConfigManagerServerSettingsUIExtension.EndChanges(array, config.newSetting, false);
		}
		else if (config.newSetting.type == ServerSettings.Type.ArrayVector4 || config.newSetting.type == ServerSettings.Type.ArrayQuaternion)
		{
			EditorGUILayout.LabelField(valueText);
				
			Vector4[] array = config.newSetting.value.ToArrayVector4();
			for (int i = 0; i < array.Length; i++)
				array[i] = EditorGUILayout.Vector4Field(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
			
			if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
				array[array.Length - 1] = Vector4.zero;
			ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
			ConfigManagerServerSettingsUIExtension.EndChanges(array, config.newSetting, false);
		}
		else if (config.newSetting.type == ServerSettings.Type.ArrayColor)
		{
			EditorGUILayout.LabelField(valueText);
				
			Color[] array = config.newSetting.value.ToArrayColor();
			for (int i = 0; i < array.Length; i++)
				array[i] = EditorGUILayout.ColorField(" ".Multiply(10) + "[" + i.ToString() + "]", array[i]);	
			
			if (ConfigManagerServerSettingsUIExtension.MoreButton(ref array))
				array[array.Length - 1] = Vector4.zero;
			ConfigManagerServerSettingsUIExtension.MinusButton(ref array);
			ConfigManagerServerSettingsUIExtension.EndChanges(array, config.newSetting, false);
		}
		
		config.newSetting.type = (ServerSettings.Type)EditorGUILayout.EnumPopup("Type", config.newSetting.type);
			
		if (GUILayout.Button("Create")) ConfigManagerServerSettingsExtension.AddSettings(config, config.serverSettingKey, config.newSetting.value, config.newSetting.type);
		
		ConfigManagerServerSettingsUIExtension.SetSpace();
	}
}
