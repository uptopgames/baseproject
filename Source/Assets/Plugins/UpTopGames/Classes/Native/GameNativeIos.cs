using UnityEngine;
using System;
using System.Collections;

#if UNITY_IPHONE

public class GameNativeIos: GameNativeBase
{
	// Evento da acao de selecionar uma foto
	private event Action<string, Texture2D> actionPhotoPicked;
	
	public GameNativeIos()
	{
		EtceteraManager.imagePickerChoseImageEvent += handlePhotoChosen;
	}
	
	// Eventos de mensagem
	public override void addActionShowMessage(Action<string> action)
	{
		//Debug.Log("Added action " + action);
		EtceteraManager.alertButtonClickedEvent += action;
	}
	
	public override void removeActionShowMessage(Action<string> action)
	{
		//Debug.Log("Removed action " + action);
		EtceteraManager.alertButtonClickedEvent -= action;
	}
	
	// Eventos de foto
	public override void addActionPhotoChosen(Action<string, Texture2D> action)
	{
		actionPhotoPicked += action;
	}
	
	public override void removeActionPhotoChosen(Action<string, Texture2D> action)
	{
		actionPhotoPicked -= action;
	}
	
	// Eventos de compra
	public override void addActionUserCancelledPurchase(Action<string> action)
	{
		StoreKitManager.purchaseCancelledEvent += action;
	}
	
	public override void removeActionUserCancelledPurchase(Action<string> action)
	{
		StoreKitManager.purchaseCancelledEvent -= action;
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
		
		EtceteraBinding.showAlertWithTitleMessageAndButtons(title, filterMessage(message), new string[] { button });
	}
	
	// Mostra uma mensagem na tela com dois botoes
	public override void showMessageOkCancel(string title, string message, string ok_button, string cancel_button)
	{
		if (ok_button == null) ok_button = default_ok_button;
		if (cancel_button == null) cancel_button = default_cancel_button;
		
		EtceteraBinding.showAlertWithTitleMessageAndButtons(title, filterMessage(message), new string[] { ok_button, cancel_button });
	}
	
	// Obtem a imagem e chama os eventos com o resultado
	private IEnumerator handleGetImage(string path)
	{
		if (actionPhotoPicked == null) yield break;
		
		Vector2 size = EtceteraBinding.getImageSize(path);
		
		if (size.x < size.y)
		{
			size.x *= PHOTO_HEIGHT / size.y;
			size.y = PHOTO_HEIGHT;
		}
		else
		{
			size.y *= PHOTO_WIDTH / size.x;
			size.x = PHOTO_WIDTH;
		}
		
		EtceteraBinding.resizeImageAtPath(path, size.x, size.y);
		
		WWW conn = new WWW("file://" + path);
		yield return conn;
		
		actionPhotoPicked(path, conn.texture);
	}
	
	// Funcao auxiliar para a escolha de foto
	private void handlePhotoChosen(string path)
	{
		Flow.config.GetComponent<ConfigManager>().StartCoroutine(handleGetImage(path));
	}
	
	// Abre a tela para o usuario escolher uma foto
	public override void cameraRoll(float x, float y, string photo_name)
	{
		//x *= GameGUI.components.scale;
		//y *= GameGUI.components.yscale * GameGUI.components.scale;
		
		this.photo_name = photo_name;
		
		EtceteraBinding.setPopoverPoint(x, y);
		EtceteraBinding.promptForPhoto(1f, PhotoPromptType.CameraAndAlbum);
	}
	
	// Mostra o loading com uma mensagem
	public override void loadingMessage(GameObject loadingDialog, string title = "", string message = "")
	{
		if(message.IsEmpty())
		{
			message = loadingDialog.transform.FindChild("MessagePanel").FindChild("Message").GetComponent<SpriteText>().Text;
		}
		
		EtceteraBinding.showBezelActivityViewWithLabel(message);
	}
	
	// Inicia a tela de loading
	public override void stopLoading(GameObject loadingDialog)
	{
		EtceteraBinding.hideActivityView();
	}
	
	// Abre uma URL com o navegador dentro do proprio aplicativo
	public override bool openUrlInline(string url)
	{
		EtceteraBinding.showWebPage(url, false);
		return true;
	}
}

#endif
