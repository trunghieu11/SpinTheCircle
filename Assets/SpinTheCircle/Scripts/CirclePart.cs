
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AppAdvisory.SpinTheCircle {
    /// <summary>
    /// Each part of the circle is a circle. We use the fillAmount component of UI image to get "parts". All the circles are child of the Game Object PartParent (= CircleRotator). The Circle prefab is in the Prefabs folder. Each Circles are instantiate in the CircleLogic at the start of each level
    /// </summary>
    public class CirclePart : MonoBehaviour {
        /// <summary>
        /// The image = a simple circle
        /// </summary>
        public Image image;

        /// <summary>
        /// Init the circle = the part of the circle. Each part is defined with a fillAmount = 1 / number of part in the circle, an angle and a color
        /// </summary>
        public CirclePart Init(float fillAmout, float angle, Color color) {
            image.type = Image.Type.Filled;
            image.fillAmount = fillAmout;
            image.rectTransform.localPosition = Vector3.zero;
            image.rectTransform.eulerAngles = Vector3.forward * angle;
            image.rectTransform.SetSiblingIndex(0);

            float width = Util.getWidth();

            image.rectTransform.sizeDelta = Vector2.one * width * 0.9f;
            image.color = color;

            gameObject.name = transform.GetSiblingIndex().ToString();

            return this;
        }

        /// <summary>
        /// Get the angle of the middle of the part of circle
        /// </summary>
        public float GetMiddleAngle() {
            float midAngle = image.rectTransform.eulerAngles.z;

            while (midAngle < 0) {
                midAngle += 360f;
            }

            while (midAngle > 360) {
                midAngle -= 360f;
            }

            return midAngle;
        }
    }
}