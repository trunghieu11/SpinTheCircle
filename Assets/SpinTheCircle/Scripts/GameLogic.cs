
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Advertisements;

namespace AppAdvisory.SpinTheCircle {
    /// <summary>
    /// In charge of all the circle logic. Attached to the game object: "CircleParent". Create the colors, Spawn each element of the circle. Check the color when the player tap the screen etc...
    /// In charge of the rotation of the circle and of the input in the game (who will stop the rotation, check the color, and start the rotation in the other direction). Attached to the game object: "PartParent".
    /// </summary>
    public class GameLogic : MonoBehaviour {
        /// <summary>
        /// Prefab of Circle. Use to create the circle. Each part is a UI Image with a certain fillAmount
        /// </summary>
        public CirclePart circlePrefab;
        
        /// <summary>
        /// Number of parts in the circle, for the current level
        /// </summary>
        int numOfPart = 12;
        
        /// <summary>
        /// Number of colors in the circle, for the current level
        /// </summary>
        int numOfColor = 3;
        
        /// <summary>
        /// Reference to the GameObject who contains all the part of the circle we will spawn
        /// </summary>
        public RectTransform partParent;
        
        /// <summary>
        /// Reference to the ball Image = player
        /// </summary>
        public Image ball;
        
        /// <summary>
        /// Reference to all the parts contained in the circle, for the current level
        /// </summary>
        List<CirclePart> allCircles = new List<CirclePart>();
        
        /// <summary>
        /// Reference to the last color to find, to avoid duplicate check
        /// </summary>
        Color lastColor;
        bool shuffleColorAray = true;
        
        /// <summary>
        /// Reference to a list of color built for a level
        /// </summary>
        public List<Color> listColorReordered = new List<Color>();
        
        /// <summary>
        /// Tutorial image
        /// </summary>
        public Image tutorialImage;
        
        /// <summary>
        /// Speed up image
        /// </summary>
        public Image speedUpImage;
        
        /// <summary>
        /// diamond image
        /// </summary>
        public Image diamondImage;
        
        /// <summary>
        /// diamond counter
        /// </summary>
        public int totalDiamond = 0;
        
        /// <summary>
        /// diamond counter text
        /// </summary>
        public Text totalDiamondText;
        
        /// <summary>
        /// Check circle is move on and game is started
        /// </summary>
        bool gameStarted = false;
        
        /// <summary>
        /// The height of the ball jump
        /// </summary>
        float jumpHeight = -1f;
        GameManager _gameManager;
        GameManager gameManager {
            get {
                if (_gameManager == null) {
                    _gameManager = FindObjectOfType<GameManager>();
                }

                return _gameManager;
            }
        }
        
        /// <summary>
        /// ball throw speed
        /// </summary>
        float _ballSpeed = 0.65f;
        
        /// <summary>
        /// Speed of the circle, in seconds (total time in seconds to make 360 degree rotation), for the current level
        /// </summary>
        [System.NonSerialized]
        public float speedCircle = 0.05f;

        /// <summary>
        /// Continue group rectTransform
        /// </summary>
        public RectTransform continueGroupRect;

        /// <summary>
        /// Count number of continue
        /// </summary>
        private int _continuePlayCount = 0;

        /// <summary>
        /// zone id for ads
        /// </summary>
        public string zoneId;
        
        /// <summary>
        /// Create a new list of corlors for this level, randomly : listColorReordered and save it in PlayerPrefsX to use the same list of colors in case of game over
        /// </summary>
        void Awake() {
            DefineLevel();

            if (Util.RestartFromGameOver()) {
                listColorReordered = new List<Color>();
                listColorReordered.AddRange(PlayerPrefsX.GetColorArray(Util.ARRAY_COLOR_SAVED_PREF));
            } else {
                listColorReordered.AddRange(FindObjectOfType<ColorManager>().colors);
            }

            if (shuffleColorAray) {
                listColorReordered.Shuffle();
                listColorReordered.Shuffle();
            }
            if (!Util.RestartFromGameOver()) {
                while (listColorReordered.Count > numOfColor) {
                    listColorReordered.RemoveAt(0);
                }
            }

            PlayerPrefsX.SetColorArray(Util.ARRAY_COLOR_SAVED_PREF, listColorReordered.ToArray());
            PlayerPrefs.Save();
        }

