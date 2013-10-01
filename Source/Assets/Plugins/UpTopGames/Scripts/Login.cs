using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class Login : MonoBehaviour 
{
	public UIPanelManager panelManager;
	public GameObject passwordDialog;
	
	public GameObject loadingDialog;
	
	public SpriteText passwordText;
	public UITextField passwordField, emailTextfield;
	public GameObject messageOkDialog;
	
	private string authentication;
	
	// Login e senha
	private string email;
	private string password;
	
	public SpriteText emailText;
	
	// Use this for initialization
	void Start ()
	{
		email = "";
		password = "";
		
		emailTextfield.SetFocusDelegate(ClearText);
	}
	
	void CheckLogin()
	{
		//Debug.Log ("Token: " + Save.GetString(PlayerPrefsKeys.TOKEN.ToString()));
		//Debug.Log ("FacebookToken: " + Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()));
		//Debug.Log("vamos checar o login");
		//if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()) || Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()))
		if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
		{
			panelManager.BringIn("MultiplayerScenePanel");
		}
		else
		{
			panelManager.BringIn("LoginScenePanel");
		}
	}
	
	void StandaloneLogin()
	{
		email = emailText.Text;
		logUserIn();
	}
	
	void FacebookLogin()
	{
#if UNITY_EDITOR || !UNITY_WEBPLAYER
			requestToken();
#elif UNITY_WEBPLAYER
			Application.ExternalEval("refreshCanvas()");
#endif
	}
	
	// Obtem o token com o servidor
	protected void requestToken()
	{
		if (!Info.HasConnection(true)) 
		{
			Debug.Log("sem conexao");
			return;
		}
		
		Flow.game_native.startLoading();
		
		// Criar o codigo de autenticacao
		authentication = System.Guid.NewGuid().ToString();
		
		// Chama a pagina do Facebook no nosso servidor
		string fb_login_url = Flow.URL_BASE + "login/facebook/";
		fb_login_url += "?app_id=" + Info.appId;
		fb_login_url += "&authentication=" + authentication;
		fb_login_url += "&device=" + SystemInfo.deviceUniqueIdentifier.Replace("-", "");
		
		string device_push = PushNotifications.getPushDevice();
		if (device_push != null) fb_login_url += "&device_push=" + WWW.EscapeURL(device_push);
		fb_login_url += "&app_version=" + Info.version.ToString();
		fb_login_url += "&app_type=" + Info.appType.ToString();

#if UNITY_EDITOR
		fb_login_url += "&app_platform=UnityEditor";
#elif UNITY_WEBPLAYER
		fb_login_url += "&app_platform=Facebook";
#elif UNITY_ANDROID
		fb_login_url += "&app_platform=Android";
#elif UNITY_IPHONE
		fb_login_url += "&app_platform=iOS";
#endif

		Flow.game_native.openUrlInline(fb_login_url);
		

		// Obtem a resposta do servidor
		StartCoroutine(getFacebookInfo());
	}
	
	// Obtem as informacoes do Facebook no servidor
	private IEnumerator getFacebookInfo()
	{
		Debug.Log("pegando info face");
		// Numero maximo de tentativas
		int max_attempts = 5;
		
		WWW conn = null;
		
		WWWForm form = new WWWForm();
		form.AddField("device", SystemInfo.deviceUniqueIdentifier.Replace("-", ""));
		form.AddField("authentication", authentication);
		
		while (max_attempts > 0)
		{
			conn = new WWW(Flow.URL_BASE + "login/facebook/fbinfo.php", form);
			yield return conn;
			
			if (conn.error != null || conn.text != "") break;
			
			max_attempts--;
			
			yield return new WaitForSeconds(1);
		}
		
		Flow.game_native.stopLoading();
		
		if (max_attempts == 0 || conn.error != null)
		{
			Debug.LogError("Server error: " + conn.error);
			Flow.game_native.showMessage("Error", GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE);
			
			yield break;
		}
		
		JSonReader reader = new JSonReader();
		IJSonObject data = reader.ReadAsJSonObject(conn.text);
		
		if (data == null || data.Contains("error"))
		{
			Debug.LogError("Json error: " + conn.text);
			Flow.game_native.showMessage("Error", GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE);
			
			yield break;
		}
		
		Debug.Log("data: " + data);
		
		GameToken.save(data);
		Save.Set(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString(), data["fbtoken"].ToString(), true);
		Save.Set(PlayerPrefsKeys.NAME.ToString(), data["username"].ToString(),true);
		Save.Set(PlayerPrefsKeys.ID.ToString(), data["user_id"].ToString(),true);
		
		// Atualiza token da FacebookAPI
		if (Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString())) {
			FacebookAPI facebook = new FacebookAPI();
			facebook.SetToken(Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()));
		}
		
		CheckLogin();
	}
	
	// GLA
	// Loga o usuario no servidor
	private void logUserIn()
	{
		//Debug.Log("email: "+email);
		//Debug.Log("password: "+password);
		if (!Info.HasConnection(true) || email.IsEmpty())
		{
			//screen_status = ScreenStatus.not_logged;
			Debug.Log("error, inform email");
			if (email.IsEmpty()) Flow.game_native.showMessage("Error", "Please inform an e-mail.");
			
			return;
		}
		
		// Up Top Fix Me
		Flow.game_native.startLoading();
		
		//GameConnection conn = new GameConnection(components.url_login, connectionResult);
		GameConnection conn = new GameConnection(Flow.URL_BASE + "login/", connectionResult);
			
		WWWForm form = new WWWForm();
		form.AddField("app_id", Info.appId);
		form.AddField("email", email);
		form.AddField("password", password);
		form.AddField("device", SystemInfo.deviceUniqueIdentifier.Replace("-",""));
		
		string device_push = PushNotifications.getPushDevice();
		if (device_push != null) form.AddField("device_push", device_push);
		// Info.GetAppType() = Free, Pay, Custom1, Custom2, Custom3
		form.AddField("app_version", Info.version.ToString());
		form.AddField("app_type", Info.appType.ToString());
		
#if UNITY_EDITOR
		form.AddField("app_platform","UnityEditor");
#elif UNITY_IPHONE
		form.AddField("app_platform","iOS");
#elif UNITY_ANDROID
		form.AddField("app_platform","Android");	
#elif UNITY_WEBPLAYER
		form.AddField("app_platform","Facebook");	
#endif
		
		conn.connect(form);
		//Debug.Log("conecta novamente");
	}

	// Processa o resultado da conexao de login
	private void connectionResult(string error, WWW data)
	{
		//Debug.Log("resultado chegou");
		JSonReader reader = new JSonReader();
		IJSonObject json = null;
		
		//Debug.Log("data: "+data.text);
		
		// Tenta ler o retorno
		if (data == null) error = "json_error";
		else
		{
			try
			{
				if (error == null) json = reader.ReadAsJSonObject(data.text);
			}
			catch (JSonReaderException)
			{
				error = "json_error";
			}
		}
		
		// Verifica se houve erro
		if (error == null && json.Contains("error")) error = json["error"].ToString();
		
		Flow.game_native.stopLoading();
		
		// Trata o erro
		if (error != null)
		{
			switch (error)
			{
				case "empty_email": error = "Please inform an e-mail."; break;
				case "invalid_email": error = "Invalid e-mail. Please try another account."; break;
				default: error = GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE; break;
			}
			Flow.game_native.showMessage("Error", error);
			
			return;
		}
		
		if (json.Contains("ask") && json["ask"].ToString() == "password")
		{			
			passwordDialog.SetActive(true);
			UIManager.instance.FocusObject = passwordField;
			
			return;
		}
		
		GameToken.save(json);
		Save.Set(PlayerPrefsKeys.EMAIL.ToString(), email);
		Save.Set(PlayerPrefsKeys.PASSWORD.ToString(), password);
		
		Save.Set(PlayerPrefsKeys.NAME.ToString(), json["username"].ToString(),true);
		Save.Set(PlayerPrefsKeys.ID.ToString(), json["user_id"].ToString(),true);
		
		// Verifica se possui Facebook
		if (json.Contains("fbtoken") && json.Contains("facebook_id"))
		{
			Save.Set(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString(), json["fbtoken"].ToString());
			Save.Set(PlayerPrefsKeys.FACEBOOK_ID.ToString(), json["facebook_id"].ToString());
		}
		
		// Verifica se e uma conta nova
		if (json["new_account"].ToString() != "0")
		{						
			messageOkDialog.transform.FindChild("ConfirmButtonPanel").FindChild("ConfirmButton").GetComponent<UIButton>().methodToInvoke = "BringInInvite";
			Flow.game_native.showMessage("Hello!", "Hi! You've registered with us! We've emailed you your password.");
			return;
		}
		
		// Atualiza token da FacebookAPI
		if (Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString())) 
		{
			FacebookAPI facebook = new FacebookAPI();
			facebook.SetToken(Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()));
		}
		
		// Redireciona a proxima cena
		
		panelManager.BringIn("MultiplayerScenePanel");
	}
	
	void ClickedConfirmPasswordDialog()
	{
		password = passwordText.Text;
		logUserIn();
		
		passwordDialog.SetActive(false);
	}
	
	void ClickedOkMessageDialog()
	{
		messageOkDialog.SetActive(false);
	}
	
	void BringInInvite()
	{
		messageOkDialog.transform.FindChild("ConfirmButtonPanel").FindChild("ConfirmButton").GetComponent<UIButton>().methodToInvoke = "ClickedOkMessageDialog";
		ClickedOkMessageDialog();
		
		// TO DO: Chamar método que carrega a lista de invite antes de chamar o painel de invite.
		
		panelManager.BringIn("InviteScenePanel");
	}
	
	void ClearText(UITextField field)
	{
		//Debug.Log("vaaaaaai");
		field.Text = "";
	}
}
