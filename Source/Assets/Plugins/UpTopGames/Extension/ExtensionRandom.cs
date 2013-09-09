using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionRandom
{
	private static string API = "ExtensionRandom";
	
	private const string
		ERROR_NULL_ARRAY = "You need pass a not null array!",
		ERROR_EMPTY_ARRAY = "You need pass a not empty array!",
	
		ERROR_NULL_LIST = "You need pass a not null list!",
		ERROR_EMPTY_LIST = "You need pass a not empty list!";
	
	private static bool Error(string method, string parameter, string error)
	{
		Debug.LogWarning(API + "." + method + "(" +
			(
				(error == ERROR_NULL_ARRAY || error == ERROR_EMPTY_ARRAY)
				 ? parameter + "[]"
				: "List<" + parameter + ">"
			)
			+ ") " + error
		);
		return false;
	}
	
	public static T Choose<T>(this T[] choose)
	{
		if (choose == null)
		{
			Error("Choose", typeof(T).ToString(), ERROR_NULL_ARRAY);
			return default(T);
		}
		
		if (choose.Length > 0)
            return choose[UnityEngine.Random.Range(0, choose.Length - 1)];
		
		Error("Choose", typeof(T).ToString(), ERROR_EMPTY_ARRAY);
        return default(T);	
	}
	
	public static T Choose<T>(this List<T> choose)
	{
		if (choose == null)
		{
			Error("Choose", typeof(T).ToString(), ERROR_NULL_LIST);
			return default(T);
		}
			
		if (choose.Count > 0)
            return choose[UnityEngine.Random.Range(0, choose.Count - 1)];
		
		Error("Choose", typeof(T).ToString(), ERROR_EMPTY_LIST);
        return default(T);
	}
	
	public static List<T> Random<T>(this List<T> list)
	{
		if (list == null || list.Count <= 0)
			return list;
		
		List<T> newList = new List<T>();
		
		while (list.Count > 0)
		{
			int choose = UnityEngine.Random.Range(0, list.Count);
			newList.Add(list[choose]);
			list.RemoveAt(choose);
		}
		
		foreach(T value in newList)
			list.Add(value);
		
		return list;
	}
	
	public static Dictionary<Key, Value> Random<Key, Value>(this Dictionary<Key, Value> list)
	{
		if (list == null || list.Count <= 0)
			return list;
		
		Dictionary<Key, Value> newList = new Dictionary<Key, Value>();
		
		while (list.Count > 0)
		{
			List<Key> keyList = new List<Key>(list.Keys);
			Key choose = keyList.Choose();
			
			newList.Add(choose, list[choose]);
			list.Remove(choose);
		}
		
		foreach (KeyValuePair<Key, Value> value in newList)
			list.Add(value.Key, value.Value);
		
		return list;
	}	
}
