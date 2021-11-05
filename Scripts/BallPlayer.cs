using UnityEngine;
using Mirror;

namespace BallGame
{
    public class BallPlayer : NetworkBehaviour
    {
        public BallsData ballsData;

        [SyncVar] public int index;
        public float speed = 30;
        public Rigidbody2D rigidbody2d;
        public GameObject getBall;
        public Player player;

        public bool isGetBall = false;

        private void Start()
        {
            getBall.GetComponent<SpriteRenderer>().sprite = ballsData.ballDatas[Player.localPlayer.ball].sprite;
        }

        public void SetPlayer()
        {
            player = GameObject.Find("BallGameManager(Clone)").GetComponent<BallGameManager>().players[index - 1].GetComponent<Player>();
        }

        // need to use FixedUpdate for rigidbody
        [ClientCallback]
        void FixedUpdate()
        {
            if(index != 0 && player == null)
            {
                SetPlayer();
            }

            if (player != null)
            {
                if (player == Player.localPlayer)
                {
                    if (Input.GetAxisRaw("Horizontal") != 0)
                    {
                        rigidbody2d.velocity = transform.right * Input.GetAxisRaw("Horizontal") * speed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        if (Input.GetMouseButton(0))
                        {
                            if (Input.mousePosition.x > Screen.width / 2)
                                rigidbody2d.velocity = transform.right * 1 * speed * Time.fixedDeltaTime;
                            else
                                rigidbody2d.velocity = transform.right * -1 * speed * Time.fixedDeltaTime;
                        }
                        else
                            rigidbody2d.velocity = transform.right * 0 * speed * Time.fixedDeltaTime;
                    }

                    if (Input.GetKey(KeyCode.Space) || Input.GetMouseButtonDown(1) || Input.touchCount >= 2 && isGetBall)
                    {
                        isGetBall = false;
                        FireBall();
                    }

                }
            }

        }
        
        public void GetBall()
        {
            isGetBall = true;
            getBall.SetActive(true);
        }
        
        public void FireBall()
        {
            Player.localPlayer.FireBall();
        }
        
        public void ClientFireBall()
        {
            isGetBall = false;
            getBall.SetActive(false);
        }

        public void ButtonClick()
        {
            Debug.Log(gameObject.name + isLocalPlayer);
            if (player == Player.localPlayer)
            {
                GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
                RestartGame();
            }
        }

        [Command]
        public void RestartGame()
        {
            Debug.Log("RE1");
        }
    }
}