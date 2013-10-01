using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class Multiplayer : MonoBehaviour 
{
	public UIPanelManager panelManager;
	public GameObject loadingDialog;
	public GameObject messageOkDialog;
	public GameObject messageOkCancelDialog;
	public GameObject gamePrefab;
	public GameObject yourTurnPrefab;
	public GameObject theirTurnPrefab;
		
	public bool notInThisPanel = false;
	
	public SpriteText noGamesYet;
	
	public UIScrollList scroll;
	
	public BattleStatus battleStatus;
	
	// Tempo de espera entre consultas
	private const float MIN_WAITING_TIME = 4;
	private const float MAX_WAITING_TIME = 30;
	private const float START_WAITING_TIME = 10;
	private const float RATIO_WAITING_TIME = 1.25f;
	public static float waitingTime = START_WAITING_TIME;
	public static bool updatedAutomatically = false;
	
	// Facebook
	GameFacebook fb_account;
	
	// Use this for initialization
	void Start () 
	{
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(Connect);
		GetComponent<UIInteractivePanel>().transitions.list[1].AddTransitionStartDelegate(Connect);
		GetComponent<UIInteractivePanel>().transitions.list[2].AddTransitionStartDelegate(LeavePanel);
		GetComponent<UIInteractivePanel>().transitions.list[3].AddTransitionStartDelegate(LeavePanel);
	}
	
	void LeavePanel(EZTransition transition)
	{
		notInThisPanel = true;
	}
	
	void Connect(EZTransition transition)
	{
		notInThisPanel = false;
		Debug.Log ("gameListCount: " + Flow.gameList.Count);
		Debug.Log ("scrollCount: " + scroll.Count);
		
		if (Flow.gameList.Count > 0)
		{
			noGamesYet.Text = "";
				
			// Fix Me Up Top Coloquei um porque adicionei a forca um container offline
			if (scroll.Count == 0)
			{
				Debug.Log ("adicioneiVelhosContainersNaLista");
				
				sortList();
								
				for (int i = 0; i < Flow.gameList.Count; i++)
				{	
					if (Flow.gameList[i].id != -999) 
					{
						Debug.Log("coloquei " + Flow.gameList[i].friend.name);
						// seta past index na lista para atualizacoes que foram feitas em battle status...
						Flow.gameList[i].pastIndex = i;
						//scroll.InsertItem(Flow.gameList[i].GetComponent<UIListItemContainer>(), i);
						Debug.Log("picture flow: "+Flow.gameList[i].friend.rawText.ToString());
						CreateGameContainer (Flow.gameList[i], i);
					}
					else if(Flow.gameList[i].whoseMove == "your")
					{
						Debug.Log("coloquei label your " + i);
						AddTurnLabel("your", i);
					}
					else if(Flow.gameList[i].whoseMove == "their")
					{
						Debug.Log("coloquei label their " + i);
						AddTurnLabel("their", i);
					}
				}
			}
		}
		
		WWWForm form = new WWWForm();
		form.AddField("lastUpdate",TimeZoneInfo.ConvertTimeToUtc(Flow.lastUpdate).ToString());
		new GameJsonAuthConnection(Flow.URL_BASE + "base/getgames.php", OnReceiveGames).connect(form);
	}
	
	void updatingAutomatically()
	{
		if(notInThisPanel) return;
		
		updatedAutomatically = true;
		
		WWWForm form = new WWWForm();
		form.AddField("lastUpdate",TimeZoneInfo.ConvertTimeToUtc(Flow.lastUpdate).ToString());
		new GameJsonAuthConnection(Flow.URL_BASE + "base/getgames.php", OnReceiveGames).connect(form);
	}
	
	void OnReceiveGames(string error, IJSonObject data)
	{
		if(notInThisPanel) return;
		
		if(error != null) Debug.Log(error);
		else 
		{
			int oldYourTurnNumber = Flow.yourTurnGames;
			int oldTheirTurnNumber = Flow.theirTurnGames;
			
			Debug.Log(data);
			
			for(int i = 0 ; i < data["games"].Count ; i++)
			{
				if(!data["games"][i]["lastUpdate"].IsNull && Flow.lastUpdate < data["games"][i]["lastUpdate"].DateTimeValue)
				{
					Flow.lastUpdate = data["games"][i]["lastUpdate"].DateTimeValue;
				}
				
				string[] scores = new string[Flow.ROUNDS_PER_TURN];
				string[] times = new string[Flow.ROUNDS_PER_TURN];
				string[] pastMyScores = new string[Flow.ROUNDS_PER_TURN];
				string[] pastMyTimes = new string[Flow.ROUNDS_PER_TURN];
				string[] pastTheirScores = new string[Flow.ROUNDS_PER_TURN];
				string[] pastTheirTimes = new string[Flow.ROUNDS_PER_TURN];
				
				string faceID = "";
				int lastTurnID = -1;
				int tempWorldID = -1;
				string tempWorldName = "";
				string tempPastWorldName = "";
				
				List<Round> tempRoundList = new List<Round>();
				List<Round> tempPastMyRoundList = new List<Round>();
				List<Round> tempPastTheirRoundList = new List<Round>();
				
				string[] separator = {"|$@@$|"};
				
				if(data["games"][i]["turnStatus"].StringValue != "waitingChoice" && data["games"][i]["whoseMove"].StringValue != "their")
				{
					Debug.Log("adicionei rounds atuais");
					if(!data["games"][i]["scores"].IsNull) scores = data["games"][i]["scores"].StringValue.Split(separator,StringSplitOptions.None);
					if(!data["games"][i]["times"].IsNull) times = data["games"][i]["times"].StringValue.Split(separator,StringSplitOptions.None);
					
					for(int j = 0 ; j < Flow.ROUNDS_PER_TURN ; j++)
					{
						tempRoundList.Add (new Round (-1, data["games"][i]["turn"].Int32Value, data["games"][i]["friendID"].Int32Value, times[j].ToFloat(),
							scores[j].ToInt32()));
					}
				}
				if(!data["games"][i]["lastTurn"].IsNull) 
				{
					Debug.Log("adicionei os rounds passados");
					pastMyScores = data["games"][i]["myPastScores"].StringValue.Split(separator,StringSplitOptions.None);
					pastMyTimes = data["games"][i]["myPastTimes"].StringValue.Split(separator,StringSplitOptions.None);
					pastTheirScores = data["games"][i]["theirPastScores"].StringValue.Split(separator,StringSplitOptions.None);
					pastTheirTimes = data["games"][i]["theirPastTimes"].StringValue.Split(separator,StringSplitOptions.None);
					
					for(int k = 0 ; k < pastMyScores.Length ; k++)
					{
						tempPastMyRoundList.Add(new Round(-1,-1,-1,pastMyTimes[k].ToFloat(),int.Parse(pastMyScores[k])));
						tempPastTheirRoundList.Add(new Round(-1,-1,-1,pastTheirTimes[k].ToFloat(),int.Parse(pastTheirScores[k])));
					}
				}
					
				if(!data["games"][i]["facebookID"].IsNull) faceID = data["games"][i]["facebookID"].StringValue;
				if(!data["games"][i]["lastTurn"].IsNull) lastTurnID = data["games"][i]["lastTurn"].Int32Value;
				if(!data["games"][i]["world"].IsNull) tempWorldID = data["games"][i]["world"].Int32Value;
				if(!data["games"][i]["worldName"].IsNull) tempWorldName = data["games"][i]["worldName"].StringValue;
				if(!data["games"][i]["pastWorldName"].IsNull) tempPastWorldName = data["games"][i]["pastWorldName"].StringValue;
				
				Debug.Log("nome do cara "+data["games"][i]["username"].StringValue);
				
				bool foundGame = false;
				
				Friend tempFriend = new Friend();
				//GameObject tempObj = CreateGameContainer(data["games"][i]);
				
				//tempObj.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
				
				//Game tempGame = tempObj.GetComponent<Game>();
				
				tempFriend = tempFriend.SetFriend(
					data["games"][i]["friendID"].StringValue,
					faceID,
					data["games"][i]["username"].StringValue,
					FriendshipStatus.NONE,
					data["games"][i]["hasApp"].StringValue.ToBool(),
					//null,
					loadingDialog,
					messageOkDialog,
					messageOkCancelDialog);
					
				Game tempGame =  new Game
				(
					data["games"][i]["gameID"].Int32Value,
					tempFriend,
					tempWorldID,
					tempWorldName,
					new List<Round>(),
					tempRoundList,
					data["games"][i]["turn"].Int32Value,
					lastTurnID,
					data["games"][i]["turnsWon"].Int32Value,
					data["games"][i]["turnsLost"].Int32Value,
					data["games"][i]["whoseMove"].StringValue,
					data["games"][i]["turnStatus"].StringValue,
					data["games"][i]["lastUpdate"].DateTimeValue,
					tempPastMyRoundList,
					tempPastTheirRoundList,
					tempPastWorldName
				);
				
				for(int h = 0 ; h < Flow.gameList.Count ; h++)
				{
					//Debug.Log ("temGameFriendName" + h + ": " + tempGame.friend.name);
					
					
					//Debug.Log ("ListfriendId: " + Flow.gameList[h].friend.id);
					//Debug.Log ("gameFriendId: " + data["games"][i]["friendID"]);
					//Debug.Log ("foundGame: " + foundGame);
					//Debug.Log ("gameLastUpdate: " + data["games"][i]["lastUpdate"]);
					//Debug.Log ("lisLastUpdate: " + Flow.gameList[h].lastUpdate);
					
					if(data["games"][i]["friendID"].Int32Value == int.Parse(Flow.gameList[h].friend.id))
					{
						Debug.Log ("foundGame");
						if(data["games"][i]["lastUpdate"].DateTimeValue > Flow.gameList[h].lastUpdate)
						{
							//eh o mesmo e tah mais atualizado
							Debug.Log ("atualizacao chegou!");
							
							if(data["games"][i]["whoseMove"].StringValue != Flow.gameList[h].whoseMove && Flow.gameList[h].whoseMove == "their")
							{
								Debug.Log ("mudou de their para your! :)");
								Flow.yourTurnGames++;
								Flow.theirTurnGames--;
							}
							
							tempGame.wasUpdated = true;
							//tempGame.friend.picture = Flow.gameList[h].friend.picture;
							tempGame.friend.rawText = Flow.gameList[h].friend.rawText;
							tempGame.friend.got_picture = Flow.gameList[h].friend.got_picture;
							tempGame.pastIndex = Flow.gameList[h].pastIndex;
							Flow.gameList[h] = tempGame;
						}
					 	foundGame = true;
						break;
					}
				}
				
				if(!foundGame)
				{
					tempGame.isNewGame = true;
					
					if(data["games"][i]["whoseMove"].StringValue == "your") Flow.yourTurnGames++;
					if(data["games"][i]["whoseMove"].StringValue == "their") Flow.theirTurnGames++;
					
					Flow.gameList.Add(tempGame);
				}
			}
			
			if(data["games"].Count > 0)
			{
				/*foreach(Game g in Flow.gameList) 
				{
					Debug.Log("signs nome da pessoa " + g.friend.name+" signs whose move da pessoa: "+ g.whoseMove+" signs atualizacao da pessoa" + g.lastUpdate);
				}*/
			
				if(oldYourTurnNumber > 0 && Flow.yourTurnGames == 0)
				{
					Flow.gameList.RemoveAt(0);
					Debug.Log("removi label yourturn na gamelist");
				}
				
				if(oldTheirTurnNumber > 0 && Flow.theirTurnGames == 0)
				{
					if(oldYourTurnNumber > 0) Flow.gameList.RemoveAt(oldYourTurnNumber+1);
					else Flow.gameList.RemoveAt(0);
					Debug.Log("removi label theirturn na gamelist");
				}
				
				if(oldYourTurnNumber == 0  && Flow.yourTurnGames > 0)
				{	
					Game g = new Game();
					g.id = -999;
					g.whoseMove = "your";
					g.lastUpdate = new DateTime(2999,12,31);
					g.friend = new Friend();
					g.friend.id = "-999";
					Debug.Log("adicionei label yourturn na gamelist");
					Flow.gameList.Add(g);
				}
				
				if(oldTheirTurnNumber == 0 && Flow.theirTurnGames > 0)
				{
					Game g = new Game();
					g.id = -999;
					g.whoseMove = "their";
					g.lastUpdate = new DateTime(2999,12,31);
					g.friend = new Friend();
					g.friend.id = "-999";
					Debug.Log("adicionei label theirturn na gamelist");
					Flow.gameList.Add(g);
				}
			
			
				//Debug.Log("yt number: "+Flow.yourTurnGames);
				//Debug.Log("tt number: "+Flow.theirTurnGames);
			
				/*foreach(Game g in Flow.gameList) 
				{
					Debug.Log("before nome da pessoa " + g.friend.name+" before whose move da pessoa: "+ g.whoseMove+" before atualizacao da pessoa" + g.lastUpdate);
				}*/
				
				sortList();
			
				// verifica se chegou um jogo novo e atualiza o pastIndex dos jogos depois do jogo novo
				for (int z = 0 ; z < Flow.gameList.Count ; z++)
				{
					if(Flow.gameList[z].isNewGame)
					{
						//Debug.Log("newGame atualizando os index dos jogos abaixo de "+Flow.gameList[z].friend.name);
						// achou um jogo novo, todos os indices passados de jogos abaixo do jogo novo tem que somar 1
						for (int q = z+1 ; q < Flow.gameList.Count ; q++)
						{
							Flow.gameList[q].pastIndex++;
						}
					}
					
					if(Flow.gameList[z].wasUpdated)
					{
						//Debug.Log("wasUpdated atualizando os index dos jogos abaixo de "+Flow.gameList[z].friend.name);
						// achou um jogo novo, todos os indices passados de jogos abaixo do jogo novo tem que somar 1
						for (int q = z+1 ; q < Flow.gameList.Count ; q++)
						{
							Flow.gameList[q].pastIndex++;
						}
					}
					
					
				}
			
				/*foreach(Game g in Flow.gameList) 
				{
					Debug.Log("nome da pessoa " + g.friend.name+" whose move da pessoa: "+ g.whoseMove+" atualizacao da pessoa" + g.lastUpdate);
				}*/
			
				for(int j = 0; j < Flow.gameList.Count; j++)
				{
					if(Flow.gameList[j].isNewGame)
					{
						Debug.Log("nomes new game: "+Flow.gameList[j].friend.name);
						
						CreateGameContainer(Flow.gameList[j], j);
						//quando o jogador já respondeu a um jogo e está desafiando o amigo nesse mesmo jogo, criando um novo turno,
						//é necessário colocar esse antigo jogo na lista de jogos do Flow (por algum motivo, ele some de lá)
					}
					else if (Flow.gameList[j].wasUpdated)
					{
						Debug.Log ("pastIndex: " + Flow.gameList[j].pastIndex);
						Debug.Log ("gameListIndex: " + j);
						Debug.Log ("game friend: " + Flow.gameList[j].friend.name);
						
						GameObject tempContainer;
						
						// seta jogo novo no container com o pastIndex = -1, devemos atualizar depois
						scroll.GetItem(Flow.gameList[j].pastIndex).transform.GetComponent<Game>().SetGame(Flow.gameList[j]);
						tempContainer = GameObject.Instantiate(scroll.GetItem(Flow.gameList[j].pastIndex).gameObject) as GameObject;
						tempContainer.transform.GetComponent<Game>().SetGame(Flow.gameList[j]);
						
						scroll.RemoveItem(Flow.gameList[j].pastIndex, true);
						scroll.InsertItem(tempContainer.GetComponent<UIListItemContainer>(), j);
						
						scroll.GetItem(j).transform.GetComponent<Game>().pastIndex = j;
						
						foreach (Round r in scroll.GetItem(j).transform.GetComponent<Game>().pastTheirRoundList)
						{
							Debug.Log("set update round: "+ r.score);
						}
					}
				}
			
				if(oldYourTurnNumber > 0 && Flow.yourTurnGames == 0)
				{
					Debug.Log("scroll remove your turn");
					scroll.RemoveItem(0,true);
				}
				
				if(oldTheirTurnNumber > 0 && Flow.theirTurnGames == 0)
				{
					Debug.Log("scroll remove their turn");
					if(Flow.yourTurnGames > 0) scroll.RemoveItem(Flow.yourTurnGames+1,true);
					else scroll.RemoveItem(0, true);
				}
				
				if(oldYourTurnNumber == 0  && Flow.yourTurnGames > 0)
				{
					Debug.Log("scroll add your turn");
					
					if(oldTheirTurnNumber > 0 && Flow.theirTurnGames > 0)
					{
						scroll.RemoveItem(0,true);
					}
					
					AddTurnLabel("your",0);
					if(oldTheirTurnNumber > 0 && Flow.theirTurnGames > 0)
					{
						AddTurnLabel("their", Flow.yourTurnGames + 1);
					}
				}
				
				if(oldTheirTurnNumber == 0 && Flow.theirTurnGames > 0)
				{
					Debug.Log("scroll add their turn");
					if(Flow.yourTurnGames > 0) AddTurnLabel("their",Flow.yourTurnGames+1);
					else AddTurnLabel("their",0);
				}
			
				SetListPastIndex();
				
				if (Flow.gameList.Count > 0)
				{
					noGamesYet.Text = "";
				}
				else
				{
					noGamesYet.Text = "No Games Yet";
				}
			
			}
			
			// Recalcula o tempo de espera somente se a conexao deu certo, caso contrario, mais abaixo chamara a conexao com o mesmo tempo de espera
			if(updatedAutomatically)
			{
				updatedAutomatically = false;
				if (data["games"].Count > 0)
				{
					//Debug.Log("chegou jogo novo");
					if (waitingTime > MIN_WAITING_TIME) waitingTime /= RATIO_WAITING_TIME;
				}
				else 
				{
					if (waitingTime < MAX_WAITING_TIME) waitingTime *= RATIO_WAITING_TIME;
				}
			}
		}
		
		Invoke("updatingAutomatically", waitingTime);
	}
	
	void sortList()
	{
		// sort eh uma funcao de lista da unity
		Flow.gameList.Sort
		(
		    // delegate retorna -1, 0 ou 1 que neste caso foi atribuido a variavel "resultado"
			// whoseMove pode ser "yourTurn" ou "theirThurn"
			// se forem diferentes, o sort ja vai alocar na lista "yourTurn" em cima e "theirTurn" embaixo
			// se forem iguais, o resultado sera 0. Neste caso, uma nova comparacao eh feita para o sort colocar em cima de acordo com o lastUpdate
			delegate(Game p1, Game p2)
		    {
		        int resultado = -p1.whoseMove.CompareTo(p2.whoseMove);
				if(resultado == 0)
				{
					resultado = -p1.lastUpdate.CompareTo(p2.lastUpdate);
				}
		        return resultado;
		    }
		);
	}
	
	void FindFriends()
	{
		Flow.path = TurnStatus.BeginGame;
		
		if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()))
		{
			panelManager.BringIn("InviteScenePanel");
		}
		else
		{
			fb_account = new GameFacebook(HandleLinkFacebook);
			Flow.game_native.startLoading();
			fb_account.link();
		}
	}
	
	// Obtem o resultado da vinculacao com o Facebook
	private void HandleLinkFacebook(string error, IJSonObject data)
	{
		Debug.Log(data);
		
		Flow.game_native.stopLoading();

		if (error != null)
		{
			Flow.game_native.showMessage("Error", error);
			return;
		}
		
		panelManager.BringIn("InviteScenePanel");
	}
	
	void AddTurnLabel(string status, int index)
	{
		if(status == "your")
		{
			GameObject tempLabelContainer = GameObject.Instantiate(yourTurnPrefab) as GameObject;
			scroll.InsertItem(tempLabelContainer.GetComponent<UIListItemContainer>(),index);
		}
		else
		{
			GameObject tempLabelContainer = GameObject.Instantiate(theirTurnPrefab) as GameObject;
			scroll.InsertItem(tempLabelContainer.GetComponent<UIListItemContainer>(),index);
		}
	}
	
	GameObject CreateGameContainer (Game game, int index)
	{
		GameObject tempGameContainer = GameObject.Instantiate(gamePrefab) as GameObject;
		
		//Debug.Log ("game: " + game);
		tempGameContainer.GetComponent<Friend>().SetFriend
		(
			game.friend.id,
			game.friend.facebook_id,
			game.friend.name,
			FriendshipStatus.NONE,
			game.friend.is_playing,
			//game.friend.picture.material.mainTexture,
			loadingDialog,
			messageOkDialog,
			messageOkCancelDialog
		);
		if(game.friend.rawText != null)
		{
			//Debug.Log("atribuiu a foto");
			//tempGameContainer.GetComponent<Friend>().picture = game.friend.picture;
			tempGameContainer.GetComponent<Friend>().picture.material.mainTexture = game.friend.rawText;
			tempGameContainer.GetComponent<Friend>().got_picture = game.friend.got_picture;
		}
		game.friend = tempGameContainer.GetComponent<Friend>();
		tempGameContainer.transform.FindChild("Name").GetComponent<SpriteText>().Text = game.friend.name;
		
		scroll.InsertItem(tempGameContainer.GetComponent<UIListItemContainer>(), index);
		tempGameContainer.GetComponent<Game>().SetGame(game);
		//tempGameContainer.GetComponent<Game>().pastIndex = index;
		//Flow.gameList[index].pastIndex = index;
		return tempGameContainer;
	}
	
	GameObject CreateGameContainer (IJSonObject game)
	{
		string[] scores = new string[Flow.ROUNDS_PER_TURN];
		string[] times = new string[Flow.ROUNDS_PER_TURN];
		string[] pastMyScores = new string[Flow.ROUNDS_PER_TURN];
		string[] pastMyTimes = new string[Flow.ROUNDS_PER_TURN];
		string[] pastTheirScores = new string[Flow.ROUNDS_PER_TURN];
		string[] pastTheirTimes = new string[Flow.ROUNDS_PER_TURN];
		
		string faceID = "";
		int lastTurnID = -1;
		int tempWorldID = -1;
		string tempWorldName = "";
		string tempPastWorldName = "";
		
		List<Round> tempRoundList = new List<Round>();
		List<Round> tempPastMyRoundList = new List<Round>();
		List<Round> tempPastTheirRoundList = new List<Round>();
		
		string[] separator = {"|$@@$|"};
		
		if(game["turnStatus"].StringValue != "waitingChoice" && game["whoseMove"].StringValue != "their")
		{
			Debug.Log("adicionei rounds atuais");
			if(!game["scores"].IsNull) scores = game["scores"].StringValue.Split(separator,StringSplitOptions.None);
			if(!game["times"].IsNull) times = game["times"].StringValue.Split(separator,StringSplitOptions.None);
			
			for(int j = 0 ; j < Flow.ROUNDS_PER_TURN ; j++)
			{
				tempRoundList.Add (new Round (-1, game["turn"].Int32Value, game["friendID"].Int32Value, times[j].ToFloat(),
					scores[j].ToInt32()));
			}
		}
		if(!game["lastTurn"].IsNull) 
		{
			Debug.Log("adicionei os rounds passados");
			pastMyScores = game["myPastScores"].StringValue.Split(separator,StringSplitOptions.None);
			pastMyTimes = game["myPastTimes"].StringValue.Split(separator,StringSplitOptions.None);
			pastTheirScores = game["theirPastScores"].StringValue.Split(separator,StringSplitOptions.None);
			pastTheirTimes = game["theirPastTimes"].StringValue.Split(separator,StringSplitOptions.None);
			
			for(int k = 0 ; k < pastMyScores.Length ; k++)
			{
				tempPastMyRoundList.Add(new Round(-1,-1,-1,pastMyTimes[k].ToFloat(),int.Parse(pastMyScores[k])));
				tempPastTheirRoundList.Add(new Round(-1,-1,-1,pastTheirTimes[k].ToFloat(),int.Parse(pastTheirScores[k])));
			}
		}
			
		if(!game["facebookID"].IsNull) faceID = game["facebookID"].StringValue;
		if(!game["lastTurn"].IsNull) lastTurnID = game["lastTurn"].Int32Value;
		if(!game["world"].IsNull) tempWorldID = game["world"].Int32Value;
		if(!game["worldName"].IsNull) tempWorldName = game["worldName"].StringValue;
		if(!game["pastWorldName"].IsNull) tempPastWorldName = game["pastWorldName"].StringValue;
		
		GameObject tempGameContainer = GameObject.Instantiate(gamePrefab) as GameObject;
		//Debug.Log ("game: " + game);
		tempGameContainer.GetComponent<Friend>().SetFriend
		(
			game["friendID"].StringValue,
			faceID,
			game["username"].StringValue,
			FriendshipStatus.NONE,
			game["hasApp"].StringValue.ToBool(),
			//null,
			loadingDialog,
			messageOkDialog,
			messageOkCancelDialog
		);
		
		tempGameContainer.transform.FindChild("Name").GetComponent<SpriteText>().Text = game["username"].StringValue;
		
		//scroll.InsertItem(tempGameContainer.GetComponent<UIListItemContainer>(), index);
		tempGameContainer.GetComponent<Game>().SetGame(game["gameID"].Int32Value,tempGameContainer.GetComponent<Friend>(),game["world"].Int32Value,
			game["worldName"].StringValue,new List<Round>(),tempRoundList,game["turn"].Int32Value,lastTurnID,game["turnsWon"].Int32Value,
			game["turnsLost"].Int32Value,game["whoseMove"].StringValue,game["turnStatus"].StringValue,game["lastUpdate"].DateTimeValue,
			tempPastMyRoundList,tempPastTheirRoundList,tempPastWorldName);
					
		return tempGameContainer;
	}
				
	void SetListPastIndex()
	{
		//for (int i = 0 ; i < Flow.gameList.Count ; i++) Debug.Log("setscroll " + Flow.gameList[i].friend.name+" setscroll: "+ Flow.gameList[i].whoseMove+" setscroll" + Flow.gameList[i].lastUpdate);
		
		for (int i = 0 ; i < Flow.gameList.Count ; i++)
		{
			
			//Debug.Log("rodando index... "+i);
			Flow.gameList[i].pastIndex = i;
			Flow.gameList[i].isNewGame = false;
			Flow.gameList[i].wasUpdated = false;
			
			if(Flow.gameList[i].id != -999) scroll.GetItem(i).transform.GetComponent<Game>().pastIndex = i;
		}
	}
	
	void CreateGame()
	{
		Flow.path = TurnStatus.BeginGame;
		Flow.currentGame = new Game();
	}
}