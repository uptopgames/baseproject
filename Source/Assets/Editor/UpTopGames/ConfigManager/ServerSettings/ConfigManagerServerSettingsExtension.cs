using UnityEngine;
using System.Collections;
using System;

public static class ConfigManagerServerSettingsExtension
{
	public static void FixEmptySetting(ServerSettings.Serializable setting)
	{
		setting.value = "";
		
		// singles
		if (setting.type == ServerSettings.Type.String)
			setting.value = "";
		else if (setting.type == ServerSettings.Type.Int || setting.type == ServerSettings.Type.Float ||
			setting.type == ServerSettings.Type.Double || setting.type == ServerSettings.Type.Bool)
			setting.value = 0.ToString();
		else if (setting.type == ServerSettings.Type.Vector2)
			setting.value = Vector2.zero.ToString();
		else if (setting.type == ServerSettings.Type.Vector3)
			setting.value = Vector3.zero.ToString();
		else if (setting.type == ServerSettings.Type.Vector4 || setting.type == ServerSettings.Type.Quaternion)
			setting.value = Vector4.zero.ToString();
		else if (setting.type == ServerSettings.Type.Color)
			setting.value = Color.black.ToHex();
		
		// arrays
		else if (setting.type == ServerSettings.Type.ArrayString)
		{
			string[] array = new string[1];
			array[0] = "";
			setting.value = array.ToStringArray();
		}
		else if (setting.type == ServerSettings.Type.ArrayInt)
			setting.value = new int[1].ToStringArray();
		else if (setting.type == ServerSettings.Type.ArrayFloat || setting.type == ServerSettings.Type.ArrayDouble)
			setting.value = new float[1].ToStringArray();
		else if (setting.type == ServerSettings.Type.ArrayBool)
			setting.value = new bool[1].ToStringArray();
		else if (setting.type == ServerSettings.Type.ArrayVector2)
			setting.value = new Vector2[1].ToStringArray();
		else if (setting.type == ServerSettings.Type.ArrayVector3)
			setting.value = new Vector3[1].ToStringArray();
		else if (setting.type == ServerSettings.Type.ArrayVector4 || setting.type == ServerSettings.Type.ArrayQuaternion)
			setting.value = new Vector4[1].ToStringArray();
		else if (setting.type == ServerSettings.Type.ArrayColor)
			setting.value = new Color[1].ToStringArray();
	}
	
	public static void AddSettings(ConfigManager config, string key, string value, ServerSettings.Type type)
	{
		if (key.IsEmpty())
			return;
		
		int duplicated = -1;
		
		ConfigManager.CachedServerSettings newSetting = new ConfigManager.CachedServerSettings();
		newSetting.key = key;
		newSetting.setting = new ServerSettings.Serializable(value, type);
		
		for(int i = 0; i < config.serverSettings.Length; i++)
		{
			ConfigManager.CachedServerSettings setting = config.serverSettings[i];
			if (setting.key == key)
			{
				duplicated = i;
				break;	
			}
		}
		
		if (!(duplicated >= 0))
		{
			Array.Resize(ref config.serverSettings, config.serverSettings.Length + 1);
			config.serverSettings[config.serverSettings.Length - 1] = newSetting;
			
			Array.Sort(config.serverSettings,
	    		delegate(ConfigManager.CachedServerSettings a, ConfigManager.CachedServerSettings b)
				{ return a.key.CompareTo(b.key); }
			);
			
			return;
		}
		
		config.serverSettings[duplicated] = newSetting;
	}
	
	public static void DeleteSettings(ConfigManager config, string key)
	{
		for(int i = 0; i < config.serverSettings.Length; i++)
		{
			ConfigManager.CachedServerSettings setting = config.serverSettings[i];
			if (setting.key == key)
			{
				
				setting.key = "z".Multiply(256);
				
				Array.Sort(config.serverSettings,
		    		delegate(ConfigManager.CachedServerSettings a, ConfigManager.CachedServerSettings b)
					{ return a.key.CompareTo(b.key); }
				);
				
				Array.Resize(ref config.serverSettings, config.serverSettings.Length - 1);
				
				return;
			}
		}
	}

}
