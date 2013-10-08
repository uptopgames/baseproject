using UnityEngine;
using System.Collections;

public class ShopInfo : MonoBehaviour 
{
	public string id;
	public MeshRenderer itemRenderer;
	
	// metodo chamado so por itens
	void ClickedShopItem()
	{
		
	}
	
	// metodo chamado so por inapps
	void ClickedShopInApp()
	{
		Debug.Log("clicou");
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
