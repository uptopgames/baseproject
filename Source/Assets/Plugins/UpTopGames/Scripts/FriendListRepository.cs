using UnityEngine;
using System.Collections;

public class FriendListRepository : MonoBehaviour 
{
	void Awake ()
	{
		// Checa para nao criar outro quando entrar em uma cena que tenha o prefab de config
		GameObject[] lists = GameObject.FindGameObjectsWithTag("RepoFLists");
		foreach(GameObject g in lists)
		{
			if(gameObject != g)
			{
				GameObject.Destroy(gameObject);
				return;
			}
		}
		
		// Seta a prefab #Configuration# para nao ser destruida na troca de cenas
		DontDestroyOnLoad(gameObject);
	}
}
