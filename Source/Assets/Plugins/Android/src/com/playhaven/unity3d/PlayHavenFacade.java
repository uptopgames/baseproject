package com.playhaven.unity3d;

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

import com.playhaven.src.common.PHConfig;
import com.playhaven.src.common.PHAPIRequest;
import com.playhaven.src.common.PHSession;
import com.playhaven.src.publishersdk.open.PHPublisherOpenRequest;
import com.playhaven.src.publishersdk.content.PHPublisherContentRequest;
import com.playhaven.src.publishersdk.content.PHPublisherContentRequest.PHDismissType;
import com.playhaven.src.publishersdk.content.PHReward;
import com.playhaven.src.publishersdk.content.PHContent;
import com.playhaven.src.publishersdk.content.PHPurchase;
import com.playhaven.src.publishersdk.metadata.PHPublisherMetadataRequest;
import com.playhaven.src.publishersdk.purchases.PHPublisherIAPTrackingRequest;
import com.playhaven.src.publishersdk.content.PHContentView.ButtonState;

import com.unity3d.player.UnityPlayer;

import android.app.Activity;
import android.util.Log;
import android.graphics.Bitmap;

import org.json.JSONObject;
import org.json.JSONException;

import java.util.HashMap;
import java.lang.Runnable;

/**
 *
 */
public class PlayHavenFacade implements PHPublisherContentRequest.ContentDelegate, PHPublisherContentRequest.PurchaseDelegate, PHPublisherContentRequest.RewardDelegate, PHPublisherContentRequest.CustomizeDelegate, PHPublisherContentRequest.FailureDelegate
{	
    public final static String UNITY_SDK_VERSION = "android-unity-1.14.1";
    
	private Activity currentActivity;
	private PHPurchase currentPurchase;
	private boolean isPreloading;
	
	private class RequestRunner implements Runnable
	{
		private PHAPIRequest request;
		
		public void run(final Activity currentActivity, final PHAPIRequest request)
		{
			this.request = request;
			currentActivity.runOnUiThread(this);	
		}
		
		public void run()
		{
			request.send();
		}
	}

	private class PreloadRequestRunner implements Runnable
	{
		private PHPublisherContentRequest contentRequest;
		
		public void run(final Activity currentActivity, final PHPublisherContentRequest contentRequest)
		{
			this.contentRequest = contentRequest;
			currentActivity.runOnUiThread(this);
		}
		
		public void run()
		{
			contentRequest.preload();
		}
	}    
	
	/**
	  * Constructor.
	  * @param currentActivity The current activity.
      * @param token The application token, obtained from the PlayHaven web dashboard for your game.
      * @param secret The application secret, obtained from the PlayHaven web dashboard for your game.
	  */
	public PlayHavenFacade(final Activity currentActivity, String token, String secret)
	{		
		setCurrentActivity(currentActivity);
		setKeys(token, secret);
        //PlayHaven.setSDKPlatform(currentActivity, UNITY_SDK_VERSION);
	}
	
	/**
	  * Set the current activity.  This is needed so that asynchronous code can be
	  * executed on the UI thread.
	  * @param currentActivity The current activity.
	  */
	public void setCurrentActivity(final Activity currentActivity)
	{
		this.currentActivity = currentActivity;
		PHConfig.cacheDeviceInfo(currentActivity);
	}
	
    /**
     * Set the token and secret keys with the Android SDK.
     * @param token The application token, obtained from the PlayHaven web dashboard for your game.
     * @param secret The application secret, obtained from the PlayHaven web dashboard for your game.
     */
    public void setKeys(String token, String secret)
    {
 		Log.d("PlayHavenFacade", "setKeys");
		PHConfig.token = token;
		PHConfig.secret = secret;
    }

    /**
     * Register for tracking.
     */
    public void register()
    {
        if (currentActivity != null)
            PHSession.register(currentActivity);
    }
    
    /**
     * Un-register for tracking.
     */
    public void unregister()
    {
        if (currentActivity != null)
            PHSession.unregister(currentActivity);
    }
    
	/**
	 * Send the Open() request to PlayHaven, notifying the system that the game
	 * has launched.
     * @param hash A hash value that uniquely identifies the request.
	 */
	public void openRequest(int hash)
	{
 		Log.d("PlayHavenFacade", "openRequest");
		PHPublisherOpenRequest request = new PHPublisherOpenRequest(currentActivity, this);
		request.setRequestTag(hash);
		new RequestRunner().run(currentActivity, request);
	}
	
