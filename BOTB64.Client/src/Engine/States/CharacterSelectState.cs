using BOTB64.Engine.Net;
using BOTB64.Entities;
using BOTB64.Entities.DTOs;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
using BOTB64.Shared;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Engine.States
{
    public class CharacterSelectState : IGameState
    {
        public GameType GameType { get; set; }
        public GameSizeType GameSizeType { get; set; }
        public NetSession? Session { get; set; }

        private CharacterSelectScreen Screen = new();
        private IPickChannel Channel;

        private int CurrentSelection = -1;
        private int TotalCharacters = 6;

        private readonly List<Faction> DefaultFactionOrder = new List<Faction>
    {
        Faction.BlueTeam, Faction.RedTeam, Faction.RedTeam, Faction.BlueTeam,
        Faction.BlueTeam, Faction.RedTeam, Faction.RedTeam, Faction.BlueTeam,
        Faction.BlueTeam, Faction.RedTeam,
    };

        private int PickingIndex = 0;
        private readonly HashSet<int> TakenCharacterIndices = new();
        private readonly Dictionary<int, RL.Texture2D> CharacterIcons = new();

        private LevelDTO Level;
        private List<CharacterDTO> AllCharacters;
        private List<CharacterDTO> BlueTeam = new();
        private List<CharacterDTO> RedTeam = new();

        public void OnEnter()
        {
            JsonDataFile<LevelDTO> lf = new JsonDataFile<LevelDTO>();
            Level = lf.DeserializeAll(new DataFile(CommonURIs.LevelJSON))[0];

            JsonDataFile<CharacterDTO> cf = new JsonDataFile<CharacterDTO>();
            AllCharacters = cf.DeserializeAll(new DataFile(CommonURIs.CharacterJSON));

            TotalCharacters = GameSizeType switch
            {
                GameSizeType.V2P or GameSizeType.V2T => 4,
                GameSizeType.V3P or GameSizeType.V3T => 6,
                GameSizeType.V5P or GameSizeType.V5T => 10,
                _ => TotalCharacters
            };

            Channel = Session == null
                ? new LocalPickChannel(this)
                : new NetworkedPickChannel(this, Session);

            Screen.LockButton.OnClick = () =>
            {
                if (CurrentSelection < 0) return;
                if (PickingIndex >= DefaultFactionOrder.Count || PickingIndex >= TotalCharacters) return;
                if (TakenCharacterIndices.Contains(CurrentSelection)) return;
                if (!IsLocalPlayersTurn()) return;

                Channel.Submit(new PickCommand
                {
                    PlayerID = Session?.LocalPlayerID ?? 0,
                    PickingIndex = PickingIndex,
                    CharacterIndex = CurrentSelection
                });
            };

            Screen.StartButton.Visible = Session == null || Session.IsHost;
            Screen.StartButton.OnClick = () => StartGame();

            if (Session != null)
                Session.OnMatchStartReceived += OnMatchStart;

            if (Session != null && IsSpectator())
                Screen.LockButton.Visible = false;

            FillCharacterButtons();
            UpdateNowPickingLabel();
            Screen.Enter();
        }

        private bool IsLocalPlayersTurn()
        {
            if (Session == null) return true; // hotseat local play, unchanged behavior
            return Session.LocalPlayer.OwnedPickSlots.Contains(PickingIndex);
        }

        // Host-only (or local-only) — the one place picks are decided
        public PickEvent ResolvePick(PickCommand command)
        {
            var evt = new PickEvent
            {
                PickingIndex = command.PickingIndex,
                CharacterIndex = command.CharacterIndex,
                Faction = DefaultFactionOrder[command.PickingIndex]
            };
            ApplyPickEvent(evt); // host applies to its own state immediately too
            return evt;
        }

        public bool ValidatePick(PickCommand command)
        {
            if (command.PickingIndex != PickingIndex) return false; // out of order / stale
            if (TakenCharacterIndices.Contains(command.CharacterIndex)) return false;
            var owner = Session!.Players.FirstOrDefault(p => p.PlayerID == command.PlayerID);
            return owner != null && owner.OwnedPickSlots.Contains(command.PickingIndex);
        }

        // Called on host (from ResolvePick) AND on every client (from NetworkedPickChannel.OnEventFromHost)
        public void ApplyPickEvent(PickEvent evt)
        {
            var picked = AllCharacters[evt.CharacterIndex];
            if (evt.Faction == Faction.RedTeam) RedTeam.Add(picked);
            else BlueTeam.Add(picked);

            TakenCharacterIndices.Add(evt.CharacterIndex);
            PickingIndex = evt.PickingIndex + 1;
            CurrentSelection = -1;

            var icon = GetOrLoadIcon(evt.CharacterIndex);
            if (evt.Faction == Faction.RedTeam) Screen.RedStrip.AddIcon(icon);
            else Screen.BlueStrip.AddIcon(icon);

            UpdateNowPickingLabel();
        }

        private void UpdateNowPickingLabel()
        {
            if (PickingIndex >= DefaultFactionOrder.Count || PickingIndex >= TotalCharacters)
            {
                Screen.NowPickingLabel.Text = "All picks complete";
                return;
            }

            if (Session == null)
            {
                Screen.NowPickingLabel.Text = $"Now picking: {DefaultFactionOrder[PickingIndex]}";
                return;
            }

            var picker = Session.Players.FirstOrDefault(p => p.OwnedPickSlots.Contains(PickingIndex));
            Screen.NowPickingLabel.Text = picker != null
                ? $"Now picking: {picker.DisplayName}"
                : $"Now picking: (spectated slot)";
        }

        private RL.Texture2D GetOrLoadIcon(int characterIndex)
        {
            if (!CharacterIcons.TryGetValue(characterIndex, out var tex))
            {
                tex = RB.LoadTexture(new DataFile(CommonURIs.GetCharacterIcon(AllCharacters[characterIndex])).Path);
                CharacterIcons[characterIndex] = tex;
            }
            return tex;
        }

        public void OnExit()
        {
            Screen.Exit();
        }

        public void Update(float dt)
        {
            Session?.PumpMainThreadActions();
            Screen.Update(dt);
        }

        public void Render()
        {
            Screen.Draw();
        }

        private void StartGame()
        {
            GameInitializer init = new GameInitializer
            {
                Level = Level,
                BlueTeam = BlueTeam,
                RedTeam = RedTeam
            };

            if (Session == null)
            {
                TransitionToGameplay(init);
            }
            else
            {
                Session.BroadcastMatchStart(init);
            }
        }

        private bool IsSpectator() => Session != null && Session.LocalPlayer.OwnedPickSlots.Count == 0;

        private void FillCharacterButtons()
        {
            for (int i = 0; i < AllCharacters.Count; i++)
            {
                if (AllCharacters[i].Enabled)
                {
                    int index = i;
                    IconButton btn = new IconButton
                    {
                        Bounds = new RL.Rectangle(5 + index * (64 + 5), 5, 64, 64),
                        Icon = GetOrLoadIcon(index),
                        OnClick = () => CurrentSelection = index
                    };
                    Screen.AddElement(btn);
                }
            }
        }

        private void OnMatchStart(GameInitializer init) => TransitionToGameplay(init);

        private void TransitionToGameplay(GameInitializer init)
        {
            GameplayState gs = new GameplayState { Initer = init, Session = Session };
            StateManager.ChangeState(gs);
        }
    }
}
