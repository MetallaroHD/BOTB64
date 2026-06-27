using BOTB64.Entities.DTOs;
using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using BOTB64.Graphics.Animations;
using BOTB64.Entities.Effects;
using BOTB64.Engine;

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
        private Level Level = new Level();
        private List<Character> Characters = new();
        private List<Spell> Spells = new();
        private List<Aura> Auras = new();

        public Turn CurrentTurn;
        public Character CurrentCharacter => CurrentTurn.ActiveCharacter;

        public Board GetBoard() => Level.LevelBoard;

        public void Initialize(GameInitializer lI)
        {
            Level = Level.Load(lI.Level.ScriptURI, lI.Level.ModelURI);
            LoadStartingCharacters(lI);
            CurrentTurn = new Turn(0, Characters[0], this);
        }

        public void Update(float dt)
        {

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
                    Level.LevelBoard.SpawnCharacter(character, Level.LevelBoard.BlueSpawns[blueIndex].Position);
                    blueIndex++;
                }
                else if (character.Faction == Faction.RedTeam && redIndex < Level.LevelBoard.RedSpawns.Count)
                {
                    Level.LevelBoard.SpawnCharacter(character, Level.LevelBoard.RedSpawns[redIndex].Position);
                    redIndex++;
                }
            }
        }

        private void MoveCharacter(Character character, List<Tile> path)
        {
            var anim = new CharacterMoveAnimation(character, path);
            Level.LevelBoard.MoveCharacter(character, path);
            AnimationManager.Play(anim);
            foreach (var tile in path)
            {
                AuraTriggerManager.Execute(new EffectContext(character), EffectTrigger.OnMove, AuraType.Character | AuraType.Tile);
            }
        }

        public void MoveCurrentCharacter(List<Tile> path)
        {
            MoveCharacter(CurrentCharacter, path);
        }

        public void AutoAttack(Character attacker, Character attacked)
        {
            //animation
            DoDamageEffect eff = new DoDamageEffect();
            DamageContext ctx = new DamageContext(attacker, attacker, attacked)
            {
                DamageType = attacker.AutoAttackDamageType,
                SourceType = DamageSourceType.AutoAttack,
            };
            eff.Execute(ctx);
            AuraTriggerManager.Execute(ctx, EffectTrigger.OnDamageDone, AuraType.Character | AuraType.Tile);
        }
    }
}