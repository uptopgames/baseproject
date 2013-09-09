using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionArray
{
	
	public static void Add<T>(this T[] value, T item, ref T[] _out)
	{
		List<T> tmp = _out.ToList();
		tmp.Add(item);
		_out = tmp.ToArray();
	}
	
	public static void AddRange<T>(this T[] value, IEnumerable<T> collection, ref T[] _out)
	{
		List<T> tmp = _out.ToList();
		tmp.AddRange(collection);
		_out = tmp.ToArray();
	}
	
	public static void Clear<T>(this T[] value, ref T[] _out)
	{
		List<T> tmp = _out.ToList();
		tmp.Clear();
		_out = tmp.ToArray();
	}
	
	public static bool Contains<T>(this T[] value, T item, ref T[] _out)
	{
		return _out.ToList().Contains(item);
	}
	
	public static int IndexOf<T>(this T[] value, T item, ref T[] _out)
	{
		return _out.ToList().IndexOf(item);
	}
	
	public static void Insert<T>(this T[] value, int index, T item, ref T[] _out)
	{
		List<T> tmp = _out.ToList();
		tmp.Insert(index, item);
		_out = tmp.ToArray();
	}
	
	public static void InsertRange<T>(this T[] value, int index, IEnumerable<T> collection, ref T[] _out)
	{
		List<T> tmp = _out.ToList();
		tmp.InsertRange(index, collection);
		_out = tmp.ToArray();
	}
	
	public static int LastIndexOf<T>(this T[] value, T item, ref T[] _out)
	{
		return _out.ToList().LastIndexOf(item);
	}
	
	public static void Remove<T>(this T[] value, T item, ref T[] _out)
	{
		List<T> tmp = _out.ToList();
		tmp.Remove(item);
		_out = tmp.ToArray();
	}
	
	public static void Remove<T>(this T[] value, int index, ref T[] _out)
	{
		List<T> tmp = _out.ToList();
		tmp.RemoveAt(index);
		_out = tmp.ToArray();
	}
	
	public static void RemoveRange<T>(this T[] value, int index, int count, ref T[] _out)
	{
		List<T> tmp = _out.ToList();
		tmp.RemoveRange(index, count);
		_out = tmp.ToArray();
	}
	
	public static bool IsArray(this string value)
	{
		return value.StartsWith(__fixArrayStr(ArrayString.Init));
	}
	
	public static string ToStringArray(this string[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(string value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static string[] ToArrayString(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(string[]);
		
		string[] newArray = new string[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i];
		
		return newArray;
	}
	
	public static string ToStringArray(this int[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(int value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static int[] ToArrayInt32(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(int[]);
		
		int[] newArray = new int[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i].ToInt32();
		
		return newArray;
	}
	
	public static string ToStringArray(this float[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(float value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static float[] ToArrayFloat(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(float[]);
		
		float[] newArray = new float[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i].ToFloat();
		
		return newArray;
	}
	
	public static string ToStringArray(this double[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(double value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static double[] ToArrayDouble(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(double[]);
		
		double[] newArray = new double[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i].ToDouble();
		
		return newArray;
	}
	
	public static string ToStringArray(this bool[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(bool value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static bool[] ToArrayBool(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(bool[]);
		
		bool[] newArray = new bool[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i].ToBool();
		
		return newArray;
	}
	
	public static string ToStringArray(this Vector2[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Vector2 value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static Vector2[] ToArrayVector2(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(Vector2[]);
		
		Vector2[] newArray = new Vector2[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i].ToVector2();
		
		return newArray;
	}
	
	public static string ToStringArray(this Vector3[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Vector3 value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static Vector3[] ToArrayVector3(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(Vector3[]);
		
		Vector3[] newArray = new Vector3[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i].ToVector3();
		
		return newArray;
	}
	
	public static string ToStringArray(this Vector4[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Vector4 value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static Vector4[] ToArrayVector4(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(Vector4[]);
		
		Vector4[] newArray = new Vector4[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i].ToVector4();
		
		return newArray;
	}
	
	public static string ToStringArray(this Quaternion[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Quaternion value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static Quaternion[] ToArrayQuaternion(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(Quaternion[]);
		
		Quaternion[] newArray = new Quaternion[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i].ToQuaternion();
		
		return newArray;
	}
	
	public static string ToStringArray(this Color[] array)
	{
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Color value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToHex();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static Color[] ToArrayColor(this string array)
	{
		array = __fixArrayStr(array, ArrayString.Fix);
		
		string[] split = array.Split(__fixArrayStr(ArrayString.Split).ToChar());
		
		if (split == null || split.Length <= 0)
			return default(Color[]);
		
		Color[] newArray = new Color[split.Length];
		
		for(int i = 0; i < split.Length; i++)
			newArray[i] = split[i].ToColor();
		
		return newArray;
	}
	
	public static string ToStringList(this List<string> list)
	{
		string[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(string value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static string ToStringList(this List<int> list)
	{
		int[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(int value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static string ToStringList(this List<float> list)
	{
		float[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(float value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static string ToStringList(this List<double> list)
	{
		double[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(double value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static string ToStringList(this List<bool> list)
	{
		bool[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(bool value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static string ToStringList(this List<Vector2> list)
	{
		Vector2[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Vector2 value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static string ToStringList(this List<Vector3> list)
	{
		Vector3[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Vector3 value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static string ToStringList(this List<Vector4> list)
	{
		Vector4[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Vector4 value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}
	
	public static string ToStringList(this List<Quaternion> list)
	{
		Quaternion[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Quaternion value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToString();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}

	public static string ToStringList(this List<Color> list)
	{
		Color[] array = list.ToArray();
		string strArray = __fixArrayStr(ArrayString.Init);
		
		foreach(Color value in array)
			strArray += __fixArrayStr(ArrayString.Split) + value.ToHex();

		strArray = __fixArrayStr(strArray, ArrayString.End);
		
		return strArray;
	}

	
	
	private enum ArrayString { Init, Split, End, Fix };
		
	private static string __fixArrayStr(ArrayString method)
	{
		return __fixArrayStr("", method);
	}
	
	private static string __fixArrayStr(string param, ArrayString method)
	{
		if (method == ArrayString.Init)
			return "array:";
		else if (method == ArrayString.Split)
			return ";";
		else if (method == ArrayString.End)
			return param.Replace("array:;", "array:");
		else if (method == ArrayString.Fix)
			return param.Replace("array:", "");
		return default(string);
	}
}
