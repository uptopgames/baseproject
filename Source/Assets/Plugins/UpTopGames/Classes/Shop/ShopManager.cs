using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;
using System;

public enum ShopResultStatus { Success, Failed };
public delegate void ShopDelegate(ShopResultStatus status, params string[] products);

public class ShopManager : MonoBehaviour 
{
	ShopInApp[] inappsList;
	ShopItem[] itemList;
	ShopFeatures features;
	
	// Use this for initialization
	void Start () 
	{

	}
	
	
	void Init()
	{
		inappsList = GetComponent<ConfigManager>().shopInApps;
		itemList = GetComponent<ConfigManager>().shopItems;
		features = GetComponent<ConfigManager>().shopFeatures;
		
		string androidKey = Info.androidKey;
		IAP.init (androidKey);	
		Debug.Log("Started IAP");
		
		List<string> androidList = new List<string>();
		List<string> appleList = new List<string>();
		
		for(int i = 0 ; i < inappsList.Length ; i++)
		{
			androidList.Add(inappsList[i].androidBundle);
			appleList.Add(inappsList[i].appleBundle);
		}
		
		string[] androidProductIDs = androidList.ToArray();
		string[] iosProductIds = appleList.ToArray();

		IAP.requestProductData(iosProductIds,androidProductIDs, productList => 
		{	
			Debug.Log("product list received"); 
			//Utils.logObject(productList);
		});
	}
	
	public void RequestNonConsumableProduct(ShopInApp inapp, ShopInAppsDelegate callback)
	{
		Flow.game_native.startLoading();
		
#if UNITY_ANDROID
		string id = inapp.androidBundle;
#elif UNITY_IPHONE
		string id = inapp.appleBundle;
#else
		Flow.game_native.stopLoading();
		Flow.game_native.showMessage("Currently Unavailable","Purchasing only available in mobile devices.");
		return;
#endif
		IAP.purchaseNonconsumableProduct(id, didSucceed =>
		{
			Flow.game_native.stopLoading();
			
			if(didSucceed)
			{
				callback(ShopResultStatus.Success, inapp);
			}
			else
			{
				callback(ShopResultStatus.Failed, inapp);
			}
			
		});
		
	}
	
	public void RestoreTransactions(ShopInAppsDelegate callback)
	{
		IAP.restoreCompletedTransactions(productID =>
		{
			callback(ShopResultStatus.Success, GetInApp(productID));
		});
	}	
	
	public void RequestConsumableProduct(ShopInApp inapp, ShopInAppsDelegate callback)
	{
		Flow.game_native.startLoading();
#if UNITY_ANDROID
		string id = inapp.androidBundle;
#elif UNITY_IPHONE
		string id = inapp.appleBundle;
#else
		Flow.game_native.stopLoading();
		Flow.game_native.showMessage("Currently Unavailable","Purchasing only available in mobile devices.");
		return;
#endif		
		IAP.purchaseConsumableProduct(id, didSucceed =>
		{
			Flow.game_native.stopLoading();
			
			if(didSucceed)
			{
				callback(ShopResultStatus.Success, inapp);
				
				/*Debug.Log("Sucesso na compra de "+id);
				int currentCoins = PlayerPrefs.GetInt(PlayerPrefsKeys.Coins.ToString());

				if(id.Contains("coins_10k"))
				{
					PlaceOrder(currentCoins, Coins_Package_1);
				} 
				else if(id.Contains("coins_25k"))
				{
					PlaceOrder(currentCoins, Coins_Package_2);
				} 
				else if(id.Contains("coins_50k"))
				{
					PlaceOrder(currentCoins, Coins_Package_3);
				} 
				else if(id.Contains("coins_100k"))
				{
					PlayerPrefs.SetInt(PlayerPrefsKeys.DoNotBotherTheUsers.ToString(), 1);
					PlaceOrder(currentCoins, Coins_Package_4);				
				}*/
			}
			else
			{
				callback(ShopResultStatus.Failed, inapp);
				
				/*Debug.Log("Falha na compra de "+id);
				purchaseDialog.Show();
				purchaseDialog.SetMessage("Purchase failed");
				purchaseDialog.SetType(PurchaseDialog.DialogTypeEnum.OK);*/
			}

			/*slider.RefreshSliceValues();
			cashMachineSound.audio.Play();*/
			
		});
	}
	
