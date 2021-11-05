using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BallGame
{
    public class Player : NetworkBehaviour
    {
        public static Player localPlayer;
        [SyncVar] public string matchID;
        [SyncVar] public int playerIndex;
        [SyncVar] public int map;
        [SyncVar] public int ball;
        NetworkMatchChecker networkMatchChecker;
        public BallGameManager gameManager;

        private void Start()
        {
            if (isLocalPlayer)
            {
                localPlayer = this;
            }
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
            DontDestroyOnLoad(gameObject);
        }

        /*
         * HOST
         */

        public void HostGame()
        {
            CmdHostGame();
        }

        [Command]
        void CmdHostGame()
        {
            if(MatchMaker.instance.HostGame(out matchID, gameObject, out playerIndex))
            {
                Debug.Log($"<color = green>Game hosted successhully</color>");
                networkMatchChecker.matchId = matchID.ToGuid();
                TargetHostGame(true, matchID, playerIndex);
            }
            else
            {
                Debug.Log($"<color = red>Game hosted failed</color>");
                TargetHostGame(false, matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string _matchID, int _playerIndex)
        {
            Debug.Log($"Match ID : {matchID} == {_matchID}");
            matchID = _matchID;
            playerIndex = _playerIndex;
            UILobby.instanse.HostSuccess(success, _matchID);
        }

        /*
         * JOIN
         */

        public void JoinGame(string _inputID)
        {
            CmdJoinGame(_inputID);
        }

        [Command]
        void CmdJoinGame(string _matchID)
        {
            matchID = _matchID;
            if (MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex))
            {
                Debug.Log($"<color = green>Game Joined successhully</color>");
                networkMatchChecker.matchId = _matchID.ToGuid();
                TargetJoinGame(true, _matchID, playerIndex);
                if (playerIndex == 2)
                {
                    MatchMaker.instance.BeginGame(matchID);
                    Debug.Log($"<color = red>{matchID} Game Begined</color>");
                }
            }
            else
            {
                Debug.Log($"<color = green>Game Joined failed</color>");
                TargetJoinGame(false, _matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success, string _matchID, int _playerIndex)
        {
            Debug.Log($"Match ID : {matchID} == {_matchID}");
            matchID = _matchID;
            playerIndex = _playerIndex;
            UILobby.instanse.JoinSuccess(success, _matchID);
        }
        /*
         * QuickJoin
         */

        public void QuickJoinGame()
        {
            CmdQuickJoinGame();
        }

        [Command]
        void CmdQuickJoinGame()
        {
            if (MatchMaker.instance.QuickJoinGame(out matchID, gameObject, out playerIndex))
            {
                Debug.Log($"<color = green>Game Joined successhully</color>");
                networkMatchChecker.matchId = matchID.ToGuid();
                TargetQuickJoinGame(true, matchID, playerIndex);
                if (playerIndex == 2)
                {
                    MatchMaker.instance.BeginGame(matchID);
                    Debug.Log($"<color = red>{matchID} Game Begined</color>");
                }
            }
            else
            {
                if (MatchMaker.instance.QuickHostGame(out matchID, gameObject, out playerIndex))
                {
                    Debug.Log($"<color = green>Game hosted successhully</color>");
                    networkMatchChecker.matchId = matchID.ToGuid();
                    TargetQuickJoinGame(true, matchID, playerIndex);
                }
                else
                {
                    Debug.Log($"<color = red>Game hosted failed</color>");
                    TargetQuickJoinGame(false, matchID, playerIndex);
                }
            }
        }

        [TargetRpc]
        void TargetQuickJoinGame(bool success, string _matchID, int _playerIndex)
        {
            Debug.Log($"Match ID : {matchID} == {_matchID}");
            matchID = _matchID;
            playerIndex = _playerIndex;
            UILobby.instanse.QuickJoinSuccess(success, _matchID);
        }

        /*
         * 게임 시작 시 설정 통신
         */

        public void BeginGame()
        {
            CmdBeginGame();
        }

        [Command]
        void CmdBeginGame()
        {
            MatchMaker.instance.BeginGame(matchID);
            Debug.Log($"<color = red>{matchID} Game Begined</color>");
        }

        public void StartGame()
        {
            TargetBeginGame();
        }

        [TargetRpc]
        void TargetBeginGame()
        {
            Debug.Log($"Match ID : {matchID} | Beginning");
            //load gamescene
            UnityEngine.SceneManagement.SceneManager.LoadScene("InGame", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        public void Ready()
        {
            CmdReady(playerIndex, Player.localPlayer.matchID, map);
            Debug.Log($" {Player.localPlayer.matchID} : Ready {playerIndex} Player Client");
            gameManager = GameObject.Find("BallGameManager(Clone)").GetComponent<BallGameManager>();
        }
        
        [Command]
        void CmdReady(int playerIndex, string _matchID, int map)
        {
            Debug.Log($" {_matchID} : Ready {playerIndex} Player");
            gameManager.Ready(playerIndex, _matchID, map);
        }
        
        public void SetGame(Vector3 game)
        {
            TargetSetGame(game);
        }

        [TargetRpc]
        void TargetSetGame(Vector3 game)
        {
            GameObject.Find("Game").transform.position = game;
        }

        [TargetRpc]
        public void CountDownStart()
        {
            GameObject.Find("InGameUI").GetComponent<InGameUIManager>().CountDownStart();
        }

        [TargetRpc]
        public void SetMaps(int map1, int map2)
        {
            GameObject.Find("InGameUI").GetComponent<InGameUIManager>().SetMaps(map1, map2);
        }

        /*
         * 게임 실행 중 통신
         */


        public void SetCastleHP(int index, int hp)
        {
            TargetSetCastleHP(index, hp);
        }

        [TargetRpc]
        void TargetSetCastleHP(int index, int hp)
        {
            if(index == 1)
            {
                gameManager.Objects[1].GetComponent<Castle>().SetHP(hp);
            }
            else
            {
                gameManager.Objects[0].GetComponent<Castle>().SetHP(hp);
            }
        }


        public void GetBall(int index)
        {
            TargetGetBall(index);
        }

        [TargetRpc]
        void TargetGetBall(int index)
        {
            if (index == 1)
            {
                gameManager.Objects[3].GetComponent<BallPlayer>().GetBall();
            }
            else
            {
                gameManager.Objects[2].GetComponent<BallPlayer>().GetBall();
            }
        }

        public void FireBall()
        {
            CmdFireBall(playerIndex);
        }

        [Command]
        public void CmdFireBall(int index)
        {
            gameManager.FireBall(index);
        }
        
        [TargetRpc]
        public void TargetFireBall(int index)
        {
            if(index == 1)
            {
                gameManager.Objects[3].GetComponent<BallPlayer>().ClientFireBall();
            }
            else
            {
                gameManager.Objects[2].GetComponent<BallPlayer>().ClientFireBall();
            }
        }

        public void Win()
        {
            TargetWin();
        }

        [TargetRpc]
        public void TargetWin()
        {
            GameObject.Find("InGameUI").GetComponent<InGameUIManager>().Win();
        }

        public void Lose()
        {
            TargetLose();
        }

        [TargetRpc]
        public void TargetLose()
        {
            GameObject.Find("InGameUI").GetComponent<InGameUIManager>().Lose();
        }

        public void ResetGame()
        {
            CmdResetGame();
        }

        [Command]
        public void CmdResetGame()
        {
            gameManager.ResetGame();
        }

        public void ExitGame()
        {
            CmdExitGame();
        }

        [Command]
        public void CmdExitGame()
        {
            gameManager.ExitGame();
        }

        public void GoMain()
        {
            matchID = null;
            playerIndex = -1;
            gameManager = null;
            TargetGoMain();
        }

        [TargetRpc]
        public void TargetGoMain()
        {
            GameObject.Find("SceneManager").GetComponent<SceneLoadManager>().LoadScene("Main");
        }
    }
}
