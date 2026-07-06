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
    public interface IGameEvent
    {
        void Apply(Game game);
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
        [Key(1)] public List<Hex> Path;
        public void Apply(Game game)
        {
            if (Path.Count < 2)
                return;
            var character = game.FindCharacter(CharacterID);
            if (character == null) 
                return;
            var anim = new CharacterMoveAnimation(character, Path);
            character.RemainMovement -= Path.Count - 1;
            game.GetBoard().MoveCharacter(character, Path);
            AnimationManager.Play(anim);
        }
    }
}
