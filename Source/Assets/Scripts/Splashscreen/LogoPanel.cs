using UnityEngine;
using System.Collections;

public class LogoPanel : MonoBehaviour 
{
	public UIInteractivePanel logoPanel;
	
	// Use this for initialization
	void Start () 
	{
		logoPanel.Transitions.list[0].animParams[0].duration = 10;
		
		logoPanel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		
		logoPanel.AddTempTransitionDelegate(
		delegate {
			Application.LoadLevel(1);	
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
