using UnityEngine;
using System.Collections;

public class LoadingDialog : MonoBehaviour 
{
	/*void Awake ()
	{	
		// Checa para nao criar outro quando entrar em uma cena que tenha o prefab de config
		GameObject[] configs = GameObject.FindGameObjectsWithTag("LoadingDialog");
		foreach(GameObject g in configs)
		{
			if(gameObject != g)
			{
				GameObject.Destroy(gameObject);
				return;
			}
		}
		
		// Seta a prefab #Config# para nao ser destruida na troca de cenas
		DontDestroyOnLoad(gameObject);
		gameObject.SetActive(false);
		//Flow.loadingDialog = gameObject;
	}*/
}
