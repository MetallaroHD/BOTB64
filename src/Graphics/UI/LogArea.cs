using System;
using System.Collections.Generic;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;
using BOTB64.Runtime;

namespace BOTB64.Graphics.UI
{
    public class LogArea : UIElement
    {
        public RL.Rectangle Bounds;

        public RL.Color BackgroundColor = new RL.Color(30, 30, 30, 200);
        public RL.Color TextColor = RL.Color.White;

        public int FontSize = 18;
        public int LineSpacing = 4;

        public int MaxLines = 200;

        private readonly List<string> _lines = new();

        // 0 = bottom (latest logs), higher = scrolling up
        private int _scroll = 0;

        private int LineHeight => FontSize + LineSpacing;

        public void Append(string text)
        {
            bool wasAtBottom = IsAtBottom();

            var wrapped = WrapText(text);

            foreach (var line in wrapped)
                _lines.Add(line);

            while (_lines.Count > MaxLines)
                _lines.RemoveAt(0);

            if (wasAtBottom)
                _scroll = 0; // stay pinned to bottom
            else
                ClampScroll();
        }

        public void Update()
        {
            int pageSize = Math.Max(1, (int)(Bounds.Height / LineHeight));

            int scrollDelta = 0;

            if (InputManager.IsKeyPressed(RL.KeyboardKey.PageUp))
                scrollDelta += pageSize;

            if (InputManager.IsKeyPressed(RL.KeyboardKey.PageDown))
                scrollDelta -= pageSize;

            if (scrollDelta != 0)
            {
                _scroll += scrollDelta;
                ClampScroll();
            }
        }

        public override void Draw()
        {
            RB.DrawRectangleRec(Bounds, BackgroundColor);

            RB.BeginScissorMode(
                (int)Bounds.X,
                (int)Bounds.Y,
                (int)Bounds.Width,
                (int)Bounds.Height
            );

            int visibleLines = (int)(Bounds.Height / LineHeight);

            int startIndex = _lines.Count - visibleLines - _scroll;
            startIndex = Math.Max(0, startIndex);

            float y = Bounds.Y + 4;

            for (int i = startIndex; i < _lines.Count; i++)
            {
                RB.DrawText(
                    _lines[i],
                    (int)Bounds.X + 4,
                    (int)y,
                    FontSize,
                    TextColor
                );

                y += LineHeight;

                if (y > Bounds.Y + Bounds.Height)
                    break;
            }

            RB.EndScissorMode();
        }

        private bool IsAtBottom()
        {
            return _scroll == 0;
        }

        private void ClampScroll()
        {
            int maxScroll = GetMaxScroll();
            _scroll = Math.Clamp(_scroll, 0, maxScroll);
        }

        private int GetMaxScroll()
        {
            int visibleLines = (int)(Bounds.Height / LineHeight);
            return Math.Max(0, _lines.Count - visibleLines);
        }

        private List<string> WrapText(string text)
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(text))
            {
                result.Add("");
                return result;
            }

            string currentLine = "";

            foreach (char c in text)
            {
                string testLine = currentLine + c;

                int width = RB.MeasureText(testLine, FontSize);

                if (width > Bounds.Width - 8)
                {
                    result.Add(currentLine);
                    currentLine = c.ToString();
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                result.Add(currentLine);

            return result;
        }
    }
}