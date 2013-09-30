using UnityEngine;
using System.Collections;
using CodeTitans.JSon;
using System;

public class Shop : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		
	}
	
	public static void RefreshShop()
	{
		GameJsonConnection conn = new GameJsonConnection(Flow.URL_BASE + "login/shop/refresh.php", OnRefreshShop);
		WWWForm form = new WWWForm();
		form.AddField("app_id", Info.appId.ToString());
		form.AddField("type", Info.appType.ToString());
		
		conn.connect(form);
	}
	
	public static void OnRefreshShop(string error, IJSonObject data)
	{
		if(error != null) Debug.Log(error);
		else
		{
			Debug.Log(data);
			
			IJSonObject inapps = data["inapps"];
			IJSonObject shopItems = data["items"];
			IJSonObject features = data["features"];
			
			foreach(IJSonObject inapp in inapps.ArrayItems)
			{
				ShopInApp tempInApp = new ShopInApp();
				
				tempInApp.androidBundle = inapp["android"].StringValue;
				tempInApp.appleBundle = inapp["apple"].StringValue;
				tempInApp.dolarPrice = inapp["price"].ToFloat();
				tempInApp.name = inapp["name"].StringValue;
				tempInApp.id = inapp["id"].Int32Value;
				
				Debug.Log(tempInApp.name);
				
				bool foundInApp = false;
				
				for(int i = 0 ; i < Flow.config.GetComponent<ConfigManager>().shopInApps.Length ; i++)
				{
					ShopInApp ia = Flow.config.GetComponent<ConfigManager>().shopInApps[i];	
					if(ia.appleBundle == tempInApp.appleBundle || ia.androidBundle == tempInApp.androidBundle)
					{
						ia = tempInApp;
						
						foundInApp = true;
						break;
					}
				}
				
				if(!foundInApp)
				{
					Flow.config.GetComponent<ConfigManager>().shopInApps.Add(tempInApp, ref Flow.config.GetComponent<ConfigManager>().shopInApps);
				}
			}
			
			Array.Sort(Flow.config.GetComponent<ConfigManager>().shopInApps, delegate(ShopInApp a, ShopInApp b) {
						return a.id.CompareTo(b.id);	
			});
			
			foreach(IJSonObject item in shopItems.ArrayItems)
			{
				ShopItem tempItem = new ShopItem();
				
				tempItem.id = item["item_id"].Int32Value;
				tempItem.name = item["name"].StringValue;
				tempItem.coinPrice = item["price"].Int32Value;
				
			}
			
		}
	}
	
}

[System.Serializable]
public class ShopInApp
{
	public int id;
	public ShopInAppType type;
	public string appleBundle;
	public string androidBundle;
	public float dolarPrice;
	public string name;
	public Texture image;
	public string description;
	public int goodCount;
	public int coinsCount;
	public bool isPackOfCoins = true;
}

[System.Serializable]
public class ShopItem
{
	public int id;
	public ShopItemType type;
	public string name;
	public int coinPrice;
	public Texture image;
	public bool hide;
	public string description;
	public int count;
}

public enum ShopInAppType { Consumable, NonConsumable };
public enum ShopItemType { Consumable, NonConsumable };
public enum FeatureType { Share, Like, Rate, Video, Widget, Invite };

[System.Serializable]
public class ShopFeature
{
	public FeatureType type;
	public int coins;
	//public Texture image;
}
