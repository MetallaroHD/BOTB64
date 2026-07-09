using RL = Raylib_cs;
using System.Numerics;

namespace BOTB64.Graphics.UI
{
    public class GameOverlayScreen : UIScreen
    {
        public LogArea Log = new LogArea { Bounds = new RL.Rectangle(980, 560, 300, 160) };
        public CharacterStatusFrame PlayerStatus = new CharacterStatusFrame();
        public CharacterStatusFrame TargetStatus = new CharacterStatusFrame();
        public IconTextButton MoveButton = new IconTextButton { Bounds = new RL.Rectangle(363, 628, 64, 64), TopRightText = "M" };
        public IconTextButton AttackButton = new IconTextButton { Bounds = new RL.Rectangle(433, 628, 64, 64), TopRightText = "K" };
        public IconTextButton Spell1Button = new IconTextButton { Bounds = new RL.Rectangle(503, 628, 64, 64), TopRightText = "1" };
        public IconTextButton Spell2Button = new IconTextButton { Bounds = new RL.Rectangle(573, 628, 64, 64), TopRightText = "2" };
        public IconTextButton Spell3Button = new IconTextButton { Bounds = new RL.Rectangle(643, 628, 64, 64), TopRightText = "3" };
        public IconTextButton Spell4Button = new IconTextButton { Bounds = new RL.Rectangle(713, 628, 64, 64), TopRightText = "4" };
        public IconTextButton Spell5Button = new IconTextButton { Bounds = new RL.Rectangle(783, 628, 64, 64), TopRightText = "5" };
        public IconTextButton TurnButton = new IconTextButton { Bounds = new RL.Rectangle(853, 628, 64, 64), TopRightText = "Space" };
        public GameOverlayScreen()
        {
            PlayerStatus.SetLayout(new Vector2(50, 600), 200, 58);
            TargetStatus.SetLayout(new Vector2(1030, 24), 200, 58);
            AddElement(Log);
            AddElement(PlayerStatus);
            AddElement(TargetStatus);
            AddElement(MoveButton);
            AddElement(AttackButton);
            AddElement(Spell1Button);
            AddElement(Spell2Button);
            AddElement(Spell3Button);
            AddElement(Spell4Button);
            AddElement(Spell5Button);
            AddElement(TurnButton);
        }
    }
}
