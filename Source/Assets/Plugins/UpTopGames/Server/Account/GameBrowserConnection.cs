using UnityEngine;
using System.Collections;
using CodeTitans.JSon;

public class GameBrowserConnection: MonoBehaviour
{
	protected const string
		
		MESSAGE_LOGIN_FAILED	=	"We were unable to login you in. Please refresh the page to try again";
	
	protected new void Start ()
    {
		if (Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()).IsEmpty() && !Info.IsEditor() && Info.IsWeb())
		{
		    // Up Top Fix Me
			//game_native.startLoading();
		    Application.ExternalEval("getUserInfo()");
		}
    }
	
	bool loginHappened = false;
	
	public void loginFromCanvas(string fbtoken)
    {
		if(loginHappened) return;
		loginHappened = true;
		Debug.Log("que que na unity: "+fbtoken);
		Save.Set(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString(), fbtoken);
		
		GameConnection conn = new GameConnection(Flow.URL_BASE + "login/facebook/cvlogin.php", handleLoginFromCanvas);
		
		WWWForm form = new WWWForm();
		form.AddField("app_id", Info.appId);
		form.AddField("device", SystemInfo.deviceUniqueIdentifier.Replace("-", ""));
		form.AddField("fbtoken", fbtoken);
		
		conn.connect(form);
    }
	
	private void handleLoginFromCanvas(string error, WWW data)
	{
		// Up Top Fix Me
		//GameGUI.enableGui();
		//game_native.stopLoading();
		
		Debug.Log("esta acontecendo: "+data.text);
		
		JSonReader reader = new JSonReader();
		IJSonObject json = null;
		
		// Tenta ler o retorno
		try
		{
			if (error == null) json = reader.ReadAsJSonObject(data.text);
		}
		catch (JSonReaderException e)
		{
			Debug.Log(e.StackTrace);
			Debug.Log(e.Source);
			Debug.Log(e.InnerException);
			Debug.Log(e.Message);
			error = MESSAGE_LOGIN_FAILED;
		}
		
		//Debug.Log(json);
		
		if (error != null || json == null || !json["logged"].BooleanValue)
		{
			Debug.Log(error);
			// Up Top Fix Me
			//game_native.showMessage("Error", MESSAGE_LOGIN_FAILED);
			return;
		}
		
		Save.Set(PlayerPrefsKeys.TOKEN.ToString(), json["token"].ToString());
		Save.Set(PlayerPrefsKeys.NAME.ToString(), json["username"].ToString());
		Save.Set(PlayerPrefsKeys.ID.ToString(), json["user_id"].ToString());
		
		Save.Set(PlayerPrefsKeys.FIRST_NAME.ToString(), json["first_name"].ToString());
		Save.Set(PlayerPrefsKeys.LAST_NAME.ToString(), json["last_name"].ToString());
		Save.Set(PlayerPrefsKeys.LOCATION.ToString(), json["location"].ToString());
		if(!json["gender"].IsNull) Save.Set(PlayerPrefsKeys.GENDER.ToString(), json["gender"].ToString());
		string day, month, year;
		string[] separator = {"-"};
		string[] birthday = json["birthday"].StringValue.Split(separator,System.StringSplitOptions.None);
		
		day = birthday[2];
		month = birthday[1];
		year = birthday[0];
		
		Save.Set(PlayerPrefsKeys.DATE_DAY.ToString(), day);
		Save.Set(PlayerPrefsKeys.DATE_MONTH.ToString(), month);
		Save.Set(PlayerPrefsKeys.DATE_YEAR.ToString(), year);
		
		Debug.Log(json);
		
		// Se a conta nao for nova, redireciona a cena
		if (!json["new_account"].BooleanValue) 
		{
			Debug.Log("conta velha");
			//Scene.First.Load();
		}
		else 
		{
			Debug.Log("conta nova");
			Application.ExternalEval("inviteFriends()");
		}
		
		ConfigManager.offlineUpdater.updateOfflineInApps();
	}
	
	// Convida os amigos a partir do canvas
	public void inviteFriendsFromCanvas(string request)
	{
		if (request.IsEmpty())
		{
			Debug.Log("empty request");
			//Scene.First.Load();
			return;
		}
		
		JSonReader reader = new JSonReader();
		IJSonObject json = null;
		
		// Tenta ler o retorno
		try
		{
			json = reader.ReadAsJSonObject(request);
		}
		catch (JSonReaderException e)
		{
			Debug.Log("exception: "+e.Message);
			//Scene.First.Load();
			return;
		}
		
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/facebook/invite.php", handleInviteFriendsFromCanvas);
		
		WWWForm form = new WWWForm();
		form.AddField("request", json["request"].StringValue);
		form.AddField("redirect", "no");
		
		int i = 0;
		foreach (IJSonObject friend in json["to"].ArrayItems)
		{
			form.AddField("to[" + i + "]", friend.StringValue);
			i++;
		}
		
		conn.connect(form);
	}
	
	private void handleInviteFriendsFromCanvas(string error, IJSonObject data)
	{
		Debug.Log("handleInviteFriendsFromCanvas error"+error);
		Debug.Log("handleInviteFriendsFromCanvas data"+data);
		//Scene.First.Load();
	}
}
