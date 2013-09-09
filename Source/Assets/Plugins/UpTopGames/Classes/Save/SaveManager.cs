using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class SaveManager : MonoBehaviour 
{
	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	public Dictionary<string, string> _string = new Dictionary<string, string>();
	public Dictionary<string, int> _int = new Dictionary<string, int>();
	public Dictionary<string, float> _float = new Dictionary<string, float>();
	public Dictionary<string, bool> _bool = new Dictionary<string, bool>();
	public Dictionary<string, double> _double = new Dictionary<string, double>();
	public Dictionary<string, Vector2> _Vector2 = new Dictionary<string, Vector2>();
	public Dictionary<string, Vector3> _Vector3 = new Dictionary<string, Vector3>();
	public Dictionary<string, Vector4> _Vector4 = new Dictionary<string, Vector4>();
	public Dictionary<string, Quaternion> _Quaternion = new Dictionary<string, Quaternion>();
	public Dictionary<string, Color> _Color = new Dictionary<string, Color>();
	
	public bool HasKey(string key) {
		if (_string.ContainsKey(key)) return true;
		if (_int.ContainsKey(key)) return true;
		if (_float.ContainsKey(key)) return true;
		if (_bool.ContainsKey(key)) return true;
		if (_double.ContainsKey(key)) return true;
		if (_Vector2.ContainsKey(key)) return true;
		if (_Vector3.ContainsKey(key)) return true;
		if (_Vector4.ContainsKey(key)) return true;
		if (_Quaternion.ContainsKey(key)) return true;
		if (_Color.ContainsKey(key)) return true;
		return false;
	}
	
	public void Delete(string key) {
		if (_string.ContainsKey(key)) _string.Remove(key);
		if (_int.ContainsKey(key)) _int.Remove(key);
		if (_float.ContainsKey(key)) _float.Remove(key);
		if (_bool.ContainsKey(key)) _bool.Remove(key);
		if (_double.ContainsKey(key)) _double.Remove(key);
		if (_Vector2.ContainsKey(key)) _Vector2.Remove(key);
		if (_Vector3.ContainsKey(key)) _Vector3.Remove(key);
		if (_Vector4.ContainsKey(key)) _Vector4.Remove(key);
		if (_Quaternion.ContainsKey(key)) _Quaternion.Remove(key);
		if (_Color.ContainsKey(key)) _Color.Remove(key);
	}
	
	public void DeleteAll() { _string.Clear(); _int.Clear(); _float.Clear(); _bool.Clear(); _double.Clear();
		_Vector2.Clear(); _Vector3.Clear(); _Vector4.Clear(); _Quaternion.Clear(); _Color.Clear(); }
	
	public void Set(string key, string value) { Delete(key); _string.Add(key, value); }
	public void Set(string key, int value) { Delete(key); _int.Add(key, value); }
	public void Set(string key, float value) { Delete(key); _float.Add(key, value); }
	public void Set(string key, bool value) { Delete(key); _bool.Add(key, value); }
	public void Set(string key, double value) { Delete(key); _double.Add(key, value); }
	public void Set(string key, Vector2 value) { Delete(key); _Vector2.Add(key, value); }
	public void Set(string key, Vector3 value) { Delete(key); _Vector3.Add(key, value); }
	public void Set(string key, Vector4 value) { Delete(key); _Vector4.Add(key, value); }
	public void Set(string key, Quaternion value) { Delete(key); _Quaternion.Add(key, value); }
	public void Set(string key, Color value) { Delete(key); _Color.Add(key, value); }
	
	public string GetString(string key) { return (_string.ContainsKey(key)) ? _string[key] : default(string); }
	public int GetInt(string key) { return (_int.ContainsKey(key)) ? _int[key] : default(int); }
	public float GetFloat(string key) { return (_float.ContainsKey(key)) ? _float[key] : default(float); }
	public bool GetBool(string key) { return (_bool.ContainsKey(key)) ? _bool[key] : default(bool); }
	public double GetDouble(string key) { return (_double.ContainsKey(key)) ? _double[key] : default(double); }
	public Vector2 GetVector2(string key) { return (_Vector2.ContainsKey(key)) ? _Vector2[key] : default(Vector2); }
	public Vector3 GetVector3(string key) { return (_Vector3.ContainsKey(key)) ? _Vector3[key] : default(Vector3); }
	public Vector4 GetVector4(string key) { return (_Vector4.ContainsKey(key)) ? _Vector4[key] : default(Vector4); }
	public Quaternion GetQuaternion(string key) { return (_Quaternion.ContainsKey(key)) ? _Quaternion[key] : default(Quaternion); }
	public Color GetColor(string key) { return (_Color.ContainsKey(key)) ? _Color[key] : default(Color); }
	
	public void SaveAll()
	{
		var __string = new Dictionary<string, string>(_string);
		var __int = new Dictionary<string, int>(_int);
		var __float = new Dictionary<string, float>(_float);
		var __bool = new Dictionary<string, bool>(_bool);
		var __double = new Dictionary<string, double>(_double);
		var __Vector2 = new Dictionary<string, Vector2>(_Vector2);
		var __Vector3 = new Dictionary<string, Vector3>(_Vector3);
		var __Vector4 = new Dictionary<string, Vector4>(_Vector4);
		var __Quaternion = new Dictionary<string, Quaternion>(_Quaternion);
		var __Color = new Dictionary<string, Color>(_Color);
		if (__string.Count > 0) foreach (var pair in __string) Save.Set(pair.Key, pair.Value, true);
		if (__int.Count > 0) foreach (var pair in __int) Save.Set(pair.Key, pair.Value, true);
		if (__float.Count > 0) foreach (var pair in __float) Save.Set(pair.Key, pair.Value, true);
		if (__bool.Count > 0) foreach (var pair in __bool) Save.Set(pair.Key, pair.Value, true);
		if (__double.Count > 0) foreach (var pair in __double) Save.Set(pair.Key, pair.Value, true);
		if (__Vector2.Count > 0) foreach (var pair in __Vector2) Save.Set(pair.Key, pair.Value, true);
		if (__Vector3.Count > 0) foreach (var pair in __Vector3) Save.Set(pair.Key, pair.Value, true);
		if (__Vector4.Count > 0) foreach (var pair in __Vector4) Save.Set(pair.Key, pair.Value, true);
		if (__Quaternion.Count > 0) foreach (var pair in __Quaternion) Save.Set(pair.Key, pair.Value, true);
		if (__Color.Count > 0) foreach (var pair in __Color) Save.Set(pair.Key, pair.Value, true);
	}
	
	public void DebugAll()
	{
		var __string = new Dictionary<string, string>(_string);
		var __int = new Dictionary<string, int>(_int);
		var __float = new Dictionary<string, float>(_float);
		var __bool = new Dictionary<string, bool>(_bool);
		var __double = new Dictionary<string, double>(_double);
		var __Vector2 = new Dictionary<string, Vector2>(_Vector2);
		var __Vector3 = new Dictionary<string, Vector3>(_Vector3);
		var __Vector4 = new Dictionary<string, Vector4>(_Vector4);
		var __Quaternion = new Dictionary<string, Quaternion>(_Quaternion);
		var __Color = new Dictionary<string, Color>(_Color);
		string title = "List of values and keys on SavePrefs:\n";
		string debug = title + "";
		string s = "          ";
		if (__string.Count > 0) { debug += "- String\n"; foreach (var pair in __string) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (__int.Count > 0) { debug += "- Int\n"; foreach (var pair in __int) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (__float.Count > 0) { debug += "- Float\n"; foreach (var pair in __float) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (__bool.Count > 0) { debug += "- Bool\n"; foreach (var pair in __bool) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (__double.Count > 0) { debug += "- Double\n"; foreach (var pair in __double) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (__Vector2.Count > 0) { debug += "- Vector2\n"; foreach (var pair in __Vector2) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (__Vector3.Count > 0) { debug += "- Vector3\n"; foreach (var pair in __Vector3) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (__Vector4.Count > 0) { debug += "- Vector4\n"; foreach (var pair in __Vector4) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (__Quaternion.Count > 0) { debug += "- Quaternion\n"; foreach (var pair in __Quaternion) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (__Color.Count > 0) { debug += "- Color\n"; foreach (var pair in __Color) debug += s + "Key: '" + pair.Key + "' || Value: '" + pair.Value + "'\n"; }
		if (debug == title) debug += "Empty!";
		Debug.Log(debug);
	}
}
