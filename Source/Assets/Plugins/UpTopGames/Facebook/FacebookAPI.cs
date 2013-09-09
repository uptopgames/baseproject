using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Instancia a classe FacebookAPI
public class FacebookAPI {
	
	private FacebookAPIManager manager;

    // String padrão referente ao proprio usuario (padrão do Facebook)
	private const string USER_ME = "me",

    // Mensagem de erro padrão, caso nao existir nenhum Token setado na FacebookAPI
    EMPTY_TOKEN = "Facebook Token is empty or null! Use facebook.SetToken(string token).";
	
    // Tipos de fotos (padrão do Facebook)
	public enum Picture {
        Small, Large, Medium, Square
    };
	
    // Classe de cache por cada usuario
	public class User
    {
		public class Picture {
            public Texture small, large, medium, square;
        }
		public enum Gender {
            Male, Female, None
        };
		public string facebookId, firstName, lastName, userName;
		public Picture picture;
	}
	
    // Constructor da FacebookAPI
    public FacebookAPI()
	{
		manager = GetFB();
		manager.facebookId = Info.facebookId;
		manager.facebookSecret = Info.facebookSecret;
		if (manager.users == null)
			manager.users = new Dictionary<string, User>();
		HasToken();
	}
	
    // Retorna a prefab da Instancia do FacebookAPI
	private FacebookAPIManager GetFB()
	{
		if (manager != null)
            return manager;

		GameObject _fbAPI = Flow.config;

		FacebookAPIManager _manager = _fbAPI.GetComponent<FacebookAPIManager>();
		if (_manager == null) _manager = _fbAPI.AddComponent<FacebookAPIManager>();
		return _manager;		
	}
	
    // Checa se a prefab ja foi instanciada por inteiro
	bool FacebookAPIReady() {
		if (manager == null) manager = GetFB();
		return (manager.facebookId != null && manager.facebookId != "" &&
			manager.facebookSecret != null && manager.facebookSecret != "") ?
            true : false;
	}
	
