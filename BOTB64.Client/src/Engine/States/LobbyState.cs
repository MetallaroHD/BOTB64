using BOTB64.Engine.Net;
using BOTB64.Shared;
using BOTB64.Runtime;

namespace BOTB64.Engine.States
{
    public class LobbyState : IGameState
    {
        public GameType GameType { get; set; }
        public GameSizeType GameSizeType { get; set; }
        public bool IsHost { get; set; } // set by MainMenuState depending on Create vs Join flow
        public string? JoinCode { get; set; } // only used when joining an existing lobby

        private LobbyApiClient API;
        private Guid LobbyID;
        private int PlayerID;

        private Task<LobbyDto?>? PendingPoll;
        private Task<(Guid lobbyId, string joinCode)>? PendingCreate;
        private float Timer;

        public void OnEnter()
        {
            API = new LobbyApiClient("http://localhost:5000"); // TODO: pull from shared config once that exists
            PlayerID = LocalPlayerIdentity.PlayerId; // wherever you end up tracking "who am I" locally

            if (IsHost)
                PendingCreate = API.CreateLobby(PlayerID, GameSizeType);
            else
                _ = JoinExisting();
        }

        private async Task JoinExisting()
        {
            var result = await API.JoinLobby(LocalPlayerIdentity.PlayerId, JoinCode!);
            if (result != null)
                LobbyID = result.LobbyId;
            // else: show an error state — bad code / lobby full / etc.
        }

        public void Update(float dt)
        {
            if (PendingCreate != null)
            {
                if (PendingCreate.IsCompletedSuccessfully)
                {
                    (LobbyID, JoinCode) = PendingCreate.Result;
                    PendingCreate = null;
                }
                return;
            }

            if (LobbyID == Guid.Empty) return;

            Timer += dt;
            if (Timer > 1.0f && (PendingPoll == null || PendingPoll.IsCompleted))
            {
                Timer = 0;
                PendingPoll = API.GetLobby(LobbyID);
            }

            if (PendingPoll?.IsCompletedSuccessfully == true)
            {
                var lobby = PendingPoll.Result;
                if (lobby?.IsFull == true)
                    TransitionToCharacterSelect(lobby);
            }
        }

        private void TransitionToCharacterSelect(LobbyDto lobby)
        {
            var session = new NetSession(LobbyID, PlayerID, lobby.HostPlayerId);
            session.Players = lobby.Players.Select(p => new Player { PlayerID = p.PlayerId }).ToList();

            var seats = SeatAssignment.Assign(GameSizeType, lobby.Players.Select(p => p.PlayerId).ToList(), GameModeRules.DefaultFactionOrder, GameModeRules.TotalCharacters(GameSizeType));
            foreach (var seat in seats)
                session.Players.First(p => p.PlayerID == seat.PlayerID).OwnedPickSlots = seat.OwnedPickSlots;

            var cs = new CharacterSelectState { GameType = GameType, GameSizeType = GameSizeType, Session = session };
            StateManager.ChangeState(cs);
        }

        public void OnExit()
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