	public ShopItem GetShopItem(string id)
	{
		ShopItem item = new ShopItem();
		for (int i = 0 ; i < itemList.Length ; i++)
		{
			if(itemList[i].id == id)
			{
				item = itemList[i];
				break;
			}
		}
		return item;
	}
	
	public ShopInApp GetInApp(string appleBundle)
	{
		ShopInApp inapp = new ShopInApp();
		for (int i = 0 ; i < inappsList.Length ; i++)
		{
			if(inappsList[i].appleBundle == appleBundle)
			{
				inapp = inappsList[i];
				break;
			}
		}
		return inapp;
	}
	
	public void BuyItem(ShopItemsDelegate callback, params ShopItem[] ids)
	{
		Flow.game_native.startLoading();
		
		ShopItem sum = new ShopItem();
		
		foreach (ShopItem si in ids)
		{
			if(si.forFree == false)
			{
				sum.coinPrice += si.coinPrice*si.count;
			}
		}
		
		
		if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
		{
			// esta logado
			GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/shop/buy.php", BuyingConfirmation);
			WWWForm form = new WWWForm();
			
			for(int i = 0 ; i < ids.Length ; i++)
			{
				form.AddField("items["+i+"]['id']", ids[i].id);
				form.AddField("items["+i+"]['count']", ids[i].count);
				form.AddField("items["+i+"]['free']", ids[i].forFree);
			}
			
			conn.connect(form);
		}
		else	
		{
			// nao esta logado
			
			if(sum.coinPrice > Flow.header.coins)
			{
				// nao tem coins
				int difference = sum.coinPrice - Flow.header.coins;
				foreach(ShopInApp pack in coinPacks)
				{
					if(pack.coinsCount >= difference)
					{
						coinPackToOffer = pack;
						break;
					}
				}
				delegateToCallAfterOffer = callback;
				
				
				Flow.game_native.showMessageOkCancel(this, "DontHaveCoinsButChoseToBuy", DontHaveCoinsNative, "DontHaveCoins", "Not enough coins", "You don't have enough coins. Wanna buy some?", "Buy!", "Don't buy");
				
				
			}
			else
			{
				// tem coins
				Flow.header.coins -= sum.coinPrice;
				
				callback(ShopResultStatus.Success, ids);
				
			}
		}
		
	}
	
	private ShopInApp[] _coinPacks;
	public ShopInApp[] coinPacks
	{
		get
		{
			if(_coinPacks == null)
			{
				List<ShopInApp> list = new List<ShopInApp>();
				foreach (ShopInApp inapp in inappsList)
				{
					if(inapp.isPackOfCoins)
					{
						list.Add(inapp);
					}
				}
				
				_coinPacks = list.ToArray();
			}
			return _coinPacks;
		}
	}
	
	public ShopInApp coinPackToOffer;
	public ShopInAppsDelegate delegateToCallAfterOffer;
	
	public void DontHaveCoinsButChoseToBuy()
	{
		if(coinPackToOffer == null || delegateToCallAfterOffer) return;
		
		RequestConsumableProduct(coinPackToOffer, delegateToCallAfterOffer);
		delegateToCallAfterOffer = null;
		coinPackToOffer = null;
	}
	
	public void DontHaveCoins()
	{
		Flow.messageOkCancelDialog.SetActive(false);
		delegateToCallAfterOffer(ShopResultStatus.Failed);
		delegateToCallAfterOffer = null;
		coinPackToOffer = null;
	}
	
	public void DontHaveCoinsNative(string button)
	{
		if(button == "Buy!") DontHaveCoinsButChoseToBuy();
		else if(button == "Don't buy")
		{
			delegateToCallAfterOffer(ShopResultStatus.Failed);
		}
	}
	
