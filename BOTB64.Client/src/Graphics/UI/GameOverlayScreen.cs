using BOTB64.Runtime;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class GameOverlayScreen : UIScreen
    {
        public LogArea Log = new LogArea { Bounds = new RL.Rectangle(980, 560, 300, 160) };
        public CharacterStatusFrame PlayerStatus = new CharacterStatusFrame();
        public CharacterStatusFrame TargetStatus = new CharacterStatusFrame();
        public ActionButton MoveButton = new ActionButton { Bounds = new RL.Rectangle(363, 628, 64, 64), TopRightText = "M" };
        public ActionButton AttackButton = new ActionButton { Bounds = new RL.Rectangle(433, 628, 64, 64), TopRightText = "K" };
        public ActionButton Spell1Button = new ActionButton { Bounds = new RL.Rectangle(503, 628, 64, 64), TopRightText = "1" };
        public ActionButton Spell2Button = new ActionButton { Bounds = new RL.Rectangle(573, 628, 64, 64), TopRightText = "2" };
        public ActionButton Spell3Button = new ActionButton { Bounds = new RL.Rectangle(643, 628, 64, 64), TopRightText = "3" };
        public ActionButton Spell4Button = new ActionButton { Bounds = new RL.Rectangle(713, 628, 64, 64), TopRightText = "4" };
        public ActionButton Spell5Button = new ActionButton { Bounds = new RL.Rectangle(783, 628, 64, 64), TopRightText = "5" };
        public ActionButton TurnButton = new ActionButton { Bounds = new RL.Rectangle(853, 628, 64, 64), TopRightText = "Space" };
        public TextButton ExitButton = new TextButton { Visible = false, Bounds = new RL.Rectangle(540, 365, 200, 60), Text = "Exit Game", OnClick = () => { InputManager.WantsClose = true; } };
        public TextButton ResumeButton = new TextButton { Visible = false, Bounds = new RL.Rectangle(540, 290, 200, 60), Text = "Resume" };

        public GameOverlayScreen()
        {
            PlayerStatus.SetLayout(new Vector2(50, 600), 200, 58);
            TargetStatus.SetLayout(new Vector2(1030, 24), 200, 58);

            MoveButton.Icon = ResourceManager.LoadTexture("Misc\\move.png");
            AttackButton.Icon = ResourceManager.LoadTexture("Misc\\attack.png");
            TurnButton.Icon = ResourceManager.LoadTexture("Misc\\turn.png");

            MoveButton.SetTooltip("Move");
            AttackButton.SetTooltip("Attack");
            TurnButton.SetTooltip("End Turn");

            TargetStatus.Visible = false;

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
            AddElement(ExitButton);
            AddElement(ResumeButton);
        }

        public void Pause()
        {
            Log.Visible = false;
            PlayerStatus.Visible = false;
            TargetStatus.Visible = false;
            MoveButton.Visible = false;
            AttackButton.Visible = false;
            Spell1Button.Visible = false;
            Spell2Button.Visible = false;
            Spell3Button.Visible = false;
            Spell4Button.Visible = false;
            Spell5Button.Visible = false;
            TurnButton.Visible = false;
            ExitButton.Visible = true;
            ResumeButton.Visible = true;
        }

        public void UnPause()
        {
            Log.Visible = true;
            PlayerStatus.Visible = true;
            TargetStatus.Visible = true;
            MoveButton.Visible = true;
            AttackButton.Visible = true;
            Spell1Button.Visible = true;
            Spell2Button.Visible = true;
            Spell3Button.Visible = true;
            Spell4Button.Visible = true;
            Spell5Button.Visible = true;
            TurnButton.Visible = true;
            ExitButton.Visible = false;
            ResumeButton.Visible = false;
        }
    }
}
