using UnityEngine;
using System.Collections;

public enum ShopItemType { Consumable, NonConsumable };

[System.Serializable]
public class ShopItem
{
	public string id;
	public ShopItemType type;
	public string name;
	public int coinPrice;
	public Texture image;
	//public bool hide;
	public string description;
	public ShopItem[] itemsWithin;
	public bool forFree = false;
	public int arraySize;
	public int count;
}
