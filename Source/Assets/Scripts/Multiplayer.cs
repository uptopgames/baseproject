using UnityEngine;
using System.Collections;

public class Multiplayer : MonoBehaviour 
{
	public UIPanelManager panelManager;
	
	public GameObject loadingDialog;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void GetFriendList()
	{
		//Flow.game_native.startLoading(loadingDialog);
		// TO DO: Chamar conexao que pega os amigos da pessoa para mostrar na cena de invite
		
		panelManager.BringIn("InviteScenePanel");
	}
}
