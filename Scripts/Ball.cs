using UnityEngine;
using Mirror;

namespace BallGame
{
    public class Ball : NetworkBehaviour
    {
        public BallsData ballsData;

        public float speed = 30;
        public int pow = 10;
        public Rigidbody2D rigidbody2d;
        public BallGameManager networkManager;
        

        public Vector3 game;

        public override void OnStartClient()
        {
            base.OnStartClient();
            GetComponent<SpriteRenderer>().sprite = ballsData.ballDatas[Player.localPlayer.ball].sprite;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            // only simulate ball physics on server
            rigidbody2d.simulated = true;
            if (rigidbody2d.velocity == Vector2.zero)
            {
                Vector2 dir = new Vector2(1, 1).normalized;
                rigidbody2d.velocity = dir * speed;
            }
        }

        float HitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight)
        {
            return (ballPos.x - racketPos.x) / racketHeight;
        }

        // only call this on server
        [ServerCallback]
        void OnCollisionEnter2D(Collision2D col)
        {
            // Note: 'col' holds the collision information. If the
            // Ball collided with a racket, then:
            //   col.gameObject is the racket
            //   col.transform.position is the racket's position
            //   col.collider is the racket's collider

            // did we hit a racket? then we need to calculate the hit factor
            if (col.transform.GetComponent<BallPlayer>())
            {
                // 공이 위에 있으면 위로 튕김
                float y = col.transform.position.y < transform.position.y ? 1 : -1;
                // 공의 진행방향은 유지시킴
                float x = HitFactor(transform.position, col.transform.position, 8f);
                // 방향만 남기도록 nomalized 함
                Vector2 dir = new Vector2(x, y).normalized;
                speed += 1;
                // 방향과 속도로 가속도 설정
                rigidbody2d.velocity = dir * speed;
            }
        }

        [ServerCallback]
        void OnCollisionExit2D(Collision2D col)
        {
            // Note: 'col' holds the collision information. If the
            // Ball collided with a racket, then:
            //   col.gameObject is the racket
            //   col.transform.position is the racket's position
            //   col.collider is the racket's collider

            // did we hit a racket? then we need to calculate the hit factor
            if (col.transform.GetComponent<BallPlayer>())
            {
                // 공이 위에 있으면 위로 튕김
                float y = col.transform.position.y < transform.position.y ? 1 : -1;
                // 공의 진행방향은 유지시킴
                float x = HitFactor(transform.position, col.transform.position, 8f);
                // 방향만 남기도록 nomalized 함
                Vector2 dir = new Vector2(x, y).normalized;
                speed += 1;
                // 방향과 속도로 가속도 설정
                rigidbody2d.velocity = dir * speed;
            }
            else if (col.transform.GetComponent<Castle>())
            {
                networkManager.AttackCastle(col.transform.GetComponent<Castle>());
                speed = 30;
                Debug.Log(col.gameObject.name);
            }
        }

        [ServerCallback]
        void FixedUpdate()
        {
            //맵 밖으로 나간다면 다시 들어오게
            Vector2 po = transform.position;

            if (po.x < game.x - 22.3f)
            {
                po.x = game.x - 22.3f;
            }
            else if(po.x > game.x + 22.3f)
            {
                po.x = game.x + 22.3f;
            }

            if(po.y < game.y - 46f)
            {
                po.y = game.y - 46f;
            }
            else if (po.y > game.y + 46f)
            {
                po.y = game.y + 46f;
            }

            transform.position = po;
        }

        public void SetVelocity(Vector2 velocity)
        {
            Vector2 dir = velocity.normalized;
            rigidbody2d.velocity = dir * speed;
        }
    }
}
