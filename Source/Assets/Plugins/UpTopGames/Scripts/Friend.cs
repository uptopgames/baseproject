using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

// Tipo da amizade
public enum FriendshipStatus
{
	NONE, STANDALONE, FACEBOOK
}

// Estado do convite
public enum InviteStatus
{
	PENDING, ACCEPTED, DENIED
}

public class Friend : MonoBehaviour
{
	///public static FacebookAPI facebook = new FacebookAPI();
	
	// Identificado global do usuario, pode ser nulo
	public string id;
	
	// Identificador do usuario no Facebook, pode ser nulo
	public string facebook_id;
	
	// Nome do usuario para exibicao
	public string name;
	
	// Indice do amigo na lista
	public int index;
	
	// Situacao da amizade
	public FriendshipStatus status;
	
	// Indica se o usuario no jogo
	public bool is_playing;
	
	public MeshRenderer picture;
	public Material baseMaterial;
	private Material friendMaterial;
	
	// Indica se ja obteve a foto
	public bool got_picture;
	
	public Texture rawText; 
	
	private bool isDownloadingPicture = false;
	
	private float imageLoadCounter = 1;
	
	public GameObject loadWindow;
	public GameObject messageOkWindow;
	public GameObject messageOkCancelWindow;
	
	public Friend SetFriend(string idNew, string facebook_idNew, string nameNew, FriendshipStatus statusNew, bool is_playingNew /*Texture newPicture,*/)
	{
		id = idNew;
		facebook_id = facebook_idNew;
		name = nameNew;
		status = statusNew;
		is_playing = is_playingNew;
		//if(newPicture != null) picture.material.mainTexture = newPicture;
				
		//got_picture = false;//picture.material.mainTexture != null;
		
		return this;
	}
	
	public void Update()
	{
		if(!isDownloadingPicture)
		{
			imageLoadCounter -= Time.deltaTime;
			if(imageLoadCounter<=0)
			{
				isDownloadingPicture = true;
				GetPicture();
				//Debug.Log("downloade esse cara " + id);
			}
		}
	}
	
	public void OnDisable()
	{
		imageLoadCounter = 1;
	}
	
	// Obtem a foto caso necessario
	public void GetPicture()
	{
		if (got_picture) return;
		
		//got_picture = true;
		
		GameRawAuthConnection conn = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", HandleGetPicture);
		
		WWWForm form = new WWWForm();
		
		if(id != null) form.AddField("user_id", id);
		else form.AddField("facebook_id", facebook_id);
		
		conn.connect(form);
	}
	
	public void HandleGetPicture(string error, WWW conn)
	{
		if(this==null) return; //se o scroll foi deletado, mas a coroutine foi chamada (apesar da instancia do script n existir)
		
		//Debug.Log(conn.text);
		friendMaterial = new Material(baseMaterial);
		picture.material = friendMaterial;
		
		if (error != null || conn.error != null || conn.bytes.Length == 0)
		{
			/*Texture2D tempTexture = new Texture2D(1,1);
			tempTexture.SetPixel(0,0, Color.clear);
			tempTexture.Apply();
			
			picture.material.mainTexture = tempTexture;*/
			
			return;
		}
		
		got_picture = true;
		
		picture.material.mainTexture = conn.texture;
		
		if(GetComponent<Game>()) 
		{
			//Debug.Log("atribuiu no flow");
			Flow.gameList[GetComponent<Game>().pastIndex].friend.rawText = conn.texture;
			Flow.gameList[GetComponent<Game>().pastIndex].friend.got_picture = got_picture;
		}
	}
	
	void ChooseFriend()
	{		
		// Se ja tem identificador, retorna
		if (id != null)
		{
			AssignDataToFlow();
			return;
		}
		
		Debug.Log("nao tem id, vamos criar...");
		//Flow.game_native.startLoading();
		
		// Cria o identificador para o usuario
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/create.php", HandleChoose);
		WWWForm form = new WWWForm();
		form.AddField("facebook_id", facebook_id);
		conn.connect(form);
	}
	
	void HandleChoose(string error, IJSonObject data)
	{
		//GameGUI.game_native.stopLoading();
		
		if (error != null)
		{
			//GameGUI.game_native.showMessage("Error", error);
			return;
		}
		
		id = data["user_id"].ToString();
		Debug.Log("e o id novo do cara é..."+id);
		
		AssignDataToFlow();
	}
	
	void AssignDataToFlow()
	{
		if(name.Contains("@"))
		{
			string[] ar = {};
			ar = name.Split('@');
			name = ar[0];
		}
		Flow.currentGame.friend = this;
		GetComponent<UIListItemContainer>().GetScrollList().transform.parent.GetComponent<Invite>().EraseFriendsList();
		UIPanelManager.instance.BringIn("WorldSelectionScenePanel");
	}
	
	public void DeleteFriend()
	{
		Flow.game_native.showMessageOkCancel(this, "ConfirmDeletion" , ConfirmDeletionDelegate, "", "Are you sure?",
			"This is going to delete \"" + name + "\" from your list.");
	}
	
	public void ConfirmDeletionDelegate(string buttonPressed)
	{
		if(buttonPressed.ToLower() == "ok") ConfirmDeletion();
	}
	
