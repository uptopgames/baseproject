using UnityEngine;
using System.Collections;
using CodeTitans.JSon;
using System;

public class Shop : MonoBehaviour 
{
	public GameObject goodsPage;
	public UIScrollList goodsScroll;
	public UIScrollList coinsScroll;
	public UIBistateInteractivePanel goodsPanel;
	public UIBistateInteractivePanel coinsPanel;
	public UIStateToggleBtn shopToggle;
	
	public int itemsPerPage;
	public int inAppsPerPage;
	
	int items;
	int pages;
	int itemsLastPage;
	
	// Use this for initialization
	void Start () 
	{
		Invoke("GanheiItem",30);
		RefreshItemsScroll(true);
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(InitShop);
		
		shopToggle.SetState(0);
		coinsPanel.Reveal();
		goodsPanel.Hide();
	}
	
	void InitShop(EZTransition transition)
	{
		RefreshItemsScroll();
		RefreshCoinsScroll();
	}
	
	void GanheiItem()
	{
		Debug.Log("ganhei");
		Save.Set("item_2",true);
	}
	
	public void RefreshCoinsScroll()
	{
		for(int i = 0 ; i < coinsScroll.Count ; i++)
		{
			Transform tempPage = coinsScroll.GetItem(i).transform;
			
			for(int j = 0 ; j < inAppsPerPage ; j++)
			{
				Transform inApp = tempPage.FindChild("ShopInApp"+j);
				string id = inApp.GetComponent<ShopInfo>().id;
				
				if(Save.HasKey(id) || (id.Contains("noads") && Save.HasKey(PlayerPrefsKeys.NOADS.ToString())))
				{
					inApp.FindChild("Purchased").gameObject.SetActive(true);
					inApp.FindChild("Coins").FindChild("Label").gameObject.SetActive(false);
				}
				else if(!id.Contains("com."))
				{
					// eh feature
					if(id == "Like")
					{
						inApp.FindChild("Coins").FindChild("Label").GetComponent<SpriteText>().Text = Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsLike.ToString();
						if(Save.HasKey(PlayerPrefsKeys.LIKE.ToString()))
						{
							// se jah deu like, nao pode mais clicar no botao
							inApp.GetComponent<ShopInfo>().has = true;
							inApp.FindChild("Coins").gameObject.SetActive(false);
							inApp.FindChild("Purchased").gameObject.SetActive(true);
						}
					}
					else if(id == "Share")
					{
						inApp.FindChild("Coins").FindChild("Label").GetComponent<SpriteText>().Text = Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsShare.ToString();
						if(Save.HasKey(PlayerPrefsKeys.SHARE.ToString()) && (DateTime.Parse(Save.GetString(PlayerPrefsKeys.SHARE.ToString())) - DateTime.UtcNow) > TimeSpan.FromDays(1))
						{
							// se jah deu share naquele dia, nao pode dar mais
							inApp.GetComponent<ShopInfo>().has = true;
							inApp.FindChild("Coins").gameObject.SetActive(false);
							inApp.FindChild("Purchased").gameObject.SetActive(true);
						}
					}
					else if(id == "Rate")
					{
						inApp.FindChild("Coins").FindChild("Label").GetComponent<SpriteText>().Text = Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsRate.ToString();
						if(Save.HasKey(PlayerPrefsKeys.RATE.ToString()))
						{
							// se jah deu rate, nao pode mais clicar no botao
							inApp.GetComponent<ShopInfo>().has = true;
							inApp.FindChild("Coins").gameObject.SetActive(false);
							inApp.FindChild("Purchased").gameObject.SetActive(true);
						}
					}	
					else if(id == "Video")
					{
						inApp.FindChild("Coins").FindChild("Label").GetComponent<SpriteText>().Text = Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsVideo.ToString();
						if(Save.HasKey(PlayerPrefsKeys.VIDEO.ToString()) && (DateTime.Parse(Save.GetString(PlayerPrefsKeys.VIDEO.ToString())) - DateTime.UtcNow) > TimeSpan.FromDays(1))
						{
							// se jah viu o video naquele dia, nao pode ver mais
							inApp.GetComponent<ShopInfo>().has = true;
							inApp.FindChild("Coins").gameObject.SetActive(false);
							inApp.FindChild("Purchased").gameObject.SetActive(true);
						}
					}
					else if(id == "Widget")
					{
						inApp.FindChild("Coins").FindChild("Label").GetComponent<SpriteText>().Text = Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsWidget.ToString();
						if(Save.HasKey(PlayerPrefsKeys.WIDGET.ToString()) && (DateTime.Parse(Save.GetString(PlayerPrefsKeys.WIDGET.ToString())) - DateTime.UtcNow) > TimeSpan.FromDays(1))
						{
							// se jah viu o link naquele dia, nao pode ver mais
							inApp.GetComponent<ShopInfo>().has = true;
							inApp.FindChild("Coins").gameObject.SetActive(false);
							inApp.FindChild("Purchased").gameObject.SetActive(true);
						}
					}
					else if(id == "Invite")
					{
						inApp.FindChild("Coins").FindChild("Label").GetComponent<SpriteText>().Text = Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsInvite.ToString();
					}
				}
			}
		}
		
		
		
		
	
	}
	
