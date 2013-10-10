using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using CodeTitans.JSon;

public class AccountSettings : MonoBehaviour 
{
	public const string firstNamePlaceholder = "First Name";
	public const string lastNamePlaceholder = "Last Name";
	public const string locationPlaceholder = "City, Country";
	public const string dayPlaceholder = "DD";
	public const string monthPlaceholder = "MM";
	public const string yearPlaceholder = "YYYY";
	
	public UIInteractivePanel accountSettingsPanel;
	
	public UITextField firstNameField;
	public UITextField lastNameField;
	public UITextField countryField;
	public UITextField dayField;
	public UITextField monthField;
	public UITextField yearField;
	public UIButton emailField;
	public UIStateToggleBtn maleFemaleToggle;
	public UIButton facebookButton;
	
	public GameObject changeEmailDialog;
	public UITextField changeEmailPasswordField;
	public UITextField changeEmailNewMailField;
	public SpriteText changeEmailMessage;
	
	public GameObject changePasswordDialog;
	public UITextField changePasswordOldPassword;
	public UITextField changePasswordNewPassword;
	public UITextField changePasswordConfirmPassword;
	public SpriteText changePasswordMessage;
	
	bool loggingOut = false;
	public UIScrollList gamesScroll;
	
	// Nome da foto do usuario
	public const string PHOTO_NAME = "settings.photo.img";
	
	bool photoChanged = false;
	public MeshRenderer photoMeshRenderer;
	
	GameFacebook fb_account;
	
	
	// Use this for initialization
	void Start () 
	{
		accountSettingsPanel.transitions.list[0].AddTransitionStartDelegate(AccountSettingsLoad);
		
		dayField.AddValidationDelegate(DateFieldValidation);
		monthField.AddValidationDelegate(DateFieldValidation);
		yearField.AddValidationDelegate(DateFieldValidation);
		dayField.AddFocusDelegate(ClearText);
		monthField.AddFocusDelegate(ClearText);
		yearField.AddFocusDelegate(ClearText);
		firstNameField.AddFocusDelegate(ClearText);
		lastNameField.AddFocusDelegate(ClearText);
		countryField.AddFocusDelegate(ClearText);
		changeEmailNewMailField.AddFocusDelegate(ClearText);
		
		fb_account = new GameFacebook(HandleLinkFacebook);
	}
	
	string dayValidation = "";
	string monthValidation = "";
	string yearValidation = "";
	
	string DateFieldValidation(UITextField field, string text, ref int insertion)
	{
		//Debug.Log("before: "+dayValidation);
		//Debug.Log(insertion);
		int validating = -97236872;
		//Debug.Log("now: "+text);
		try
		{
			validating = int.Parse(text);
		}
		catch(FormatException e)
		{
			if(field == dayField && text != "") text = dayValidation;
			if(field == monthField && text != "") text = monthValidation;
			if(field == yearField && text != "") text = yearValidation;
		}
		
		if(field == dayField && validating != -927236872 && validating > 31) text = dayValidation;
		if(field == monthField && validating != -927236872 && validating > 12) text = monthValidation;
		if(field == yearField && validating != -927236872 && insertion > 4) text = yearValidation;
		
		if(field == dayField) dayValidation = text;
		if(field == monthField) monthValidation = text;
		if(field == yearField) yearValidation = text;
		return text.Trim();
	}
	
	void ClearText(UITextField field)
	{
		if(field == dayField && field.Text == dayPlaceholder) field.Text = "";
		if(field == monthField && field.Text == monthPlaceholder) field.Text = "";
		if(field == yearField && field.Text == yearPlaceholder) field.Text = "";
		if(field == firstNameField && field.Text == firstNamePlaceholder) field.Text = "";
		if(field == lastNameField && field.Text == lastNamePlaceholder) field.Text = "";
		if(field == countryField && field.Text == locationPlaceholder) field.Text = "";
		if(field == changeEmailNewMailField && field.Text == "New Email") field.Text = "";
	}
	
