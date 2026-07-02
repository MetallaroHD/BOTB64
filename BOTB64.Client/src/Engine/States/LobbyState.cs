using BOTB64.Engine.Net;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
using BOTB64.Shared;

namespace BOTB64.Engine.States
{
    public class LobbyState : IGameState
    {
        private LobbyScreen Screen = new();
        private LobbyApiClient? API;
        private Guid LobbyID;
        private int PlayerID;
        private bool IsHost;
        private GameSizeType CurrentMode = GameSizeType.v2P;
        private string JoinCode = "";

        private Task<(Guid lobbyId, string joinCode)>? PendingCreate;
        private Task<JoinLobbyResponse?>? PendingJoin;
        private Task<LobbyDto?>? PendingPoll;
        private float PollTimer;

        public void OnEnter()
        {
            LocalPlayerIdentity.Init();
            Screen.Controller = this;
            Screen.Enter();
        }

        public void OnExit() { }

        public void Update(float dt)
        {
            Screen.Update(dt);

            if (PendingCreate != null && PendingCreate.IsCompletedSuccessfully)
            {
                (LobbyID, JoinCode) = PendingCreate.Result;
                PendingCreate = null;
                IsHost = true;

                Screen.ShowWaitingRoom(JoinCode, CurrentMode, IsHost);
            }

            if (PendingJoin != null && PendingJoin.IsCompletedSuccessfully)
            {
                var result = PendingJoin.Result;
                PendingJoin = null;

                if (result == null)
                {
                    Screen.SetStatus("Could not join — check the Lobby ID and try again.");
                }
                else
                {
                    LobbyID = result.LobbyId;
                    JoinCode = result.JoinCode;

                    IsHost = result.HostPlayerId == PlayerID;

                    Screen.ShowWaitingRoom(JoinCode, CurrentMode, IsHost);
                }
            }

            if (LobbyID != Guid.Empty)
            {
                PollTimer += dt;

                if (PollTimer > 1.0f && (PendingPoll == null || PendingPoll.IsCompleted))
                {
                    PollTimer = 0;
                    PendingPoll = API!.GetLobby(LobbyID);
                }

                if (PendingPoll?.IsCompletedSuccessfully == true)
                {
                    var lobby = PendingPoll.Result;

                    if (lobby != null)
                    {
                        CurrentMode = lobby.GameSizeType;

                        Screen.RefreshPlayerList(
                            lobby.Players.Select(p =>
                                p.PlayerId == PlayerID
                                    ? $"{p.DisplayName} (you)"
                                    : p.DisplayName
                            ).ToList()
                        );
                    }
                }
            }
        }

        public void Render() => Screen.Draw();

        // --- called by LobbyScreen ---

        public void OnConnectClicked(string displayName, string address, string lobbyIdOrCode)
        {
            LocalPlayerIdentity.SetDisplayName(displayName);
            PlayerID = LocalPlayerIdentity.PlayerId;
            API = new LobbyApiClient(NormalizeAddress(address));
            PendingJoin = API.JoinLobby(PlayerID, displayName, lobbyIdOrCode);
        }

        public void OnCreateClicked(string displayName, string address)
        {
            LocalPlayerIdentity.SetDisplayName(displayName);
            PlayerID = LocalPlayerIdentity.PlayerId;
            API = new LobbyApiClient(NormalizeAddress(address));
            PendingCreate = API.CreateLobby(PlayerID, displayName, CurrentMode);
        }

        public void OnModeSelected(GameSizeType mode)
        {
            if (!IsHost || API == null) return;
            CurrentMode = mode;
            _ = API.SetLobbyMode(LobbyID, PlayerID, mode);
            Screen.ShowWaitingRoom(JoinCode, CurrentMode, IsHost);
        }

        public void OnStartClicked()
        {
            // TODO: host-only, triggers the actual roster-full check + transition into CharacterSelectState,
            // same shape as TransitionToCharacterSelect sketched a couple messages ago.
        }

        public void OnBackFromSetup() => StateManager.ChangeState(new MainMenuState());
        public void OnLeaveClicked()
        {
            if (API != null && LobbyID != Guid.Empty)
                _ = API.LeaveLobby(LobbyID, PlayerID);
            StateManager.ChangeState(new MainMenuState());
        }

        private static string NormalizeAddress(string address)
        {
            if (!address.StartsWith("http://") && !address.StartsWith("https://"))
                address = "http://" + address;
            return address;
        }
    }
}