	/**
	 * Send a metadata request to PlayHaven.
     * @param hash A hash value that uniquely identifies the request.
     * @param placement The placement to associate the metadata to.
	 */
	public void metaDataRequest(int hash, String placement)
	{
 		Log.d("PlayHavenFacade", "metaDataRequest");		
		PHPublisherMetadataRequest request = new PHPublisherMetadataRequest(currentActivity, this, placement);
		request.setRequestTag(hash);
		new RequestRunner().run(currentActivity, request);
	}
	
	/**
	 * Report a resolution to a purchase promotion content unit.
	 */
	public void reportResolution(int resolution)
	{
 		Log.d("PlayHavenFacade", "reportResolution");
		if (currentPurchase != null)
		{
			currentPurchase.reportResolution(PHPurchase.Resolution.values()[resolution], currentActivity);
		}
		currentPurchase = null;
	}
	
	/**
	 * Submit an IAP tracking request.
	 */
	public void iapTrackingRequest(String productId, int quantity, int resolution)
	{
 		Log.d("PlayHavenFacade", "iapTrackingRequest");
		PHPublisherIAPTrackingRequest request = new PHPublisherIAPTrackingRequest(currentActivity, this);
		request.product = productId;
		request.quantity = quantity;
		request.resolution = PHPurchase.Resolution.values()[resolution];
		new RequestRunner().run(currentActivity, request);		
	}
	
	/**
	 * Send a content request to PlayHaven.
     * @param hash A hash value that uniquely identifies the request.
     * @param placement The placement to associate the metadata to.
	 */
	public void contentRequest(int hash, String placement)
	{
 		Log.d("PlayHavenFacade", "contentRequest");	
		PHPublisherContentRequest request = new PHPublisherContentRequest(currentActivity, this, placement);
		request.setRequestTag(hash);
		new RequestRunner().run(currentActivity, request);
	}

	/**
	 * Send a content preload request to PlayHaven.
     * @param hash A hash value that uniquely identifies the request.
     * @param placement The placement to associate the metadata to.
	 */
	public void preloadRequest(int hash, String placement)
	{
 		Log.d("PlayHavenFacade", "preloadRequest");
		PHPublisherContentRequest request = new PHPublisherContentRequest(currentActivity, this, placement);
		request.setRequestTag(hash);
		isPreloading = true;
		new PreloadRequestRunner().run(currentActivity, request);
	}
	
	// PHAPIRequestDelegate interface
	
	public void requestSucceeded(PHAPIRequest request, JSONObject responseData)
	{
 		Log.d("PlayHavenFacade", "requestSucceeded ("+request.toString()+")");
        
		if (request instanceof PHPublisherOpenRequest || (request instanceof PHPublisherContentRequest && isPreloading) || request instanceof PHPublisherMetadataRequest)
		{
			HashMap<String, Object> data = new HashMap<String, Object>(1);
			if (request instanceof PHPublisherContentRequest && isPreloading)
			{
				data.put("name", "gotcontent");
				isPreloading = false;
			}
			else
			{
				data.put("name", "success");
			}
			data.put("hash", request.getRequestTag());
			if (responseData == null)
				data.put("data", "");
			else
				data.put("data", responseData);
			responseData = new JSONObject(data);
		
			UnityPlayer.UnitySendMessage("PlayHavenManager", "HandleNativeEvent", responseData.toString());
		}
	}
	
	public void requestFailed(PHAPIRequest request, Exception e)
	{
 		Log.d("PlayHavenFacade", "requestFailed");
		
		if (request instanceof PHPublisherContentRequest && isPreloading)
			isPreloading = false;
		
		HashMap<String, Object> error = new HashMap<String, Object>(2);
		error.put("code", 0);
		error.put("description", e.getMessage());
		JSONObject errorData = new JSONObject(error);

		HashMap<String, Object> data = new HashMap<String, Object>(4);
		data.put("data", errorData);
		data.put("name", "error");
		data.put("hash", request.getRequestTag());
		JSONObject responseData = new JSONObject(data);
		
		UnityPlayer.UnitySendMessage("PlayHavenManager", "HandleNativeEvent", responseData.toString());
	}
	
	// PHPublisherContentRequestDelegate interface
	
