
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace AppAdvisory.SpinTheCircle
{
	/// <summary>
	/// Utility class. This class is static, so you can use it in all your projects!
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Compare two colors
		/// </summary>
		public static bool IsEqual(this Color c, Color o)
		{
			if(c.r != o.r)
				return false;

			if(c.g != o.g)
				return false;

			if(c.b != o.b)
				return false;

			return true;
		}

		private static System.Random rng = new System.Random();  
		/// <summary>
		/// Real shuffle of List
		/// </summary>
		public static void Shuffle<T>(this IList<T> list)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rng.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
		public static bool SetLastScore(int score)
		{
			PlayerPrefs.SetInt("LAST_SCORE",score);

			bool isBest = false;

			int best = GetBestScore();

			if(best < score)
			{
				isBest = true;
				PlayerPrefs.SetInt("BEST_SCORE", score);
			}


			PlayerPrefs.Save();

			return isBest;
		}

		public static int GetLastScore()
		{
			return PlayerPrefs.GetInt("LAST_SCORE", 0);
		}

		public static int GetBestScore()
		{
			return PlayerPrefs.GetInt("BEST_SCORE", 0);
		}

		/// <summary>
		/// Clean the memory and reload the scene
		/// </summary>
		public static void ReloadLevel()
		{
			CleanMemory();

			#if UNITY_5_3_OR_NEWER
			SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
			#else
			Application.LoadLevel(Application.loadedLevel);
			#endif

			CleanMemory();
		}
		/// <summary>
		/// Clean the memory
		/// </summary>
		public static void CleanMemory()
		{
			DOTween.KillAll();
			GC.Collect();
			Application.targetFrameRate = 60;
		}
		/// <summary>
		/// Resturn true if last time we play we lose (= Game Over)
		/// </summary>
		public static bool RestartFromGameOver()
		{
			return PlayerPrefs.GetInt("_RestartFromGameOver",0) == 1;
		}
	}
}