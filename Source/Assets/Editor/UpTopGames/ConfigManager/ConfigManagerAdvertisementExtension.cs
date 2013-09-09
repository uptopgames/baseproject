using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class ConfigManagerAdvertisementExtension
{
	public static void DrawInterface(ConfigManager config)
	{
		if (!config.gameObject.GetComponent<AdvertisementManager>())
			config.gameObject.AddComponent<AdvertisementManager>();
		AdvertisementManager ads = config.gameObject.GetComponent<AdvertisementManager>();
		
		EditorGUILayout.LabelField("'".Multiply(500), EditorStyles.miniBoldLabel);
		
		EditorGUILayout.LabelField("Advertisement", EditorStyles.boldLabel);
		
		if (GUILayout.Button(config.showAds ? "Close" : "Open"))
		{
			if (ads != null)
				ads.curEditorAds = "";
			
			config.showAds = !config.showAds;
		}

		if (ads != null && config.showAds)
			CheckList(ads);
	}
	
	private static void CheckList(AdvertisementManager ads)
	{
		string space = " ".Multiply(5);
	
		EditorGUILayout.Space();
		ads.isActive = EditorGUILayout.Toggle("Enabled", ads.isActive);
		EditorGUILayout.Space();
	
		DrawList(ads, space);
	}
	
	private static void DrawList(AdvertisementManager ads, string space)
	{
		EditorGUILayout.LabelField("Advertisement -> Configuration", EditorStyles.boldLabel);
		AdvertisementAdColony(ads, space);
		AdvertisementAdMob(ads, space);
		AdvertisementFlurry(ads, space);
		AdvertisementRevMob(ads, space);
		//AdvertisementTapGage(ads, space);
		AdvertisementPlayHaven(ads, space);
		AdvertisementVungle(ads, space);
		AdvertisementDefaults(ads, space);
	}
	
	private static bool SwitcherButton(AdvertisementManager ads, string ad)
	{
		if (GUILayout.Button(ad))
		{
			ads.curEditorAds = ad;
			ads.curListAds = "";
			ads.curPreferenceAds = "";
		}

		if (ads.curEditorAds != ad)
			return false;
		
		return true;
	}
	
	private static void AdvertisementAdColony(AdvertisementManager ads, string tab)
	{
		if (!SwitcherButton(ads, "AdColony"))
			return;
		
		EditorGUILayout.LabelField("Apple", EditorStyles.miniBoldLabel);
		
		ads.adcolony.ios = EditorGUILayout.TextField(tab + "App ID", ads.adcolony.ios);
		ads.adcolony_zoneid.ios = EditorGUILayout.TextField(tab + "Secret", ads.adcolony_zoneid.ios);
		
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Android", EditorStyles.miniBoldLabel);
		
		ads.adcolony.android = EditorGUILayout.TextField(tab + "App ID", ads.adcolony.android);
		ads.adcolony_zoneid.android = EditorGUILayout.TextField(tab + "Secret", ads.adcolony_zoneid.android);
	}
	
	private static void AdvertisementAdMob(AdvertisementManager ads, string tab)
	{
		if (!SwitcherButton(ads, "AdMob"))
			return;
		
		EditorGUILayout.LabelField("Android", EditorStyles.miniBoldLabel);
		
		ads.admob = EditorGUILayout.TextField(tab + "Publisher ID", ads.admob);
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(tab + "Mediation", EditorStyles.miniBoldLabel);
		
		ads.adbmob_inters = EditorGUILayout.TextField(tab.Multiply(2) + "Interstitial", ads.adbmob_inters);
	}
	
	private static void AdvertisementFlurry(AdvertisementManager ads, string tab)
	{
		if (!SwitcherButton(ads, "Flurry"))
			return;
		
		EditorGUILayout.LabelField("Apple", EditorStyles.miniBoldLabel);
		
		ads.flurry.ios = EditorGUILayout.TextField(tab + "API Key", ads.flurry.ios);
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(tab + "Ad Space", EditorStyles.miniBoldLabel);
		
		ads.flurry_banner.ios = EditorGUILayout.TextField(tab.Multiply(2) + "Banner", ads.flurry_banner.ios);
		ads.flurry_inters.ios = EditorGUILayout.TextField(tab.Multiply(2) + "Interstitial", ads.flurry_inters.ios);
		
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Android", EditorStyles.miniBoldLabel);
		
		ads.flurry.android = EditorGUILayout.TextField(tab + "API Key", ads.flurry.android);
		
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField(tab + "Ad Space", EditorStyles.miniBoldLabel);
		
		ads.flurry_banner.android = EditorGUILayout.TextField(tab.Multiply(2) + "Banner", ads.flurry_banner.android);
		ads.flurry_inters.android = EditorGUILayout.TextField(tab.Multiply(2) + "Interstitial", ads.flurry_inters.android);
	}
	
	private static void AdvertisementRevMob(AdvertisementManager ads, string tab)
	{
		if (!SwitcherButton(ads, "RevMob"))
			return;
		
		EditorGUILayout.LabelField("Apple", EditorStyles.miniBoldLabel);
		
		ads.revmob.ios = EditorGUILayout.TextField(tab + "App ID", ads.revmob.ios);
		
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Android", EditorStyles.miniBoldLabel);
		
		ads.revmob.android = EditorGUILayout.TextField(tab + "App ID", ads.revmob.android);
	}
	
	/*private static void AdvertisementTapGage(AdvertisementManager ads, string tab)
	{
		if (!SwitcherButton(ads, "TapGage"))
			return;
		
		ads.tapgage = EditorGUILayout.TextField("Key", ads.tapgage);
	}*/
	
	private static void AdvertisementPlayHaven(AdvertisementManager ads, string tab)
	{
		if (!SwitcherButton(ads, "PlayHaven"))
			return;
		
		EditorGUILayout.LabelField("Apple", EditorStyles.miniBoldLabel);
		
		ads.playhaven_inters.ios = EditorGUILayout.TextField(tab + "Placement Tag", ads.playhaven_inters.ios);
		
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Android", EditorStyles.miniBoldLabel);
		
		ads.playhaven_inters.android = EditorGUILayout.TextField(tab + "Placement Tag", ads.playhaven_inters.android);
	}
	
	private static void AdvertisementVungle(AdvertisementManager ads, string tab)
	{
		if (!SwitcherButton(ads, "Vungle"))
			return;
		
		EditorGUILayout.LabelField("Apple", EditorStyles.miniBoldLabel);
		
		ads.vungle.ios = EditorGUILayout.TextField(tab + "App ID", ads.vungle.ios);
		
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Android", EditorStyles.miniBoldLabel);
		
		ads.vungle.android = EditorGUILayout.TextField(tab + "App ID", ads.vungle.android);
		
		if (GUILayout.Button(""))
			ads.curEditorAds = "";
	}
	

	private static void AdvertisementDefaults(AdvertisementManager ads, string tab)
	{
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Advertisement -> Preferences", EditorStyles.boldLabel);
		
		string[] adsList = new string[]
		{
			"Banner",
			"Interstitial",
			"PopUp",
			"Video",
			"Widget"
		};
		
		foreach(string adList in adsList)
		{
			if (GUILayout.Button(adList))
			{
				ads.curListAds = adList;
				ads.curPreferenceAds = "";
				ads.curEditorAds = "";
			}
			
			if (ads.curListAds == adList)
			{
				if (adList == "Banner")
					AdvertisementPreferencesBanner(ads);
				else if (adList == "Interstitial")
					AdvertisementPreferencesInterstitial(ads);
				else if (adList == "PopUp")
					AdvertisementPreferencesPopUp(ads);
				else if (adList == "Video")
					AdvertisementPreferencesVideo(ads);
				else if (adList == "Widget")
					AdvertisementPreferencesWidget(ads);
			}
		}
	}
	
	private static void AdvertisementPreferencesBanner(AdvertisementManager ads)
	{
		EditorGUILayout.LabelField("Enabled", EditorStyles.miniBoldLabel);
		if (ads.default_on_banners.Length > 0)
			for (int i = 0; i < ads.default_on_banners.Length; i++)
			{
				if (GUILayout.Button(ads.default_on_banners[i], EditorStyles.miniButton))
					ads.curPreferenceAds = ads.default_on_banners[i];
			}
		else
			EditorGUILayout.LabelField("Empty", EditorStyles.toolbarButton);
		
		EditorGUILayout.LabelField("Disabled", EditorStyles.miniBoldLabel);
		if (ads.default_off_banners.Length > 0)
			for (int i = 0; i < ads.default_off_banners.Length; i++)
			{
				if (GUILayout.Button(ads.default_off_banners[i], EditorStyles.miniButton))
					ads.curPreferenceAds = ads.default_off_banners[i];
			}
		else
			EditorGUILayout.LabelField("Empty", EditorStyles.toolbarButton);
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		if (ads.default_on_banners.Length + ads.default_off_banners.Length != 4)
		{
			ads.default_off_banners =
				new string[]
				{
					"AdMob",
					"Flurry",
					"iAd",
					"RevMob"
				};
			ads.default_on_banners =
				new string[] {};
		}
		
		EditorGUILayout.LabelField("Selected", EditorStyles.miniBoldLabel);
		EditorGUILayout.LabelField(ads.curPreferenceAds.IsEmpty() ? "None" : ads.curPreferenceAds, EditorStyles.toolbarButton);
		EditorGUILayout.LabelField("Manage", EditorStyles.miniBoldLabel);
		if (GUILayout.Button("Move Up", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_banners.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
			if (index >= 0)
			{
				if (index > 0 && index < tmp.Count)
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index - 1);
					ads.default_on_banners = tmp.ToArray();
				}
			}
			else
			{
				tmp = ads.default_off_banners.ToList();
				tmp.Remove(ads.curPreferenceAds);
				ads.default_off_banners = tmp.ToArray();
				
				tmp = ads.default_on_banners.ToList();
				tmp.Add(ads.curPreferenceAds);
				ads.default_on_banners = tmp.ToArray();
			}
			
		}
		
		if (GUILayout.Button("Move Down", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_banners.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
		
			if (index >= 0 && index < tmp.Count)
			{
				if (index + 1 >= tmp.Count)
				{
					tmp.Remove(ads.curPreferenceAds);
					ads.default_on_banners = tmp.ToArray();
					tmp = ads.default_off_banners.ToList();
					tmp.Add(ads.curPreferenceAds);
					ads.default_off_banners = tmp.ToArray();
				}
				else
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index + 1);
					ads.default_on_banners = tmp.ToArray();
				}
			}
		}
		
		EditorGUILayout.Space();
	}
	
	private static void AdvertisementPreferencesInterstitial(AdvertisementManager ads)
	{
		EditorGUILayout.LabelField("Enabled", EditorStyles.miniBoldLabel);
		for (int i = 0; i < ads.default_on_interstitials.Length; i++)
		{
			if (GUILayout.Button(ads.default_on_interstitials[i], EditorStyles.miniButton))
				ads.curPreferenceAds = ads.default_on_interstitials[i];
		}
		
		EditorGUILayout.LabelField("Disabled", EditorStyles.miniBoldLabel);
		for (int i = 0; i < ads.default_off_interstitials.Length; i++)
		{
			if (GUILayout.Button(ads.default_off_interstitials[i], EditorStyles.miniButton))
				ads.curPreferenceAds = ads.default_off_interstitials[i];
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		if (ads.default_on_interstitials.Length + ads.default_off_interstitials.Length != 5)
		{
			ads.default_off_interstitials =
				new string[]
				{
					"AdMob",
					"Flurry",
					"RevMob",
					//"TapGage",
					"PlayHaven"
				};
			ads.default_on_interstitials =
				new string[] {};
		}
		
		EditorGUILayout.LabelField("Selected", EditorStyles.miniBoldLabel);
		EditorGUILayout.LabelField(ads.curPreferenceAds.IsEmpty() ? "None" : ads.curPreferenceAds, EditorStyles.toolbarButton);
		EditorGUILayout.LabelField("Manage", EditorStyles.miniBoldLabel);
		if (GUILayout.Button("Move Up", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_interstitials.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
			if (index >= 0)
			{
				if (index > 0 && index < tmp.Count)
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index - 1);
					ads.default_on_interstitials = tmp.ToArray();
				}
			}
			else
			{
				tmp = ads.default_off_interstitials.ToList();
				tmp.Remove(ads.curPreferenceAds);
				ads.default_off_interstitials = tmp.ToArray();
				
				tmp = ads.default_on_interstitials.ToList();
				tmp.Add(ads.curPreferenceAds);
				ads.default_on_interstitials = tmp.ToArray();
			}
			
		}
		
		if (GUILayout.Button("Move Down", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_interstitials.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
		
			if (index >= 0 && index < tmp.Count)
			{
				if (index + 1 >= tmp.Count)
				{
					tmp.Remove(ads.curPreferenceAds);
					ads.default_on_interstitials = tmp.ToArray();
					tmp = ads.default_off_interstitials.ToList();
					tmp.Add(ads.curPreferenceAds);
					ads.default_off_interstitials = tmp.ToArray();
				}
				else
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index + 1);
					ads.default_on_interstitials = tmp.ToArray();
				}
			}
		}
		
		EditorGUILayout.Space();
	}
	
	private static void AdvertisementPreferencesPopUp(AdvertisementManager ads)
	{
		EditorGUILayout.LabelField("Enabled", EditorStyles.miniBoldLabel);
		for (int i = 0; i < ads.default_on_popups.Length; i++)
		{
			if (GUILayout.Button(ads.default_on_popups[i], EditorStyles.miniButton))
				ads.curPreferenceAds = ads.default_on_popups[i];
		}
		
		EditorGUILayout.LabelField("Disabled", EditorStyles.miniBoldLabel);
		for (int i = 0; i < ads.default_off_popups.Length; i++)
		{
			if (GUILayout.Button(ads.default_off_popups[i], EditorStyles.miniButton))
				ads.curPreferenceAds = ads.default_off_popups[i];
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		if (ads.default_on_popups.Length + ads.default_off_popups.Length != 1)
		{
			ads.default_off_popups =
				new string[]
				{
					"RevMob",
				};
			ads.default_on_popups =
				new string[] {};
		}
		
		EditorGUILayout.LabelField("Selected", EditorStyles.miniBoldLabel);
		EditorGUILayout.LabelField(ads.curPreferenceAds.IsEmpty() ? "None" : ads.curPreferenceAds, EditorStyles.toolbarButton);
		EditorGUILayout.LabelField("Manage", EditorStyles.miniBoldLabel);
		if (GUILayout.Button("Move Up", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_popups.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
			if (index >= 0)
			{
				if (index > 0 && index < tmp.Count)
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index - 1);
					ads.default_on_popups = tmp.ToArray();
				}
			}
			else
			{
				tmp = ads.default_off_popups.ToList();
				tmp.Remove(ads.curPreferenceAds);
				ads.default_off_popups = tmp.ToArray();
				
				tmp = ads.default_on_popups.ToList();
				tmp.Add(ads.curPreferenceAds);
				ads.default_on_popups = tmp.ToArray();
			}
			
		}
		
		if (GUILayout.Button("Move Down", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_popups.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
		
			if (index >= 0 && index < tmp.Count)
			{
				if (index + 1 >= tmp.Count)
				{
					tmp.Remove(ads.curPreferenceAds);
					ads.default_on_popups = tmp.ToArray();
					tmp = ads.default_off_popups.ToList();
					tmp.Add(ads.curPreferenceAds);
					ads.default_off_popups = tmp.ToArray();
				}
				else
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index + 1);
					ads.default_on_popups = tmp.ToArray();
				}
			}
		}
		
		EditorGUILayout.Space();
	}
	
	private static void AdvertisementPreferencesVideo(AdvertisementManager ads)
	{
		EditorGUILayout.LabelField("Enabled", EditorStyles.miniBoldLabel);
		for (int i = 0; i < ads.default_on_videos.Length; i++)
		{
			if (GUILayout.Button(ads.default_on_videos[i], EditorStyles.miniButton))
				ads.curPreferenceAds = ads.default_on_videos[i];
		}
		
		EditorGUILayout.LabelField("Disabled", EditorStyles.miniBoldLabel);
		for (int i = 0; i < ads.default_off_videos.Length; i++)
		{
			if (GUILayout.Button(ads.default_off_videos[i], EditorStyles.miniButton))
				ads.curPreferenceAds = ads.default_off_videos[i];
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		if (ads.default_on_videos.Length + ads.default_off_videos.Length != 2)
		{
			ads.default_off_videos =
				new string[]
				{
					"AdColony",
					"Vungle"
				};
			ads.default_on_videos =
				new string[] {};
		}
		
		EditorGUILayout.LabelField("Selected", EditorStyles.miniBoldLabel);
		EditorGUILayout.LabelField(ads.curPreferenceAds.IsEmpty() ? "None" : ads.curPreferenceAds, EditorStyles.toolbarButton);
		EditorGUILayout.LabelField("Manage", EditorStyles.miniBoldLabel);
		if (GUILayout.Button("Move Up", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_videos.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
			if (index >= 0)
			{
				if (index > 0 && index < tmp.Count)
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index - 1);
					ads.default_on_videos = tmp.ToArray();
				}
			}
			else
			{
				tmp = ads.default_off_videos.ToList();
				tmp.Remove(ads.curPreferenceAds);
				ads.default_off_videos = tmp.ToArray();
				
				tmp = ads.default_on_videos.ToList();
				tmp.Add(ads.curPreferenceAds);
				ads.default_on_videos = tmp.ToArray();
			}
			
		}
		
		if (GUILayout.Button("Move Down", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_videos.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
		
			if (index >= 0 && index < tmp.Count)
			{
				if (index + 1 >= tmp.Count)
				{
					tmp.Remove(ads.curPreferenceAds);
					ads.default_on_videos = tmp.ToArray();
					tmp = ads.default_off_videos.ToList();
					tmp.Add(ads.curPreferenceAds);
					ads.default_off_videos = tmp.ToArray();
				}
				else
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index + 1);
					ads.default_on_videos = tmp.ToArray();
				}
			}
		}
		
		EditorGUILayout.Space();
	}
	
	private static void AdvertisementPreferencesWidget(AdvertisementManager ads)
	{
		EditorGUILayout.LabelField("Enabled", EditorStyles.miniBoldLabel);
		for (int i = 0; i < ads.default_on_widgets.Length; i++)
		{
			if (GUILayout.Button(ads.default_on_widgets[i], EditorStyles.miniButton))
				ads.curPreferenceAds = ads.default_on_widgets[i];
		}
		
		EditorGUILayout.LabelField("Disabled", EditorStyles.miniBoldLabel);
		for (int i = 0; i < ads.default_off_widgets.Length; i++)
		{
			if (GUILayout.Button(ads.default_off_widgets[i], EditorStyles.miniButton))
				ads.curPreferenceAds = ads.default_off_widgets[i];
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		if (ads.default_on_widgets.Length + ads.default_off_widgets.Length != 3)
		{
			ads.default_off_widgets =
				new string[]
				{
					"RevMob",
					//"TapGage",
					"PlayHaven"
				};
			ads.default_on_widgets =
				new string[] {};
		}
		
		EditorGUILayout.LabelField("Selected", EditorStyles.miniBoldLabel);
		EditorGUILayout.LabelField(ads.curPreferenceAds.IsEmpty() ? "None" : ads.curPreferenceAds, EditorStyles.toolbarButton);
		EditorGUILayout.LabelField("Manage", EditorStyles.miniBoldLabel);
		if (GUILayout.Button("Move Up", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_widgets.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
			if (index >= 0)
			{
				if (index > 0 && index < tmp.Count)
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index - 1);
					ads.default_on_widgets = tmp.ToArray();
				}
			}
			else
			{
				tmp = ads.default_off_widgets.ToList();
				tmp.Remove(ads.curPreferenceAds);
				ads.default_off_widgets = tmp.ToArray();
				
				tmp = ads.default_on_widgets.ToList();
				tmp.Add(ads.curPreferenceAds);
				ads.default_on_widgets = tmp.ToArray();
			}
			
		}
		
		if (GUILayout.Button("Move Down", EditorStyles.miniButton))
		{
			List<string> tmp = ads.default_on_widgets.ToList();
			int index = tmp.IndexOf(ads.curPreferenceAds);
		
			if (index >= 0 && index < tmp.Count)
			{
				if (index + 1 >= tmp.Count)
				{
					tmp.Remove(ads.curPreferenceAds);
					ads.default_on_widgets = tmp.ToArray();
					tmp = ads.default_off_widgets.ToList();
					tmp.Add(ads.curPreferenceAds);
					ads.default_off_widgets = tmp.ToArray();
				}
				else
				{
					tmp.ChangeIndex(ads.curPreferenceAds, index + 1);
					ads.default_on_widgets = tmp.ToArray();
				}
			}
		}
		
		EditorGUILayout.Space();
	}
	
	private enum AdsType { Adcolony, Admob, Flurry, Iad, Playhaven, Revmob, Vungle }; 
	
	private static AdsType StringToAds(string ads) {
		for (int i = 0; i < 8; i++) {
			if(((AdsType)i).ToString() == ads)
				return (AdsType)i;	
		}
		return AdsType.Admob;
	}
}
