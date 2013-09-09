using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ConfigManagerServerSettingsNativeExtension
{
	private static string
		manifestSamplePath = "Editor/KazzAPI/Sample/Manifest.xml",
		manifestFinishPath = "Plugins/Android/AndroidManifest.xml",
			
		pushwooshSamplePath = "Editor/KazzAPI/Sample/Pushwoosh.xml",
		pushwooshFinishPath = "Plugins/PushWoosh/iOS/Pushwoosh/Info.xml";
	
	public static void Setup()
	{
		string version = Info.version.ToString();
		if (!version.Contains(".")) version += ".0";
		PlayerSettings.bundleVersion = version;

		string bundle = Info.bundle;
		if (bundle != null && bundle != "")
			PlayerSettings.bundleIdentifier = bundle;
		
		PlayerSettings.productName = Info.name;
	
		TextAsset loadAndroidManifest = (TextAsset)Resources.LoadAssetAtPath("Assets/" + manifestSamplePath, typeof(TextAsset));
		if (loadAndroidManifest != null)
		{
			string androidManifest = loadAndroidManifest.ToString();
			androidManifest = androidManifest.Replace("#BUNDLE.IDENTIFIER#", Info.bundle);
			androidManifest = androidManifest.Replace("#APP.PROTOCOL#", Info.appProtocol);
			androidManifest = androidManifest.Replace("#PUSH.ID#", Info.pushAndroidId);
			androidManifest = androidManifest.Replace("#PUSH.PROJECT#", Info.pushProjectId);
			
			StreamWriter saveAndroidManifest = new StreamWriter("Assets/" + manifestFinishPath);
			saveAndroidManifest.WriteLine(androidManifest);
			saveAndroidManifest.Close();
			
		}
		else Debug.LogWarning("'" + manifestSamplePath + "' sample for AndroidManifest not found!");
		
		TextAsset loadPushwoosh = (TextAsset)Resources.LoadAssetAtPath("Assets/" + pushwooshSamplePath, typeof(TextAsset));
		if (loadPushwoosh != null)
		{
			string pushwoosh = loadPushwoosh.ToString();
			pushwoosh = pushwoosh.Replace("#APP.PROTOCOL#", Info.appProtocol);
			pushwoosh = pushwoosh.Replace("#PUSH.ID#", Info.pushAppleId);
			
			StreamWriter savePushwoosh = new StreamWriter("Assets/" + pushwooshFinishPath);
			savePushwoosh.WriteLine(pushwoosh);
			savePushwoosh.Close();
			
		}
		else Debug.LogWarning("'" + pushwooshSamplePath + "' sample for PushWooshInfo not found!");
		
		AssetDatabase.Refresh();
	}
}
