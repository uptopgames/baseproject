using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;
using System;

public enum ShopResultStatus { Success, Failed };
public delegate void ShopDelegate(ShopResultStatus status, string product);

public class ShopManager : MonoBehaviour 
{
	ShopInApp[] inappsList;
	ShopItem[] itemList;
	ShopFeatures features;
	
	public void PurchaseInApp(string id, ShopDelegate callback)
	{
		ShopInApp inapp = GetInApp(id);
		
		if(inapp.type == ShopInAppType.Consumable) RequestConsumableProduct(inapp, callback);
		else RequestNonConsumableProduct(inapp, callback);
	}
	
	void Start()
	{
		inappsList = GetComponent<ConfigManager>().shopInApps;
		itemList = GetComponent<ConfigManager>().shopItems;
		features = GetComponent<ConfigManager>().shopFeatures;
	}
	
	void Init()
	{
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
		
		//foreach(string a in iosProductIds) Debug.Log("produto: "+a);
		
		IAP.requestProductData(iosProductIds,androidProductIDs, productList => 
		{
			/*foreach(IAPProduct prod in productList)
			{
				Debug.Log("IAP: "+prod.productId);
			}*/
			Debug.Log("product list received"); 
			//Utils.logObject(productList);
		});
	}
	
	void RequestNonConsumableProduct(ShopInApp inapp, ShopDelegate callback)
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
				if(inapp.type == ShopInAppType.NonConsumable && inapp.appleBundle.ToLower().Contains("noads"))
				{
					Save.Set(PlayerPrefsKeys.NOADS.ToString(), true);
					Save.SaveAll();
				}
				
