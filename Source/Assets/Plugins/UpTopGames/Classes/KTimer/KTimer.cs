using UnityEngine;
using System.Collections;

public class KTimer
{
	private KTimerManager timer;
	
	public delegate void TimerCallback();
	public delegate void TimerCallbackId(object state);
	
	public KTimer(float time, TimerCallback callback)
    {
        GameObject obj = Flow.config;

		KTimerManager timer = obj.AddComponent<KTimerManager>();
		timer.StartCoroutine(timer.Timer(time, callback));

		this.timer = timer;
	}
	
	public KTimer(float time, TimerCallbackId callback, object state)
    {
        GameObject obj = Flow.config;

		KTimerManager timer = obj.AddComponent<KTimerManager>();
		timer.StartCoroutine(timer.TimerId(time, state, callback));

		this.timer = timer;
	}
	
	public float GetTime()
    {
		if (timer != null)
			return timer.curTime;

		return default(float);
	}

	public void Stop()
    {
        if (timer != null)
            timer.Stop();
	}
}