using UnityEngine;
using System.Collections;
using CodeTitans.JSon;

public class UpdateOffline
{
	public void UpdateOfflineItems()
	{
		Debug.Log("chamou");
		if(!Save.GetString(PlayerPrefsKeys.TOKEN).IsEmpty())
		{
			Debug.Log("o cara ta logado, pega us iti deli");
			// Manda para o servidor as compras offline que o usuario daquele app fez
			GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/shop/updateuseritems.php",OnUpdateOfflineItems);
			WWWForm form = new WWWForm();
			
			int key = 0;
			// Adicionar nesse form os ids dos itens que o usuario comprou, salvos no Save
#if !UNITY_WEBPLAYER
			ShopItem[] allItems = Flow.config.GetComponent<ConfigManager>().shopItems;
			
			for(int i = 0 ; i < allItems.Length ; i++)
			{
				if(Save.HasKey(PlayerPrefsKeys.ITEM+allItems[i].id))
				{	
					form.AddField("items["+key+"][id]",allItems[i].id);
					form.AddField("items["+key+"][count]", Save.GetInt(PlayerPrefsKeys.ITEM+allItems[i].id));
					key++;
				}
			}
#endif
			
			conn.connect(form);
			
			// Se a foto do usuario estiver nula, abrir conexao pra baixar ela
			if(Flow.playerPhoto == null && !Flow.isDownloadingPlayerPhoto)
			{
				GameRawAuthConnection conn2 = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", Flow.getPlayerPhoto);
                WWWForm form2 = new WWWForm();
                form2.AddField("user_id", "me");
                conn2.connect(form2);
                Flow.isDownloadingPlayerPhoto = true;
			}
			
		}
	}
		
	void OnUpdateOfflineItems(string error, IJSonObject data)
	{
		if(error != null) Debug.Log(error);
		else
		{
			Debug.Log("meu deus do ceu: "+data);
		}
	}
}
