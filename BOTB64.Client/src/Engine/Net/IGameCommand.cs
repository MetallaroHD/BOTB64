using BOTB64.Entities;
using BOTB64.Entities.Effects;
using BOTB64.Runtime;
namespace BOTB64.Engine.Net
{
    public interface IGameCommand
    {
        int ActingCharacterID { get; }
        bool Validate(Game game);
        void Resolve(Game game);
    }

    public struct MoveCommand : IGameCommand
    {
        public int ActingCharacterID { get; set; }
        public List<Tile> Path { get; set; }

        public bool Validate(Game game)
        {
            if (game.CurrentCharacter.GameID != ActingCharacterID)
                return false;
            var character = game.FindCharacter(ActingCharacterID);
            return character != null && character.Alive; // TODO: validate path is reachable/legal
        }

        public void Resolve(Game game)
        {
            game.RecordAndApply(new MoveEvent
            {
                CharacterID = ActingCharacterID,
                Path = Path
            });
        }
    }

    public struct AutoAttackCommand : IGameCommand
    {
        public int ActingCharacterID { get; set; }
        public int TargetID { get; set; }

        public bool Validate(Game game)
        {
            if (game.CurrentCharacter.GameID != ActingCharacterID)
                return false;
            var attacker = game.FindCharacter(ActingCharacterID);
            var target = game.FindCharacter(TargetID);
            return attacker != null && attacker.Alive && target != null && target.Alive;
        }

        public void Resolve(Game game)
        {
            var attacker = game.FindCharacter(ActingCharacterID);
            var target = game.FindCharacter(TargetID);

            DoDamageEffect eff = new DoDamageEffect();
            DamageContext ctx = new DamageContext(attacker, attacker, target)
            {
                DamageType = attacker.AutoAttackDamageType,
                SourceType = DamageSourceType.AutoAttack,
            };
            eff.Execute(game, ctx);

            AuraTriggerManager.Execute(ctx, EffectTrigger.OnDamageDone, AuraType.Character | AuraType.Tile);
            if(target.Alive)
                AuraTriggerManager.Execute(ctx, EffectTrigger.OnDamageTaken, AuraType.Character | AuraType.Tile);
            if (ctx.Crit)
                AuraTriggerManager.Execute(ctx, EffectTrigger.OnCrit, AuraType.Character | AuraType.Tile);
        }
    }

    public struct EndTurnCommand : IGameCommand
    {
        public int ActingCharacterID { get; set; }
        public bool Validate(Game game) => game.CurrentCharacter.GameID == ActingCharacterID;
        public void Resolve(Game game) => game.AdvanceTurnInternal();
    }
}