	public void ConfirmDeletion()
	{
		Debug.Log("deletando");
		messageOkCancelWindow.SetActive(false);
		
		Flow.game_native.startLoading();
		
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/delete.php", handleDeleteFriendConnection);

		WWWForm form = new WWWForm();
		form.AddField("friend_id", id);

		conn.connect(form);
	}
	
	void handleDeleteFriendConnection(string error, IJSonObject data)
	{
		Flow.game_native.stopLoading();

		if (error != null)
		{
			Flow.game_native.showMessage("Error", error);
			return;
		}

		// Remove o amigo da lista all e da lista playing independentemente se você clicou no delete de uma ou de outra.
		
		if(gameObject.GetComponent<UIListItemContainer>().GetScrollList().transform.tag == "FriendList")
		{
			UIScrollList playingScroll = gameObject.GetComponent<UIListItemContainer>().GetScrollList().transform.parent.FindChild("PlayingList").GetComponent<UIScrollList>();
						
			for(int i = 0 ; i < playingScroll.Count ; i++)
			{
				if(playingScroll.GetItem(i).transform.GetComponent<Friend>() && id == playingScroll.GetItem(i).transform.GetComponent<Friend>().id)
				{
					// se for 2, soh tem um container e uma letra
					if(playingScroll.Count != 2)
					{
						// se o proximo cara nao tiver o component friend, indica que ele é uma letra
						if(!playingScroll.GetItem(i+1).transform.GetComponent<Friend>())
						{
							// se o cara anterior nao tiver o component friend, indica que ele é uma letra
							if(!playingScroll.GetItem(i-1).transform.GetComponent<Friend>())
							{
								// remove a letra passada pois o container sendo removido é o unico da letra
								playingScroll.RemoveItem(i-1,true);
								// o cara passou a ter o indice da letra, remove ele
								playingScroll.RemoveItem(i-1,true);
								
								break;
							}
						}
					}
					else
					{
						// remove letra
						playingScroll.RemoveItem(0, true);
						// o cara passou a ter o indice da letra, remove ele
						playingScroll.RemoveItem(0, true);
						
						break;
					}
					
					// remove o container
					playingScroll.RemoveItem(i,true);
					break;
				}
			}
		}
		else
		{
			UIScrollList allScroll = gameObject.GetComponent<UIListItemContainer>().GetScrollList().transform.parent.FindChild("FriendList").GetComponent<UIScrollList>();
			
			for(int i = 0 ; i < allScroll.Count ; i++)
			{
				if(allScroll.GetItem(i).transform.GetComponent<Friend>() && id == allScroll.GetItem(i).transform.GetComponent<Friend>().id)
				{
					// se for 2, soh tem um container e uma letra
					if(allScroll.Count != 2)
					{
						// se o proximo cara nao tiver o component friend, indica que ele é uma letra
						if(!allScroll.GetItem(i+1).transform.GetComponent<Friend>())
						{
							// se o cara anterior nao tiver o component friend, indica que ele é uma letra
							if(!allScroll.GetItem(i-1).transform.GetComponent<Friend>())
							{
								// remove a letra passada pois o container sendo removido é o unico da letra
								allScroll.RemoveItem(i-1,true);
								// o cara passou a ter o indice da letra, remove ele
								allScroll.RemoveItem(i-1,true);
								
								break;
							}
						}
					}
					else
					{
						// remove a letra
						allScroll.RemoveItem(0, true);
						// o cara passou a ter o indice da letra, remove ele
						allScroll.RemoveItem(0, true);
						
						break;
					}
					
					// remove o container
					allScroll.RemoveItem(i,true);
					break;
				}
			}
		}
		
		// se for 2, soh tem um container e uma letra
		if(gameObject.GetComponent<UIListItemContainer>().GetScrollList().Count != 2)
		{
			// se o proximo cara nao tiver o component friend, indica que ele é uma letra
			if(!gameObject.GetComponent<UIListItemContainer>().GetScrollList().GetItem(GetComponent<UIListItemContainer>().Index+1).transform.GetComponent<Friend>())
			{
				// se o cara anterior nao tiver o component friend, indica que ele é uma letra
				if(!gameObject.GetComponent<UIListItemContainer>().GetScrollList().GetItem(GetComponent<UIListItemContainer>().Index-1).transform.GetComponent<Friend>())
				{
					// remove a letra passada pois o container sendo removido é o unico da letra
					gameObject.GetComponent<UIListItemContainer>().GetScrollList().RemoveItem(GetComponent<UIListItemContainer>().Index-1,true);
					// o cara passou a ter o indice da letra, remove ele (MAS NESSE CASO O EZGUI ATUALIZOU O INDEX)
					gameObject.GetComponent<UIListItemContainer>().GetScrollList().RemoveItem(GetComponent<UIListItemContainer>().Index,true);
					
					return;
				}
			}
		}
		else
		{
			// remove a letra
			gameObject.GetComponent<UIListItemContainer>().GetScrollList().RemoveItem(0, true);
			// o cara passou a ter o indice da letra, remove ele
			gameObject.GetComponent<UIListItemContainer>().GetScrollList().RemoveItem(0, true);
			
			return;
		}
		
		gameObject.GetComponent<UIListItemContainer>().GetScrollList().RemoveItem(GetComponent<UIListItemContainer>(),false);
		
	}
	
	public override string ToString()
	{
		return "id: " + id + "\nfacebook_id: " + facebook_id + "\nname: " + name + "\nstatus: " + status.ToString() + "\nis playing: " + is_playing;
	}
}