        void DOOnEnable() {
            InputTouch.OnTouchLeft += OnTouchLeft;
            InputTouch.OnTouchRight += OnTouchRight;
        }

        void OnDisable() {
            InputTouch.OnTouchLeft -= OnTouchLeft;
            InputTouch.OnTouchRight -= OnTouchRight;
        }

        void OnTouchLeft() {
            DORotateCircle(1);
        }

        void OnTouchRight() {
            DORotateCircle(-1);
        }

        public void DOStart() {
            DOOnEnable();
            gameStarted = true;
            _continuePlayCount = 0;

            if (jumpHeight == -1f) {
                jumpHeight = partParent.GetChild(0).GetComponent<RectTransform>().sizeDelta.y / 2f;
            }

            // ball bouncing speed
            ball.rectTransform.DOLocalMoveY(-jumpHeight * 0.75f, _ballSpeed)
                .SetEase(Ease.InQuad)
                .SetLoops(-1, LoopType.Yoyo)
                .OnStepComplete(() => {
                    if (ball.rectTransform.localPosition.y < -150) {
                        bool isSameColor = CheckIfBallColorEqualCircleColor();
                        if (!isSameColor) {
                            if (_continuePlayCount < 1) {
                                PauseGame();
                                gameManager.isGameOver = true;
                            } else {
                                EndGame();
                            }
                        } else {
                            gameManager.point++;
                            totalDiamond++;
                            totalDiamondText.text = totalDiamond.ToString();
                            UpdateBallSpeed(gameManager.point);
                            DOColorBall();
                        }
                    }
                })
                .OnKill(() => {
                    ball.rectTransform.DOLocalMoveY(-1000, 2f);
                });

            if (Util.FirstPlay()) {
                PauseGame();
            }
        }

        /// <summary>
        /// Manage continue actions
        /// </summary>
        private void ShowContinuePopup() {
            float width = Util.getWidth();

            continueGroupRect.localPosition = new Vector2(Screen.width * 2f, continueGroupRect.localPosition.y);
            continueGroupRect.DOLocalMoveX(0, 0.3f);
        }

        /// <summary>
        /// Clicked button no thanks
        /// </summary>
        public void OnClickedButtonNoThanks() {
            MoveOutContinuePopup(EndGame);
        }

        /// <summary>
        /// Clicked button cost diamond
        /// </summary>
        public void OnClickedButtonCostDiamond() {
            if (totalDiamond >= Util.COST_DIAMOND_FOR_CONTINUE) {
                totalDiamond -= Util.COST_DIAMOND_FOR_CONTINUE;
                totalDiamondText.text = totalDiamond.ToString();
                MoveOutContinuePopup(PlayGame);
            }
        }

        /// <summary>
        /// Clicked button get diamond from ads
        /// </summary>
        public void OnClickedButtonGetDiamond() {
            ShowAdPlacement();
        }

        void ShowAdPlacement() {
            if (string.IsNullOrEmpty(zoneId)) {
                zoneId = null;
            }

            var options = new ShowOptions();
            options.resultCallback = HandleShowResult;
            Advertisement.Show(zoneId, options);
        }
        void HandleShowResult(ShowResult result) {
            switch (result) {
                case ShowResult.Finished:
                    Debug.Log("Video completed. Offer a reward to the player.");
                    totalDiamond += Util.GET_DIAMOND_FROM_ADS;
                    totalDiamondText.text = totalDiamond.ToString();
                    break;
                case ShowResult.Skipped:
                    Debug.LogWarning("Video was skipped.");
                    break;
                case ShowResult.Failed:
                    Debug.LogError("Video failed to show.");
                    break;
            }
        }

        /// <summary>
        /// Move out continue popup
        /// </summary>
        public void MoveOutContinuePopup(Action callback) {
            continueGroupRect.DOLocalMoveX(-Screen.width * 2f, 0.3f)
                .OnComplete(() => {
                    callback();
                });
        }

