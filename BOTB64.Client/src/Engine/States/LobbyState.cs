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
        private GameSizeType CurrentMode = GameSizeType.V2P;
        private string JoinCode = "";

        private Task<(Guid lobbyId, string joinCode)>? PendingCreate;
        private Task<JoinLobbyResponse?>? PendingJoin;
        private Task<LobbyDto?>? PendingPoll;
        private float PollTimer;

        private string RelayAddress = "";
        private const int RelayPort = 9050;

        private bool TransitionStarted = false;

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
                var (lobbyId, joinCode) = PendingCreate.Result;
                PendingCreate = null;

                if (lobbyId == Guid.Empty)
                {
                    // already handled by createsafe
                }
                else
                {
                    LobbyID = lobbyId;
                    JoinCode = joinCode;
                    IsHost = true;
                    Screen.ShowWaitingRoom(JoinCode, CurrentMode, IsHost);
                }
            }

            if (PendingJoin != null && PendingJoin.IsCompletedSuccessfully)
            {
                var result = PendingJoin.Result;
                PendingJoin = null;

                if (result == null)
                {
                    // already handled by joinsafe
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

                if (PendingPoll?.IsFaulted == true)
                {
                    Console.WriteLine(PendingPoll.Exception);
                    Console.WriteLine(PendingPoll.Exception?.InnerException);
                }

                if (PendingPoll?.IsCompletedSuccessfully == true)
                {
                    var lobby = PendingPoll.Result;

                    if (lobby != null)
                    {
                        if (CurrentMode != lobby.GameSizeType)
                        {
                            CurrentMode = lobby.GameSizeType;

                            Screen.ShowWaitingRoom(JoinCode, CurrentMode, IsHost);
                        }

                        Screen.RefreshPlayerList(
                            lobby.Players.Select(p =>
                                p.PlayerId == PlayerID
                                    ? $"{p.DisplayName} (you)"
                                    : p.DisplayName
                            ).ToList()
                        );

                        if (lobby.Started && !TransitionStarted)
                        {
                            TransitionStarted = true;
                            _ = TransitionToCharacterSelect(lobby);
                        }
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
            RelayAddress = StripScheme(address);
            API = new LobbyApiClient(NormalizeAddress(address));
            PendingJoin = JoinSafe(displayName, lobbyIdOrCode);
        }

        private async Task<JoinLobbyResponse?> JoinSafe(string displayName, string lobbyIdOrCode)
        {
            try
            {
                return await API!.JoinLobby(PlayerID, displayName, lobbyIdOrCode);
            }
            catch (Exception ex)
            {
                Screen.SetStatus(ex.Message);
                return null;
            }
        }

        public void OnCreateClicked(string displayName, string address)
        {
            LocalPlayerIdentity.SetDisplayName(displayName);
            PlayerID = LocalPlayerIdentity.PlayerId;
            RelayAddress = StripScheme(address);
            API = new LobbyApiClient(NormalizeAddress(address));
            PendingCreate = CreateSafe(displayName);
        }

        private async Task<(Guid lobbyId, string joinCode)> CreateSafe(string displayName)
        {
            try
            {
                return await API!.CreateLobby(PlayerID, displayName, CurrentMode);
            }
            catch (Exception ex)
            {
                Screen.SetStatus(ex.Message);
                return (Guid.Empty, ""); // sentinel — Update below needs to recognize this as "don't proceed"
            }
        }

        private static string StripScheme(string address)
        {
            address = address.Trim();
            if (address.StartsWith("http://")) address = address["http://".Length..];
            if (address.StartsWith("https://")) address = address["https://".Length..];
            // also strip a trailing :port if the user typed one for the HTTP address — relay uses its own fixed port
            int colon = address.IndexOf(':');
            return colon >= 0 ? address[..colon] : address;
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
            if (!IsHost || API == null || LobbyID == Guid.Empty) return;
            _ = StartGameAsync();
        }

        private async Task StartGameAsync()
        {
            await API!.StartLobby(LobbyID, PlayerID);
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
        private async Task TransitionToCharacterSelect(LobbyDto lobby)
        {
            var seats = SeatAssignment.Assign(CurrentMode, lobby.Players, GameModeRules.DefaultFactionOrder, GameModeRules.TotalCharacters(CurrentMode));
            var session = new NetSession(LobbyID, PlayerID, lobby.HostPlayerId) { Players = seats };

            try
            {
                await session.Connect(RelayAddress, RelayPort);
            }
            catch (Exception ex)
            {
                Screen.SetStatus($"Failed to connect to relay: {ex.Message}");
                return; // don't transition into a broken session
            }

            var cs = new CharacterSelectState { GameType = GameType.IPMultiplayer, GameSizeType = CurrentMode, Session = session };
            StateManager.ChangeState(cs);
        }
    }
}