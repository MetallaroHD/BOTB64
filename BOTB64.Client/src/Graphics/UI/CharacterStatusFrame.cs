using System.Numerics;
using System.Security;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class CharacterStatusFrame : UIElement
    {
        public readonly Label Name = new Label();
        public readonly CharacterResourceBars Bars = new();
        public readonly EffectsLine Effects = new();

        public int GapBetween = 10;

        public void SetLayout(Vector2 position, float width, float barsHeight)
        {
            Name.Position = new Vector2(position.X, position.Y - 16);
            Name.Text = "Test";
            Name.FontSize = 16;

            Bars.Bounds = new RL.Rectangle(position.X, position.Y, width, barsHeight);
            Effects.SetPosition(new Vector2(position.X, position.Y + barsHeight + GapBetween));
        }

        public override void Draw()
        {
            if (!Visible) return;

            Name.Draw();
            Bars.Draw();
            Effects.Draw();
        }

        public void SetName(string name)
        {
            Name.Text = name;
        }
        public void SetHealth(int current, int max)
        {
            Bars.Health = (float)current / max;
            Bars.HP.Text = current.ToString();
        }
        public void SetResource(int current, int max)
        {
            Bars.Resource = (float)current / max;
            Bars.Res.Text = current.ToString();
        }
        public void SetResourceColor(RL.Color color)
        {
            Bars.ResourceColor = color;
        }
    }
}