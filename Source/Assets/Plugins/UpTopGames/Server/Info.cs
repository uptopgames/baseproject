using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Info
{
    public enum Platform {
        Unknown, Android, iPhone,
        WebPlayer, FlashPlayer, GoogleNativeClient,
        Windows, Linux, MacOS,
        PlayStation3, NintendoWii, Xbox360
    };

    public static Platform GetPlatform()
    {
        #if UNITY_NACL
            return Platform.GoogleNativeClient;
        #elif UNITY_WEBPLAYER
            return Platform.WebPlayer;
        #elif UNITY_FLASH
            return Platform.FlashPlayer;
        #elif UNITY_ANDROID
            return Platform.Android;
        #elif UNITY_IPHONE
            return Platform.iPhone;
        #elif UNITY_PS3
            return Platform.PlayStation3;
        #elif UNITY_XBOX360
            return Platform.Xbox360;
        #elif UNITY_WII
            return Platform.NintendoWii;
        #elif UNITY_STANDALONE_WIN
            return Platform.Windows;
        #elif UNITY_STANDALONE_LINUX
            return Platform.Linux;
        #elif UNITY_STANDALONE_OSX
            return Platform.MacOS;
        #else
            return Platform.Unknown;
        #endif
    }

	public enum AppType {
        Free, Pay, Custom1, Custom2, Custom3
    };

	public static class Enum
    {
		public enum DebugType {
            DontShow, ShowOnEditor, ShowOnDebug, ShowOnRelease
        };
		public enum DeleteType {
            DontDelete, DeleteOnEditor, DeleteOnDebug, DeleteOnRelease
        };
		public enum SandboxType {
            NeverEnable, EnableOnEditor, EnableOnDebug, EnableOnRelease
        }	
	}
	
	private static ConfigManager _manager;
	private static ConfigManager manager
	{
		get
		{
			if (_manager != null)
				return _manager;
			
			GameObject obj = GameObject.Find(ConfigManager.API);
			if (obj == null)
	        {
	            Debug.LogWarning("Prefab '" + ConfigManager.API + "' not found.");
	            return default(ConfigManager);
	        }
	
			ConfigManager config = obj.GetComponent<ConfigManager>();
			if (obj == null)
	        {
	            Debug.LogWarning("Component '" + ConfigManager.API + "' not found.");
	            return default(ConfigManager);
	        }
			_manager = config;
			
			return _manager;
		}
		
	}
	
	
    public static bool IsRelease()
    {
        return !Debug.isDebugBuild;
    }

    public static bool IsDevelopment()
    {
        return !IsRelease();
    }

    public static bool IsEditor()
    {
        return Application.isEditor;
    }

    public static bool IsPlatform(Platform platform)
    {
        return GetPlatform() == platform;
    }

    public static bool IsWeb()
    {
        return GetPlatform() == Platform.WebPlayer || GetPlatform() == Platform.FlashPlayer ||
            GetPlatform() == Platform.GoogleNativeClient;
    }
	
	public static bool HasConnection(bool dialog = false)
	{
		bool connection = Application.internetReachability != NetworkReachability.NotReachable;
		
		// Up Top Fix Me
		//if (dialog && !connection)
			//GameGUI.game_native.showMessage(Info.name,"There's is no internet connection. Please, try again later");
		
		return connection;
	}

	public static bool IsSandbox()
    {
		ConfigManager config = manager;
		if (IsEditor() && config.sandboxType != Enum.SandboxType.NeverEnable)
			return true;
		if (IsDevelopment() && (config.sandboxType == Enum.SandboxType.EnableOnDebug || config.sandboxType == Enum.SandboxType.EnableOnRelease))
			return true;
		if (IsRelease() && config.sandboxType == Enum.SandboxType.EnableOnRelease)
			return true;

		return false;	
	}
	
	public static bool CanDebug()
    {
		ConfigManager config = manager;
		if (IsEditor() && config.debugType != Enum.DebugType.DontShow)
			return true;
		if (IsDevelopment() && (config.debugType == Enum.DebugType.ShowOnDebug || config.debugType == Enum.DebugType.ShowOnRelease))
			return true;
		if (IsRelease() && config.debugType == Enum.DebugType.ShowOnRelease)
			return true;

		return false;	
	}
	
	public static bool CanDelete()
    {
		ConfigManager config = manager;
		if (IsEditor() && config.deleteType != Enum.DeleteType.DontDelete)
			return true;
		if (IsDevelopment() && (config.deleteType == Enum.DeleteType.DeleteOnDebug || config.deleteType == Enum.DeleteType.DeleteOnRelease))
			return true;
		if (IsRelease() && config.deleteType == Enum.DeleteType.DeleteOnRelease)
			return true;

		return false;	
	}
	
	public static string name
	{
		get
		{
        	return manager.appName;
		}
    }
	
	public static float version
	{
		get
		{
        	return manager.appVersion;
		}
    }
	
	public static int appId
	{
		get
		{
        	return manager.appId;
		}
    }
	
	public static AppType appType
	{
		get
		{
        	return manager.gameType;
		}
    }
	
	public static string appProtocol
	{
		get
		{
       		return manager.appProtocol;
		}
    }
	
	public static string bundle
    {
		get
		{
# if UNITY_WEBPLAYER
			//if (!Mobile.IsMobile())
				return default(string);
#endif
	
			ConfigManager config = manager;
			if (config.gameType == AppType.Free)
				return (Info.IsPlatform(Platform.iPhone)) ?
	                config.appleFreeBundle : config.androidFreeBundle;
			if (config.gameType == AppType.Pay)
				return (Info.IsPlatform(Platform.iPhone)) ?
	                config.applePayBundle : config.androidPayBundle;
			if (config.gameType == AppType.Custom1)
				return (Info.IsPlatform(Platform.iPhone)) ?
	                config.appleCustom1Bundle : config.androidCustom1Bundle;
			if (config.gameType == AppType.Custom2)
				return (Info.IsPlatform(Platform.iPhone)) ?
	                config.appleCustom3Bundle	 : config.androidCustom2Bundle;
			if (config.gameType == AppType.Custom3)
				return (Info.IsPlatform(Platform.iPhone)) ?
	                config.appleCustom2Bundle : config.androidCustom3Bundle;
	
			return default(string);
		}
	}
	
	public static string pushId
    {
		get
		{
#if UNITY_WEBPLAYER
			//if (!Mobile.IsMobile())
				return default(string);
#endif
			
			return (Info.IsPlatform(Platform.iPhone)) ?
				pushAppleId : pushAndroidId;
		}
	}
	
	public static string pushAndroidId
    {
		get
		{
#if UNITY_WEBPLAYER
			//if (!Mobile.IsMobile())
				return default(string);
#endif
	
			ConfigManager config = manager;
			if (config.gameType == AppType.Free)
				return config.androidPushFreeId;
			if (config.gameType == AppType.Pay)
				return config.androidPushPayId;
			if (config.gameType == AppType.Custom1)
				return config.androidPushCustom1Id;
			if (config.gameType == AppType.Custom2)
				return config.androidPushCustom2Id;
			if (config.gameType == AppType.Custom3)
				return config.androidPushCustom3Id;
	
			return default(string);
		}
	}
	
	public static string pushAppleId
    {
		get
		{
#if UNITY_WEBPLAYER
			//if (!Mobile.IsMobile())
				return default(string);
#endif
	
			ConfigManager config = manager;
			if (config.gameType == AppType.Free)
				return config.applePushFreeId;
			if (config.gameType == AppType.Pay)
				return config.applePushPayId;
			if (config.gameType == AppType.Custom1)
				return config.applePushCustom1Id;
			if (config.gameType == AppType.Custom2)
				return config.applePushCustom2Id;
			if (config.gameType == AppType.Custom3)
				return config.applePushCustom3Id;
	
			return default(string);
		}
	}
	
	public static string pushProjectId
    {
		get
		{
			ConfigManager config = manager;
			if (config.gameType == AppType.Free)
				return config.androidPushFreePj;
			if (config.gameType == AppType.Pay)
				return config.androidPushPayPj;
			if (config.gameType == AppType.Custom1)
				return config.androidPushCustom1Pj;
			if (config.gameType == AppType.Custom2)
				return config.androidPushCustom2Pj;
			if (config.gameType == AppType.Custom3)
				return config.androidPushCustom3Pj;
	
			return default(string);
		}
	}
	
	public static string appleId
    {
		get
		{
			if (Info.IsPlatform(Platform.iPhone))
				return default(string);
	
			ConfigManager config = manager;
			if (config.gameType == AppType.Free)
				return config.appleFreeId;
			if (config.gameType == AppType.Pay)
				return config.applePayId;
			if (config.gameType == AppType.Custom1)
				return config.appleCustom1Id;
			if (config.gameType == AppType.Custom2)
				return config.appleCustom2Id;
			if (config.gameType == AppType.Custom3)
				return config.appleCustom3Id;
	
			return default(string);
		}
	}
	
	public static string facebookId
	{
		get 
		{
        	return manager.facebookId;
		}
    }

    public static string facebookSecret
    {
		get 
		{
        	return manager.facebookSecret;
		}
    }

    public static string facebookPageId
    {
		get 
		{
        	return manager.facebookPageId.Replace(new Dictionary<string, string>()
				{
					{"https://", "http://"},
					{"http://", ""},
					{"www.facebook.com/", "facebook.com/"},
					{"facebook.com/", ""}
				}
			);
		}
    }

    public static string facebookCanvas
    {
		get 
		{
        	return manager.facebookCanvas.Replace(new Dictionary<string, string>()
				{
					{"https://", "http://"},
					{"http://", ""},
					{"www.facebook.com/", "facebook.com/"},
					{"apps.facebook.com/", "facebook.com/"},
					{"facebook.com/", ""}
				}
			);
		}
    }

    public static string androidKey
    {
		get 
		{
        	return manager.GetAndroidKey();
		}
    }
	
    // Refazer
	public static bool IsFastSave()
	{
        return true;
    }
}
