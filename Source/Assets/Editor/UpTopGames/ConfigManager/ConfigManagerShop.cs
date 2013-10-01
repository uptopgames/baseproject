using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public static class ConfigManagerShop
{
	public static void DrawInApps(ConfigManager config)
	{
		for(int i = 0 ; i < config.shopInApps.Length ; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("InApp");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			config.shopInApps[i].type = (ShopInAppType) EditorGUILayout.EnumPopup("In App Type", config.shopInApps[i].type);
			config.shopInApps[i].isPackOfCoins = EditorGUILayout.Toggle("Is Pack of Coins",config.shopInApps[i].isPackOfCoins);
			
			if(config.shopInApps[i].isPackOfCoins)
			{
				config.shopInApps[i].coinsCount = EditorGUILayout.IntField("Coins Delivered",config.shopInApps[i].coinsCount);
			}
			else
			{
				config.shopInApps[i].goodCount = EditorGUILayout.IntField("Goods Delivered",config.shopInApps[i].goodCount);
			}
			config.shopInApps[i].id = EditorGUILayout.IntField("Server ID", config.shopInApps[i].id);
			config.shopInApps[i].name = EditorGUILayout.TextField("InApp Name", config.shopInApps[i].name);
			config.shopInApps[i].dolarPrice = EditorGUILayout.FloatField("Dolar price",config.shopInApps[i].dolarPrice);
			config.shopInApps[i].appleBundle = EditorGUILayout.TextField("Apple Bundle", config.shopInApps[i].appleBundle);
			config.shopInApps[i].androidBundle = EditorGUILayout.TextField("Android Bundle", config.shopInApps[i].androidBundle);
			config.shopInApps[i].description = EditorGUILayout.TextField("Description", config.shopInApps[i].description);
			config.shopInApps[i].image = (Texture) EditorGUILayout.ObjectField("Image",config.shopInApps[i].image,typeof(Texture));
			
			
			if(GUILayout.Button("Delete InApp"))
			{
				config.shopInApps.Remove(i, ref config.shopInApps);
			}
			
			EditorGUILayout.LabelField("-".Multiply(500));
		}
	}
	
	public static void DrawShopItems(ConfigManager config)
	{
		for(int i = 0 ; i < config.shopItems.Length ; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Shop Item");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			config.shopItems[i].type = (ShopItemType) EditorGUILayout.EnumPopup("Item Type", config.shopItems[i].type);
			config.shopItems[i].id = EditorGUILayout.TextField("ID",config.shopItems[i].id);
			config.shopItems[i].name = EditorGUILayout.TextField("Item Name", config.shopItems[i].name);
			config.shopItems[i].coinPrice = EditorGUILayout.IntField("Coin price",config.shopItems[i].coinPrice);
			config.shopItems[i].description = EditorGUILayout.TextField("Description", config.shopItems[i].description);
			config.shopItems[i].image = (Texture) EditorGUILayout.ObjectField("Image",config.shopItems[i].image,typeof(Texture));
			
			if(GUILayout.Button("Delete Shop Item"))
			{
				config.shopItems.Remove(i, ref config.shopItems);
			}
			
			EditorGUILayout.LabelField("-".Multiply(500));
		}
	}
	
	public static void DrawFeatures(ConfigManager config)
	{
		if(config.shopFeatures == null) config.shopFeatures = new ShopFeatures();
		
		config.shopFeatures.coinsInvite = EditorGUILayout.IntField("Invite Coins", config.shopFeatures.coinsInvite);
		config.shopFeatures.coinsLike = EditorGUILayout.IntField("Like Coins", config.shopFeatures.coinsLike);
		config.shopFeatures.coinsRate = EditorGUILayout.IntField("Rate Coins", config.shopFeatures.coinsRate);
		config.shopFeatures.coinsShare = EditorGUILayout.IntField("Share Coins", config.shopFeatures.coinsShare);
		config.shopFeatures.coinsVideo = EditorGUILayout.IntField("Video Coins", config.shopFeatures.coinsVideo);
		config.shopFeatures.coinsWidget = EditorGUILayout.IntField("Widget Coins", config.shopFeatures.coinsWidget);
		EditorGUILayout.LabelField("-".Multiply(500));
		//config.shopFeatures[i].image = (Texture) EditorGUILayout.ObjectField("Image", config.shopFeatures[i].image, typeof(Texture));
	}
}
