//#define PH_USE_GENERICS
#define ANDROID_IAP_SUPPORT

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
	public delegate void RequestCompletedHandler(int requestId);
	public delegate void BadgeUpdateHandler(int requestId, string badge);
	public delegate void RewardTriggerHandler(int requestId, Reward reward);
	public delegate void PurchasePresentedTriggerHandler(int requestId, Purchase purchase);
	public delegate void SuccessHandler(int requestId);
	public delegate void WillDisplayContentHandler(int requestId);
	public delegate void DidDisplayContentHandler(int requestId);
	public delegate void DismissHandler(int requestId, DismissType dismissType);
	public delegate void SimpleDismissHandler();
	public delegate void ErrorHandler(int requestId, Error error);
	
	public enum PurchaseResolution { Buy, Cancel, Error };
	
	public enum DismissType
	{
		Unknown,
		PHPublisherContentUnitTriggeredDismiss,
		PHPublisherNativeCloseButtonTriggeredDismiss,
		PHPublisherApplicationBackgroundTriggeredDismiss,
		PHPublisherNoContentTriggeredDismiss,
	};
	
	public interface IPlayHavenListener
	{
		event RequestCompletedHandler OnRequestCompleted;
		event BadgeUpdateHandler OnBadgeUpdate;
		event RewardTriggerHandler OnRewardGiven;
		event PurchasePresentedTriggerHandler OnPurchasePresented;
		event SimpleDismissHandler OnDismissCrossPromotionWidget;
		event DismissHandler OnDismissContent;
		event WillDisplayContentHandler OnWillDisplayContent;
		event DidDisplayContentHandler OnDidDisplayContent;
		event SuccessHandler OnSuccessOpenRequest;
		event SuccessHandler OnSuccessPreloadRequest;
		event ErrorHandler OnErrorOpenRequest;
		event ErrorHandler OnErrorCrossPromotionWidget;
		event ErrorHandler OnErrorContentRequest;
		event ErrorHandler OnErrorMetadataRequest;
		
		void NotifyRequestCompleted(int requestId);
		void NotifyOpenSuccess(int requestId);
		void NotifyPreloadSuccess(int requestId);
		void NotifyOpenError(int requestId, Error error);
		void NotifyWillDisplayContent(int requestId);
		void NotifyDidDisplayContent(int requestId);
		void NotifyBadgeUpdate(int requestId, string badge);
		void NotifyRewardGiven(int requestId, Reward reward);
		void NotifyPurchasePresented(int requestId, Purchase purchase);
		void NotifyCrossPromotionWidgetDismissed();
		void NotifyCrossPromotionWidgetError(int requestId, Error error);
		void NotifyContentDismissed(int requestId, DismissType dismissType);
		void NotifyContentError(int requestId, Error error);
		void NotifyMetaDataError(int requestId, Error error);
	}
	
	public class Error
	{
		public int code;
		public string description = string.Empty;

		public override string ToString()
		{
			return "code: "+code+", description: "+description;
		}
	}
	
	public class Reward
	{
		public string receipt;
		public string name;
		public int quantity;
		
		public override string ToString()
		{
			return "name: "+name+", quantity: "+quantity+", receipt: "+receipt;
		}
	}
	
	public class Purchase
	{
		public string productIdentifier;
		public int quantity;
		public string receipt;

		public override string ToString()
		{
			return "productIdentifier: "+productIdentifier+", quantity: "+quantity+", receipt: "+receipt;
		}
	}
	
	public class PlayHavenBinding : IDisposable
	{		
		public static string token, secret;
		public static IPlayHavenListener listener;
		
#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void _PlayHavenCancelRequest(int hash);
		
		[DllImport("__Internal")]
		private static extern void _PlayHavenProductPurchaseResolution(int action);
		
		[DllImport("__Internal")]
		private static extern void _PlayHavenIAPTrackingRequest(string token, string secret, string productId, int quantity, int resolution);

		[DllImport("__Internal")]
		private static extern bool _PlayHavenOptOutStatus();

		[DllImport("__Internal")]
		private static extern void _PlayHavenSetOptOutStatus(bool yesOrNo);
#endif			

#if UNITY_ANDROID
		public static AndroidJavaObject obj_PlayHavenFacade;
#endif		
		
#if PH_USE_GENERICS
		protected static Dictionary<int, PlayHavenBinding.IPlayHavenRequest> sRequests = new Dictionary<int, PlayHavenBinding.IPlayHavenRequest>();	
#else
		protected static Hashtable sRequests = new Hashtable();
#endif
		public enum RequestType { Open, Metadata, Content, Preload, CrossPromotionWidget };
		
		public void Dispose()
		{
#if UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android && obj_PlayHavenFacade != null)
				obj_PlayHavenFacade.Dispose();
#endif
		}

#if UNITY_ANDROID
		public static void Initialize()
		{
			if (Application.platform == RuntimePlatform.Android && PlayHavenManager.IsAndroidSupported)
			{
				// Pass on the UnityPlayer's current activity to the Java facade object.
				// Also, the token and secret are set up differently from Android vs. iOS SDKs.  The Android
				// SDK doesn't receive the token and secret with every single call made to it.
				using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						obj_PlayHavenFacade = new AndroidJavaObject("com.playhaven.unity3d.PlayHavenFacade", obj_Activity, token, secret);
					}
				}
			}
		}
		
