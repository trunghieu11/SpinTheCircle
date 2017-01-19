using UnityEngine;
//using UnityEditor;
//using UnityEditorInternal;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace AppAdvisory.social
{
	public class OpenMoreGames : MonoBehaviour 
	{
		public void OnClicked()
		{
			Application.OpenURL("http://u3d.as/9cs");
		}
	}
}