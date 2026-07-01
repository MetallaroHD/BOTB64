using BOTB64.Engine;
using BOTB64.Engine.Net;
using BOTB64.Entities.DTOs;
using BOTB64.Entities.Effects;
using BOTB64.Graphics.Animations;
using BOTB64.Graphics.G3D;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
using System.Runtime.CompilerServices;

namespace BOTB64.Entities
{
    public struct GameInitializer
    {
        public LevelDTO Level;
        public List<CharacterDTO> BlueTeam;
        public List<CharacterDTO> RedTeam;
    }

    public class Game
    {
        private int CharAlloc = 0;

        private List<IGameEvent> Pending = new();

        private Level Level = new Level();
        private List<Character> Characters = new();
        private List<Spell> Spells = new();
        private List<Aura> Auras = new();

        public Faction Winner = Faction.Neutral;

        public Turn CurrentTurn;
        public Character CurrentCharacter => CurrentTurn.ActiveCharacter;

        public Board GetBoard() => Level.LevelBoard;

        public void Initialize(GameInitializer lI)
        {
            (string script, string model, string wall, string shaderv, string shaderf) = CommonURIs.GetLevelResources(lI.Level);
            ShaderManager.Load(shaderv, shaderf);
            Level = Level.Load(script, model, wall);
            LoadStartingCharacters(lI);
            if (Characters.Count < 1)
                throw new Exception("Must pick at least one character.");
            CurrentTurn = new Turn(0, Characters[0], this);
        }

        public void Update(float dt, out bool gameOver)
        {
            gameOver = CheckGameOver(out Winner);
        }

        public void Render()
        {
            Level.LevelBoard.Draw();
            foreach (var character in Characters)
                if(character.Alive)
                    character.Draw();
        }

        public void Unload() 
        {
            AssetManager.UnloadAll();
            ShaderManager.Unload();
        }

        public List<IGameEvent> ExecuteAndResolve(IGameCommand command)
        {
            if(!command.Validate(this)) return null;
            Pending.Clear();
            command.Resolve(this);
            return new List<IGameEvent>(Pending);
        }

        internal void RecordAndApply(IGameEvent evt)
        {
            evt.Apply(this);
            Pending.Add(evt);
        }

        public void ApplyEventLog(List<IGameEvent> events)
        {
            foreach (var evt in events)
                evt.Apply(this);
        }

        private void LoadStartingCharacters(GameInitializer lI)
        {
            foreach (var chara in lI.BlueTeam)
            {
                Character character = new Character();
                character.Name = chara.Name;
                character.ID = chara.ID;
                character.Model = new ModelInstance(AssetManager.GetModel(chara.ModelURI));
                character.Faction = Faction.BlueTeam;
                // now we fill the rest using the script URI
                Characters.Add(character);
            }
            foreach (var chara in lI.RedTeam)
            {
                Character character = new Character();
                character.Name = chara.Name;
                character.ID = chara.ID;
                character.Model = new ModelInstance(AssetManager.GetModel(chara.ModelURI));
                character.Faction = Faction.RedTeam;
                // now we fill the rest using the script URI
                Characters.Add(character);
            }

            //LOAD SPELLS AND AURAS as per requirements
            //ASSIGN SPELLS AND AURAS TO CHARACTERS
            SpawnAllCharacters();
        }

        private void SpawnAllCharacters()
        { 
            int blueIndex = 0;
            int redIndex = 0;

            foreach (var character in Characters)
            {
                if (character.Faction == Faction.BlueTeam && blueIndex < Level.LevelBoard.BlueSpawns.Count)
                {
                    Level.LevelBoard.SpawnCharacter(ref CharAlloc, character, Level.LevelBoard.BlueSpawns[blueIndex].Position);
                    blueIndex++;
                }
                else if (character.Faction == Faction.RedTeam && redIndex < Level.LevelBoard.RedSpawns.Count)
                {
                    Level.LevelBoard.SpawnCharacter(ref CharAlloc, character, Level.LevelBoard.RedSpawns[redIndex].Position);
                    redIndex++;
                }
            }
        }

        internal void AdvanceTurnInternal()
        {
            CurrentTurn.End();
            var next = GetNextLivingCharacter(CurrentTurn.ActiveCharacter);
            CurrentTurn = new Turn(CurrentTurn.Number + 1, next, this);
            CurrentTurn.Begin();
        }

        public Character? FindCharacter(int id) => Characters.FirstOrDefault(c => c.GameID == id);

        private Character GetNextLivingCharacter(Character current)
        {
            // ACTUALLY NEED TO IMPLEMENT HASTE SYSTEM
            int startIndex = Characters.IndexOf(current);
            for (int i = 1; i <= Characters.Count; i++)
            {
                var candidate = Characters[(startIndex + i) % Characters.Count];
                if (candidate.Alive)
                    return candidate;
            }
            return current;
        }

        private bool CheckGameOver(out Faction winner)
        {
            winner = Faction.Neutral;

            if (Characters.Count < 1)
                return true;

            bool blueFound = false;
            bool redFound = false;

            foreach (var character in Characters)
            {
                if (!character.Alive)
                    continue;

                if (character.Faction == Faction.BlueTeam)
                    blueFound = true;
                if (character.Faction == Faction.RedTeam)
                    redFound = true;

                if (blueFound && redFound)
                    return false;
            }

            if (blueFound && !redFound)
            {
                winner = Faction.BlueTeam;
                return true;
            }
            else if (!blueFound && redFound)
            {
                winner = Faction.RedTeam;
                return true;
            }

            return false;
        }
    }
}