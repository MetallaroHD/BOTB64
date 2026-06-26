using BOTB64.Entities;
using BOTB64.Entities.DTOs;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;
using System.Runtime.CompilerServices;

namespace BOTB64.Engine.States
{
    public class CharacterSelectState : IGameState
    {
        private CharacterSelectScreen Screen = new();
        private int CurrentSelection = -1;
        // set using the modes
        private int TotalCharacters = 6;

        private readonly List<Faction> DefaultFactionOrder = new List<Faction>
        {
            Faction.BlueTeam,
            Faction.RedTeam,
            Faction.RedTeam,
            Faction.BlueTeam,
            Faction.BlueTeam,
            Faction.RedTeam,
            Faction.RedTeam,
            Faction.BlueTeam,
            Faction.BlueTeam,
            Faction.RedTeam,
        };

        private int PickingIndex = 0;

        private LevelDTO Level;
        private List<CharacterDTO> AllCharacters;

        private List<CharacterDTO> BlueTeam = new();
        private List<CharacterDTO> RedTeam = new();

        public void OnEnter()
        {
            JsonDataFile<LevelDTO> lf = new JsonDataFile<LevelDTO>();
            List<LevelDTO> levels = lf.DeserializeAll(new DataFile(CommonURIs.LevelJSON));
            Level = levels[0];

            JsonDataFile<CharacterDTO> cf = new JsonDataFile<CharacterDTO>();
            AllCharacters = cf.DeserializeAll(new DataFile(CommonURIs.CharacterJSON));

            Screen.LockButton.OnClick = () =>
            {
                if (CurrentSelection < 0)
                    return;
                if (PickingIndex >= DefaultFactionOrder.Count)
                    return;
                if (PickingIndex >= TotalCharacters)
                    return;

                if (DefaultFactionOrder[PickingIndex] == Faction.RedTeam)
                    RedTeam.Add(AllCharacters[CurrentSelection]);
                else
                    BlueTeam.Add(AllCharacters[CurrentSelection]);

                PickingIndex++;
            };

            Screen.StartButton.OnClick = () => StartGame();

            FillCharacterButtons();

            Screen.Enter();
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
