using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

// Instancia (manager) da classe FacebookAPI
public class FacebookAPIManager : MonoBehaviour {
	
    // URL para facil acesso ao graph e dialogo do Facebook
	public const string GRAPH_URL = "https://graph.facebook.com/", DIALOG_URL = "https://m.facebook.com/dialog/",
    
    // Mensagem de erro defaul ao enviar um dicionario vazio nos metodos de post
	NULL_PARAMETERS = "Parameters dictionary cannot be null!";
	
	public Dictionary<string, FacebookAPI.User> users;
	public string facebookId, facebookSecret, facebookToken;
	public bool isChaching = true;
	
	public string tempFace = "";
	public FacebookAPI.WallPostCallback tempCallback = null;
	public Dictionary<string,string> tempParams = new Dictionary<string, string>();
	
    // Inicia o "giro" do Facebook nativo e envia callback pra refazer a conexão
	public void Login(object state, GameFacebook.LoginCallback callback)
	{
        new GameFacebook().login(state, callback);
    }
	
    // Cacheia uma conexão, para poder ser executada novamente, apos fazer o "giro" do Facebook ou perder o token/conexão
	public class GenerateState
    {
        // Numero de tentativas de conexão (limite = 3)
        public int tries = 0;

        // Tipo de funcao cacheada
		public enum StateType {
            GetInfo, GetPicture, IsLikedPage, SetScore, WallPost, OpenInvite
        };
        public StateType stateType;
		
        // Callbacks para cada tipo de funcao
		public FacebookAPI.GetInfoCallback getInfoCallback;
		public FacebookAPI.GetPictureCallback getPictureCallback;
		public FacebookAPI.IsLikedPageCallback isLikedPageCallback;
		public FacebookAPI.SetScoreCallback setScoreCallback;
		public FacebookAPI.WallPostCallback wallPostCallback;
		public FacebookAPI.InviteCallback inviteCallback;

        public int setScore;
		public string globalFacebookId, pageId;

        // Tipo de foto para o metodo de GetPicture (square, small, etc)
		public FacebookAPI.Picture getPictureType;
		public Dictionary<string, string> wallParameters;
		
        // Constructor para cada tipo de funcao (GetInfo, GetPicture, WallPost, etc..)
		public GenerateState(string facebookId, FacebookAPI.GetInfoCallback callback, int tries = 0)
		{ stateType = StateType.GetInfo; globalFacebookId = facebookId; getInfoCallback = callback; this.tries = tries; }
		public GenerateState(string facebookId, FacebookAPI.Picture type, FacebookAPI.GetPictureCallback callback, int tries = 0)
		{ stateType = StateType.GetPicture; globalFacebookId = facebookId; getPictureType = type; getPictureCallback = callback;  this.tries = tries; }
		public GenerateState(string facebookId, Dictionary<string, string> parameters, FacebookAPI.WallPostCallback callback, int tries = 0)
		{ stateType = StateType.WallPost; globalFacebookId = facebookId; wallParameters = parameters; wallPostCallback = callback;  this.tries = tries; }
		public GenerateState(string facebookId, int score, FacebookAPI.SetScoreCallback callback, int tries = 0)
		{ stateType = StateType.SetScore; globalFacebookId = facebookId; setScore = score; setScoreCallback = callback;  this.tries = tries; }
		public GenerateState(string _pageId, FacebookAPI.IsLikedPageCallback callback, int tries = 0)
		{ stateType = StateType.IsLikedPage; pageId = _pageId; isLikedPageCallback = callback;  this.tries = tries; }
		public GenerateState(FacebookAPI.InviteCallback callback, int tries = 0)
		{ stateType = StateType.OpenInvite; inviteCallback = callback;  this.tries = tries; }
	}
	
    // Recebe o callback do "giro" do Facebook e refaz a conexão (limite de tentativas = 3)
	public void HandleState(object data)
	{
		GenerateState state = (GenerateState)data;
		state.tries++; if (state.tries >= 3) return;
		if (state.stateType == GenerateState.StateType.WallPost)
			StartCoroutine(WallPost(state.globalFacebookId, state.wallParameters, state.wallPostCallback, state.tries));
		else if (state.stateType == GenerateState.StateType.GetInfo)
			StartCoroutine(GetInfo(state.globalFacebookId, state.getInfoCallback, state.tries));
		else if (state.stateType == GenerateState.StateType.GetPicture)
			StartCoroutine(GetPicture(state.globalFacebookId, state.getPictureType, state.getPictureCallback, state.tries));
		else if (state.stateType == GenerateState.StateType.IsLikedPage)
			StartCoroutine(IsLikedPage(state.pageId, state.isLikedPageCallback, state.tries));
		else if (state.stateType == GenerateState.StateType.SetScore)
			StartCoroutine(SetScore(state.globalFacebookId, state.setScore, state.setScoreCallback, state.tries));
		else if (state.stateType == GenerateState.StateType.OpenInvite)
			StartCoroutine(OpenInvite(state.inviteCallback, state.tries));
	}
	
