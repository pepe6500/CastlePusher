using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace BallGame
{
    public class BallGameManager : NetworkBehaviour
    {
        public SynclistGameObject players = new SynclistGameObject();

        [SerializeField] GameObject GamePlane;
        [SerializeField] GameObject castle;
        [SerializeField] GameObject racketPrefab;
        [SerializeField] GameObject ballPrefab;
        string matchID;

        public SynclistGameObject Objects = new SynclistGameObject();
        public GameObject gamePlane;
        public GameObject ball;
        public GameObject TopCastle;
        public GameObject BottomCastle;
        GameObject TopRacket;
        GameObject BottomRacket;
        bool[] ready = new bool[2];
        int[] maps = new int[2];

        Vector3 gamePosition;
        bool end = false;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void AddPlayer(Player _player)
        {
            players.Add(_player.gameObject);
        }

        public void Ready(int index, string _matchID, int map)
        {
            maps[index - 1] = map;
            ready[index - 1] = true;
            if(ready[0] && ready[1])
            {
                SetGame(_matchID);
            }
        }
        
        public void SetGame(string _matchID)
        {
            Debug.Log($" {_matchID} : SetGame");
            matchID = _matchID;
            gamePosition = new Vector3((-50 + (int.Parse(_matchID) % 100)) * 100, (50 - (int.Parse(_matchID) / 100)) * 100);

            GameObject tc = Instantiate(castle);
            NetworkServer.Spawn(tc);
            TopCastle = tc;
            TopCastle.name = "TopCastle";
            TopCastle.transform.position = gamePosition + new Vector3(0, 42.8f, 0);
            TopCastle.transform.rotation = Quaternion.Euler(0,0,180);
            TopCastle.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();
            TopCastle.GetComponent<Castle>().ballGameManager = this;
            TopCastle.GetComponent<Castle>().index = 2;
            Objects.Add(TopCastle);

            tc = Instantiate(castle);
            NetworkServer.Spawn(tc);
            BottomCastle = tc;
            BottomCastle.name = "BottomCastle";
            BottomCastle.transform.position = gamePosition + new Vector3(0, -42.8f, 0);
            BottomCastle.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();
            BottomCastle.GetComponent<Castle>().ballGameManager = this;
            BottomCastle.GetComponent<Castle>().index = 1;
            Objects.Add(BottomCastle);

            TopRacket = Instantiate(racketPrefab);
            TopRacket.name = "TopRacket";
            TopRacket.transform.position = gamePosition + new Vector3(0, 33, 0);
            TopRacket.transform.rotation = Quaternion.Euler(0, 0, 180);
            TopRacket.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();
            TopRacket.GetComponent<BallPlayer>().index = 2;
            NetworkServer.AddPlayerForConnection(players[1].GetComponent<Player>().connectionToClient, TopRacket);
            NetworkServer.Spawn(TopRacket, players[1].GetComponent<Player>().connectionToClient);
            Objects.Add(TopRacket);

            BottomRacket = Instantiate(racketPrefab);
            BottomRacket.name = "BottomRacket";
            BottomRacket.transform.position = gamePosition + new Vector3(0, -33, 0);
            BottomRacket.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();
            BottomRacket.GetComponent<BallPlayer>().index = 1;
            NetworkServer.AddPlayerForConnection(players[0].GetComponent<Player>().connectionToClient, BottomRacket);
            NetworkServer.Spawn(BottomRacket, players[0].GetComponent<Player>().connectionToClient);
            Objects.Add(BottomRacket);

            gamePlane = Instantiate(GamePlane);
            gamePlane.transform.position = gamePosition;
            players[0].GetComponent<Player>().SetGame(gamePosition);
            players[1].GetComponent<Player>().SetGame(gamePosition);
            Debug.Log($" {_matchID} : SetGame Successe {gamePosition}");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetComponent<Player>().CountDownStart();
                players[i].GetComponent<Player>().SetMaps(maps[0],maps[1]);
            }
            StartCoroutine(IEGameStart(gamePosition));
        }

        public IEnumerator IEGameStart(Vector3 game)
        {
            yield return new WaitForSeconds(5.5f);
            GameObject ba = Instantiate(ballPrefab);
            NetworkServer.Spawn(ba);
            ball = ba;
            ball.transform.position = game;
            ball.GetComponent<NetworkMatchChecker>().matchId = matchID.ToGuid();
            ball.GetComponent<Ball>().networkManager = this;
            ball.GetComponent<Ball>().game = game;
            Objects.Add(ball);
        }

        public void AttackCastle(Castle castle)
        {
            if (!end)
            {
                Objects.Remove(ball);
                NetworkServer.Destroy(ball);
                if (castle.GetHP() > 1)
                {
                    castle.SetHP(castle.GetHP() - 1);
                    if (castle.index == 1)
                        BottomRacket.GetComponent<BallPlayer>().GetBall();
                    else
                        TopRacket.GetComponent<BallPlayer>().GetBall();
                    for (int i = 0; i < players.Count; i++)
                    {
                        players[i].GetComponent<Player>().SetCastleHP(castle.index, castle.GetHP());
                        players[i].GetComponent<Player>().GetBall(castle.index);
                    }

                }
                else
                {
                    castle.SetHP(0);
                    for (int i = 0; i < players.Count; i++)
                        players[i].GetComponent<Player>().SetCastleHP(castle.index, 0);

                    if(castle.index == 1)
                    {
                        players[0].GetComponent<Player>().Lose();
                        players[1].GetComponent<Player>().Win();
                    }
                    else
                    {
                        players[1].GetComponent<Player>().Lose();
                        players[0].GetComponent<Player>().Win();
                    }
                    end = true;
                }
            }

        }

        public void FireBall(int playerIndex)
        {
            if(playerIndex == 1)
            {
                if (BottomRacket.GetComponent<BallPlayer>().isGetBall)
                {
                    BottomRacket.GetComponent<BallPlayer>().ClientFireBall();
                    GameObject ba = Instantiate(ballPrefab);
                    NetworkServer.Spawn(ba);
                    ball = ba;
                    ball.transform.position = BottomRacket.GetComponent<BallPlayer>().getBall.transform.position;
                    ball.GetComponent<NetworkMatchChecker>().matchId = matchID.ToGuid();
                    ball.GetComponent<Ball>().networkManager = this;
                    ball.GetComponent<Ball>().game = gamePosition;
                    ball.GetComponent<Ball>().rigidbody2d.velocity = Vector3.up * ball.GetComponent<Ball>().speed;
                    Objects.Add(ball);
                }
            }
            else
            {
                if (TopRacket.GetComponent<BallPlayer>().isGetBall)
                {
                    TopRacket.GetComponent<BallPlayer>().ClientFireBall();
                    GameObject ba = Instantiate(ballPrefab);
                    NetworkServer.Spawn(ba);
                    ball = ba;
                    ball.transform.position = TopRacket.GetComponent<BallPlayer>().getBall.transform.position;
                    ball.GetComponent<NetworkMatchChecker>().matchId = matchID.ToGuid();
                    ball.GetComponent<Ball>().networkManager = this;
                    ball.GetComponent<Ball>().game = gamePosition;
                    ball.GetComponent<Ball>().rigidbody2d.velocity = Vector3.down * ball.GetComponent<Ball>().speed;
                    Objects.Add(ball);
                }
            }

            for (int i = 0; i < players.Count; i++)
                players[i].GetComponent<Player>().TargetFireBall(playerIndex);
        }

        public void ResetGame()
        {
            if(end)
            {
                for (int i = Objects.Count - 1; i >= 0; i--)
                {
                    GameObject temp = Objects[i];
                    Objects.Remove(temp);
                    NetworkServer.Destroy(temp);
                }
                SetGame(matchID);
                end = false;
            }
        }

        public void ExitGame()
        {
            Destroy(gamePlane);
            for(int i = 0; i < MatchMaker.instance.matches.Count; i++)
            {
                if(MatchMaker.instance.matches[i].matchID == matchID)
                {
                    MatchMaker.instance.matches.Remove(MatchMaker.instance.matches[i]);
                    return;
                }
            }
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetComponent<Player>().GoMain();
            }
            NetworkServer.Destroy(gameObject);
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                GameObject temp = Objects[i];
                Objects.Remove(temp);
                NetworkServer.Destroy(temp);
            }
        }
    }
}
