﻿using UnityEngine;
using System.Collections;
using AppAdvisory.SpinTheCircle;

namespace AppAdvisory.UI {

    /// <summary>
	/// Attached to exit button
	/// </summary>
    public class ButtonExit : MonoBehaviour {

        public void OnClickedExitButton() {
            PlayerPrefsX.SetBool(Util.FIRST_PLAY_PREF, true);
            PlayerPrefs.SetInt(Util.TOTAL_DIAMOND_PREF, FindObjectOfType<GameLogic>().totalDiamond);
            Application.Quit();
        }
    }
}