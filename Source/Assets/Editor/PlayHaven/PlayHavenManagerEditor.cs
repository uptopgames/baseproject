using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
	[CustomEditor(typeof(PlayHavenManager))]
	public class PlayHavenManagerEditor : PlayHavenInspector
	{
		private static string ANDROID_JAR_PATH = "/Plugins/Android/libs/playhaven.jar";
		
		private bool guiDisabledOrig = false;
		
		private SerializedProperty token;
		private SerializedProperty lockToken;
		private SerializedProperty secret;
		private SerializedProperty lockSecret;
		private SerializedProperty doNotDestroyOnLoad;
		private SerializedProperty whenToSendOpen;
		private SerializedProperty whenToGetNotifications;
		private SerializedProperty badgeMoreGamesPlacement;
		private SerializedProperty notificationPollDelay;
		private SerializedProperty notificationPollRate;
		private SerializedProperty defaultShowsOverlayImmediately;
		private SerializedProperty maskShowsOverlayImmediately;
		private SerializedProperty isAndroidSupported;
		private SerializedProperty cancelAllOnLevelLoad;
		private SerializedProperty suppressContentRequestsForLaunches;
		private SerializedProperty suppressedPlacements;
		private SerializedProperty suppressionExceptions;
		private SerializedProperty showContentUnitsInEditor;
		
		private Color originalBackgroundColor;
		private PlayHavenSettings settings;
		private bool secretsAreFrozen = false;
		private bool performLaunchSupression;
		private List<string> placementTagsList = null;
		
		protected override void OnEnable()
		{
			base.OnEnable();
			PublisherUI.OnGameSelectionChanged -= OnGameSelectionChanged;
			PublisherUI.OnGameSelectionChanged += OnGameSelectionChanged;
			PublisherUI.OnGameSelectionCleared -= OnGameSelectionCleared;
			PublisherUI.OnGameSelectionCleared += OnGameSelectionCleared;
			
			originalBackgroundColor = GUI.backgroundColor;
			
			#if UNITY_ANDROID
			token = serializedObject.FindProperty("tokenAndroid");
			lockToken = serializedObject.FindProperty("lockTokenAndroid");
			secret = serializedObject.FindProperty("secretAndroid");
			lockSecret = serializedObject.FindProperty("lockSecretAndroid");
			#else
			token = serializedObject.FindProperty("token");
			lockToken = serializedObject.FindProperty("lockToken");
			secret = serializedObject.FindProperty("secret");
			lockSecret = serializedObject.FindProperty("lockSecret");
			#endif
			
			doNotDestroyOnLoad = serializedObject.FindProperty("doNotDestroyOnLoad");
			defaultShowsOverlayImmediately = serializedObject.FindProperty("defaultShowsOverlayImmediately");
			maskShowsOverlayImmediately = serializedObject.FindProperty("maskShowsOverlayImmediately");
			
			whenToSendOpen = serializedObject.FindProperty("whenToSendOpen");
			
			whenToGetNotifications = serializedObject.FindProperty("whenToGetNotifications");
			badgeMoreGamesPlacement = serializedObject.FindProperty("badgeMoreGamesPlacement");
			notificationPollDelay = serializedObject.FindProperty("notificationPollDelay");
			notificationPollRate = serializedObject.FindProperty("notificationPollRate");
			cancelAllOnLevelLoad = serializedObject.FindProperty("cancelAllOnLevelLoad");
			suppressContentRequestsForLaunches = serializedObject.FindProperty("suppressContentRequestsForLaunches");
			performLaunchSupression = suppressContentRequestsForLaunches.intValue > 0;
			suppressedPlacements = serializedObject.FindProperty("suppressedPlacements");
			suppressionExceptions = serializedObject.FindProperty("suppressionExceptions");
			
			isAndroidSupported = serializedObject.FindProperty("isAndroidSupported");
			showContentUnitsInEditor = serializedObject.FindProperty("showContentUnitsInEditor");
					
			AutofillTokens();
			RefreshPlacementList();
			
			// determine if the playhaven-x.x.x.jar is found in the assets; if it is,
			// set the PlayHavenManager.isAndroidSupported flag to true.
			isAndroidSupported.boolValue = File.Exists(Application.dataPath + ANDROID_JAR_PATH);
			serializedObject.ApplyModifiedProperties();
		}
		
		public override void CustomInspectorGUI()
		{
			guiDisabledOrig = GUI.enabled;
			
			//DrawDefaultInspector();
			serializedObject.Update();
			
			#if UNITY_ANDROID
			Comment("Android");
			#else
			Comment("iOS");
			#endif
			
			// token
			GUILayout.BeginHorizontal();
			GUI.enabled = guiDisabledOrig && !lockToken.boolValue && !secretsAreFrozen;
			if (token.stringValue.Length == 0)
				GUI.backgroundColor = Color.red;
			EditorGUILayout.PropertyField(token, new GUIContent("Token"));
			GUI.backgroundColor = originalBackgroundColor;
			GUI.enabled = guiDisabledOrig;
			if (!secretsAreFrozen)
			{
				bool tokenLockPressed = LockControl(lockToken.boolValue);
				if (tokenLockPressed)
					lockToken.boolValue = !lockToken.boolValue;
				GUILayout.Space(8);
			}
			GUILayout.EndHorizontal();
			
			// secret
			GUILayout.BeginHorizontal();
			GUI.enabled = guiDisabledOrig && !lockSecret.boolValue && !secretsAreFrozen;
			if (secret.stringValue.Length == 0)
				GUI.backgroundColor = Color.red;
			EditorGUILayout.PropertyField(secret, new GUIContent("Secret"));
			GUI.backgroundColor = originalBackgroundColor;
			GUI.enabled = guiDisabledOrig;
			if (!secretsAreFrozen)
			{
				bool secretLockPressed = LockControl(lockSecret.boolValue);
				if (secretLockPressed)
					lockSecret.boolValue = !lockSecret.boolValue;
				GUILayout.Space(8);
			}
			GUILayout.EndHorizontal();
			
			if (!secretsAreFrozen)
			{
				if (GUILayout.Button("Import or Create Game"))
				{
					PlayHavenWindow.CreateWindow();
				}
			}
			else
			{
				if (GUILayout.Button("Edit Game"))
				{
					PlayHavenWindow.CreateWindow();
				}
			}
			
			Comment("The token and secret are unique to your account and game. You can be automatically imported for you or obtained from the PlayHaven dashboard.");
			
			// doNotDestroyOnLoad
			EditorGUILayout.PropertyField(doNotDestroyOnLoad, new GUIContent("Keep"));
			Comment("If \"Keep\" is set, this game object will persist from scene to scene."); 
		
			// pauseUnity
			
			// overlay behavior
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Default Overlay Settings");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(defaultShowsOverlayImmediately, new GUIContent("Show By Default")); // defaultShowsOverlayImmediately		
			EditorGUILayout.PropertyField(maskShowsOverlayImmediately, new GUIContent("Mask"));	// maskShowsOverlayImmediately
			Comment("Defines the default behavior for the content request \"Loading Overlay.\" Setting \"Mask\" completely disables them.");
			
			// whenToSendOpen
			EditorGUILayout.PropertyField(whenToSendOpen, new GUIContent("Notify Open"));
			Comment("The \"Notify Open\" setting specifies when the manager will notify PlayHaven when your game has launched. If set to manual you will need to call OpenNotification() yourself in your code.");
	
			// showContentUnitsInEditor
			EditorGUILayout.PropertyField(showContentUnitsInEditor, new GUIContent("Content Units in Editor"));
			Comment("Simulated content units can be displayed in the editor to assist in testing the triggering of placements.");
			
			// notifications
			{
				// whenToGetNotifications
				EditorGUILayout.PropertyField(whenToGetNotifications, new GUIContent("Fetch Badges"));
				if (whenToGetNotifications.enumValueIndex != (int)PlayHavenManager.WhenToGetNotifications.Disabled)
				{
					// badgeMoreGamesPlacement
					EditorGUILayout.PropertyField(badgeMoreGamesPlacement, new GUIContent("More Games Placement"));
				}
				if (whenToGetNotifications.enumValueIndex == (int)PlayHavenManager.WhenToGetNotifications.Poll)
				{
					//GUILayout.Space(4);
					GUILayout.Label("Fetch Polling Attributes:");
					EditorGUI.indentLevel = 1;
					
					// notificationPollDelay
					GUILayout.BeginHorizontal();
					//GUILayout.Space(16);
					EditorGUILayout.PropertyField(notificationPollDelay, new GUIContent("Delay (seconds)"));
					if (notificationPollDelay.floatValue < 0)
						notificationPollDelay.floatValue = 0;
					GUILayout.EndHorizontal();
					
					// notificationPollRate
					GUILayout.BeginHorizontal();
					//GUILayout.Space(16);
					EditorGUILayout.PropertyField(notificationPollRate, new GUIContent("Rate (seconds)"));
					if (notificationPollRate.floatValue < 1)
						notificationPollRate.floatValue = 1;
					GUILayout.EndHorizontal();
					
					EditorGUI.indentLevel = 0;
				}
				Comment("Specify if and how often to fetch badge notifications. When a badge value is fetched, its value will be available from the Fetch property of the manager.");
			}
			
			// auto-cancelling of stale requests
			{
				EditorGUILayout.PropertyField(cancelAllOnLevelLoad, new GUIContent("Cancel On Load"));			
				Comment("This setting automatically cancels any pending requests when a new level is loaded.");
			}
			
			// automatic content request suppression
			{
				performLaunchSupression = EditorGUILayout.Toggle("Launch Suppression", performLaunchSupression);
				if (performLaunchSupression)
				{
					// count
					if (suppressContentRequestsForLaunches.intValue < 1)
						suppressContentRequestsForLaunches.intValue = 1;
					EditorGUILayout.IntSlider(suppressContentRequestsForLaunches, 1, 10, new GUIContent("Number of Launches"));
					
					// exceptions
					SerializedProperty suppressedPlacement;
					SerializedProperty suppressionException;
					int numSupressions = suppressedPlacements.arraySize;
					int numExceptions = suppressionExceptions.arraySize;
					GUILayout.Space(4);
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (numExceptions == 0 && GUILayout.Button("Add Suppressed Placement"))
					{
						suppressedPlacements.arraySize++;
						suppressedPlacement = serializedObject.FindProperty(System.String.Format("suppressedPlacements.Array.data[{0}]", suppressedPlacements.arraySize-1));
						suppressedPlacement.stringValue = string.Empty;
					}
					if (numSupressions == 0 && GUILayout.Button("Add Exempted Placement"))
					{
						suppressionExceptions.arraySize++;
						suppressionException = serializedObject.FindProperty(System.String.Format("suppressionExceptions.Array.data[{0}]", suppressionExceptions.arraySize-1));
						suppressionException.stringValue = string.Empty;
					}
					GUILayout.EndHorizontal();
					if (numSupressions > 0)
					{
						int suppressionToDelete = -1;
						GUILayout.Label("Placement to Suppress", EditorStyles.boldLabel);
						for (int i=0; i<numSupressions; i++)
						{
							suppressedPlacement = serializedObject.FindProperty(System.String.Format("suppressedPlacements.Array.data[{0}]", i));
							
							GUILayout.BeginHorizontal();						
							if (suppressedPlacement.stringValue.Length > 0 && !placementTagsList.Contains(suppressedPlacement.stringValue))
							{
								EditorGUILayout.PropertyField(suppressedPlacement, new GUIContent("Placement #"+(i+1)));
							}
							else
							{
								int selectedPlacement = -1;
								for (int j=0; j<placementTagsList.Count; j++)
								{
									if (placementTagsList[j] == suppressedPlacement.stringValue)
									{
										selectedPlacement = j;
										break;
									}
								}
								int newSelectedPlacement = EditorGUILayout.Popup("Placement #"+(i+1), selectedPlacement, placementTagsList.ToArray()); 
								if (newSelectedPlacement != selectedPlacement)
								{
									suppressedPlacement.stringValue = placementTagsList[newSelectedPlacement]; 
								}
							}
							if (GUILayout.Button("-", GUILayout.Width(18), GUILayout.Height(14)))
							{
								if (EditorUtility.DisplayDialog("Confirm Deletion", "Are you sure you want to remove this placement suppression?", "Yes", "No"))
								{
									suppressionToDelete = i;
								}
							}
							GUILayout.EndHorizontal();
						}
						if (suppressionToDelete >= 0)
						{
							for (int i=suppressionToDelete; i<numSupressions-1; i++)
								suppressedPlacements.MoveArrayElement(i+1, i);
							suppressedPlacements.arraySize--;
						}
					}
					if (numExceptions > 0)
					{
						int exemptionToDelete = -1;
						GUILayout.Label("Placement to Exempt", EditorStyles.boldLabel);
						for (int i=0; i<numExceptions; i++)
						{
							suppressionException = serializedObject.FindProperty(System.String.Format("suppressionExceptions.Array.data[{0}]", i));
							
							GUILayout.BeginHorizontal();						
							if (suppressionException.stringValue.Length > 0 && !placementTagsList.Contains(suppressionException.stringValue))
							{
								EditorGUILayout.PropertyField(suppressionException, new GUIContent("Placement #"+(i+1)));
							}
							else
							{
								int selectedPlacement = -1;
								for (int j=0; j<placementTagsList.Count; j++)
								{
									if (placementTagsList[j] == suppressionException.stringValue)
									{
										selectedPlacement = j;
										break;
									}
								}
								int newSelectedPlacement = EditorGUILayout.Popup("Placement #"+(i+1), selectedPlacement, placementTagsList.ToArray()); 
								if (newSelectedPlacement != selectedPlacement)
								{
									suppressionException.stringValue = placementTagsList[newSelectedPlacement]; 
								}
							}
							if (GUILayout.Button("-", GUILayout.Width(18), GUILayout.Height(14)))
							{
								if (EditorUtility.DisplayDialog("Confirm Deletion", "Are you sure you want to remove this placement suppression excemption?", "Yes", "No"))
								{
									exemptionToDelete = i;
								}
							}
							GUILayout.EndHorizontal();
						}
						if (exemptionToDelete >= 0)
						{
							for (int i=exemptionToDelete; i<numExceptions-1; i++)
								suppressionExceptions.MoveArrayElement(i+1, i);
							suppressionExceptions.arraySize--;
						}
					}
				}
				else
					suppressContentRequestsForLaunches.intValue = 0;
				Comment("Launch Suppression allows the automatic suppression of content requests until the game has been launched a programmable number of times.");
			}
			
			serializedObject.ApplyModifiedProperties();
		}	
		
		void AutofillTokens()
		{
			// auto-fill of token/secret if possible
			settings = PlayHavenSettings.Get();
			secretsAreFrozen = false;
			if (Selection.activeObject != null && Selection.activeObject is GameObject && ((GameObject)Selection.activeObject).GetComponent<PlayHavenManager>() != null)
			{			
				#if UNITY_ANDROID
				if (settings != null && settings.HasGameAndroid)
				{
					serializedObject.Update();
					token.stringValue = settings.thisGameAndroid.token;		
					secret.stringValue = settings.thisGameAndroid.secret;		
					serializedObject.ApplyModifiedProperties();
					secretsAreFrozen = true;
				}
				#elif UNITY_IPHONE
				if (settings != null && settings.HasGameIOS)
				{
					serializedObject.Update();
					token.stringValue = settings.thisGameIOS.token;		
					secret.stringValue = settings.thisGameIOS.secret;		
					serializedObject.ApplyModifiedProperties();
					secretsAreFrozen = true;
				}
				#endif
			}
			else
			{
				// locate the PlayHavenManager in the scene and update it this way instead
				PlayHavenManager phm = GameObject.FindObjectOfType(typeof(PlayHavenManager)) as PlayHavenManager;
				if (phm != null)
				{
					#if UNITY_ANDROID
					if (settings != null && settings.HasGameAndroid)
					{
						phm.tokenAndroid = settings.thisGameAndroid.token;
						phm.secretAndroid = settings.thisGameAndroid.secret;
						EditorUtility.SetDirty(phm);
						secretsAreFrozen = true;
					}
					#elif UNITY_IPHONE
					if (settings != null && settings.HasGameIOS)
					{
						phm.token = settings.thisGameIOS.token;
						phm.secret = settings.thisGameIOS.secret;
						EditorUtility.SetDirty(phm);
						secretsAreFrozen = true;
					}
					#endif
				}
				else
				{
					string warning = "Unable to locate PlayHavenManager in this scene. In order for the token and secret to properly update, please open the scene(s) that have one and select the game object with the PlayHavenManager and resave the scene.";
					if (EditorUtility.DisplayDialog("Cannot find PlayHavenManager", warning, "OK"))
					{}
					Debug.LogWarning(warning);
				}
			}
		}
	
		void RefreshPlacementList()
		{
			placementTagsList = settings.AllUniquePlacementTags;
			placementTagsList.Sort();
		}
		
		void OnGameSelectionChanged()
		{
			AutofillTokens();
			Repaint();
		}
		
		void OnGameSelectionCleared()
		{
			secretsAreFrozen = false;
			if (Selection.activeObject != null && Selection.activeObject is GameObject && ((GameObject)Selection.activeObject).GetComponent<PlayHavenManager>() != null)
			{			
				#if UNITY_ANDROID || UNITY_IPHONE
				serializedObject.Update();
				token.stringValue = string.Empty;
				lockToken.boolValue = false;
				secret.stringValue = string.Empty;
				lockSecret.boolValue = false;
				serializedObject.ApplyModifiedProperties();
				#endif
				Repaint();
			}
			else
			{
				settings = PlayHavenSettings.Get();
				if (settings != null)
				{
					// locate the PlayHavenManager in the scene and update it this way instead
					PlayHavenManager phm = GameObject.FindObjectOfType(typeof(PlayHavenManager)) as PlayHavenManager;
					if (phm != null)
					{
						#if UNITY_ANDROID
						if (!settings.HasGameAndroid)
						{
							phm.tokenAndroid = string.Empty;
							phm.secretAndroid = string.Empty;
						}
						#elif UNITY_IPHONE
						if (!settings.HasGameIOS)
						{
							phm.token = string.Empty;
							phm.secret = string.Empty;
						}
						#endif
						EditorUtility.SetDirty(phm);
					}
					else
					{
						string warning = "Unable to locate PlayHavenManager in this scene. In order for the token and secret to properly update, please open the scene(s) that have one and select the game object with the PlayHavenManager and resave the scene.";
						if (EditorUtility.DisplayDialog("Cannot find PlayHavenManager", warning, "OK"))
						{}
						Debug.LogWarning(warning);
					}
				}
			}
		}
	}
}