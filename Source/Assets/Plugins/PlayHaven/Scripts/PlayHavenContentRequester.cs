using UnityEngine;
using System.Collections;

// COPYRIGHT(c) 2011, Medium Entertainment, Inc., a Delaware corporation, which operates a service
// called PlayHaven., All Rights Reserved
//  
// NOTICE:  All information contained herein is, and remains the property of Medium Entertainment, Inc.
// and its suppliers, if any.  The intellectual and technical concepts contained herein are 
// proprietary to Medium Entertainment, Inc. and its suppliers and may be covered by U.S. and Foreign
// Patents, patents in process, and are protected by trade secret or copyright law. Dissemination of this 
// information or reproduction of this material is strictly forbidden unless prior written permission 
// is obtained from Medium Entertainment, Inc.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// 
// Contact: support@playhaven.com

namespace PlayHaven
{
	[AddComponentMenu("PlayHaven/Content Requester")]
	public class PlayHavenContentRequester : MonoBehaviour 
	{
		public enum WhenToRequest { Awake, Start, OnEnable, OnDisable, Manual };
		public WhenToRequest whenToRequest = WhenToRequest.Manual;	// When to notify PlayHaven of the request.
		public string placement = string.Empty;
		public WhenToRequest prefetch = WhenToRequest.Manual;
		public enum InternetConnectivity { WiFiOnly, CarrierNetworkOnly, WiFiAndCarrierNetwork, Always = 100 };
		public InternetConnectivity connectionForPrefetch = InternetConnectivity.WiFiOnly;
		public bool refetchWhenUsed = false;
		public bool showsOverlayImmediately = false;
		public bool rewardMayBeDelivered = false;
		public enum MessageType { None, Send, Broadcast, Upwards };
		public MessageType rewardMessageType = MessageType.Broadcast;
		public bool useDefaultTestReward = false;
		public string defaultTestRewardName = string.Empty;
		public int defaultTestRewardQuantity = 1;
		public float requestDelay = 0;
		//public bool pauseGameWhenDisplayed = true;
		
		public bool limitedUse = false;
		public int maxUses;
		public enum ExhaustedAction { None, DestroySelf, DestroyGameObject, DestroyRoot };
		public ExhaustedAction exhaustAction = ExhaustedAction.None;
		
		private PlayHavenManager playHaven;
		private bool exhausted;
		private int uses;
		private int contentRequestId;
		private int prefetchRequestId;
		private bool requestIsInProgress;
		private bool prefetchIsInProgress;
		private bool refetch;
		
		void Awake()
		{		
			refetch = refetchWhenUsed;
			
			if (whenToRequest == WhenToRequest.Awake)
			{
				if (requestDelay > 0)
					Invoke("Request", requestDelay);
				else
					Request();
			}
			else if (prefetch == WhenToRequest.Awake)
			{
				PreFetch();
			}
		}
		
		void OnEnable()
		{
			if (whenToRequest == WhenToRequest.OnEnable)
			{
				if (requestDelay > 0)
					Invoke("Request", requestDelay);
				else
					Request();
			}
			else if (prefetch == WhenToRequest.OnEnable)
			{
				PreFetch();
			}
		}
		
		void OnDisable()
		{
			if (whenToRequest == WhenToRequest.OnDisable)
			{
				// not technically possible to delay a request that is set to
				// be called in OnDisable, so no Invoke() is implemented here
				Request();
			}
			else if (prefetch == WhenToRequest.OnDisable)
			{
				PreFetch();
			}
		}
		
		void OnDestroy()
		{
			if (Manager)
			{
				Manager.OnRewardGiven -= HandlePlayHavenManagerOnRewardGiven;
				Manager.OnDismissContent -= HandlePlayHavenManagerOnDismissContent;
				//Manager.OnWillDisplayContent -= HandleManagerOnWillDisplayContent;
				//Manager.OnDidDisplayContent -= HandleManagerOnDidDisplayContent;
			}
		}
		
		void Start()
		{
			if (whenToRequest == PlayHavenContentRequester.WhenToRequest.Start)
			{
				if (requestDelay > 0)
					Invoke("Request", requestDelay);
				else
					Request();
			}
			else if (prefetch == WhenToRequest.Start)
			{
				PreFetch();
			}
		}
		
