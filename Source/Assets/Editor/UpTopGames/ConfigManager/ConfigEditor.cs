// Interface da classe de configuração, para facil edição da prefab (roda somente no editor)

using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ConfigManager))] 
public class ConfigEditor : Editor
{
	
	public override void OnInspectorGUI()
	{
        ConfigManager config = (ConfigManager) target;
		
		Draw(config, Initializate);
		Draw(config, "Application Settings", ApplicationSettings);
		Draw(config, "Runtime Settings", RuntimeSettings);
		ConfigManagerBaseSettings.Bundle(config);
		ConfigManagerBaseSettings.Shop(config);
    	Draw(config, "Tools Settings", ToolsSettings);
 		ConfigManagerBaseSettings.Facebook(config);
		Draw(config, "Server Settings", ServerSettings_);
		ConfigManagerAdvertisementExtension.DrawInterface(config);
		Draw(config, "Shop Settings", ShopSettings);
    }
	
	void Initializate(ConfigManager config)
	{
		EditorGUILayout.Space();
		
		if (GUILayout.Button("Apply Changes"))
		{
			EditorApplication.ExecuteMenuItem("GameObject/Apply Changes To Prefab");
			ConfigManagerServerSettingsNativeExtension.Setup();
		}
		EditorGUILayout.LabelField("Necessário ser executado ao terminar a modificações para atualizar informações nativas (como: AndroidManifest, PushWoosh e Advertisement)", EditorStyles.whiteMiniLabel);
		
	}
	
	void ApplicationSettings(ConfigManager config)
	{
		
		config.appName = EditorGUILayout.TextField("App Name", config.appName);
		EditorGUILayout.LabelField("Nome do App exibido nas PopUps nativas", EditorStyles.whiteMiniLabel);
		
		config.appId = EditorGUILayout.IntField("App ID", config.appId);
		EditorGUILayout.LabelField("ID do App na database", EditorStyles.whiteMiniLabel);
		
		
		config.appVersion = EditorGUILayout.FloatField("App Version", config.appVersion);
		EditorGUILayout.LabelField("Versão do App (float)", EditorStyles.whiteMiniLabel);
	
		config.appProtocol = EditorGUILayout.TextField("App Protocol", config.appProtocol).Replace(":", "").Replace("/", "");
		EditorGUILayout.LabelField("Protocolo do App (ex: utgbase://)", EditorStyles.whiteMiniLabel);
		
        EditorGUILayout.Space();
		
		config.headerObject = (GameObject) EditorGUILayout.ObjectField("Header", config.headerObject, typeof(GameObject));
		
		EditorGUILayout.LabelField("Objeto Header", EditorStyles.whiteMiniLabel);
		
		EditorGUILayout.Space();
				
		config.loading = (GameObject) EditorGUILayout.ObjectField("Loading Dialog", config.loading, typeof(GameObject));
		config.messageOk = (GameObject) EditorGUILayout.ObjectField("Message Ok Dialog", config.messageOk, typeof(GameObject));
		config.messageOkCancel = (GameObject) EditorGUILayout.ObjectField("Message Ok Cancel Dialog", config.messageOkCancel, typeof(GameObject));
		config.inviteAllScroll = (UIScrollList) EditorGUILayout.ObjectField("Invite 'all' ScrollList ", config.inviteAllScroll, typeof(UIScrollList));
		config.invitePlayingScroll = (UIScrollList) EditorGUILayout.ObjectField("Invite 'playing' ScrollList", config.invitePlayingScroll, typeof(UIScrollList));
		
		EditorGUILayout.LabelField("Janelas padrao", EditorStyles.whiteMiniLabel);
		
		config.appInitialCoins = EditorGUILayout.IntField("Initial Coins Number", config.appInitialCoins);
		
	}
	
	void RuntimeSettings(ConfigManager config)
	{
		if (GUILayout.Button("Delete SavePrefs"))
		{
			PlayerPrefs.DeleteAll();
			if (Application.isPlaying)
				Save.DeleteAll();
		}
		EditorGUILayout.LabelField("Deleta preferencias do usuário (poder ser utilizado durante o Play)", EditorStyles.whiteMiniLabel);

		if (!Application.isPlaying)
			return;
		
		if (GUILayout.Button("Show Camera Template"))
		{
			EditorApplication.ExecuteMenuItem("Window/KazzAPI/Camera/iPad");
			EditorApplication.ExecuteMenuItem("Window/KazzAPI/Camera/iPhone");
		}
		EditorGUILayout.LabelField("Cria cameras demonstrando como o App se estruturaria em casa device.", EditorStyles.whiteMiniLabel);
	}
	
