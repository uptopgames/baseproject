using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
 
internal static class ContinuationManager
{
    private class Job
    {
        public Job(Func<bool> completed, Action continueWith)
        {
            Completed = completed;
            ContinueWith = continueWith;
        }
        public Func<bool> Completed { get; private set; }
        public Action ContinueWith { get; private set; }
    }
 
    private static readonly List<Job> jobs = new List<Job>();
 
    public static void Add(Func<bool> completed, Action continueWith)
    {
        if (!jobs.Any()) EditorApplication.update += Update;
        jobs.Add(new Job(completed, continueWith));
    }
 
    private static void Update()
    {
		Debug.Log("pqp...");
        for (int i = 0; i >= 0; --i)
        {
			Debug.Log("for louco "+i);
            var jobIt = jobs[i];
            if (jobIt.Completed())
            {
                jobIt.ContinueWith();
                jobs.RemoveAt(i);
            }
        }
		
		Debug.Log("jobs: "+ jobs.Any());
        if (!jobs.Any()) EditorApplication.update -= Update;
    }
}