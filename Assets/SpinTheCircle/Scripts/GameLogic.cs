
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
        /// Create a new list of corlors for this level, randomly : listColorReordered and save it in PlayerPrefsX to use the same list of colors in case of game over
        /// </summary>
        void Awake() {
            DefineLevel();

            if (Util.RestartFromGameOver()) {
                listColorReordered = new List<Color>();
                listColorReordered.AddRange(PlayerPrefsX.GetColorArray("_arrayColorSaved"));
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

            PlayerPrefsX.SetColorArray("_arrayColorSaved", listColorReordered.ToArray());
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
                            gameManager.GameOver();
                            ball.rectTransform.DOKill();
                        } else {
                            gameManager.point++;
                            updateBallSpeed(gameManager.point);
                            DOColorBall();
                        }
                    }
                })
                .OnKill(() => {
                    ball.rectTransform.DOLocalMoveY(-1000, 2f);
                });

            if (Util.FirstPlay()) {
                DOTween.Pause(partParent);
                DOTween.Pause(ball.rectTransform);
                OnDisable();
            }
        }
        /// <summary>
        /// update ball speed when point increases
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private void updateBallSpeed(int point) {
            if (point >= 100) {
                DOTween.timeScale = 1.4f;
            } else if (point >= 60) {
                DOTween.timeScale = 1.3f;
            } else if (point >= 30) {
                DOTween.timeScale = 1.2f;
            } else if (point >= 10) {
                DOTween.timeScale = 1.1f;
            }
        }
        
        /// <summary>
        /// Listen if the player tap or click, and if the game is not game over after the click (so ball color = part of the circle color) launch again the rotation but in the oposite direction
        /// </summary>
        void Update() {
            if (Util.FirstPlay() && Input.GetMouseButtonDown(0) && gameStarted) {
                MoveOutTutorial();
                PlayerPrefsX.SetBool("_FirstPlay", false);
            }

            if (gameManager.isGameOver) {
                gameStarted = false;
                if (rotateTweener != null && rotateTweener.IsPlaying()) {
                    rotateTweener.Kill();
                }
                return;
            }
        }
        /// <summary>
        /// Move out tutorial
        /// </summary>
        void MoveOutTutorial() {
            float width = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;
            DOVirtual.Float(0f, -1.5f * width, 0.3f,
                (float f) => {
                    tutorialImage.rectTransform.anchoredPosition = new Vector3(f, 0, 0);
                })
                .OnComplete(() => {
                    DOTween.Play(partParent);
                    DOTween.Play(ball.rectTransform);
                    DOOnEnable();
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
            PrepareTutorial();

            ball.color = GetSelection().image.color;
        }
        /// <summary>
        /// Init tutorial image size
        /// </summary>
        void PrepareTutorial() {
            float width = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;
            tutorialImage.rectTransform.sizeDelta = Vector2.right * width * 0.9f + Vector2.up * width * 0.6f;
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