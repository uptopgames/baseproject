using UnityEngine;
using System.Collections;
using CodeTitans.JSon;

public class UpdateOffline
{
	public void updateOfflineInApps()
	{
		Debug.Log("chamou");
		if(!Save.GetString(PlayerPrefsKeys.TOKEN.ToString()).IsEmpty())
		{
			Debug.Log("o cara ta logado, pega us iti deli");
			// Manda para o servidor as compras offline que o usuario daquele app fez
			GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/updateoffineinapps.php",OnUpdateOfflineInApps);
			WWWForm form = new WWWForm();
			
			int key = 0;
			// Adicionar nesse form os ids dos itens que o usuario comprou, salvos no Save
#if !UNITY_WEBPLAYER
			for(int i = 0 ; i < Flow.localThemes.Count ; i++)
			{
				if(Save.HasKey("purchasedTheme"+i))
				{
					form.AddField("items["+key+"][id]",i);
					form.AddField("items["+key+"][count]",1);
					form.AddField("items["+key+"][date]",Save.GetString("datePurchasedTheme"+i));
					key++;
				}
			}
			
			if(Save.HasKey("purchasedItems"))
			{
				//Debug.Log("hints menu: "+Save.GetInt("purchasedHints"));
				//Debug.Log("data hints menu: "+Save.GetString("datePurchasedHints"));
				form.AddField("items["+key+"][id]","consumable_1");
				form.AddField("items["+key+"][count]",Save.GetInt("purchasedItems"));
				form.AddField("items["+key+"][date]",Save.GetString("datePurchasedItems"));
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
		
	void OnUpdateOfflineInApps(string error, IJSonObject data)
	{
		if(error != null) Debug.Log(error);
		else
		{
			Debug.Log("meu deus do ceu: "+data);
			
			// Salvar retorno no Save (as comparacoes ja foram feitas no php, portanto o que retornar sera o mais atualizado)
			// O php retornara item_id, type, count e date - caso qualquer um seja necessario para suas coisas. Ver ex. abaixo:
			//
			// EX: data[0]["item_id"] --> id do item, como esta na tabela
			//
			// O padrao eh salvar no Save o item_id e o count (no caso de itens consumiveis)
			
			for(int i = 0 ; i < data.Count ; i++)
			{
				if(data[i]["count"].Int32Value != -1)
				{
					if(data[i]["item_id"].StringValue == "consumable_1")
					{
						Save.Set("purchasedItems", data[i]["count"].Int32Value);
						Save.Set("datePurchasedItems", data[i]["date"].StringValue);
					}
					else
					{
						//Save.Set("purchasedTheme"+data[i]["item_id"].Int32Value, true);
						//Save.Set("datePurchasedTheme"+data[i]["item_id"].Int32Value, data[i]["date"].StringValue);
					}
				}
			}
			
			Save.SaveAll();
		}
	}
}
