using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallGame
{
    public class BallPan : MonoBehaviour
    {
        public float StartDistansX;
        public float EndDistansX;
        Image image;

        // Start is called before the first frame update
        void Start()
        {
            image = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            float dis = Mathf.Abs(transform.parent.GetComponent<RectTransform>().anchoredPosition.x + transform.localPosition.x);
            if (StartDistansX < dis)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp(1 - ((dis - StartDistansX) / (EndDistansX - StartDistansX)), 0, 1));
            }
            else
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            }
        }
    }
}