	void ToolsSettings(ConfigManager config)
	{
		config.debugType = (Info.Enum.DebugType)EditorGUILayout.EnumPopup("Debugger", config.debugType);
		config.deleteType = (Info.Enum.DeleteType)EditorGUILayout.EnumPopup("SavePrefs", config.deleteType);
	}

	void ServerSettings_(ConfigManager config)
	{
		if (GUILayout.Button(!config.showServerSettings ? "Open" : "Close"))
			config.showServerSettings = !config.showServerSettings;
	
		if (!config.showServerSettings)
		{
			EditorGUILayout.LabelField("Mostra configuracoes do Server Settings", EditorStyles.whiteMiniLabel);
			return;
		}
		
		EditorGUILayout.Space();
		
		ConfigManagerServerSettingsListExtension.Draw(config);
		EditorGUILayout.Space();
		ConfigManagerServerSettingsCreateExtension.Draw(config);
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("- Delete Server Setting");
		config.deleteServerSetting = EditorGUILayout.TextField("Key", config.deleteServerSetting);
		
		if (GUILayout.Button("Delete"))
			ConfigManagerServerSettingsExtension.DeleteSettings(config, config.deleteServerSetting);
	}
	
	void ShopSettings(ConfigManager config)
	{
		
		EditorGUILayout.LabelField("Game In Apps", EditorStyles.miniBoldLabel);
		
		if (GUILayout.Button(!config.showInApps ? "Open InApps" : "Close InApps")) config.showInApps = !config.showInApps;
		
		if (!config.showInApps)
		{
			EditorGUILayout.LabelField("Mostra InApps", EditorStyles.whiteMiniLabel);
		}
		else
		{
			ConfigManagerShop.DrawInApps(config);
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("New InApp"))
			{
				ShopInApp inapp = new ShopInApp();
				
				config.shopInApps.Add(inapp,ref config.shopInApps);
			}
		}
			
		EditorGUILayout.LabelField("Game Items", EditorStyles.miniBoldLabel);
		
		if (GUILayout.Button(!config.showShopItems ? "Open Shop Items" : "Close Shop Items")) config.showShopItems = !config.showShopItems;
		
		if (!config.showShopItems)
		{
			EditorGUILayout.LabelField("Mostra Itens comprados com coins", EditorStyles.whiteMiniLabel);
		}
		else
		{
			ConfigManagerShop.DrawShopItems(config);
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("New Shop Item"))
			{
				ShopItem shopItem = new ShopItem();
				
				config.shopItems.Add(shopItem,ref config.shopItems);
			}
		}
		
		EditorGUILayout.LabelField("Game Features", EditorStyles.miniBoldLabel);
		
		if (GUILayout.Button(!config.showShopFeatures ? "Open Shop Features" : "Close Shop Features")) config.showShopFeatures = !config.showShopFeatures;
		
		if (!config.showShopFeatures)
		{
			EditorGUILayout.LabelField("Mostra Features", EditorStyles.whiteMiniLabel);
		}
		else
		{
			ConfigManagerShop.DrawFeatures(config);
		}
		
		if(GUILayout.Button("Download Shop Info From Server"))
		{
			Debug.Log("oi");
			config.GetComponent<ShopManager>().RefreshShop(false);
		}
		
		if(GUILayout.Button("Delete All"))
		{
			config.shopInApps = new ShopInApp[]{};
			config.shopFeatures = new ShopFeatures();
			config.shopItems = new ShopItem[]{};
		}
	}
	
	delegate void RegisterConfig(ConfigManager config);
	
	void Break()
	{
		EditorGUILayout.Space();
	}
	
	void Draw(ConfigManager config, RegisterConfig register)
	{
		register(config);
		EditorGUILayout.Space();
	}
	
	void Draw(ConfigManager config, string title, RegisterConfig register)
	{
		EditorGUILayout.LabelField("'".Multiply(500), EditorStyles.miniBoldLabel);
		EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
		Draw(config, register);
	}
}
