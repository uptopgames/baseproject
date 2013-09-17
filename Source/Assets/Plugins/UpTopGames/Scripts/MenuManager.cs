using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		switch(Flow.nextPanel)
		{
			case PanelToLoad.Menu:
				UIPanelManager.instance.BringIn("MenuScenePanel");
			break;
			case PanelToLoad.BattleStatus:
				UIPanelManager.instance.BringIn("BattleStatusScenePanel");
			break;
			case PanelToLoad.WinLose:
				UIPanelManager.instance.BringIn("WinLoseScenePanel");
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