	void AccountSettingsLoad(EZTransition transition)
	{
		//Debug.Log(Save.GetString(PlayerPrefsKeys.EMAIL.ToString()));
		if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString())) facebookButton.Text = "Logged";
		if(Save.HasKey(PlayerPrefsKeys.EMAIL.ToString())) emailField.Text = Save.GetString(PlayerPrefsKeys.EMAIL.ToString());
		if(Save.HasKey(PlayerPrefsKeys.FIRST_NAME.ToString())) firstNameField.Text = Save.GetString(PlayerPrefsKeys.FIRST_NAME.ToString());
		if(Save.HasKey(PlayerPrefsKeys.LAST_NAME.ToString())) lastNameField.Text = Save.GetString(PlayerPrefsKeys.LAST_NAME.ToString());
		if(Save.HasKey(PlayerPrefsKeys.LOCATION.ToString())) countryField.Text = Save.GetString(PlayerPrefsKeys.LOCATION.ToString());
		if(Save.HasKey(PlayerPrefsKeys.DATE_DAY.ToString())) dayField.Text = Save.GetString(PlayerPrefsKeys.DATE_DAY.ToString());
		if(Save.HasKey(PlayerPrefsKeys.DATE_MONTH.ToString())) monthField.Text = Save.GetString(PlayerPrefsKeys.DATE_MONTH.ToString());
		if(Save.HasKey(PlayerPrefsKeys.DATE_YEAR.ToString())) yearField.Text = Save.GetString(PlayerPrefsKeys.DATE_YEAR.ToString());
		if(Save.HasKey(PlayerPrefsKeys.GENDER.ToString())) 
		{
			if(Save.GetString(PlayerPrefsKeys.GENDER.ToString()) == "Male") maleFemaleToggle.SetState(0);
			else maleFemaleToggle.SetState(1);
		}
		dayValidation = dayField.Text;
		monthValidation = monthField.Text;
		yearValidation = yearField.Text;
		
		if(Flow.playerPhoto == null && !Flow.isDownloadingPlayerPhoto)
		{
			GameRawAuthConnection conn2 = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", GetPlayerPhoto);
            WWWForm form2 = new WWWForm();
            form2.AddField("user_id", "me");
            conn2.connect(form2);
            Flow.isDownloadingPlayerPhoto = true;
		}
	}
	
	void GetPlayerPhoto(string error, WWW data)
    {
        Flow.isDownloadingPlayerPhoto = false;
        if (error != null)
        {
            if (error.IndexOf("404") >= 0)
            {
				
            }
        }
        else
        {
            Flow.playerPhoto = data.texture;
			
			photoMeshRenderer.material.mainTexture = Flow.playerPhoto;
        }
    }
	
	bool changed = false;
	
	void DoneButton()
	{
		
		//Debug.Log("save "+Save.GetString(PlayerPrefsKeys.GENDER.ToString()));
		//Debug.Log("field "+maleFemaleToggle.StateName);
		if(Save.HasKey(PlayerPrefsKeys.FIRST_NAME.ToString()) && firstNameField.Text != Save.GetString(PlayerPrefsKeys.FIRST_NAME.ToString())) 
		{
			changed = true;
			Debug.Log("first != 1");
		}
		else if(!Save.HasKey(PlayerPrefsKeys.FIRST_NAME.ToString()) && firstNameField.Text != firstNamePlaceholder && firstNameField.Text != "") 
		{
			changed = true;
			Debug.Log("first != 2");
		}
		
		if(Save.HasKey(PlayerPrefsKeys.LAST_NAME.ToString()) && lastNameField.Text != Save.GetString(PlayerPrefsKeys.LAST_NAME.ToString()))
		{
			changed = true;
			Debug.Log("last != 1");
		}
		else if(!Save.HasKey(PlayerPrefsKeys.LAST_NAME.ToString()) && lastNameField.Text != lastNamePlaceholder && lastNameField.Text != "")
		{
			changed = true;
			Debug.Log("last != 2");
		}
		
		if(Save.HasKey(PlayerPrefsKeys.LOCATION.ToString()) && countryField.Text != Save.GetString(PlayerPrefsKeys.LOCATION.ToString()))
		{
			changed = true;
			Debug.Log("loc != 1");
		}
		else if(!Save.HasKey(PlayerPrefsKeys.LOCATION.ToString()) && countryField.Text != locationPlaceholder && countryField.Text != "")
		{
			changed = true;
			Debug.Log("loc != 2");
		}
		
		if(Save.HasKey(PlayerPrefsKeys.DATE_DAY.ToString()) && dayField.Text != Save.GetString(PlayerPrefsKeys.DATE_DAY.ToString()))
		{
			changed = true;
			Debug.Log("day != 1");
		}
		else if(!Save.HasKey(PlayerPrefsKeys.DATE_DAY.ToString()) && dayField.Text != dayPlaceholder && dayField.Text != "")
		{
			changed = true;
			Debug.Log("day != 2");
		}
		
		if(Save.HasKey(PlayerPrefsKeys.DATE_MONTH.ToString()) && monthField.Text != Save.GetString(PlayerPrefsKeys.DATE_MONTH.ToString()))
		{
			changed = true;
			Debug.Log("month != 1");
		}
		else if(!Save.HasKey(PlayerPrefsKeys.DATE_MONTH.ToString()) && monthField.Text != monthPlaceholder && monthField.Text != "")
		{
			changed = true;
			Debug.Log("month != 2");
		}
		
		if(Save.HasKey(PlayerPrefsKeys.DATE_YEAR.ToString()) && yearField.Text != Save.GetString(PlayerPrefsKeys.DATE_YEAR.ToString()))
		{
			changed = true;
			Debug.Log("year != 1");
		}
		else if(!Save.HasKey(PlayerPrefsKeys.DATE_YEAR.ToString()) && yearField.Text != yearPlaceholder && yearField.Text != "")
		{
			changed = true;
			Debug.Log("year != 2");
		}
		Debug.Log("Save: "+Save.GetString(PlayerPrefsKeys.GENDER));
		Debug.Log("Toggle: "+maleFemaleToggle.StateName);
		if(Save.HasKey(PlayerPrefsKeys.GENDER.ToString()) && maleFemaleToggle.StateName != Save.GetString(PlayerPrefsKeys.GENDER.ToString()))
		{
			changed = true;
			Debug.Log("gender != 1");
		}
		else if(!Save.HasKey(PlayerPrefsKeys.GENDER.ToString()) && maleFemaleToggle.StateName != "Male")
		{
			changed = true;
			Debug.Log("gender != 2");
		}
		
		if(photoChanged)
		{
			Debug.Log("photo changed");
			changed = true;
		}
		
		if(changed)
		{
			Flow.game_native.startLoading();
			
			Save.Set (PlayerPrefsKeys.DATE_DAY.ToString(), dayField.Text, true);
			Save.Set (PlayerPrefsKeys.DATE_MONTH.ToString(), monthField.Text, true);
			Save.Set (PlayerPrefsKeys.DATE_YEAR.ToString(), yearField.Text, true);
			Save.Set (PlayerPrefsKeys.LOCATION.ToString(), countryField.Text, true);
			Save.Set (PlayerPrefsKeys.FIRST_NAME.ToString(), firstNameField.Text, true);
			Save.Set (PlayerPrefsKeys.LAST_NAME.ToString(), lastNameField.Text, true);
			Save.Set (PlayerPrefsKeys.GENDER.ToString(), maleFemaleToggle.StateName, true);
			
			Dictionary <string, string> messages = new Dictionary<string, string>();
			messages.Add("invalid_date", "Invalid date. Please, try again.");
			
			GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/settings/set.php", HandleSaveOnServer, messages);
			WWWForm form = new WWWForm();
			if(monthField.Text != monthPlaceholder) form.AddField("month", monthField.Text);
			if(dayField.Text != dayPlaceholder) form.AddField("day", dayField.Text);
			if(yearField.Text != yearPlaceholder) form.AddField("year", yearField.Text);
			if(countryField.Text != locationPlaceholder) form.AddField("location", countryField.Text);
			if(firstNameField.Text != firstNamePlaceholder) form.AddField("first_name", firstNameField.Text);
			if(lastNameField.Text != lastNamePlaceholder) form.AddField("last_name", lastNameField.Text);
			form.AddField("gender", maleFemaleToggle.StateName);
			if(Flow.playerPhoto != null && photoChanged) form.AddBinaryData("photo", Flow.playerPhoto.EncodeToPNG(), PHOTO_NAME, "image/png");
			
			conn.connect(form);
		}
		else
		{
			UIPanelManager.instance.BringIn("MenuScenePanel",UIPanelManager.MENU_DIRECTION.Backwards);
		}
		
	}
	
	void HandleSaveOnServer(string error, IJSonObject data)
	{
		Flow.game_native.stopLoading();
		if (error != null)
		{
			Flow.game_native.showMessage("Error", error);
			dayField.Text = dayPlaceholder;
			monthField.Text = monthPlaceholder;
			yearField.Text = yearPlaceholder;
			Save.Set (PlayerPrefsKeys.DATE_DAY.ToString(), dayField.Text, true);
			Save.Set (PlayerPrefsKeys.DATE_MONTH.ToString(), monthField.Text, true);
			Save.Set (PlayerPrefsKeys.DATE_YEAR.ToString(), yearField.Text, true);
			
			return;
		}
		
		changed = false;
		photoChanged = false;
		
		UIPanelManager.instance.BringIn("MenuScenePanel",UIPanelManager.MENU_DIRECTION.Backwards);
	}
	
	void LinkFacebook()
	{
		if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString())) return;
		
		Flow.game_native.startLoading();
		fb_account.link();
		
	}
	
	void HandleLinkFacebook(string error, IJSonObject data)
	{
		Flow.game_native.stopLoading();
		
		if (error != null)
		{
			Flow.game_native.showMessage("Error", error);
			return;
		}
		
		Save.Set(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString(), data["fbtoken"].ToString(), true);
		facebookButton.Text = "Logged";
	}
	
	void Logout()
	{
		Flow.game_native.showMessageOkCancel(this,"ConfirmLogout",ConfirmLogoutNative,"","Logout","Are you sure you want to logout?","Yes","Nevermind");
		loggingOut = true;
	}
	
	void ConfirmLogout()
	{
		loggingOut = false;
		Flow.messageOkCancelDialog.SetActive(false);
		if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()))
		{
			bool opened = Flow.game_native.openAuthUrlInline(Flow.URL_BASE + "login/logout.php?");
			if(!opened) return;
			
			Flow.OnLogoutFromServer();
			
			firstNameField.Text = firstNamePlaceholder;
			lastNameField.Text = lastNamePlaceholder;
			countryField.Text = locationPlaceholder;
			maleFemaleToggle.SetState(0);
			emailField.Text = "Email";
			dayField.Text = dayPlaceholder;
			monthField.Text = monthPlaceholder;
			yearField.Text = yearPlaceholder;
			facebookButton.Text = "Login with Facebook";
			
			gamesScroll.ClearList(true);
			
			UIPanelManager.instance.BringIn("MenuScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
			
			return;
		}
		
		// Desloga com o servidor
		Flow.game_native.startLoading();
		
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/logout.php", HandleUserLogoutConnection);
		conn.connect();
		
	}
	
	void HandleUserLogoutConnection(string error, IJSonObject data)
	{
		Flow.game_native.stopLoading();
		if(error != null) Flow.game_native.showMessage("Error", error);
		else
		{
			Flow.OnLogoutFromServer();
			
			firstNameField.Text = firstNamePlaceholder;
			lastNameField.Text = lastNamePlaceholder;
			countryField.Text = locationPlaceholder;
			maleFemaleToggle.SetState(0);
			emailField.Text = "Email";
			dayField.Text = dayPlaceholder;
			monthField.Text = monthPlaceholder;
			yearField.Text = yearPlaceholder;
			facebookButton.Text = "Login with Facebook";
			
			gamesScroll.ClearList(true);
			
			UIPanelManager.instance.BringIn("MenuScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
		}
	}
	
	void ConfirmLogoutNative(string button)
	{
		if(loggingOut && button == "Yes") ConfirmLogout();
	}
	
	void ChangePassword()
	{
		changePasswordDialog.SetActive(true);
	}
	
	void ConfirmChangePassword()
	{
		// Mensagens de erro da conexao
		Dictionary <string, string> messages = new Dictionary<string, string>();
		messages.Add("empty_password", "Please inform your password.");
		messages.Add("inform_password", "Please inform a new password.");
		messages.Add("differ_password", "Your passwords are different.");
		messages.Add("short_password", "Your new password is too short.");
		messages.Add("wrong_password", "Your current password is incorrect.");
		messages.Add("cant_send_email", "We couldn't send you a confirmation e-mail.");
		
		// Informa ao servidor a troca de senha
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/settings/password.php", HandleChangePassword, messages);
		
		WWWForm form = new WWWForm();
		form.AddField("password", changePasswordOldPassword.Text);
		form.AddField("new_password", changePasswordNewPassword.Text);
		form.AddField("new2_password", changePasswordConfirmPassword.Text);
		
		conn.connect(form);
	}
	
	void HandleChangePassword(string error, IJSonObject data)
	{
		Debug.Log(error);
		Debug.Log(data);
		if(error != null)
		{
			changePasswordMessage.Text = error;
			if(error == "Please inform your password." || error == "Your current password is incorrect.") changePasswordOldPassword.SetColor(new Color(135f/255f,70f/255f,70f/255f,1));
			else if(error == "Please inform a new password." || error == "Your new password is too short.") changePasswordNewPassword.SetColor(new Color(135f/255f,70f/255f,70f/255f,1));
			else if(error == "Your passwords are different.") 
			{
				changePasswordNewPassword.SetColor(new Color(135f/255f,70f/255f,70f/255f,1));
				changePasswordConfirmPassword.SetColor(new Color(135f/255f,70f/255f,70f/255f,1));
			}
		}
		else
		{
			Save.Set(PlayerPrefsKeys.PASSWORD.ToString(), changePasswordNewPassword.Text, true);
			CloseChangePasswordDialog();
			Flow.game_native.showMessage("Success", "Your password has been changed! We've sent you a confirmation e-mail.");
		}
	}
	
	void CloseChangePasswordDialog()
	{
		changePasswordDialog.SetActive(false);
		changePasswordMessage.Text = "Your password must have at least 6 characters long";
		changePasswordNewPassword.SetColor(new Color(1,1,1,1));
		changePasswordConfirmPassword.SetColor(new Color(1,1,1,1));
		changePasswordOldPassword.SetColor(new Color(1,1,1,1));
		changePasswordOldPassword.Text = "";
		changePasswordConfirmPassword.Text = "";
		changePasswordNewPassword.Text = "";
	}
	
	void ChangeEmail()
	{
		if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()))
		{
			changeEmailDialog.SetActive(true);
		}
		else
		{
			Flow.game_native.showMessage("Can't change email", "You can't change your e-mail without linking a Facebook account");
		}
	}
	
	void ConfirmChangeEmail()
	{
		// Mensagens de erro da conexao
		Dictionary <string, string> messages = new Dictionary<string, string>();
		messages.Add("empty_password", "Please inform your password.");
		messages.Add("empty_email", "Please. Inform a new e-mail.");
		messages.Add("invalid_email", "Your e-mail is invalid. Please, try another account.");
		messages.Add("no_facebook_connected", "You have to link your Facebook to change your e-mail.");
		messages.Add("wrong_password", "Wrong password. Please try again.");
		messages.Add("cant_send_email", "We couldn't check your e-mail. Please try again later.");
		
		// Informa ao servidor a troca de email
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/settings/email.php", HandleChangeEmail, messages);
		
		WWWForm form = new WWWForm();
		form.AddField("password", changeEmailPasswordField.Text);
		form.AddField("email", changeEmailNewMailField.Text);
		
		conn.connect(form);
	}
	
	void HandleChangeEmail(string error, IJSonObject data)
	{
		Debug.Log(data);
		Debug.Log(error);
		
		if(error != null) 
		{
			changeEmailMessage.Text = error;
			if(error == "Your e-mail is invalid. Please, try another account." || error == "Please. Inform a new e-mail.") changeEmailNewMailField.SetColor(new Color(135f/255f,70f/255f,70f/255f,1));
			else if(error == "Wrong password. Please try again." || error == "Please inform your password.") changeEmailPasswordField.SetColor(new Color(135f/255f,70f/255f,70f/255f,1));
		}
		else
		{
			CloseChangePasswordDialog();
			Flow.game_native.showMessage("Success!", "Please, check your e-mail to confirm your change.");
		}
		
	}
	
	void CloseChangeEmailDialog()
	{
		changeEmailDialog.SetActive(false);
		changeEmailMessage.Text = "Type your password to change your email";
		changeEmailPasswordField.Text = "";
		changeEmailNewMailField.Text = "New Email";
		changeEmailNewMailField.SetColor(new Color(1,1,1,1));
		changeEmailPasswordField.SetColor(new Color(1,1,1,1));
	}
	
	void ChangePhoto()
	{
		Flow.game_native.cameraRoll(0,0, PHOTO_NAME);
	}
	
	void OnEnable()
	{
		Flow.game_native.addActionPhotoChosen(HandleUserChangePhoto);
	}
	
	void OnDisable()
	{
		Flow.game_native.removeActionPhotoChosen(HandleUserChangePhoto);
	}
	
	// Atualiza a foto do usuario
	void HandleUserChangePhoto(string filename, Texture2D photo)
	{
		DeletePhoto();
		
		Flow.playerPhoto = photo;
		photoMeshRenderer.material.mainTexture = Flow.playerPhoto;
		photoChanged = true;
	}
	
	// Deleta a photo
	public void DeletePhoto()
	{
		File.Delete(PHOTO_NAME);
		
		photoChanged = false;
	}
}
