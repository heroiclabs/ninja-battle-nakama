using System;
using System.Collections.Generic;
using UnityEngine;

using Nakama;
using Nakama.Helpers;

namespace NinjaBattle.Game
{
    public class PlayersManager : MonoBehaviour
    {
        #region FIELDS

        private MultiplayerManager multiplayerManager = null;
        private bool blockJoinsAndLeaves = false;

        #endregion

        #region EVENTS

        public event Action<List<IUserPresence>> onPlayersReceived;
        public event Action<IUserPresence> onPlayerJoined;
        public event Action<IUserPresence> onPlayerLeft;

        #endregion

        #region PROPERTIES

        public static PlayersManager Instance { get; private set; } = null;
        public List<IUserPresence> Players { get; private set; } = new List<IUserPresence>();
        public IUserPresence CurrentPlayer { get; private set; } = null;
        public int CurrentPlayerNumber { get; private set; } = 0;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            multiplayerManager = MultiplayerManager.Instance;
            multiplayerManager.onMatchJoin += ResetPlayersData;
            multiplayerManager.onMatchLeave += ResetPlayersData;
            multiplayerManager.Subscribe(MultiplayerManager.Code.Players, SetPlayers);
            multiplayerManager.Subscribe(MultiplayerManager.Code.ChangeScene, MatchStarted);
        }

        private void OnDestroy()
        {
            multiplayerManager.onMatchJoin -= ResetPlayersData;
            multiplayerManager.onMatchLeave -= ResetPlayersData;
            multiplayerManager.Unsubscribe(MultiplayerManager.Code.Players, SetPlayers);
            multiplayerManager.Unsubscribe(MultiplayerManager.Code.ChangeScene, MatchStarted);
        }

        private void SetPlayers(MultiplayerMessage message)
        {
            Players = message.GetData<List<IUserPresence>>();
            onPlayersReceived?.Invoke(Players);
            CurrentPlayer = Players.Find(player => player.SessionId == multiplayerManager.Self.SessionId);
            CurrentPlayerNumber = Players.IndexOf(CurrentPlayer);
        }

        private void PlayersChanged(IMatchPresenceEvent matchPresenceEvent)
        {
            if (blockJoinsAndLeaves)
                return;

            foreach (IUserPresence userPresence in matchPresenceEvent.Leaves)
            {
                Players.ForEach(presence =>
                {
                    if (presence != null && presence.SessionId == userPresence.SessionId)
                        presence = null;
                });

                onPlayerLeft?.Invoke(userPresence);
            }

            foreach (IUserPresence userPresence in matchPresenceEvent.Joins)
            {
                int index = Players.IndexOf(null);
                if (index > -1)
                    Players[index] = userPresence;
                else
                    Players.Add(userPresence);

                onPlayerJoined?.Invoke(userPresence);
            }
        }

        private void ResetPlayersData()
        {
            blockJoinsAndLeaves = false;
            Players = null;
        }

        public void MatchStarted(MultiplayerMessage message)
        {
            blockJoinsAndLeaves = true;
        }

        #endregion
    }
}
