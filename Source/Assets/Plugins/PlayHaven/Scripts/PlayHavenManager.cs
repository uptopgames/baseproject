//#define PH_USE_GENERICS

using UnityEngine;
using System;
using System.Collections;
#if PH_USE_GENERICS
using System.Collections.Generic;
#endif
using System.Runtime.InteropServices;
using LitJson;

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
	[AddComponentMenu("PlayHaven/Manager")]
	public class PlayHavenManager : MonoBehaviour, IPlayHavenListener 
	{	
		public static string KEY_LAUNCH_COUNT = "playhaven-launch-count";
		public const int NO_HASH_CODE = 0;
			
		public event RequestCompletedHandler OnRequestCompleted;
		public event BadgeUpdateHandler OnBadgeUpdate;
		public event RewardTriggerHandler OnRewardGiven;
		public event PurchasePresentedTriggerHandler OnPurchasePresented;
		public event SimpleDismissHandler OnDismissCrossPromotionWidget;
		public event DismissHandler OnDismissContent;
		public event WillDisplayContentHandler OnWillDisplayContent;
		public event DidDisplayContentHandler OnDidDisplayContent;
		public event SuccessHandler OnSuccessOpenRequest;
		public event SuccessHandler OnSuccessPreloadRequest;
		public event ErrorHandler OnErrorOpenRequest;
		public event ErrorHandler OnErrorCrossPromotionWidget;
		public event ErrorHandler OnErrorContentRequest;
		public event ErrorHandler OnErrorMetadataRequest;
		
		public delegate void CancelRequestHandler(int requestId);
		public event CancelRequestHandler OnSuccessCancelRequest;
		public event CancelRequestHandler OnErrorCancelRequest;
	
		public string token = "";				// Your PlayHaven application token.
		[HideInInspector] public bool lockToken = false;
		public string secret = "";				// Your PlayHaven application secret.
		[HideInInspector] public bool lockSecret = false;
		public string tokenAndroid = "";		// Your PlayHaven application token.
		[HideInInspector] public bool lockTokenAndroid = false;
		public string secretAndroid = "";		// Your PlayHaven application secret.
		[HideInInspector] public bool lockSecretAndroid = false;
		public bool doNotDestroyOnLoad = true;	// If true, the game object with the manager attached to it will persist from one scene to the next
		public bool defaultShowsOverlayImmediately = false;
		public bool maskShowsOverlayImmediately = false;
		
		public enum WhenToOpen { Awake, Start, Manual };
		public WhenToOpen whenToSendOpen = WhenToOpen.Awake;	// When to notify PlayHaven that your game has opened.
			
		public enum WhenToGetNotifications { Disabled, Awake, Start, OnEnable, Manual, Poll };
		public WhenToGetNotifications whenToGetNotifications = WhenToGetNotifications.Start;
		public string badgeMoreGamesPlacement = "more_games";
		public float notificationPollDelay = 1f;
		public float notificationPollRate = 15f;
		
		public bool cancelAllOnLevelLoad = false;
		#if PH_USE_GENERICS
		private List<int> requestsInProgress = new List<int>(8);
		#else
		private ArrayList requestsInProgress = new ArrayList(8);
		#endif
		
		public int suppressContentRequestsForLaunches = 0; // suppress content requests until this many game launches have occurred
		public string[] suppressedPlacements; // placements that should be suppressed
		public string[] suppressionExceptions; // placements that should not be suppressed
		private int launchCount;
		
		public bool showContentUnitsInEditor = true;
		#if UNITY_EDITOR
		#pragma warning disable 0414
		private GUISkin integrationSkin;
		#endif
		
		private string badge = string.Empty;
		private string customUDID = string.Empty;
		
		#if UNITY_IPHONE
		private bool autoCallOpenUponUnpause = false;
		#endif
		private bool networkReachable = true;
		public bool maskNetworkReachable = false; // for testing
		public bool isAndroidSupported = false; // automatically set to true or false if the playhaven-x.x.x.jar is found or not; needs to be serialized
		
		// Access to the singleton instance of the PlayHaven manager.
		private static PlayHavenManager _instance;
		private static bool wasWarned;	
		public static PlayHavenManager instance
		{
			get
			{
				if (!_instance)
				{
					_instance = FindInstance();
				}
				return _instance;
			}
		}
	
		private static PlayHavenManager FindInstance()
		{
			PlayHavenManager i = GameObject.FindObjectOfType(typeof(PlayHavenManager)) as PlayHavenManager;
			if (!i)
			{
				GameObject go = GameObject.Find("PlayHavenManager");
				if (go != null)
					i = go.GetComponent<PlayHavenManager>();
			}
			if (!i && !wasWarned)
			{
				Debug.LogWarning("unable to locate a PlayHavenManager in the scene");
				wasWarned = true;
			}
			return i;
		}
		
		// Executed when the manager's game object is recognized by the engine.  If "whenToSendOpen" is set to 
		// Awake, PlayHaven will be notified that your game has opened.
	    void Awake()
		{ 
			_instance = FindInstance();
			
			DetectNetworkReachable();
						
			gameObject.name = "PlayHavenManager";
			if (doNotDestroyOnLoad)
				DontDestroyOnLoad(this);
			
			#if UNITY_EDITOR
			DetermineInEditorDevice();
			integrationSkin = (GUISkin)Resources.Load("PlayHavenIntegrationSkin", typeof(GUISkin));
			#endif
			
			// launch counting
			if (suppressContentRequestsForLaunches > 0)
			{
				launchCount = PlayerPrefs.GetInt(KEY_LAUNCH_COUNT, 0);
				launchCount++;
				PlayerPrefs.SetInt(KEY_LAUNCH_COUNT, launchCount);
				PlayerPrefs.Save();
				if (Debug.isDebugBuild)
					Debug.Log("Launch count: "+launchCount);
			}
					
			// 
			#if UNITY_ANDROID
			PlayHavenBinding.Initialize();
			#endif
			
			#if !ENABLE_MANUAL_PH_MANAGER_INSTANTIATION
			#if UNITY_IPHONE
			if (string.IsNullOrEmpty(token))
				Debug.LogError("PlayHaven token has not been specified in the PlayerHavenManager");
			if (string.IsNullOrEmpty(secret))
				Debug.LogError("PlayHaven secret has not been specified in the PlayerHavenManager");
			#elif UNITY_ANDROID
			if (string.IsNullOrEmpty(tokenAndroid))
				Debug.LogError("PlayHaven token has not been specified in the PlayerHavenManager");
			if (string.IsNullOrEmpty(secretAndroid))
				Debug.LogError("PlayHaven secret has not been specified in the PlayerHavenManager");
			#endif
			#if UNITY_ANDROID
			PlayHavenBinding.SetKeys(tokenAndroid, secretAndroid);
			#elif UNITY_IPHONE
			PlayHavenBinding.SetKeys(token, secret);
			#endif
			PlayHavenBinding.listener = this;
			if (whenToSendOpen == PlayHavenManager.WhenToOpen.Awake)
				OpenNotification();
	
			if (whenToGetNotifications == PlayHavenManager.WhenToGetNotifications.Awake)
				BadgeRequest(badgeMoreGamesPlacement);
			#endif
		}
		
		void OnEnable()
		{
			if (whenToGetNotifications == PlayHavenManager.WhenToGetNotifications.OnEnable)
				BadgeRequest(badgeMoreGamesPlacement);
		}
		
		// Executed before the first frame.  If "whenToSendOpen" is set to Start, PlayHaven will
		// be notified that your game has opened.
		void Start()
		{
			if (whenToSendOpen == PlayHavenManager.WhenToOpen.Start)
				OpenNotification();
			
			if (whenToGetNotifications == PlayHavenManager.WhenToGetNotifications.Start)
				BadgeRequest(badgeMoreGamesPlacement);
			else if (whenToGetNotifications == PlayHavenManager.WhenToGetNotifications.Poll)
				PollForBadgeRequests();
	
			#if UNITY_IPHONE
			autoCallOpenUponUnpause = true;
			#endif
		}
		
		void DetectNetworkReachable()
		{
			#if UNITY_IPHONE || UNITY_ANDROID
				#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5
				networkReachable = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || 
								   Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
				#else
				networkReachable = iPhoneSettings.internetReachability == iPhoneNetworkReachability.ReachableViaCarrierDataNetwork || 
								   iPhoneSettings.internetReachability == iPhoneNetworkReachability.ReachableViaWiFiNetwork;
				#endif
			#endif
			networkReachable &= !maskNetworkReachable;
		}
		
		void OnApplicationPause(bool pause)
		{
			#if UNITY_IPHONE
			if (!pause)
			{
				DetectNetworkReachable();
				if (autoCallOpenUponUnpause)
					OpenNotification();
			}
			#elif UNITY_ANDROID
			if (!pause)
				DetectNetworkReachable();
			PlayHavenBinding.RegisterActivityForTracking(!pause);
			#endif
		}
		
		void OnLevelWasLoaded(int level)
		{
			if (cancelAllOnLevelLoad)
				CancelAllPendingRequests();
		}
	
		/// <summary>
		/// Accessor for the optional custom UDID value.  This should be set prior to
		/// to the OpenNotification() being sent.  If this is set, then it is passed
		/// along with the OpenNotification().  It is used if you are tracking individual
		/// devices with your own custom unique identifier that is not the devices built-in
		/// unique device identifier.
		/// </summary>
		public string CustomUDID
		{
			get { return customUDID; }
			set { customUDID = value; }
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PlayHavenManager"/> opt out status.
		/// </summary>
		/// <value>
		/// <c>true</c> if opt out status; otherwise, <c>false</c>.
		/// </value>
		public bool OptOutStatus
		{
			get { return PlayHavenBinding.OptOutStatus; }
			set { PlayHavenBinding.OptOutStatus = value; }
		}
		
		/// <summary>
		/// Returns true if Android is supported.  During build time, the isAndroidSupported serializable
		/// boolean is set to true if the playhave-x.x.x.jar can be found.  There is/was a time whereby
		/// Android functionality was only provided to a select number of customers.
		/// 
		/// You should not go setting isAndroidSupported to true yourself and expect it to work!
		/// </summary>
		public static bool IsAndroidSupported
		{
			get
			{
				PlayHavenManager manager = PlayHavenManager.instance;
				if (manager != null)
					return manager.isAndroidSupported;
				return false;
			}
		}
		
		/// <summary>
		/// Determines whether the specified placement will be suppressed.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the specified placement will be suppressed; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='placement'>
		/// Th placement to check.
		/// </param>
		public bool IsPlacementSuppressed(string placement)
		{
			if (suppressContentRequestsForLaunches > 0 && launchCount < suppressContentRequestsForLaunches)
			{
				if (suppressedPlacements != null && suppressedPlacements.Length > 0)
				{
					foreach (string suppressedPlacement in suppressedPlacements)
					{
						if (suppressedPlacement == placement)
							return true; // the placement is in the list, so it WILL be suppressed
					}
					return false; // the placement was not in the list
				}
				if (suppressionExceptions != null && suppressionExceptions.Length > 0)
				{
					foreach (string suppressionException in suppressionExceptions)
					{
						if (suppressionException == placement)
							return false; // the placement is in the exception list, so it WON'T be suppressed
					}
					return true; // the placement was not in the exception list, do suppress it
				}
				return true; // suppression is enabled but there are no specific suppressions or exceptions, so everything goes
			}
			return false; // suppression is not enabled
		}
			
		// Inform PlayHaven that the game has opened.  This can be called automatically upon Awake, Start, or
		// manually by calling this method anywhere in your own code.  A custom UDID is used if you are tracking 
		// individual devices with your own custom unique identifier that is not the devices built-in
		// unique device identifier.
		public int OpenNotification(string customUDID)
		{
			if (networkReachable)
			{
				CustomUDID = customUDID;
				int requestId = PlayHavenBinding.Open(customUDID);
				#if !UNITY_EDITOR
				requestsInProgress.Add(requestId);
				#endif
				return requestId;
			}
			return NO_HASH_CODE;
		}
		
		// Inform PlayHaven that the game has opened.  This can be called automatically upon Awake, Start, or
		// manually by calling this method anywhere in your own code.
		public int OpenNotification()
		{
			if (networkReachable)
			{
				int requestId = PlayHavenBinding.Open(CustomUDID);
				#if !UNITY_EDITOR
				requestsInProgress.Add(requestId);
				#endif
				return requestId;
			}
			return NO_HASH_CODE;
		}
	
		#region request canceling
		
		/// <summary>
		/// Cancel any requests that are currently pending.
		/// </summary>
		public void CancelAllPendingRequests()
		{
			foreach (int requestId in requestsInProgress)
			{
				PlayHavenBinding.CancelRequest(requestId);				
			}
			requestsInProgress.Clear();
		}
		
		#endregion
		
		#region IAP
		
		/// <summary>
		/// Resolve a purchase request (buy, cancel, fail).
		/// </summary>
		public void ProductPurchaseResolutionRequest(PurchaseResolution resolution)
		{
			PlayHavenBinding.SendProductPurchaseResolution(resolution);
		}
		
		/// <summary>
		/// Track an in-app purchase.
		/// </summary>
		public void ProductPurchaseTrackingRequest(Purchase purchase, PurchaseResolution resolution)
		{
			PlayHavenBinding.SendIAPTrackingRequest(purchase, resolution);
		}
		
		#endregion
		
		# region Content Request
		
		// Perform a content preload request.
		public int ContentPreloadRequest(string placement)
		{
			#if UNITY_EDITOR
			inEditorContentUnitType = InEditorContentUnitType.None;
			#endif
			if (networkReachable)
			{
				int requestId = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.Preload, placement);
				#if !UNITY_EDITOR
				requestsInProgress.Add(requestId);
				#endif
				return requestId;
			}
			return NO_HASH_CODE;
		}	
		
		// Perform a content request.
		public int ContentRequest(string placement)
		{
			if (IsPlacementSuppressed(placement)) return NO_HASH_CODE;
			
			#if UNITY_EDITOR
			inEditorContentUnitType = InEditorContentUnitType.Generic;
			#endif
			if (networkReachable)
			{
				int requestId = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.Content, placement, defaultShowsOverlayImmediately);
				#if !UNITY_EDITOR
				requestsInProgress.Add(requestId);
				#endif
				return requestId;
			}
			return NO_HASH_CODE;
		}
		
		#if UNITY_EDITOR
		// Perform a content request (editor only, for testing).
		public int ContentRequest(string placement, bool showsOverlayImmediately, PlayHavenContentRequester requester)
		{
			if (IsPlacementSuppressed(placement)) return NO_HASH_CODE;
			
			inEditorContentUnitType = (requester.rewardMayBeDelivered) ? InEditorContentUnitType.Reward : InEditorContentUnitType.Generic;
			if (networkReachable)
			{
				return PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.Content, placement, showsOverlayImmediately && !maskShowsOverlayImmediately);
			}
			return NO_HASH_CODE;
		}
		#endif
	
		// Perform a content request.
		public int ContentRequest(string placement, bool showsOverlayImmediately)
		{
			if (IsPlacementSuppressed(placement)) return NO_HASH_CODE;
			
			#if UNITY_EDITOR
			inEditorContentUnitType = InEditorContentUnitType.Generic;
			#endif
			if (networkReachable)
			{
				int requestId = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.Content, placement, showsOverlayImmediately && !maskShowsOverlayImmediately);
				#if !UNITY_EDITOR
				requestsInProgress.Add(requestId);
				#endif
				return requestId;
			}
			return NO_HASH_CODE;
		}
	
		#endregion
		
		#region Cross Promotion Widget
		
		// Show the cross-promotion widget
		[Obsolete("This method is obsolete; it assumes that you will have a placement called more_games; instead, simply use ContentRequest() but with the relevant placement.",false)]
		public int ShowCrossPromotionWidget()
		{
			#if UNITY_EDITOR
			inEditorContentUnitType = InEditorContentUnitType.CrossPromotionWidget;
			#endif
			if (networkReachable)
			{
				int requestId = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.CrossPromotionWidget, string.Empty, defaultShowsOverlayImmediately);
				#if !UNITY_EDITOR
				requestsInProgress.Add(requestId);
				#endif
				return requestId;
			}
			return NO_HASH_CODE;
		}
	
		#endregion
		
		#region Badges
		
		// Request notification badge data
		public int BadgeRequest(string placement)
		{
			if (networkReachable && whenToGetNotifications != PlayHavenManager.WhenToGetNotifications.Disabled)
			{
				int requestId = PlayHavenBinding.SendRequest(PlayHavenBinding.RequestType.Metadata, placement);
				#if !UNITY_EDITOR
				requestsInProgress.Add(requestId);
				#endif
				return requestId;
			}
			return NO_HASH_CODE;
		}

		// Request notification badge data
		[Obsolete("This method is obsolete; it assumes that you will have a placement called more_games; instead, simply use BadgeRequest() but with the relevant placement.",false)]
		public int BadgeRequest()
		{
			return BadgeRequest("more_games");
		}
	
		// Poll for badge requests.
		public void PollForBadgeRequests()
		{
			CancelInvoke("BadgeRequestPolled");
			if (notificationPollRate > 0)
			{
				if (!string.IsNullOrEmpty(badgeMoreGamesPlacement))
					InvokeRepeating("BadgeRequestPolled", notificationPollDelay, notificationPollRate);
				else if (Debug.isDebugBuild)
					Debug.LogError("A more games badge placement is not defined.");
			}
			else
				Debug.LogError("cannot have a notification poll rate <= 0");
		}
		
		private void BadgeRequestPolled()
		{
			BadgeRequest(badgeMoreGamesPlacement);
		}
				
		#region In-editor Testing
		
		#if UNITY_EDITOR
		private enum InEditorContentUnitType { None, Generic, CrossPromotionWidget, Announcement, Advertisement, Reward };
		#pragma warning disable 0414
		private InEditorContentUnitType inEditorContentUnitType = InEditorContentUnitType.None;
		
		private enum InEditorDevice { Unknown, iPhone, iPhoneRetina, iPad };
		#pragma warning disable 0414
		private InEditorDevice inEditorDevice = InEditorDevice.Unknown;
		
		#pragma warning disable 0414
		private bool isRetina = false;
		
		private enum InEditorOrientation { Unknown, Landscape, Portrait };
		#pragma warning disable 0414
		private InEditorOrientation inEditorOrientation = InEditorOrientation.Unknown;
		
		private const int STYLE_INEDITOR_OVERLAY = 0;
		private const int STYLE_INEDITOR_DISMISS_BUTTON = 1;
		private const int STYLE_INEDITOR_CONTENTUNIT_LANDSCAPE_GENERIC = 2;
		private const int STYLE_INEDITOR_CONTENTUNIT_PORTRAIT_GENERIC = 3;
		private const int STYLE_INEDITOR_CONTENTUNIT_LANDSCAPE_REWARD = 4;
		private const int STYLE_INEDITOR_CONTENTUNIT_PORTRAIT_REWARD = 5;
		
		void DetermineInEditorDevice()
		{
			#if UNITY_IPHONE
			switch (Screen.width)
			{
			case 480:
				inEditorDevice = InEditorDevice.iPhone;
				inEditorOrientation = InEditorOrientation.Landscape;
				break;
			case 960:
				inEditorDevice = InEditorDevice.iPhoneRetina;
				inEditorOrientation = InEditorOrientation.Landscape;
				isRetina = true;
				break;
			case 320:
				inEditorDevice = InEditorDevice.iPhone;
				inEditorOrientation = InEditorOrientation.Portrait;
				break;
			case 640:
				inEditorDevice = InEditorDevice.iPhoneRetina;
				inEditorOrientation = InEditorOrientation.Portrait;
				isRetina = true;
				break;
			case 1024:
				inEditorDevice = InEditorDevice.iPad;
				inEditorOrientation = InEditorOrientation.Landscape;
				break;
			case 768:
				inEditorDevice = InEditorDevice.iPad;
				inEditorOrientation = InEditorOrientation.Portrait;
				break;
			}
			#endif
		}
		
		void OnGUI()
		{
			if (!showContentUnitsInEditor) return;
			#if UNITY_ANDROID
			if (!isAndroidSupported) return;
			#endif
			#if UNITY_IPHONE || UNITY_ANDROID
			if (integrationSkin == null) return;
			
			if (inEditorContentUnitType != InEditorContentUnitType.None)
			{
				GUI.depth = -10;
				
				Vector3 scaleVector = Vector3.one;
				if (!isRetina && inEditorDevice != InEditorDevice.iPad)
					scaleVector *= 0.5f;
				
				// overlay
				GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);			
				GUI.Label(new Rect(0,0,Screen.width,Screen.height), string.Empty, integrationSkin.customStyles[STYLE_INEDITOR_OVERLAY]);
			
				// scaling
				GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scaleVector);
				//GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / refScreenWidth, (Screen.width / (refScreenWidth/refScreenHeight)) / refScreenHeight, 1));
				
				string buttonLabel = "Dismiss";
				switch (inEditorContentUnitType)
				{
				case InEditorContentUnitType.Generic:
					break;
				case InEditorContentUnitType.CrossPromotionWidget:
					break;
				case InEditorContentUnitType.Announcement:
					break;
				case InEditorContentUnitType.Advertisement:
					break;
				case InEditorContentUnitType.Reward:
					buttonLabel = "Give Me!";
					break;
				}
				
				// background image
				Rect backgroundRect;
				int backgroundStyleIndex = (inEditorContentUnitType == InEditorContentUnitType.Reward) ? STYLE_INEDITOR_CONTENTUNIT_LANDSCAPE_REWARD : STYLE_INEDITOR_CONTENTUNIT_LANDSCAPE_GENERIC;
				if (inEditorOrientation == InEditorOrientation.Landscape)
				{
					backgroundRect = new Rect(50,34,860,573);
					if (inEditorDevice == InEditorDevice.iPad)
					{
						backgroundRect.x += 32;
						backgroundRect.y += 63;
					}
				}
				else
				{
					backgroundRect = new Rect(34,50,573,860);
					backgroundStyleIndex = (inEditorContentUnitType == InEditorContentUnitType.Reward) ? STYLE_INEDITOR_CONTENTUNIT_PORTRAIT_REWARD : STYLE_INEDITOR_CONTENTUNIT_PORTRAIT_GENERIC;
					if (inEditorDevice == InEditorDevice.iPad)
					{
						backgroundRect.x += 63;
						backgroundRect.y += 32;
					}
				}
				GUI.Label(backgroundRect, string.Empty, integrationSkin.customStyles[backgroundStyleIndex]);
				
				// dismiss button
				Rect dismissButtonRect;
				if (inEditorOrientation == InEditorOrientation.Landscape)
				{
					dismissButtonRect = new Rect(960-256-16,640-96-16,256,96);
					if (inEditorDevice == InEditorDevice.iPad)
					{
						dismissButtonRect.x += 32;
						dismissButtonRect.y += 63;
					}
				}
				else
				{
					dismissButtonRect = new Rect(640-256-16,960-96-16,256,96);
					if (inEditorDevice == InEditorDevice.iPad)
					{
						dismissButtonRect.x += 63;
						dismissButtonRect.y += 32;
					}
				}
				if (GUI.Button(dismissButtonRect, buttonLabel, integrationSkin.customStyles[STYLE_INEDITOR_DISMISS_BUTTON]))
				{
					inEditorContentUnitType = InEditorContentUnitType.None;
				}
			}
			#endif
		}
		#endif
		
		#endregion
		
		#region Notifications from Binding
		
		//
		public void NotifyRequestCompleted(int requestId)
		{
			#if UNITY_EDITOR
			Debug.Log("Request completed: "+requestId);
			#endif
			#if !UNITY_EDITOR
			requestsInProgress.Remove(requestId);
			#endif
			
			if (OnRequestCompleted != null)
			{
				OnRequestCompleted(requestId);
			}
		}
		
		//
		public void NotifyOpenSuccess(int requestId)
		{
			if (OnSuccessOpenRequest != null)
			{
				OnSuccessOpenRequest(requestId);
			}
		}
		
		//
		public void NotifyOpenError(int requestId, PlayHaven.Error error)
		{
			if (OnErrorOpenRequest != null)
			{
				OnErrorOpenRequest(requestId, error);
			}
		}
		
		//
		public void NotifyWillDisplayContent(int requestId)
		{
			if (OnWillDisplayContent != null)
			{
				OnWillDisplayContent(requestId);
			}
		}
		
		//
		public void NotifyDidDisplayContent(int requestId)
		{
			if (OnDidDisplayContent != null)
			{
				OnDidDisplayContent(requestId);
			}
		}
	
		//
		public void NotifyPreloadSuccess(int requestId)
		{
			if (OnSuccessPreloadRequest != null)
			{
				OnSuccessPreloadRequest(requestId);
			}
		}
		
		// 
		public void NotifyBadgeUpdate(int requestId, string badge)
		{
			this.badge = badge;
			if (OnBadgeUpdate != null) 
			{
				OnBadgeUpdate(requestId, badge);
			}
		}
		
		//
		public void NotifyRewardGiven(int requestId, Reward reward)
		{
			if (OnRewardGiven != null)
			{
				OnRewardGiven(requestId, reward);
			}
		}
		
		//
		public void NotifyPurchasePresented(int requestId, Purchase purchase)
		{
			if (OnPurchasePresented != null)
			{
				OnPurchasePresented(requestId, purchase);
			}
		}
		
		//
		public void NotifyCrossPromotionWidgetDismissed()
		{
			if (OnDismissCrossPromotionWidget != null)
			{
				OnDismissCrossPromotionWidget();
			}
		}
		
		//
		public void NotifyCrossPromotionWidgetError(int requestId, PlayHaven.Error error)
		{
			if (OnErrorCrossPromotionWidget != null)
			{
				OnErrorCrossPromotionWidget(requestId, error);
			}
		}
		
		//
		public void NotifyContentDismissed(int requestId, PlayHaven.DismissType dismissType)
		{
			if (OnDismissContent != null)
			{
				OnDismissContent(requestId, dismissType);
			}
		}
		
		//
		public void NotifyContentError(int requestId, PlayHaven.Error error)
		{
			if (OnErrorContentRequest != null)
			{
				OnErrorContentRequest(requestId, error);
			}
		}
		
		//
		public void NotifyMetaDataError(int requestId, PlayHaven.Error error)
		{
			if (OnErrorMetadataRequest != null)
			{
				OnErrorMetadataRequest(requestId, error);
			}
		}
		
		#endregion
		
		// The current badge value.
		public string Badge
		{
			get { return badge; }
		}
		
		// Clear the badge.
		public void ClearBadge()
		{
			badge = string.Empty;
		}
		
		#endregion
		
		// Calls from Objective-C code or when running in editor; there is no need to use these methods
		// manually from anywhere within your code.
		
		public void HandleNativeEvent(string json)
		{
			if (Debug.isDebugBuild)
				Debug.Log("JSON (native event): "+json);
			JsonData nativeData = JsonMapper.ToObject(json);
			int hash = (int)nativeData["hash"];
	
			PlayHavenBinding.IPlayHavenRequest request = PlayHavenBinding.GetRequestWithHash(hash);
			if (request != null)
			{
				string eventName = (string)nativeData["name"];
				if (Debug.isDebugBuild)
					Debug.Log(string.Format("PlayHaven event={0} (id={1})", eventName, hash));
				
				// possible events:
				//		willdisplay
				//		diddisplay
				//		dismiss
				//		success
				//		reward
				//		error
				//		purchasePresentation
				//		gotcontent
				
				JsonData eventData = (JsonData)nativeData["data"];
				request.TriggerEvent(eventName, eventData);
				if (request is PlayHavenBinding.ContentRequest && eventName == "reward")
				{
					// Delay reward request clearing because they may have more than 1 reward
					// configured but there is nothing to inform us how many there will be.
					// Since these will come into Unity super fast, 1s should be plenty.
					StartCoroutine(DelayedClearRequestWithHash(1f, hash));
				}
				else if (eventName != "willdisplay" && eventName != "diddisplay" && eventName != "gotcontent")
				{
					PlayHavenBinding.ClearRequestWithHash(hash);
				}
			}
			else if (Debug.isDebugBuild)
			{
				Debug.LogError("Unable to locate request with id="+hash);
			}
		}
		
		IEnumerator DelayedClearRequestWithHash(float delay, int hash)
		{
			yield return new WaitForSeconds(delay);
			PlayHavenBinding.ClearRequestWithHash(hash);
		}

		public void RequestCancelSuccess(string hashCodeString)
		{
			int hashCode = System.Convert.ToInt32(hashCodeString);
			PlayHavenBinding.ClearRequestWithHash(hashCode);
			if (OnSuccessCancelRequest != null)
				OnSuccessCancelRequest(hashCode);
		}
	
		public void RequestCancelFailed(string hashCodeString)
		{
			if (OnErrorCancelRequest != null)
			{
				int hashCode = System.Convert.ToInt32(hashCodeString);
				OnErrorCancelRequest(hashCode);
			}
		}
	}
}
