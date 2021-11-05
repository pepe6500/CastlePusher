using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallGame
{
    public class Ready : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Player.localPlayer.Ready();
            Debug.Log(Player.localPlayer.playerIndex);
            if(Player.localPlayer.playerIndex == 2)
            {
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
