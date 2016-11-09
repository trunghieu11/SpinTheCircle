
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using System.Collections;

namespace AppAdvisory.UI
{
	/// <summary>
	/// Attached to rate button
	/// </summary>
	public class ButtonRate : MonoBehaviour 
	{
		public string iosRateURL = "fb://profile/515431001924232" ;
		public string androidRateURL = "https://www.facebook.com/appadvisory";
		public string amazonRateURL = "https://www.facebook.com/appadvisory";

		public bool isAmazon = false;

		public void OnClickedRate()
		{
			string URL = "";

			#if UNITY_IOS
			URL = iosRateURL;
			#else
			URL = androidRateURL;
			if(isAmazon)
				URL = amazonRateURL;
			#endif

			Application.OpenURL(URL);


		}
	}
}