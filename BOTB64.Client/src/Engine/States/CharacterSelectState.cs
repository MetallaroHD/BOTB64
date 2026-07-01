using BOTB64.Engine.Net;
using BOTB64.Entities;
using BOTB64.Entities.DTOs;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
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
                GameSizeType.v2P or GameSizeType.v2T => 4,
                GameSizeType.v3P or GameSizeType.v3T => 6,
                GameSizeType.v5P or GameSizeType.v5T => 10,
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

            Screen.StartButton.OnClick = () => { if (Session == null || Session.IsHost) StartGame(); };

            FillCharacterButtons();
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
        }

        public void OnExit()
        {
            Screen.Exit();
        }

        public void Update(float dt)
        {
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

            GameplayState gs = new GameplayState();
            gs.Initer = init;
            StateManager.ChangeState(gs);
        }

        private void FillCharacterButtons()
        {
            for (int i = 0; i < AllCharacters.Count; i++)
            {
                if (AllCharacters[i].Enabled)
                {
                    int index = i;

                    IconButton btn = new IconButton
                    {
                        Bounds = new RL.Rectangle(index * 50 + 5, 5, 64, 64),
                        Icon = RB.LoadTexture(new DataFile(AllCharacters[index].IconURI).Path),
                        OnClick = () => CurrentSelection = index
                    };
                    Screen.AddElement(btn);
                }
            }
        }
    }
}
