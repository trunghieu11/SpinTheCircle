
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using DG.Tweening;

namespace AppAdvisory.UI
{
	public class UIController : MonoBehaviour 
	{
		#region UI elements
		public RectTransform[] toAnimateFromTop;
		public RectTransform[] toAnimateHorizontaly;
		public Text textLast;
		public Text textBest;
		public void SetLastText(int point)
		{
			textLast.text  = "Last\n " + point;
		}
		public void SetBestText(int point)
		{
			textBest.text  = "Best\n " + point;
		}
		public HorizontalLayoutGroup horizontalLayoutGroup;
		public GridLayoutGroup gridLayoutGroup;
		public LayoutElement layoutElement;

		void Awake()
		{
			horizontalLayoutGroup.enabled = false;
			gridLayoutGroup.enabled = false;
			layoutElement.enabled = false;
		}
		#endregion

		#region Unity Events
		[System.Serializable] public class OnUIAnimInStartHandler : UnityEvent{}
		[SerializeField] public OnUIAnimInStartHandler OnUIAnimInStart;

		[System.Serializable] public class OnUIAnimInEndHandler : UnityEvent{}
		[SerializeField] public OnUIAnimInEndHandler OnUIAnimInEnd;

		[System.Serializable] public class OnUIAnimOUTStartHandler : UnityEvent{}
		[SerializeField] public OnUIAnimOUTStartHandler OnUIAnimOutStart;

		[System.Serializable] public class OnUIAnimOUTEndHandler : UnityEvent{}
		[SerializeField] public OnUIAnimOUTEndHandler OnUIAnimOutEnd;
		#endregion

	
		#region Anim IN
		public void DOAnimIN () 
		{
			OnUIAnimInStart.Invoke();

			bool animFromTopFinished = false;
			bool animHorizontallyFinished = false;
			AnimateINFromTop(() => {
				animFromTopFinished = true;

				if(animFromTopFinished && animHorizontallyFinished)
				{
					animFromTopFinished = false;
					animHorizontallyFinished = false;
					OnUIAnimInEnd.Invoke();
				}
			});
			AnimateINHorizontaly(() => {
				animHorizontallyFinished = true;

				if(animFromTopFinished && animHorizontallyFinished)
				{
					animFromTopFinished = false;
					animHorizontallyFinished = false;
					OnUIAnimInEnd.Invoke();
				}
			});
		}

		void AnimateINFromTop(Action callback)
		{
			int countCompleted = 0;

			if(toAnimateFromTop != null && toAnimateFromTop.Length != 0)
			{
				for(int i = 0; i < toAnimateFromTop.Length; i++)
				{
					var r = toAnimateFromTop[i];

					var p = r.localPosition;

					p.y = Screen.height * 2;
					r.localPosition = p;

					r.DOLocalMoveY(0, 0.5f).SetDelay(0.5f + i * 0.1f)
						.OnComplete(() => {
							countCompleted++;
							if(countCompleted >= toAnimateFromTop.Length)
							{
								if(callback!=null)
									callback();
							}
						});
				}
			}
		}

		void AnimateINHorizontaly(Action callback)
		{
			int countCompleted = 0;

			if(toAnimateHorizontaly != null && toAnimateHorizontaly.Length != 0)
			{
				for(int i = 0; i < toAnimateHorizontaly.Length; i++)
				{
					var r = toAnimateHorizontaly[i];

					if(i%2==0)
					{
						r.localPosition = new Vector2(-Screen.width * 2f, r.localPosition.y);
					}
					else
					{
						r.localPosition = new Vector2(+Screen.width * 2f, r.localPosition.y);
					}

					r.DOLocalMoveX(0, 0.5f).SetDelay(0.5f + i * 0.1f)
						.OnComplete(() => {
							countCompleted++;
							if(countCompleted >= toAnimateHorizontaly.Length)
							{
								if(callback!=null)
									callback();
							}
						});

				}
			}
		}
		#endregion

		#region Anim OUT

		public void DOAnimOUT () 
		{
			OnUIAnimOutStart.Invoke();

			bool animFromTopFinished = false;
			bool animHorizontallyFinished = false;
			AnimateOUTFromTop(() => {
				animFromTopFinished = true;

				if(animFromTopFinished && animHorizontallyFinished)
				{
					animFromTopFinished = false;
					animHorizontallyFinished = false;
					OnUIAnimOutEnd.Invoke();
				}
			});
			AnimateOUTHorizontaly(() => {
				animHorizontallyFinished = true;

				if(animFromTopFinished && animHorizontallyFinished)
				{
					animFromTopFinished = false;
					animHorizontallyFinished = false;
					OnUIAnimOutEnd.Invoke();
				}
			});
		}

		void AnimateOUTFromTop(Action callback)
		{
			int countCompleted = 0;

			if(toAnimateFromTop != null && toAnimateFromTop.Length != 0)
			{
				for(int i = 0; i < toAnimateFromTop.Length; i++)
				{
					var r = toAnimateFromTop[i];

					r.DOLocalMoveY(Screen.height * 2f, 0.5f).SetDelay(0.1f + i * 0.03f)
						.OnComplete(() => {
							countCompleted++;
							if(countCompleted >= toAnimateFromTop.Length)
							{
								if(callback!=null)
									callback();
							}
						});
				}
			}
		}

		void AnimateOUTHorizontaly(Action callback)
		{
			int countCompleted = 0;

			if(toAnimateHorizontaly != null && toAnimateHorizontaly.Length != 0)
			{
				for(int i = 0; i < toAnimateHorizontaly.Length; i++)
				{
					var r = toAnimateHorizontaly[i];

					int sign = 1;

					if(i%2==0)
					{
						sign = -1;
					}

					r.DOLocalMoveX(sign * Screen.width * 2f, 0.5f).SetDelay(0.1f + i * 0.03f)
						.OnComplete(() => {
							countCompleted++;
							if(countCompleted >= toAnimateHorizontaly.Length)
							{
								if(callback!=null)
									callback();
							}
						});

				}
			}
		}
		#endregion
	}
}