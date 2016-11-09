
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using System.Collections;


public class ButtonOpenUrl : MonoBehaviour 
{
	public string URL = "http://app-advisory.com";

	void Start()
	{
		GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClickedOpenURL);
	}

	public void OnClickedOpenURL()
	{
		//		Application.OpenURL(URL);
		Application.ExternalEval("window.open('" + URL + "')");
	}
}