	// Funcao para pegar informacoes do usuario
    // Retorna FacebookId, Nome, Sobrenome, e Login
	public IEnumerator GetInfo(string _facebookId, FacebookAPI.GetInfoCallback callback, int tries = 0)
	{
        // Se ja estiver cacheada as informacoes, somente enviar para o callback
		if (users.ContainsKey(_facebookId) && users[_facebookId].firstName != null)
        {
			if (callback != null)
				callback(null, _facebookId, users[_facebookId].firstName, users[_facebookId].lastName, users[_facebookId].userName);
			yield break;
		}
        // Caso contrario, fazer uma conexão ao facebook para pegar as informacoes
        else
        {
            // Cria URL do request
			string url = GRAPH_URL + _facebookId + "/?";
			if (_facebookId == "me")
				url += "access_token=" + WWW.EscapeURL(facebookToken) + "&";
			url += "fields=username,first_name,last_name";
			WWW getInfo = new WWW(url);
			yield return getInfo;

            // Se houver algum erro, enviar erro para o callback
			if (getInfo.error != null)
            {
				if (callback != null) 
					callback(getInfo.error, _facebookId, null, null, null);
			}

            // Caso contrario, decodar JSON recebido
            else
            {
				IJSonObject data = getInfo.text.ToJSon();
				
				// Se o JSON recebido for invalido, retornar e enviar para o callback
				if (data.IsEmpty() || data.IsError())
				{
					if (callback != null) 
						callback("Invalid JSon: " + getInfo.text, _facebookId, null, null, null);
					
					yield break;
				}
				
                // Cacheia as informacoes recebidas
				FacebookAPI.User user = (users.ContainsKey(_facebookId)) ?
                    users[_facebookId] : new FacebookAPI.User();
				user.facebookId = _facebookId;
				user.firstName = data.GetString("first_name");
				user.lastName = data.GetString("last_name");
				user.userName = data.GetString("username");

                // Envia pro callback
				if (callback != null) 
					callback(null, _facebookId, user.firstName, user.lastName, user.userName);
			}
		}
		yield break;
	}

    // Funcao para pegar a foto do usuario
    // Retorna foto como Texture
	public IEnumerator GetPicture(string _facebookId, FacebookAPI.Picture type, FacebookAPI.GetPictureCallback callback, int tries = 0)
	{
        // Se ja estiver cacheada as informacoes, somente enviar para o callback
		if (users.ContainsKey(_facebookId) && users[_facebookId].picture != null)
        {
            // Cacheia todos os tamanhos de fotos possiveis (square, small, etc) e retorna pro callback somente
            // se tiver cacheado o tamanho de foto correto solicitado
			if (type == FacebookAPI.Picture.Square && users[_facebookId].picture.square != null)
			{
                if (callback != null)
                    callback(null, _facebookId, users[_facebookId].picture.square);
                yield break;
            }
			else if (type == FacebookAPI.Picture.Large && users[_facebookId].picture.large != null)
			{
                if (callback != null)
                    callback(null, _facebookId, users[_facebookId].picture.large);
                yield break;
            }
			else if (type == FacebookAPI.Picture.Small && users[_facebookId].picture.small != null)
			{
                if (callback != null)
                    callback(null, _facebookId, users[_facebookId].picture.small);
                yield break;
            }
			else if (type == FacebookAPI.Picture.Medium && users[_facebookId].picture.medium != null)
			{
                if (callback != null)
                    callback(null, _facebookId, users[_facebookId].picture.medium);
                yield break;
            }
		}

        // Caso contrario, fazer uma conexão ao facebook para pegar as informacoes
        // Cria URL do request
		string url = GRAPH_URL + _facebookId + "/picture/?";
		if (_facebookId == "me")
			url += "access_token=" + WWW.EscapeURL(facebookToken) + "&";
		url += "type=" + type.ToString().ToLower();
		WWW getPicture = new WWW(url);
		yield return getPicture;

        // Se houver algum erro, enviar erro para o callback
		if (getPicture.error != null)
        {
			if (callback != null) 
				callback(getPicture.error, _facebookId, null);
		}

        // Caso contrario, validar foto recebida
        else
        {
            // Checa se a foto não esta no formato de GIF (formato não suportado pela Unity3D)
			if (getPicture.text.StartsWith("GIF"))
            {
				if (callback != null)
					callback("User images are GIF format!", _facebookId, null);
				yield break;
            }

            // Cacheia as informacoes recebidas
			Texture picture = getPicture.texture;
			FacebookAPI.User user = (users.ContainsKey(_facebookId)) ? users[_facebookId] : new FacebookAPI.User();
			if (user.picture == null)
                user.picture = new FacebookAPI.User.Picture();
			if (type == FacebookAPI.Picture.Square)
				user.picture.square = picture;
			else if (type == FacebookAPI.Picture.Large)
				user.picture.large = picture;
			else if (type == FacebookAPI.Picture.Small)
				user.picture.small = picture;
			else user.picture.medium = picture;

            // Envia pro callback
			if (callback != null) 
				callback(null, _facebookId, picture);
		}
		yield break;
	}

