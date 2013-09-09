using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class ConfigManagerBaseSettings
{
	private static string pos = " Settings";
	
	public static void Bundle(ConfigManager config)
	{
		DrawTitle("Bundle");
		
		config.gameType = (Info.AppType)EditorGUILayout.EnumPopup("App Type", config.gameType);
		EditorGUILayout.LabelField("Os parametros sao setados por AppType", EditorStyles.whiteMiniLabel);
		
		string tab = " ".Multiply(5);
		
		SetSpace();
		
		EditorGUILayout.LabelField("Apple", EditorStyles.miniBoldLabel);
		
		config.SetAppleBundleId(EditorGUILayout.TextField(tab + "Bundle ID", config.GetAppleBundleId()));
		EditorGUILayout.LabelField(tab + "  Bundle ID do App na Apple (ex: com.freegamesandtopapps.base)", EditorStyles.whiteMiniLabel);
		
		config.SetAppleId(EditorGUILayout.TextField(tab + "Product ID", config.GetAppleId()));
		EditorGUILayout.LabelField(tab + "  Product ID do App na Apple (ex: 54391298)", EditorStyles.whiteMiniLabel);
		
		SetSpace();
		SetSpace();
		
		EditorGUILayout.LabelField("Android", EditorStyles.miniBoldLabel);
		
		config.SetAndroidBundleId(EditorGUILayout.TextField(tab + "Package ID", config.GetAndroidBundleId()));
		EditorGUILayout.LabelField(tab + "  Package ID do App no Android (ex: com.freegamesandtopapps.base)", EditorStyles.whiteMiniLabel);
		
		config.SetAndroidProjectId(EditorGUILayout.TextField(tab + "PW Project ID", config.GetAndroidProjectId()));
		EditorGUILayout.LabelField(tab + "  PushWoosh Project ID do App (ex: AN2B3RNOUP)", EditorStyles.whiteMiniLabel);
		
		SetSpace();
		SetSpace();
		
		config.SetApplePushId(EditorGUILayout.TextField("PushWoosh ID", config.GetApplePushId()));
		config.SetAndroidPushId(config.GetApplePushId());
		EditorGUILayout.LabelField("PushWoosh ID do App (ex: 29NF-9H5D)", EditorStyles.whiteMiniLabel);
	}
	
	public static void Shop(ConfigManager config)
	{
		DrawTitle("Shop");
		
		config.shopType = (ConfigManager.ShopType)EditorGUILayout.EnumPopup("Start", config.shopType);
		EditorGUILayout.LabelField("Se iniciado automaticamente ou manualmente", EditorStyles.whiteMiniLabel);
		
		config.sandboxType = (Info.Enum.SandboxType)EditorGUILayout.EnumPopup("Sandbox", config.sandboxType);
		EditorGUILayout.LabelField("Quando deve efetuar compras pelo sandbox", EditorStyles.whiteMiniLabel);
		SetSpace();
		
		EditorGUILayout.LabelField("Android", EditorStyles.miniBoldLabel);
		
		config.SetAndroidKey(EditorGUILayout.TextField(" ".Multiply(5) + "GoogleIAB Key", config.GetAndroidKey()));
		EditorGUILayout.LabelField(" ".Multiply(5) + "  Google InAppBiling Key (obrigatorio) - LEMBRAR DE COLOCAR O DO FULL E DO FREE", EditorStyles.whiteMiniLabel);
		SetSpace();
		
	}
	
	public static void Facebook(ConfigManager config)
	{
		DrawTitle("Facebook");
		
		config.facebookId = EditorGUILayout.TextField("App ID", config.facebookId);
		EditorGUILayout.LabelField("Facebook App ID (ex: 332890576817578)", EditorStyles.whiteMiniLabel);
        config.facebookSecret = EditorGUILayout.TextField("Secret Code", config.facebookSecret);
		EditorGUILayout.LabelField("Secret Code (ex: f15fbf7c89601b4aaa82483aeeed51848)", EditorStyles.whiteMiniLabel);
		
        config.facebookPageId = EditorGUILayout.TextField("App Page ID", config.facebookPageId);
		EditorGUILayout.LabelField("Pagina para ser curtida (ex: https://facebook.com/304324723847)", EditorStyles.whiteMiniLabel);
		
        config.facebookCanvas = EditorGUILayout.TextField("App Canvas", config.facebookCanvas);
		EditorGUILayout.LabelField("Pagina aonde o jogo vem a ser exibido (ex: https://apps.facebook.com/seacombatup/app)", EditorStyles.whiteMiniLabel);
		
		config.facebookDelayTime = EditorGUILayout.FloatField("Canvas Delay", config.facebookDelayTime);
		EditorGUILayout.LabelField("Delay em segundos de ping ao servidor, informando que o usuario esta com o jogo aberto", EditorStyles.whiteMiniLabel);
		SetSpace();
	}
	
	private static void SetSpace()
	{
		EditorGUILayout.Space();
	}
	
	private static void DrawTitle(string title, bool _pos = true)
	{
		EditorGUILayout.LabelField("'".Multiply(500), EditorStyles.miniBoldLabel);
		
		if (_pos) title += pos;
		EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
	}

}
