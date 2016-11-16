using UnityEngine;
using System.Collections;

namespace AppAdvisory.UI {

    /// <summary>
	/// Attached to exit button
	/// </summary>
    public class ButtonExit : MonoBehaviour {

        public void OnClickedExitButton() {
            Application.Quit();
        }
    }
}