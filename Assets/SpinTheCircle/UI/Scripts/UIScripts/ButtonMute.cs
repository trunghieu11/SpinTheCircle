using UnityEngine;
using System.Collections;
using AppAdvisory.SpinTheCircle;

namespace AppAdvisory.UI {

    /// <summary>
    /// Attached to mute button
    /// </summary>
    public class ButtonMute : MonoBehaviour {

        public GameObject audioOnItem;
        public GameObject audioOffItem;

        /// <summary>
        /// Toggle button mute
        /// </summary>
        public void ToggleSound() {
            if (PlayerPrefsX.GetBool(Util.MUTED_PREF)) {
                PlayerPrefsX.SetBool(Util.MUTED_PREF, false);
            } else {
                PlayerPrefsX.SetBool(Util.MUTED_PREF, true);
            }

            SetSoundState();
        }

        /// <summary>
        /// Set sound state
        /// </summary>
        public void SetSoundState() {
            if (!PlayerPrefsX.GetBool(Util.MUTED_PREF)) {
                AudioListener.volume = 1;
                audioOnItem.SetActive(true);
                audioOffItem.SetActive(false);
            } else {
                AudioListener.volume = 0;
                audioOnItem.SetActive(false);
                audioOffItem.SetActive(true);
            }
        }
    }
}