	public void BuyingConfirmation(string error, IJSonObject data)
	{
		if(error != null)
		{
			Debug.Log(error);
		}
		else
		{
			
		}
	}
		
	public void RefreshShop(bool refreshPrime=true)
	{
		Debug.Log("RefreshShop");
		GameJsonConnection conn = new GameJsonConnection(Flow.URL_BASE + "login/shop/refresh.php", OnRefreshShop);
		WWWForm form = new WWWForm();
		form.AddField("app_id", Info.appId.ToString());
		form.AddField("type", Info.appType.ToString());
		
		conn.connect(form,refreshPrime);
	}
	
	public void OnRefreshShop(string error, IJSonObject data, object state)
	{
		bool refreshPrime = (bool) state;
		
		if(error != null) Debug.Log(error);
		else
		{
			Debug.Log(data);
			
			IJSonObject inapps = data["inapps"];
			IJSonObject items = data["items"];
			IJSonObject features = data["features"];
			
			foreach(IJSonObject inapp in inapps.ArrayItems)
			{
				ShopInApp tempInApp = new ShopInApp();
				
				tempInApp.androidBundle = inapp["android"].StringValue;
				tempInApp.appleBundle = inapp["apple"].StringValue;
				tempInApp.dolarPrice = inapp["price"].ToFloat();
				tempInApp.name = inapp["name"].StringValue;
				tempInApp.id = inapp["id"].Int32Value;
				tempInApp.description = inapp["description"].StringValue;
				tempInApp.isPackOfCoins = inapp["coins"].Int32Value > 0;
				tempInApp.type = inapp["type"].StringValue == "Consumable"? ShopInAppType.Consumable : ShopInAppType.NonConsumable;
				tempInApp.goodCount = inapp["goodCount"].Int32Value;
				
				//Debug.Log(tempInApp.name);
				
				bool foundInApp = false;
				
				for(int i = 0 ; i < Flow.config.GetComponent<ConfigManager>().shopInApps.Length ; i++)
				{	
					if(Flow.config.GetComponent<ConfigManager>().shopInApps[i].appleBundle == tempInApp.appleBundle || Flow.config.GetComponent<ConfigManager>().shopInApps[i].androidBundle == tempInApp.androidBundle)
					{
						Flow.config.GetComponent<ConfigManager>().shopInApps[i] = tempInApp;
						
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
			
			bool foundItem = false;
			
			foreach(IJSonObject item in items.ArrayItems)
			{
				ShopItem tempItem = new ShopItem();
				
				tempItem.id = item["item_id"].StringValue;
				tempItem.name = item["name"].StringValue;
				tempItem.coinPrice = item["price"].Int32Value;
				tempItem.type = item["type"].StringValue == "Item"? ShopItemType.NonConsumable : ShopItemType.Consumable;
				tempItem.count = item["count"].Int32Value;
				tempItem.description = item["description"].StringValue;
				tempItem.hide = item["hide"].Int32Value == 1;
				
				for(int i = 0 ; i < Flow.config.GetComponent<ConfigManager>().shopItems.Length ; i++)
				{
					if(Flow.config.GetComponent<ConfigManager>().shopItems[i].id == tempItem.id)
					{
						Flow.config.GetComponent<ConfigManager>().shopItems[i] = tempItem;
						
						foundItem = true;
						break;
					}
				}
				
				if(!foundItem)
				{
					Flow.config.GetComponent<ConfigManager>().shopItems.Add(tempItem, ref Flow.config.GetComponent<ConfigManager>().shopItems);
				}
				
			}
			
			Array.Sort(Flow.config.GetComponent<ConfigManager>().shopItems, delegate(ShopItem a, ShopItem b) {
						return a.id.CompareTo(b.id);	
			});
			
			Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsInvite = features["invite"].Int32Value;
			Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsLike = features["like"].Int32Value;
			Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsRate = features["rate"].Int32Value;
			Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsShare = features["share"].Int32Value;
			Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsVideo = features["video"].Int32Value;
			Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsWidget = features["widget"].Int32Value;
			
		}
		
		if(refreshPrime) Init();
		
	}
	
}