        /// <summary>
        /// End game actions
        /// </summary>
        public void EndGame() {
            PlayerPrefs.SetInt(Util.TOTAL_DIAMOND_PREF, totalDiamond);
            gameManager.GameOver();
            ball.rectTransform.DOKill();
        }

        /// <summary>
        /// Pause game method
        /// </summary>
        private void PauseGame() {
            DOTween.Pause(partParent);
            DOTween.Pause(ball.rectTransform);
            OnDisable();
        }

        /// <summary>
        /// Play game method
        /// </summary>
        private void PlayGame() {
            DOTween.Play(partParent);
            DOTween.Play(ball.rectTransform);
            DOOnEnable();
        }
        
        /// <summary>
        /// update ball speed when point increases
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private void UpdateBallSpeed(int point) {
            float lastTimeScale = DOTween.timeScale;
            if (point >= 60) {
                DOTween.timeScale = 1.4f;
            } else if (point >= 30) {
                DOTween.timeScale = 1.3f;
            } else if (point >= 10) {
                DOTween.timeScale = 1.2f;
            }

            if (lastTimeScale != DOTween.timeScale) {
                ShowSpeedUp();
            }
        }

        /// <summary>
        /// Show speed up text
        /// </summary>
        void ShowSpeedUp() {
            float width = Util.getWidth();

            DOVirtual.Float(+width * 1.5f, 0f, 0.5f,
                    (float f) => {
                        speedUpImage.rectTransform.anchoredPosition = new Vector3(f, width / 5, 0);
                    })
                    .OnStart(() => {
                        FindObjectOfType<GameManager>().soundManager.PlaySpeedIncrease();
                    });

            DOVirtual.Float(0f, -1.5f * width, 0.5f,
                (float f) => {
                    speedUpImage.rectTransform.anchoredPosition = new Vector3(f, width / 5, 0);
                })
                .SetDelay(1.5f);
        }

        /// <summary>
        /// Listen if the player tap or click, and if the game is not game over after the click (so ball color = part of the circle color) launch again the rotation but in the oposite direction
        /// </summary>
        void Update() {
            if (Util.FirstPlay() && Input.GetMouseButtonDown(0) && gameStarted) {
                MoveOutTutorial();
                PlayerPrefsX.SetBool(Util.FIRST_PLAY_PREF, false);
            }

            if (gameManager.isGameOver) {
                if (_continuePlayCount < 1) {
                    gameManager.isGameOver = false;
                    _continuePlayCount++;
                    ShowContinuePopup();
                } else {
                    gameStarted = false;
                    if (rotateTweener != null && rotateTweener.IsPlaying()) {
                        rotateTweener.Kill();
                    }
                    return;
                }
            }
        }
        
        /// <summary>
        /// Move out tutorial
        /// </summary>
        void MoveOutTutorial() {
            float width = Util.getWidth();
            DOVirtual.Float(0f, -1.5f * width, 0.3f,
                (float f) => {
                    tutorialImage.rectTransform.anchoredPosition = new Vector3(f, 0, 0);
                })
                .OnComplete(() => {
                    PlayGame();
                });
        }
        
        /// <summary>
        /// Reference to the tweener who rotate the circle
        /// </summary>
        Tweener rotateTweener;
        
        /// <summary>
        /// Start the rotation of the circle. Check in each updates if the ball enter a part of the circle with the same color of him. If we are inside a same color and we go out, that means the player doesn't tap before the ball go out of the part with the same color, so it's game over.
        /// </summary>
        void DORotateCircle(int direction) {
            //		if(firstMove)
            //		{
            //			DOStart();
            //			firstMove = false;
            //			return;
            //		}

            if (rotateTweener != null && rotateTweener.IsPlaying()) {
                return;
            }

            float target = partParent.rotation.eulerAngles.z + direction * 360f / 7f;

            rotateTweener = partParent.DORotate(Vector3.forward * target, speedCircle, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);
        }
        
        /// <summary>
        /// Place the border and the border shadow at the good place
        /// </summary>
        void Start() {
            BuildCircle();
            PrepareImages();
            LoadTotalDiamond();

            totalDiamondText.text = totalDiamond.ToString();
            ball.color = GetSelection().image.color;
        }
        
