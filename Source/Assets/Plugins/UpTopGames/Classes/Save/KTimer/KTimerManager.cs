using UnityEngine;
using System.Collections;

public class KTimerManager : MonoBehaviour 
{
	
	public float curTime = 0f;
	bool running = false; 
	
	public IEnumerator TimerId(float time, object state, KTimer.TimerCallbackId callback) 
	{
		running = true;
		yield return new WaitForSeconds(time);
		if (callback != null) callback(state);
		Destroy(this);
	}
	
	public IEnumerator Timer(float time, KTimer.TimerCallback callback) 
	{
		running = true;
		yield return new WaitForSeconds(time);
		if (callback != null) callback();
		Destroy(this);
	}
	
	void Update()
	{ 
		if (running) curTime += Time.deltaTime; 
	}
	
	public void Stop()
	{ 
		Destroy(this); 
	}
}
