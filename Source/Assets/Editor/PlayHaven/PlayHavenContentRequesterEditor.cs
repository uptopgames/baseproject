using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using PlayHaven;

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

namespace PlayHavenEditor
{
	[CustomEditor(typeof(PlayHavenContentRequester))]
	public class PlayHavenContentRequesterEditor : PlayHavenInspector
	{
		private SerializedObject requester;
		private SerializedProperty whenToRequest;
		private SerializedProperty placement;
		private SerializedProperty showsOverlayImmediately;
		private SerializedProperty rewardMayBeDelivered;
		private SerializedProperty rewardMessageType;
		private SerializedProperty requestDelay;
		private SerializedProperty limitedUse;
		private SerializedProperty maxUses;
		private SerializedProperty exhaustAction;
		private SerializedProperty prefetch;
		private SerializedProperty refetchWhenUsed;
		private SerializedProperty connectionForPrefetch;
		//private SerializedProperty pauseGameWhenDisplayed;
		
		private SerializedProperty useDefaultTestReward;
		private SerializedProperty defaultTestRewardName;
		private SerializedProperty defaultTestRewardQuantity;
		
		private Color originalBackgroundColor;
		private bool guiDisabledOrig = false;
		private bool delayRequest = false;
		private PlayHavenSettings settings;
		private List<string> placementTagsList = null;
		private int selectedPlacement = -1;
		private List<string> rewardTagsList = null;
		private int selectedReward = -1;
		
		private const float DELAY_MIN = 0.1f;
		private const float DELAY_MAX = 30f;
		
		private string testRewardKey = string.Empty;
		private int testRewardQuantity = 1;
			
		protected override void OnEnable()
		{
			base.OnEnable();
			
			requester = new SerializedObject(target);
			whenToRequest = requester.FindProperty("whenToRequest");
			placement = requester.FindProperty("placement");
			showsOverlayImmediately = requester.FindProperty("showsOverlayImmediately");
			rewardMayBeDelivered = requester.FindProperty("rewardMayBeDelivered");
			rewardMessageType = requester.FindProperty("rewardMessageType");
			requestDelay = requester.FindProperty("requestDelay");
			limitedUse = requester.FindProperty("limitedUse");
			maxUses = requester.FindProperty("maxUses");
			exhaustAction = requester.FindProperty("exhaustAction");
			useDefaultTestReward = requester.FindProperty("useDefaultTestReward");
			defaultTestRewardName = requester.FindProperty("defaultTestRewardName");
			defaultTestRewardQuantity = requester.FindProperty("defaultTestRewardQuantity");
			prefetch = requester.FindProperty("prefetch");
			refetchWhenUsed = requester.FindProperty("refetchWhenUsed");
			connectionForPrefetch = requester.FindProperty("connectionForPrefetch");
			//pauseGameWhenDisplayed = serializedObject.FindProperty("pauseGameWhenDisplayed");
			
			originalBackgroundColor = GUI.backgroundColor;
			
			delayRequest = requestDelay.floatValue >= DELAY_MIN;
			
			// settings
			settings = PlayHavenSettings.Get();
			RefreshPlacementList();
			RefreshRewardsList();
		}
		
		void RefreshPlacementList()
		{
			placementTagsList = settings.AllUniquePlacementTags;
			placementTagsList.Sort();
			for (int i=0; i<placementTagsList.Count; i++)
			{
				if (placementTagsList[i] == placement.stringValue)
				{
					selectedPlacement = i;
					break;
				}
			}
		}
	
		void RefreshRewardsList()
		{
			rewardTagsList = settings.AllUniqueRewardTags;
			rewardTagsList.Sort();
			for (int i=0; i<rewardTagsList.Count; i++)
			{
				if (rewardTagsList[i] == testRewardKey)
				{
					selectedReward = i;
					break;
				}
			}
		}
		
