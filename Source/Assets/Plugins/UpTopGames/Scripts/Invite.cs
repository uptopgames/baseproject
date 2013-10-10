using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class Invite : MonoBehaviour
{
	public UIInteractivePanel nextPanel;
	public UIInteractivePanel multiplayerPanel;
	public UIScrollList scroll;
	public UIScrollList playingScroll;
	public GameObject friendPrefab;
	public GameObject letterPrefab;
	public GameObject inviteFriendPrefab;
	
	public UIStateToggleBtn allPlayingButton;
	
	public UIScrollList inviteFriendScroll;
	
	//public UITextField searchText;
	
	public GameObject findFriendsPanel;
	public GameObject noFriendsText;
	public GameObject inviteFriendPanel;
	
	public GameObject repositoryLists;
	
	// Facebook
	protected GameFacebook fb_account;
	
	// lista de amigos novos convidados (atraves do botao de invite e da popup)
	List<Friend> newFriends = new List<Friend>();
	
	// Use this for initialization
	void Start () 
	{		
		scroll = Flow.config.GetComponent<ConfigManager>().inviteAllScroll;
		playingScroll = Flow.config.GetComponent<ConfigManager>().invitePlayingScroll;
		
		scroll.transform.parent = gameObject.transform;
		playingScroll.transform.parent = gameObject.transform;
		
		scroll.transform.position = new Vector3
		(
			// primeira coluna refere-se a posicao do panel manager no mundo
			// segunda coluna refere-se a posicao do invite em relacao ao panel manager
			// terceira coluna refere-se a posicao do scroll em relacao ao invite panel
			-31.4336f 		+65.6814f		-0.7232132f, // x
			-0.9254344f 	+0.9254344f  	+0.7468368f, // y
			914.5213f 		+5.99231f		-7.011475f // z
		);
				
		playingScroll.transform.position = new Vector3
		(
			// primeira coluna refere-se a posicao do panel manager no mundo
			// segunda coluna refere-se a posicao do invite em relacao ao panel manager
			// terceira coluna refere-se a posicao do scroll em relacao ao invite panel
			-31.4336f 		+65.6814f 		-0.7232132f, 	// x
			-0.9254344f		+0.9254344f 	 	+0.7468368f, 	// y
			914.5213f 		+5.99231f 		-7.011475f 		// z
		);
		
		UIInteractivePanel panel = GetComponent<UIInteractivePanel>();
		
		panel.transitions.list[0].AddTransitionStartDelegate(EnteredInvite);
		panel.transitions.list[1].AddTransitionStartDelegate(EnteredInvite);
		
		inviteFriendPanel.GetComponent<UIInteractivePanel>().transitions.list[2].AddTransitionEndDelegate(SetInviteFriendsInactive);
		
		//searchText.SetFocusDelegate(ClearText);
		//searchText.AddValidationDelegate(TextChanged);
		
		ChangedAllPlaying();
		
		// Find Friends Button
		if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()))
		{
			findFriendsPanel.SetActive(false);
		}
		else
		{
			fb_account = new GameFacebook(handleLinkFacebook);
		}
	}
	
	/*string TextChanged(UITextField field, string text, ref int insertion)
	{
		Debug.Log("TO DO: FAZER O SEARCH");
		///Debug.Log(text);
		
		//Debug.Log(scroll.Count);
		
		for(int i = 0 ; i < removedScroll.Count ; i++)
		{
			if(removedScroll.GetItem(i).transform.FindChild("Name").GetComponent<SpriteText>().Text.Contains(text)) 
			{
				scroll.InsertItem(removedScroll.GetItem(i),removedScroll.GetItem(i).transform.GetComponent<Friend>().index);
				removedScroll.RemoveItem(removedScroll.GetItem(i),false);
			}
		}
		
		for(int i = 0 ; i < scroll.Count ; i++)
		{
			if(scroll.GetItem(i).transform.tag != "Letter")
			{
				//Debug.Log(scroll.GetItem(i).transform.FindChild("Name").GetComponent<SpriteText>().Text);
				if(!scroll.GetItem(i).transform.FindChild("Name").GetComponent<SpriteText>().Text.Contains(text)) 
				{
					scroll.GetItem(i).transform.GetComponent<Friend>().index = i;
					removedScroll.AddItem(scroll.GetItem(i));
					scroll.RemoveItem(scroll.GetItem(i),false);
				}
			}
		}
		
		
		return text;
	}*/
	
	public void GetFriends()
	{
		Flow.game_native.startLoading();
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/list.php", HandleGetFriends);
		conn.connect(null, 10);
	}
	
	public void EnteredInvite(EZTransition transition)
	{	
		scroll.gameObject.SetActive(true);
		
		if(scroll.Count == 0 && playingScroll.Count == 0)
		{
			GetFriends();
		}
	}
	
	// Obtem as informacoes dos amigos do usuario
	void HandleGetFriends(string error, IJSonObject data, object counter_o)
	{
		Debug.Log(data);
		int counter = (int) counter_o;
		
		Flow.game_native.stopLoading();
		
		if (error != null || data == null)
		{
			if (counter > 0)
			{
				GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/list.php", HandleGetFriends);
				conn.connect(null, counter - 1);
				return;
			}
			
			Flow.game_native.showMessage("Error", error);
			return;
		}
		
		string allLetter = "";
		string playingLetter = "";
		if(data.Count>0)
		{
			allLetter = data[0]["name"].StringValue.Substring(0,1).ToUpper();
			GameObject firstL = GameObject.Instantiate(letterPrefab) as GameObject;
			
			firstL.transform.FindChild("Letter").GetComponent<SpriteText>().Text = allLetter.ToUpper ();
			scroll.AddItem(firstL);
			
			if(data[0]["is_playing"].BooleanValue)
			{
				playingLetter = data[0]["name"].StringValue.Substring(0,1).ToUpper();
				GameObject firstPlayingL = GameObject.Instantiate(letterPrefab) as GameObject;
				firstPlayingL.transform.FindChild("Letter").GetComponent<SpriteText>().Text = playingLetter.ToUpper ();
				playingScroll.AddItem(firstPlayingL);
			}
		}
		
		foreach (IJSonObject friend in data.ArrayItems)
		{
			GameObject allContainer = CreateFriendContainer(friend);
			
			if(friend["name"].StringValue.Substring(0,1).ToUpper() != allLetter)
			{
				allLetter = friend["name"].StringValue.Substring(0,1).ToUpper();
				GameObject l = GameObject.Instantiate(letterPrefab) as GameObject;
				l.transform.FindChild("Letter").GetComponent<SpriteText>().Text = allLetter.ToUpper ();
				scroll.AddItem(l);
			}
			scroll.AddItem(allContainer);
			
			if(friend["is_playing"].BooleanValue) 
			{
				if(friend["name"].StringValue.Substring(0,1).ToUpper() != playingLetter)
				{
					playingLetter = friend["name"].StringValue.Substring(0,1).ToUpper();
					GameObject l = GameObject.Instantiate(letterPrefab) as GameObject;
					l.transform.FindChild("Letter").GetComponent<SpriteText>().Text = playingLetter.ToUpper ();
					playingScroll.AddItem(l);
				}
				GameObject playingContainer = CreateFriendContainer(friend);
				playingScroll.AddItem(playingContainer);
			}
		}
		
		if(data.Count==0)
		{
			noFriendsText.SetActive(true);
		}
		else
		{
			noFriendsText.SetActive(false);
		}
	}
	
	/*void ClearText(UITextField field)
	{
		Debug.Log("vaaaaaai");
		field.Text = "";
	}*/
	
	GameObject CreateFriendContainer(IJSonObject friend)
	{
		GameObject t = GameObject.Instantiate(friendPrefab) as GameObject;
		t.GetComponent<Friend>().SetFriend(
			friend["user_id"].ToString(), 
			friend["facebook_id"].ToString(),
			friend["name"].ToString(),
			friend["from_facebook"].BooleanValue? FriendshipStatus.FACEBOOK: FriendshipStatus.STANDALONE,
			friend["is_playing"].BooleanValue
			//null,
			);
		
		t.transform.FindChild("Name").GetComponent<SpriteText>().Text = friend["name"].ToString();
		
		if(t.GetComponent<Friend>().status == FriendshipStatus.FACEBOOK)
		{
			t.transform.FindChild("FacebookIcon").gameObject.SetActive(true);
		}
		else
		{
			t.transform.FindChild("StandaloneIcon").gameObject.SetActive(true);
		}
		
		return t;
	}
	
	GameObject CreateFriendContainer(Friend friend)
	{
		GameObject t = GameObject.Instantiate(friendPrefab) as GameObject;
		t.GetComponent<Friend>().SetFriend(
			friend.id, 
			friend.facebook_id,
			friend.name,
			friend.status,
			friend.is_playing);
		
		t.transform.FindChild("Name").GetComponent<SpriteText>().Text = friend.name;
		
		if(t.GetComponent<Friend>().status == FriendshipStatus.FACEBOOK)
		{
			t.transform.FindChild("FacebookIcon").gameObject.SetActive(true);
		}
		else
		{
			t.transform.FindChild("StandaloneIcon").gameObject.SetActive(true);
		}
		
		return t;
	}
	
	void ChangedAllPlaying()
	{
		if(allPlayingButton.StateName == "All")
		{
			if(scroll.Count == 0) noFriendsText.SetActive(true);
			else noFriendsText.SetActive(false);
			
			playingScroll.gameObject.SetActive(false);
			scroll.gameObject.SetActive(true);
			
		}
		else
		{
			if(playingScroll.Count == 0) noFriendsText.SetActive(true);
			else noFriendsText.SetActive(false);
			
			scroll.gameObject.SetActive(false);
			playingScroll.gameObject.SetActive(true);
		}
	}
	
	/*public void RemoveFromFriendList()
	{
		// retira objetos do scroll sem destrui-los para posteriormente adiciona-los quando voltar a essa cena
		
		foreach(UIListItemContainer item in playingScroll)
		{
			item.gameObject.tag = "playingScroll";
		}
		
		playingScroll.ClearList(false);
		
		foreach(UIListItemContainer item in scroll)
		{
			item.gameObject.tag = "allScroll";
		}
		
		scroll.ClearList(false);
	}*/
	
	public void EraseFriendsList()
	{
		//Debug.Log("EraseFriendsList");
		
		scroll.transform.gameObject.SetActive(false);
		playingScroll.transform.gameObject.SetActive(false);
		
		scroll.transform.parent = repositoryLists.transform;
		playingScroll.transform.parent = repositoryLists.transform;
		//scroll.ClearList(false);
		//playingScroll.ClearList(false);
	}
	
	public void EraseFriendsListAndLoad()
	{
		Debug.Log("EraseFriendsListAndLoad");
		//Flow.game_native.startLoading(loadingDialog);
		
		//scroll.ClearList(false);
		//playingScroll.ClearList(false);
		
		//multiplayerPanel.AddTempTransitionDelegate(StopLoading);
	}
	
	void StopLoading(UIPanelBase panel, EZTransition transition)
	{
		Flow.game_native.stopLoading();
	}
	
	void ChooseRandomFriend()
	{	
		Flow.game_native.startLoading();
		
		(new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/random.php", HandleChooseRandom)).connect();
	}
	
	void HandleChooseRandom(string error, IJSonObject data)
	{
		Debug.Log(data);
		
		Flow.game_native.stopLoading();
		
		if (error != null || data.IsEmpty()) return;	
		
		string fail = "";
		try { fail = data.StringValue; } catch {}
		
		if(fail == "no more users")
		{
			Flow.game_native.showMessage("Random fail","You'll have to invite a friend!");
			return;
		}
		
		string user_id = data["user_id"].StringValue;
		if (user_id == null) return;
		
		Flow.currentGame = new Game();
		Flow.currentGame.friend.id = user_id;
		Flow.currentGame.friend.is_playing = true;
		Flow.currentGame.friend.name = data["user_name"].StringValue;
		Flow.currentGame.friend.facebook_id = data["facebook_id"].StringValue;
		
		Flow.game_native.startLoading();
		//EraseFriendsList();
		
		Debug.Log(Flow.currentGame.friend.ToString());
		
		//mandar para próxima cena e colocar o temp delegate para parar o loading
	}
	
	void LinkWithFacebook()
	{
		Flow.game_native.startLoading();
		fb_account.link();
	}

	// Obtem o resultado da vinculacao com o Facebook
	private void handleLinkFacebook(string error, IJSonObject data)
	{
		Debug.Log(data);
		
		Flow.game_native.stopLoading();

		if (error != null)
		{
			Flow.game_native.showMessage("Error", error);
			return;
		}
		
		//EraseFriendsList();

		//GetFriends(nextPanel.transitions.list[0]); //gambiarra: ele n precisa receber parâmetro nenhum, esse aqui é aleatório
	}
	
	void OpenInviteFriendsWindow()
	{
		inviteFriendPanel.SetActive(true);
		
		inviteFriendPanel.GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		
		GameObject t = GameObject.Instantiate(inviteFriendPrefab) as GameObject;
		t.transform.FindChild("TextField").GetComponent<UITextField>().AddFocusDelegate(CreateInviteFriendsContainer);
		inviteFriendScroll.AddItem(t);
	}
	
	void CloseInviteFriendsWindow()
	{
		AddNewFriendsAndClearList();
		inviteFriendScroll.ClearList(true);
		
		inviteFriendPanel.GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}
	
	void SetInviteFriendsInactive(EZTransition transition)
	{
		inviteFriendPanel.SetActive(false);
	}
	
	void InviteNewFriends()
	{
		for (int r = 0 ; r < inviteFriendScroll.Count ; r++) 
		{
			if(inviteFriendScroll.GetItem(r).transform.FindChild("TextField").GetComponent<UITextField>().Text == "Friend E-mail")
			{
				inviteFriendScroll.RemoveItem(r, true);
			}
			
		}
		
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/invite.php", HandleInviteFriends);
		
		WWWForm form = new WWWForm();
		
		for(int i = 0; i<inviteFriendScroll.Count; i++)
		{
			form.AddField("emails[" + i + "]", inviteFriendScroll.GetItem(i).transform.FindChild("TextField").GetComponent<UITextField>().Text);
		}
		
		Flow.game_native.startLoading();
		conn.connect(form);
	}
	
	private void HandleInviteFriends(string error, IJSonObject data)
	{
		Debug.Log("inviteNew: "+data);
		Flow.game_native.stopLoading();
		
		if (error != null)
		{
			Flow.game_native.showMessage("Error", error);
			return;
		}
		
		if (data == null)
		{
			Flow.game_native.showMessage("Error", GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE);
			return;
		}
		
		bool allRight = true;
		
		int i = 0;
		foreach (IJSonObject valid in data.ArrayItems)
		{
			if(valid["id"].Int32Value == 0)
			{
				inviteFriendScroll.GetItem(i).transform.FindChild("Right Panel").gameObject.SetActive(true);
			}
			else
			{
				if(valid["valid"].BooleanValue)
				{
					inviteFriendScroll.GetItem(i).transform.FindChild("Right Panel").gameObject.SetActive(true);
					
					Friend tempFriend = new Friend();
					tempFriend = tempFriend.SetFriend(valid["id"].StringValue,valid["facebook_id"].StringValue,valid["name"].StringValue,FriendshipStatus.STANDALONE,valid["is_playing"].BooleanValue/*,null,loadingDialog,messageOkDialog,messageOkCancelDialog*/);
					
					bool found = false;
					for(int y = 0 ; y < newFriends.Count ; y++)
					{
						if(newFriends[y].id == valid["id"].StringValue) found = true;
					}
					
					if(!found) newFriends.Add(tempFriend);
				}
				else
				{
					inviteFriendScroll.GetItem(i).transform.FindChild("Wrong Panel").gameObject.SetActive(true);
					allRight = false;
				}
			}
			
			i++;
		}
		
		if (allRight)
		{
			Flow.game_native.showMessage("Success", "All your friends has been invited!");
			CloseInviteFriendsWindow();
						
			return;
		}
		
		Flow.game_native.showMessage("Error", "Some e-mails are not valid.");
	}
	
	void AddNewFriendsAndClearList()
	{
		for(int i = 0 ; i < newFriends.Count ; i++)
		{
			if(newFriends[i].is_playing)
			{
				bool placeSettledPlaying = false;
				GameObject tempPlayingObj = CreateFriendContainer(newFriends[i]);
				int oldPlayingComparison = -1;
				
				string lastLetterPlaying;
				if(playingScroll.Count > 0 ) lastLetterPlaying = playingScroll.GetItem(0).Text;
				else lastLetterPlaying = "";
				
				for(int j = 0 ; j < playingScroll.Count ; j++)
				{
					int playingComparison;
					
					if(!playingScroll.GetItem(j).transform.GetComponent<Friend>()) 
					{
						playingComparison = playingScroll.GetItem(j).Text.CompareTo(newFriends[i].name);
					}
					else playingComparison = playingScroll.GetItem(j).transform.GetComponent<Friend>().name.CompareTo(newFriends[i].name);
					
					if(oldPlayingComparison <= 0 && playingComparison == 1)
					{
						if(lastLetterPlaying.ToUpper() != newFriends[i].name.Substring(0,1).ToUpper())
						{
							// tem que adicionar container de letra antes
							GameObject tempPlayingLetter = GameObject.Instantiate(letterPrefab) as GameObject;
							tempPlayingLetter.transform.FindChild("Letter").GetComponent<SpriteText>().Text = newFriends[i].name.Substring(0,1).ToUpper ();
							playingScroll.InsertItem(tempPlayingLetter.GetComponent<UIListItemContainer>(),j);
							playingScroll.InsertItem(tempPlayingObj.GetComponent<UIListItemContainer>(),j+1);
						}
						else playingScroll.InsertItem(tempPlayingObj.GetComponent<UIListItemContainer>(),j);
						
						placeSettledPlaying = true;
						break;
					}
					
					if(!playingScroll.GetItem(j).transform.GetComponent<Friend>()) lastLetterPlaying = playingScroll.GetItem(j).Text;
					
					oldPlayingComparison = playingComparison;
				}
				
				if(!placeSettledPlaying)
				{
					if(lastLetterPlaying.ToUpper() != newFriends[i].name.Substring(0,1).ToUpper())
					{
						// tem que adicionar container de letra antes
						GameObject tempPlayingLetter = GameObject.Instantiate(letterPrefab) as GameObject;
						tempPlayingLetter.transform.FindChild("Letter").GetComponent<SpriteText>().Text = newFriends[i].name.Substring(0,1).ToUpper ();
						playingScroll.AddItem(tempPlayingLetter.GetComponent<UIListItemContainer>());
						playingScroll.AddItem(tempPlayingObj.GetComponent<UIListItemContainer>());
					}
					else playingScroll.AddItem(tempPlayingObj.GetComponent<UIListItemContainer>());
				}
			}
			
			bool placeSettled = false;
			GameObject tempObj = CreateFriendContainer(newFriends[i]);
			int oldComparison = -1;
			
			string lastLetter;
			if(scroll.Count > 0) lastLetter = scroll.GetItem(0).Text;
			else lastLetter = "";
			
			for(int k = 0 ; k < scroll.Count ; k++)
			{
				int comparison;
				if(!scroll.GetItem(k).transform.GetComponent<Friend>()) 
				{
					comparison = scroll.GetItem(k).Text.CompareTo(newFriends[i].name);
				}
				else comparison = scroll.GetItem(k).transform.GetComponent<Friend>().name.CompareTo(newFriends[i].name);
				
				if(oldComparison <= 0 && comparison == 1)
				{
					if(lastLetter.ToUpper() != newFriends[i].name.Substring(0,1).ToUpper())
					{
						// tem que adicionar container de letra antes
						GameObject tempLetter = GameObject.Instantiate(letterPrefab) as GameObject;
						tempLetter.transform.FindChild("Letter").GetComponent<SpriteText>().Text = newFriends[i].name.Substring(0,1).ToUpper ();
						scroll.InsertItem(tempLetter.GetComponent<UIListItemContainer>(),k);
						scroll.InsertItem(tempObj.GetComponent<UIListItemContainer>(),k+1);
					}
					else scroll.InsertItem(tempObj.GetComponent<UIListItemContainer>(),k);
					
					placeSettled = true;
					break;
				}
				
				if(!scroll.GetItem(k).transform.GetComponent<Friend>()) lastLetter = scroll.GetItem(k).Text;
				
				oldComparison = comparison;
			}
			
			if(!placeSettled)
			{
				if(lastLetter.ToUpper() != newFriends[i].name.Substring(0,1).ToUpper())
				{
					// tem que adicionar container de letra antes
					GameObject tempLetter = GameObject.Instantiate(letterPrefab) as GameObject;
					tempLetter.transform.FindChild("Letter").GetComponent<SpriteText>().Text = newFriends[i].name.Substring(0,1).ToUpper ();
					scroll.AddItem(tempLetter.GetComponent<UIListItemContainer>());
					scroll.AddItem(tempObj.GetComponent<UIListItemContainer>());
				}
				else scroll.AddItem(tempObj.GetComponent<UIListItemContainer>());
			}
		}
		
		newFriends.Clear();
	}
	
	void CreateInviteFriendsContainer(UITextField field)
	{
		if(field.Text == "Friend E-mail") field.Text = "";
		
		//Debug.Log(field.transform.parent.GetComponent<UIListItemContainer>().Index);
		
		if(field.transform.parent.GetComponent<UIListItemContainer>().Index >= inviteFriendScroll.Count-1)
		{
			GameObject t = GameObject.Instantiate(inviteFriendPrefab) as GameObject;
			t.transform.FindChild("TextField").GetComponent<UITextField>().AddFocusDelegate(CreateInviteFriendsContainer);
			inviteFriendScroll.AddItem(t);
		}
	}
}