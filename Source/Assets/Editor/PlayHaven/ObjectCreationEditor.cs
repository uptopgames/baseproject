using UnityEngine;
using UnityEditor;
using System.Collections;
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
	public class ObjectCreationEditor : Editor 
	{
	    [MenuItem("GameObject/Create Other/PlayHaven/PlayHaven Manager", false, 12901)]
	    static void DoCreatePlayHavenManagerObject()
	    {
			if (ExistingComponentTypeExists(typeof(PlayHavenManager)))
			{
				EditorUtility.DisplayDialog("Sorry!", "A PlayHavenManager component is already located in this scene.", "OK");
			}
			else
			{
				GameObject go = ObjectCreationEditor.CreateGameObjectInScene("PlayHavenManager");
				go.AddComponent<PlayHavenManager>();
				go.transform.parent = null;
				
				Selection.activeGameObject = go;
			}
		}
	
	    [MenuItem("GameObject/Create Other/PlayHaven/Content Requester", false, 12902)]
	    static void DoCreatePlayHavenContentRequesterObject()
	    {
			GameObject go = ObjectCreationEditor.CreateGameObjectInScene("PlayHavenContentRequester");
			go.AddComponent<PlayHavenContentRequester>();
			
			Selection.activeGameObject = go;
		}
		
		public static bool ExistingComponentTypeExists(System.Type type)
		{
			Component c = FindObjectOfType(type) as Component;
			return c != null;
		}
		
		public static GameObject CreateGameObjectInScene(string name)
		{
			string realName = name;
			int counter = 0;
			while (GameObject.Find(realName) != null)
			{
				realName = name + counter++;
			}
			
	        GameObject go = new GameObject(realName);
			if (Selection.activeGameObject != null)
			{
				string assetPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
				if (assetPath.Length == 0) go.transform.parent = Selection.activeGameObject.transform;
			}
	        go.transform.localPosition = Vector3.zero;
	        go.transform.localRotation = Quaternion.identity;
	        go.transform.localScale = Vector3.one;	
	        return go;
		}
	}
}