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
	
	// Semaforo para o loading
	protected static Semaphore semaphore = new Semaphore(1, 1);
	
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
		else
		{
			messageOkDialog.transform.FindChild("TitlePanel").FindChild("Title").GetComponent<SpriteText>().Text = title;
		}
		
		if(message.IsEmpty())
		{
			message = messageOkDialog.transform.FindChild("MessagePanel").FindChild("Message").GetComponent<SpriteText>().Text;
		}
		else
		{
			messageOkDialog.transform.FindChild("MessagePanel").FindChild("Message").GetComponent<SpriteText>().Text = message;
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
		else
		{
			 messageOkDialog.transform.FindChild("ConfirmButtonPanel").FindChild("ConfirmButton").FindChild("control_text").GetComponent<SpriteText>().Text = button;
		}

		EtceteraAndroid.showAlert(title, filterMessage(message), button);
	}
	
	// Mostra uma mensagem na tela com dois botoes
	public override void showMessageOkCancel(string title, string message, string ok_button, string cancel_button)
	{
		if (ok_button == null) ok_button = default_ok_button;
		if (cancel_button == null) cancel_button = default_cancel_button;
		
		EtceteraAndroid.showAlert(title, filterMessage(message), ok_button, cancel_button);
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
	public override void loadingMessage(string title, string message)
	{
		if (!GameNativeAndroid.semaphore.WaitOne(0)) return;
		
		EtceteraAndroid.showProgressDialog(title, message);
	}
	
	// Termina a tela de loading
	public override void stopLoading()
	{
		EtceteraAndroid.hideProgressDialog();
		
		try
		{
			GameNativeAndroid.semaphore.Release();
		}
		catch (SemaphoreFullException) {}
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
