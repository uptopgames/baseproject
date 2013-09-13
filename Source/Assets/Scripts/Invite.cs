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
	public UIScrollList scroll;
	public UIScrollList playingScroll;
	public GameObject friendPrefab;
	public GameObject letterPrefab;
	
	public UIStateToggleBtn allPlayingButton;
	
	public UIScrollList removedScroll;
	
	public UITextField searchText;
	
	// Use this for initialization
	void Start () 
	{
		UIInteractivePanel panel = GetComponent<UIInteractivePanel>();
		
		panel.transitions.list[0].AddTransitionStartDelegate(GetFriends);
		panel.transitions.list[1].AddTransitionStartDelegate(GetFriends);
		
		searchText.SetFocusDelegate(ClearText);
		searchText.AddValidationDelegate(TextChanged);
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
		
		scroll.sceneItems.ToList().Sort
		(
			delegate(GameObject o1, GameObject o2)
			{
				return o1.transform.FindChild("Name").GetComponent<SpriteText>().Text.CompareTo(o2.transform.FindChild("Name").GetComponent<SpriteText>().Text);
			}
		);
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
}