    // Checa se existe um token setado na FacebookAPI
	bool HasToken() {
		if (manager == null)
            manager = GetFB();
		if (manager.facebookToken.IsEmpty())
        {
			if (!Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()).IsEmpty())
			{
                SetToken(Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()));
                return true;
            }
			return false;
		}
        return true;
	}
	
    // Deleta o cache dos usuarios (Informacoes e Fotos)
	public void CleanCache()
	{
        GetFB().users.Clear();
    }
	
    // Seta token na FacebookAPI
	public void SetToken(string token)
	{
        GetFB().facebookToken = token;
    }
	
    // Atualiza o token direto do Save (setado na scene Login/Settings)
	public void UpdateToken()
	{
		if (!Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()).IsEmpty())
        	GetFB().facebookToken = Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString());
    }
	
    // Remove token atual
	public void DeleteToken()
	{
        GetFB().facebookToken = null;
    }

    // Funcao para pegar informacoes do usuario
    // Retorna FacebookId, Nome, Sobrenome, e Login
    //
    // Ex:
    //     GetInfo("kazzxd1", OnReceiveInfo);
    //     GetInfo("me", OnReceiveInfo);
    //
    //     void OnReceiveInfo(string error, string facebookId, string firstName, string lastName, string userName)
    //     {
    //     }

    // Delegate (callback)
    public delegate void GetInfoCallback(string error, string facebookId, string firstName, string lastName, string userName);

    // Funcao
	public void GetInfo(string facebookId, GetInfoCallback callback)
    {
		if (FacebookAPIReady())
        {
            // Se o usuario solicitado for o proprio e nao existir token
            // fazer o "giro" do Facebook
			if (facebookId == USER_ME && !HasToken())
            {
                // Cacheia conexão e tenta novamente apos o "giro"
				GetFB().Login(
                    new FacebookAPIManager.GenerateState(facebookId, callback),
                    GetFB().HandleState
                );

                // Enviar erro para o callback
				Debug.LogWarning(EMPTY_TOKEN);
				if (callback != null)
					callback(EMPTY_TOKEN, facebookId, null, null, null);
				return;
            }

            // Caso contrario, fazer a conexão
			GetFB().StartCoroutine(
				GetFB().GetInfo(facebookId, callback)
			);
		}
	}

	public void GetInfo(GetInfoCallback callback)
	{
        GetInfo(USER_ME, callback);
    }

    // Funcao para pegar Foto do perfil usuario
    // Retorna Foto nos tamanhos Square, Small, Medium e Large
    //
    // Ex:
    //     // Retorna tamanho "square" (padrão) do proprio usuario
    //     GetPicture(OnReceivePicture);
    //
    //     // Retorna tamanho "small"  do proprio usuario
    //     GetPicture(Picture.Small, OnReceivePicture);
    //
    //     // Retorna tamanho "Large" do algum usuario
    //     GetPicture("kazzxd", Picture.Large, OnReceivePicture); // Retorna tamanho "large"
    //
    //     void OnReceivePicture(string error, string facebookId, Texture picture)
    //     {
    //     }

    // Delegate (callback)
	public delegate void GetPictureCallback(string error, string facebookId, Texture picture);

    // Funcao
	public void GetPicture(string facebookId, Picture type, GetPictureCallback callback)
	{
		if (FacebookAPIReady())
        {
            // Se o usuario solicitado for o proprio e nao existir token
            // fazer o "giro" do Facebook
			if (facebookId == USER_ME && !HasToken())
            {
                // Cacheia conexão e tenta novamente apos o "giro"
				GetFB().Login(
                    new FacebookAPIManager.GenerateState(facebookId, type, callback),
                    GetFB().HandleState
                );

                // Enviar erro para o callback
				Debug.LogWarning(EMPTY_TOKEN);
				if (callback != null)
					callback(EMPTY_TOKEN, facebookId, null);
				return;
            }

            // Caso contrario, fazer a conexão
			GetFB().StartCoroutine(
				GetFB().GetPicture(facebookId, type, callback)
			);
		}
	}

	public void GetPicture(string facebookId, GetPictureCallback callback)
	{
        GetPicture(facebookId, Picture.Square, callback);
    }

	public void GetPicture(Picture type, GetPictureCallback callback)
	{
        GetPicture(USER_ME, type, callback);
    }

	public void GetPicture(GetPictureCallback callback)
	{
        GetPicture(USER_ME, Picture.Square, callback);
    }

    // Funcao para postar no proprio mural ou no mural do amigo, com ou sem dialogo
    // Retorna ID do post (no caso de não ser dialogo)
    //
    // Ex:
    //     // Faz um post no proprio mural do usuario (sem ser dialogo)
    //     WallPost("me", Dictionary<string, string>()
    //         {
    //             {"message", "Hello there! Lets google?!"},
    //             {"link", "google.com"},
    //             {"description, "Click here for enter google!"}
    //         }
    //         OnWallPost
    //     );
    //
    //     // Faz um post no mural de um amigo (COM dialogo)
    //     WallPost("kazzxd1", Dictionary<string, string>()
    //         {
    //             {"message", "Hello there! Lets google?!"},
    //             {"link", "google.com"},
    //             {"description, "Click here for enter google!"},
    //             {"dialog", "true"},
    //         }
    //         OnWallPost
    //     );
    //     
    //     void OnWallPost(string error, string facebookId, Dictionary<string, string> parameters, string postId)
    //     {
    //     }

    // Delegate (callback)
	public delegate void WallPostCallback(string error, string facebookId, Dictionary<string, string> parameters, string postId);

    // Funcao
	public void WallPost(string facebookId, Dictionary<string, string> parameters, WallPostCallback callback)
	{
		if (FacebookAPIReady())
        {
			// Se for Web, abrir funcao nativa do JavaScript no browser, o callback do post na web devera ser definido na sua classe que chama o post, com
			// o nome de "SetShareWeb(string response)", no parametro response, você recebera se a pessoa fez o post (om o id dele) ou se a pessoa cancelou
			if (Info.IsWeb())
			{
				if(!parameters.ContainsKey("link")) parameters.Add("link","");
				if(!parameters.ContainsKey("name")) parameters.Add("name","");
				if(!parameters.ContainsKey("description")) parameters.Add("description","");
				if(!parameters.ContainsKey("picture")) parameters.Add("picture","");

				Application.ExternalCall("postUserWall", facebookId, parameters["link"], parameters["name"], parameters["description"], parameters["picture"]);
				
				return;
			}
			
            // Se o usuario solicitado for o proprio e nao existir token
            // fazer o "giro" do Facebook
			if (!HasToken())
            {
                // Cacheia conexão e tenta novamente apos o "giro"
				GetFB().Login(
                    new FacebookAPIManager.GenerateState(facebookId, parameters, callback),
                    GetFB().HandleState
                );

                // Enviar erro para o callback
				Debug.LogWarning(EMPTY_TOKEN);
				if (callback != null)
					callback(EMPTY_TOKEN, facebookId, parameters, null);
				return;
            }

            // Caso contrario, fazer a conexão
			GetFB().StartCoroutine(
				GetFB().WallPost(facebookId, parameters, callback)
			);
		}
	}

    // Funcao para setar Score ao usuario
    // Retorna bool informando se setou ou nao o score
    //
    // Ex:
    //     // Seta score ao proprio usuario
    //     SetScore(666, OnSetScore);
    //
    //     // Seta score em outro usuario
    //     SetToken("fb_token_from_kazzxd1");
    //     SetScore("kazzxd1", 666, OnSetScore);
    //
    //     void OnSetScore(string error, string facebookId, int score)
    //     {
    //     }

    // Delegate (callback)
	public delegate void SetScoreCallback(string error, string facebookId, int score);

    // Funcao
	public void SetScore(string facebookId, int score, SetScoreCallback callback)
	{
		if (FacebookAPIReady())
        {
            // Se nao existir token, fazer o "giro" do Facebook
			if (!HasToken())
            {
                // Cacheia conexão e tenta novamente apos o "giro"
				GetFB().Login(
                    new FacebookAPIManager.GenerateState(facebookId, score, callback),
                    GetFB().HandleState
                );

                // Enviar erro para o callback
				Debug.LogWarning(EMPTY_TOKEN);
				if (callback != null)
					callback(EMPTY_TOKEN, facebookId, score);
				return;
            }

            // Caso contrario, fazer a conexão
			GetFB().StartCoroutine(
				GetFB().SetScore(facebookId, score, callback)
			);
		}
	}

	public void SetScore(string facebookId, float score, SetScoreCallback callback)
	{
        SetScore(facebookId, (int)score, callback);
    }

    // Funcao para checar se o usuario curtiu alguma pagina
    // Retorna bool informando se curtiu ou nao
    //
    // Ex:
    //     // Checa like do proprio usuario
    //     SetScore("pageId", OnCheckLike);
    //
    //    // Checa like de outro usuario
    //     SetToken("fb_token_from_kazzxd1");
    //     SetScore("kazzxd1", "pageId", OnCheckLike);
    //
    //     void OnCheckLike(string error, bool liked, string pageId)
    //     {
    //     }

    // Delegate (callback)
	public delegate void IsLikedPageCallback(string error, bool liked, string pageId);

    // Funcao
	public void IsLikedPage(string pageId, IsLikedPageCallback callback)
	{
		if (FacebookAPIReady())
        {
            // Se nao existir token, fazer o "giro" do Facebook
			if (!HasToken())
            {
                // Cacheia conexão e tenta novamente apos o "giro"
				GetFB().Login(
                    new FacebookAPIManager.GenerateState(pageId, callback),
                    GetFB().HandleState
                );

                // Enviar erro para o callback
				Debug.LogWarning(EMPTY_TOKEN);
				if (callback != null)
					callback(EMPTY_TOKEN, false, pageId);
				return;
            }

            // Caso contrario, fazer a conexão
			GetFB().StartCoroutine(
				GetFB().IsLikedPage(pageId, callback)
			);
		}
	}
	
	public delegate void InviteCallback(string error);
	
	public void OpenInvite(InviteCallback callback)
	{
		if (FacebookAPIReady())
        {
			// Se for Web, abrir funcao nativa do JavaScript no browser
			if (Info.IsWeb())
			{
				Application.ExternalEval("inviteFriends()");
				return;
			}

            // Se nao existir token, fazer o "giro" do Facebook
			if (!HasToken())
            {
                // Cacheia conexão e tenta novamente apos o "giro"
				GetFB().Login(
                    new FacebookAPIManager.GenerateState(callback),
                    GetFB().HandleState
                );

                // Enviar erro para o callback
				Debug.LogWarning(EMPTY_TOKEN);
				if (callback != null)
					callback(EMPTY_TOKEN);
				return;
            }

            // Caso contrario, fazer a conexão
			GetFB().StartCoroutine(
				GetFB().OpenInvite(callback)
			);
		}
	}
}
