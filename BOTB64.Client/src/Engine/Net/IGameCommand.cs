using BOTB64.Entities;
using BOTB64.Entities.Effects;
using BOTB64.Runtime;
using MessagePack;
namespace BOTB64.Engine.Net
{
    [Union(0, typeof(MoveCommand))]
    [Union(1, typeof(AutoAttackCommand))]
    [Union(2, typeof(EndTurnCommand))]
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

        public bool Validate(Game game)
        {
            if (game.CurrentCharacter.GameID != ActingCharacterID)
                return false;
            var attacker = game.FindCharacter(ActingCharacterID);
            var target = game.FindCharacter(TargetID);
            if (attacker == null) return false;
            if (attacker.RemainAction <= 0) return false;
            return attacker.Alive && target != null && target.Alive;
        }

        public void Resolve(Game game)
        {
            var attacker = game.FindCharacter(ActingCharacterID);
            var target = game.FindCharacter(TargetID);

            game.RecordAndApply(new ActionSpentEvent { CharacterID = ActingCharacterID, FastAction = false });

            DoDamageEffect eff = new DoDamageEffect();
            DamageContext ctx = new DamageContext(attacker, attacker, target)
            {
                DamageType = attacker.AutoAttackDamageType,
                SourceType = DamageSourceType.AutoAttack,
            };
            eff.Execute(game, ctx);

            AuraTriggerManager.Execute(ctx, EffectTrigger.OnDamageDone, AuraType.Character | AuraType.Tile);
            if (target.Alive)
                AuraTriggerManager.Execute(ctx, EffectTrigger.OnDamageTaken, AuraType.Character | AuraType.Tile);
            if (ctx.Crit)
                AuraTriggerManager.Execute(ctx, EffectTrigger.OnCrit, AuraType.Character | AuraType.Tile);
        }
    }

    [MessagePackObject]
    public struct EndTurnCommand : IGameCommand
    {
        [Key(0)] public int ActingCharacterID { get; set; }
        public bool Validate(Game game) => game.CurrentCharacter.GameID == ActingCharacterID;
        public void Resolve(Game game) => game.AdvanceTurnInternal();
    }
}