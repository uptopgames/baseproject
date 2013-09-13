using UnityEngine;
using System;
using System.Collections;
using System.Threading;

#if UNITY_ANDROID

public class GameNativeAndroid: GameNativeBase
{
	// Mensagem padrao dos botoes de foto
	public string default_take_picture = "Take a picture";
	public string default_from_library = "Choose one";
	
	// Mensagem atual dos botoes da foto
	private string take_picture = null;
	private string from_library = null;
	
	public GameNativeAndroid()
	{
		addActionShowMessage(handlePhotoOption);
	}
	
	// Eventos de mensagem
	public override void addActionShowMessage(Action<string> action)
	{
		EtceteraAndroidManager.alertButtonClickedEvent += action;
	}
	
	public override void removeActionShowMessage(Action<string> action)
	{
		EtceteraAndroidManager.alertButtonClickedEvent -= action;
	}
	
	// Eventos de foto
	public override void addActionPhotoChosen(Action<string, Texture2D> action)
	{
		EtceteraAndroidManager.albumChooserSucceededEvent += action;
		EtceteraAndroidManager.photoChooserSucceededEvent += action;
	}
	
	public override void removeActionPhotoChosen(Action<string, Texture2D> action)
	{
		EtceteraAndroidManager.albumChooserSucceededEvent -= action;
		EtceteraAndroidManager.photoChooserSucceededEvent -= action;
	}
	
	// Eventos de compra
	public override void addActionUserCancelledPurchase(Action<string> action)
	{
		GoogleIABManager.purchaseFailedEvent += action;
	}
	
	public override void removeActionUserCancelledPurchase(Action<string> action)
	{
		GoogleIABManager.purchaseFailedEvent -= action;
	}
	
	// Mostra uma mensagem na tela com um botao
	public override void showMessage(GameObject messageOkDialog, string title = "", string message = "", string button = "")
	{
		if(title.IsEmpty())
		{
			title = messageOkDialog.transform.FindChild("TitlePanel").FindChild("Title").GetComponent<SpriteText>().Text;
		}
		
		if(message.IsEmpty())
		{
			message = messageOkDialog.transform.FindChild("MessagePanel").FindChild("Message").GetComponent<SpriteText>().Text;
		}
		
		if(button.IsEmpty())
		{
			try
			{
				button = messageOkDialog.transform.FindChild("ConfirmButtonPanel").FindChild("ConfirmButton").FindChild("control_text").GetComponent<SpriteText>().Text;
			}
			catch
			{
				button = default_ok_button;
			}
		}

		EtceteraAndroid.showAlert(title, filterMessage(message), button);
	}
	
	// Mostra uma mensagem na tela com dois botoes
	public override void showMessageOkCancel(GameObject messageOkCancelDialog,
		string title = "", string message = "", string okButton = "", string cancelButton = "")
	{
		if(title.IsEmpty())
		{
			title = messageOkCancelDialog.transform.FindChild("TitlePanel").FindChild("Title").GetComponent<SpriteText>().Text;
		}
		
		if(message.IsEmpty())
		{
			message = messageOkCancelDialog.transform.FindChild("MessagePanel").FindChild("Message").GetComponent<SpriteText>().Text;
		}
		
		if(okButton.IsEmpty())
		{
			try
			{
				okButton = messageOkCancelDialog.transform.FindChild("ConfirmButtonPanel").FindChild("ConfirmButton").FindChild("control_text").GetComponent<SpriteText>().Text;
			}
			catch
			{
				okButton = default_ok_button;
			}
		}
		
		if(cancelButton.IsEmpty())
		{
			try
			{
				cancelButton = messageOkCancelDialog.transform.FindChild("CancelButtonPanel").FindChild("CancelButton").FindChild("control_text").GetComponent<SpriteText>().Text;
			}
			catch
			{
				cancelButton = default_cancel_button;
			}
		}
		
		EtceteraAndroid.showAlert(title, filterMessage(message), okButton, cancelButton);
	}
	
	// Funcao auxiliar para a escolha de foto
	private void handlePhotoOption(string choice)
	{
		string photo_name = this.photo_name;
		if (photo_name == null) photo_name = System.Guid.NewGuid().ToString();
		this.photo_name = null;
		
		if (choice == take_picture) EtceteraAndroid.promptToTakePhoto(PHOTO_WIDTH, PHOTO_HEIGHT, photo_name);
		else if (choice == from_library) EtceteraAndroid.promptForPictureFromAlbum(PHOTO_WIDTH, PHOTO_HEIGHT, photo_name);
	}
	
	// Abre a tela para o usuario escolher uma foto
	public override void cameraRoll(float x, float y, string photo_name)
	{
		/*take_picture = default_take_picture;
		from_library = default_from_library;
		
		this.photo_name = photo_name;
		
		showMessageOkCancel("Upload your picture", "Would you like to take a picture or choose one from your library?", take_picture, from_library);*/
		EtceteraAndroid.promptForPictureFromAlbum(PHOTO_WIDTH, PHOTO_HEIGHT, photo_name);
	}
	
	// Mostra o loading com uma mensagem
	public override void loadingMessage(GameObject loadingDialog, string title = "", string message = "")
	{
		if(title.IsEmpty())
		{
			title = loadingDialog.transform.FindChild("TitlePanel").FindChild("Title").GetComponent<SpriteText>().Text;
		}
		if(message.IsEmpty())
		{
			message = loadingDialog.transform.FindChild("MessagePanel").FindChild("Message").GetComponent<SpriteText>().Text;
		}
		
		EtceteraAndroid.showProgressDialog(title, message);
	}
	
	// Termina a tela de loading
	public override void stopLoading(GameObject loadingDialog)
	{
		EtceteraAndroid.hideProgressDialog();
	}
	
	// Abre uma URL com o navegador dentro do proprio aplicativo
	public override bool openUrlInline(string url)
	{
		url += "&close=1";
		EtceteraAndroid.showCustomWebView(url, true, false);
		return true;
	}
}

#endif
