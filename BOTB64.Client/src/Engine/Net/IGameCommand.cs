using BOTB64.Entities;
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
            game.RecordAndApply(new ActionSpentEvent { CharacterID = ActingCharacterID, FastAction = false });

            var ctx = new SpellCastContext(attacker, [target.Position]);
            var runner = game.GetLuaRunner();
            runner.Run(attacker.AutoAttackEffect, ctx);

            AuraTriggerManager.Execute(ctx, EffectTrigger.OnAutoAttack, AuraType.Character | AuraType.Tile);
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