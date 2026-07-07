using BOTB64.Entities;
using BOTB64.Graphics.Animations;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
using MessagePack;

namespace BOTB64.Engine.Net
{
    [Union(0, typeof(DamageEvent))]
    [Union(1, typeof(DeathEvent))]
    [Union(2, typeof(MoveEvent))]
    [Union(3, typeof(TurnAdvancedEvent))]
    [Union(4, typeof(CharacterReassignedEvent))]
    [Union(5, typeof(TeamEliminatedEvent))]
    [Union(6, typeof(ActionSpentEvent))]
    public interface IGameEvent
    {
        void Apply(Game game);
    }

    [MessagePackObject]
    public struct CharacterReassignedEvent : IGameEvent
    {
        [Key(0)] public int CharacterGameID;
        [Key(1)] public int NewOwnerID;
        public void Apply(Game game)
        {
            var character = game.FindCharacter(CharacterGameID);
            if (character != null) character.OwnerID = NewOwnerID;
        }
    }

    [MessagePackObject]
    public struct TeamEliminatedEvent : IGameEvent
    {
        [Key(0)] public Faction Winner;
        public void Apply(Game game) => game.ForceGameOver(Winner);
    }

    [MessagePackObject]
    public struct TurnAdvancedEvent : IGameEvent
    {
        [Key(0)] public int NextCharacterID;
        [Key(1)] public int TurnNumber;

        public void Apply(Game game) => game.ApplyTurnAdvance(NextCharacterID, TurnNumber);
    }

    [MessagePackObject]
    public struct DamageEvent : IGameEvent 
    {
        [Key(0)] public int TargetID;
        [Key(1)] public int Amount;
        [Key(2)] public bool Crit;
        public void Apply(Game game)
        {
            var target = game.FindCharacter(TargetID);
            if (target != null)
            {
                target.CurrentHP -= Amount;
                FloatingTextManager.Add(Amount.ToString(), HexAlgo.HexToWorld(target.Position));
                Logger.Log(target.Name + " receives " + Amount + " damage." + (Crit ? " A critical hit!" : ""));
            }
        }
    }

    [MessagePackObject]
    public struct DeathEvent : IGameEvent
    {
        [Key(0)] public int CharacterID;
        public void Apply(Game game) => game.FindCharacter(CharacterID)?.Die();
    }

    [MessagePackObject]
    public struct MoveEvent : IGameEvent
    {
        [Key(0)] public int CharacterID;
        [Key(1)] public Hex Step; // single destination tile, not a path

        public void Apply(Game game)
        {
            var character = game.FindCharacter(CharacterID);
            if (character == null) return;

            var tile = game.GetBoard().GetTile(Step);
            if (tile == null) return;

            character.RemainMovement -= 1;
            var anim = new CharacterMoveAnimation(character, new List<Hex> { character.Position, tile.AxialPosition });
            game.GetBoard().MoveCharacter(character, new List<Hex> { character.Position, tile.AxialPosition });
            AnimationManager.Play(anim);
        }
    }

    [MessagePackObject]
    public struct ActionSpentEvent : IGameEvent
    {
        [Key(0)] public int CharacterID;
        [Key(1)] public bool FastAction;
        public void Apply(Game game)
        {
            var c = game.FindCharacter(CharacterID);
            if (c == null) return;
            if (FastAction) c.RemainFastAction--; else c.RemainAction--;
        }
    }
}
