using UnityEngine;
using System.Collections;
using CodeTitans.JSon;

public class GameToken
{
	public static void save(IJSonObject data)
	{
		if (!Save.HasKey(PlayerPrefsKeys.TOKEN_EXPIRATION.ToString()))
		{
			Save.Set(PlayerPrefsKeys.TOKEN_EXPIRATION.ToString(), data["expiration"].StringValue,true);
			Save.Set(PlayerPrefsKeys.TOKEN.ToString(), data["token"].StringValue,true);
		}
		
		else
		{
			System.DateTime old_date = System.DateTime.Parse(Save.GetString(PlayerPrefsKeys.TOKEN_EXPIRATION.ToString()));
			System.DateTime new_date = System.DateTime.Parse(data["expiration"].StringValue);
			
			if (new_date > old_date) 
			{
				Save.Set(PlayerPrefsKeys.TOKEN.ToString(), data["token"].StringValue,true);
			}
		}
	}
}
