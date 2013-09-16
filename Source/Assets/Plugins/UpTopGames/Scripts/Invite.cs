using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class Invite : MonoBehaviour
{
	public GameObject loadingDialog;
	public GameObject messageOkDialog;
	public GameObject messageOkCancelDialog;
	public UIInteractivePanel nextPanel;
	public UIInteractivePanel multiplayerPanel;
	public UIScrollList scroll;
	public UIScrollList playingScroll;
	public GameObject friendPrefab;
	public GameObject letterPrefab;
	public GameObject inviteFriendPrefab;
	
	public UIStateToggleBtn allPlayingButton;
	
	public UIScrollList inviteFriendScroll;
	
	public UITextField searchText;
	
	public GameObject findFriendsPanel;
	public GameObject noFriendsText;
	public GameObject inviteFriendPanel;
	
	// Facebook
	protected GameFacebook fb_account;
	
	// Use this for initialization
	void Start () 
	{
		UIInteractivePanel panel = GetComponent<UIInteractivePanel>();
		
		panel.transitions.list[0].AddTransitionStartDelegate(GetFriends);
		panel.transitions.list[1].AddTransitionStartDelegate(GetFriends);
		
		searchText.SetFocusDelegate(ClearText);
		searchText.AddValidationDelegate(TextChanged);
		
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
	
	string TextChanged(UITextField field, string text, ref int insertion)
	{
		Debug.Log("TO DO: FAZER O SEARCH");
		///Debug.Log(text);
		
		//Debug.Log(scroll.Count);
		
		/*for(int i = 0 ; i < removedScroll.Count ; i++)
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
		
		
		*/return text;
	}
	
	public void GetFriends(EZTransition transition)
	{
		Flow.game_native.startLoading(loadingDialog);
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/list.php", HandleGetFriends);
		conn.connect(null, 10);
	}
	
	// Obtem as informacoes dos amigos do usuario
	void HandleGetFriends(string error, IJSonObject data, object counter_o)
	{
		Debug.Log(data);
		int counter = (int) counter_o;
		
		Flow.game_native.stopLoading(loadingDialog);
		
		if (error != null || data == null)
		{
			if (counter > 0)
			{
				GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/list.php", HandleGetFriends);
				conn.connect(null, counter - 1);
				return;
			}
			
			Flow.game_native.showMessage(messageOkDialog, "Error", error);
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
				firstL.transform.FindChild("Letter").GetComponent<SpriteText>().Text = playingLetter.ToUpper ();
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
	
	void ClearText(UITextField field)
	{
		Debug.Log("vaaaaaai");
		field.Text = "";
	}
	
	GameObject CreateFriendContainer(IJSonObject friend)
	{
		GameObject t = GameObject.Instantiate(friendPrefab) as GameObject;
		t.GetComponent<Friend>().SetFriend(
			friend["user_id"].ToString(), 
			friend["facebook_id"].ToString(),
			friend["name"].ToString(),
			friend["from_facebook"].BooleanValue? FriendshipStatus.FACEBOOK: FriendshipStatus.STANDALONE,
			friend["is_playing"].BooleanValue,
			loadingDialog,
			messageOkDialog,
			messageOkCancelDialog);
		
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
	
	void ChangedAllPlaying()
	{
		if(allPlayingButton.StateName == "All")
		{
			playingScroll.gameObject.SetActive(false);
			scroll.gameObject.SetActive(true);
		}
		else
		{
			scroll.gameObject.SetActive(false);
			playingScroll.gameObject.SetActive(true);
		}
	}
	
	public void EraseFriendsList()
	{
		scroll.ClearList(true);
		playingScroll.ClearList(true);
	}
	
	public void EraseFriendsListAndLoad()
	{
		Flow.game_native.startLoading(loadingDialog);
		
		scroll.ClearList(true);
		playingScroll.ClearList(true);
		
		multiplayerPanel.AddTempTransitionDelegate(StopLoading);
	}
	
	void StopLoading(UIPanelBase panel, EZTransition transition)
	{
		Flow.game_native.stopLoading(loadingDialog);
	}
	
	void ChooseRandomFriend()
	{	
		Flow.game_native.startLoading(loadingDialog);
		
		(new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/random.php", HandleChooseRandom)).connect();
	}
	
	void HandleChooseRandom(string error, IJSonObject data)
	{
		Debug.Log(data);
		
		Flow.game_native.stopLoading(loadingDialog);
		
		if (error != null || data.IsEmpty()) return;	
		
		string fail = "";
		try { fail = data.StringValue; } catch {}
		
		if(fail == "no more users")
		{
			Flow.game_native.showMessage(messageOkDialog, "Random fail","You'll have to invite a friend!");
			return;
		}
		
		string user_id = data["user_id"].StringValue;
		if (user_id == null) return;
		
		Flow.currentGame = new Game();
		Flow.currentGame.friend.id = user_id;
		Flow.currentGame.friend.is_playing = true;
		Flow.currentGame.friend.name = data["user_name"].StringValue;
		Flow.currentGame.friend.facebook_id = data["facebook_id"].StringValue;
		
		Flow.game_native.startLoading(loadingDialog);
		EraseFriendsList();
		
		Debug.Log(Flow.currentGame.friend.ToString());
		
		//mandar para próxima cena e colocar o temp delegate para parar o loading
	}
	
	void LinkWithFacebook()
	{
		Flow.game_native.startLoading(loadingDialog);
		fb_account.link();
	}

	// Obtem o resultado da vinculacao com o Facebook
	private void handleLinkFacebook(string error, IJSonObject data)
	{
		Debug.Log(data);
		
		Flow.game_native.stopLoading(loadingDialog);

		if (error != null)
		{
			Flow.game_native.showMessage(messageOkDialog, "Error", error);
			return;
		}
		
		EraseFriendsList();

		GetFriends(nextPanel.transitions.list[0]); //gambiarra: ele n precisa receber parâmetro nenhum, esse aqui é aleatório
	}
	
	void OpenInviteFriendsWindow()
	{
		inviteFriendPanel.SetActive(true);
		
		GameObject t = GameObject.Instantiate(inviteFriendPrefab) as GameObject;
		t.transform.FindChild("TextField").GetComponent<UITextField>().AddFocusDelegate(CreateInviteFriendsContainer);
		inviteFriendScroll.AddItem(t);
	}
	
	void CloseInviteFriendsWindow()
	{
		inviteFriendScroll.ClearList(true);
		inviteFriendPanel.SetActive(false);
	}
	
	void InviteNewFriends()
	{
		inviteFriendScroll.RemoveItem(inviteFriendScroll.Count-1, true);
		
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/invite.php", HandleInviteFriends);
		
		WWWForm form = new WWWForm();
		
		for(int i = 0; i<inviteFriendScroll.Count; i++)
		{
			form.AddField("emails[" + i + "]", inviteFriendScroll.GetItem(i).transform.FindChild("TextField").GetComponent<UITextField>().Text);
		}
		
		Flow.game_native.startLoading(loadingDialog);
		conn.connect(form);
	}
	
	private void HandleInviteFriends(string error, IJSonObject data)
	{
		Flow.game_native.stopLoading(loadingDialog);
		
		if (error != null)
		{
			Flow.game_native.showMessage(messageOkDialog, "Error", error);
			return;
		}
		
		if (data == null)
		{
			Flow.game_native.showMessage(messageOkDialog, "Error", GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE);
			return;
		}
		
		bool allRight = true;
		
		int i = 0;
		foreach (IJSonObject valid in data.ArrayItems)
		{
			if(valid["valid"].BooleanValue)
			{
				inviteFriendScroll.GetItem(i).transform.FindChild("Right Panel").gameObject.SetActive(true);
			}
			else
			{
				inviteFriendScroll.GetItem(i).transform.FindChild("Wrong Panel").gameObject.SetActive(true);
				allRight = false;
			}
			i++;
		}
		
		if (allRight)
		{
			Flow.game_native.showMessage(messageOkDialog, "Success", "All your friends has been invited!");
			CloseInviteFriendsWindow();
			EraseFriendsList();
			GetFriends(nextPanel.transitions.list[0]); //gambiarra: ele n precisa receber parâmetro nenhum, esse aqui é aleatório
			return;
		}
		
		Flow.game_native.showMessage(messageOkDialog, "Error", "Some e-mails are not valid.");
	}
	
	void CreateInviteFriendsContainer(UITextField field)
	{
		field.Text = "";
		
		Debug.Log(field.transform.parent.GetComponent<UIListItemContainer>().Index);
		
		if(field.transform.parent.GetComponent<UIListItemContainer>().Index >= inviteFriendScroll.Count-1)
		{
			GameObject t = GameObject.Instantiate(inviteFriendPrefab) as GameObject;
			t.transform.FindChild("TextField").GetComponent<UITextField>().AddFocusDelegate(CreateInviteFriendsContainer);
			inviteFriendScroll.AddItem(t);
		}
	}
}