	public void RefreshItemsScroll(bool start=false)
	{
		if(!start) goodsScroll.ClearList(true);
		
		ShopItem[] shopItems = Flow.config.GetComponent<ConfigManager>().shopItems;
		items = shopItems.Length;
		pages = items / itemsPerPage;
		itemsLastPage = items % itemsPerPage;
		if(itemsLastPage != 0) pages++;
		else itemsLastPage = itemsPerPage;
		
		//Debug.Log("pages: "+ pages);
		
		for(int i = 0 ; i < pages ; i++)
		{
			int itemsOnPage = itemsPerPage;
			if(i == pages-1) itemsOnPage = itemsLastPage;
			
			//Debug.Log("items on page: "+ itemsOnPage);
			
			GameObject tempPage = Instantiate(goodsPage) as GameObject;
			for(int j = 0 ; j < itemsOnPage ; j++)
			{
				tempPage.transform.FindChild("Item"+j).GetComponent<ShopInfo>().id = shopItems[(i*itemsPerPage)+j].id;
				tempPage.transform.FindChild("Item"+j).FindChild("BuyButton").GetComponent<UIButton>().Text = shopItems[(i*itemsPerPage)+j].name;
				tempPage.transform.FindChild("Item"+j).FindChild("Price").FindChild("Label").GetComponent<SpriteText>().Text = shopItems[(i*itemsPerPage)+j].coinPrice.ToString();
				if(shopItems[(i*itemsPerPage)+j].image != null) tempPage.transform.FindChild("Item"+j).FindChild("ItemImage").GetComponent<MeshRenderer>().material.mainTexture = shopItems[(i*itemsPerPage)+j].image;
				else tempPage.transform.FindChild("Item"+j).GetComponent<ShopInfo>().DownloadImage();
				
				// se tem o item, arruma seu transform
				if(Save.HasKey(tempPage.transform.FindChild("Item"+j).GetComponent<ShopInfo>().id))
				{
					tempPage.transform.FindChild("Item"+j).FindChild("Purchased").gameObject.SetActive(true);
					tempPage.transform.FindChild("Item"+j).FindChild("Price").gameObject.SetActive(false);
				}
			}
			
			for(int k = itemsOnPage ; k < itemsPerPage ; k++)
			{
				tempPage.transform.FindChild("Item"+k).gameObject.SetActive(false);
			}
			
			goodsScroll.AddItem(tempPage.GetComponent<UIListItemContainer>());
		}
	}
	
	void ShopToggle()
	{
		if(shopToggle.StateName == "Coins")
		{
			coinsPanel.Reveal();
			if(goodsPanel.IsShowing) goodsPanel.Hide();
		}
		else
		{
			if(coinsPanel.IsShowing) coinsPanel.Hide();
			goodsPanel.Reveal();
		}
	}
	
}

