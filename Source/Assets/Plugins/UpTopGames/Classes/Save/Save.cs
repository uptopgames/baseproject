using UnityEngine;
using System.Collections;

public class Save
{
	private static SaveManager _manager;
	
    private static string pre = "k:";
	
	public static SaveManager manager
    {
		get
		{
			if (Save._manager != null) return Save._manager;
			
			GameObject obj = Flow.config;
	
			Save._manager = (!obj.GetComponent<SaveManager>())? obj.AddComponent<SaveManager>(): obj.GetComponent<SaveManager>();
			
			return Save._manager;
		}
	}
	
    public static bool SaveAll()
    {
        PlayerPrefs.Save();
        manager.SaveAll();
        return true;
    }

    public static bool DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        manager.DeleteAll();
        return true;
    }
	
	public static bool DebugAll()
	{
        manager.DebugAll();
        return true;
    }

    public static bool HasKey(string key)
    {
		if (manager.HasKey(key))
			return true;

        if (PlayerPrefs.HasKey(pre + key + ":value"))
            return true;

        return false;
    }
   
    //string
    public static string Set(string key, string value, bool save)
    {
		if (Info.IsFastSave() && manager.GetString(key) != value) manager.Set(key, value);

		if (Info.IsFastSave() && !save) return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "string") Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "string");
        PlayerPrefs.SetString(pre + key + ":value", value);

        return value;
    }

	public static string Set(string key, string value)
	{
        return Set(key, value, false);
    }

    public static string GetString(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetString(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "string") {
			string _value = PlayerPrefs.GetString(pre + key + ":value");
			manager.Set(key, _value);
            return _value;
		}
        else
        {
            if (type != "")
                Debug.LogError("Save.GetString() Key '" + key + "' (" + type + ") is not a string value.");
            return default(string);
        }
    }

    public static string Add(string key, string value, bool save)
    {
        return Set(key, GetString(key) + value, save);
    }
	public static string Add(string key, string value)
    {
        return Set(key, GetString(key) + value, false);
    }

    //int
    public static int Set(string key, int value, bool save)
    {
		if (Info.IsFastSave() && manager.GetInt(key) != value)
			manager.Set(key, value);

		if (Info.IsFastSave() && !save)
            return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "int")
            Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "int");
        PlayerPrefs.SetInt(pre + key + ":value", value);

        return value;
    }

	public static int Set(string key, int value)
	{
        return Set(key, value, false);
    }

    public static int GetInt(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetInt(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "int")
        {
			int _value = PlayerPrefs.GetInt(pre + key + ":value");
			manager.Set(key, _value);
            return _value;
		}
        else
        {
            if (type != "")
                Debug.LogError("Save.GetInt() Key '" + key + "' (" + type + ") is not a int value.");
            return default(int);
        }
    }

    public static int Add(string key, int value, bool save)
    {
        return Set(key, GetInt(key) + value, save);
    }

	public static int Add(string key, int value)
    {
        return Set(key, GetInt(key) + value, false);
    }

    //float
    public static float Set(string key, float value, bool save)
    {
		if (Info.IsFastSave() && manager.GetFloat(key) != value)
			manager.Set(key, value);

		if (Info.IsFastSave() && !save)
            return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "float")
            Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "float");
        PlayerPrefs.SetFloat(pre + key + ":value", value);

        return value;
    }
	public static float Set(string key, float value)
	{
        return Set(key, value, false);
    }

    public static float GetFloat(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetFloat(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "float")
        {
			float _value = PlayerPrefs.GetFloat(pre + key + ":value");
			Set (key, _value);
            return _value;
		}
        else
        {
            if (type != "")
                Debug.LogError("Save.GetFloat() Key '" + key + "' (" + type + ") is not a float value.");
            return default(float);
        }
    }

    public static float Add(string key, float value, bool save)
    {
        return Set(key, GetFloat(key) + value, save);
    }

	public static float Add(string key, float value)
    {
        return Set(key, GetFloat(key) + value, false);
    }

    //bool
    public static bool Set(string key, bool value, bool save)
    {
		if (Info.IsFastSave() && manager.GetBool(key) != value)
			manager.Set(key, value);

		if (Info.IsFastSave() && !save)
            return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "bool")
            Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "bool");

        if (value)
            PlayerPrefs.SetInt(pre + key + ":value", 1);
        else PlayerPrefs.SetInt(pre + key + ":value", 0);

        return true;
    }

	public static bool Set(string key, bool value)
	{
        return Set(key, value, false);
    }

    public static bool GetBool(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetBool(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "bool")
        {
            if (PlayerPrefs.GetInt(pre + key + ":value") == 1)
			{
                Set(key, true);
                return true;
            }
			manager.Set(key, false);
            return false;
        }
        else
        {
            if (type != "")
                Debug.LogError("Save.GetBool() Key '" + key + "' (" + type + ") is not a bool value.");
            return default(bool);
        }
    }

    //double
    public static double Set(string key, double value, bool save)
    {
		if (Info.IsFastSave() && manager.GetDouble(key) != value)
			manager.Set(key, value);

		if (Info.IsFastSave() && !save)
            return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "double")
            Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "double");
        PlayerPrefs.SetString(pre + key + ":value", value.ToString());

        return value;
    }
	public static double Set(string key, double value)
	{
        return Set(key, value, false);
    }

    public static double GetDouble(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetDouble(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "double")
        {
			double _value = double.Parse(PlayerPrefs.GetString(pre + key + ":value"));
			manager.Set (key, _value); return _value;
		}
        else
        {
            if (type != "")
                Debug.LogError("Save.GetDouble() Key '" + key + "' (" + type + ") is not a double value.");
            return default(double);
        }
    }

    public static double Add(string key, double value, bool save)
    {
        return Set(key, GetDouble(key) + value, save);
    }

	public static double Add(string key, double value)
    {
        return Set(key, GetDouble(key) + value, false);
    }

    //Vector2
    public static Vector2 Set(string key, Vector2 value, bool save)
    {
		if (Info.IsFastSave() && manager.GetVector2(key) != value)
			manager.Set(key, value);

		if (Info.IsFastSave() && !save)
            return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "Vector2")
            Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "Vector2");
        PlayerPrefs.SetFloat(pre + key + ":value:x", value.x);
        PlayerPrefs.SetFloat(pre + key + ":value:y", value.y);

        return value;
    }

	public static Vector2 Set(string key, Vector2 value)
	{
        return Set(key, value, false);
    }

    public static Vector2 GetVector2(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetVector2(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "Vector2")
        {
			Vector2 _value = new Vector2(PlayerPrefs.GetFloat(pre + key + ":value:x"),
                PlayerPrefs.GetFloat(pre + key + ":value:y"));
			manager.Set(key, _value); return _value;
		}
        else
        {
            if (type != "")
                Debug.LogError("Save.GetVector2() Key '" + key + "' (" + type + ") is not a Vector2 value.");
            return default(Vector2);
        }
    }

    public static Vector2 Add(string key, Vector2 value, bool save)
    {
        return Set(key, GetVector2(key) + value, save);
    }

	public static Vector2 Add(string key, Vector2 value)
    {
        return Set(key, GetVector2(key) + value, false);
    }

    //Vector3
    public static Vector3 Set(string key, Vector3 value, bool save)
    {
		if (Info.IsFastSave() && manager.GetVector3(key) != value)
			manager.Set(key, value);

		if (Info.IsFastSave() && !save)
            return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "Vector3")
            Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "Vector3");
        PlayerPrefs.SetFloat(pre + key + ":value:x", value.x);
        PlayerPrefs.SetFloat(pre + key + ":value:y", value.y);
        PlayerPrefs.SetFloat(pre + key + ":value:z", value.z);

        return value;
    }
	public static Vector3 Set(string key, Vector3 value)
	{ return Set(key, value, false); }

    public static Vector3 GetVector3(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetVector3(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "Vector3")
        {
			Vector3  _value = new Vector3(PlayerPrefs.GetFloat(pre + key + ":value:x"),
                PlayerPrefs.GetFloat(pre + key + ":value:y"), PlayerPrefs.GetFloat(pre + key + ":value:z"));
			manager.Set(key, _value);
            return _value;	
		}
        else
        {
            if (type != "")
                Debug.LogError("Save.GetVector3() Key '" + key + "' (" + type + ") is not a Vector3 value.");
            return default(Vector3);
        }
    }

    public static Vector3 Add(string key, Vector3 value, bool save)
    {
        return Set(key, GetVector3(key) + value, save);
    }

	public static Vector3 Add(string key, Vector3 value)
    {
        return Set(key, GetVector3(key) + value, false);
    }

    //Vector4
    public static Vector4 Set(string key, Vector4 value, bool save)
    {
		if (Info.IsFastSave() && manager.GetVector4(key) != value)
			manager.Set(key, value);

		if (Info.IsFastSave() && !save)
            return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "Vector4")
            Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "Vector4");
        PlayerPrefs.SetFloat(pre + key + ":value:x", value.x);
        PlayerPrefs.SetFloat(pre + key + ":value:y", value.y);
        PlayerPrefs.SetFloat(pre + key + ":value:z", value.z);
        PlayerPrefs.SetFloat(pre + key + ":value:w", value.w);

        return value;
    }

	public static Vector4 Set(string key, Vector4 value)
	{
        return Set(key, value, false);
    }

    public static Vector4 GetVector4(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetVector4(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "Vector4")
        {
			Vector4 _value = new Vector4(PlayerPrefs.GetFloat(pre + key + ":value:x"),
                PlayerPrefs.GetFloat(pre + key + ":value:y"), PlayerPrefs.GetFloat(pre + key + ":value:z"),
                    PlayerPrefs.GetFloat(pre + key + ":value:w"));
			manager.Set(key, _value);
            return _value;
		}
        else
        {
            if (type != "")
                Debug.LogError("Save.GetVector4() Key '" + key + "' (" + type + ") is not a Vector4 value.");
            return default(Vector4);
        }
    }

    public static Vector4 Add(string key, Vector4 value, bool save)
    {
        return Set(key, GetVector4(key) + value, save);
    }

	public static Vector4 Add(string key, Vector4 value)
    {
        return Set(key, GetVector4(key) + value, false);
    }

    //Quaternion
    public static Quaternion Set(string key, Quaternion value, bool save)
    {
		if (Info.IsFastSave() && manager.GetQuaternion(key) != value)
			manager.Set(key, value);

		if (Info.IsFastSave() && !save)
            return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "Quaternion")
            Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "Quaternion");
        PlayerPrefs.SetFloat(pre + key + ":value:x", value.x);
        PlayerPrefs.SetFloat(pre + key + ":value:y", value.y);
        PlayerPrefs.SetFloat(pre + key + ":value:z", value.z);
        PlayerPrefs.SetFloat(pre + key + ":value:w", value.w);

        return value;
    }
	public static Quaternion Set(string key, Quaternion value)
	{
        return Set(key, value, false);
    }

    public static Quaternion GetQuaternion(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetQuaternion(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "Quaternion")
        {
			Quaternion _value = new Quaternion(PlayerPrefs.GetFloat(pre + key + ":value:x"),
                PlayerPrefs.GetFloat(pre + key + ":value:y"), PlayerPrefs.GetFloat(pre + key + ":value:z"),
                    PlayerPrefs.GetFloat(pre + key + ":value:w"));
			manager.Set(key, _value); return _value;
		}
        else
        {
            if (type != "")
                Debug.LogError("Save.Quaternion() Key '" + key + "' (" + type + ") is not a Quaternion value.");
            return default(Quaternion);
        }
    }

    //Color
    public static Color Set(string key, Color value, bool save)
    {
		if (Info.IsFastSave() && manager.GetColor(key) != value)
			manager.Set(key, value);

		if (Info.IsFastSave() && !save)
            return value;

        if (PlayerPrefs.GetString(pre + key + ":type") != "Color")
            Delete(key);

        PlayerPrefs.SetString(pre + key + ":type", "Color");
        PlayerPrefs.SetFloat(pre + key + ":value:r", value.r);
        PlayerPrefs.SetFloat(pre + key + ":value:g", value.g);
        PlayerPrefs.SetFloat(pre + key + ":value:b", value.b);
        PlayerPrefs.SetFloat(pre + key + ":value:a", value.a);

        return value;
    }
	public static Color Set(string key, Color value)
	{
        return Set(key, value, false);
    }

    public static Color GetColor(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			return manager.GetColor(key);

        string type = PlayerPrefs.GetString(pre + key + ":type");
        if (type == "Color")
        {
            Color _value = new Color(PlayerPrefs.GetFloat(pre + key + ":value:r"),
                PlayerPrefs.GetFloat(pre + key + ":value:g"), PlayerPrefs.GetFloat(pre + key + ":value:b"),
                    PlayerPrefs.GetFloat(pre + key + ":value:a"));
			manager.Set(key, _value);
            return _value;
		}
        else
        {
            if (type != "")
                Debug.LogError("Save.Color() Key '" + key + "' (" + type + ") is not a Color value.");
            return default(Color);
        }
    }

    public static Color Add(string key, Color value, bool save)
    {
        return Set(key, GetColor(key) + value, save);
    }

	public static Color Add(string key, Color value)
    {
        return Set(key, GetColor(key) + value, false);
    }
	
	// ARRAYS
	public static string[] Set(string key, string[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static string[] Set(string key, string[] value)
	{
		return Set(key, value, false);
	}
	
	public static string[] GetStringArray(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayString();
		
		Debug.LogError("Save.GetStringArray() Key '" + key + "' is not an Array of String value.");
		return default(string[]);
	}
	
	public static int[] Set(string key, int[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static int[] Set(string key, int[] value)
	{
		return Set(key, value, false);
	}
	
	public static int[] GetIntArray(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayInt32();
		
		Debug.LogError("Save.GetIntArray() Key '" + key + "' is not an Array of Int value.");
		return default(int[]);
	}
	
	public static float[] Set(string key, float[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static float[] Set(string key, float[] value)
	{
		return Set(key, value, false);
	}
	
	public static float[] GetFloatArray(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayFloat();
		
		Debug.LogError("Save.GetFloatArray() Key '" + key + "' is not an Array of Float value.");
		return default(float[]);
	}
	
	public static double[] Set(string key, double[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static double[] Set(string key, double[] value)
	{
		return Set(key, value, false);
	}
	
	public static double[] GetDoubleArray(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayDouble();
		
		Debug.LogError("Save.GetDoubleArray() Key '" + key + "' is not an Double of Float value.");
		return default(double[]);
	}
	
	public static bool[] Set(string key, bool[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static bool[] Set(string key, bool[] value)
	{
		return Set(key, value, false);
	}
	
	public static bool[] GetBoolArray(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayBool();
		
		Debug.LogError("Save.GetBoolArray() Key '" + key + "' is not an Bool of Float value.");
		return default(bool[]);
	}
	
	public static Vector2[] Set(string key, Vector2[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static Vector2[] Set(string key, Vector2[] value)
	{
		return Set(key, value, false);
	}
	
	public static Vector2[] GetVector2Array(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayVector2();
		
		Debug.LogError("Save.GetVector2Array() Key '" + key + "' is not an Vector2 of Float value.");
		return default(Vector2[]);
	}
	
	public static Vector3[] Set(string key, Vector3[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static Vector3[] Set(string key, Vector3[] value)
	{
		return Set(key, value, false);
	}
	
	public static Vector3[] GetVector3Array(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayVector3();
		
		Debug.LogError("Save.GetVector3Array() Key '" + key + "' is not an Vector3 of Float value.");
		return default(Vector3[]);
	}
	
	public static Vector4[] Set(string key, Vector4[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static Vector4[] Set(string key, Vector4[] value)
	{
		return Set(key, value, false);
	}
	
	public static Vector4[] GetVector4Array(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayVector4();
		
		Debug.LogError("Save.GetVector4Array() Key '" + key + "' is not an Vector4 of Float value.");
		return default(Vector4[]);
	}
	
	public static Quaternion[] Set(string key, Quaternion[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static Quaternion[] Set(string key, Quaternion[] value)
	{
		return Set(key, value, false);
	}
	
	public static Quaternion[] GetQuaternionArray(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayQuaternion();
		
		Debug.LogError("Save. GetQuaternionArray() Key '" + key + "' is not an Quaternion of Float value.");
		return default(Quaternion[]);
	}
	
	public static Color[] Set(string key, Color[] value, bool save)
	{
		Save.Set(key, value.ToStringArray(), save);
		return value;
	}
	
	public static Color[] Set(string key, Color[] value)
	{
		return Set(key, value, false);
	}
	
	public static Color[] GetColorArray(string key)
	{
		string array = Save.GetString(key);
		if (array.IsArray())
			return array.ToArrayColor();
		
		Debug.LogError("Save.GetColornArray() Key '" + key + "' is not an Color of Float value.");
		return default(Color[]);
	}
	
    //Delete Key
    public static void Delete(string key)
    {
		if (Info.IsFastSave() && manager.HasKey(key))
			manager.Delete(key);

        if (HasKey(key))
        {
            string type = PlayerPrefs.GetString(pre + key + ":type");
            if (type == "Vector2" || type == "Vector3" || type == "Vector4" || type == "Quaternion")
            {
                PlayerPrefs.DeleteKey(pre + key + ":value:x"); // Vector2
                PlayerPrefs.DeleteKey(pre + key + ":value:y"); // Vector2
                PlayerPrefs.DeleteKey(pre + key + ":value:z"); // Vector3
                PlayerPrefs.DeleteKey(pre + key + ":value:w"); // Quaternion
            }
            else if (type == "Color")
            {
                PlayerPrefs.DeleteKey(pre + key + ":value:r"); // Color
                PlayerPrefs.DeleteKey(pre + key + ":value:g"); // Color
                PlayerPrefs.DeleteKey(pre + key + ":value:b"); // Color
                PlayerPrefs.DeleteKey(pre + key + ":value:a"); // Alpha
            }
            PlayerPrefs.DeleteKey(pre + key + ":value");
            PlayerPrefs.DeleteKey(pre + key + ":type");
        }
    }

}