using UnityEngine;
using System;
using System.Runtime.InteropServices;

#if UNITY_ANDROID
public class RevMobAndroidBanner : RevMobBanner {
    //private AndroidJavaObject javaObject;

    public RevMobAndroidBanner(AndroidJavaObject javaObject) {
        //this.javaObject = javaObject;
    }

    public override void Show() {
    	//javaObject.Call("show");
    }

    public override void Hide() {
    	//javaObject.Call("hide");
    }

}
#endif