		private PlayHavenManager Manager
		{
			get
			{
				if (!playHaven)
					playHaven = PlayHavenManager.instance;
				return playHaven;
			}
		}
		
		/// <summary>
		/// Accessor for the request identifier.
		/// </summary>
		public int RequestId
		{
			get { return contentRequestId; }
		}
		
		void RequestPlayHavenContent()
		{
			if (requestDelay > 0)
				Invoke("Request", requestDelay);
			else
				Request();
		}
		
		public bool IsExhausted
		{
			get
			{
				return limitedUse && uses > maxUses;
			}
		}
		
		public void PreFetch()
		{
			bool connectivityPermitted = true;
			switch (connectionForPrefetch)
			{
			case InternetConnectivity.WiFiOnly:
				#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5
				connectivityPermitted = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
				#else
				connectivityPermitted = iPhoneSettings.internetReachability == iPhoneNetworkReachability.ReachableViaWiFiNetwork;
				#endif
				break;
			case InternetConnectivity.CarrierNetworkOnly:
				#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5
				connectivityPermitted = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
				#else
				connectivityPermitted = iPhoneSettings.internetReachability == iPhoneNetworkReachability.ReachableViaCarrierDataNetwork;
				#endif
				break;
			case InternetConnectivity.WiFiAndCarrierNetwork:
				#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5
				connectivityPermitted = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || 
								        Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
				#else
				connectivityPermitted = iPhoneSettings.internetReachability == iPhoneNetworkReachability.ReachableViaCarrierDataNetwork || 
								        iPhoneSettings.internetReachability == iPhoneNetworkReachability.ReachableViaWiFiNetwork;
				#endif
				break;
			}
			if (!connectivityPermitted) return;
			
			if (prefetchIsInProgress)
			{
				if (Debug.isDebugBuild)
					Debug.Log("prefetch request is in progress; not making another request");
				return;
			}
			
			if (Manager)
			{
				if (placement.Length > 0)
				{
					prefetchIsInProgress = true;
					Manager.OnSuccessPreloadRequest += HandleManagerOnSuccessPreloadRequest;
					if (Debug.isDebugBuild)
						Debug.Log("Making content preload request for placement: "+placement);
					prefetchRequestId = Manager.ContentPreloadRequest(placement);
				}
				else if (Debug.isDebugBuild)
					Debug.LogError("placement value not set in PlayHaventContentRequester");
			}
			//else if (Debug.isDebugBuild)
			//	Debug.LogError("PlayHaven manager is not available in the scene. Content requests cannot be initiated.");
		}
	
		void HandleManagerOnSuccessPreloadRequest(int requestId)
		{
			if (requestId == prefetchRequestId)
			{
				prefetchIsInProgress = false;
				if (Debug.isDebugBuild)
					Debug.Log("prefetch of placement successful: "+placement);
			}
		}
		
		public void Request()
		{
			Request(refetchWhenUsed);
		}
		
		public void Request(bool refetch)
		{
			StartCoroutine(_Request(refetch));
		}
		
