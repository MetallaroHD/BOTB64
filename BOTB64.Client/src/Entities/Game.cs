using BOTB64.Engine;
using BOTB64.Engine.Net;
using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using BOTB64.Shared.DTOs;
using BOTB64.Shared.Files;
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
        private LuaEffectRunner LuaRunner;

        public LuaEffectRunner GetLuaRunner() => LuaRunner;

        public Faction Winner = Faction.Neutral;
        private bool ForcedGameOver = false;
        private Faction ForcedWinner = Faction.Neutral;

        public int RoundNumber = 0;
        private List<int> TurnOrder = new();
        private int TurnIndex = -1;
        public Turn CurrentTurn;
        public Character CurrentCharacter => CurrentTurn.ActiveCharacter;

        public Board GetBoard() => Level.LevelBoard;

        public void Initialize(GameInitializer lI)
        {
            LuaRunner = new LuaEffectRunner(this);
            (string script, string model, string wall, string env, string shaderv, string shaderf) = CommonURIs.GetLevelResources(lI.Level);
            ShaderManager.LoadWorld(shaderv, shaderf);
            Level = Level.Load(script, model, wall, env);
            LoadStartingCharacters(lI);
            if (Characters.Count < 1)
                throw new Exception("Must pick at least one character.");
            CurrentTurn = new Turn(0, Characters[0], this);
            AdvanceTurnInternal();
        }

        public void Update(float dt, out bool gameOver)
        {
            if (ForcedGameOver) { gameOver = true; Winner = ForcedWinner; return; }
            gameOver = CheckGameOver(out Winner);
        }

        public void Render(Faction? viewerFaction = null)
        {
            Level.LevelBoard.Draw(viewerFaction);
            foreach (var character in Characters)
                if(character.Alive)
                    character.Draw();
        }

        public void Unload() 
        {
            ShaderManager.Unload();
            LuaRunner.End();
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
                Character character = LoadCharacter(chara);
                character.Faction = Faction.BlueTeam;
                character.OwnerID = i < lI.BlueOwners.Count ? lI.BlueOwners[i] : -1;
                Characters.Add(character);
            }
            for (int i = 0; i < lI.RedTeam.Count; i++)
            {
                var chara = lI.RedTeam[i];
                Character character = LoadCharacter(chara);
                character.Faction = Faction.RedTeam;
                character.OwnerID = i < lI.RedOwners.Count ? lI.RedOwners[i] : -1;
                Characters.Add(character);
            }

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
                    Spawn(character, Level.LevelBoard.BlueSpawns[blueIndex].Position, new Hex(0, -1));
                    blueIndex++;
                }
                else if (character.Faction == Faction.RedTeam && redIndex < Level.LevelBoard.RedSpawns.Count)
                {
                    Spawn(character, Level.LevelBoard.RedSpawns[redIndex].Position, new Hex(0, 1));
                    redIndex++;
                }
            }
        }

        private Character LoadCharacter(CharacterDTO dto)
        {
            (string script, string model, string icon) = CommonURIs.GetCharacterResources(dto);

            var reader = new CharacterDataFile();
            DataFile df = new DataFile(script);
            Character character = reader.Read(df);
            character.Name = dto.Name;
            character.ID = dto.ID;
            character.Model = new ModelInstance(ResourceManager.GetModel(model, ModelPurpose.Game));
            return character;
        }

        private void Spawn(Character chara, Hex pos, Hex dir)
        {
            chara.CurrentHP = chara.MaxHP.GetI();
            chara.CurrentResource = chara.StartRes;
            StartLoadout(chara);
            Level.LevelBoard.SpawnCharacter(ref CharAlloc, chara, pos, dir);
        }

        internal void AdvanceTurnInternal()
        {
            if (CurrentCharacter.Alive)
                AuraTriggerManager.Execute(new EffectContext(CurrentCharacter), EffectTrigger.OnEndTurn, AuraType.Character | AuraType.Tile);

            var next = GetNextInTurnOrder();
            if (next == null)
            {
                BeginRound();
                next = GetNextInTurnOrder();
                if (next == null) return; // no living characters at all — CheckGameOver will catch this
            }

            RecordAndApply(new TurnAdvancedEvent { NextCharacterID = next.GameID, TurnNumber = CurrentTurn.Number + 1 });

            if (!next.Alive) return;

            RecordAndApply(new ActionRefreshEvent { CharacterID = next.GameID, Movement = next.Speed.GetI(), Action = 1, FastAction = 1 });
            RecordAndApply(new RegenTickEvent { CharacterID = next.GameID, HPAmount = next.HPRegen.GetI(), ResourceAmount = next.ResRegen.GetI() });

            if (CurrentCharacter.Alive)
                AuraTriggerManager.Execute(new EffectContext(CurrentCharacter), EffectTrigger.OnStartTurn, AuraType.Character | AuraType.Tile);

            foreach (Spell s in next.ActiveSpells.Values)
                if (s.CurrentCD > 0)
                    RecordAndApply(new SpellCooldownReduceEvent { CharacterID = next.GameID, SpellID = s.ID, NewRemaining = Math.Max(0, s.CurrentCD - 1) });

            // Duration < 0 is the permanent-aura convention (mirrors ProcessWorldTick's tile effects).
            foreach (Aura a in next.CurrentAuras.ToList())
            {
                if (a.Duration < 0)
                    continue;

                int newRemaining = Math.Max(0, a.Remaining - 1);
                if (newRemaining <= 0)
                {
                    // Mirrors EffectProcessor.DropAura: run OnDrop while the aura is still
                    // present so its own effects can react, then remove it.
                    AuraTriggerManager.Execute(new ApplyAuraContext(a.Wearer, a.Owner, a.Wearer, a), EffectTrigger.OnDrop, AuraType.Character);
                    RecordAndApply(new AuraExpiredEvent { CharacterID = next.GameID, AuraID = a.ID });
                }
                else
                {
                    RecordAndApply(new AuraDurationTickEvent { CharacterID = next.GameID, AuraID = a.ID, NewRemaining = newRemaining });
                }
            }
        }

        private void BeginRound()
        {
            RoundNumber++;
            var order = Characters.Where(c => c.Alive)
                .OrderByDescending(c => c.Haste.GetF())
                .ThenBy(c => c.GameID)
                .Select(c => c.GameID)
                .ToList();

            RecordAndApply(new RoundStartedEvent { RoundNumber = RoundNumber, TurnOrder = order });
            ProcessWorldTick();
        }

        public void ApplyRoundStart(int roundNumber, List<int> turnOrder)
        {
            RoundNumber = roundNumber;
            TurnOrder = turnOrder;
            TurnIndex = -1;
        }

        private Character? GetNextInTurnOrder()
        {
            while (TurnIndex + 1 < TurnOrder.Count)
            {
                TurnIndex++;
                var c = FindCharacter(TurnOrder[TurnIndex]);
                if (c != null && c.Alive) return c;
            }
            return null; // exhausted this round
        }

        private void ProcessWorldTick()
        {
            foreach (var row in GetBoard().Tiles)
            {
                foreach (var tile in row)
                {
                    // iterate a snapshot — effects may expire/remove themselves mid-loop
                    foreach (var effect in tile.Effects.ToList())
                    {
                        if (effect.Duration >= 0) // convention: Duration < 0 = permanent, never ticks
                        {
                            int newRemaining = Math.Max(0, effect.Remaining - 1);
                            if (newRemaining <= 0)
                                RecordAndApply(new TileEffectExpiredEvent { Position = tile.AxialPosition, TileEffectID = effect.ID });
                            else
                                RecordAndApply(new TileEffectDurationTickEvent { Position = tile.AxialPosition, TileEffectID = effect.ID, NewRemaining = newRemaining });
                        }

                        var ctx = new TileEffectContext(effect.Owner ?? CurrentCharacter, tile.AxialPosition);
                        effect.Execute(this, ctx, EffectTrigger.OnRoundStart);
                    }
                }
            }
        }

        public void ApplyTurnAdvance(int nextCharacterId, int turnNumber)
        {
            var next = FindCharacter(nextCharacterId);
            CurrentTurn = new Turn(turnNumber, next, this);
            Logger.Log("Turn " + CurrentTurn.Number + " - " + next.Name);
        }

        public Character? FindCharacter(int id) => Characters.FirstOrDefault(c => c.GameID == id);
        public Character? FindCharacter(int q, int r) => Characters.FirstOrDefault(c => ((c.Position.Q == q) && (c.Position.R == r)));
        public List<Character> GetCharactersOwnedBy(int playerId) => Characters.Where(c => c.OwnerID == playerId).ToList();
        public List<int> GetAllCharacterIDs() => Characters.Select(c => c.GameID).ToList();
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

        public double Random()
        {
            return System.Random.Shared.NextDouble();
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

        private void StartLoadout(Character character)
        {
            foreach ((int key, int val) in character.SpellLoadout)
            {
                Spell sp = AuraTriggerManager.GetSpell(val);
                
                character.ActiveSpells.Add(key, sp);
            }
            foreach (int id in character.PermanentAuras)
            {
                Aura aura = AuraTriggerManager.GetAura(id);
                aura.Owner = character;
                aura.Wearer = character;
                aura.Remaining = aura.Duration;
                character.CurrentAuras.Add(aura);
            }
        }
    }
}