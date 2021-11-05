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
                // ���� ���� ������ ���� ƨ��
                float y = col.transform.position.y < transform.position.y ? 1 : -1;
                // ���� ��������� ������Ŵ
                float x = HitFactor(transform.position, col.transform.position, 8f);
                // ���⸸ ���⵵�� nomalized ��
                Vector2 dir = new Vector2(x, y).normalized;
                speed += 1;
                // ����� �ӵ��� ���ӵ� ����
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
                // ���� ���� ������ ���� ƨ��
                float y = col.transform.position.y < transform.position.y ? 1 : -1;
                // ���� ��������� ������Ŵ
                float x = HitFactor(transform.position, col.transform.position, 8f);
                // ���⸸ ���⵵�� nomalized ��
                Vector2 dir = new Vector2(x, y).normalized;
                speed += 1;
                // ����� �ӵ��� ���ӵ� ����
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
            //�� ������ �����ٸ� �ٽ� ������
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
