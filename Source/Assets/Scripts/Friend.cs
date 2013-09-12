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
	private Material friendMaterial;
	
	// Indica se ja obteve a foto
	public bool got_picture;
	
	// Tempo da ultima imagem carregada
	private float last_time;
	
	public Friend SetFriend(string idNew, string facebook_idNew, string nameNew, FriendshipStatus statusNew, bool is_playingNew)
	{
		id = idNew;
		facebook_id = facebook_idNew;
		name.Text = nameNew;
		status = statusNew;
		is_playing = is_playingNew;
		
		last_time = -999;
		
		got_picture = false;
		
		return this;
	}
	
	// Obtem a foto caso necessario
	public void GetPicture()
	{
		if (got_picture) return;
		
		if (last_time > Time.realtimeSinceStartup - 10) return;
		last_time = Time.realtimeSinceStartup;
		
		//got_picture = true;
		
		GameRawAuthConnection conn = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", HandleGetPicture);
		
		WWWForm form = new WWWForm();
		form.AddField("user_id", id);
		
		conn.connect(form);
		
	}
	
	public void HandleGetPicture(string error, WWW conn)
	{	
		friendMaterial = new Material(friendMaterial);
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
}