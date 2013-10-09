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
		Flow.shopManager.BuyItem(CheckBuyItem, Flow.shopManager.GetShopItem(id));
	}
	
	void CheckBuyItem(ShopResultStatus status, string id)
	{
		
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
			itemRenderer.material.mainTexture = data.texture;
		}
	}
}
