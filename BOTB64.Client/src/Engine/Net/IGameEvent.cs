using BOTB64.Entities;
using BOTB64.Graphics.UI;
using BOTB64.Graphics.Animations;
using BOTB64.Runtime;

namespace BOTB64.Engine.Net
{
    public interface IGameEvent
    {
        void Apply(Game game);
    }

    public struct DamageEvent : IGameEvent 
    {
        public int TargetID;
        public int Amount;
        public bool Crit;
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

    public struct DeathEvent : IGameEvent
    {
        public int CharacterID;
        public void Apply(Game game) => game.FindCharacter(CharacterID)?.Die();
    }

    public struct MoveEvent : IGameEvent 
    {
        public int CharacterID;
        public List<Tile> Path;
        public void Apply(Game game)
        {
            var character = game.FindCharacter(CharacterID);
            if (character == null) return;
            var anim = new CharacterMoveAnimation(character, Path);
            game.GetBoard().MoveCharacter(character, Path);
            AnimationManager.Play(anim);
        }
    }
}
