using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallGame
{
    public class UILobby : MonoBehaviour
    {
        public static UILobby instanse;

        [SerializeField] InputField joinMatchInput;
        [SerializeField] List<Selectable> lobbySelectables = new List<Selectable>();
        [SerializeField] Button joinButton;
        [SerializeField] Button hostButton;
        [SerializeField] Canvas lobbyCanvas;
        [SerializeField] Text matchIDText;
        [SerializeField] SnapScroll maps;
        [SerializeField] SnapScroll balls;
        [SerializeField] GameObject quickJoining;
        [SerializeField] GameObject quickJoiningFailed;
        private void Start()
        {
            instanse = this;
        }

        public void QuickJoin()
        {
            quickJoining.SetActive(true);
            Player.localPlayer.map = maps.GetSelectIndex();
            Player.localPlayer.ball = balls.GetSelectIndex();
            Player.localPlayer.QuickJoinGame();
        }

        public void QuickJoinSuccess(bool success, string matchID)
        {
            if (!success)
            {
                quickJoining.SetActive(false);
                quickJoiningFailed.SetActive(true);
            }
        }

        public void Host()
        {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;
            Player.localPlayer.map = maps.GetSelectIndex();
            Player.localPlayer.ball = balls.GetSelectIndex();

            Player.localPlayer.HostGame();
        }

        public void HostSuccess(bool success, string matchID)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
                matchIDText.text = matchID;
            }
            else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void Join()
        {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;
            Player.localPlayer.map = maps.GetSelectIndex();
            Player.localPlayer.ball = balls.GetSelectIndex();

            Player.localPlayer.JoinGame(joinMatchInput.text);
        }

        public void JoinSuccess(bool success, string matchID)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
                matchIDText.text = matchID;
            }
            else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void BeginGame()
        {

        }
    }
}
