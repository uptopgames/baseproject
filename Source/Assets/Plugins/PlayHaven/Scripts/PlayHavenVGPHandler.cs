//#define USE_GENERICS
using UnityEngine;
using System.Collections;
#if USE_GENERICS
using System.Collections.Generic;
#endif

namespace PlayHaven
{
	[AddComponentMenu("PlayHaven/VGP Handler")]
	public class PlayHavenVGPHandler : MonoBehaviour 
	{
		public delegate void PurchaseEventHandler(int requestId, PlayHaven.Purchase purchase);
		public event PurchaseEventHandler OnPurchasePresented;
		
		private static PlayHavenVGPHandler instance;
		private PlayHavenManager playHaven;
		#if USE_GENERICS
		private Dictionary<int, PlayHaven.Purchase> purchases = new Dictionary<int, PlayHaven.Purchase>(4);
		#else
		private Hashtable purchases = new Hashtable(4);
		#endif
		
		/// <summary>
		/// Gets the singleton instance of the VGP handler.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static PlayHavenVGPHandler Instance
		{
			get
			{
				if (!instance)
					instance = GameObject.FindObjectOfType(typeof(PlayHavenVGPHandler)) as PlayHavenVGPHandler;
				return instance;
			}
		}
		
		void Awake()
		{
			playHaven = PlayHavenManager.instance;
		}
		
		void OnEnable()
		{
			playHaven.OnPurchasePresented += PlayHavenOnPurchasePresented;
		}
		
		void OnDisable()
		{
			playHaven.OnPurchasePresented -= PlayHavenOnPurchasePresented;
		}
	
		void PlayHavenOnPurchasePresented(int requestId, PlayHaven.Purchase purchase)
		{
			if (OnPurchasePresented != null)
			{
				purchases.Add(requestId, purchase);
				OnPurchasePresented(requestId, purchase);
			}
		}
		
		/// <summary>
		/// Resolves the purchase indicated by the request identifier.
		/// </summary>
		/// <param name='requestId'>
		/// Request identifier.
		/// </param>
		/// <param name='track'>
		/// If true, also submit a tracking request to PlayHaven.
		/// </param>
		public void ResolvePurchase(int requestId, PlayHaven.PurchaseResolution resolution, bool track)
		{
			if (purchases.ContainsKey(requestId))
			{
				#if USE_GENERICS
				PlayHaven.Purchase purchase = purchases[requestId];
				#else
				PlayHaven.Purchase purchase = (PlayHaven.Purchase)purchases[requestId];
				#endif
				purchases.Remove(requestId);
				playHaven.ProductPurchaseResolutionRequest(resolution);
				if (track)
					playHaven.ProductPurchaseTrackingRequest(purchase, resolution);
			}
			else if (Debug.isDebugBuild)
			{
				Debug.LogWarning("PlayHaven VGP handler does not have a record of a purchase with the provided request identifier: "+requestId);
			}
		}
		
		/// <summary>
		/// Resolves the purchase indicated by the purchase object.
		/// </summary>
		/// <param name='requestId'>
		/// Request identifier.
		/// </param>
		/// <param name='track'>
		/// If true, also submit a tracking request to PlayHaven.
		/// </param>
		public void ResolvePurchase(PlayHaven.Purchase purchase, PlayHaven.PurchaseResolution resolution, bool track)
		{
			if (!purchases.ContainsValue(purchase))
			{
				if (Debug.isDebugBuild)
					Debug.LogWarning("PlayHaven VGP handler does not have a record of a purchase with the provided purchase object; will track only if requested.");
				if (track)
					playHaven.ProductPurchaseTrackingRequest(purchase, resolution);
			}
			else
			{
				int requestId = -1;
				foreach (int rid in purchases.Keys)
				{
					if (purchases[rid] == purchase)
					{
						requestId = rid;
						break;
					}
				}
				if (requestId > -1)
				{
					purchases.Remove(requestId);
					playHaven.ProductPurchaseResolutionRequest(resolution);
					if (track)
						playHaven.ProductPurchaseTrackingRequest(purchase, resolution);
				}
				else
					Debug.LogError("Unable to determine request identifier for provided purchase object.");
			}
		}
	}
}