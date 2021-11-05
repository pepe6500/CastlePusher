using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallGame
{
    public class InGameUIManager : MonoBehaviour
    {
        public CastlesData castlesData;

        public GameObject winPanel;
        public GameObject losePanel;
        public GameObject topBack;
        public GameObject bottomBack;

        public CountDown countDown;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Win()
        {
            winPanel.SetActive(true);
        }

        public void Lose()
        {
            losePanel.SetActive(true);
        }

        public void ResetGame()
        {
            Player.localPlayer.ResetGame();
        }

        public void CountDownStart()
        {
            countDown.CountDownStart();
            winPanel.SetActive(false);
            losePanel.SetActive(false);
        }

        public void SetMaps(int map1, int map2)
        {
            bottomBack.GetComponent<SpriteRenderer>().sprite = castlesData.castleDatas[map1].sprite;
            topBack.GetComponent<SpriteRenderer>().sprite = castlesData.castleDatas[map2].sprite;
        }

        public void ExitGame()
        {
            Player.localPlayer.ExitGame();
        }
    }
}
