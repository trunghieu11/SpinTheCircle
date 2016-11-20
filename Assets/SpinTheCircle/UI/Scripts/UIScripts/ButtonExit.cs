using UnityEngine;
using System.Collections;
using AppAdvisory.SpinTheCircle;

namespace AppAdvisory.UI {

    /// <summary>
	/// Attached to exit button
	/// </summary>
    public class ButtonExit : MonoBehaviour {

        public void OnClickedExitButton() {
            PlayerPrefsX.SetBool("_FirstPlay", true);
            Application.Quit();
        }
    }
}