				callback(ShopResultStatus.Success, id);
			}
			else
			{
				callback(ShopResultStatus.Failed, id);
			}
			
		});
		
	}
	
	public void RestoreTransactions(ShopDelegate callback)
	{
		IAP.restoreCompletedTransactions(productID =>
		{
			callback(ShopResultStatus.Success, productID);
		});
	}	
	
	void RequestConsumableProduct(ShopInApp inapp, ShopDelegate callback)
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
				if(inapp.isPackOfCoins)
				{
					int coinStock = Save.GetInt(PlayerPrefsKeys.COINS.ToString());
					coinStock += inapp.coinsCount;
					Save.Set(PlayerPrefsKeys.COINS.ToString(),coinStock);
					Save.SaveAll();
				}
				
				callback(ShopResultStatus.Success, id);
				
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
				callback(ShopResultStatus.Failed, id);
				
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
	
	public void BuyItem(ShopDelegate callback, ShopItem item)
	{
		foreach(ShopItem itemWithin in item.itemsWithin)
		{
			if(Save.HasKey(itemWithin.id) && itemWithin.type == ShopItemType.NonConsumable)
			{
				// ja tem o item
				Flow.game_native.showMessage("Already has item","You already have this item.");
				callback(ShopResultStatus.Failed, item.id);
				return;
			}
			//else if(itemWithin.forFree
		}
			
		//Flow.game_native.startLoading();
		
		if(item.coinPrice > Flow.header.coins)
		{
			// nao tem coins
			OfferCoinPackAndBuyItem(callback, item);
		}
		else
		{
			Debug.Log("tem coins, soma eh: "+item.coinPrice);
			// tem coins
			Flow.header.coins -= item.coinPrice;
			
			foreach(ShopItem iw in item.itemsWithin)
			{
				Debug.Log(iw.id);
				
				if(Save.HasKey(iw.id) && iw.type == ShopItemType.Consumable)
				{
					int userStock = Save.GetInt(iw.id);
					userStock += iw.count;
					Save.Set(iw.id,userStock);
					Debug.Log("tem item, eh consumivel");
				}
				else if(!Save.HasKey(iw.id) && item.type == ShopItemType.NonConsumable)
				{
					Save.Set(iw.id,1);
					Debug.Log("nao tem item, eh nao consumivel");
				}
				else
				{
					Save.Set(iw.id, iw.count);
					Debug.Log("nao tem item, eh consumivel");
				}
			}
			
			Save.Set(PlayerPrefsKeys.COINS.ToString(),Flow.header.coins);
			Save.SaveAll();
			
			callback(ShopResultStatus.Success, item.id);
			
			if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
			{
				// se a compra deu sucesso e o cara esta logado, registrar no server
				GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/shop/buy.php", BuyingConfirmation);
				WWWForm form = new WWWForm();
				
				for(int i = 0 ; i < item.itemsWithin.Length ; i++)
				{
					form.AddField("items["+i+"][id]", item.itemsWithin[i].id);
					form.AddField("items["+i+"][count]", Save.GetInt(item.itemsWithin[i].id));
					form.AddField("coins", Flow.header.coins);
				}
				
				conn.connect(form);
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
	
	public void OfferCoinPackAndBuyItem(ShopDelegate callback, ShopItem item)
	{
		// nao tem coins
		int difference = item.coinPrice - Flow.header.coins;
		ShopInApp coinPackToOffer = new ShopInApp();
		foreach(ShopInApp pack in coinPacks)
		{
			Debug.Log(pack.coinsCount);
			if(pack.coinsCount >= difference)
			{
				Debug.Log("coinpack selecionado: "+ pack.appleBundle);
				coinPackToOffer = pack;
				break;
			}
		}
#if UNITY_ANDROID && !UNITY_EDITOR
		string packBundle = coinPackToOffer.androidBundle;
#elif UNITY_IPHONE && !UNITY_EDITOR
		string packBundle = coinPackToOffer.appleBundle;
		Debug.Log("caiu no iOS e o packBundle: "+ coinPackToOffer.appleBundle);
#else
		string packBundle;
		Flow.game_native.showMessage("Not Enough Coins", "You don't have enough coins to buy this item");
		
		return;
#endif
		//List<string> tList = new List<string>();
		
		Flow.game_native.startLoading();
		
		IAP.purchaseConsumableProduct(packBundle, purchased =>
		{
			Flow.game_native.stopLoading();
			if(purchased)
			{
				Flow.header.coins += coinPackToOffer.coinsCount;
				Flow.header.coins -= item.coinPrice;
				
				foreach(ShopItem iw in item.itemsWithin)
				{
					//tList.Add(item.id);
					
					if(Save.HasKey(iw.id) && iw.type == ShopItemType.Consumable)
					{
						int userStock = Save.GetInt(iw.id);
						userStock += iw.count;
						Save.Set (iw.id,userStock);
					}
					else if(!Save.HasKey(iw.id) && iw.type == ShopItemType.NonConsumable)
					{
						Save.Set(iw.id,1);
					}
					else
					{
						Save.Set(iw.id, iw.count);
					}
					
				}
				
				Save.Set(PlayerPrefsKeys.COINS.ToString(),Flow.header.coins);
				Save.SaveAll();
				
				callback(ShopResultStatus.Success, item.id);
				
				if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
				{
					// se a compra deu sucesso e o cara esta logado, registrar no server
					GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/shop/buy.php", BuyingConfirmation);
					WWWForm form = new WWWForm();
					
					for(int i = 0 ; i < item.itemsWithin.Length ; i++)
					{
						form.AddField("items["+i+"][id]", item.itemsWithin[i].id);
						form.AddField("items["+i+"][count]", Save.GetInt(item.itemsWithin[i].id));
						form.AddField("coins", Flow.header.coins);
					}
					
					conn.connect(form);
				}
			}
			else
			{
				
				// nao tem coins para comprar o item
				Flow.game_native.showMessage("Not Enough Coins", "You don't have enough coins to buy this item");
				callback(ShopResultStatus.Failed, item.id);
			}		
		});
		
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
		//Debug.Log("RefreshShop");
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
				tempInApp.coinsCount = inapp["coins"].Int32Value;
				
				//Debug.Log(tempInApp.name);
				
				bool foundInApp = false;
				
				for(int i = 0 ; i < Flow.config.GetComponent<ConfigManager>().shopInApps.Length ; i++)
				{	
					if(Flow.config.GetComponent<ConfigManager>().shopInApps[i].appleBundle == tempInApp.appleBundle || Flow.config.GetComponent<ConfigManager>().shopInApps[i].androidBundle == tempInApp.androidBundle)
					{
						tempInApp.image = Flow.config.GetComponent<ConfigManager>().shopInApps[i].image;
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
			
			bool hasToRefreshGoodsScroll = false;
			foreach(IJSonObject item in items.ArrayItems)
			{
				ShopItem tempItem = new ShopItem();
				
				tempItem.id = item["item_id"].StringValue;
				tempItem.name = item["name"].StringValue;
				tempItem.coinPrice = item["price"].Int32Value;
				tempItem.type = item["type"].StringValue == "Item"? ShopItemType.NonConsumable : ShopItemType.Consumable;
				
				string[] ids = {};
				string[] counts = {};
				
				try
				{
					ids = item["itemsWithin"].StringValue.Split(',');
					counts = item["itemsCount"].StringValue.Split(',');
				}
				catch
				{
					ids = new string[]{ item["itemsWithin"].StringValue };
					counts = new string[]{ item["itemsCount"].StringValue };
				}
				
				List<ShopItem> tempIWList = new List<ShopItem>();
				for(int i = 0 ; i < ids.Length ; i++)
				{
					ShopItem iw = GetShopItem(ids[i]);
					iw.count = int.Parse(counts[i]);
				}
				
				tempItem.arraySize = tempIWList.Count;
				tempItem.itemsWithin = tempIWList.ToArray();
				
				tempItem.description = item["description"].StringValue;
				//tempItem.hide = item["hide"].Int32Value == 1;
				
				//Debug.Log("item: "+tempItem.name);
				
				bool foundItem = false;
				
				for(int i = 0 ; i < Flow.config.GetComponent<ConfigManager>().shopItems.Length ; i++)
				{
					if(Flow.config.GetComponent<ConfigManager>().shopItems[i].id == tempItem.id)
					{
						tempItem.image = Flow.config.GetComponent<ConfigManager>().shopItems[i].image;
						Flow.config.GetComponent<ConfigManager>().shopItems[i] = tempItem;
						
						foundItem = true;
						hasToRefreshGoodsScroll = true;
						break;
					}
				}
				
				if(!foundItem)
				{
					//Debug.Log("adicionando "+tempItem.name);
					hasToRefreshGoodsScroll = true;
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
			
			if(refreshPrime && hasToRefreshGoodsScroll) UIPanelManager.instance.transform.FindChild("ShopScenePanel").GetComponent<Shop>().RefreshItemsScroll();
			
		}
		
		if(refreshPrime) Init();
		
	}
	
}
