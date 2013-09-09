using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionDebug
{
	private const string depth = "     ";
	
	public static string Depth(string str, int depth)
	{
		return str.Replace("*", ExtensionDebug.depth.Multiply(depth));
	}
	
	private static string d(string str, int depth)
	{
		return Depth(str, depth);
	}
	
	public static void Debug<T>(this T[] value)
	{	
		int depth = 0;
		
		object debug = 
			d("*(\n", depth) +
				
				d("*type: ", depth + 1) + typeof(T).ToString() + "\n" +
				d("*count: ", depth + 1);
		
				if (value != null)
				{
					debug += value.Length + "\n";
					
					if (value.Length > 0)
					{
						debug += d("*", depth + 1) + "{" + "\n";
				
						for (int i = 0; i < value.Length; i++)
							debug += d("*", depth + 2) + value[i] + ((i < value.Length - 1) ? "," : "") + "\n";
					
						debug += d("*", depth + 1) + "}" + "\n";
					}
				}
				else
					debug += "null\n";
		
			debug += d("*)\n", depth);
		
		UnityEngine.Debug.Log(debug);
	}
	
	public static void Debug<T>(this List<T> value)
	{	
		int depth = 0;
		
		object debug = 
			d("*(\n", depth) +
				
				d("*type: ", depth + 1) + typeof(T).ToString() + "\n" +
				d("*count: ", depth + 1);
				
				if (value != null)
				{
					debug += value.Count + "\n";
			
					if (value.Count > 0)
					{
						debug += d("*", depth + 1) + "{" + "\n";
				
						for (int i = 0; i < value.Count; i++)
							debug += d("*", depth + 2) + value[i] + ((i < value.Count - 1) ? "," : "") + "\n";
					
						debug += d("*", depth + 1) + "}" + "\n";
					}
				}
			else
				debug += "null\n";
		
			debug += d("*)\n", depth);
		
		UnityEngine.Debug.Log(debug);
	}
	
	public static void Debug<T, V>(this Dictionary<T,V> value)
	{	
		int depth = 0;
		
		object debug = 
			d("*(\n", depth) +
				
				d("*type: ", depth + 1) + typeof(T).ToString() + "\n" +
				d("*count: ", depth + 1);
				
				if (value != null)
				{
					debug += value.Count + "\n";
					
					if (value.Count > 0)
					{
						debug += d("*", depth + 1) + "{" + "\n";
			
						int i = 0;
						foreach(KeyValuePair<T, V> single in value)
						{
							debug += d("*", depth + 2) +
								"{ " + single.Key + ", " + single.Value + " }" +
							((i < value.Count - 1) ? "," : "") + "\n";
							i++;
						}
				
						debug += d("*", depth + 1) + "}" + "\n";
					}
				}
			else
				debug += "null\n";
		
			debug += d("*)\n", depth);
		
		UnityEngine.Debug.Log(debug);
	}
	
	public static void Debug(this GameObject value)
	{
		int depth = 0;
		
		object debug = 
				d("*(\n", depth) +
					d("*name: ", depth + 1);
					
					if (value != null)
					{
						debug += value.name + "\n" +
						d("*position: ", depth + 1) + value.transform.position + "\n" +
						d("*rotation: ", depth + 1) + value.transform.rotation + "\n" +
						d("*parent: ", depth + 1) + value.transform.parent + "\n" +
						d("*tag: ", depth + 1) + value.transform.tag + "\n";
			
						Component[] components = value.GetComponents(typeof(MonoBehaviour));
						if (components.Length > 0)
						{
							debug += d("*", depth + 1) + "{" + "\n";
				
							int i = 0;
							foreach(Component component in components)
							{
								debug += d("*", depth + 2) + component.ToString().
									Replace(value.name, "").Replace("(", "").Replace(")", "") +
								((i < components.Length - 1) ? "," : "") + "\n";
								i++;
							}
				
							debug += d("*", depth + 1) + "}" + "\n";
						}
					}
				else
					debug += "null\n";
		
			debug += d("*)\n", depth);
		
		UnityEngine.Debug.Log(debug);
	}
	
	public static void Debug(this Component value)
	{
		int depth = 0;
		
		object debug = 
			d("*(\n", depth) +
				d("*name: ", depth + 1);
		
				if (value != null)
				{
					debug += value.ToString() + "\n";
			
					System.Reflection.FieldInfo[] fields = value.GetType().GetFields(
					System.Reflection.BindingFlags.Public    |
					System.Reflection.BindingFlags.Instance  |
		            System.Reflection.BindingFlags.NonPublic |
		            System.Reflection.BindingFlags.Static);
			
					if (fields.Length > 0)
					{
						debug += d("*", depth + 1) + "{" + "\n";
				
						int i = 0;
						foreach(System.Reflection.FieldInfo field in fields)
						{
							debug += d("*", depth + 2) +
								"{ " + field.Name + ", '" + field.GetValue(value) + "' }" +
							((i < fields.Length - 1) ? "," : "") + "\n";
							i++;
						}
				
						debug += d("*", depth + 1) + "}" + "\n";
					}
				}
				else
					debug += "null\n";
		
			debug += d("*)\n", depth);
		
		UnityEngine.Debug.Log(debug);
	}
}