#endif			

		public static void SetKeys(string token, string secret)
		{
			if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(secret))
				return;
			
			PlayHavenBinding.token = token;
			PlayHavenBinding.secret = secret;
#if UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android && obj_PlayHavenFacade != null)
			{
				obj_PlayHavenFacade.Call("setKeys", token, secret);
			}
#endif			
		}
		
		private static bool optOutStatus;
		public static bool OptOutStatus
		{
			get
			{
#if UNITY_IPHONE
				if (Application.isEditor)
				{
					return optOutStatus;
				}
				else
				{
					return _PlayHavenOptOutStatus();
				}
#else
				return optOutStatus;
#endif				
			}
			set
			{
#if UNITY_IPHONE
				if (Application.isEditor)
				{
					optOutStatus = value;
				}
				else
				{
					_PlayHavenSetOptOutStatus(value);
				}
#else
				optOutStatus = value;
#endif
			}
		}
		
		public static int Open()
		{
			return SendRequest(PlayHavenBinding.RequestType.Open, string.Empty);
		}

		public static int Open(string customUDID)
		{
			return SendRequest(PlayHavenBinding.RequestType.Open, customUDID);
		}
		
		public static void CancelRequest(int requestId)
		{
#if UNITY_IPHONE
			if (Application.isEditor)
			{
				Debug.Log("PlayHaven: cancel request for request code = "+requestId);
				PlayHavenManager manager = PlayHavenManager.instance;
				if (manager != null)
				{
					manager.RequestCancelSuccess(requestId.ToString());
				}
			}
			else
			{
				_PlayHavenCancelRequest(requestId);
			}
#endif
		}

#if UNITY_ANDROID
		public static void RegisterActivityForTracking(bool register)
		{
			if (Application.platform == RuntimePlatform.Android && PlayHavenManager.IsAndroidSupported)
				PlayHavenBinding.obj_PlayHavenFacade.Call((register) ? "register" : "unregister");
		}
