#pragma warning disable 0162 // code unreached.
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0618 // obslolete
#pragma warning disable 0108 
#pragma warning disable 0649 //never used
#pragma warning disable 0429 //never used

/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System;
// from the excellent http://answers.unity3d.com/questions/45186/can-i-auto-run-a-script-when-editor-launches-or-a.html

///
/// This must be added to "Editor" folder: http://unity3d.com/support/documentation/ScriptReference/index.Script_compilation_28Advanced29.html
/// Execute some code exactly once, whenever the project is opened, recompiled, or run.
///

namespace AppAdvisory.social
{
	[InitializeOnLoad]
	public class Autorun
	{

		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		private const bool DOSCIRPTINGSYMBOL = false;
		/******* TO MODIFY **********/
		private const string APPADVISORY_LEADERBOARD = "APPADVISORY_LEADERBOARD";
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/

		static void SetScriptingDefineSymbols () 
		{
			SetSymbolsForTarget (BuildTargetGroup.Android, APPADVISORY_LEADERBOARD);
			SetSymbolsForTarget (BuildTargetGroup.iOS, APPADVISORY_LEADERBOARD); 
//			SetSymbolsForTarget (BuildTargetGroup.WSA, APPADVISORY_LEADERBOARD);
//			#if !UNITY_5_5_OR_NEWER
//			#if !UNITY5_0 && !UNITY_5_1
//			SetSymbolsForTarget (BuildTargetGroup.Nintendo3DS, APPADVISORY_LEADERBOARD);
//			#endif
//			SetSymbolsForTarget (BuildTargetGroup.PS3, APPADVISORY_LEADERBOARD);
//			SetSymbolsForTarget (BuildTargetGroup.XBOX360, APPADVISORY_LEADERBOARD);
//			#endif
//			SetSymbolsForTarget (BuildTargetGroup.PS4, APPADVISORY_LEADERBOARD);
//			SetSymbolsForTarget (BuildTargetGroup.PSM, APPADVISORY_LEADERBOARD);
//			SetSymbolsForTarget (BuildTargetGroup.PSP2, APPADVISORY_LEADERBOARD);
//			SetSymbolsForTarget (BuildTargetGroup.SamsungTV, APPADVISORY_LEADERBOARD); 
//			SetSymbolsForTarget (BuildTargetGroup.Standalone, APPADVISORY_LEADERBOARD);
//			SetSymbolsForTarget (BuildTargetGroup.Tizen, APPADVISORY_LEADERBOARD);
//			#if !UNITY5_0 && !UNITY_5_1
//			SetSymbolsForTarget (BuildTargetGroup.tvOS, APPADVISORY_LEADERBOARD);
//			SetSymbolsForTarget (BuildTargetGroup.WiiU, APPADVISORY_LEADERBOARD); 
//			#endif
//			SetSymbolsForTarget (BuildTargetGroup.WebGL, APPADVISORY_LEADERBOARD);
//			SetSymbolsForTarget (BuildTargetGroup.XboxOne, APPADVISORY_LEADERBOARD);
		}


		static void SetSymbolsForTarget(BuildTargetGroup target, string scriptingSymbol)
		{

			if(target == BuildTargetGroup.Unknown)
				return;

			var s = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

			string sTemp = scriptingSymbol;

			if(!s.Contains(sTemp)) 
			{

				s = s.Replace(scriptingSymbol + ";","");

				s = s.Replace(scriptingSymbol,"");  

				s = scriptingSymbol + ";" + s;

				PlayerSettings.SetScriptingDefineSymbolsForGroup(target,s);
			}
		}


		static Autorun()
		{
			EditorApplication.update += RunOnce;
		}

		static void RunOnce() 
		{
			EditorApplication.update -= RunOnce;

			// do something here. You could open an EditorWindow, for example.

			if (DGChecker.needDotween == true && (!Directory.Exists ("Assets/Demigiant") || Directory.Exists ("Assets/DOTween")))
			{
				DGChecker.OpenPopupDGCHECKERStartup();

				return;
			}


			if(DOSCIRPTINGSYMBOL)
				SetScriptingDefineSymbols ();

			int count = EditorPrefs.GetInt(WelcomeVerySimpleLeaderboard.PREFSHOWATSTARTUP + "autoshow",0);

			if(count == 10 || count == 30 || count == 50 || count == 80 || count == 100)
			{
				Application.OpenURL("http://u3d.as/oWD");
			}

			EditorPrefs.SetInt(WelcomeVerySimpleLeaderboard.PREFSHOWATSTARTUP + "autoshow", count + 1);

			WelcomeVerySimpleLeaderboard.showAtStartup = EditorPrefs.GetInt(WelcomeVerySimpleLeaderboard.PREFSHOWATSTARTUP, 1) == 1;
			 
			if (WelcomeVerySimpleLeaderboard.showAtStartup)
			{
				DGChecker.CheckItNow();
			
				WelcomeVerySimpleLeaderboard.OpenPopupStartup();
			}
		}
	}
}         