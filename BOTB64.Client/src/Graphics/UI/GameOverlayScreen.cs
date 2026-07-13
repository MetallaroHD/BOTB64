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
        public Label EndTurnQuestion = new Label { Visible = false, Position = new Vector2(540, 290), Text = "End Turn?", FontSize = 36 };
        public TextButton YesButton = new TextButton { Visible = false, Bounds = new RL.Rectangle(540, 365, 80, 50), Text = "Yes" };
        public TextButton NoButton = new TextButton { Visible = false, Bounds = new RL.Rectangle(660, 365, 80, 50), Text = "No" };

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
            AddElement(EndTurnQuestion);
            AddElement(YesButton);
            AddElement(NoButton);
        }

        public void TogglePause(bool yes)
        {
            Log.Visible = !yes;
            PlayerStatus.Visible = !yes;
            TargetStatus.Visible = !yes;
            MoveButton.Visible = !yes;
            AttackButton.Visible = !yes;
            Spell1Button.Visible = !yes;
            Spell2Button.Visible = !yes;
            Spell3Button.Visible = !yes;
            Spell4Button.Visible = !yes;
            Spell5Button.Visible = !yes;
            TurnButton.Visible = !yes;
            ExitButton.Visible = yes;
            ResumeButton.Visible = yes;
        }

        public void Pause()
        {
            TogglePause(true);
        }

        public void UnPause()
        {
            TogglePause(false);
        }

        public void AskEndTurn(bool yes)
        {
            Log.Visible = !yes;
            PlayerStatus.Visible = !yes;
            TargetStatus.Visible = !yes;
            MoveButton.Visible = !yes;
            AttackButton.Visible = !yes;
            Spell1Button.Visible = !yes;
            Spell2Button.Visible = !yes;
            Spell3Button.Visible = !yes;
            Spell4Button.Visible = !yes;
            Spell5Button.Visible = !yes;
            TurnButton.Visible = !yes;
            EndTurnQuestion.Visible = yes;
            YesButton.Visible = yes;
            NoButton.Visible = yes;
        }

        public void ShowEndTurn()
        {
            AskEndTurn(true);
        }

        public void HideEndTurn()
        { 
            AskEndTurn(false); 
        }
    }
}