    // Funcao para fazer uma postagem no mural do usuario/amigo
    // Retorna ID do post se nao for feito por dialogo
	public IEnumerator WallPost(string _facebookId, Dictionary<string, string> parameters, FacebookAPI.WallPostCallback callback, int tries = 0)
	{
        // Cria URL do request
		bool isDialog = (parameters != null && parameters.ContainsKey("dialog")) ? true : false;
		string url = (isDialog) ? DIALOG_URL : GRAPH_URL;
		if (!isDialog) url += _facebookId + "/";
		url += "feed/?";
		
        // Se for dialogo, adiciona redirecionamento para php aonde fecha o browser nativo do Prime31
        // e informacoes extras para fazer o post
		if (isDialog)
        {
			url += "app_id=" + facebookId + "&";
			url += "redirect_uri=" + WWW.EscapeURL(Flow.URL_BASE + "login/tables/input_share.php?" +
				//"app_id=" + Info.appId + "&token=" + WWW.EscapeURL(Save.GetString(PlayerPrefsKeys.TOKEN.ToString())) +"&device=" + GameInfo.device()) + "&";
				"app_id=" + Info.appId + "&token=" + WWW.EscapeURL(Save.GetString(PlayerPrefsKeys.TOKEN.ToString())) +"&device=" + SystemInfo.deviceUniqueIdentifier.Replace("-", "")) + "&";
			if (_facebookId != "me")
                url += "to=" + WWW.EscapeURL(_facebookId) + "&";
		}
        else url += "method=post&";
		
        // Checa se os parametros enviados nao sao nulos e adiciona no form
		if (parameters != null)
        {
			if (parameters.ContainsKey("message"))
				url += "message=" + WWW.EscapeURL(parameters["message"]) + "&";
			if (parameters.ContainsKey("name"))
				url += "name=" + WWW.EscapeURL(parameters["name"]) + "&";
			if (parameters.ContainsKey("link"))
				url += "link=" + WWW.EscapeURL(parameters["link"]) + "&";
			if (parameters.ContainsKey("description"))
				url += "description=" + WWW.EscapeURL(parameters["description"]) + "&";
			if (parameters.ContainsKey("picture"))
				url += "picture=" + WWW.EscapeURL(parameters["picture"]) + "&";
		}

        // Se for nulo, enviar erro para o callback
        else
        {
			if (callback != null)
				callback(NULL_PARAMETERS, facebookId, parameters, null);
			Debug.LogWarning(NULL_PARAMETERS);
			yield break;
		}

        // Access token do usuario
		url += "access_token=" + WWW.EscapeURL(facebookToken);
		
        // Se nao for dialogo, fazer o post
		if (!isDialog)
        {
			WWW wallPost = new WWW(url);
			yield return wallPost;

            // Se houver algum erro, enviar erro para o callback
			if (wallPost.error != null)
            {
                // Envia para o callback
				if (callback != null)
					callback(wallPost.error, _facebookId, parameters, null);

                // Cacheia a conexão atual para tentar novamente apos o "giro"
				Login(
                    new GenerateState(_facebookId, parameters, callback)
                    , HandleState
                );
				yield break;
			}

            // Caso contrario, decodar JSON recebido
            else
            {
				IJSonObject data = wallPost.text.ToJSon();
				
				// Se o JSON recebido for invalido, retornar e enviar para o callback
				if (data.IsEmpty() || data.IsError())
				{
					if (callback != null) 
						callback("Invalid JSon: " + wallPost.text, _facebookId, parameters, null);
					
					yield break;
				}
				
                // Envia ID do post para o callback
				if (callback != null)
					callback(null, _facebookId, parameters, data.GetString("id"));
			}
		}

        // Se for dialogo, abrir a url no navegador do Prime31
        else
        {
			// Up Top Fix me (dialog)
			//GameGUI.game_native.openUrlInline(url);
			Flow.game_native.openUrlInline(url);
			
			
			tempFace = _facebookId;
			tempParams = parameters;
			tempCallback = callback;
			
			if(_facebookId == "me") 
			{
				new GameJsonAuthConnection(Flow.URL_BASE+"login/tables/check_share.php",GetShareData).connect();
			}
			else
			{
				if (callback != null) callback(null, _facebookId, parameters, null);
			}
            // Como nao tem como receber o callback do navegador do Prime31
            // enviar callback de sucesso (mesmo se o usuario nao estiver postado)
			
		}
		yield break;
	}
	
