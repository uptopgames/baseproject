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
		EditorGUILayout.Space();
		
		config.firstSceneObject = EditorGUILayout.ObjectField("First Scene", config.firstSceneObject, typeof(UnityEngine.Object), false);
		if (config.firstSceneObject != null)
			config.firstScene = config.firstSceneObject.name;
		EditorGUILayout.LabelField("Primeira scene do jogo (selecionar *.unity3d)", EditorStyles.whiteMiniLabel);
		
		config.headerObject = EditorGUILayout.ObjectField("Header Component", config.headerObject, typeof(UnityEngine.Object), false);
		
		if (config.headerObject != null && config.headerObject.name != null)
			config.headerComponent = config.headerObject.name;
		else config.headerComponent = null;
		EditorGUILayout.LabelField("Component do Header (ex: GameHeader.cs)", EditorStyles.whiteMiniLabel);
		
        config.isSingleGame = EditorGUILayout.Toggle("Offline Game", config.isSingleGame);
		EditorGUILayout.LabelField("Se existe uma scene antes do Login", EditorStyles.whiteMiniLabel);
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
		EditorGUILayout.LabelField("Itens compraveis", EditorStyles.whiteMiniLabel);
		
		ConfigManagerShop.Draw(config);
		
		if(GUILayout.Button("New Item"))
		{
			Debug.Log("yay");
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
