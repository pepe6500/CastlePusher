using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallGame
{
    public class MainUIManager : MonoBehaviour
    {
        public BallsData ballsData;
        public CastlesData castlesData;

        public SnapScroll ballSnapScroll;
        public SnapScroll castleSnapScroll;

        // Start is called before the first frame update
        void Start()
        {
            for(int i = 0; i < ballsData.ballDatas.Length; i++)
            {
                GameObject ball = new GameObject();
                ball.AddComponent<RectTransform>();
                ball.AddComponent<Image>();
                ball.AddComponent<BallPan>();
                ball.GetComponent<Image>().sprite = ballsData.ballDatas[i].sprite;
                ball.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 180);
                ball.GetComponent<BallPan>().StartDistansX = 0;
                ball.GetComponent<BallPan>().EndDistansX = 270;
                ballSnapScroll.AddPan(ball);
            }
            for (int i = 0; i < castlesData.castleDatas.Length; i++)
            {
                GameObject ball = new GameObject();
                ball.AddComponent<RectTransform>();
                ball.AddComponent<Image>();
                ball.GetComponent<Image>().sprite = castlesData.castleDatas[i].sprite;
                ball.GetComponent<RectTransform>().sizeDelta = new Vector2(1080, 1100);
                castleSnapScroll.AddPan(ball);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
