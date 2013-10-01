using UnityEngine;
using System.Collections;

public enum ShopInAppType { Consumable, NonConsumable };

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
