using BOTB64.Engine.Net;
using BOTB64.Entities;
using BOTB64.Shared.DTOs;
using BOTB64.Graphics.G3D;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
using BOTB64.Shared;
using BOTB64.Shared.Files;
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

        private List<int> BlueOwners = new();
        private List<int> RedOwners = new();

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
        private List<CharacterDTO> BlueTeam = new();
        private List<CharacterDTO> RedTeam = new();

        public void OnEnter()
        {
            ShaderManager.LoadCharSelect("Misc\\champselect.vs", "Misc\\champselect.fs");

            Level = DatabaseFileManager.Levels[0]; //FOR NOW JUST 1 LEVEL

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
            if (Session == null) return true;
            return Session.LocalPlayer.OwnedPickSlots.Contains(PickingIndex);
        }

        public PickEvent ResolvePick(PickCommand command)
        {
            var evt = new PickEvent
            {
                PickingIndex = command.PickingIndex,
                CharacterIndex = command.CharacterIndex,
                Faction = DefaultFactionOrder[command.PickingIndex]
            };
            ApplyPickEvent(evt);
            return evt;
        }

        public bool ValidatePick(PickCommand command)
        {
            if (command.PickingIndex != PickingIndex) return false;
            if (TakenCharacterIndices.Contains(command.CharacterIndex)) return false;
            var owner = Session!.Players.FirstOrDefault(p => p.PlayerID == command.PlayerID);
            return owner != null && owner.OwnedPickSlots.Contains(command.PickingIndex);
        }

        public void ApplyPickEvent(PickEvent evt)
        {
            var picked = DatabaseFileManager.Characters[evt.CharacterIndex];
            int ownerId = Session?.Players.FirstOrDefault(p => p.OwnedPickSlots.Contains(evt.PickingIndex))?.PlayerID ?? -1;

            if (evt.Faction == Faction.RedTeam) { RedTeam.Add(picked); RedOwners.Add(ownerId); }
            else { BlueTeam.Add(picked); BlueOwners.Add(ownerId); }

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
                tex = ResourceManager.LoadTexture(CommonURIs.GetCharacterIcon(DatabaseFileManager.Characters[characterIndex]));
                CharacterIcons[characterIndex] = tex;
            }
            return tex;
        }

        public void OnExit()
        {
            Screen.CharacterPreview.Unload();
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
                RedTeam = RedTeam,
                BlueOwners = BlueOwners,
                RedOwners = RedOwners
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
            int positionIndex = 0;
            for (int i = 0; i < DatabaseFileManager.Characters.Count; i++)
            {
                if (DatabaseFileManager.Characters[i].Enabled)
                {
                    int index = i;
                    IconButton btn = new IconButton
                    {
                        Bounds = new RL.Rectangle(5 + (positionIndex % 12) * (64 + 5), 5 + (64 + 5) * (positionIndex / 12), 64, 64),
                        Icon = GetOrLoadIcon(index),
                        OnClick = () =>
                        {
                            CurrentSelection = index;
                            Screen.CharacterPreview.SetCharacter(DatabaseFileManager.Characters[index], index);
                            Screen.CharacterNameLabel.Text = DatabaseFileManager.Characters[index].Name;
                        }
                    };
                    Screen.AddElement(btn);
                    positionIndex++;
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
