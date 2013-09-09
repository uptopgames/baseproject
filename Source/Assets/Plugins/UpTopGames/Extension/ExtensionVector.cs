using UnityEngine;
using System.Collections;

public static class ExtensionVector
{
	public static Vector2 ToVector2(this string vector)
	{
		string fix = __fixVectorSrt(vector);
		string[] split = fix.Split(',');
		
		Vector2 _vector = Vector2.zero;
		if (split.Length > 0)
			_vector.x = split[0].ToFloat();
		if (split.Length > 1)
			_vector.y = split[1].ToFloat();
		
		return _vector;
	}
	
	public static Vector3 ToVector3(this string vector)
	{
		string fix = __fixVectorSrt(vector);
		string[] split = fix.Split(',');
		
		Vector3 _vector = Vector3.zero;
		if (split.Length > 0)
			_vector.x = split[0].ToFloat();
		if (split.Length > 1)
			_vector.y = split[1].ToFloat();
		if (split.Length > 2)
			_vector.z = split[2].ToFloat();
		
		return _vector;
	}
	
	public static Vector4 ToVector4(this string vector)
	{
		string fix = __fixVectorSrt(vector);
		string[] split = fix.Split(',');
		
		Vector4 _vector = Vector4.zero;
		if (split.Length > 0)
			_vector.x = split[0].ToFloat();
		if (split.Length > 1)
			_vector.y = split[1].ToFloat();
		if (split.Length > 2)
			_vector.z = split[2].ToFloat();
		if (split.Length > 3)
			_vector.w = split[3].ToFloat();
		
		return _vector;
	}
	
	public static Quaternion ToQuaternion(this string quaternion)
	{
		string fix = __fixVectorSrt(quaternion);
		string[] split = fix.Split(',');
		
		Quaternion _quaternion = Quaternion.identity;
		if (split.Length > 0)
			_quaternion.x = split[0].ToFloat();
		if (split.Length > 1)
			_quaternion.y = split[1].ToFloat();
		if (split.Length > 2)
			_quaternion.z = split[2].ToFloat();
		if (split.Length > 3)
			_quaternion.w = split[3].ToFloat();
		
		return _quaternion;
	}
	
	public static Vector2 SetX(this Vector2 vect, float x) { vect.x = x; return vect; }
    public static Vector2 SetY(this Vector2 vect, float y) { vect.y = y; return vect; }

    public static Vector2 AddX(this Vector2 vect, float x) { vect.x += x; return vect; }
    public static Vector2 AddY(this Vector2 vect, float y) { vect.y += y; return vect; }
	
    public static Vector3 SetX(this Vector3 vect, float x) { vect.x = x; return vect; }
    public static Vector3 SetY(this Vector3 vect, float y) { vect.y = y; return vect; }
    public static Vector3 SetZ(this Vector3 vect, float z) { vect.z = z; return vect; }

    public static Vector3 AddX(this Vector3 vect, float x) { vect.x += x; return vect; }
    public static Vector3 AddY(this Vector3 vect, float y) { vect.y += y; return vect; }
    public static Vector3 AddZ(this Vector3 vect, float z) { vect.z += z; return vect; }
	
	public static Vector4 SetX(this Vector4 vect, float x) { vect.x = x; return vect; }
    public static Vector4 SetY(this Vector4 vect, float y) { vect.y = y; return vect; }
    public static Vector4 SetZ(this Vector4 vect, float z) { vect.z = z; return vect; }
	public static Vector4 SetW(this Vector4 vect, float w) { vect.w = w; return vect; }
	
    public static Vector4 AddX(this Vector4 vect, float x) { vect.x += x; return vect; }
    public static Vector4 AddY(this Vector4 vect, float y) { vect.y += y; return vect; }
    public static Vector4 AddZ(this Vector4 vect, float z) { vect.z += z; return vect; }
	public static Vector4 AddW(this Vector4 vect, float w) { vect.w += w; return vect; }

    public static Quaternion SetX(this Quaternion quat, float x) { quat.x = x; return quat; }
    public static Quaternion SetY(this Quaternion quat, float y) { quat.y = y; return quat; }
    public static Quaternion SetZ(this Quaternion quat, float z) { quat.z = z; return quat; }
    public static Quaternion SetW(this Quaternion quat, float w) { quat.w = w; return quat; }

    public static Quaternion AddX(this Quaternion quat, float x) { quat.x += x; return quat; }
    public static Quaternion AddY(this Quaternion quat, float y) { quat.y += y; return quat; }
    public static Quaternion AddZ(this Quaternion quat, float z) { quat.z += z; return quat; }
    public static Quaternion AddW(this Quaternion quat, float w) { quat.w += w; return quat; }
	
	private static string __fixVectorSrt(this string vector)
	{
		return vector.Replace("(", "").Replace(")","");
	}
}
