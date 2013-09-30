using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		
	}

}

[System.Serializable]
public class ShopInApp
{
	public int id;
	public string appleBundle;
	public string androidBundle;
	public int dolarPrice;
	public string name;
	public Texture image;
}

public class ShopItem
{
	public int id;
	public string name;
	public int coinPrice;
	public Texture image;
}