#endif
				
		public static void SendProductPurchaseResolution(PurchaseResolution resolution)
		{
			if (!Application.isEditor)
			{
#if UNITY_IPHONE
				_PlayHavenProductPurchaseResolution((int)resolution);
#elif UNITY_ANDROID && ANDROID_IAP_SUPPORT
				PlayHavenBinding.obj_PlayHavenFacade.Call("reportResolution", (int)resolution);
#endif
			}
		}
		
		public static void SendIAPTrackingRequest(Purchase purchase, PurchaseResolution resolution)
		{
			if (!Application.isEditor)
			{
#if UNITY_IPHONE
				_PlayHavenIAPTrackingRequest(token, secret, purchase.productIdentifier, purchase.quantity, (int)resolution);
#elif UNITY_ANDROID && ANDROID_IAP_SUPPORT
				PlayHavenBinding.obj_PlayHavenFacade.Call("iapTrackingRequest", purchase.productIdentifier, purchase.quantity, (int)resolution);
#endif
			}
		}
		
		public static int SendRequest(PlayHavenBinding.RequestType type, string placement)
		{
			return SendRequest(type, placement, false);
		}

		public static int SendRequest(PlayHavenBinding.RequestType type, string placement, bool showsOverlayImmediately)
		{
			IPlayHavenRequest request = null;
			
			switch (type)
			{
			case PlayHavenBinding.RequestType.Open:
				request = new OpenRequest(placement); // placement is actually customUDID
				request.OnSuccess += HandleOpenRequestOnSuccess;
				request.OnError += HandleOpenRequestOnError;
				break;
			case PlayHavenBinding.RequestType.Metadata:
				request = new MetadataRequest(placement);
				request.OnSuccess += HandleMetadataRequestOnSuccess;
				request.OnError += HandleMetadataRequestOnError;
				request.OnWillDisplay += HandleMetadataRequestOnWillDisplay;
				request.OnDidDisplay += HandleMetadataRequestOnDidDisplay;
				break;
			case PlayHavenBinding.RequestType.Content:
				request = new ContentRequest(placement);
				request.OnError += HandleContentRequestOnError;
				request.OnDismiss += HandleContentRequestOnDismiss;
				request.OnReward += HandleContentRequestOnReward;
				request.OnPurchasePresented += HandleRequestOnPurchasePresented;
				request.OnWillDisplay += HandleContentRequestOnWillDisplay;
				request.OnDidDisplay += HandleContentRequestOnDidDisplay;
				break;
			case PlayHavenBinding.RequestType.Preload:
				request = new ContentPreloadRequest(placement);
				request.OnError += HandleContentRequestOnError;
				request.OnSuccess += HandlePreloadRequestOnSuccess;
				break;
			case PlayHavenBinding.RequestType.CrossPromotionWidget:
				request = new ContentRequest("more_games");
				request.OnError += HandleCrossPromotionWidgetRequestOnError;
				request.OnDismiss += HandleCrossPromotionWidgetRequestOnDismiss;
				request.OnWillDisplay += HandleCrossPromotionWidgetRequestOnWillDisplay;
				request.OnDidDisplay += HandleCrossPromotionWidgetRequestOnDidDisplay;
				break;
			}		
			
			if (request != null)
			{
				request.Send(showsOverlayImmediately);
				return request.HashCode;
			}
			return 0;
		}

		static void HandlePreloadRequestOnSuccess (IPlayHavenRequest request, JsonData responseData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyPreloadSuccess(request.HashCode);
			}			
		}
		
		static Error CreateErrorFromJSON(JsonData errorData)
		{
			Error error = new Error();
			try
			{
				error.code = (int)errorData["code"];
			}
#if PH_USE_GENERICS
			catch (KeyNotFoundException e)
#else
			catch (Exception e)
#endif
			{
				if (Debug.isDebugBuild)
					Debug.Log(e.Message);				
			}
			try
			{
				error.description = (string)errorData["description"];
			}
#if PH_USE_GENERICS
			catch (KeyNotFoundException e)
#else
			catch (Exception e)
#endif
			{
				if (Debug.isDebugBuild)
					Debug.Log(e.Message);				
			}
			return error;
		}
		
		static void HandleCrossPromotionWidgetRequestOnDismiss(IPlayHavenRequest request, JsonData dismissData)
		{
			if (listener != null)
				listener.NotifyCrossPromotionWidgetDismissed();
		}
		
		static void HandleCrossPromotionWidgetRequestOnWillDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyWillDisplayContent(request.HashCode);
			}
		}
		
		static void HandleCrossPromotionWidgetRequestOnDidDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyDidDisplayContent(request.HashCode);
			}
		}
		
		static void HandleCrossPromotionWidgetRequestOnError (IPlayHavenRequest request, JsonData errorData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				
				Error error = CreateErrorFromJSON(errorData);
				listener.NotifyCrossPromotionWidgetError(request.HashCode, error);
			}
		}
		
		static void HandleContentRequestOnDismiss(IPlayHavenRequest request, JsonData dismissData)
		{
			DismissType dismissType = DismissType.Unknown;
			try
			{
				#pragma warning disable 0219
				string dismissTypeString = (string)dismissData["type"];
#if UNITY_IPHONE
				dismissType = (DismissType)System.Enum.Parse(typeof(DismissType), dismissTypeString);
#elif UNITY_ANDROID
				if (dismissTypeString == "ApplicationTriggered")
					dismissType = DismissType.PHPublisherApplicationBackgroundTriggeredDismiss;
				else if (dismissTypeString == "ContentUnitTriggered")
					dismissType = DismissType.PHPublisherContentUnitTriggeredDismiss;
				else if (dismissTypeString == "CloseButtonTriggered")
					dismissType = DismissType.PHPublisherNativeCloseButtonTriggeredDismiss;
				else if (dismissTypeString == "NoContentTriggered")
					dismissType = DismissType.PHPublisherNoContentTriggeredDismiss;
				
#endif
			}
#if PH_USE_GENERICS
			catch (KeyNotFoundException e)
#else
			catch (Exception e)
#endif
			{
				if (Debug.isDebugBuild)
					Debug.Log(e.Message);				
			}
			if (listener != null)
				listener.NotifyContentDismissed(request.HashCode, dismissType);
		}
		
		static void HandleContentRequestOnWillDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyWillDisplayContent(request.HashCode);
			}
		}
		
		static void HandleContentRequestOnDidDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
				listener.NotifyDidDisplayContent(request.HashCode);
		}
		
		static void HandleContentRequestOnReward (IPlayHavenRequest request, JsonData rewardData)
		{
			Reward reward = new Reward();
			try
			{
				reward.receipt = (string)rewardData["receipt"];
			}
#if PH_USE_GENERICS
			catch (KeyNotFoundException e)
#else
			catch (Exception e)
#endif
			{
				if (Debug.isDebugBuild)
					Debug.Log(e.Message);
			}
			try
			{
				reward.name = (string)rewardData["name"];
				reward.quantity = (int)rewardData["quantity"];
				
				if (listener != null)
					listener.NotifyRewardGiven(request.HashCode, reward);
			}
#if PH_USE_GENERICS
			catch (KeyNotFoundException e)
#else
			catch (Exception e)
#endif
			{
				if (Debug.isDebugBuild)
					Debug.Log(e.Message);				
			}
		}
		
		static void HandleRequestOnPurchasePresented (IPlayHavenRequest request, JsonData purchaseData)
		{
			Purchase purchase = new Purchase();
			try
			{
				purchase.receipt = (string)purchaseData["receipt"];
			}
#if PH_USE_GENERICS
			catch (KeyNotFoundException e)
#else
			catch (Exception e)
#endif
			{
				if (Debug.isDebugBuild)
					Debug.Log(e.Message);				
			}
			try
			{
				purchase.productIdentifier = (string)purchaseData["productIdentifier"];
				purchase.quantity = (int)purchaseData["quantity"];
				
				if (listener != null)
					listener.NotifyPurchasePresented(request.HashCode, purchase);
			}
#if PH_USE_GENERICS
			catch (KeyNotFoundException e)
#else
			catch (Exception e)
#endif
			{
				if (Debug.isDebugBuild)
					Debug.Log(e.Message);				
			}			
		}
			
		static void HandleContentRequestOnError (IPlayHavenRequest request, JsonData errorData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				
				Error error = CreateErrorFromJSON(errorData);
				listener.NotifyContentError(request.HashCode, error);
			}
		}
	
		static void HandleMetadataRequestOnError (IPlayHavenRequest request, JsonData errorData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				
				Error error = CreateErrorFromJSON(errorData);
				listener.NotifyMetaDataError(request.HashCode, error);
			}
		}
	
		static void HandleMetadataRequestOnSuccess (IPlayHavenRequest request, JsonData responseData)
		{
			string type, value;
			try
			{
				type = (string)responseData["notification"]["type"];
			}
#if PH_USE_GENERICS
			catch (KeyNotFoundException e)
#else
			catch (Exception e)
#endif
			{
				if (Debug.isDebugBuild)
					Debug.Log(e.Message);
				type = string.Empty;
			}
			if (type == "badge")
			{
				try
				{
					value = (string)responseData["notification"]["value"];
					if (listener != null)
					{
						//listener.NotifyRequestCompleted(request.HashCode);
						listener.NotifyBadgeUpdate(request.HashCode, value);
					}
				}
#if PH_USE_GENERICS
				catch (KeyNotFoundException e)
#else
				catch (Exception e)
#endif
				{
					if (Debug.isDebugBuild)
						Debug.Log(e.Message);					
				}
			}
		}
		
		static void HandleMetadataRequestOnWillDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyWillDisplayContent(request.HashCode);
			}
		}
		
		static void HandleMetadataRequestOnDidDisplay(IPlayHavenRequest request)
		{
			if (listener != null)
				listener.NotifyDidDisplayContent(request.HashCode);
		}
	
		static void HandleOpenRequestOnError (IPlayHavenRequest request, JsonData errorData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				
				Error error = CreateErrorFromJSON(errorData);
				listener.NotifyOpenError(request.HashCode, error);
			}
		}
	
		static void HandleOpenRequestOnSuccess (IPlayHavenRequest request, JsonData responseData)
		{
			if (listener != null)
			{
				listener.NotifyRequestCompleted(request.HashCode);
				listener.NotifyOpenSuccess(request.HashCode);
			}
		}
	
		public static IPlayHavenRequest GetRequestWithHash(int hash)
		{
			if (sRequests.ContainsKey(hash))
#if PH_USE_GENERICS
				return sRequests[hash];
#else
				return (IPlayHavenRequest)sRequests[hash];
#endif
			return null;
		}
		
		public static void ClearRequestWithHash(int hash)
		{
			if (sRequests.ContainsKey(hash))
			{
				sRequests.Remove(hash);
				if (Debug.isDebugBuild)
					Debug.Log(string.Format("Cleared request (id={0})", hash));
			}
		}
		
		// Event Handlers
		public delegate void SuccessHandler(IPlayHavenRequest request, JsonData responseData);
		public delegate void ErrorHandler(IPlayHavenRequest request, JsonData errorData);
		public delegate void RewardHandler(IPlayHavenRequest request, JsonData rewardData);
		public delegate void PurchaseHandler(IPlayHavenRequest request, JsonData purchaseData);
		public delegate void DismissHandler(IPlayHavenRequest request, JsonData dismissData);
		public delegate void GeneralHandler(IPlayHavenRequest request);
		
		//Interface
		public interface IPlayHavenRequest
		{
			event GeneralHandler OnWillDisplay;
			event GeneralHandler OnDidDisplay;
		    event SuccessHandler OnSuccess;
		    event ErrorHandler OnError;
			event DismissHandler OnDismiss;  
		    event RewardHandler OnReward;
			event PurchaseHandler OnPurchasePresented;
			
			int HashCode { get; }
			void Send();
			void Send(bool showsOverlayImmediately);
			void TriggerEvent(string eventName, JsonData eventData);
		}
		
		public class OpenRequest: IPlayHavenRequest
		{
			private int hashCode;
			
#if UNITY_IPHONE
			private string customUDID;
			
			[DllImport("__Internal")]
			private static extern void _PlayHavenOpenRequest(int hash, string token, string secret, string customUDID);
#endif			
			
			public OpenRequest()
			{
				hashCode = GetHashCode();
				sRequests.Add(hashCode, this);
			}
			
			public OpenRequest(string customUDID)
			{
#if UNITY_IPHONE
				this.customUDID = customUDID;
#endif			
				hashCode = GetHashCode();
				sRequests.Add(hashCode, this);
			}
			
			public int HashCode
			{
				get { return hashCode; }
			}
			
			public void Send()
			{
				Send(false);
			}
			
			public void Send(bool showsOverlayImmediately)
			{			
#if UNITY_IPHONE || UNITY_ANDROID
				if (Application.isEditor)
				{
					Debug.Log("PlayHaven: open request");
					PlayHavenManager manager = PlayHavenManager.instance;
					if (manager != null)
					{						
						// mimic a payload for in-editor integration validation
						// going to "dismiss"
						
						Hashtable data = new Hashtable();
						data["notification"] = new Hashtable();
						
						Hashtable result = new Hashtable();
						result["data"] = data;
						result["hash"] = hashCode;
						result["name"] = "success";
						
						string jsonResult = JsonMapper.ToJson(result);
						manager.HandleNativeEvent(jsonResult);
					}
				} 
				else 
				{
					if (Debug.isDebugBuild)
						Debug.Log(string.Format("PlayHaven: open request (id={0})", hashCode));
					#if UNITY_IPHONE
					_PlayHavenOpenRequest(hashCode, PlayHavenBinding.token, PlayHavenBinding.secret, customUDID);
					#elif UNITY_ANDROID
					if (PlayHavenManager.IsAndroidSupported)
						PlayHavenBinding.obj_PlayHavenFacade.Call("openRequest", hashCode);
					#endif
				}
#endif
			}
			
			// Events
		    public event SuccessHandler OnSuccess = delegate {};
		    public event ErrorHandler OnError = delegate {};
			public event DismissHandler OnDismiss; // not used in this implementation 
		    public event RewardHandler OnReward; // not used in this implementation
			public event PurchaseHandler OnPurchasePresented; // not used in this implementation
		    public event GeneralHandler OnWillDisplay; // not used in this implementation
		    public event GeneralHandler OnDidDisplay; // not used in this implementation
		    
		    public void TriggerEvent(string eventName, JsonData eventData)
			{
		    	if (String.Compare(eventName,"success") == 0)
				{
		    		Debug.Log("PlayHaven: Open request success!");
	 				if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
		    		OnSuccess(this, eventData);
		    	} 
				else if (String.Compare(eventName, "error") == 0)
				{
		    		Debug.LogError("PlayHaven: Open request failed!");
	 				if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
					OnError(this, eventData);
		    	}
		    }
		}
		
		public class MetadataRequest: IPlayHavenRequest
		{
			protected string mPlacement;
			private int hashCode;
			
#if UNITY_IPHONE
			[DllImport("__Internal")]
			private static extern void _PlayHavenMetadataRequest(int hash, string token, string secret, string placement);
#endif
			
			public MetadataRequest(string placement)
			{
				mPlacement = placement;
				hashCode = GetHashCode();
				sRequests.Add(hashCode, this);
			}
			
			public int HashCode
			{
				get { return hashCode; }
			}
			
			public void Send()
			{
				Send(false);
			}
			
			public void Send(bool showsOverlayImmediately)
			{
#if UNITY_IPHONE || UNITY_ANDROID
				if (Application.isEditor)
				{
					Debug.Log("PlayHaven: metadata request ("+mPlacement+")");
					PlayHavenManager manager = PlayHavenManager.instance;
					if (manager != null)
					{
						// mimic a payload for in-editor integration validation
						
						Hashtable notification = new Hashtable();
						notification["type"] = "badge";
						notification["value"] = "1";
						
						Hashtable data = new Hashtable();
						data["notification"] = notification;
						
						Hashtable result = new Hashtable();
						result["data"] = data;
						result["hash"] = hashCode;			// not part of a real result; for in-editor only
						result["name"] = "success";			// not part of a real result; for in-editor only
						result["content"] = mPlacement;						
						
						string jsonResult = JsonMapper.ToJson(result);
						manager.HandleNativeEvent(jsonResult);
					}
				} 
				else 
				{
					if (Debug.isDebugBuild)
						Debug.Log(string.Format("PlayHaven: metadata request (id={0}, placement={1})", hashCode, mPlacement));
					#if UNITY_IPHONE
					_PlayHavenMetadataRequest(hashCode, PlayHavenBinding.token, PlayHavenBinding.secret, mPlacement);
					#elif UNITY_ANDROID
					if (PlayHavenManager.IsAndroidSupported)
						PlayHavenBinding.obj_PlayHavenFacade.Call("metaDataRequest", hashCode, mPlacement);
					#endif
				}
#endif
			}
			
			// Events	    
		    public event SuccessHandler OnSuccess = delegate {};
		    public event ErrorHandler OnError = delegate {};
			public event DismissHandler OnDismiss; // not used in this implementation 
		    public event RewardHandler OnReward; // not used in this implementation
			public event PurchaseHandler OnPurchasePresented; // not used in this implementation
		    public event GeneralHandler OnWillDisplay = delegate {};
		    public event GeneralHandler OnDidDisplay = delegate {};
		    
		    public void TriggerEvent(string eventName, JsonData eventData)
			{
		    	if (String.Compare(eventName,"success") == 0)
				{
	    			Debug.Log("PlayHaven: Metadata request success!");
	 				if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
	   				//Debug.Log(JsonMapper.ToJson(eventData));
					OnSuccess(this, eventData);
		    	} 
				else if (String.Compare(eventName, "willdisplay") == 0)
				{
					OnWillDisplay(this);
				}
				else if (String.Compare(eventName, "diddisplay") == 0)
				{
					OnDidDisplay(this);
				}
				else if (String.Compare(eventName, "error") == 0)
				{
	    			Debug.LogError("PlayHaven: Metadata request failed!");
	 				if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
		    		OnError(this, eventData);
		    	}
		    }
		} 
		
		public class ContentRequest: IPlayHavenRequest
		{
			protected string mPlacement;
			private int hashCode;
			
#if UNITY_IPHONE
			[DllImport("__Internal")]
			private static extern void _PlayHavenContentRequest(int hash, string token, string secret, string placement, bool showsOverlayImmediately);
#endif
			
			public ContentRequest(string placement)
			{
				mPlacement = placement;
				hashCode = GetHashCode();				
				sRequests.Add(hashCode, this);	
			}
			
			public int HashCode
			{
				get { return hashCode; }
			}
			
			public void Send()
			{
				Send(false);
			}
			
			public void Send(bool showsOverlayImmediately)
			{
#if UNITY_IPHONE || UNITY_ANDROID
				if (Application.isEditor)
				{
					Debug.Log("PlayHaven: content request ("+mPlacement+")");
					PlayHavenManager manager = PlayHavenManager.instance;
					if (manager != null)
					{
						// mimic a payload for in-editor integration validation
						// going to "dismiss"
						
						Hashtable data = new Hashtable();
						data["notification"] = new Hashtable();
						
						Hashtable result = new Hashtable();
						result["data"] = data;
						result["hash"] = hashCode;
						result["name"] = "dismiss";
						
						string jsonResult = JsonMapper.ToJson(result);
						manager.HandleNativeEvent(jsonResult);
					}
				} 
				else 
				{
					if (Debug.isDebugBuild)
						Debug.Log(string.Format("PlayHaven: content request (id={0}, placement={1})", hashCode, mPlacement));
					#if UNITY_IPHONE
					_PlayHavenContentRequest(hashCode, PlayHavenBinding.token, PlayHavenBinding.secret, mPlacement, showsOverlayImmediately);
					#elif UNITY_ANDROID
					if (PlayHavenManager.IsAndroidSupported)
						PlayHavenBinding.obj_PlayHavenFacade.Call("contentRequest", hashCode, mPlacement);
					#endif
				}
#endif
			}
			
			// Events
		    public event SuccessHandler OnSuccess; // not used in this implementation
			public event DismissHandler OnDismiss = delegate {};  
		    public event ErrorHandler OnError = delegate {};
		    public event RewardHandler OnReward = delegate {};
		    public event PurchaseHandler OnPurchasePresented = delegate {};
		    public event GeneralHandler OnWillDisplay = delegate {};
		    public event GeneralHandler OnDidDisplay = delegate {};
		    
		    public void TriggerEvent(string eventName, JsonData eventData)
			{
		    	if (String.Compare(eventName,"reward") == 0)
				{
		   			Debug.Log("PlayHaven: Reward unlocked");
					if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
		    		OnReward(this, eventData);
		    	} 
		    	else if (String.Compare(eventName,"purchasePresentation") == 0)
				{
		   			Debug.Log("PlayHaven: Purchase presented");
					if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
		    		OnPurchasePresented(this, eventData);
		    	} 
				else if (String.Compare(eventName,"dismiss") == 0)
				{
		    		Debug.Log("PlayHaven: Content was dismissed!");
					if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
		    		OnDismiss(this, eventData);
		    	} 
				else if (String.Compare(eventName, "willdisplay") == 0)
				{
					OnWillDisplay(this);
				}
				else if (String.Compare(eventName, "diddisplay") == 0)
				{
					OnDidDisplay(this);
				}
				else if (String.Compare(eventName, "error") == 0)
				{
		    		Debug.LogError("PlayHaven: Content error!");
					if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
		    		OnError(this, eventData);
		    	}
		    }
		} 
		
		public class ContentPreloadRequest: IPlayHavenRequest
		{
			protected string mPlacement;
			private int hashCode;
			
#if UNITY_IPHONE
			[DllImport("__Internal")]
			private static extern void _PlayHavenPreloadRequest(int hash, string token, string secret, string placement);
#endif
			
			public ContentPreloadRequest(string placement)
			{
				mPlacement = placement;
				hashCode = GetHashCode();				
				sRequests.Add(hashCode, this);	
			}
			
			public int HashCode
			{
				get { return hashCode; }
			}
			
			public void Send()
			{
				Send(false);
			}
			
			public void Send(bool showsOverlayImmediately)
			{
#if UNITY_IPHONE || UNITY_ANDROID
				if (Application.isEditor)
				{
					Debug.Log("PlayHaven: content preload request ("+mPlacement+")");
					PlayHavenManager manager = PlayHavenManager.instance;
					if (manager != null)
					{
						// mimic a payload for in-editor integration validation
						// going to "dismiss"
						
						Hashtable data = new Hashtable();
						data["notification"] = new Hashtable();
						
						Hashtable result = new Hashtable();
						result["data"] = data;
						result["hash"] = hashCode;
						result["name"] = "dismiss";
						
						string jsonResult = JsonMapper.ToJson(result);
						manager.HandleNativeEvent(jsonResult);
					}
				} 
				else 
				{
					if (Debug.isDebugBuild)
						Debug.Log(string.Format("PlayHaven: content preload request (id={0}, placement={1})", hashCode, mPlacement));
					#if UNITY_IPHONE
					_PlayHavenPreloadRequest(hashCode, PlayHavenBinding.token, PlayHavenBinding.secret, mPlacement);
					#elif UNITY_ANDROID
					if (PlayHavenManager.IsAndroidSupported)
						PlayHavenBinding.obj_PlayHavenFacade.Call("preloadRequest", hashCode, mPlacement);
					#endif
				}
#endif
			}
			
			// Events
		    public event SuccessHandler OnSuccess = delegate {};
			public event DismissHandler OnDismiss;  // not used in this implementation
		    public event ErrorHandler OnError = delegate {};
		    public event RewardHandler OnReward;  // not used in this implementation
		    public event PurchaseHandler OnPurchasePresented;  // not used in this implementation
		    public event GeneralHandler OnWillDisplay;  // not used in this implementation
		    public event GeneralHandler OnDidDisplay;  // not used in this implementation
		    
		    public void TriggerEvent(string eventName, JsonData eventData)
			{
		    	if (String.Compare(eventName,"gotcontent") == 0)
				{
		   			Debug.Log("PlayHaven: Preloaded content");
					if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
		    		OnSuccess(this, eventData);
		    	} 
				else if (String.Compare(eventName, "error") == 0)
				{
		    		Debug.LogError("PlayHaven: Content error!");
					if (Debug.isDebugBuild)
						Debug.Log("JSON (trigger event): "+eventData.ToJson());
		    		OnError(this, eventData);
		    	}
		    }
		} 
	}
}