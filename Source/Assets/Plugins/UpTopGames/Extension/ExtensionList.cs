using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionList
{
	public static void ChangeIndex<T>(this List<T> list, T value, int newIndex)
	{	
		if (newIndex + 1 > list.Count || newIndex < 0)
			return;
		
		T oldIndexT = value;
		T newIndexT = list[newIndex];
		
		int newIndexId = list.IndexOf(newIndexT);
		
		list.Remove(value);
		list.Insert(newIndexId, oldIndexT);
	}
	
	public static void ChangeIndex<T>(this List<T> list, int oldIndex, int newIndex)
	{	
		if (oldIndex + 1 > list.Count || oldIndex < 0 ||
			newIndex + 1 > list.Count || newIndex < 0)
			return;
		
		T oldIndexT = list[oldIndex];
		T newIndexT = list[newIndex];
		
		int newIndexId = list.IndexOf(newIndexT);
		
		list.Remove(oldIndexT);
		list.Insert(newIndexId, oldIndexT);
	}
	
	public static void Merge<T>(this List<T> value, List<T> list)
	{	
		foreach(T insert in value)
			list.Add(insert);
	}
	
	public static List<T> Clone<T>(this List<T> value, List<T> list)
	{	
		return new List<T>(value);
	}
}
