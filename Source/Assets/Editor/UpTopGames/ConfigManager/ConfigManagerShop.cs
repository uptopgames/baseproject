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
			config.shopInApps[i].isPackOfCoins = EditorGUILayout.Toggle("Is Pack of Coins",config.shopInApps[i].isPackOfCoins);
			
			if(config.shopInApps[i].isPackOfCoins)
			{
				config.shopInApps[i].coinsCount = EditorGUILayout.IntField("Coins Delivered",config.shopInApps[i].coinsCount);
			}
			else
			{
				config.shopInApps[i].goodCount = EditorGUILayout.IntField("Goods Delivered",config.shopInApps[i].goodCount);
			}
			
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
			config.shopItems[i].id = EditorGUILayout.IntField("ID",config.shopItems[i].id);
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
		if(config.shopFeatures.Length == 0)
		{
			ShopFeature like = new ShopFeature();
			like.type = FeatureType.Like;
			
			ShopFeature share = new ShopFeature();
			like.type = FeatureType.Share;
			
			ShopFeature rate = new ShopFeature();
			like.type = FeatureType.Rate;
			
			ShopFeature video = new ShopFeature();
			like.type = FeatureType.Video;
			
			ShopFeature widget = new ShopFeature();
			like.type = FeatureType.Widget;
			
			ShopFeature invite = new ShopFeature();
			like.type = FeatureType.Invite;
			
			config.shopFeatures.Add(like, ref config.shopFeatures);
			config.shopFeatures.Add(share, ref config.shopFeatures);
			config.shopFeatures.Add(rate, ref config.shopFeatures);
			config.shopFeatures.Add(video, ref config.shopFeatures);
			config.shopFeatures.Add(invite, ref config.shopFeatures);
			config.shopFeatures.Add(widget, ref config.shopFeatures);
		}
		
		for(int i = 0 ; i < config.shopFeatures.Length ; i++)
		{
			config.shopFeatures[i].type = (FeatureType) EditorGUILayout.EnumPopup("Type", config.shopFeatures[i].type);
			config.shopFeatures[i].coins = EditorGUILayout.IntField("Coins", config.shopFeatures[i].coins);
			EditorGUILayout.LabelField("-".Multiply(500));
			//config.shopFeatures[i].image = (Texture) EditorGUILayout.ObjectField("Image", config.shopFeatures[i].image, typeof(Texture));
		}
	}
}
