using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class ExtensionString
{
	private static string API = "ExtensionString";
	
	public static bool IsNull(this string value)
	{
		return value == null;
	}
	
	public static bool IsEmpty(this string value)
	{
		return value.IsNull() || value == "";
	}
	
	public static string NotNull(this string value)
	{
		return value.IsNull() ? "" : value;
	}
	
	public static string Multiply(this string value, int count)
	{
		if (count <= 0)
			return "";
		
		string _value = value;
		for (int i = 1; i < count; i++)
			_value += value;
		
		return _value;
	}
	
	public static string Replace(this string value, Dictionary<string, string> replaceList)
	{
		if (replaceList.Count <= 0)
			return value;
		
		foreach (KeyValuePair<string, string> replace in replaceList)
			value = value.Replace(replace.Key, replace.Value);	
		
		return value;
	}
	
	public static string ToTimeText(this float seconds)
	{
		int n = 0;
		string outString = "";
    	if (seconds < 0) {
        	return "0 seconds";
    	}
    	if (seconds < 60) {
    	    n = (int)Mathf.Floor(seconds);
    	    outString = n + " second" + __fixCountStr(n);
    	    return outString;
    	}
    	if (seconds < 60 * 60) {
    	    n = (int)Mathf.Floor(seconds/60);
    	    outString = n + " minute" + __fixCountStr(n);
    	    return outString;
    	}
    	if (seconds < 60 * 60 * 24) {
    	    n = (int)Mathf.Floor(seconds/60/60);
    	    outString = n + " hour" + __fixCountStr(n);
    	    return outString;
    	}
    	if (seconds < 60 * 60 * 24 * 7) {
     	    n = (int)Mathf.Floor(seconds/60/60/24);
    	    outString = n + " day" + __fixCountStr(n);
    	    return outString;
    	}
    	if (seconds < 60 * 60 * 24 * 31) {
    	    n = (int)Mathf.Floor(seconds/60/60/24/7);
    	    outString = n + " week" + __fixCountStr(n);
    	    return outString;
    	}
    	if (seconds < 60 * 60 * 24 * 365) {
    	    n = (int)Mathf.Floor(seconds/60/60/24/31);
    	    outString = n + " month" + __fixCountStr(n);
    	    return outString;
    	}
    	n = (int)Mathf.Floor(seconds/60/60/24/365);
    	outString = n + " year" + __fixCountStr(n);
    	return outString;
	}
	
	public static string ToMD5(this string plain)
	{
		try
		{
			System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
			byte[] bytes = ue.GetBytes(plain);
		 
			System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(bytes);
		 
			string hashString = "";
		 
			for (int i = 0; i < hashBytes.Length; i++)
				hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
			
			return hashString.PadLeft(32, '0');
		}
	 	catch
		{
			Debug.LogWarning(API + ".ToMD5() cannot convert '" + plain + "' to MD5!");
			return plain;
		}
	}
	
	private static string __fixCountStr(int n)
	{
    	return n == 1 ? " " : "s";
	}
	
	public static bool IsValidEmailAddress(this string email)
    {
        return new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$").IsMatch(email);
    }
	
	public static string RemoveHTMLTags(this string value)
	{
		return value.
			Replace(@"<style>(.|\n)*?</style>", string.Empty).
			Replace(@"<xml>(.|\n)*?</xml>", string.Empty).
			Replace(@"<(.|\n)*?>", string.Empty).
			Replace(@"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", string.Empty);
	}
	
	public static string Replace(this string value, string oldValue, string newValue, bool regex)
	{
		if (!regex)
			return value.Replace(oldValue, newValue);
		
		return Regex.Replace(value, oldValue, newValue);
	}
	
	public static string Replace(this string value, string oldValue, int newValue)
	{
		return value.Replace(oldValue, newValue.ToString());
	}
	
	public static string Replace(this string value, int oldValue, string newValue)
	{
		return value.Replace(oldValue.ToString(), newValue);
	}
	
	public static string Replace(this string value, int oldValue, int newValue)
	{
		return value.Replace(oldValue.ToString(), newValue.ToString());
	}
	
	public static int Replace(this int value, string oldValue, string newValue)
	{
		return value.ToString().Replace(oldValue, newValue).ToInt32();
	}
	
	public static int Replace(this int value, string oldValue, int newValue)
	{
		return value.ToString().Replace(oldValue, newValue.ToString()).ToInt32();
	}
	
	public static int Replace(this int value, int oldValue, string newValue)
	{
		return value.ToString().Replace(oldValue.ToString(), newValue).ToInt32();
	}
	
	public static int Replace(this int value, int oldValue, int newValue)
	{
		return value.ToString().Replace(oldValue.ToString(), newValue.ToString()).ToInt32();
	}
}
