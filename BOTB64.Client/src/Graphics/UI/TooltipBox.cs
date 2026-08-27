using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public readonly struct EffectDisplayInfo
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Tooltip;
        public readonly int Stacks;
        public readonly int Remaining;
        public readonly RL.Texture2D Icon;

        public EffectDisplayInfo(int id, string name, string tooltip, int stacks, int remaining, RL.Texture2D icon)
        {
            Id = id; Name = name; Tooltip = tooltip; Stacks = stacks; Remaining = remaining; Icon = icon;
        }
    }

    public class TooltipBox : UIElement
    {
        public int FontSize = 16;
        public int Padding = 6;
        public int LineSpacing = 4;
        public float MaxWidth = 300f;
        public RL.Color BackgroundColor = new RL.Color(0, 0, 0, 210);
        public RL.Color TextColor = RL.Color.White;

        private RL.Rectangle _bounds;
        public RL.Rectangle Bounds => _bounds;

        private List<string> _paragraphs = new();
        private List<string> _wrappedLines = new();

        // Each entry in `paragraphs` is wrapped independently and may become multiple lines.
        // Pass an empty string to insert a blank spacer line between sections.
        public void SetContent(List<string> paragraphs)
        {
            _paragraphs = paragraphs ?? new List<string>();
            RecalculateSize();
        }

        public void SetText(string text) =>
            SetContent(string.IsNullOrEmpty(text) ? new List<string>() : new List<string> { text });

        public void PositionAbove(RL.Rectangle target, float gap = 4f)
        {
            float x = target.X + (target.Width - _bounds.Width) * 0.5f;
            float y = target.Y - _bounds.Height - gap;
            _bounds.X = x;
            _bounds.Y = y;
            ClampToScreen();
        }

        public void SetPosition(Vector2 position)
        {
            _bounds.X = position.X;
            _bounds.Y = position.Y;
            ClampToScreen();
        }

        private void ClampToScreen()
        {
            const int screenW = 1280;
            const int screenH = 720;

            if(_bounds.X < 0)
                _bounds.X = 0;
            if (_bounds.Y < 0)
                _bounds.Y = 0;
            if(_bounds.X + _bounds.Width > screenW)
                _bounds.X = screenW - _bounds.Width;
            if(_bounds.Y + _bounds.Height > screenH)
                _bounds.Y += screenH - _bounds.Height;
        }

        private void RecalculateSize()
        {
            _wrappedLines.Clear();
            float innerWidth = MaxWidth - Padding * 2;

            foreach (var paragraph in _paragraphs)
            {
                if (string.IsNullOrEmpty(paragraph))
                {
                    _wrappedLines.Add("");
                    continue;
                }
                _wrappedLines.AddRange(WrapText(paragraph, innerWidth));
            }

            float widest = 0f;
            foreach (var line in _wrappedLines)
                widest = MathF.Max(widest, RB.MeasureText(line, FontSize));

            _bounds.Width = widest + Padding * 2;
            _bounds.Height = _wrappedLines.Count * (FontSize + LineSpacing) - LineSpacing + Padding * 2;
        }

        private List<string> WrapText(string text, float maxWidth)
        {
            var result = new List<string>();
            var words = text.Split(' ');
            string current = "";

            foreach (var word in words)
            {
                string candidate = current.Length == 0 ? word : current + " " + word;
                if (RB.MeasureText(candidate, FontSize) > maxWidth && current.Length > 0)
                {
                    result.Add(current);
                    current = word;
                }
                else
                {
                    current = candidate;
                }
            }
            if (current.Length > 0 || result.Count == 0)
                result.Add(current);

            return result;
        }

        public override void Draw()
        {
            if (!Visible || _wrappedLines.Count == 0) return;

            RB.DrawRectangleRec(_bounds, BackgroundColor);
            float y = _bounds.Y + Padding;
            foreach (var line in _wrappedLines)
            {
                RB.DrawText(line, (int)(_bounds.X + Padding), (int)y, FontSize, TextColor);
                y += FontSize + LineSpacing;
            }
        }
    }
}