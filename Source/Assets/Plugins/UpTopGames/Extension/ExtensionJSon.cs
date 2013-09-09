using UnityEngine;
using System.Collections;
using CodeTitans.JSon;

public static class ExtensionJSon
{
	private static IJSonObject empty;
	
	private static string API = "ExtensionJSON";
	
	private const string
		ERROR_KEY_NOT_FOUND = "key '$key' not found",
		ERROR_INVALID_DATETIME = "'$key' is not a DateTime value.",
		ERROR_INVALID_JSON = "{\"error\":\"error parsing JSon\"}",
		ERROR_EMPTY_JSON = "{}";

	private static void Error(string method, string error, string key)
	{
		Debug.LogWarning(API + "." + method + "() " + error.Replace("$key", key));
	}
	
	public static IJSonObject Empty()
	{
		if (ExtensionJSon.empty != null)
			return ExtensionJSon.empty;
		
		ExtensionJSon.empty = ERROR_EMPTY_JSON.ToJSon();
		return ExtensionJSon.empty;
	}
	
	public static bool IsNull(this IJSonObject json)
	{
		return json == null;
	}
	
	public static bool IsError(this IJSonObject json)
	{
		return json.ToString() == ERROR_INVALID_JSON;	
	}
	
	public static bool IsEmpty(this IJSonObject json)
	{
		return json == null || json.IsNull() | json.ToString() == ERROR_EMPTY_JSON || json.ToString().IsEmpty();
	}
	
	public static IJSonObject Get(this IJSonObject json, string key)
	{
		return json.Contains(key) ? json[key] : Empty();
	}
	
	public static string GetString(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty() || !_json.ToString().IsNull())
			return _json.ToString();
		
		Error("GetString", ERROR_KEY_NOT_FOUND, key);
		
		return "";
	}
	
	public static int GetInt(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToInt32();
		
		Error("GetInt", ERROR_KEY_NOT_FOUND, key);
		
		return default(int);
	}
	
	
	public static float GetFloat(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToFloat();
		
		Error("GetFloat", ERROR_KEY_NOT_FOUND, key);
		
		return default(float);
	}
	
	public static double GetDouble(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToDouble();
		
		Error("GetDouble", ERROR_KEY_NOT_FOUND, key);
		
		return default(float);
	}
	
	public static bool GetBool(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToBool();
		
		Error("GetBool", ERROR_KEY_NOT_FOUND, key);
		
		return default(bool);
	}
	
	public static Vector2 GetVector2(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToVector2();
		
		Error("GetVector2", ERROR_KEY_NOT_FOUND, key);
		
		return default(Vector2);
	}
	
	public static Vector3 GetVector3(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToVector3();
		
		Error("GetVector3", ERROR_KEY_NOT_FOUND, key);
		
		return default(Vector3);
	}
	
	public static Vector4 GetVector4(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToVector4();
		
		Error("GetVector4", ERROR_KEY_NOT_FOUND, key);
		
		return default(Vector4);
	}
	
	public static Quaternion GetQuaternion(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToQuaternion();
		
		Error("GetQuarternion", ERROR_KEY_NOT_FOUND, key);
		
		return default(Quaternion);
	}
	
	public static Color GetColor(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToColor();
		
		Error("GetColor", ERROR_KEY_NOT_FOUND, key);
		
		return default(Color);
	}
	
	public static System.DateTime GetDateTime(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
		{
			System.DateTime date = default(System.DateTime);
			try
			{
				date = _json.DateTimeValue;
			}
			catch
			{
				Error("GetDateTime", ERROR_INVALID_DATETIME, json.ToString());
			}
			
			return date;
		}
		
		Error("GetDateTime", ERROR_KEY_NOT_FOUND, key);
		
		return default(System.DateTime);
	}
	
	public static char GetChar(this IJSonObject json, string key)
	{
		IJSonObject _json = json.Get(key);
		
		if (!_json.IsEmpty())
			return _json.ToString().ToChar();
		
		Error("GetChar", ERROR_KEY_NOT_FOUND, key);
		
		return default(char);
	}
	
	public static IJSonObject ToJSon(this string value)
    {
		JSonReader reader = new JSonReader();
		
		try
		{
			IJSonObject data = reader.ReadAsJSonObject(value);
			return data;
		}
		catch (JSonReaderException)
		{
			return reader.ReadAsJSonObject(ERROR_INVALID_JSON);
		}
    }
	
	public static int ToInt32(this IJSonObject json)
	{
		return json.ToString().ToInt32();
	}
	
	public static float ToFloat(this IJSonObject json)
	{
		return json.ToString().ToFloat();
	}
	
	public static double ToDouble(this IJSonObject json)
	{
		return json.ToString().ToDouble();
	}
	
	public static bool ToBool(this IJSonObject json)
	{
		return json.ToString().ToBool();
	}
	
	public static Vector2 ToVector2(this IJSonObject json)
	{
		return json.ToString().ToVector2();
	}
	
	public static Vector3 ToVector3(this IJSonObject json)
	{
		return json.ToString().ToVector3();
	}
	
	public static Vector4 ToVector4(this IJSonObject json)
	{
		return json.ToString().ToVector4();
	}
	
	public static Quaternion ToQuaternion(this IJSonObject json)
	{
		return json.ToString().ToQuaternion();
	}
	
	public static Color ToColor(this IJSonObject json)
	{
		return json.ToString().ToColor();
	}
	
	public static System.DateTime ToDateTime(this IJSonObject json)
	{
		System.DateTime date = default(System.DateTime);
		try
		{
			date = json.DateTimeValue;
		}
		catch
		{
			Error("ToDateTime", ERROR_INVALID_DATETIME, json.ToString());
		}
		
		return date;
	}
	
	public static char ToChar(this IJSonObject json)
	{
		return json.ToString().ToChar();
	}
}