        /// <summary>
        /// Load total diamond from db
        /// </summary>
        void LoadTotalDiamond() {
            totalDiamond = PlayerPrefs.GetInt(Util.TOTAL_DIAMOND_PREF, 100);
        }
        
        /// <summary>
        /// All image methods
        /// </summary>
        void PrepareImages() {
            PrepareTutorialImage();
            PrepareSpeedUpImage();
            PrepareDiamondImage();
        }
        
        /// <summary>
        /// Initial tutorial image size
        /// </summary>
        void PrepareTutorialImage() {
            float width = Util.getWidth();
            tutorialImage.rectTransform.sizeDelta = Vector2.right * width * 0.9f + Vector2.up * width * 0.6f;
        }
        
        /// <summary>
        /// Initial speed up image size
        /// </summary>
        void PrepareSpeedUpImage() {
            float width = Util.getWidth();
            speedUpImage.rectTransform.sizeDelta = Vector2.right * width * 0.5f + Vector2.up * width * 0.125f;
        }
        
        /// <summary>
        /// Initial diamond image size
        /// </summary>
        void PrepareDiamondImage() {
            float width = Util.getWidth();
            diamondImage.rectTransform.sizeDelta = Vector2.right * width * 0.07f + Vector2.up * width * 0.07f;
        }
        
        /// <summary>
        /// IMPORTANT ==> It's here we define the levels. Change the formulas if you want. 
        /// </summary>
        void DefineLevel() {
            this.numOfColor = 7;
            this.numOfPart = 7;
        }
        
        /// <summary>
        /// Change the color of the ball = color to find
        /// </summary>
        public void DOColorBall() {
            lastColor = ball.color;

            Color newColor = allCircles[UnityEngine.Random.Range(0, allCircles.Count)].image.color;

            while (lastColor.IsEqual(newColor)) {
                newColor = allCircles[UnityEngine.Random.Range(0, allCircles.Count)].image.color;
            }


            ball.color = newColor;
        }

        CirclePart GetSelection() {
            return allCircles.Aggregate((x, y) => Math.Abs(x.GetMiddleAngle() - 180 - 25) < Math.Abs(y.GetMiddleAngle() - 180 - 25) ? x : y);
        }
        
        /// <summary>
        /// Check if the player tap at the good moment on the screen, ie. check if the color of the ball = the color of the part of the circle below the ball
        /// </summary>
        public bool CheckIfBallColorEqualCircleColor() {

            CirclePart selection = allCircles[0];

            selection = GetSelection();

            //		print("selection = " + selection.name + " - angle = " + selection.GetMiddleAngle());

            if (selection.image.color.IsEqual(ball.color)) {
                var initialPos = selection.transform.localPosition;
                selection.transform.DOMoveY(selection.transform.position.y - 30f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => {
                    selection.transform.localPosition = initialPos;
                });
                return true;
            } else {
                return false;
            }
        }
        
        /// <summary>
        /// Method to build the circle. Each part of the circle is an UI Image, type = fill image. We use the fill amout property to create the parts of the circle
        /// </summary>
        void BuildCircle() {
            float countAngle = 0f;
            float sizePart = 1f / numOfPart;

            for (int i = 0; i < numOfPart; i++) {
                float angle = i * 360 * sizePart;
                Color c = Color.white;

                int numColor = i;

                while (numColor >= listColorReordered.Count) {
                    numColor -= listColorReordered.Count;
                }

                c = listColorReordered[numColor];

                CirclePart circle = InstantiateCircle(sizePart, angle, c);
                circle.name = i.ToString();

                countAngle += angle;

                allCircles.Add(circle);
            }
        }
        
        /// <summary>
        /// Method to create a new circle = new part of the circle
        /// </summary>
        CirclePart InstantiateCircle() {
            var go = Instantiate(circlePrefab.gameObject) as GameObject;
            go.transform.SetParent(partParent, false);
            var circle = go.GetComponent<CirclePart>();
            return circle;
        }
        
        /// <summary>
        /// Method to create a new circle = new part of the circle
        /// </summary>
        CirclePart InstantiateCircle(float fillAmout, float angle, Color c) {
            return InstantiateCircle().Init(fillAmout, angle, c);
        }
    }
}