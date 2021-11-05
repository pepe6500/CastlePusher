using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Text;
using System.Security.Cryptography;

namespace BallGame
{
    [System.Serializable]
    public class Match
    {
        public string matchID;

        public SynclistGameObject players = new SynclistGameObject();
        public GameObject inGameManager;

        public bool publicMatch;

        public Match(string matchID, GameObject player, bool publicMatch)
        {
            this.matchID = matchID;
            this.publicMatch = publicMatch;
            players.Add(player);
        }

        public Match() { }
    }

    public class SynclistGameObject : SyncList<GameObject>
    {

    }

    public class SynclistMatch : SyncList<Match>
    {

    }

    public class MatchMaker : NetworkBehaviour
    {
        public static MatchMaker instance;

        public SynclistMatch matches = new SynclistMatch();
        public SyncList<string> matchIDs = new SyncList<string>();

        [SerializeField] GameObject managerBallPrefab;

        private void Start()
        {
            instance = this;
        }

        public bool QuickHostGame(out string _matchID, GameObject _player, out int playerIndex)
        {
            playerIndex = -1;
            _matchID = GetRandomMatchID();
            if (!matchIDs.Contains(_matchID))
            {
                matchIDs.Add(_matchID);
                matches.Add(new Match(_matchID, _player, true));
                Debug.Log("Match generated");
                playerIndex = 1;
                return true;
            }
            else
            {
                Debug.Log("Match ID already exists");
                return false;
            }
        }

        public bool QuickJoinGame(out string _matchID, GameObject _player, out int playerIndex)
        {
            playerIndex = -1;
            _matchID = null;
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].publicMatch == true && matches[i].players.Count < 2)
                {
                    _matchID = matches[i].matchID;
                    matches[i].players.Add(_player);
                    playerIndex = matches[i].players.Count;
                    Debug.Log("Match joined");
                    return true;
                }
            }
            Debug.Log("Match ID does not exists");
            return false;
        }

        public bool HostGame(out string _matchID, GameObject _player, out int playerIndex)
        {
            playerIndex = -1;
            _matchID = GetRandomMatchID();
            if (!matchIDs.Contains(_matchID))
            {
                matchIDs.Add(_matchID);
                matches.Add(new Match(_matchID, _player, false));
                Debug.Log("Match generated");
                playerIndex = 1;
                return true;
            }
            else
            {
                Debug.Log("Match ID already exists");
                return false;
            }
        }

        public bool JoinGame(string _matchID, GameObject _player, out int playerIndex)
        {
            playerIndex = -1;
            if (matchIDs.Contains(_matchID))
            {
                for(int i = 0; i < matches.Count; i++)
                {
                    if(matches[i].matchID == _matchID)
                    {
                        matches[i].players.Add(_player);
                        playerIndex = matches[i].players.Count;
                        break;
                    }
                }

                Debug.Log("Match joined");
                return true;
            }
            else
            {
                Debug.Log("Match ID does not exists");
                return false;
            }
        }

        public void BeginGame(string _matchID)
        {
            //게임매니저 설정

            GameObject networkManagerBall = Instantiate(managerBallPrefab);
            NetworkServer.Spawn(networkManagerBall);
            networkManagerBall.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();

            for(int i = 0; i < matches.Count; i++)
            {
                if(matches[i].matchID == _matchID)
                {
                    foreach(var player in matches[i].players)
                    {
                        Player _player = player.GetComponent<Player>();
                        networkManagerBall.GetComponent<BallGameManager>().AddPlayer(player.GetComponent<Player>());
                        player.GetComponent<Player>().gameManager = networkManagerBall.GetComponent<BallGameManager>();
                        _player.StartGame();
                    }
                    break;
                }
            }
        }


        public void SetGame(string _matchID)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    foreach (var player in matches[i].players)
                    {
                        Player _player = player.GetComponent<Player>();
                        NetworkServer.SetClientReady(_player.connectionToClient);
                    }
                    break;
                }
            }
        }

        public string GetRandomMatchID()
        {
            string id = string.Empty;
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                    int random = UnityEngine.Random.Range(0, 10);
                    id += random.ToString();
                }
                if (!matchIDs.Contains(id))
                {
                    Debug.Log("Random Match ID: " + id);
                    break;
                }
                id = string.Empty;
            }

            return id;
        }

    }

    public static class MatchExtensions
    {
        public static System.Guid ToGuid(this string id)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);
        }
    }
}
