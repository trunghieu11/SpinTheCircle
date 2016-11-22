using UnityEngine;
using System.Collections;

namespace AppAdvisory.UI {
    public class ButtonShare : MonoBehaviour {

        /// <summary>
        /// Share message when click button share
        /// </summary>
        public void OnClickedShareButton() {
#if UNITY_ANDROID
            // Create Refernece of AndroidJavaClass class for intent
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            // Create Refernece of AndroidJavaObject class intent
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            // Set action for intent
            intentObject.Call("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            //Set Subject of action
            intentObject.Call("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Text Sharing ");
            //Set title of action or intent
            intentObject.Call("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Text Sharing ");
            // Set actual data which you want to share
            intentObject.Call("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Text Sharing Android Demo");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            // Invoke android activity for passing intent to share data
            currentActivity.Call("startActivity", intentObject);
#endif
        }
    }
}
