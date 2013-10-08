using UnityEngine;
using System.Collections;
using CodeTitans.JSon;
using System;

public class Shop : MonoBehaviour 
{
	public GameObject goodsPage;
	public UIScrollList goodsScroll;
	public UIScrollList coinsScroll;
	public UIStateToggleBtn shopToggle;
	
	public int itemsPerPage;
	
	int items;
	int pages;
	int itemsLastPage;
	
	// Use this for initialization
	void Start () 
	{
		RefreshItemsScroll(true);
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(InitShop);
	}
	
	void InitShop(EZTransition transition)
	{
		
		shopToggle.SetState(0);
		coinsScroll.gameObject.SetActive(true);
		goodsScroll.gameObject.SetActive(false);
		
		
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
			coinsScroll.gameObject.SetActive(true);
			goodsScroll.gameObject.SetActive(false);
		}
		else
		{
			coinsScroll.gameObject.SetActive(false);
			goodsScroll.gameObject.SetActive(true);
		}
	}
	
}

