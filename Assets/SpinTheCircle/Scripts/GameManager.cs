﻿
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;
using GoogleMobileAds.Api;
#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif

#if APPADVISORY_ADS
using AppAdvisory.Ads;
#endif

#if APPADVISORY_LEADERBOARD
//using AppAdvisory.social;
#endif

using AppAdvisory.UI;

namespace AppAdvisory.SpinTheCircle {
    /// <summary>
    /// In charge of the game logic: Game Start, Game Over, Score, Ads etc... Attached to the Canvas game object. In Charge to all the game management (game over, point, restart etc..) and in charge to show interstitial in the game.
    /// FOr monetizing this game with ads, everythign is already coded for you. You just need to get VERY SIMPLE ADS here: http://u3d.as/oWD
    /// </summary>
    public class GameManager : MonoBehaviour {
        /// <summary>
        /// If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
        /// </summary>
        public string VerySimpleAdsURL = "http://u3d.as/oWD";
        
        /// <summary>
        /// Number of "play" to show an interstitial. If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
        /// </summary>
        public int numberOfPlayToShowInterstitial = 5;
        
        /// <summary>
        /// to reset the player pref. Use if for debug only!!
        /// </summary>
        public bool RESET_PLAYER_PREF = false;
        
        /// <summary>
        /// True if game over
        /// </summary>
        public bool isGameOver = false;
        
        /// <summary>
        /// Text in the center of the screen = number of colors to find to clear the level
        /// </summary>
        public Text levelCenterScreen;
        
        /// <summary>
        /// Reference to circle parent, to do the animation in and out for transition between level 
        /// </summary>
        SoundManager _soundManager;
        public SoundManager soundManager {
            get {
                if (_soundManager == null)
                    _soundManager = FindObjectOfType<SoundManager>();

                return _soundManager;
            }
        }
        private int m_point;
        
        /// <summary>
        /// The number of move we have to do to clear this level = the level number
        /// </summary>
        public int point {
            set {
                if (value > m_point) {
                    soundManager.PlayTouch();
                }

                m_point = value;
                levelCenterScreen.text = value.ToString();
            }
            get {
                return m_point;
            }
        }

        InterstitialAd fullAdmob;

        /// <summary>
        /// Clean the memory and place the circleparent at the good place
        /// </summary>
        void Awake() {
            Util.CleanMemory();
            
            if (!Util.RestartFromGameOver()) {
                float width = Util.getWidth();
                FindObjectOfType<GameLogic>().GetComponent<RectTransform>().anchoredPosition = new Vector3(5 * width, 0, 0);
                FindObjectOfType<GameLogic>().tutorialImage.rectTransform.anchoredPosition = new Vector3(5 * width, 0, 0);
                FindObjectOfType<GameLogic>().speedUpImage.rectTransform.anchoredPosition = new Vector3(5 * width, 0, 0);
                FindObjectOfType<GameLogic>().diamondImage.rectTransform.anchoredPosition = new Vector3(5 * width, 0, 0);
                FindObjectOfType<GameLogic>().totalDiamondText.rectTransform.anchoredPosition = new Vector3(5 * width, 0, 0);
            }
        }
        
        /// <summary>
        /// Clean the memory and place the circleparent at the good place
        /// </summary>
        void Start() {
            if (Application.isEditor) {
                if (RESET_PLAYER_PREF) {
                    PlayerPrefs.DeleteAll();
                }
            }

            RESET_PLAYER_PREF = false;

            if (!Util.RestartFromGameOver()) {
                float width = Util.getWidth();
                FindObjectOfType<GameLogic>().GetComponent<RectTransform>().anchoredPosition = new Vector3(width, 0, 0);
                FindObjectOfType<GameLogic>().tutorialImage.rectTransform.anchoredPosition = new Vector3(width, 0, 0);
                FindObjectOfType<GameLogic>().speedUpImage.rectTransform.anchoredPosition = new Vector3(width, 0, 0);
                FindObjectOfType<GameLogic>().diamondImage.rectTransform.anchoredPosition = new Vector3(width, 0, 0);
                FindObjectOfType<GameLogic>().totalDiamondText.rectTransform.anchoredPosition = new Vector3(width, 0, 0);
            }

            FindObjectOfType<UIController>().SetLastText(Util.GetLastScore());
            FindObjectOfType<UIController>().SetBestText(Util.GetBestScore());
            FindObjectOfType<UIController>().DOAnimIN();

            FindObjectOfType<ButtonMute>().SetSoundState();

            RequestFullAd();
        }

        /// <summary>
        /// Method update, exit game when user click back
        /// </summary>
        void Update() {
#if UNITY_ANDROID
            if (Input.GetKey(KeyCode.Escape)) {
                FindObjectOfType<ButtonExit>().OnClickedExitButton();
            }
#endif
        }

