using BOTB64.Entities;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
using MessagePack;
namespace BOTB64.Engine.Net
{
    [Union(0, typeof(MoveCommand))]
    [Union(1, typeof(AutoAttackCommand))]
    [Union(2, typeof(EndTurnCommand))]
    [Union(3, typeof(SpellCastCommand))]
    public interface IGameCommand
    {
        int ActingCharacterID { get; }
        bool Validate(Game game);
        void Resolve(Game game);
    }

    [MessagePackObject]
    public struct MoveCommand : IGameCommand
    {
        [Key(0)] public int ActingCharacterID { get; set; }
        [Key(1)] public List<Hex> Path { get; set; }

        public bool Validate(Game game)
        {
            if (game.CurrentCharacter.GameID != ActingCharacterID) return false;
            var character = game.FindCharacter(ActingCharacterID);
            return character != null && character.Alive && Path.Count > 1;
        }

        public void Resolve(Game game)
        {
            var character = game.FindCharacter(ActingCharacterID);

            for (int i = 1; i < Path.Count; i++)
            {
                if (!character.Alive) break;

                var stepHex = Path[i];

                game.RecordAndApply(new MoveEvent
                {
                    CharacterID = ActingCharacterID,
                    Step = stepHex
                });

                bool isFirstStepThisTurn = !character.HasMovedThisTurn;
                character.HasMovedThisTurn = true;

                var ctx = new EffectContext(character);
                AuraTriggerManager.Execute(ctx, EffectTrigger.OnMove, AuraType.Character | AuraType.Tile);
                if (isFirstStepThisTurn)
                    AuraTriggerManager.Execute(ctx, EffectTrigger.OnMoveFirstTime, AuraType.Character | AuraType.Tile);
            }
        }
    }

    [MessagePackObject]
    public struct AutoAttackCommand : IGameCommand
    {
        [Key(0)] public int ActingCharacterID { get; set; }
        [Key(1)] public int TargetID { get; set; }

        // Used for caching
        Character? attacker;
        Character? target;

        public bool Validate(Game game)
        {
            if (game.CurrentCharacter.GameID != ActingCharacterID)
                return false;

            attacker = game.FindCharacter(ActingCharacterID);
            target = game.FindCharacter(TargetID);

            if (attacker == null || target == null) 
                return false;
            if (attacker.RemainAction <= 0) 
                return false;
            if (attacker.Faction == target.Faction) 
                return false;
            if (!attacker.Alive || !target.Alive)
                return false;
            return true;
        }

        public void Resolve(Game game)
        {
            var ctx = new SpellCastContext(attacker, [target.Position]);
            var runner = game.GetLuaRunner();
            LuaResult ret = runner.Run(attacker.AutoAttackEffect, ctx);
            if (!ret.Success)
            {
                FloatingMessageManager.AddMessage(ret.ErrorMessage);
                return;
            }
            game.RecordAndApply(new ActionSpentEvent { CharacterID = ActingCharacterID, FastAction = false });
            AuraTriggerManager.Execute(ctx, EffectTrigger.OnAutoAttack, AuraType.Character | AuraType.Tile);
        }
    }

    [MessagePackObject]
    public struct SpellCastCommand : IGameCommand
    {
        [Key(0)] public int ActingCharacterID { get; set; }
        [Key(1)] public int SpellID { get; set; }
        [Key(2)] public List<Hex> ExplicitTarget { get; set; }

        Character? caster;
        Spell? spell;

        public bool Validate(Game game)
        {
            if (game.CurrentCharacter.GameID != ActingCharacterID) return false;

            caster = game.FindCharacter(ActingCharacterID);
            if (caster == null || !caster.Alive) return false;

            int spellId = SpellID;
            spell = caster.ActiveSpells.Values.FirstOrDefault(s => s.ID == spellId);
            if (spell == null) 
                return false;
            if (spell.CurrentCD > 0) 
                return false;
            if (spell.CurrentCharges <= 0) 
                return false;
            if (caster.CurrentResource < spell.Cost) 
                return false;
            if (spell.CastTime == 0 && caster.RemainAction <= 0) 
                return false;
            if (spell.CastTime == -1 && caster.RemainFastAction <= 0) 
                return false;

            return true;
        }

        public void Resolve(Game game)
        {
            var ctx = new SpellCastContext(caster, ExplicitTarget);

            LuaResult ret = spell.Cast(game, ctx);

            if (!ret.Success)
            {
                FloatingMessageManager.AddMessage(ret.ErrorMessage);
                return;
            }

            if (spell.CastTime == 0)
                game.RecordAndApply(new ActionSpentEvent { CharacterID = ActingCharacterID, FastAction = false });
            else if (spell.CastTime == -1)
                game.RecordAndApply(new ActionSpentEvent { CharacterID = ActingCharacterID, FastAction = true });

            game.RecordAndApply(new SpellCastEvent { CharacterID = ActingCharacterID, SpellID = SpellID, CostSpent = spell.Cost });

            AuraTriggerManager.Execute(ctx, EffectTrigger.OnSpellCast, AuraType.Character | AuraType.Tile);
        }
    }

    [MessagePackObject]
    public struct EndTurnCommand : IGameCommand
    {
        [Key(0)] public int ActingCharacterID { get; set; }
        public bool Validate(Game game) => game.CurrentCharacter.GameID == ActingCharacterID;
        public void Resolve(Game game)
        {
            if (game.CurrentCharacter.Alive)
                AuraTriggerManager.Execute(new EffectContext(game.CurrentCharacter), EffectTrigger.OnEndTurn, AuraType.Character | AuraType.Tile);
            game.AdvanceTurnInternal();
            if(game.CurrentCharacter.Alive)
                AuraTriggerManager.Execute(new EffectContext(game.CurrentCharacter), EffectTrigger.OnStartTurn, AuraType.Character | AuraType.Tile);
        }
    }
}