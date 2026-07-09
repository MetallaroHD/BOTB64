using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class CharacterResourceBars : UIElement
    {
        public readonly ProgressBar HealthBar = new();
        public readonly ProgressBar ResourceBar = new();
        public readonly Label HP = new Label();
        public readonly Label Res = new Label();

        public int Gap = 2;

        private RL.Rectangle _bounds;
        public RL.Rectangle Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                Layout();
            }
        }

        public CharacterResourceBars()
        {
            HealthBar.FillColor = RL.Color.Green;
            ResourceBar.FillColor = RL.Color.Blue;
        }

        public RL.Color ResourceColor
        {
            get => ResourceBar.FillColor;
            set => ResourceBar.FillColor = value;
        }

        public float Health
        {
            get => HealthBar.Value;
            set => HealthBar.Value = value;
        }

        public float Resource
        {
            get => ResourceBar.Value;
            set => ResourceBar.Value = value;
        }

        private void Layout()
        {
            float barHeight = (_bounds.Height - Gap) / 2f;

            HealthBar.Bounds = new RL.Rectangle(_bounds.X, _bounds.Y, _bounds.Width, barHeight);
            ResourceBar.Bounds = new RL.Rectangle(_bounds.X, _bounds.Y + barHeight + Gap, _bounds.Width, barHeight);
            HP.Position = new Vector2(_bounds.X + 2, _bounds.Y + barHeight / 4);
            Res.Position = new Vector2(_bounds.X + 2, _bounds.Y + 5 * barHeight / 4 + 2);
            HP.FontSize = 16;
            Res.FontSize = 16;
            HP.Text = "34000";
            Res.Text = "7400";
        }

        public override void Draw()
        {
            if (!Visible) return;

            HealthBar.Draw();
            ResourceBar.Draw();
            HP.Draw();
            Res.Draw();
        }
    }
}