        /// <summary>
        /// Create a new game: Set the texts, the numTotalOfMove and if the last game was not a game over : do the animation in
        /// </summary>
        public void SetNewGame() {
            isGameOver = false;
            DOTween.timeScale = 1.1f;
            point = 0;
            
            if (!Util.RestartFromGameOver()) {
                DOMoveLevelIn(() => {
                    FindObjectOfType<GameLogic>().DOStart();
                });
            }
        }

        /// <summary>
        /// When a move is done, ie. player tap at the good moment, we decrease the numTotalOfMove ( -1 ) and we check if success (numTotalOfMove = 0). If success, we call the function LevelClear. If not, play a sound
        /// </summary>
        public void MoveDone() {
            soundManager.PlayTouch();
        }
        
        /// <summary>
        /// When a move is done, ie. player tap on the screen and the color of the ball is not equal of the color of the part of the circle below => Game Over. We restart the game and show interstitial. If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
        /// </summary>
        public void GameOver() {
            soundManager.PlayFail();
            isGameOver = true;

            StopAllCoroutines();

            ReportScoreToLeaderboard(point);

            ShowAds();

            Util.SetLastScore(point);

            FindObjectOfType<GameLogic>().GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(Util.ReloadLevel);
            //		FindObjectOfType<GameLogic>().GetComponent<RectTransform>().DOShakePosition(0.30f,10,100,90).OnComplete(() => {
            //		});
        }
        
        /// <summary>
        /// Animation out of the circle (from center to left)
        /// </summary>
        void DOMoveLevelOut(Action callback) {
            float width = Util.getWidth();

            DOVirtual.Float(0f, -1.5f * width, 0.3f,
                (float f) => {
                    FindObjectOfType<GameLogic>().GetComponent<RectTransform>().anchoredPosition = new Vector3(f, 0, 0);
                })
                .OnComplete(() => {
                    if (callback != null) {
                        callback();
                    }
                });
        }
        
        /// <summary>
        /// Animation in of the circle (from right to center)
        /// </summary>
        void DOMoveLevelIn(Action callback) {
            float width = Util.getWidth();
            float height = Util.getHeight();

            // show tutorial on first play
            if (Util.FirstPlay()) {
                DOVirtual.Float(+width * 1.5f, 0f, 0.3f,
                    (float f) => {
                        FindObjectOfType<GameLogic>().tutorialImage.rectTransform.anchoredPosition = new Vector3(f, 0, 0);
                    })
                    .SetDelay(0.5f);
            }

            DOVirtual.Float(+width * 1.5f, 0f, 0.3f,
                (float f) => {
                    FindObjectOfType<GameLogic>().diamondImage.rectTransform.anchoredPosition = new Vector3(f - width / 2.3f, height / 2.25f, 0);
                    FindObjectOfType<GameLogic>().totalDiamondText.rectTransform.anchoredPosition = new Vector3(f - width / 2.3f + width / 6f, height / 2.25f, 0);
                })
                .SetDelay(0.3f);

            DOVirtual.Float(+width * 1.5f, 0f, 0.3f,
                (float f) => {
                    FindObjectOfType<GameLogic>().GetComponent<RectTransform>().anchoredPosition = new Vector3(f, 0, 0);
                })
                .SetDelay(0.1f)
                .OnComplete(() => {
                    if (callback != null)
                        callback();
                });
        }
        
        /// <summary>
        /// If using Very Simple Leaderboard by App Advisory, report the score : http://u3d.as/qxf
        /// </summary>
        void ReportScoreToLeaderboard(int p) {
#if APPADVISORY_LEADERBOARD
            //LeaderboardManager.ReportScore(p);
#else
			print("Get very simple leaderboard to use it : http://u3d.as/qxf");
#endif
        }


        void RequestFullAd() {
            string fullId = "ca-app-pub-7722608051498261/7101042638";
            fullAdmob = new InterstitialAd(fullId);
            AdRequest adRequest = new AdRequest.Builder().Build();
            fullAdmob.LoadAd(adRequest);
        }

        /// <summary>
        /// Show Ads - Interstitial. If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
        /// </summary>
        public void ShowAds() {
            int count = PlayerPrefs.GetInt("GAMEOVER_COUNT", 0);
            count++;

            if (count > numberOfPlayToShowInterstitial) {
                if (fullAdmob.IsLoaded()) {
                    PlayerPrefs.SetInt("GAMEOVER_COUNT", 0);
                    fullAdmob.Show();
                }

            } else {
                PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
            }
            PlayerPrefs.Save();
        }
    }
}