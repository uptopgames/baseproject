using UnityEngine;
using System;
using System.Collections;

#if UNITY_EDITOR || UNITY_WEBPLAYER

public class GameNativeDefault: GameNativeBase
{
	// Evento disparado ao clicar em um botao
	private event Action<string> actionShowMessage;
	
	// Evento disparado ao escolher uma foto
	private event Action<string, Texture2D> actionPhotoChosen;
	
	// Evento disparado ao cancelar uma compra
	private event Action<string> actionCancelPurchase;
	
	public GameNativeDefault()
	{
		//GameWindow.addActionOnCloseWindow(actionOnWindowClosed);
	}
	
	// Evento global chamado ao fechar uma janela
	private void actionOnWindowClosed(string message)
	{
		if (actionShowMessage != null) actionShowMessage(message);
	}
	
	// Eventos de mensagem
	public override void addActionShowMessage(Action<string> action)
	{
		actionShowMessage += action;
	}
	
	public override void removeActionShowMessage(Action<string> action)
	{
		actionShowMessage -= action;
	}
	
	// Mostra uma mensagem na tela com um botao
	public override void showMessage(string title, string message, string button)
	{
		//GameGUI.setMessageWindow(new GameMessageWindow(title, message, button));
	}
	
	// Eventos de foto
	public override void addActionPhotoChosen(Action<string, Texture2D> action)
	{
		actionPhotoChosen += action;
	}
	
	public override void removeActionPhotoChosen(Action<string, Texture2D> action)
	{
		actionPhotoChosen -= action;
	}
	
	// Eventos de compra
	public override void addActionUserCancelledPurchase(Action<string> action)
	{
		actionCancelPurchase += action;
	}
	
	public override void removeActionUserCancelledPurchase(Action<string> action)
	{
		actionCancelPurchase -= action;
	}
	
	// Mostra uma mensagem na tela com dois botoes
	public override void showMessageOkCancel(string title, string message, string ok_button, string cancel_button)
	{
		//GameGUI.setMessageWindow(new GameTwoButtonWindow(title, message, ok_button, cancel_button));
	}
	
	// Abre a tela para o usuario escolher uma foto
	public override void cameraRoll(float x, float y, string photo_name)
	{
		Debug.Log("Trying to open portrait folder");
	}
	
	// Inicia a tela de loading
	public override void loadingMessage(string title, string message)
	{
		//GameGUI.startLoading(new GameLoadingWindow(title, message));
	}
	
	// Termina a tela de loading
	public override void stopLoading()
	{
		//GameGUI.stopLoading();
	}
	
	// Abre uma URL com o navegador dentro do proprio aplicativo
	public override bool openUrlInline(string url)
	{
		//Application.ExternalEval("window.open('" + url + "', 'Window title')");
		Application.OpenURL(url);
		return true;
	}
}

#endif
