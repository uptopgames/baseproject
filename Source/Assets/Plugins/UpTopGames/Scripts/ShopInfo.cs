using UnityEngine;
using System.Collections;
using System;

public class ShopInfo : MonoBehaviour 
{
	public string id;
	public MeshRenderer itemRenderer;
	public bool isInApp = false;
	[HideInInspector] public bool has = false;
	
	// metodo chamado so por itens
	void ClickedShopItem()
	{
		if(!has) Flow.shopManager.BuyItem(CheckBuyItem, Flow.shopManager.GetShopItem(id));
	}
	
	void CheckBuyItem(ShopResultStatus status, string product)
	{
		Debug.Log("status: "+status);
		Debug.Log("product: "+product);
		if(Flow.shopManager.GetShopItem(product).type == ShopItemType.NonConsumable)
		{
			has = true;
			transform.FindChild("Purchased").gameObject.SetActive(true);
			transform.FindChild("Price").gameObject.SetActive(false);
		}
	}
	
	// metodo chamado so por inapps
	void ClickedShopInApp()
	{
		if(id.Contains("com.")) Flow.shopManager.PurchaseInApp(id, OnPurchaseInApp);
		else ClickedFeature();
	}
	
	void ClickedFeature()
	{
		if(id == "Like")
		{
			
		}
		else if(id == "Share")
		{
			
		}
		else if(id == "Rate")
		{
			
		}
		else if(id == "Video")
		{
			
		}
		else if(id == "Widget")
		{
			
		}
		else if(id == "Invite")
		{
			
		}
	}
	
	void OnPurchaseInApp(ShopResultStatus status, string product)
	{
		if(status == ShopResultStatus.Success)
		{
			
		}
	}
	
	public void DownloadImage()
	{
		GameRawAuthConnection conn = new GameRawAuthConnection(Flow.URL_BASE + "login/shop/itemimage.php", HandleGetImage);
		
		WWWForm form = new WWWForm();
		
		form.AddField("item_id", id);
		form.AddField("app_id", Info.appId);
		
		conn.connect(form);
	}
	
	void HandleGetImage(string error, WWW data)
	{
		if(error != null) Debug.Log(error);
		else
		{
			for(int i = 0 ; i < Flow.config.GetComponent<ConfigManager>().shopItems.Length ; i++)
			{
				if(Flow.config.GetComponent<ConfigManager>().shopItems[i].id == id)
				{
					Flow.config.GetComponent<ConfigManager>().shopItems[i].image = data.texture;
				}
			}
			//itemRenderer.material.mainTexture = data.texture;
		}
	}
}