	int tries = 0;
	
	void GetShareData(string error, IJSonObject data)
	{
		tries++;
		if(error != null) 
		{
			Debug.Log(error);
			tries = 0;
			if (tempCallback != null) tempCallback("Connection Error. Please try again later.", tempFace, tempParams, null);
			tempFace = "";
			tempCallback = null;
			tempParams = new Dictionary<string, string>();
		}
		else
		{
			if(data["result"].ToString() == "shared")
			{
				tries = 0;
				if (tempCallback != null)
					tempCallback(null, tempFace, tempParams, data["post_id"].StringValue);
				
				tempFace = "";
				tempCallback = null;
				tempParams = new Dictionary<string, string>();
			}
			else
			{
				if(tries > 5)
				{
					tries = 0;
					if (tempCallback != null)
					tempCallback("Connection Error. Please try again later.", tempFace, tempParams, null);
					
					tempFace = "";
					tempCallback = null;
					tempParams = new Dictionary<string, string>();
				}
				else 
				{
					Invoke("promptAgain",1f);
				}
			}
				
		}
		
	}
	
	void promptAgain()
	{
		new GameJsonAuthConnection(Flow.URL_BASE+"login/tables/check_share.php",GetShareData).connect();
	}
	
    // Funcao para setar score ao usuario
	public IEnumerator SetScore(string _facebookId, int score, FacebookAPI.SetScoreCallback callback, int tries = 0)
	{
        // Cria URL do request
		string url = GRAPH_URL + _facebookId + "/scores?";
		url += "score=" + score.ToString() + "&";
		url += "access_token=";
		url += ((_facebookId == "me") ? WWW.EscapeURL(facebookToken) :
			WWW.EscapeURL(facebookId + "|" + facebookSecret)) + "&";
		url += "method=post";
		WWW _score = new WWW(url);
		yield return _score;

        // Se houver algum erro, enviar erro para o callback
		if (_score.error != null)
        {
			if (callback != null)
				callback(_score.error, _facebookId, score);
			yield break;
		}

        // Caso contrario, validar conexão
        else
        {
			if (callback != null)
            {
                // Se realmente estiver setado o score, enviar para o callback
				if (_score.text == "true")
					callback(null, _facebookId, score);

                // Caso contrario, enviar erro para o callback
				else callback("Failed to post score.", _facebookId, score);
			}
		}
		yield break;		
	}

    // Funcao para checar se o usuario curtiu alguma pagina
	public IEnumerator IsLikedPage(string pageId, FacebookAPI.IsLikedPageCallback callback, int tries = 0)
	{
        // Cria URL do request
		string url = GRAPH_URL + "me/likes/" + pageId + "/?";
		url += "access_token=" + facebookToken;
		WWW like = new WWW(url);
		yield return like;

        // Se houver algum erro, enviar erro para o callback
		if (like.error != null)
        {
			if (callback != null)
				callback(like.error, false, pageId);
			yield break;
		}

        // Caso contrario, decodar JSON recebido e validar conexão
        else
        {
			IJSonObject data = like.text.ToJSon();
			
			// Se o JSON recebido for invalido, retornar e enviar para o callback
			if (data.IsEmpty() || data.IsError())
			{
				if (callback != null) 
					callback("Invalid JSon: " + like.text, false, pageId);
				
				yield break;
			}
			
            // Valida conexão e envia para o callback
			if (callback != null)
                callback(null,
					(
						data.Contains("data") &&
						data["data"].Count > 0 &&
						data["data"][0].GetString("id") == pageId
					),
				pageId);
		}
		yield break;
	}
	
	public IEnumerator OpenInvite(FacebookAPI.InviteCallback callback, int tries = 0)
	{
			string url = DIALOG_URL + "apprequests?";
			url += "app_id=" + facebookId + "&";
			url += "message=" + WWW.EscapeURL("Invite your friends and got coins!") + "&";
			url += "title=" + WWW.EscapeURL("Sea Combat") + "&";
			url += "redirect_uri=" + WWW.EscapeURL(Flow.URL_BASE +
						"login/facebook/invite.php?" +
							"token=" + PlayerPrefs.GetString("token") + "&" +
							"device=" + SystemInfo.deviceUniqueIdentifier.Replace("-", "") + "&" +
							"close=" + (Info.IsPlatform(Info.Platform.Android) ? "1" : "0")
			);
		
			// Up Top Fix Me (dialog)		
			//GameGUI.game_native.openUrlInline(url);
			Flow.game_native.openUrlInline(url);

            // Como nao tem como receber o callback do navegador do Prime31
            // enviar callback de sucesso (mesmo se o usuario nao estiver postado)
			if (callback != null)
				callback(null);
		
		yield break;
	}
}