		public override void CustomInspectorGUI ()
		{
			guiDisabledOrig = GUI.enabled;
			
			//DrawDefaultInspector();
			requester.Update();
			
			// placement
			if (placement.stringValue.Length == 0)
				GUI.backgroundColor = Color.red;
			if (placement.stringValue.Length > 0 && !placementTagsList.Contains(placement.stringValue))
			{
				EditorGUILayout.PropertyField(placement, new GUIContent("Placement"));
			}
			else
			{
				selectedPlacement = EditorGUILayout.Popup("Placement", selectedPlacement, placementTagsList.ToArray());
				placement.stringValue = (selectedPlacement >= 0) ? placementTagsList[selectedPlacement] : string.Empty;
			}
			GUI.backgroundColor = originalBackgroundColor;
			EditorGUILayout.BeginHorizontal();
			if (placement.stringValue.Length > 0 && placementTagsList.Contains(placement.stringValue))
			{
				if (GUILayout.Button("Edit..."))
				{
					Placement placementObj = null;
					#if UNITY_ANDROID
					placementObj = settings.GetPlacementByTag(placement.stringValue, Game.OperatingSystem.android);
					#else
					placementObj = settings.GetPlacementByTag(placement.stringValue, Game.OperatingSystem.ios);
					#endif
					if (placementObj != null)
					{
						PlayHavenWindow.CreateWindow(PlayHavenWindow.TOOLBAR_PLACEMENTS);
						PlayHavenWindow w = PlayHavenWindow.Get();
						w.SetPlacementToEdit(placementObj);
					}
				}
			}
			if (settings.HasGameAndroid || settings.HasGameIOS)
			{
				if (GUILayout.Button("New..."))
				{
					PlayHavenWindow.CreateWindow(PlayHavenWindow.TOOLBAR_PLACEMENTS);
				}
			}
			if (placement.stringValue.Length > 0 && placementTagsList.Contains(placement.stringValue))
			{
				if (GUILayout.Button("Refresh"))
				{
					RefreshPlacementList();
				}
			}
			EditorGUILayout.EndHorizontal();
			Comment("Placements define the locations in your game where content may appear to your players.");
			
			// whenToRequest
			EditorGUILayout.PropertyField(whenToRequest);
			if (whenToRequest.enumValueIndex != 3 ) // !OnDisable
			{
				delayRequest = EditorGUILayout.Toggle("Delay Request", delayRequest);
				GUI.enabled = guiDisabledOrig && delayRequest;
				GUILayout.BeginHorizontal();
				requestDelay.floatValue = EditorGUILayout.Slider("Seconds", requestDelay.floatValue, DELAY_MIN, DELAY_MAX);
				GUI.enabled = guiDisabledOrig;
				GUILayout.EndHorizontal();
				if (!delayRequest)
					requestDelay.floatValue = 0f;
			}
			Comment("Specify when to make the request. If set to manual, you will need to call the Request() method on this component yourself in your code.");
			
			// prefetching
			EditorGUILayout.PropertyField(prefetch);
			EditorGUILayout.PropertyField(connectionForPrefetch);
			EditorGUILayout.PropertyField(refetchWhenUsed);
			Comment("Automatic pre-fetching characteristics.");
			
			// pauseGameWhenDisplayed
			/*
			EditorGUILayout.PropertyField(pauseGameWhenDisplayed, new GUIContent("Pause When Displayed"));
			Comment("If set, the timescale will be set to 0 when the content unit is displayed and returned to the previous timescale when dismissed."); 		
			*/
			
			// showsOverlayImmediately
			EditorGUILayout.PropertyField(showsOverlayImmediately, new GUIContent("Loading Overlay"));
			Comment("If \"Loading Overlay\" is set, an overlay will be shown so that the user cannot interact with the game while the content is being fetched."); 
			
			EditorGUILayout.PropertyField(rewardMayBeDelivered, new GUIContent("Rewardable"));		
			{
				// rewardMessageType
				if (rewardMayBeDelivered.boolValue)
				{
					//EditorGUI.indentLevel = 1;
					EditorGUILayout.PropertyField(rewardMessageType, new GUIContent("Message Type"));
					
					// default test reward
					EditorGUILayout.PropertyField(useDefaultTestReward, new GUIContent("Test Def. Reward"));
					if (useDefaultTestReward.boolValue)
					{
						EditorGUI.indentLevel = 0;
						GUILayout.BeginHorizontal();
						GUILayout.Space(6);
						GUILayout.Label("Test Default Reward");
						GUILayout.EndHorizontal();
						EditorGUI.indentLevel = 1;
						EditorGUILayout.PropertyField(defaultTestRewardName, new GUIContent("Name"));
						EditorGUILayout.PropertyField(defaultTestRewardQuantity, new GUIContent("Quantity"));
						EditorGUI.indentLevel = 0;
						GUI.enabled = guiDisabledOrig;
						//GUILayout.Space(8);
					}
					
					// manual test capability
					if (Application.isPlaying)
					{
						EditorGUI.indentLevel = 0;
						GUILayout.BeginHorizontal();
						GUILayout.Space(6);
						GUILayout.Label("Test Reward Now");
						GUILayout.EndHorizontal();
						EditorGUI.indentLevel = 1;
						if (testRewardKey.Length > 0 && !rewardTagsList.Contains(testRewardKey))
						{
							testRewardKey = EditorGUILayout.TextField("Name", testRewardKey);
						}
						else
						{
							selectedReward = EditorGUILayout.Popup("Name", selectedReward, rewardTagsList.ToArray());
							testRewardKey = (selectedReward > -1) ? rewardTagsList[selectedReward] : string.Empty;
						}
						testRewardQuantity = EditorGUILayout.IntField("Quantity", testRewardQuantity);
						EditorGUI.indentLevel = 0;
						GUI.enabled = guiDisabledOrig && testRewardKey.Length > 0;
						if (GUILayout.Button("Give Reward"))
						{
							PlayHaven.Reward reward = new PlayHaven.Reward();
							reward.name = testRewardKey;
							reward.quantity = testRewardQuantity;						
							((PlayHavenContentRequester)target).HandlePlayHavenManagerOnRewardGiven(0, reward);
						}
						GUI.enabled = guiDisabledOrig;
						//GUILayout.Space(8);
					}
					
					EditorGUI.indentLevel = 0;
				}
			}
			Comment("If \"Rewardable\" is set, this game object is automatically wired to receive reward notifications when this placement is reported. If a reward is delivered, a message OnPlayHavenRewardGiven(PlayHaven.Reward) will be broadcast to this game object."); 
			
			// usage
			EditorGUILayout.PropertyField(limitedUse, new GUIContent("Limited Use"));
			GUI.enabled = guiDisabledOrig && limitedUse.boolValue;
			EditorGUILayout.PropertyField(maxUses, new GUIContent("Max Uses"));
			EditorGUILayout.PropertyField(exhaustAction, new GUIContent("Exhaust Action"));
			GUI.enabled = guiDisabledOrig;
			if (maxUses.intValue < 1)
				maxUses.intValue = 1;
			Comment("You can limit how many times a content requester can be used with the above settings.");
			
			requester.ApplyModifiedProperties();
			
			if (Application.isPlaying)
			{
				PlayHavenContentRequester playHavenContentRequester = (PlayHavenContentRequester)target;
				if (playHavenContentRequester != null)
				{
					GUILayout.Space(8);
					if (GUILayout.Button("Test this Request"))
						playHavenContentRequester.Request();
				}
			}
		}	
	}
}