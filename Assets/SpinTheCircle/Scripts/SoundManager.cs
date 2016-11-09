
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using System.Collections;

namespace AppAdvisory.SpinTheCircle
{
	/// <summary>
	/// Class in charge to play FX in the game. Attached to the Canvas game object. Change the audioSource to customize the sounds.
	/// </summary>
	public class SoundManager : MonoBehaviour 
	{
		/// <summary>
		/// Reference to the audiosouce use to play fx, attached to the same game object
		/// </summary>
		AudioSource audioSource;
		/// <summary>
		/// Sound played when the level is clear = success
		/// </summary>
		[SerializeField] private AudioClip soundSuccess;
		/// <summary>
		/// Sound played when game over
		/// </summary>
		[SerializeField] private AudioClip soundFail;
		/// <summary>
		/// Sound played when the player tap at the good moment on the screen
		/// </summary>
		[SerializeField] private AudioClip soundTouch;
		/// <summary>
		/// Find the audiosource attached to the same game object
		/// </summary>
		void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}
		/// <summary>
		/// Method called when the level is clear = success
		/// </summary>
		public void PlaySuccess()
		{
			audioSource.PlayOneShot (soundSuccess,1f);
		}
		/// <summary>
		/// Method called when game over
		/// </summary>
		public void PlayFail()
		{
			audioSource.PlayOneShot (soundFail,1f);
		}
		/// <summary>
		/// Method called when the player tap at the good moment on the screen
		/// </summary>
		public void PlayTouch()
		{
			audioSource.PlayOneShot (soundTouch,1f);
		}
	}
}