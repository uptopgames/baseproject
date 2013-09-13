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
	public SpriteText name;
	
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
	private bool isDownloadingPicture = false;
	
	// Tempo da ultima imagem carregada
	private float last_time;
	
	private float imageLoadCounter = 1;
	
	private GameObject loadWindow;
	private GameObject messageOkWindow;
	private GameObject messageOkCancelWindow;
	
	public Friend SetFriend(string idNew, string facebook_idNew, string nameNew, FriendshipStatus statusNew, bool is_playingNew,
		GameObject loadWindowNew, GameObject messageOkWindowNew, GameObject messageOkCancelWindowNew)
	{
		id = idNew;
		facebook_id = facebook_idNew;
		name.Text = nameNew;
		status = statusNew;
		is_playing = is_playingNew;
		loadWindow = loadWindowNew;
		messageOkWindow = messageOkWindowNew;
		messageOkCancelWindow = messageOkCancelWindowNew;
		
		last_time = -999;
		
		got_picture = false;
		
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
		//Debug.Log(conn.text);
		friendMaterial = new Material(baseMaterial);
		picture.material = friendMaterial;
		
		if (error != null || conn.error != null || conn.bytes.Length == 0)
		{
			Texture2D tempTexture = new Texture2D(1,1);
			tempTexture.SetPixel(0,0, Color.clear);
			tempTexture.Apply();
			
			picture.material.mainTexture = tempTexture;
			return;
		}
		
		got_picture = true;
		
		picture.material.mainTexture = conn.texture;
	}
	
	void ChooseFriend()
	{
		// Se ja tem identificador, retorna
		if (id != null)
		{
			//redirectScene(scene, id);
			Debug.Log(name.Text);
			Debug.Log(id);
			Flow.currentGame.friend = this;
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
		Flow.currentGame.friend = this;
	}
	
	public void DeleteFriend()
	{
		Flow.game_native.showMessageOkCancel(messageOkCancelWindow, this, "ConfirmDeletion" , ConfirmDeletionDelegate, "", "Are you sure?",
			"This is going to delete \"" + name.Text + "\" from your list.");
	}
	
	public void ConfirmDeletionDelegate(string buttonPressed)
	{
		if(buttonPressed.ToLower() == "ok") ConfirmDeletion();
	}
	
	public void ConfirmDeletion()
	{
		Debug.Log("deletando");
		messageOkCancelWindow.SetActive(false);
		
		Flow.game_native.startLoading(loadWindow);
		
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/delete.php", handleDeleteFriendConnection);

		WWWForm form = new WWWForm();
		form.AddField("friend_id", id);

		conn.connect(form);
	}
	
	void handleDeleteFriendConnection(string error, IJSonObject data)
	{
		Flow.game_native.stopLoading(loadWindow);

		if (error != null)
		{
			Flow.game_native.showMessage(messageOkWindow,"Error", error);
			return;
		}

		// Remove o amigo da lista de exibicao
		gameObject.GetComponent<UIListItemContainer>().GetScrollList().RemoveItem(gameObject.GetComponent<UIListItemContainer>(),false);
	}
}