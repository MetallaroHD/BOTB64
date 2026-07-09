using BOTB64.Engine;
using BOTB64.Engine.Net;
using BOTB64.Entities.DTOs;
using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using MessagePack;

namespace BOTB64.Entities
{
    [MessagePackObject]
    public struct GameInitializer
    {
        [Key(0)] public LevelDTO Level;
        [Key(1)] public List<CharacterDTO> BlueTeam;
        [Key(2)] public List<CharacterDTO> RedTeam;
        [Key(3)] public List<int> BlueOwners;
        [Key(4)] public List<int> RedOwners;
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
        private bool ForcedGameOver = false;
        private Faction ForcedWinner = Faction.Neutral;

        public Turn CurrentTurn;
        public Character CurrentCharacter => CurrentTurn.ActiveCharacter;

        public Board GetBoard() => Level.LevelBoard;

        public void Initialize(GameInitializer lI)
        {
            (string script, string model, string wall, string shaderv, string shaderf) = CommonURIs.GetLevelResources(lI.Level);
            ShaderManager.LoadWorld(shaderv, shaderf);
            Level = Level.Load(script, model, wall);
            LoadStartingCharacters(lI);
            if (Characters.Count < 1)
                throw new Exception("Must pick at least one character.");
            CurrentTurn = new Turn(1, Characters[0], this);
            Logger.Log("Turn " + CurrentTurn.Number + " - " + Characters[0].Name);
        }

        public void Update(float dt, out bool gameOver)
        {
            if (ForcedGameOver) { gameOver = true; Winner = ForcedWinner; return; }
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
            for (int i = 0; i < lI.BlueTeam.Count; i++)
            {
                var chara = lI.BlueTeam[i];
                (string script, string model, string icon) = CommonURIs.GetCharacterResources(chara);

                Character character = new Character();
                character.Name = chara.Name;
                character.ID = chara.ID;
                character.Model = new ModelInstance(AssetManager.GetModel(model, ModelPurpose.Game));
                character.Faction = Faction.BlueTeam;
                // now we fill the rest using the script URI
                character.OwnerID = i < lI.BlueOwners.Count ? lI.BlueOwners[i] : -1;
                Characters.Add(character);
            }
            for (int i = 0; i < lI.RedTeam.Count; i++)
            {
                var chara = lI.RedTeam[i];
                (string script, string model, string icon) = CommonURIs.GetCharacterResources(chara);

                Character character = new Character();
                character.Name = chara.Name;
                character.ID = chara.ID;
                character.Model = new ModelInstance(AssetManager.GetModel(model, ModelPurpose.Game));
                character.Faction = Faction.RedTeam;
                // now we fill the rest using the script URI
                character.OwnerID = i < lI.RedOwners.Count ? lI.RedOwners[i] : -1;
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
            var next = GetNextLivingCharacter(CurrentTurn.ActiveCharacter);
            RecordAndApply(new TurnAdvancedEvent { NextCharacterID = next.GameID, TurnNumber = CurrentTurn.Number + 1 });
        }

        public void ApplyTurnAdvance(int nextCharacterId, int turnNumber)
        {
            CurrentTurn.End();
            var next = FindCharacter(nextCharacterId);
            CurrentTurn = new Turn(turnNumber, next, this);
            Logger.Log("Turn " + CurrentTurn.Number + " - " + next.Name);
            CurrentTurn.Begin();
        }

        public Character? FindCharacter(int id) => Characters.FirstOrDefault(c => c.GameID == id);
        public List<Character> GetCharactersOwnedBy(int playerId) => Characters.Where(c => c.OwnerID == playerId).ToList();
        public void RecordAndApplyExternal(IGameEvent evt) => RecordAndApply(evt);

        public void ForceGameOver(Faction winner)
        {
            ForcedGameOver = true;
            ForcedWinner = winner;
        }

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