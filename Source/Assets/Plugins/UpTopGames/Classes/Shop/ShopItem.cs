using UnityEngine;
using System.Collections;
using System.Reflection;

public enum ShopItemType { Consumable, NonConsumable };

[System.Serializable]
public class ShopItem
{
	public string id;
	public ShopItemType type;
	public string name;
	public int coinPrice;
	public Texture image;
	public bool hide;
	public string description;
	public ShopItem[] itemsWithin;
	//public bool forFree = false;
	public int arraySize;
	public int count;
	
	public ShopItem(){}
	public ShopItem(ShopItem item)
	{
		FieldInfo[] fields_of_class = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		foreach(FieldInfo fi in fields_of_class)
		{
			fi.SetValue(this,fi.GetValue(item));
		}
	}
}
