using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionWWWForm
{
	public static WWWForm ToWWWForm(this Dictionary<string,string> dictionary, WWWForm form)
	{
		if (form == null)
			form = new WWWForm();
		
		foreach (KeyValuePair<string,string> single in dictionary)
			form.AddField(single.Key, single.Value);
			
		return form;
	}
	
	public static WWWForm ToWWWForm(this Dictionary<string,int> dictionary, WWWForm form)
	{
		if (form == null)
			form = new WWWForm();
		
		foreach (KeyValuePair<string,int> single in dictionary)
			form.AddField(single.Key, single.Value);
			
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, int value)
	{
		form.AddField(key, value);
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, string value)
	{
		form.AddField(key, value);
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, float value)
	{
		form.AddField(key, value.ToString());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, double value)
	{
		form.AddField(key, value.ToString());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, bool value)
	{
		form.AddField(key, value.ToString());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Vector2 value)
	{
		form.AddField(key, value.ToString());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Vector3 value)
	{
		form.AddField(key, value.ToString());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Vector4 value)
	{
		form.AddField(key, value.ToString());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Quaternion value)
	{
		form.AddField(key, value.ToString());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Color value)
	{
		form.AddField(key, value.ToHex());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, int[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, string[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, float[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, double[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, bool[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Vector2[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Vector3[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Vector4[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Quaternion[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, Color[] value)
	{
		form.AddField(key, value.ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<int> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<string> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<float> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<double> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<bool> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<Vector2> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<Vector3> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<Vector4> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<Quaternion> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, List<Color> value)
	{
		form.AddField(key, value.ToArray().ToStringArray());
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, byte[] value)
	{
		form.AddBinaryData(key, value);
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, byte[] value, string file)
	{
		form.AddBinaryData(key, value, file);
		return form;
	}
	
	public static WWWForm Add(this WWWForm form, string key, byte[] value, string file, string mimeType)
	{
		form.AddBinaryData(key, value, file, mimeType);
		return form;
	}
}
