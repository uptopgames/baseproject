using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class ServerSettingsManager : MonoBehaviour
{
	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	public Dictionary<string, ServerSettings.Serializable> settings = new Dictionary<string, ServerSettings.Serializable>();
	
	private string pre = "__stng:";
	
	public void SaveKey(string _key)
	{
		if (!settings.ContainsKey(_key))
			return;
		
		string key = pre + _key;
		ServerSettings.Serializable setting = settings[_key];
		
		if (setting.type == ServerSettings.Type.String) Save.Set(key, setting.value);
		else if (setting.type == ServerSettings.Type.Int) Save.Set(key, setting.value.ToInt32());
		else if (setting.type == ServerSettings.Type.Float) Save.Set(key, setting.value.ToFloat());
		else if (setting.type == ServerSettings.Type.Double) Save.Set(key, setting.value.ToDouble());
		else if (setting.type == ServerSettings.Type.Bool) Save.Set(key, setting.value.ToBool());
		else if (setting.type == ServerSettings.Type.Vector2) Save.Set(key, setting.value.ToVector2());
		else if (setting.type == ServerSettings.Type.Vector3) Save.Set(key, setting.value.ToVector3());
		else if (setting.type == ServerSettings.Type.Vector4) Save.Set(key, setting.value.ToVector4());
		else if (setting.type == ServerSettings.Type.Quaternion) Save.Set(key, setting.value.ToQuaternion());
		
		else if (setting.type == ServerSettings.Type.ArrayString) Save.Set(key, setting.value.ToArrayString());
		else if (setting.type == ServerSettings.Type.ArrayInt) Save.Set(key, setting.value.ToArrayInt32());
		else if (setting.type == ServerSettings.Type.ArrayFloat) Save.Set(key, setting.value.ToArrayFloat());
		else if (setting.type == ServerSettings.Type.ArrayDouble) Save.Set(key, setting.value.ToArrayDouble());
		else if (setting.type == ServerSettings.Type.ArrayBool) Save.Set(key, setting.value.ToArrayBool());
		else if (setting.type == ServerSettings.Type.ArrayVector2) Save.Set(key, setting.value.ToArrayVector2());
		else if (setting.type == ServerSettings.Type.ArrayVector3) Save.Set(key, setting.value.ToArrayVector3());
		else if (setting.type == ServerSettings.Type.ArrayVector4) Save.Set(key, setting.value.ToArrayVector4());
		else if (setting.type == ServerSettings.Type.ArrayQuaternion) Save.Set(key, setting.value.ToArrayQuaternion());
	}
	
	public void Load()
	{
		new GameJsonConnection(Flow.URL_BASE + "login/app_settings.php", OnReceiveSettings).connect(new WWWForm().Add("app_id", Info.appId));
	}
	
	void OnReceiveSettings(string error, IJSonObject data)
	{
		Debug.Log(error);
		Debug.Log(data);
		if (error != null || data.IsEmpty() || data.IsError())
			return;
		
		for (int i = 0; i < data.Count; i++)
		{
			string key = (data[i].Contains("setting")) ? (pre + data[i].GetString("setting")) : default(string);
			string value = (data[i].Contains("value")) ? data[i].GetString("value") : default(string);
			string type = (data[i].Contains("type")) ? data[i].GetString("type") : default(string);
			
			if (!key.IsEmpty() && data != null && data.ToString() != "")
			{
				if (type == "string") Save.Set(key, value);
				else if (type == "int") Save.Set(key, value.ToInt32());
				else if (type == "float") Save.Set(key, value.ToFloat());
				else if (type == "double") Save.Set(key, value.ToDouble());
				else if (type == "bool") Save.Set(key, value.ToBool());
				else if (type == "Vector2") Save.Set(key, value.ToVector2());
				else if (type == "Vector3") Save.Set(key, value.ToVector3());
				else if (type == "Vector4") Save.Set(key, value.ToVector4());
				else if (type == "Quaternion") Save.Set(key, value.ToQuaternion());
				
				else if (type == "string (array)") Save.Set(key, value);
				else if (type == "int (array)") Save.Set(key, value.ToArrayInt32());
				else if (type == "float (array)") Save.Set(key, value.ToArrayFloat());
				else if (type == "double (array)") Save.Set(key, value.ToArrayDouble());
				else if (type == "bool (array)") Save.Set(key, value.ToArrayBool());
				else if (type == "Vector2 (array)") Save.Set(key, value.ToArrayVector2());
				else if (type == "Vector3 (array)") Save.Set(key, value.ToArrayVector3());
				else if (type == "Vector4 (array)") Save.Set(key, value.ToArrayVector4());
				else if (type == "Quaternion (array)") Save.Set(key, value.ToArrayQuaternion());
			}
		}
	}
}