		IEnumerator _Request(bool refetch)
		{
			if (whenToRequest == WhenToRequest.Manual && requestDelay > 0)
				yield return new WaitForSeconds(requestDelay);
			bool doRequest = true;
			
			if (requestIsInProgress)
			{
				if (Debug.isDebugBuild)
					Debug.Log("request is in progress; not making another request");
				doRequest = false;
			}
			
			if (exhausted)
			{
				if (Application.isEditor)
					Debug.LogWarning("content requester has been exhausted");
				doRequest = false;
			}
			
			if (doRequest)
			{
				this.refetch = refetch;
				
				if (Manager)
				{
					if (placement.Length > 0)
					{
						Manager.OnDismissContent += HandlePlayHavenManagerOnDismissContent;
						
						/*
						if (pauseGameWhenDisplayed)
						{
							//Manager.OnWillDisplayContent += HandleManagerOnWillDisplayContent;
							Manager.OnDidDisplayContent += HandleManagerOnDidDisplayContent;
						}
						*/
						
						if (rewardMayBeDelivered)
						{
							Manager.OnRewardGiven -= HandlePlayHavenManagerOnRewardGiven;
							Manager.OnRewardGiven += HandlePlayHavenManagerOnRewardGiven;
						}
						
						#if UNITY_EDITOR
						contentRequestId = Manager.ContentRequest(placement, showsOverlayImmediately, this);
						if (useDefaultTestReward && defaultTestRewardName.Length > 0)
						{
							PlayHaven.Reward reward = new PlayHaven.Reward();
							reward.name = defaultTestRewardName;
							reward.quantity = defaultTestRewardQuantity;
							HandlePlayHavenManagerOnRewardGiven(contentRequestId, reward);
						}
						#else
						requestIsInProgress = true;
						contentRequestId = Manager.ContentRequest(placement, showsOverlayImmediately);
						#endif
					}
					else if (Debug.isDebugBuild)
						Debug.LogError("placement value not set in PlayHaventContentRequester");
				}
				//else if (Debug.isDebugBuild)
				//	Debug.LogWarning("PlayHaven manager is not available in the scene. Content requests cannot be initiated.");
			
				uses++;
				if (limitedUse && !rewardMayBeDelivered && uses >= maxUses)
				{
					Exhaust();
				}
			}
		}
		
		/*
		void HandleManagerOnDidDisplayContent (int requestId)
		{
			Manager.OnDidDisplayContent -= HandleManagerOnDidDisplayContent;		
			if (contentRequestId == requestId)
			{
				Time.timeScale = 0;
			}
		}
		*/
		
		/*
		void HandleManagerOnWillDisplayContent (int requestId)
		{
			Manager.OnWillDisplayContent -= HandleManagerOnWillDisplayContent;
			if (contentRequestId == requestId)
			{
				Time.timeScale = 0;
			}
		}
		*/
		
		private void Exhaust()
		{
			exhausted = true;
			switch (exhaustAction)
			{
			case ExhaustedAction.DestroySelf:
				Destroy(this);
				break;
			case ExhaustedAction.DestroyGameObject:
				Destroy(gameObject);
				break;
			case ExhaustedAction.DestroyRoot:
				Destroy(transform.root.gameObject);
				break;
			}
		}
		
		void HandlePlayHavenManagerOnDismissContent(int hashCode, PlayHaven.DismissType dismissType)
		{		
			if (contentRequestId == hashCode)
			{
				requestIsInProgress = false;
				/*
				if (pauseGameWhenDisplayed && Time.timeScale == 0)
				{
					Time.timeScale = 1f;
				}
				*/
				
				if (Manager)
				{
					Manager.OnDismissContent -= HandlePlayHavenManagerOnDismissContent;
				}
				switch (rewardMessageType)
				{
				case MessageType.Broadcast:
					BroadcastMessage("OnPlayHavenContentDismissed", dismissType, SendMessageOptions.DontRequireReceiver);
					break;
				case MessageType.Send:
					SendMessage("OnPlayHavenContentDismissed", dismissType, SendMessageOptions.DontRequireReceiver);
					break;
				case MessageType.Upwards:
					SendMessageUpwards("OnPlayHavenContentDismissed", dismissType, SendMessageOptions.DontRequireReceiver);
					break;
				}
				
				if (!exhausted && limitedUse && uses > maxUses)
				{
					Exhaust();
				}
				else if (refetch)
				{
					PreFetch();
				}
			}
		}
		
		public void HandlePlayHavenManagerOnRewardGiven(int hashCode, PlayHaven.Reward reward)
		{
			if (contentRequestId == hashCode)
			{
				switch (rewardMessageType)
				{
				case MessageType.Broadcast:
					BroadcastMessage("OnPlayHavenRewardGiven", reward);
					break;
				case MessageType.Send:
					SendMessage("OnPlayHavenRewardGiven", reward);
					break;
				case MessageType.Upwards:
					SendMessageUpwards("OnPlayHavenRewardGiven", reward);
					break;
				}
			}
		}
		
	}
}
