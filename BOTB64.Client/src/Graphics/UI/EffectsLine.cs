using System.Collections.Generic;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class EffectsLine : UIElement
    {
        public Vector2 Position;
        public float SquareSize = 24f;
        public float Spacing = 4f;
        private readonly List<EffectSquare> _effects = new();
        private bool _dirty = true;
        public IReadOnlyList<EffectSquare> Effects => _effects;

        public void Sync<T>(IReadOnlyList<T> effects, Func<T, int> getId, Func<T, int> getDuration, Func<T, string> getTooltip, Func<int, RL.Texture2D> iconLookup)
        {
            while (_effects.Count > effects.Count)
            {
                _effects.RemoveAt(_effects.Count - 1);
                _dirty = true;
            }
            while (_effects.Count < effects.Count)
            {
                _effects.Add(new EffectSquare { Bounds = new RL.Rectangle(0, 0, SquareSize, SquareSize) });
                _dirty = true;
            }

            for (int i = 0; i < effects.Count; i++)
            {
                var e = effects[i];
                int id = getId(e);
                _effects[i].SetIcon(iconLookup(id));
                _effects[i].SetTooltip(getTooltip(e));
                _effects[i].Duration = getDuration(e);
            }
        }

        public void Clear()
        {
            if (_effects.Count == 0) return;
            _effects.Clear();
            _dirty = true;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
            _dirty = true;
        }

        public void MarkDirty() => _dirty = true;

        private void Layout()
        {
            float x = Position.X;
            foreach (var effect in _effects)
            {
                RL.Rectangle b = effect.Bounds;
                b.X = x;
                b.Y = Position.Y;
                b.Width = SquareSize;
                b.Height = SquareSize;
                effect.Bounds = b;
                x += SquareSize + Spacing;
            }
            _dirty = false;
        }

        public override void Draw()
        {
            if (!Visible) return;
            if (_dirty)
                Layout();
            foreach (var effect in _effects)
                effect.Draw();
        }
    }
}