	public void didFail(PHPublisherContentRequest request, String description)
	{
 		Log.d("PlayHavenFacade", "requestFailed");
		
		HashMap<String, Object> error = new HashMap<String, Object>(2);
		error.put("code", 0);
		error.put("description", description);
		JSONObject errorData = new JSONObject(error);

		HashMap<String, Object> data = new HashMap<String, Object>(4);
		data.put("data", errorData);
		data.put("name", "error");
		data.put("hash", request.getRequestTag());
		JSONObject responseData = new JSONObject(data);
		
		UnityPlayer.UnitySendMessage("PlayHavenManager", "HandleNativeEvent", responseData.toString());		
	}
	
	public void willGetContent(PHPublisherContentRequest request)
	{}
	
	public void willDisplayContent(PHPublisherContentRequest request, PHContent content)
	{
		Log.d("PlayHavenFacade", "willDisplayContent");	
		
		HashMap<String, Object> data = new HashMap<String, Object>(4);
		data.put("data", "");
		data.put("name", "willdisplay");
		data.put("hash", request.getRequestTag());
		JSONObject responseData = new JSONObject(data);
		
		UnityPlayer.UnitySendMessage("PlayHavenManager", "HandleNativeEvent", responseData.toString());				
	}
	
	public void didDisplayContent(PHPublisherContentRequest request, PHContent content)
	{
		Log.d("PlayHavenFacade", "didDisplayContent");	
		
		HashMap<String, Object> data = new HashMap<String, Object>(4);
		data.put("data", "");
		data.put("name", "diddisplay");
		data.put("hash", request.getRequestTag());
		JSONObject responseData = new JSONObject(data);
		
		UnityPlayer.UnitySendMessage("PlayHavenManager", "HandleNativeEvent", responseData.toString());		
	}
	
	public void didDismissContent(PHPublisherContentRequest request, PHDismissType type)
	{
		Log.d("PlayHavenFacade", "didDismissContent");	
		
		HashMap<String, Object> typeHash = new HashMap<String, Object>(1);
		typeHash.put("type", type.toString());
		JSONObject typeData = new JSONObject(typeHash);
		
		HashMap<String, Object> data = new HashMap<String, Object>(4);
		data.put("data", typeData);
		data.put("name", "dismiss");
		data.put("hash", request.getRequestTag());
		JSONObject responseData = new JSONObject(data);

		UnityPlayer.UnitySendMessage("PlayHavenManager", "HandleNativeEvent", responseData.toString());
	}
	
	public void didFail(PHPublisherContentRequest request, Exception e)
	{
		requestFailed(request, e);		
	}
	
	public void contentDidFail(PHPublisherContentRequest request, Exception e)
	{
		requestFailed(request, e);
	}

	public Bitmap closeButton(PHPublisherContentRequest request, ButtonState state)
	{
		return null; // resort to default
	}
	
	public int borderColor(PHPublisherContentRequest request, PHContent content)
	{
		return -1; // resort to default
	}
	
	public void unlockedReward(PHPublisherContentRequest request, PHReward reward)
	{
		Log.d("PlayHavenFacade", "unlockedReward");	
		
		HashMap<String, Object> rewardHash = new HashMap<String, Object>(4);
		rewardHash.put("name", reward.name);
		rewardHash.put("quantity", reward.quantity);
		rewardHash.put("receipt", reward.receipt);
		JSONObject rewardData = new JSONObject(rewardHash);

		HashMap<String, Object> data = new HashMap<String, Object>(4);
		data.put("data", rewardData);
		data.put("name", "reward");
		data.put("hash", request.getRequestTag());
		JSONObject responseData = new JSONObject(data);
		
		UnityPlayer.UnitySendMessage("PlayHavenManager", "HandleNativeEvent", responseData.toString());
	}
	
	// PHPurchaseDelegate interface
	
	public void shouldMakePurchase(PHPublisherContentRequest request, PHPurchase purchase)
	{
		Log.d("PlayHavenFacade", "shouldMakePurchase");
		
		currentPurchase = purchase;
		HashMap<String, Object> purchaseHash = new HashMap<String, Object>(4);
		purchaseHash.put("productIdentifier", purchase.product);
		purchaseHash.put("name", purchase.name);
		purchaseHash.put("quantity", purchase.quantity);
		purchaseHash.put("receipt", purchase.receipt);
		JSONObject purchaseData = new JSONObject(purchaseHash);

		HashMap<String, Object> data = new HashMap<String, Object>(4);
		data.put("data", purchaseData);
		data.put("name", "purchasePresentation");
		data.put("hash", request.getRequestTag());
		JSONObject responseData = new JSONObject(data);
		
		UnityPlayer.UnitySendMessage("PlayHavenManager", "HandleNativeEvent", responseData.toString());
	}
}
