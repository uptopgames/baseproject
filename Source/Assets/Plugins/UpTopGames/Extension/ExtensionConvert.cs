using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionConvert
{
	public static bool ToBool(this string value)
	{
        string _value = value.ToLower();
        if (_value == "true")
            return true;
        else if (_value == "false")
            return false;
        else if (!(_value == "null" || _value == "nil"))
		{
			if (_value == "0" || _value == "1" || _value == "-1")
				return _value.ToInt32().ToBool();
            Debug.LogWarning("string.ToBool() '" + value + "' is not a boolean value!");
		}
        return default(bool);
	}
	
	public static bool ToBool(this int value)
	{
		if (value <= 0)
			return false;
		return true;
	}

    public static int ToInt32(this string value)
    {
        int intValue = default(int);
        bool result = int.TryParse(value, out intValue);
        if (result == false)
            Debug.LogWarning("string.ToInt32() '" + value + "' is not an interger value!");
        return intValue;
    }

    public static int ToInt32(this float value)
    {
        return (int)value;
    }

    public static int ToInt32(this double value)
    {
        return (int)value;
    }

    public static float ToFloat(this string value)
    {
        float floatValue = default(float);
		if(value.Contains("f")) 
		{
			value = value.Replace("f", "");
		}
        bool result = float.TryParse(value, out floatValue);
        if (result == false)
            Debug.LogWarning("string.ToFloat() '" + value + "' is not a float value!");
        return floatValue;
    }

    public static float ToFloat(this int value)
    {
        return (float)value;
    }

    public static float ToFloat(this double value)
    {
        return (float)value;
    }

    public static double ToDouble(this string value)
    {
        double doubleValue = default(double);
        bool result = double.TryParse(value, out doubleValue);
        if (result == false)
            Debug.LogWarning("string.ToInt32() '" + value + "' is not an double value!");
        return doubleValue;
    }

    public static double ToDouble(this float value)
    {
        return (double)value;
    }

    public static double ToDouble(this int value)
    {
        return (double)value;
    }

    public static System.Enum ToEnum(this string value, System.Type enumTypeOf, bool caseSensitive = true)
    {
        try
        {
            System.Enum result = (System.Enum)System.Enum.Parse(enumTypeOf, value, !caseSensitive);
            return result;
        }
        catch (System.ArgumentException)
        {
            Debug.Log("string.ToEnum() '" + value + "' is not a enum(" + enumTypeOf + ") value!");
        }
        return default(System.Enum);
    }

    public static System.Enum ToEnum(this int value, System.Type enumTypeOf, bool caseSensitive = true)
    {
        try
        {
            System.Enum result = (System.Enum)System.Enum.Parse(enumTypeOf, value.ToString(), !caseSensitive);
            return result;
        }
        catch (System.ArgumentException)
        {
            Debug.Log("string.ToEnum() '" + value + "' is not a enum(" + enumTypeOf + ") value!");
        }
        return default(System.Enum);
    }
	
	public static string ToHex(this Color32 color)
	{
		string format = "X2";
		
		return
			color.r.ToString(format) +
			color.g.ToString(format) +
			color.b.ToString(format);
	}
	
	public static string ToHex(this Color color)
	{
		return ((Color32)color).ToHex();
	}
	 
	public static Color ToColor(this string hex)
	{
		byte r = default(byte), g = default(byte), b = default(byte);
		
		try
		{
			r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		}
		catch
		{
			Debug.LogWarning("string.ToColor() '" + hex + "' is not an Hex value!");
		}
		
		return new Color32(r, g, b, 255);
	}
	
	public static char ToChar(this string value)
	{
		if (value.IsEmpty())
			return default(char);
		
		char[] _char = value.ToCharArray();
		
		if (_char.Length > 0)
			return _char[0];
		
		return default(char);
	}
	
	public static List<T> ToList<T>(this T[] array)
	{
		if (array != null)
		{
			List<T> list = new List<T>();
		
			foreach(T insert in array)
				list.Add(insert);
			
			return list;
		}
		
        return default(List<T>);
	}
}
