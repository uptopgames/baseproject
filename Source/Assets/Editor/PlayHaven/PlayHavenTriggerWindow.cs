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
	public class PlayHavenTriggerWindow : EditorWindow
	{
		public enum TriggerType { Box, Sphere, Capsule };
		public string placement = string.Empty;
		public TriggerType triggerType;
		public bool rewardable = true;
		public enum ScriptType { None, New, Existing };
		public ScriptType customScriptType = ScriptType.None;
		public string newScriptName = string.Empty;
		public MonoScript existingScript;
		
		private GameObject newGameObject;
		
		private SerializedObject windowObject;
		private SerializedProperty placementProperty;
		private SerializedProperty triggerTypeProperty;
		private SerializedProperty rewardableProperty;
		private SerializedProperty customScriptTypeProperty;
		private SerializedProperty newScriptNameProperty;
		private SerializedProperty existingScriptProperty;
	
		private PlayHavenSettings settings;
		private List<string> placementTagsList = null;
		private int selectedPlacement = -1;
		
		private static Texture2D playHavenLogo;
		private static GUISkin skin;
		private bool waitingToAddNewScript;
		
		[MenuItem ("GameObject/Create Other/PlayHaven/Content Trigger Wizard")]
	    static void CreateWindow()
		{
			if (playHavenLogo == null)
			{
				playHavenLogo = EditorGUIUtility.Load("PlayHaven/ph-logo.png") as Texture2D;
				if (playHavenLogo == null)
					Debug.LogError("unable to locate PlayHaven logo image");
			}
			if (skin == null)
			{
				skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
				
				// modify the Box
				if (PlayHavenEditor.UIUtil.IsProSkin)
					skin.box.normal.textColor = new Color(1, 1, 1, 0.75f);
				else
					skin.box.normal.textColor = new Color(0, 0, 0, 0.75f);
			}
	
			PlayHavenTriggerWindow window = EditorWindow.GetWindow(typeof(PlayHavenTriggerWindow), true, "Create PlayHaven Content Trigger") as PlayHavenTriggerWindow;
			window.position = new Rect(256, 256, 312, 412);
			window.Initialize();
		}
		
		void Initialize()
		{
			windowObject = new SerializedObject(this);
			placementProperty = windowObject.FindProperty("placement");
			triggerTypeProperty = windowObject.FindProperty("triggerType");
			rewardableProperty = windowObject.FindProperty("rewardable");
			customScriptTypeProperty = windowObject.FindProperty("customScriptType");
			newScriptNameProperty = windowObject.FindProperty("newScriptName");
			existingScriptProperty = windowObject.FindProperty("existingScript");
	
			// settings
			settings = PlayHavenSettings.Get();
			RefreshPlacementList();
		}
	
		void RefreshPlacementList()
		{
			placementTagsList = settings.AllUniquePlacementTags;
			placementTagsList.Sort();
			for (int i=0; i<placementTagsList.Count; i++)
			{
				if (placementTagsList[i] == placementProperty.stringValue)
				{
					selectedPlacement = i;
					break;
				}
			}
		}
		
		protected void Comment(string comment)
		{
			GUILayout.Box(new GUIContent(comment), skin.box, GUILayout.ExpandWidth(true)); 
			GUILayout.Space(4);
		}
		
		void Update()
		{
			if (!EditorApplication.isCompiling && customScriptType == ScriptType.New && waitingToAddNewScript && newGameObject != null && newScriptName.Length > 0)
			{
				newGameObject.AddComponent(newScriptName);
				waitingToAddNewScript = false;
				newGameObject = null;
				
				// open the new script
				Object newScript = AssetDatabase.LoadAssetAtPath("Assets/"+newScriptName+".cs", typeof(MonoScript));
				if (newScript != null)
				{
					int line = (rewardable) ? 21 : 13;
					AssetDatabase.OpenAsset(newScript, line);
				}
	
				Close();
			}
		}
		
		void OnGUI()
		{
			GUI.enabled = !EditorApplication.isCompiling;		
	
			windowObject.Update();
	
			Comment("Create a prefabricated PlayHaven content requester specifically designed to perform a content request that can handle a reward response.");
			
			// placement
			if (placementTagsList == null || placementTagsList.Count == 0 || (placementProperty.stringValue.Length > 0 && !placementTagsList.Contains(placementProperty.stringValue)))
			{
				EditorGUILayout.PropertyField(placementProperty, new GUIContent("Placement"));
			}
			else
			{
				selectedPlacement = EditorGUILayout.Popup("Placement", selectedPlacement, placementTagsList.ToArray());
				placementProperty.stringValue = (selectedPlacement >= 0) ? placementTagsList[selectedPlacement] : string.Empty;
			}
			Comment("Placements define the locations in your game where content may appear to your players.");
			
			// triggerType
			EditorGUILayout.PropertyField(triggerTypeProperty, new GUIContent("Trigger Type"));	
			Comment("Specify the type of trigger collider shape to use.");
	
			// triggerType
			EditorGUILayout.PropertyField(rewardableProperty, new GUIContent("Rewardable"));	
			Comment("Specify if the placement can deliver rewards.");
			
			// customScriptType
			EditorGUILayout.PropertyField(customScriptTypeProperty, new GUIContent("Custom Script"));			
			{
				if (customScriptTypeProperty.enumValueIndex != 0)
				{
					EditorGUI.indentLevel = 1;
					switch (customScriptTypeProperty.enumValueIndex)
					{
					case 1:
						existingScript = null;
						EditorGUILayout.PropertyField(newScriptNameProperty, new GUIContent("New Script"));			
						break;
					case 2:
						newScriptName = string.Empty;
						EditorGUILayout.PropertyField(existingScriptProperty, new GUIContent("Existing Script"));			
						break;
					}
					EditorGUI.indentLevel = 0;
				}
			}
			Comment("Automatically create a new reward responder script or specify an existing one.");
			
			// apply changes to the properties
			windowObject.ApplyModifiedProperties();
			
			// sanitize
			placement = placement.Replace(" ", "").ToLower();
			
			newScriptName = newScriptName.Replace(" ", "_");
			newScriptName = newScriptName.Replace("-", "_");
			
			// create button
			GUI.enabled = !EditorApplication.isCompiling && 
				(
			    customScriptTypeProperty.enumValueIndex == 0
			 || (customScriptTypeProperty.enumValueIndex == 1 && newScriptName.Length > 0)
			 || (customScriptTypeProperty.enumValueIndex == 2 && existingScript != null)
				)
			 && !ScriptExists(string.Empty, newScriptName);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			bool created = false;
			if (created = GUILayout.Button("Create Trigger", GUILayout.Width(128)))
			{
				waitingToAddNewScript = true;
				CreateNewTrigger();
			}
			GUILayout.EndHorizontal();
			GUI.enabled = true;
	
			// draw the PlayHaven logo; centered and anchored to bottom of inspector GUI
			if (playHavenLogo)
			{
				//GUILayout.FlexibleSpace();
				GUILayout.Space(16);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(playHavenLogo);	// the logo
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			
			// draw the SDK version
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("v"+PluginVersion.Value);
			GUILayout.EndHorizontal();
			
			if (created && customScriptType != ScriptType.New)
				Close();
		}
		
		void CreateNewTrigger()
		{
			// create the new game object
			newGameObject = new GameObject("PlayHavenRewardTrigger");
			/*
	        newGameObject.transform.localPosition = Vector3.zero;
	        newGameObject.transform.localRotation = Quaternion.identity;
	        newGameObject.transform.localScale = Vector3.one;	
	        */		
			if (Selection.activeGameObject != null)
			{
				string assetPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
				if (assetPath.Length == 0) newGameObject.transform.parent = Selection.activeGameObject.transform;
			}
			
			// add a play haven content requester
			PlayHavenContentRequester contentRequester = newGameObject.AddComponent<PlayHavenContentRequester>();
			contentRequester.placement = placement;
			contentRequester.whenToRequest = PlayHavenContentRequester.WhenToRequest.Manual;
			contentRequester.rewardMayBeDelivered = rewardableProperty.boolValue;
			if (contentRequester.rewardMayBeDelivered)
				contentRequester.rewardMessageType = PlayHavenContentRequester.MessageType.Send;
			
			// add the collider and make it a trigger
			Collider collider = null;
			switch (triggerType)
			{
			case TriggerType.Box:
				collider = newGameObject.AddComponent<BoxCollider>();
				break;
			case TriggerType.Sphere:
				collider = newGameObject.AddComponent<SphereCollider>();
				break;
			case TriggerType.Capsule:
				collider = newGameObject.AddComponent<CapsuleCollider>();
				break;
			}
			if (collider != null)
				collider.isTrigger = true;
			
			// add existing script if specified
			if (existingScript != null)
			{
				newGameObject.AddComponent(existingScript.GetClass());
			}
			else
			{
				CreateTriggerBehaviourTemplate(rewardableProperty.boolValue, string.Empty, newScriptName);
				AssetDatabase.Refresh();
				//new Thread(new ThreadStart(MonitorCompile)).Start();
				//EditorApplication.update += EditorApplicationUpdate;
				//newGameObject.AddComponent(newScriptName);
			}
			
			// select the new game object
			Selection.activeGameObject = newGameObject;
		}
		
		/*
		void MonitorCompile()
		{
			if (EditorApplication.isCompiling)
				Thread.Sleep(100);
			newGameObject.AddComponent(newScriptName);
		}
		*/
		
		void EditorApplicationUpdate()
		{
			Debug.Log("EditorApplicationUpdate");
			if (!EditorApplication.isCompiling)
			{
				Debug.Log("adding: "+newScriptName);
				newGameObject.AddComponent(newScriptName);
				EditorApplication.update -= EditorApplicationUpdate;
			}
		}
		
		void CreateTriggerBehaviourTemplate(bool receivesMessage, string path, string className)
		{
			TextWriter cswriter = File.CreateText(Application.dataPath+"/"+path+className+".cs");
			cswriter.WriteLine("using UnityEngine;");
			cswriter.WriteLine("using System.Collections;");
			cswriter.WriteLine();
			
			cswriter.Write("public class ");
			cswriter.Write(className);
			cswriter.WriteLine(" : MonoBehaviour {");
			
			cswriter.WriteLine("\tprivate PlayHavenContentRequester contentRequester;");
			cswriter.WriteLine();
		
			cswriter.WriteLine("\tvoid Awake() {");
			cswriter.WriteLine("\t\tcontentRequester = GetComponent<PlayHavenContentRequester>();");
			cswriter.WriteLine("\t}");
			cswriter.WriteLine();
		
			cswriter.WriteLine("\tvoid OnTriggerEnter(Collider collider) {");
			cswriter.WriteLine("\t\t// alter the condition, if necessary, in which the trigger will be stimulated");
			cswriter.WriteLine("\t\tif (collider.transform.root.tag == \"Player\") // only the player, not other stuff");
			cswriter.WriteLine("\t\t{");
			cswriter.WriteLine("\t\t\tcontentRequester.Request();");
			cswriter.WriteLine("\t\t}");
			cswriter.WriteLine("\t}");
		
			if (receivesMessage)
			{
				cswriter.WriteLine();
				cswriter.WriteLine("\tvoid OnPlayHavenRewardGiven(PlayHaven.Reward reward) {");
				cswriter.WriteLine("\t\t// custom code goes here for dealing with the reward");
				cswriter.WriteLine();
				cswriter.WriteLine("\t}");
			}
			
			cswriter.WriteLine("}");
			cswriter.WriteLine();
			cswriter.Close();
		}
		
		bool ScriptExists(string path, string className)
		{
			Object newScript = AssetDatabase.LoadAssetAtPath("Assets/"+path+className+".cs", typeof(MonoScript));
			return newScript != null;
		}
	}
}